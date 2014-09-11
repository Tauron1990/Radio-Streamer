using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    [Export(typeof (IPlugInManager))]
    public class PlugInManager : IPlugInManager
    {
        private readonly InternalPackageManager _pluginManager;
        private readonly InternalPackageManager _packageManager;

        public PlugInManager()
        {
            _pluginManager = InternalPackageManager.BuildPluginManager();
            _packageManager = InternalPackageManager.BuildPackManager();
        }

        public void LoadPakage(string name)
        {
            foreach (var file in _packageManager.Install(name, true).Where(f => f.EndsWith(".dll") || f.EndsWith(".exe")))
            {
                try
                {
                    Assembly.LoadFrom(file);
                }
                catch (IOException){}
                catch(BadImageFormatException){}
                catch(ArgumentException){}
            }
        }

        public void InstallPlugIn(string name)
        {
            _pluginManager.Install(name);
        }

        public void Unistall(string name, bool plugin)
        {
            if(plugin)
                _pluginManager.UnInstall(name);
            else 
                _packageManager.UnInstall(name);
        }
    }
}