using System.Collections.Generic;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public interface IEncoderProfileDatabase
    {
        [NotNull]
        IEnumerable<string> Profiles { get; set; }

        void Serialize([NotNull] string name, [NotNull] CommonProfile profile);

        [NotNull]
        CommonProfile Deserialize([NotNull] string name);
    }
}