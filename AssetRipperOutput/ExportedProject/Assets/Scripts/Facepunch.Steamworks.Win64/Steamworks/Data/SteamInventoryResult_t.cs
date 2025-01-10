using System;

namespace Steamworks.Data
{
	internal struct SteamInventoryResult_t : IEquatable<SteamInventoryResult_t>, IComparable<SteamInventoryResult_t>
	{
		public int Value;

		public static implicit operator SteamInventoryResult_t(int value)
		{
			SteamInventoryResult_t result = default(SteamInventoryResult_t);
			result.Value = value;
			return result;
		}

		public static implicit operator int(SteamInventoryResult_t value)
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
			return Equals((SteamInventoryResult_t)p);
		}

		public bool Equals(SteamInventoryResult_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(SteamInventoryResult_t a, SteamInventoryResult_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SteamInventoryResult_t a, SteamInventoryResult_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(SteamInventoryResult_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
