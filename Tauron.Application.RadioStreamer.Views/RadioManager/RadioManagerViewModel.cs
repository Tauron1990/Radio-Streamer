#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.Application.RadioStreamer.Contracts.Scripts;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.RadioStreamer.Views.MedadataView;
using Tauron.Application.RadioStreamer.Views.RadioManager.Controls;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.RadioManager
{
    public sealed class RadioManagerRadio : ObservableObject
    {
        private readonly Action<RadioManagerRadio> _delete;
        private RadioEntry _entry;
        private bool _favorite;
        private bool _isSyncEnabled;
        private QualityList _qualitys;
        private IQualityQuery _query;
        private int _selectedIndex = -1;

        public RadioManagerRadio(RadioEntry entry, bool favorite, [NotNull] IQualityQuery qualitys,
            [NotNull] Action<RadioManagerRadio> delete)
        {
            _favorite = favorite;
            _query = qualitys;
            _delete = delete;
            _entry = entry;

            // ReSharper disable PossibleNullReferenceException
            _entry.Metadata.ValueChanged += MetadataOnValueChanged;
            _entry.Metadata.ElementDeleted += MetadataOnElementDeleted;
            _entry.Metadata.QualityChanged += MetadataOnQualityChanged;

            if (entry.Qualitys == null) entry.SetQuality(qualitys);
// ReSharper disable once AssignNullToNotNullAttribute
            _qualitys = new QualityList(entry.Qualitys);
            // ReSharper restore PossibleNullReferenceException
        }

        public RadioEntry Entry
        {
            get { return _entry; }
        }

        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                _favorite = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string RadioTitle
        {
            get { return _entry.Name; }
        }

        [NotNull]
        public string Genres
        {
            get { return _entry.Genre; }
        }

        [NotNull]
        public QualityList Qualitys
        {
            get
            {
                if (_isSyncEnabled) return _qualitys;
                BindingOperations.EnableCollectionSynchronization(_qualitys, _qualitys);
                _isSyncEnabled = true;
                return _qualitys;
            }
            set
            {
                if (_qualitys == value) return;

                _qualitys = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value) return;

                QualitySelected(this, EventArgs.Empty);
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler QualitySelected;

        private void MetadataOnQualityChanged()
        {
            _query.Reset();
            _qualitys.Reset(_query);
        }

        private void MetadataOnElementDeleted()
        {
// ReSharper disable once PossibleNullReferenceException
            _entry.Metadata.ValueChanged -= MetadataOnValueChanged;
            _entry.Metadata.QualityChanged -= MetadataOnQualityChanged;
            _entry.Metadata.ElementDeleted -= MetadataOnElementDeleted;

            _delete(this);
        }

        private void MetadataOnValueChanged([NotNull] object sender,
            [NotNull] PropertyChangingEventArgs propertyChangingEventArgs)
        {
            if (propertyChangingEventArgs.PropertyName != null)
                OnPropertyChangedExplicit(propertyChangingEventArgs.PropertyName);
        }

        [EventTarget]
        public void QualiysOpend()
        {
            _qualitys.EnsureAcess();
            SelectedIndex = 0;
        }

        public RadioQuality GetQuality()
        {
            try
            {
                if (SelectedIndex < 0) return new RadioQuality();

                var q = Qualitys[SelectedIndex] as NormalQuality;
                return q == null ? new RadioQuality() : q.Quality;
            }
            catch (ArgumentOutOfRangeException)
            {
                return new RadioQuality();
            }
        }

        public override string ToString()
        {
            return RadioTitle;
        }

        public interface IQualityEntry
        {
        }

        private class NormalQuality : IQualityEntry
        {
            private RadioQuality _quality;

            public NormalQuality(RadioQuality quality)
            {
                _quality = quality;
            }

            public RadioQuality Quality
            {
                get { return _quality; }
            }

            public override string ToString()
            {
                return _quality.Name;
            }
        }

        private class PlaceHolder : IQualityEntry
        {
            private readonly bool _notReady;

            public PlaceHolder(bool notReady)
            {
                _notReady = notReady;
            }

            public override string ToString()
            {
                return _notReady ? RadioStreamerResources.RadioItemNotReady : RadioStreamerResources.RadioItemWaitLabel;
            }
        }

        public class QualityList : ObservableCollection<IQualityEntry>
        {
            private int _finisht;
            private IQualityQuery _query;

            public QualityList([NotNull] IQualityQuery query)
            {
                Reset(query);
            }

            public void Reset([NotNull] IQualityQuery query)
            {
                if (query == null) throw new ArgumentNullException("query");
                Clear();
                Interlocked.Exchange(ref _finisht, 0);

                Add(new PlaceHolder(false));
                _query = query;
            }

            public void EnsureAcess()
            {
                if (_finisht == 1) return;

                Async.StartNew(Fill);
            }

            private void Fill()
            {
                lock (this)
                {
                    if (_finisht == 1) return;
                    Interlocked.Exchange(ref _finisht, 1);
                }

// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                _query.GetEnumerator();

                Clear();

                foreach (var item in _query)
                {
                    Add(new NormalQuality(item));
                }

                if (Count == 0)
                    Add(new PlaceHolder(true));
            }
        }
    }

    [ExportViewModel(AppConstants.RadioManagerViewModelName)]
    public sealed class RadioManagerViewModel : TabWorkspace, INotifyBuildCompled
    {
        private class InternalQualityQuery : IQualityQuery
        {
            private string[] _qualitys;
            private Metadatascope _scope;

            public InternalQualityQuery(RadioEntry radio)
            {
                if (radio.IsEmpty) return;

                _scope = radio.Metadata;
                // ReSharper disable once PossibleNullReferenceException
                _qualitys = _scope.GetQualitys().ToArray();
            }

            public IEnumerator<RadioQuality> GetEnumerator()
            {
                return _scope == null
                    ? Enumerable.Empty<RadioQuality>().GetEnumerator()
                    : _qualitys.Select(quality => new RadioQuality(_scope, quality))
                        .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count
            {
                get { return _scope == null ? 0 : _qualitys.Length; }
            }

            public bool IsCompled
            {
                get { return true; }
            }

            public void Reset()
            {
                _qualitys = _scope.GetQualitys().ToArray();
            }
        }

        private class RadioList : ObservableCollection<RadioManagerRadio>
        {
            #region Search

            private readonly SearchHelper _searchHelper;

            private string _currentLanguage;
            private IEnumerable<RadioManagerRadio> _orignal;

            [NotNull]
            public string CurrentLanguage
            {
                get { return _currentLanguage; }
                set
                {
                    if (_currentLanguage == value) return;

                    _currentLanguage = value;
                    SwitchLanguage();
                }
            }

            public bool OnlyFavorites
            {
                get { return _searchHelper.OnlyFavorites; }
                set { _searchHelper.OnlyFavorites = value; }
            }

            [NotNull]
            public string SearchText
            {
                get { return _searchHelper.SearchText; }
                set { _searchHelper.SearchText = value; }
            }

            private void SwitchLanguage()
            {
                BeginChanging();
                base.ClearItems();
                _orignal = _cache[_currentLanguage];
                foreach (var item in _orignal)
                {
                    Items.Add(item);
                }
                CompledChanging();
            }

            public void BeginSearch()
            {
                _searchHelper.Search(_orignal ?? _cache.SelectMany(ent => ent.Value));
            }

            private sealed class SearchHelper
            {
                private readonly IList<RadioManagerRadio> _current;
                private readonly IRadioFavorites _favorites;
                private readonly RadioList _list;
                private IEnumerable<RadioManagerRadio> _original;
                private int _resetSearch;
                private Task _searchTask;
                private string _searchText = String.Empty;

                public SearchHelper([NotNull] IList<RadioManagerRadio> current, [NotNull] RadioList list,
                    [NotNull] IRadioFavorites fovorites)
                {
                    _current = current;
                    _list = list;
                    _favorites = fovorites;
                }

                public bool OnlyFavorites { get; set; }

                [NotNull]
                public string SearchText
                {
                    get { return _searchText; }
                    set { _searchText = value; }
                }

                public void Search([NotNull] IEnumerable<RadioManagerRadio> original)
                {
                    Interlocked.Exchange(ref _original, original);
                    Interlocked.Exchange(ref _resetSearch, 1);
                    Thread.Sleep(20);
                    StartSeach();
                }

                private void StartSeach()
                {
                    if (_searchTask == null || _searchTask.IsCompleted)
                        _searchTask = Async.StartNew(SearchImpl);
                }

                private void SearchImpl()
                {
                    Start:

                    _list.BeginChanging();
                    _current.Clear();
                    _list.InternalCompledChanging();

                    _list.BeginChanging();

                    Parallel.ForEach(_original, (radio, stade) =>
                    {
                        if (Interlocked.Exchange(ref _resetSearch, 0) == 1)
                            stade.Break();
                        if (FilterRadio(radio))
                            _current.Add(radio);
                    });
                    _list.InternalCompledChanging();
                    if (Interlocked.Exchange(ref _resetSearch, 0) == 1)
                        goto Start;
                }

                private bool FilterRadio([NotNull] RadioManagerRadio radio)
                {
                    //if (OnlyFavorites && String.IsNullOrEmpty(SearchText)) return false;

                    if (!OnlyFavorites)
                    {
                        return String.IsNullOrEmpty(_searchText) ||
                               radio.RadioTitle.ToLowerInvariant().Contains(SearchText.ToLowerInvariant());
                    }
                    if (!_favorites.Contains(radio.RadioTitle)) return false;

                    return String.IsNullOrEmpty(_searchText) ||
                           radio.RadioTitle.ToLowerInvariant().Contains(SearchText.ToLowerInvariant());
                }
            }

            #endregion

            #region Collection

            private readonly ObservableDictionary<string, List<RadioManagerRadio>> _cache =
                new ObservableDictionary<string, List<RadioManagerRadio>>();

            private readonly ICollectionView _collectionView;
            private bool _changing;

            public RadioList([NotNull] IRadioFavorites favorites)
            {
                _collectionView = CollectionViewSource.GetDefaultView(this);
                _searchHelper = new SearchHelper(Items, this, favorites);
            }

            public void BeginChanging()
            {
                _changing = true;
            }

            protected override void InsertItem(int index, [NotNull] RadioManagerRadio item)
            {
                item.QualitySelected += QualitySelected;
                string language = item.Entry.Language;

                List<RadioManagerRadio> list;
                if (_cache.TryGetValue(language, out list)) list.Add(item);
                else
                {
                    list = new List<RadioManagerRadio> {item};
                    _cache[language] = list;
                }

                base.InsertItem(index, item);
            }

            protected override void ClearItems()
            {
                foreach (var item in _cache.SelectMany(ent => ent.Value))
                {
                    item.QualitySelected -= QualitySelected;
                }
                _cache.Clear();
                base.ClearItems();
            }

            protected override void RemoveItem(int index)
            {
                Items[index].QualitySelected -= QualitySelected;
                RadioManagerRadio ele = this[index];
                base.RemoveItem(index);

                string language = ele.Entry.Language;
                List<RadioManagerRadio> list;
                if (!_cache.TryGetValue(language, out list)) return;

                list.Remove(ele);
            }

            private void QualitySelected([NotNull] object sender, [NotNull] EventArgs e)
            {
                CurrentDispatcher.Invoke(() => _collectionView.MoveCurrentTo(sender));
            }

            private void InternalCompledChanging()
            {
                _changing = false;
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            public void CompledChanging()
            {
                InternalCompledChanging();
                BeginSearch();
            }

            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                if (!_changing)
                    base.OnCollectionChanged(e);
            }

            protected override void OnPropertyChanged([NotNull] PropertyChangedEventArgs e)
            {
                if (!_changing) base.OnPropertyChanged(e);
            }

            #endregion Collection
        }

        #region Common

        public RadioManagerViewModel()
            : base(RadioStreamerResources.RadioManagerTabTitle)
        {
            CanClose = false;
        }

        [InjectRadioDatabase] 
        private IRadioDatabase _database;
        [InjectRadioEnviroment] 
        private IRadioEnvironment _enviroment;
        [Inject] 
        private IEventAggregator _events;
        [Inject]
        private ITabManager _tabManager;

        private RadioList _radios;
        private ListCollectionView _view;

        [NotNull]
        public ListCollectionView Radios
        {
            get { return _view ?? (_view = (ListCollectionView) CollectionViewSource.GetDefaultView(_radios)); }
        }

        [NotNull]
        public IEnumerable<ViewEntry> Views { get { return _tabManager.Views.Where(v => !v.IsDefault); } }

        [CanBeNull]
        public ViewEntry SelectedView { set { if (value != null) _tabManager.View(value.Id); }
        }

        private void AddRadios([NotNull] IEnumerable<RadioEntry> entrys)
        {
            IRadioFavorites favorites = _enviroment.Settings.Favorites;

            _radios.Clear();
            _radios.BeginChanging();

            var languages = new HashSet<string>();

            foreach (var entry in entrys)
            {
                _radios.Add(new RadioManagerRadio(entry, favorites.Contains(entry.Name), new InternalQualityQuery(entry),
                    OnRadioDelete));
                languages.Add(entry.Language);
            }

            Languages = languages;
            _radios.CompledChanging();
        }

        private void OnRadioDelete([NotNull] RadioManagerRadio radio)
        {
            _radios.Remove(radio);
            if (radio.Favorite)
                _enviroment.Settings.Favorites.Remove(new RadioFavorite(radio.RadioTitle, string.Empty));
        }

        #endregion Common

        #region Loading

        void INotifyBuildCompled.BuildCompled()
        {
            _events.GetEvent<RadioPlayerPlay, EventArgs>().Subscribe(RadioPlay);
            _events.GetEvent<RadioPlayerStop, EventArgs>().Subscribe(RadioStop);

            _radios = CurrentDispatcher.Invoke(() => new RadioList(_enviroment.Settings.Favorites));
            BindingOperations.EnableCollectionSynchronization(_radios, new Object());

            var list = Radios;

            AddRadios(_database.GetRadios());

            var textLogger = new StringBuilder();
            try
            {
                CommonApplication.Current.Container.Resolve<IEngineManager>().PreCompile(new StringWriter(textLogger));
            }
            catch (Exception)
            {
                Log.Write(textLogger, TraceEventType.Error);
                throw;
            }

            list.CurrentChanged += (sendr, e) => CommandManager.InvalidateRequerySuggested();
        }

        #endregion Loading

        #region Search

        private HashSet<string> _languages;

        [NotNull]
        public HashSet<string> Languages
        {
            get { return _languages; }
            set
            {
                if (_languages == value) return;

                _languages = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string CurrentLanguage
        {
            get { return _radios.CurrentLanguage; }
            set
            {
                _radios.CurrentLanguage = value;
                SearchChanged();
                OnPropertyChanged();
            }
        }

        public bool OnlyFavorites
        {
            get { return _radios.OnlyFavorites; }
            set
            {
                _radios.OnlyFavorites = value;
                SearchChanged();
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string SearchText
        {
            get { return _radios.SearchText; }
            set
            {
                _radios.SearchText = value;
                SearchChanged();
                OnPropertyChanged();
            }
        }

        private void SearchChanged()
        {
            _radios.BeginSearch();
        }

        #endregion Search

        #region Metadata

        [CommandTarget]
        private bool CommonCanExecute()
        {
            return Radios.CurrentItem != null;
        }

        [CommandTarget(CanExecuteMember = "CommonCanExecute", Synchronize = true)]
        private void MedtadataClick()
        {
            var radio = (RadioManagerRadio) Radios.CurrentItem;

            var temp = new MetadataWindow(radio.Entry) {Owner = CurrentApplication.MainWindow};
            temp.ShowDialog();
        }

        [CommandTarget]
        private bool CanAddToFavorites()
        {
            if (!CommonCanExecute()) return false;
            var radio = (RadioManagerRadio) Radios.CurrentItem;

            return !radio.Favorite;
        }

        [CommandTarget]
        private void AddToFavorites()
        {
            RadioManagerRadio radio = CurrentDispatcher.Invoke(() => (RadioManagerRadio) Radios.CurrentItem);

            IRadioSettings settings = _enviroment.Settings;
            IRadioFavorites favorites = settings.Favorites;

            RadioQuality quality = radio.GetQuality();

            favorites.Add(new RadioFavorite(radio.RadioTitle, quality.IsEmpty ? "Empty" : quality.Name));

            radio.Favorite = true;
            settings.Save();
        }

        [CommandTarget]
        private bool CanRemoveFromFavorites()
        {
            if (!CommonCanExecute()) return false;
            var item = (RadioManagerRadio) Radios.CurrentItem;

            return item.Favorite;
        }

        [CommandTarget]
        private void RemoveFromFavorites()
        {
            var radio = CurrentDispatcher.Invoke(() => (RadioManagerRadio) Radios.CurrentItem);

            var favs = _enviroment.Settings.Favorites;
            favs.Remove(favs.First(fav => fav.Name == radio.RadioTitle));

            radio.Favorite = false;
        }

        [CommandTarget(Synchronize = true)]
        private void AddRadio()
        {
            if (Radios.CurrentItem != null)
                RadioCreateViewModel.Entry = ((RadioManagerRadio) Radios.CurrentItem).Entry;

            IWindow win = ViewManager.CreateWindow(AppConstants.RadioCreateViewModel);

            Task t = win.ShowDialog(ViewManager.GetWindow(AppConstants.MainWindowName));

            Async.StartNew(() =>
            {
                t.Wait();
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (RadioCache.Cache == null || RadioCache.Cache.Count != 1) return;

                _radios.Add(new RadioManagerRadio(RadioCache.Cache[0], false,
                    new InternalQualityQuery(RadioCache.Cache[0]),
                    OnRadioDelete));
            });
        }

        #endregion Metadata

        #region Player Control

        private bool _isPlaying;

        private void RadioStop([NotNull] EventArgs obj)
        {
            _isPlaying = false;
        }

        private void RadioPlay([NotNull] EventArgs obj)
        {
            _isPlaying = true;
        }

        [CommandTarget]
        private bool CanPlay()
        {
            if (!CommonCanExecute()) return false;

            var radio = CurrentDispatcher.Invoke(() => (RadioManagerRadio) Radios.CurrentItem);

            var quality = radio.GetQuality();

            return !quality.IsEmpty;
        }

        [CommandTarget]
        private void Play()
        {
            var radio = CurrentDispatcher.Invoke(() => (RadioManagerRadio) Radios.CurrentItem);

            var quality = radio.GetQuality();

            var playEvent = _events.GetEvent<PlayRadioEvent, PlayRadioEventArgs>();

            playEvent.Publish(new PlayRadioEventArgs(quality, radio.Entry));
        }

        [CommandTarget]
        private bool CanStop()
        {
            return _isPlaying;
        }

        [CommandTarget]
        private void Stop()
        {
            _events.GetEvent<RadioPlayerStop, EventArgs>().Publish(EventArgs.Empty);
        }

        #endregion
    }
}