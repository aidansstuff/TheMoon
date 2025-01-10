using System;

namespace Steamworks.Data
{
	internal struct HSteamPipe : IEquatable<HSteamPipe>, IComparable<HSteamPipe>
	{
		public int Value;

		public static implicit operator HSteamPipe(int value)
		{
			HSteamPipe result = default(HSteamPipe);
			result.Value = value;
			return result;
		}

		public static implicit operator int(HSteamPipe value)
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
			return Equals((HSteamPipe)p);
		}

		public bool Equals(HSteamPipe p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(HSteamPipe a, HSteamPipe b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HSteamPipe a, HSteamPipe b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(HSteamPipe other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
