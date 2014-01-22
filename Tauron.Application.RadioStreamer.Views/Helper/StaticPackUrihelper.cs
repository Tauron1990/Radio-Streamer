using System;
using Tauron.Application.Composition;

namespace Tauron.Application.RadioStreamer.Views.Helper
{
    [JetBrains.Annotations.PublicAPI]
    public static class StaticPackUrihelper
    {
        public static IPackUriHelper PackUriHelperInstance
        {
            get
            {
                return CompositionServices.Container.Resolve<IPackUriHelper>();
            }
        }

        public static Uri GetUri(string resource, string resourceAssembly, bool full)
        {
            return PackUriHelperInstance.GetUri(resource, resourceAssembly, full);
        }
    }
}
