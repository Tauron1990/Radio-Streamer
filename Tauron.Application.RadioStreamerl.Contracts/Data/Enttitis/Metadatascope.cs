using System;
using System.Collections.Generic;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
    public sealed class Metadatascope : IEnumerable<string>
    {
        private readonly IDatabaseInterface _databaseInterface;

        public Metadatascope([NotNull] IDatabaseInterface databaseInterface)
        {
            _databaseInterface = databaseInterface;
        }

        public string this[string key]
        {
            get { return _databaseInterface.Read(key); }
            set
            {
                bool flag;
                _databaseInterface.Write(key, value, out flag);
            }
        }
        public void Delete([NotNull] string key, bool quality)
        {
            if (quality)
            {
                var temp = _databaseInterface as IRadioDatabaseInterface;
                if (temp == null) throw new InvalidOperationException();
                temp.DeleteQuality(key);
            }
            else _databaseInterface.Delete(key);
        }

        public bool IsRadio { get { return _databaseInterface is IRadioDatabaseInterface; } }
        public bool IsQuality { get { return _databaseInterface is IQualityDatabaseInterface;  } }

        [NotNull]
        public Metadatascope GetQuality([NotNull] string name)
        {
            var temp = _databaseInterface as IRadioDatabaseInterface;
            if (temp == null) throw new InvalidOperationException();
            return new Metadatascope(temp.GetQuality(name));
        }

        [NotNull]
        public IEnumerable<string> GetQualitys()
        {
            var temp = _databaseInterface as IRadioDatabaseInterface;
            if (temp == null) throw new InvalidOperationException();
            return temp.GetQualitys();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _databaseInterface.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}