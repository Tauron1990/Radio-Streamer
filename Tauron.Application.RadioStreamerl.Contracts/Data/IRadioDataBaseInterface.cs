using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    [PublicAPI]
	public interface IRadioDatabaseInterface : IDatabaseInterface
	{
		void DeleteQuality([NotNull] string name);
        [NotNull]
        IEnumerable<string> GetQualitys();

        [NotNull]
        IQualityDatabaseInterface GetQuality([NotNull] string name);
	}

    [PublicAPI]
	public interface IQualityDatabaseInterface : IDatabaseInterface
	{}

    [PublicAPI]
	public interface IDatabaseInterface : IEnumerable<string>
	{
        [NotNull]
        string Read([NotNull] string key);
		void Write([NotNull] string key, [NotNull] string value, out bool added);
		void Delete([NotNull] string key);
	}
}
