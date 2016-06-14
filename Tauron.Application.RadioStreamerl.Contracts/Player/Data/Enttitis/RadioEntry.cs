using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
	public struct RadioEntry
	{
		public const string MetaName = "Name";
		public const string MetaGenre = "Genre";
		public const string MetaCountry = "Country";
		public const string MetaLanguage = "Language";
		public const string MetaDescription = "Description";
		public const string MetaIntegrated = "Integrated";
        public const string MetaScript = "Script";

        [CanBeNull]
        public IQualityQuery Qualitys { get; private set; }

        [NotNull]
        public string Script
        {
            get { return Metadata[MetaScript]; }
            set { Metadata[MetaScript] = value; }
        }

        [NotNull]
        public string Name
        {
            get { return Metadata[MetaName]; }
            set { Metadata[MetaName] = value; }
        }

        [NotNull]
        public string Genre
        {
            get { return Metadata[MetaGenre]; }
            set { Metadata[MetaGenre] = value; }
        }

        [NotNull]
        public string Country
        {
            get { return Metadata[MetaCountry]; }
            set { Metadata[MetaCountry] = value; }
        }

        [NotNull]
        public string Language
        {
            get { return Metadata[MetaLanguage]; }
            set { Metadata[MetaLanguage] = value; }
        }

        [NotNull]
        public string Description
        {
            get { return Metadata[MetaDescription]; }
            set { Metadata[MetaDescription] = value; }
        }

        public bool Integrated { get { return bool.Parse(Metadata[MetaIntegrated]); } set { Metadata[MetaIntegrated] = value.ToString(); } }
		public bool IsEmpty => Metadata == null;

        [CanBeNull]
        public Metadatascope Metadata { get; private set; }

		public RadioEntry([NotNull] Metadatascope metadata)
			: this()
		{
		    if (!metadata.IsRadio) throw new InvalidOperationException();

		    Metadata = metadata;
		}

        public void SetQuality([NotNull] IQualityQuery query)
        {
            if (query == null) throw new ArgumentNullException();
            if (Qualitys != null) throw new InvalidOperationException();
            Qualitys = query;
        }

        public override string ToString()
		{
			return Name;
		}

        //[NotNull]
        //public static string BoxDataValue([NotNull] IEnumerable<string> data)
        //{
        //    var builder = new StringBuilder();
        //    foreach (var entry in data) builder.Append(entry).Append('\t');

        //    return builder.ToString();
        //}

        //[CanBeNull]
        //public static string[] UnBoxData([CanBeNull] string data)
        //{
        //    return data == null ? null : data.Split(new[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
        //}

        //public static int GetId([NotNull] IEnumerable<string> data)
        //{
        //    return data.Aggregate(0, (current, ent) => current ^ ent.GetHashCode());
        //}
	}
}
