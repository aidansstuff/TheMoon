using System;

namespace Steamworks.Data
{
	internal struct TxnID_t : IEquatable<TxnID_t>, IComparable<TxnID_t>
	{
		public ulong Value;

		public static implicit operator TxnID_t(ulong value)
		{
			TxnID_t result = default(TxnID_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(TxnID_t value)
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
			return Equals((TxnID_t)p);
		}

		public bool Equals(TxnID_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(TxnID_t a, TxnID_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(TxnID_t a, TxnID_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(TxnID_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
