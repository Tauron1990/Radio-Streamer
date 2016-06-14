#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.Modules;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Player.Misc;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.RadioStreamer.Resources;
using Tauron.Application.RadioStreamer.Views.EncodingOptions;
using Tauron.Application.RadioStreamer.Views.Options.UiHelpers;
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

        [Inject]
        private IUIOptionsManager _optionsManager;

        [Inject]
        private IDeviceManager _deviceManager;

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

        public int Order => 1000;

        public void Initialize(CommonApplication application)
        {
            _tabManager.RegisterView(new ViewEntry(AppConstants.RadioManagerViewModelName, string.Empty, true));
            _tabManager.RegisterView(new ViewEntry(AppConstants.RadioPlayerViewModelName, string.Empty, true));

            MenuItemService.RegisterNotify(new GenericMenuItem(i =>
            {
                if (_programManager.MainWindow.IsVisible == false) _programManager.MainWindow.Show();
            }) {Id = "ViewMainWindow", Label = RadioStreamerResources.ViewMainWindowMenuLabel});

            MenuItemService.RegisterNotify(
                FillThemeMenu(new GenericMenuItem {Id = "ThemeMenu", Label = RadioStreamerResources.ThemeMenu}));
            MenuItemService.RegisterNotify(
                new GenericMenuItem(i => _programManager.ShowWindow(AppConstants.AddInViewModel, true))
                {
                    Label = RadioStreamerResources.ViewAddinManagerLabel
                });
            MenuItemService.RegisterNotify(
                new GenericMenuItem(i => _programManager.ShowWindow(AppConstants.OptionsViewModel, true))
                {
                    Label = RadioStreamerResources.ViewOptionsLabel
                });
            MenuItemService.RegisterNotify(new GenericMenuItem(i =>
            {
                _programManager.Shutdown = true;
                if (_programManager.MainWindow.IsVisible == true)
                    _programManager.MainWindow.Close();
                else
                    CommonApplication.Current.Shutdown();
            }) {Label = RadioStreamerResources.QuitLabel});

            string oGeneralRoot = RadioStreamerResources.OptionsPathGenral;
            string oPlayerRoot = RadioStreamerResources.OptionsPathPlayer;
            string oRecordingRoot = oPlayerRoot.CombinePath(RadioStreamerResources.OptionsPathRecording);
            string oEncoderRoot = oRecordingRoot.CombinePath(RadioStreamerResources.OptionsPathEncoder);

          
            _optionsManager.RegisterOption(oGeneralRoot,
                new Option(null, new CheckBoxHelper(StartWithWindowsOption), "StartWithWindows", false,
                    RadioStreamerResources.StartOnWindowsOtpion) {IsNameVisibly = false});

            _optionsManager.RegisterOption(oGeneralRoot,
                new Option(null, new CheckBoxHelper(null), "PlayAfterStart", false,
                    RadioStreamerResources.PlayAfterStartOption) {IsNameVisibly = false});

            _optionsManager.RegisterOption(oGeneralRoot,
                new Option(null, new CheckBoxHelper(AutoUpdateCallBack), "AutoUpdate", false,
                    RadioStreamerResources.AutoUpdateOption) {IsNameVisibly = false});

            _optionsManager.RegisterOption(oGeneralRoot,
                new Option(null, new CheckBoxHelper(null), "Minimizeintray", false,
                    RadioStreamerResources.MinimizeintrayOption) {IsNameVisibly = false});

            BuildDeviceOptions(oGeneralRoot);

            _optionsManager.RegisterOption(oPlayerRoot,
                new Option(null, new CheckBoxHelper(null), "Delete90SecTitles", true,
                    RadioStreamerResources.OptionsDelete90SecTitlesText) { IsNameVisibly = false });

            _optionsManager.RegisterOption(oRecordingRoot,
                new Option(null, new DefaultProfileHelper(), string.Empty, string.Empty,
                    RadioStreamerResources.DefaultProfileOptionName));

            _optionsManager.RegisterOption(oRecordingRoot,
                new Option(null, new TextBoxHelper(true, true, _dialogFactory), "RecodingPath", string.Empty,
                    RadioStreamerResources.OptionsRecordingPath));

            _optionsManager.RegisterOption(oRecordingRoot,
                new Option(null,
                    new ComboboxHelper(
                        new ComboboxHelperItem(nameof(FileExisBehavior.Override),
                            RadioStreamerResources.OptionsFileExisBehaviorOverrideText),
                        new ComboboxHelperItem(nameof(FileExisBehavior.ReName),
                            RadioStreamerResources.OptionsFileExisBehaviorRenameText),
                        new ComboboxHelperItem(nameof(FileExisBehavior.Skip),
                            RadioStreamerResources.OptionsFileExisBehaviorSkipText)), "FileExisBehavior",
                    nameof(FileExisBehavior.Override), RadioStreamerResources.OptionsFileExisBehavior));

            _optionsManager.RegisterOption(oEncoderRoot,
                new Option(null, new EncodingEditorHelper(), string.Empty,
                    ViewModelBase.ResolveViewModel(AppConstants.CommonEncoderUI), "ProfileOption")
                {
                    IsNameVisibly = false
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

        private void AutoUpdateCallBack(bool obj)
        {
            _programManager.AutoUpdate = obj;
        }

        private void StartWithWindowsOption(bool add)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if(key == null) return;
                    if (add)
                    {
                        //Surround path with " " to make sure that there are no problems
                        //if path contains spaces.
                        key.SetValue("RadioStreamerSlim", "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
                    }
                    else
                        key.DeleteValue("RadioStreamerSlim");

                    key.Close();
                }
            }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }
        }

        [NotNull]
        private GenericMenuItem FillThemeMenu([NotNull] GenericMenuItem item)
        {
            item.MenuItems = new List<GenericMenuItem>();

            if (_styleManager.Themes.Length == 0)
            {
                item.MenuItems.Add(new GenericMenuItem { IsEnabled = false, Label = RadioStreamerResources.NoneThemeLabel});
                return item;
            }

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

        private void BuildDeviceOptions(string path)
        {
            List<ComboboxHelperItem> items = new List<ComboboxHelperItem>
            {
                new ComboboxHelperItem("null", RadioStreamerResources.OptionsDefaultDeviceDefaultValue)
            };

            items.AddRange(_deviceManager.Select(device => new ComboboxHelperItem(device.Id ?? device.Name, device.Name)));

            _optionsManager.RegisterOption(path, new Option(null, new ComboboxHelper(items.ToArray()), "DefaultDevice", "null", RadioStreamerResources.OptionsDefaultDevice));
        }
    }
}