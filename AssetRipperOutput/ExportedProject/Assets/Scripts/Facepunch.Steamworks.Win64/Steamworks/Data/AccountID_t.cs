using System;

namespace Steamworks.Data
{
	internal struct AccountID_t : IEquatable<AccountID_t>, IComparable<AccountID_t>
	{
		public uint Value;

		public static implicit operator AccountID_t(uint value)
		{
			AccountID_t result = default(AccountID_t);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(AccountID_t value)
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
			return Equals((AccountID_t)p);
		}

		public bool Equals(AccountID_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(AccountID_t a, AccountID_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AccountID_t a, AccountID_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(AccountID_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
