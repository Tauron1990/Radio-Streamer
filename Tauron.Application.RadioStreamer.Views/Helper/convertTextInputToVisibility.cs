using System;
using System.Windows;
using System.Windows.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        [NotNull]
        public object Convert([NotNull] object[] values, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] System.Globalization.CultureInfo culture )
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool) values[0];
                bool hasFocus = (bool) values[1];

                if (hasFocus || hasText) return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }


        [NotNull]
        public object[] ConvertBack([NotNull] object value, [NotNull] Type[] targetTypes, [NotNull] object parameter, [NotNull] System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
