
namespace Tauron.Application.RadioStreamer.Views.Helper
{
	public class LocalizeLabelTextConverter : ValueConverterFactoryBase
	{
	    public string Name { get; set; }

		protected sealed class Converter : StringConverterBase<string>
		{
			private readonly string _name;

			public Converter(string name)
			{
				_name = name;
			}

			protected override string Convert(string value)
			{
				return Resources.RadioStreamerResources.ResourceManager.GetString(_name).SFormat(value);
			}
		}

		protected override System.Windows.Data.IValueConverter Create()
		{
			return new Converter(Name);
		}
	}
}
