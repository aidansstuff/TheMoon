using System;

namespace Steamworks.Data
{
	internal struct SNetListenSocket_t : IEquatable<SNetListenSocket_t>, IComparable<SNetListenSocket_t>
	{
		public uint Value;

		public static implicit operator SNetListenSocket_t(uint value)
		{
			SNetListenSocket_t result = default(SNetListenSocket_t);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(SNetListenSocket_t value)
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
			return Equals((SNetListenSocket_t)p);
		}

		public bool Equals(SNetListenSocket_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(SNetListenSocket_t a, SNetListenSocket_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SNetListenSocket_t a, SNetListenSocket_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(SNetListenSocket_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
