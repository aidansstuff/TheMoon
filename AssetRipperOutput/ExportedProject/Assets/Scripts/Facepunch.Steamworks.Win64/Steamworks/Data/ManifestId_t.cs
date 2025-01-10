using System;

namespace Steamworks.Data
{
	internal struct ManifestId_t : IEquatable<ManifestId_t>, IComparable<ManifestId_t>
	{
		public ulong Value;

		public static implicit operator ManifestId_t(ulong value)
		{
			ManifestId_t result = default(ManifestId_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(ManifestId_t value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object p)
		{
			return Equals((ManifestId_t)p);
		}

		public bool Equals(ManifestId_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(ManifestId_t a, ManifestId_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ManifestId_t a, ManifestId_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(ManifestId_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
