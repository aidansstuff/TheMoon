using System;

namespace Steamworks.Data
{
	internal struct HTTPRequestHandle : IEquatable<HTTPRequestHandle>, IComparable<HTTPRequestHandle>
	{
		public uint Value;

		public static implicit operator HTTPRequestHandle(uint value)
		{
			HTTPRequestHandle result = default(HTTPRequestHandle);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(HTTPRequestHandle value)
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
			return Equals((HTTPRequestHandle)p);
		}

		public bool Equals(HTTPRequestHandle p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(HTTPRequestHandle a, HTTPRequestHandle b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HTTPRequestHandle a, HTTPRequestHandle b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(HTTPRequestHandle other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
