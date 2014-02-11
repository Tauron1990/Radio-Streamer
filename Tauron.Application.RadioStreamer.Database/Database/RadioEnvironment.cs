using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Tauron.Application.BassLib.Misc;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.Core.Attributes;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Database
{
	[RadioEniromentExport]
	public class RadioEnvironment : IRadioEnvironment, IDisposable
	{
		private class FileStreamReference : IStreamReference
		{
			private readonly string _path;
			private readonly FileMode _mode;
			private readonly FileShare _share;
			private readonly FileOptions _options;

			public FileStreamReference([NotNull] string path, FileMode mode, FileShare share, FileOptions options)
			{
			    _path = path;
			    _mode = mode;
			    _share = share;
			    _options = options;
			}

		    #region Implementation of IStreamReference

			public bool UseCache { get; set; }

			private WeakReference _cache;
			public Stream Open()
			{
				if (_cache != null && _cache.IsAlive)
				{
					try
					{
						var tempS = (Stream)_cache.Target;
						tempS.Position = 0;
						return tempS;
					}
					catch (ObjectDisposedException)
					{
					}
				}

				_cache = null;
				var temp = new FileStream(_path, _mode, FileAccess.ReadWrite, _share, BufferSize, _options);

				if (UseCache)
					_cache = new WeakReference(temp);

				return temp;
			}

			#endregion
		}
		private sealed class RadioSettings : TauronProfile, IRadioSettings
		{
			private sealed class SettingComponentList : Collection<SettingComponent>
			{
				private readonly RadioSettings _settings;
				private readonly Dictionary<string, Func<SettingComponent>> _factorys = new Dictionary<string, Func<SettingComponent>>();

				public SettingComponentList([NotNull] RadioSettings settings)
				{
				    _settings = settings;
				}

			    protected override void InsertItem(int index, [NotNull] SettingComponent item)
			    {
			        item.Init(_settings);
			        item.Decode();
			        base.InsertItem(index, item);
			    }

			    [CanBeNull]
			    private SettingComponent Create([NotNull] string name)
			    {
			        Func<SettingComponent> factory;
			        if (!_factorys.TryGetValue(name, out factory)) return null;
			        SettingComponent component = factory();
			        Add(component);
			        _factorys.Remove(name);
			        return component;
			    }

			    public SettingComponent this[string name]
				{
					get 
					{ 
						SettingComponent component = this.FirstOrDefault(ele => ele.Name == name) ?? Create(name);
					    return component;
					}
				}
				public void AddFactory([NotNull] string name, [NotNull] Func<SettingComponent> factory)
				{
				    _factorys[name] = factory;
				}

			    public void Save()
				{
					foreach (var item in this)
					{
						item.Encode();
					}
				}
			}

			private abstract class SettingComponent
			{
				protected class Parser
				{
					private readonly string _input;
					private int _position;

					public Parser([NotNull] string input)
					{
					    _input = input;
					}

				    public bool EndOfInput { get { return _input.Length == _position; } }

				    [NotNull]
				    public string GetNextChars(int count)
				    {
				        int pos = _position;
				        _position += count;
				        return _input.Substring(pos, count);
				    }

				    public int GetNumber()
					{
						return Int32.Parse(GetNextChars(3));
					}
				}
				protected class Coder
				{
					private readonly StringBuilder _input;

					public Coder([NotNull] StringBuilder input)
					{
					    _input = input;
					}

				    public void Encode([NotNull] IEnumerable<string> content)
				    {
				        foreach (var item in content)
				        {
				            _input.Append(GetCodedNumber(item.Length))
				                  .Append(item);
				        }
				    }

				    [NotNull]
				    private static string GetCodedNumber(int num)
				    {
				        if (num > 99) return num.ToString(CultureInfo.InvariantCulture);
				        if (num > 9) return "0" + num;
				        return "00" + num;
				    }
				}

			    [NotNull]
			    public string Name { get; private set; }

			    [NotNull]
			    private RadioSettings Settings { get; set; }

			    protected SettingComponent([NotNull] string name)
				{
				    Name = name;
				}

			    public virtual void Init([NotNull] RadioSettings settings)
			    {
			        Settings = settings;
			    }

			    public void Decode()
				{
					DecodeImpl(Settings.GetValue(Name, ""));
				}
				public void Encode()
				{
					string temp = EncodeImpl();
					if (temp == "Null") return;

					Settings.SetVaue(Name, temp);
				}

				protected abstract void DecodeImpl([NotNull] string code);
			    [NotNull]
			    protected abstract string EncodeImpl();
			}

			private sealed class RadioFavorites : SettingComponent, IRadioFavorites
			{
				public const string FavoritesKey = "Favorites";

				private HashSet<RadioFavorite> _favorites = new HashSet<RadioFavorite>();

				public RadioFavorites()
					: base(FavoritesKey)
				{
				}

				private bool _isChanged;

				protected override void DecodeImpl(string code)
				{
					var parser = new Parser(code);

					while (!parser.EndOfInput)
					{
						int first = parser.GetNumber();
						int second = parser.GetNumber();

						var favorite = new RadioFavorite(parser.GetNextChars(first),
																   parser.GetNextChars(second));
						_favorites.Add(favorite);
					}
				}
				protected override string EncodeImpl()      
				{
					if (!_isChanged) return "Null";

					var builder = new StringBuilder();
					var coder = new Coder(builder);
					var info = new string[2];

					foreach (RadioFavorite entry in _favorites)
					{
						info[0] = entry.Name;
						info[1] = entry.QualityName;
						coder.Encode(info);
					}

					return builder.ToString();
				}

				public void Remove(RadioFavorite favorite)
				{
					_isChanged = _favorites.Remove(favorite);
				}

				public void Add(RadioFavorite favorite)
				{
					_isChanged = _favorites.Add(favorite);	
				}

				public IEnumerator<RadioFavorite> GetEnumerator()
				{
					return _favorites.GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}


				public bool Contains(string radioName)
				{
					return _favorites.Any(fav => fav.Name == radioName);
				}
			}
			private sealed class EqualizerDatabaseManager : SettingComponent, IEqualizerProfileDatabase
			{
				public const string EqualizerDatabaseKey = "EqualizerDatabase";

				private ObservableDictionary<string, float[]> _profiles;

				public EqualizerDatabaseManager()
					: base(EqualizerDatabaseKey)
				{
					_profiles = new ObservableDictionary<string, float[]>();
				}

				private bool _isChanged;

				public IEnumerable<string> Profiles
				{
					get { return _profiles.Keys; }
				}
				public void SetProfil(string name, IEqualizer equlizer)
				{
					float[] bands;
					if (_profiles.TryGetValue(name, out bands))
					{
						equlizer.Band0 = bands[0];
						equlizer.Band1 = bands[1];
						equlizer.Band2 = bands[2];
						equlizer.Band3 = bands[3];
						equlizer.Band4 = bands[4];
						equlizer.Band5 = bands[5];
						equlizer.Band6 = bands[6];
						equlizer.Band7 = bands[7];
						equlizer.Band8 = bands[8];
						equlizer.Band9 = bands[9];
					}
					else
						equlizer.Enabled = false;
				}
				public void NewProfile(string name, IEqualizer equlizer)
				{
					_isChanged = true;

					var bands = new float[10];

					bands[0] = equlizer.Band0;
					bands[1] = equlizer.Band1;
					bands[2] = equlizer.Band2;
					bands[3] = equlizer.Band3;
					bands[4] = equlizer.Band4;
					bands[5] = equlizer.Band5;
					bands[6] = equlizer.Band6;
					bands[7] = equlizer.Band7;
					bands[8] = equlizer.Band8;
					bands[9] = equlizer.Band9;

					_profiles[name] = bands;
				}
				public void DeleteProfile(string name)
				{
				    if (_profiles.Remove(name))
				        _isChanged = true;
				}

				protected override string EncodeImpl()
				{
					if (!_isChanged) return "Null";

					var builder = new StringBuilder();
					var coder = new Coder(builder);
					var info = new string[11];

					foreach (var item in _profiles)
					{
						float[] bands = item.Value;
						for (int i = 0; i < bands.Length; i++)
						{
							info[i + 1] = bands[i].ToString(CultureInfo.InvariantCulture);
						}

						info[0] = item.Key;

						coder.Encode(info);
					}

					return builder.ToString();
				}
				protected override void DecodeImpl(string code)
				{
					var parser = new Parser(code);

					while (!parser.EndOfInput)
					{
						string name = parser.GetNextChars(parser.GetNumber());
						var bands = new float[10];

						for (int i = 0; i < bands.Length; i++)
						{
							bands[i] = Single.Parse(parser.GetNextChars(parser.GetNumber()));
						}

						_profiles[name] = bands;
					}
				}
			}

			public RadioSettings([NotNull] string defaultPath)
				: base(AppConstants.AppName, defaultPath)

			{
			    _components = new SettingComponentList(this);

			    _components.AddFactory(RadioFavorites.FavoritesKey, () => new RadioFavorites());
			    _components.AddFactory(EqualizerDatabaseManager.EqualizerDatabaseKey, () => new EqualizerDatabaseManager());
                Load("Default");
			}

		    private SettingComponentList _components;

			private const string IsFirstStartKey = "IsFirstStart";
		    private const string LastSprecturmKey = "LastSprecturm";

		    public string LastSprecturm
		    {
		        get
		        {
		            return GetValue(LastSprecturmKey, "Bean");
		        }
		        set
		        {
                    if(string.IsNullOrWhiteSpace(value)) return;

		            SetVaue(LastSprecturmKey, value);
		        }
		    }

		    public bool IsFirstStart
			{
				get 
				{
					try
					{
						return Boolean.Parse(GetValue(IsFirstStartKey, Boolean.TrueString));
					}
					finally 
					{
						SetVaue(IsFirstStartKey, Boolean.FalseString);
					}
				}
			}

			public IRadioFavorites Favorites
			{
				get 
				{
					return (IRadioFavorites)_components[RadioFavorites.FavoritesKey];
				}
			}

            //private const string ParallelCountQualityTasksKey = "ParallelCountQualityTasks";
            //public int ParallelCountQualityTasks
            //{
            //    get
            //    {
            //        return Int32.Parse(GetValue(ParallelCountQualityTasksKey, "5"));
            //    }
            //    set
            //    {
            //        SetVaue(ParallelCountQualityTasksKey, value);
            //    }
            //}

            //private const string LastUpdateKey = "LastUpdate";
            //private const string DownloadLocationKey = "DownloadLocation";


			public override void Save()
			{
				_components.Save();
				base.Save();
			}


			public IEqualizerProfileDatabase EqualizerDatabase
			{
				get { return (IEqualizerProfileDatabase)_components[EqualizerDatabaseManager.EqualizerDatabaseKey]; }
			}
		}

		public const string RadioStremaer = "RadioStreamer";

		public const string Temp = "Temp";
		public const string Radios = "Radios";
		public const string Other = "Other";

		public const string SettingsName = "Settings.config";

		public const int EnumError = 2;
		public const int SimpleErrorCode = 1;

		public const int BufferSize = 4096;

		//public const string RadioPlayer = "RadioPlayer";
		//public const string DataBaseUpdater = "DataBaseUpdater";
		//public const string RadioStationManager = "RadioStationManager";
		//public const string RadioConfigurationManager = "RadioConfigurationManager";
		//public const string QualityManager = "QualityManager";

        [Inject]
	    private ITauronEnviroment _enviroment;

	    [DebuggerStepThrough]
		public void Delete(string name, EnviromentLocation location)
		{
			EnsureEnviromentLocation(location).CombinePath(GetFilename(name, location)).DeleteFile();
		}
		[DebuggerStepThrough]
		public void Rename(string name, string newName, EnviromentLocation location)
		{
			string basePath = EnsureEnviromentLocation(location);
			name = GetFilename(name, location);
			newName = GetFilename(newName, location);

			File.Move(basePath.CombinePath(name), basePath.CombinePath(newName));
		}
		[DebuggerStepThrough]
		public void ClearDirectory(EnviromentLocation location)
		{
			EnsureEnviromentLocation(location).ClearDirectory();
		}

		[NotNull,DebuggerStepThrough]
		private string EnsureEnviromentLocation(EnviromentLocation loc)
		{
		    string path;
		    switch (loc)
		    {
		        case EnviromentLocation.Temp:
		            path = Temp;
		            break;
		        case EnviromentLocation.Radios:
		            path = Radios;
		            break;
		        case EnviromentLocation.Other:
		            path = Other;
		            break;
		        default:
		            path = Temp;
		            break;
		    }

		    path = Path.Combine(_enviroment.LocalApplicationData, RadioStremaer, path);
		    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		    return path;
		}

	    [DebuggerStepThrough]
		public IStreamReference Open(string name, EnviromentLocation location, FileMode mode, FileShare share, FileOptions options)
		{
			return new FileStreamReference(Path.Combine(EnsureEnviromentLocation(location), GetFilename(name, location)), mode, share, options);
        }
		[DebuggerStepThrough]
		public IStreamReference OpenTemp()
		{
			return OpenTemp(Path.GetRandomFileName());
		}
		[DebuggerStepThrough]
		public IStreamReference OpenTemp(string name) 
		{
			return Open(name, EnviromentLocation.Temp, FileMode.Create, FileShare.None, FileOptions.DeleteOnClose);
		}
		[DebuggerStepThrough]
		public IStreamReference OpenOther(string name)
		{
			return Open(name, EnviromentLocation.Other, FileMode.OpenOrCreate, FileShare.Read, FileOptions.RandomAccess);
		}
		[DebuggerStepThrough]
		public IStreamReference OpenRadio(string name)
		{
			return Open(name, EnviromentLocation.Radios, FileMode.OpenOrCreate, FileShare.ReadWrite, FileOptions.SequentialScan | FileOptions.WriteThrough);
		}

		[DebuggerStepThrough]
		public bool Exists(string name, EnviromentLocation location)
		{
			switch (location)
			{
				case EnviromentLocation.Radios:
					name += ".radio";
					break;
				case EnviromentLocation.Other:
					break;
				default:
					name += ".temp";
					break;
			}

			return File.Exists(Path.Combine(EnsureEnviromentLocation(location), name));
		}
		[DebuggerStepThrough]
		public IEnumerable<string> GetFiles(EnviromentLocation location)
		{
			switch (location)
			{
				case EnviromentLocation.Temp:
					return Directory.EnumerateFiles(EnsureEnviromentLocation(location), "*.temp").Select(Path.GetFileNameWithoutExtension);
				case EnviromentLocation.Radios:
					return Directory.EnumerateFiles(EnsureEnviromentLocation(location), "*.radio").Select(Path.GetFileNameWithoutExtension);
				case EnviromentLocation.Other:
					return Directory.EnumerateFiles(EnsureEnviromentLocation(location)).Select(Path.GetFileName);
				default:
					throw new ArgumentOutOfRangeException("location");
			}
		}
		[DebuggerStepThrough]
		public string GetFilename(string name, EnviromentLocation location)
		{
			switch (location)
			{
				case EnviromentLocation.Temp:
					return Path.Combine(EnsureEnviromentLocation(location), name + ".temp");
				case EnviromentLocation.Radios:
					return Path.Combine(EnsureEnviromentLocation(location), name + ".radio");
				case EnviromentLocation.Other:
					return Path.Combine(EnsureEnviromentLocation(location), name);
				default:
					throw new ArgumentOutOfRangeException("location");
			}
		}

	    public string DatabasePath { get; set; }

		private IRadioSettings _settings;
		public IRadioSettings OpenSettings()
		{
			lock (this)
			{
			    return _settings ?? (_settings = new RadioSettings(_enviroment.LocalApplicationData));
			}
		}

		public void Dispose()
		{
			if (_settings != null)
				_settings.Save();
		}
		public string[] DatabaseFiles { get; set; }
	}
}
