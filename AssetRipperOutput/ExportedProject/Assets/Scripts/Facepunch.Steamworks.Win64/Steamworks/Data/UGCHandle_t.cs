using System;

namespace Steamworks.Data
{
	internal struct UGCHandle_t : IEquatable<UGCHandle_t>, IComparable<UGCHandle_t>
	{
		public ulong Value;

		public static implicit operator UGCHandle_t(ulong value)
		{
			UGCHandle_t result = default(UGCHandle_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(UGCHandle_t value)
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
			return Equals((UGCHandle_t)p);
		}

		public bool Equals(UGCHandle_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(UGCHandle_t a, UGCHandle_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(UGCHandle_t a, UGCHandle_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(UGCHandle_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
