#region Usings

using System;
using System.Collections.ObjectModel;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;

#endregion

namespace Tauron.Application.RadioStreamer.Views.MedadataView
{
    public class MetadataWindowViewModel : ObservableObject
    {
        private ObservableCollection<string> _metadata;

        public MetadataWindowViewModel(RadioEntry entry)
        {
            if (entry.IsEmpty) return;

            _metadata = new ObservableCollection<string>();

            var meta = entry.Metadata;
            foreach (var item in meta)
            {
                _metadata.Add(String.Format("{0}={1}", item, meta[item]));
            }
        }

        public ObservableCollection<string> Metadata
        {
            get { return _metadata; }
            set
            {
                if (_metadata == value) return;

                _metadata = value;
                OnPropertyChanged();
            }
        }
    }
}