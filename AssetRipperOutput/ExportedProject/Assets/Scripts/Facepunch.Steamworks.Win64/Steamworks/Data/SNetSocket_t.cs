using System;

namespace Steamworks.Data
{
	internal struct SNetSocket_t : IEquatable<SNetSocket_t>, IComparable<SNetSocket_t>
	{
		public uint Value;

		public static implicit operator SNetSocket_t(uint value)
		{
			SNetSocket_t result = default(SNetSocket_t);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(SNetSocket_t value)
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
			return Equals((SNetSocket_t)p);
		}

		public bool Equals(SNetSocket_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(SNetSocket_t a, SNetSocket_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SNetSocket_t a, SNetSocket_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(SNetSocket_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
