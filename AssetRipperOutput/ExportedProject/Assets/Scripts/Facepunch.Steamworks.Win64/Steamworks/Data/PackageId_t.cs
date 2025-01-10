using System;

namespace Steamworks.Data
{
	internal struct PackageId_t : IEquatable<PackageId_t>, IComparable<PackageId_t>
	{
		public uint Value;

		public static implicit operator PackageId_t(uint value)
		{
			PackageId_t result = default(PackageId_t);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(PackageId_t value)
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
			return Equals((PackageId_t)p);
		}

		public bool Equals(PackageId_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(PackageId_t a, PackageId_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PackageId_t a, PackageId_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(PackageId_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
