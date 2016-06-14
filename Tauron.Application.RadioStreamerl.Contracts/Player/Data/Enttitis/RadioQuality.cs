using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Enttitis
{
    [PublicAPI]
    public struct RadioQuality : IEquatable<RadioQuality>
    {
        public const string MetaName = "Name";
        public const string MetaUrl = "Url";
        public const string SourceMetaUrl = "SourceUrl";

        [NotNull]
        public string Name
        {
            get { return Metadata[MetaName]; }
            set { Metadata[MetaName] = value; }
        }

        [NotNull]
        public string Url
        {
            get { return Metadata[MetaUrl]; }
            set { Metadata[MetaUrl] = value; }
        }

        [NotNull]
        public string SourceUrl
        {
            get { return Metadata[SourceMetaUrl]; }
            set { Metadata[SourceMetaUrl] = value; }
        }

        [CanBeNull]
        public Metadatascope Metadata { get; private set; }
        public bool IsEmpty => Metadata == null;

        public RadioQuality([NotNull] Metadatascope metaData, [NotNull] string name)
            : this()
        {
            if (!metaData.IsRadio) throw new InvalidOperationException();
            
            Metadata = metaData.GetQuality(name);
        }

        public RadioQuality([NotNull] Metadatascope metadatascope)
            : this()
        {
            if(!metadatascope.IsQuality) throw new InvalidOperationException();

            Metadata = metadatascope;
        }

        public static RadioQuality CreateNew([NotNull]Metadatascope metaData, [NotNull] string name, [NotNull] string url)
        {
            if (!metaData.IsRadio) throw new InvalidOperationException();

            return new RadioQuality(metaData.CreateQuality(name, url));
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (RadioQuality)) return false;
            return Equals((RadioQuality) obj);
        }

        public bool Equals(RadioQuality other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(RadioQuality left, RadioQuality right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RadioQuality left, RadioQuality right)
        {
            return !left.Equals(right);
        }
    }
}