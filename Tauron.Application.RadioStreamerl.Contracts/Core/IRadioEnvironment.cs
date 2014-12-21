using System;
using System.Collections.Generic;
using System.IO;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    [PublicAPI]
	public interface IStreamReference
	{
		bool UseCache { get; set; }

        [NotNull]
        Stream OpenRead();

        [NotNull]
        Stream OpenNew();
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

	        return second.Name == Name;
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

    public interface IPropertyStore : IEnumerable<string>
    {
        [NotNull]
        string GetValue([NotNull] string name, [CanBeNull] string defaultValue);
        void SetName([NotNull] string name, [NotNull] string value);

        void SaveRaw();
    }

    [PublicAPI]
	public interface IRadioSettings
	{
        [NotNull]
        IEncoderProfileDatabase EncoderProfiles { get; }

        [NotNull]
        string LastSprecturm { get; set; }

		bool IsFirstStart { get; }
        [NotNull]
        IRadioFavorites Favorites { get; }

        [NotNull]
        IPropertyStore PropertyStore { get; }

        [NotNull]
        IEqualizerProfileDatabase EqualizerDatabase { get; }
		//int ParallelCountQualityTasks { get; set; }

		void Save();

        [NotNull]
        string Theme { get; set; }
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
        IRadioSettings Settings { get; }

        void ReloadSetting();

        [NotNull]
        string[] DatabaseFiles { get; set; }
	}
}
