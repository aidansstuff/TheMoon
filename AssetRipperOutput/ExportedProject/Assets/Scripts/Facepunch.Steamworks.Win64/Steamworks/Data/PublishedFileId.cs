using System;

namespace Steamworks.Data
{
	public struct PublishedFileId : IEquatable<PublishedFileId>, IComparable<PublishedFileId>
	{
		public ulong Value;

		public static implicit operator PublishedFileId(ulong value)
		{
			PublishedFileId result = default(PublishedFileId);
			result.Value = value;
			return result;
		}

		public static implicit operator ulong(PublishedFileId value)
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
			return Equals((PublishedFileId)p);
		}

		public bool Equals(PublishedFileId p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(PublishedFileId a, PublishedFileId b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PublishedFileId a, PublishedFileId b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(PublishedFileId other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
