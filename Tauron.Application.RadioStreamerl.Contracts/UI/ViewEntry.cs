using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.UI
{
    [PublicAPI]
    public sealed class ViewEntry : IEquatable<ViewEntry>
    {
        public bool Equals(ViewEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ViewEntry left, ViewEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ViewEntry left, ViewEntry right)
        {
            return !Equals(left, right);
        }

        public bool IsDefault { get; private set; }

        [NotNull]
        public string Id { get; }

        [NotNull]
        public string DisplayName { get; private set; }

        public ViewEntry([NotNull] string id, [NotNull] string displayName, bool isDefault = false)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (displayName == null) throw new ArgumentNullException(nameof(displayName));

            Id = id;
            DisplayName = displayName;
            IsDefault = isDefault;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ViewEntry && Equals((ViewEntry) obj);
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
