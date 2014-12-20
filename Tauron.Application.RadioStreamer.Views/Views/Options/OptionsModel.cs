using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options
{
    [Export(typeof (IUIOptionsManager))]
    public sealed class OptionsModel : IUIOptionsManager
    {
        public OptionsModel()
        {
            
        }

        [Inject]
        private IRadioEnvironment _radioEnvironment;

        private UISyncObservableCollection<OptionPath> _options = new UISyncObservableCollection<OptionPath>(); 

        public event EventHandler<OptionsChangedEventArgs> OptionsChanged;

        public void OnOptionsChanged([NotNull] OptionsChangedEventArgs e)
        {
            EventHandler<OptionsChangedEventArgs> handler = OptionsChanged;
            if (handler != null) handler(this, e);
        }

        public IEnumerable<OptionPath> Options { get { return _options; } }

        public void RegisterOption(string path, Option option)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (option == null) throw new ArgumentNullException("option");

            string[] segemnts = path.Split('\\');

            bool isNew = false;
            OptionPath root = null;
            OptionPath current = null;

            foreach (string name in segemnts)
            {
                if (root == null)
                {
                    string name1 = name;
                    root = _options.FirstOrDefault(op => op.DisplayName == name1);
                    if (root == null)
                    {
                        root = new OptionPath {DisplayName = name};
                        isNew = true;
                    }
                    current = root;
                    continue;
                }

                var temp =
                    (OptionPath) current.Elements.FirstOrDefault(op => op.DisplayName == name && op is OptionPath);
                if (temp == null)
                {
                    temp = new OptionPath {DisplayName = name};
                    current.Elements.Add(temp);
                }

                current = temp;
            }

            if(isNew)
                _options.Add(root);

            if (current == null) throw new InvalidOperationException();

            if (string.IsNullOrEmpty(option.Group))
                current.Elements.Add(option);
            else
            {
                var gr =
                    (OptionGroup)
                        current.Elements.FirstOrDefault(e => e.DisplayName == option.Group && e is OptionGroup);
                if (gr == null)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    gr = new OptionGroup { DisplayName = option.Group };
                    current.Elements.Add(gr);
                }
                
                gr.Options.Add(option);
            }

            option.Load(_radioEnvironment);
        }

        public void Save()
        {
            var saveSettings = false;

// ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var optionPath in Options)
                if (optionPath.Save(_radioEnvironment))
                    saveSettings = true;

            if(saveSettings)
                _radioEnvironment.Settings.Save();
        }
    }
}