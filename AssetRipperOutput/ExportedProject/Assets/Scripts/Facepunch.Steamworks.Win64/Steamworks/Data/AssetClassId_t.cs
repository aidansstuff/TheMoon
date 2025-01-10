using System;

namespace Steamworks.Data
{
	internal struct AssetClassId_t : IEquatable<AssetClassId_t>, IComparable<AssetClassId_t>
	{
		public ulong Value;

		public static implicit operator AssetClassId_t(ulong value)
		{
			AssetClassId_t result = default(AssetClassId_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(AssetClassId_t value)
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
			return Equals((AssetClassId_t)p);
		}

		public bool Equals(AssetClassId_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(AssetClassId_t a, AssetClassId_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AssetClassId_t a, AssetClassId_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(AssetClassId_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
