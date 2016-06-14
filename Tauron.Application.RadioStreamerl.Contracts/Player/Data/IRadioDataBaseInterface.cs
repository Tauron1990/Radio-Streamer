using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    [PublicAPI]
	public interface IRadioDatabaseInterface : IDatabaseInterface
    {
        event Action QualityChanged; 

		void DeleteQuality([NotNull] string name);
        [NotNull]
        IEnumerable<string> GetQualitys();

        [NotNull]
        IQualityDatabaseInterface GetQuality([NotNull] string name);

        IQualityDatabaseInterface CreateQuality(string name, string url);
    }

    [PublicAPI]
	public interface IQualityDatabaseInterface : IDatabaseInterface
	{}

    [PublicAPI]
	public interface IDatabaseInterface : IEnumerable<string>
    {
        event PropertyChangingEventHandler ValueChanged;

        event Action ElementDeleted;

        [NotNull]
        string Read([NotNull] string key);
		void Write([NotNull] string key, [NotNull] string value, out bool added);
		void Delete([NotNull] string key);
	}
}
