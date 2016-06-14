using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
    public sealed class RadioQualityFactory
    {
        private readonly Metadatascope _baseScope;

        public RadioQualityFactory(RadioEntry baseEntry)
        {
            _baseScope = baseEntry.Metadata;
        }
        public RadioQualityFactory([NotNull] Metadatascope baseScope)
        {
            if (!baseScope.IsRadio) throw new InvalidOperationException();
            _baseScope = baseScope;
        }

        public RadioQuality Create([NotNull] string name, [NotNull] string url, [NotNull] string sourceUrl)
        {
            var temp = RadioQuality.CreateNew(_baseScope, name, url);
            temp.SourceUrl = sourceUrl;
            return temp;
        }

        public void RemoveQuality([NotNull] string name)
        {
            _baseScope.Delete(name, true);
        }
    }
}