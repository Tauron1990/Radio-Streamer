using System;
using System.Collections.Generic;
using System.IO;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    [PublicAPI]
	public interface IStreamReference
	{
		bool UseCache { get; set; }

        [NotNull]
        Stream Open();
	}
	public enum EnviromentLocation
	{
		Temp,
		Radios,
		Other
	}

	public sealed class RadioFavorite
	{
	    [NotNull]
	    public string Name { get; private set; }

	    [NotNull]
	    public string QualityName { get; private set; }

		public RadioFavorite([NotNull] string name, [NotNull] string qualityName)
		{
		    if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(qualityName)) throw new ArgumentException();

		    Name = name;
		    QualityName = qualityName;
		}

	    public override bool Equals([CanBeNull] object obj)
	    {
	        if (ReferenceEquals(obj, null)) return false;

	        var second = obj as RadioFavorite;
	        if (second == null) return false;

	        return second.Name == Name && second.QualityName == QualityName;
	    }

	    public override int GetHashCode()
		{
			return Name.GetHashCode() ^ QualityName.GetHashCode();
		}
	}
    [PublicAPI]
	public interface IRadioFavorites : IEnumerable<RadioFavorite>
	{
		void Remove([NotNull] RadioFavorite favorite);
		void Add([NotNull] RadioFavorite favorite);
		bool Contains([NotNull] string radioName);
	}

    [PublicAPI]
	public interface IRadioSettings
	{
		bool IsFirstStart { get; }
        [NotNull]
        IRadioFavorites Favorites { get; }

        [NotNull]
        IEqualizerProfileDatabase EqualizerDatabase { get; }
		//int ParallelCountQualityTasks { get; set; }

		void Save();
	}

    [PublicAPI]
	public interface IRadioEnvironment
	{
		void Delete([NotNull] string name, EnviromentLocation location);
		void Rename([NotNull] string name, [NotNull] string newName, EnviromentLocation location);
		void ClearDirectory(EnviromentLocation location);

        [NotNull]
        IStreamReference Open([NotNull] string name, EnviromentLocation location, FileMode mode, FileShare share, FileOptions options);

        [NotNull]
        IStreamReference OpenTemp();

        [NotNull]
        IStreamReference OpenTemp([NotNull] string name);

        [NotNull]
        IStreamReference OpenOther([NotNull] string name);

        [NotNull]
        IStreamReference OpenRadio([NotNull] string name);

		bool Exists([NotNull] string name, EnviromentLocation location);
        [NotNull]
        IEnumerable<string> GetFiles(EnviromentLocation location);

        [NotNull]
        string GetFilename([NotNull] string name, EnviromentLocation location);

        [NotNull]
        string DatabasePath { get; set; }

        [NotNull]
        IRadioSettings OpenSettings();

        [NotNull]
        string[] DatabaseFiles { get; set; }
	}
}
