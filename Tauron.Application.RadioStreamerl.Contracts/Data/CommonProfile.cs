using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public sealed class CommonProfile
    {
        [NotNull]
        public string Id { get; private set; }

        public CommonProfile([NotNull] string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            
            Id = id;
            Properties = new List<DataProperty>();
        }

        [NotNull]
        public List<DataProperty> Properties { get; private set; }

        public void TrySetProperty<TType>([NotNull] Func<string, TType> converter, [NotNull] Action<TType> setter,
            [NotNull] string name)
        {
            if (converter == null) throw new ArgumentNullException("converter");
            if (setter == null) throw new ArgumentNullException("setter");
            if (name == null) throw new ArgumentNullException("name");

            DataProperty prop = Properties.Find(p => p.Key == name);
            if (prop == null) return;

            setter(converter(prop.Value));
        }
    }
}