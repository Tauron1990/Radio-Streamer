using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public class CommonComboBoxItem
    {
        [NotNull]
        public string DisplayName { get; private set; }

        [CanBeNull]
        public object Value { get; set; }

        public CommonComboBoxItem([NotNull] string displayName, [CanBeNull] object value)
        {
            if (displayName == null) throw new ArgumentNullException("displayName");
            DisplayName = displayName;
            Value = value;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
