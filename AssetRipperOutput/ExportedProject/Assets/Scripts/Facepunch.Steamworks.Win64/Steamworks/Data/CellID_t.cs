using System;

namespace Steamworks.Data
{
	internal struct CellID_t : IEquatable<CellID_t>, IComparable<CellID_t>
	{
		public uint Value;

		public static implicit operator CellID_t(uint value)
		{
			CellID_t result = default(CellID_t);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(CellID_t value)
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
			return Equals((CellID_t)p);
		}

		public bool Equals(CellID_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(CellID_t a, CellID_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(CellID_t a, CellID_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(CellID_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
