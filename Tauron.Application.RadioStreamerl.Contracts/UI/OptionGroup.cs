using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    public class OptionGroup : IOptionElement
    {
        public string DisplayName { get; set; }

        [NotNull]
        public UISyncObservableCollection<Option> Options { get; private set; }

        public OptionGroup()
        {
            Options = new UISyncObservableCollection<Option>();
        }

        public bool Save(IRadioEnvironment store)
        {
            bool ok = false;
            foreach (var option in Options)
            {
                var temp = option.Save(store);
                if (temp)
                    ok = true;
            }

            return ok;
        }

        public void Load(IRadioEnvironment store)
        {
            foreach (var option in Options)
            {
                option.Load(store);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
