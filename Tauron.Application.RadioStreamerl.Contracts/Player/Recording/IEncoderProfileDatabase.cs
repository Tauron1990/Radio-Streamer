using System;
using System.Collections.Generic;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoderProfileDatabase
    {
        [NotNull]
        Tuple<string, CommonProfile> Default { get; set; }

        void SetDefault([CanBeNull] string name);

        [NotNull]
        IEnumerable<string> Profiles { get; }

        void Serialize([NotNull] string name, [NotNull] CommonProfile profile);

        [CanBeNull]
        CommonProfile Deserialize([NotNull] string name);

        void Delete([CanBeNull] string profile);
    }
}