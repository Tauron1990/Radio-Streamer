#region Usings

using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using Tauron.Application.Ioc;
using Tauron.Application.Modules;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views
{
    [Export(typeof (IModule))]
    internal sealed class ViewModule : IModule
    {
        [Inject]
        private IStyleManager _styleManager;

        [Inject] 
        private IDialogFactory _dialogFactory;

        [Inject]
        private IProgramManager _programManager;

        [Inject] 
        private ITabManager _tabManager;

        [NotNull,AddinDescription]
        public AddinDescription GetDescription()
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetName();
            return new AddinDescription(name.Version,
            // ReSharper disable PossibleNullReferenceException
                asm.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                asm.GetCustomAttribute<AssemblyTitleAttribute>().Title);
            // ReSharper restore PossibleNullReferenceException
        }

        public int Order
        {
            get { return 1000; }
        }

        public void Initialize(CommonApplication application)
        {
            _tabManager.RegisterView(new ViewEntry(AppConstants.RadioManagerViewModelName, string.Empty, true));
            _tabManager.RegisterView(new ViewEntry(AppConstants.RadioPlayerViewModelName, string.Empty, true));

            MenuItemService.RegisterNotify(
                FillThemeMenu(new GenericMenuItem {Id = "ThemeMenu", Label = RadioStreamerResources.ThemeMenu}));
            MenuItemService.RegisterNotify(
                new GenericMenuItem(i => _programManager.ShowWindow(AppConstants.AddInViewModel, true))
                {
                    Label = RadioStreamerResources.ViewAddinManagerLabel
                });

            CommandBinder.Register(ApplicationCommands.Save);

            CommandBinder.Register(MediaCommands.Play);
            CommandBinder.Register(MediaCommands.Stop);
            CommandBinder.Register(MediaCommands.Record);
            CommandBinder.Register(MediaCommands.MuteVolume);

            CommandBinder.Register(RadioStreamerResources.ViewMetadataLebel, "Metadata");
            CommandBinder.Register(RadioStreamerResources.AddFavoritesLabel, "AddFavorites");
            CommandBinder.Register(RadioStreamerResources.RemoveFavoritesLabel, "RemoveFavorites");

            SimpleLocalize.Register(RadioStreamerResources.ResourceManager, GetType().Assembly);
        }

        [NotNull]
        private GenericMenuItem FillThemeMenu([NotNull] GenericMenuItem item)
        {
            item.MenuItems = new List<GenericMenuItem>();

            foreach (var theme in _styleManager.Themes)
            {
                string localTheme = theme;
                var themeItem = new GenericMenuItem(i =>
                {
                    _styleManager.SetTheme(localTheme);
                    var result = _dialogFactory.ShowMessageBox(CommonApplication.Current.MainWindow,
                        RadioStreamerResources.ThemeSwichMessage, RadioStreamerResources.ThemeMenu, MsgBoxButton.YesNo,
                        MsgBoxImage.Information, null);

                    if (result == MsgBoxResult.Yes)
                        _programManager.Restart();
                })
                {
                    Id = localTheme,
                    Label = localTheme.GetFileNameWithoutExtension(),
                    IsEnabled = _styleManager.CurrentTheme != localTheme
                };

                _styleManager.ThemeChanged +=
                    (sender, args) => themeItem.IsEnabled = localTheme != _styleManager.CurrentTheme;

                item.MenuItems.Add(themeItem);
            }

            return item;
        }
    }
}