using System;

namespace Steamworks.Data
{
	internal struct HServerQuery : IEquatable<HServerQuery>, IComparable<HServerQuery>
	{
		public int Value;

		public static implicit operator HServerQuery(int value)
		{
			HServerQuery result = default(HServerQuery);
			result.Value = value;
			return result;
		}

		public static implicit operator int(HServerQuery value)
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
			return Equals((HServerQuery)p);
		}

		public bool Equals(HServerQuery p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(HServerQuery a, HServerQuery b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HServerQuery a, HServerQuery b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(HServerQuery other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
