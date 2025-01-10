using System;

namespace Steamworks.Data
{
	internal struct HAuthTicket : IEquatable<HAuthTicket>, IComparable<HAuthTicket>
	{
		public uint Value;

		public static implicit operator HAuthTicket(uint value)
		{
			HAuthTicket result = default(HAuthTicket);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(HAuthTicket value)
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
			return Equals((HAuthTicket)p);
		}

		public bool Equals(HAuthTicket p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(HAuthTicket a, HAuthTicket b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HAuthTicket a, HAuthTicket b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(HAuthTicket other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
