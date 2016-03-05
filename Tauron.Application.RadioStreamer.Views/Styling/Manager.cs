using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.Application.Views;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Styling
{
    [Export(typeof(IStyleManager))]
    internal sealed class Manager : IStyleManager, INotifyBuildCompled
    {
        private const string ThemePath = "Themes";
        private const string ThemeExt = "skin";
        private const string ResourceAssembly = "Tauron.Application.RadioStreamer.Resources";

        [InjectRadioEnviroment] [NotNull]
        private IRadioEnvironment _tauronEnviroment;

        private readonly string[] _themes;

        [Inject(typeof (IPlugInManager))]
        private readonly IPlugInManager _plugInManager;

        private readonly ControlRegistry _registry = new ControlRegistry();

        public Manager()
        {
            ThemePath.CreateDirectoryIfNotExis();
            _themes = ThemePath.GetFiles("*." + ThemeExt, SearchOption.TopDirectoryOnly);
        }

        public event EventHandler ThemeChanged;

        private void OnThemeChanged()
        {
            EventHandler handler = ThemeChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public string CurrentTheme => _tauronEnviroment.Settings.Theme;

        public string[] Themes => _themes;

        public void SetTheme(string name)
        {
            if (string.IsNullOrEmpty(name) || !_themes.Contains(name)) name = string.Empty;
            // ReSharper disable once AssignNullToNotNullAttribute
            _tauronEnviroment.Settings.Theme = name;
            _tauronEnviroment.Settings.Save();
        
            OnThemeChanged();
        }

        public void LoadResources()
        {
            try
            {
                try
                {
                    if (string.IsNullOrEmpty(CurrentTheme) || !_themes.Contains(CurrentTheme)) throw new NotSupportedException();

                    using (var mainStream = CurrentTheme.OpenRead())
                    {
                        var archive = new ZipArchive(mainStream);

                        var entry = archive.GetEntry("Plugins.ini");
                        if (entry != null)
                        {
                            using (var pstream = entry.Open())
                            {
                                foreach (
                                    string[] parts in
                                        new StreamReader(pstream).EnumerateTextLines()
                                                                 .Where(line => !string.IsNullOrWhiteSpace(line))
                                                                 .Select(
                                                                     line =>
                                                                     line.Split(new[] {'='},
                                                                                StringSplitOptions.RemoveEmptyEntries))
                                                                 .Where(parts => parts.Length == 2))
                                {
                                    if (parts[0] == "package")
                                    {
                                        _plugInManager.LoadPakage(parts[1]);
                                    }
                                    else if (parts[0] == "plugin")
                                    {
                                        _plugInManager.InstallPlugIn(parts[1]);
                                    }
                                }
                            }
                        }

                        foreach (var image in archive.Entries.Where(e => e.FullName.StartsWith("Images")))
                        {
                            ImagesCache.ImageSources[image.Name.GetFileNameWithoutExtension()] =
                                BitmapFrame.Create(image.Open(), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        }

                        entry = archive.GetEntry("Init.cs");
                        if (entry != null)
                        {
                            var comp = new StyleScriptCompiler();
                            var builder = new StringBuilder();
                            const string moduleName = "ThemeInit.dll";
                            AssemblyBuilder dyn =
                                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(moduleName),
                                    AssemblyBuilderAccess.Run);
                            
                            comp.Init(new StringWriter(builder),  dyn, moduleName);

                            using (var stream = entry.Open())
                            {
                                comp.BuildUp(new StreamReader(stream).ReadToEnd());
                            }

                            ((ThemeLoaderBase)
                                (Activator.CreateInstance(
                                    dyn.GetTypes().First(t => t.BaseType == typeof (ThemeLoaderBase))))).Load();
                        }

                        entry = archive.GetEntry("Theme.xaml");
                        if (entry != null)
                        {
                            using (var xStream = entry.Open())
                                WpfApplication.CurrentWpfApplication.Resources.MergedDictionaries.Add(
                                    (ResourceDictionary) XamlReader.Load(xStream));
                        }

                        foreach (
                            var archiveEntry in
                                archive.Entries.Where(archiveEntry => archiveEntry.FullName.StartsWith("Controls") && archiveEntry.FullName.EndsWith("xaml")))
                        {
                            using (var xstream = archiveEntry.Open())
                            {
                                _registry.Register(archiveEntry.Name.GetFileNameWithoutExtension(),
                                    new StreamReader(xstream).ReadToEnd());
                            }
                        }
                    }

                    return;
                }
                catch (Exception e)
                {
                    if (CriticalExceptions.IsCriticalApplicationException(e))
                        throw; 
                    SetTheme(string.Empty);
                }

                ResourceDictionary resources = WpfApplication.CurrentWpfApplication.Resources;

                resources.MergedDictionaries.Clear();

                //Colors
                resources["BackgroundBrush"] = Brushes.WhiteSmoke;
                resources["MiddleLightBrush"] = Brushes.LightBlue;
                resources["FavoriteBrush"] = Brushes.DarkGoldenrod;
                resources["FavoriteNormalColor"] = Brushes.White;
                resources["TextColor"] = Brushes.Black;
                resources["RadioTitleBrush"] = Brushes.DarkSlateGray;

                //Images
                resources["PlayImage"] =
                    BitmapFrame.Create(
                        new Uri("pack://application:,,,/Tauron.Application.RadioStreamer.Resources;component/play.png"));

                resources.MergedDictionaries.Add(
                    (ResourceDictionary) System.Windows.Application.LoadComponent(
                        new Uri(
                            "/PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/Aero.NormalColor.xaml",
                            UriKind.Relative)));
            }
            finally
            {
                const string playImage = "PlayImage";
                const string stopImage = "StopImage";
                const string addRadioImage = "AddRadioImage";

                const string recordImage = "RecordImage";
                const string recordActiveImage = "RecordActiveImage";

                const string audioVolumeHigh = "AudioVolumeHighImage";
                const string audioVolumeMedium = "AudioVolumeMediumImage";
                const string audioVolumeLow = "AudioVolumeLowImage";
                const string audioVolumeMuted = "AudioVolumeMutedImage";

                var images = ImagesCache.ImageSources;
                images.AddIfNotExis(playImage,
                                    () =>
                                    BitmapFrame.Create(
                                        new Uri(
                                            "pack://application:,,,/Tauron.Application.RadioStreamer.Resources;component/play.png")));
                images.AddIfNotExis(stopImage,
                                    () => BitmapFrame.Create(
                                        new Uri(
                                            "pack://application:,,,/Tauron.Application.RadioStreamer.Resources;component/stop.png")));
                images.AddIfNotExis(addRadioImage,
                                    () => BitmapFrame.Create(
                                        new Uri(
                                            "pack://application:,,,/Tauron.Application.RadioStreamer.Resources;component/add.png")));
                images.AddIfNotExis(recordImage,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/Record.png", ResourceAssembly, true)));
                images.AddIfNotExis(recordActiveImage,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/RecordActive.png", ResourceAssembly,
                                                                                  true)));
                images.AddIfNotExis(audioVolumeHigh,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/audio-volume-high.png",
                                                                                  ResourceAssembly, true)));
                images.AddIfNotExis(audioVolumeMedium,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/audio-volume-medium.png",
                                                                                  ResourceAssembly, true)));
                images.AddIfNotExis(audioVolumeLow,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/audio-volume-low.png",
                                                                                  ResourceAssembly, true)));
                images.AddIfNotExis(audioVolumeMuted,
                                    () =>
                                    BitmapFrame.Create(StaticPackUrihelper.GetUri("/audio-volume-muted.png",
                                                                                  ResourceAssembly, true)));

                ResourceDictionary resources = WpfApplication.CurrentWpfApplication.Resources;
                resources[playImage] = images[playImage];
                resources[stopImage] = images[stopImage];

                resources[addRadioImage] = images[addRadioImage];
                resources[recordImage] = images[recordImage];

                resources[recordActiveImage] = images[recordActiveImage];
                resources[audioVolumeHigh] = images[audioVolumeHigh];
                resources[audioVolumeMedium] = images[audioVolumeMedium];
                resources[audioVolumeLow] = images[audioVolumeLow];
                resources[audioVolumeMuted] = images[audioVolumeMuted];
            }
        }

        public void BuildCompled()
        {
            ViewManager.Manager.ViewLocator = _registry;
        }
    }
}