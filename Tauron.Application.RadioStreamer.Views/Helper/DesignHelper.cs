using System;
using System.Windows.Markup;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    public sealed class DesignHelper : MarkupExtension
    {
        private readonly Type _type;

        public DesignHelper(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Activator.CreateInstance(_type);
        }
    }
}