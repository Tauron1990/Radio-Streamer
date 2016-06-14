using System;
using System.Collections.Generic;
using System.IO;
using Tauron.Application.RadioStreamer.Contracts.Player.Recording;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    public sealed class SettingsModifedEventArgs : EventArgs
    {
        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public string Value { get; private set; }

        public SettingsModifedEventArgs([NotNull] string name, [NotNull] string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            Name = name;
            Value = value;
        }
    }

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
	    public string Name { get; }

	    [NotNull]
	    public string QualityName { get; }

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
        event EventHandler<SettingsModifedEventArgs> SettingsModifed;

        int Count { get; }

        [NotNull]
        string GetValue([NotNull] string name, [CanBeNull] string defaultValue);
        void SetValue([NotNull] string name, [NotNull] string value);
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

        bool PlayAfterStart { get; set; }

        bool MinimizeInTray { get; set; }

        string RecodingPath { get; set; }

        FileExisBehavior FileExisBehavior { get; set; }

        bool Delete90SecTitles { get; set; }
        string DefaultDevice { get; set; }
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
