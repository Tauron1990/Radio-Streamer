﻿using System;
using Tauron.Application.Composition;

namespace Tauron.Application.RadioStreamer.Views.Styling
{
    [JetBrains.Annotations.PublicAPI]
    public static class StaticPackUrihelper
    {
        public static IPackUriHelper PackUriHelperInstance => CompositionServices.Container.Resolve<IPackUriHelper>();

        public static Uri GetUri(string resource, string resourceAssembly, bool full)
        {
            return PackUriHelperInstance.GetUri(resource, resourceAssembly, full);
        }
    }
}
