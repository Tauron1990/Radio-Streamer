#region Usings

using System.Windows.Data;
using Tauron.Application.Converter;
using Tauron.Application.RadioStreamer.Resources;

#endregion

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public class LocalizeLabelTextConverter : ValueConverterFactoryBase
    {
        public string Name { get; set; }

        protected override IValueConverter Create()
        {
            return new Converter(Name);
        }

        protected sealed class Converter : StringConverterBase<string>
        {
            private readonly string _name;

            public Converter(string name)
            {
                _name = name;
            }

            protected override string Convert(string value)
            {
                return RadioStreamerResources.ResourceManager.GetString(_name).SFormat(value);
            }
        }
    }
}