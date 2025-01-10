using System;

namespace Steamworks.Data
{
	internal struct SiteId_t : IEquatable<SiteId_t>, IComparable<SiteId_t>
	{
		public ulong Value;

		public static implicit operator SiteId_t(ulong value)
		{
			SiteId_t result = default(SiteId_t);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(SiteId_t value)
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
			return Equals((SiteId_t)p);
		}

		public bool Equals(SiteId_t p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(SiteId_t a, SiteId_t b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SiteId_t a, SiteId_t b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(SiteId_t other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
