using System.Windows.Media;
using Tauron.Application.Converter;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
	public sealed class FavoriteColorConverter : ValueConverterFactoryBase
	{
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


		protected override System.Windows.Data.IValueConverter Create()
		{
			return new Converter();
		}
	}
}
