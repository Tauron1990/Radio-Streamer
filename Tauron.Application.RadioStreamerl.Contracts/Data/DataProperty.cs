using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public class DataProperty
    {
        [NotNull]
        public string Key { get; private set; }

        [NotNull]
        public string Value { get; private set; }

        public DataProperty([NotNull] string key, [NotNull] string value)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            Key = key;
            Value = value;
        }
    }
}