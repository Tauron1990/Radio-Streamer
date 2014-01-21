using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data;
using Tauron.Application.RadioStreamer.Contracts.Data.Attributes;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Database
{
	[RadioDatabaseExport]
	public class RadioDatabase : IRadioDatabase, INotifyBuildCompled
	{
		private const string RadioDatabaseString = "RadioDatabase";
		private const string QualityDatabase = "QualityDatabase";
		private static readonly string[] QualitySplitter = { "[SP]" }; 

		private class RadioFactory : IRadioEntryFactory
		{
			private RadioDatabase _database;

			public RadioFactory([NotNull] RadioDatabase radioInterface)
			{
			    _database = radioInterface;
			}

		    public RadioEntry AddOrGetEntry(string name, out bool newEntry)
			{
				return _database.CreateRadio(name, out newEntry);
			}

			public void Save()
			{
				_database.Save();
			}
		}

		private class DatabaseInterface : IDatabaseInterface
		{
		    [NotNull]
		    protected DatabaseHelper.DatabaseEntry BaseEntry { get; private set; } 

			public DatabaseInterface([NotNull] DatabaseHelper.DatabaseEntry baseEntry)
			{
			    BaseEntry = baseEntry;
			}

		    public virtual string Read(string key)
			{
				return BaseEntry.FindMetadata(key).Value;
			}

			public virtual void Write(string key, string value, out bool added)
			{
				BaseEntry.AddMetadata(key, out added).Value = value;
			}

			public virtual void Delete(string key)
			{
				BaseEntry.RemoveMetadata(key);
			}

			public virtual IEnumerator<string> GetEnumerator()
			{
				return BaseEntry.Keys.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
		private class RadioDatabaseInterface : DatabaseInterface, IRadioDatabaseInterface
		{
			private DatabaseHelper.DatabaseEntry _quality;

			public RadioDatabaseInterface([NotNull] DatabaseHelper.DatabaseEntry radio, [NotNull] DatabaseHelper.DatabaseEntry quality)
				: base(radio)
			{
			}

		    public void DeleteQuality(string name)
			{
				foreach (var item in _quality.Keys.ToArray().Where(str => str.StartsWith(name)))
				{
					_quality.RemoveMetadata(item);
				}
			}

			public IEnumerable<string> GetQualitys()
			{
				var set = new HashSet<string>();

				foreach (var item in _quality.Keys)
				{
					set.Add(item.Split(QualitySplitter, StringSplitOptions.RemoveEmptyEntries)[0]);
				}

				return set;
			}

			public IQualityDatabaseInterface GetQuality(string name)
			{
				return new QualityDatabaseInterface(name, _quality);
			}
		}
		private class QualityDatabaseInterface : DatabaseInterface, IQualityDatabaseInterface
		{
			private string _qualityName;

			public QualityDatabaseInterface([NotNull] string qualityName, [NotNull] DatabaseHelper.DatabaseEntry entry)
				: base(entry)
			{
			    _qualityName = qualityName;
			}

		    public override void Delete(string key)
			{
				base.Delete(GetFullname(_qualityName, key));
			}
			public override string Read(string key)
			{
				return base.Read(GetFullname(_qualityName, key));
			}
			public override void Write(string key, string value, out bool added)
			{
				base.Write(GetFullname(_qualityName, key), value, out added);
			}
			public override IEnumerator<string> GetEnumerator()
			{
				return BaseEntry.Keys.Where(str => str.StartsWith(_qualityName))
										.Select(str => str.Split(QualitySplitter, StringSplitOptions.None)[1])
										.GetEnumerator();
			}
		}

	    [NotNull]
	    internal static string GetFullname([NotNull] string qualityName, [NotNull] string key)
	    {
	        return String.Concat(qualityName, QualitySplitter[0], key);
	    }

	    [InjectRadioEnviroment]
		private IRadioEnvironment _environment;

		private DatabaseHelper _radios;
		private DatabaseHelper _qualitys;

		private RadioFactory _factory;

		public RadioDatabase()
		{
			_factory = new RadioFactory(this);
		}

		private System.Threading.ManualResetEventSlim _startLock = new System.Threading.ManualResetEventSlim(false);

        void INotifyBuildCompled.BuildCompled()
		{
			using (var steam = _environment.OpenRadio(RadioDatabaseString).Open())
			{
				_radios = new DatabaseHelper(new StreamReader(steam).EnumerateTextLines()); 
			}
			using (var steam = _environment.OpenRadio(QualityDatabase).Open())
			{
				_qualitys = new DatabaseHelper(new StreamReader(steam).EnumerateTextLines());
			}

			_environment.DatabaseFiles = new[] { RadioDatabaseString, QualityDatabase};
			_startLock.Set();
		}

		public System.Threading.ManualResetEventSlim StartLock
		{
			get { return _startLock; }
		}

		public IRadioEntryFactory GetEntryFactory()
		{
			return _factory;
		}

		public IEnumerable<RadioEntry> GetRadios()
		{
		    return _radios.Select(item => new RadioEntry(new Metadatascope(new RadioDatabaseInterface(item, _qualitys.FindEntry(item.Name)))));
		}

	    public RadioEntry OpenRadio(string name)
		{
			return new RadioEntry(new Metadatascope(new RadioDatabaseInterface(_radios.FindEntry(name), _qualitys.FindEntry(name))));
		}

		public RadioEntry CreateRadio(string name, out bool created)
		{
			bool temp;
			var entry = _radios.AddEntry(name, out created);
			var quality = _qualitys.AddEntry(name, out temp);

			return new RadioEntry(new Metadatascope(new RadioDatabaseInterface(entry, quality)));
		}

		public void DeleteRadio(string name)
		{
			_radios.RemoveEntry(name);
			_qualitys.RemoveEntry(name);
		}

		public bool ExisRadio(string name)
		{
			return _radios.FindEntry(name).Name != "Empty";
		}

		public IEnumerable<RadioQualityPair> GetQualitys()
		{
			foreach (var radio in _radios.Names)
			{
				var quality = _qualitys.FindEntry(radio);
				var set = new HashSet<string>();

				foreach (var item in quality.Keys)
				{
					set.Add(item.Split(QualitySplitter, StringSplitOptions.RemoveEmptyEntries)[0]);
				}

				foreach (var qualityName in set)
				{
					yield return new RadioQualityPair(radio, new QualityDatabaseInterface(qualityName, quality));
				}
			}
		}

		public void Save()
		{
			using (var steam = _environment.OpenRadio(RadioDatabaseString).Open())
			{
				_radios.Save(new StreamWriter(steam));
			}
			using (var steam = _environment.OpenRadio(QualityDatabase).Open())
			{
				_qualitys.Save(new StreamWriter(steam));
			}
		}

		public void Clear()
		{
			_radios.Clear();
			_qualitys.Clear();
		}
	}
}
