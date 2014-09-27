using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Tauron.Application.Converter;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Core
{
    [MarkupExtensionReturnType(typeof (IValueConverter))]
    public sealed class ManualHeaderTemplateContentLoader : ValueConverterFactoryBase
    {
        private class  Converter : IValueConverter
        {
            [CanBeNull]
            public object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
            {
                var template = parameter as DataTemplate;
                if (template == null) return value;

                var content = template.LoadContent();
                new FrameworkObject(content).DataContext = value;

                return content;
            }

            [CanBeNull]
            public object ConvertBack([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter, [NotNull] CultureInfo culture)
            {
                return new FrameworkObject(value, false).DataContext;
            }
        }

        protected override IValueConverter Create()
        {
            return new Converter();
        }
    }
}