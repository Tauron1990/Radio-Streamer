using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.Database.Formats
{
    [PublicAPI]
    public class DatabaseHelper : IEnumerable<DatabaseHelper.DatabaseEntry>
    {
        private static readonly char[] LineSplitter = { '\t' };
        private static readonly char[] MetaSplitter = { '=' };

        internal interface ICompareName
        {
            [NotNull]
            string CompareKey { get; }
        }

        internal class DatabaseArray<TElement> : IEnumerable<TElement>
            where TElement : class ,ICompareName
        {
            private class IndexEntry : IComparable<IndexEntry>
            {
                public int IndexValue;
                public int Count;
                public char Char;

                public int CompareTo([NotNull] IndexEntry other)
                {
                    return Char - other.Char;
                }

                public override string ToString()
                {
                    return Char.ToString(CultureInfo.InvariantCulture);
                }
            }

            private int _count;
            public int Count { get { return _count; } }

            private TElement[] _elements = new TElement[0];
			
            private IndexEntry[] _index = new IndexEntry[0];

            [NotNull]
            private IndexEntry FindIndex(char targetChar)
            {
                if (_index.Length == 0)
                {
                    Array.Resize(ref _index, 1);
                    _index[0] = new IndexEntry {Char = targetChar, IndexValue = 0};

                    return _index[0];
                }

                var entry = new IndexEntry {Char = targetChar};

                int pos = Array.BinarySearch(_index, entry);
                if (pos >= 0) entry = _index[pos];
                else
                {
                    int real = ~pos;
                    Array.Resize(ref _index, _index.Length + 1);
                    if (real != _index.Length) Array.Copy(_index, real, _index, real + 1, _index.Length - 1 - real);
                    _index[real] = entry;

                    pos = real;
                }

                int result = 0;
                for (int i = 0; i < pos; i++) result += _index[i].Count;

                entry.IndexValue = result;
                return entry;
            }

            public int Search([NotNull] string key)
            {
                int sourceIndex = FindIndex(key[0]).IndexValue;
                int i;

                for (i = sourceIndex; i < _elements.Length; i++)
                {
                    TElement ele = _elements[i];
                    int result = string.CompareOrdinal(ele != null ? ele.CompareKey : null, key);
                    if (result == 0) return i;
                    if (result > 0) return ~i;
                }

                return ~i;
            }

            public void Add([NotNull] TElement element)
            {
                string key = element.CompareKey;
                IndexEntry entry = FindIndex(key[0]);
                int i;
                for (i = entry.IndexValue; i < Count; i++)
                {
                    int num = string.CompareOrdinal(_elements[i].CompareKey, element.CompareKey);
                    if (num >= 0) break;
                }

                ResizeArray();

                if (i == Count)
                {
                    _elements[i] = element;
                    _count++;
                    entry.Count++;
                    return;
                }

                for (int currPos = Count; currPos >= i; currPos--)
                {
                    if (currPos == 0) break;
                    _elements[currPos] = _elements[currPos - 1];
                }

                _elements[i] = element;
                entry.Count++;
                _count++;
            }

            [CanBeNull]
            public TElement Remove([NotNull] string key)
            {
                IndexEntry entry = FindIndex(key[0]);
                int index;
                int result = 1;
                TElement ele = null;

                for (index = entry.IndexValue; index < _elements.Length; index++)
                {
                    ele = _elements[index];
                    result = string.CompareOrdinal(ele != null ? ele.CompareKey : null, key);
                    if (result == 0) break;
                    if (result > 0) return null;
                }
                if (index == 0 && result != 0) return null;

                _elements[index] = default(TElement);

                entry.Count--;
                _count--;

                MoveArray(index);

                return ele;
            }

            public TElement this[int index]
            {
                get { return _elements[index]; }
            }

            private void ResizeArray()
            {
                if (_elements.Length > Count) return;
                int num = (_elements.Length == 0) ? 4 : (_elements.Length * 2);
                if (num > 2146435071)
                {
                    num = 2146435071;
                }
                if (num < Count)
                {
                    num = Count;
                }

                if (_elements.Length < num)
                    Array.Resize(ref _elements, num);
            }
            private void MoveArray(int emptyIndex)
            {
                for (int i = emptyIndex; i < _elements.Length; i++)
                {
                    if (i + 1 == _elements.Length)
                        break;
                    _elements[i] = _elements[i + 1];
                }

                for (int i = _elements.Length - 1; i >= _count; i--)
                {
                    _elements[i] = default(TElement);
                }
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return _elements[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [PublicAPI]
        public abstract class ChangedHelper : ICompareName
        {
            protected ChangedHelper([NotNull] DatabaseHelper database)
            {
                Database = database;
            }

            [NotNull]
            public DatabaseHelper Database { get; private set; }

            [NotNull]
            protected abstract string CompareName { get; }
            private WeakCollection<IChangedHandler> _handlers;

            public void RegisterHandler([NotNull] IChangedHandler handler)
            {
                lock (this)
                {
                    if (_handlers == null)
                    {
                        _handlers = new WeakCollection<IChangedHandler>();
                        _handlers.CleanedEvent += (sender, e) => { lock (this) if (_handlers.Count == 0) _handlers = null; };
                    }
                    _handlers.Add(handler);
                }
            }

            internal void Changed(ChangeType type, [NotNull] string oldContent, [NotNull] string content)
            {
                if (_handlers == null) return;

                foreach (var handler in _handlers) handler.Changed(type, CompareName, oldContent, content);
            }

            string ICompareName.CompareKey
            {
                get { return CompareName; }
            }
        }
        public class Metadata : ChangedHelper
        {
            public Metadata([NotNull] DatabaseHelper database)
                : base(database)
            {
            }

            protected override string CompareName { get { return _key; } }

            private string _key;

            [NotNull]
            public string Key
            {
                get { return _key; }
                set
                {
                    Changed(ChangeType.MetaKey, Interlocked.Exchange(ref _key, value), value);
                    Interlocked.Exchange(ref Database._isDirty, 1);
                }
            }

            private string _value;

            [NotNull]
            public string Value
            {
                get { return _value; }
                set
                {
                    Changed(ChangeType.MetaValue, Interlocked.Exchange(ref _value, value), value);
                    Interlocked.Exchange(ref Database._isDirty, 1);
                }
            }

            public override string ToString()
            {
                return _key  + ":" + _value;
            }
        }
        public class DatabaseEntry : ChangedHelper, IEnumerable<Metadata>
        {
            internal class DatabaseEntryAcessor
            {
                private readonly DatabaseEntry _entry;

                [NotNull]
                public DatabaseArray<Metadata> MetadataAcc
                {
                    get { return _entry._metadata; }
                }

                [NotNull]
                public HashSet<string> MetaNames
                {
                    get { return _entry._metaNames; }
                }

                public DatabaseEntryAcessor([NotNull] DatabaseEntry entry)
                {
                    _entry = entry;
                }
            }

            protected override string CompareName { get { return _name; } }

            private string _name;

            [NotNull]
            public string Name
            {
                get { return _name; }
                set
                {
                    Changed(ChangeType.Name, Interlocked.Exchange(ref _name, value), value);
                    Interlocked.Exchange(ref Database._isDirty, 1);
                }
            }

            private DatabaseArray<Metadata> _metadata = new DatabaseArray<Metadata>();
            private HashSet<string> _metaNames = new HashSet<string>();
            [NotNull]
            public ReaderWriterLockSlim Locker { get; private set; }

            public DatabaseEntry([NotNull] DatabaseHelper database)
                : base(database)
            {
                Locker = new ReaderWriterLockSlim();
            }

            [NotNull]
            public IEnumerable<string> Keys
            {
                get { return new ReadOnlyEnumerator<string>(_metaNames); }
            }

            [NotNull]
            public Metadata FindMetadata([NotNull] string key)
            {
                try
                {
                    Locker.EnterReadLock();
                    return FindMeatadataNonBlock(key);
                }
                finally
                {
                    Locker.ExitReadLock();
                }
            }

            [NotNull]
            private Metadata FindMeatadataNonBlock([NotNull] string name)
            {
                int index = _metadata.Search(name);
                return index < 0 ? new Metadata(Database) {Key = "Empty", Value = "Empty"} : _metadata[index];
            }

            [NotNull]
            public Metadata AddMetadata([NotNull] string key, out bool added)
            {
                if (String.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

                try
                {
                    Locker.EnterWriteLock();
                    if (_metaNames.Add(key))
                    {
                        added = true;
                        var meta = new Metadata(Database) {Key = key};

                        _metadata.Add(meta);

                        Interlocked.Exchange(ref Database._isDirty, 1);

                        return meta;
                    }
                    else
                    {
                        added = false;
                        return FindMeatadataNonBlock(key);
                    }
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }

            public void RemoveMetadata([NotNull] string name)
            {
                try
                {
                    Locker.EnterWriteLock();

                    var meta = _metadata.Remove(name);
                    _metaNames.Remove(name);

                    if(meta != null)
                        meta.Changed(ChangeType.Deleted, name, string.Empty);

                    Interlocked.Exchange(ref Database._isDirty, 1);
                }
                finally
                {
                    Locker.ExitWriteLock();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<Metadata> GetEnumerator()
            {
                Locker.EnterReadLock();
                try
                {
                    foreach (var item in _metadata)
                    {
                        yield return item;
                    }
                }
                finally
                {
                    Locker.ExitReadLock();
                }
            }

            public override string ToString()
            {
                return _name;
            }
        }

        private static class Coder
        {
            private class EscapePart
            {
                public char Escaped;
                public string Sequence;

                public override string ToString()
                {
                    return Escaped + " " + Sequence;
                }
            }
            private static readonly EscapePart[] Parts =
            {
                new EscapePart {Escaped = '\r', Sequence = "001"},
                new EscapePart {Escaped = '\t', Sequence = "002"},
                new EscapePart {Escaped = '\n', Sequence = "003"},
                new EscapePart {Escaped = ':', Sequence = "004"}
            };

            private const string EscapeString = @"\\";

            [CanBeNull]
            private static EscapePart GetPartforChar(char @char)
            {
                char toSearch;

                switch (@char)
                {
                    case '\r':
                        toSearch = @char;
                        break;
                    case '\t':
                        toSearch = @char;
                        break;
                    case '\n':
                        toSearch = @char;
                        break;
                    case ':':
                        toSearch = @char;
                        break;
                    default:
                        toSearch = 'a';
                        break;
                }

                return toSearch == 'a' ? null : Parts.First(p => p.Escaped == toSearch);
            }

            [CanBeNull]
            private static EscapePart GetPartforSequence([NotNull] string @char)
            {
                string toSearch;

                switch (@char)
                {
                    case "001":
                        toSearch = @char;
                        break;
                    case "002":
                        toSearch = @char;
                        break;
                    case "003":
                        toSearch = @char;
                        break;
                    case "004":
                        toSearch = @char;
                        break;
                    default:
                        toSearch = "a";
                        break;
                }

                return toSearch == "a" ? null : Parts.First(p => p.Sequence == toSearch);
            }

            [NotNull]
            public static string Encode([NotNull] IEnumerable<char> toEncode)
            {
                var builder = new StringBuilder();
                foreach (var @char in toEncode)
                {
                    EscapePart part = GetPartforChar(@char);
                    if (part == null) builder.Append(@char);
                    else builder.Append(EscapeString + part.Sequence);
                }

                return builder.ToString();
            }

            [NotNull]
            public static string Decode([NotNull] IEnumerable<char> toDecode)
            {
                var builder = new StringBuilder();

                bool flag = false;
                bool flag2 = false;
                int pos = 0;
                string sequence = String.Empty;
                string temp = String.Empty;

                foreach (var @char in toDecode)
                {
                    if (flag2)
                    {
                        sequence += @char;
                        pos++;

                        if (pos != 3) continue;
		                
                        EscapePart part = GetPartforSequence(sequence);
                        if (part == null) builder.Append(temp).Append(sequence);
                        else builder.Append(part.Escaped);

                        flag = false;
                        flag2 = false;
                        pos = 0;
                        sequence = String.Empty;
                        temp = String.Empty;
                    }
                    else if (flag)
                    {
                        flag2 = @char == '\\';
                        if (!flag2)
                        {
                            builder.Append("\\").Append(@char);
                            flag = false;
                        }
                        else temp += @char;
                    }
                    else
                    {
                        flag = @char == '\\';

                        if (!flag)
                        {
                            builder.Append(@char);
                        }
                        else temp += @char;
                    }
                }

                return builder.ToString();
            }
        }

        private DatabaseArray<DatabaseEntry> _entrys = new DatabaseArray<DatabaseEntry>();
        public int DatabasElements { get { return _entrys.Count; } }
        private HashSet<string> _databaseNames = new HashSet<string>();
        private int _isDirty;
        public bool IsDirty { get { return _isDirty == 1; } }

        [NotNull]
        public ReaderWriterLockSlim Locker { get; private set; }

        public DatabaseHelper([NotNull] IEnumerable<string> lines)
        {
            Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _databaseNames = new HashSet<string>();
            foreach (var line in lines)
            {
                if(line == "End") return;
                var tempentry = new DatabaseEntry(this);
                var acsessor = new DatabaseEntry.DatabaseEntryAcessor(tempentry);
                string[] pairs = line.Split(LineSplitter, StringSplitOptions.RemoveEmptyEntries);
                tempentry.Name = DecodeIfRequied(pairs[0]);
                if (!_databaseNames.Add(tempentry.Name)) continue;

                _entrys.Add(tempentry);

                var data = acsessor.MetadataAcc;
                foreach (var pair in pairs.Skip(1))
                {
                    string[] pairValue = pair.Split(MetaSplitter, StringSplitOptions.RemoveEmptyEntries);

                    var meta = new Metadata(this) {Key = DecodeIfRequied(pairValue[0])};
                    if (pairValue.Length == 2) meta.Value = DecodeIfRequied(pairValue[1]);

                    if (acsessor.MetaNames.Add(meta.Key)) data.Add(meta);
                }
            }
        }

        [NotNull]
        public IEnumerable<string> Names
        {
            get { return new ReadOnlyEnumerator<string>(_databaseNames); }
        }

        [NotNull]
        public DatabaseEntry FindEntry([NotNull] string name)
        {
            try
            {
                Locker.EnterReadLock();
                return FindEntryNonBlock(name);
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }

        [NotNull]
        private DatabaseEntry FindEntryNonBlock([NotNull] string name)
        {
            int index = _entrys.Search(name);
            return index >= 0 ? _entrys[index] : new DatabaseEntry(this) {Name = "Empty"};
        }

        [NotNull]
        public DatabaseEntry AddEntry([NotNull] string name, out bool added)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            try
            {
                Locker.EnterWriteLock();
                if (_databaseNames.Add(name))
                {
                    added = true;
                    var entry = new DatabaseEntry(this) {Name = name};

                    _entrys.Add(entry);

                    Interlocked.Exchange(ref _isDirty, 1);

                    return entry;
                }
                else
                {
                    added = false;
                    return FindEntryNonBlock(name);
                }
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        public void RemoveEntry([NotNull] string name)
        {
            try
            {
                Locker.EnterWriteLock();

                var ent = _entrys.Remove(name);
                _databaseNames.Remove(name);
                if(ent != null)
                    ent.Changed(ChangeType.Deleted, name, string.Empty);

                Interlocked.Exchange(ref _isDirty, 1);
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Save([NotNull] TextWriter writer)
        {
            if (_isDirty == 0) return;
            Interlocked.Exchange(ref _isDirty, 0);

            foreach (DatabaseEntry entry in this)
            {
                writer.Write(EncodeIfRequied(entry.Name));

                foreach (Metadata data in entry) writer.Write("\t{0}={1}", EncodeIfRequied(data.Key), EncodeIfRequied(data.Value));
                writer.Write(writer.NewLine);
            }

            writer.Write("End");
        }

        public void Clear()
        {
            Locker.EnterWriteLock();
            try
            {
                if (_entrys.Count == 0) return;

                _entrys = new DatabaseArray<DatabaseEntry>();
                _databaseNames.Clear();

                Interlocked.Exchange(ref _isDirty, 1);
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        [NotNull]
        private static string EncodeIfRequied([CanBeNull] string name)
        {
            if (name == null) return String.Empty;

            if (!name.Contains("\r") && !name.Contains("\t") && !name.Contains("\n") && !name.StartsWith(":")) return name;

            var temp = Coder.Encode(name);
            return ":" + temp;
        }

        [NotNull]
        private static string DecodeIfRequied([NotNull] string name)
        {
            return !name.StartsWith(":") ? name : Coder.Decode(name.Substring(1));
        }

        public IEnumerator<DatabaseEntry> GetEnumerator()
        {
            Locker.EnterReadLock();
            try
            {
                foreach (var item in _entrys)
                {
                    yield return item;
                }
            }
            finally
            {
                Locker.ExitReadLock();
            }
        }
    }
}