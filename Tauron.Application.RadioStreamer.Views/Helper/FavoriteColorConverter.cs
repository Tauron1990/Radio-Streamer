#region Usings

using System.Windows.Data;
using System.Windows.Media;
using Tauron.Application.Converter;

#endregion

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public sealed class FavoriteColorConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        private sealed class Converter : ValueConverterBase<bool, Brush>
        {
            protected override Brush Convert(bool value)
            {
                return
                    (Brush)
                        (value
                            ? WpfApplication.CurrentWpfApplication.Resources["FavoriteBrush"]
                            : WpfApplication.CurrentWpfApplication.Resources["FavoriteNormalColor"]);
            }
        }
    }
}