using System;

namespace Steamworks.Data
{
	internal struct SteamNetworkingPOPID : IEquatable<SteamNetworkingPOPID>, IComparable<SteamNetworkingPOPID>
	{
		public uint Value;

		public static implicit operator SteamNetworkingPOPID(uint value)
		{
			SteamNetworkingPOPID result = default(SteamNetworkingPOPID);
			result.Value = value;
			return result;
		}

		public static implicit operator uint(SteamNetworkingPOPID value)
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
			return Equals((SteamNetworkingPOPID)p);
		}

		public bool Equals(SteamNetworkingPOPID p)
		{
			return p.Value == Value;
		}

		public static bool operator ==(SteamNetworkingPOPID a, SteamNetworkingPOPID b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SteamNetworkingPOPID a, SteamNetworkingPOPID b)
		{
			return !a.Equals(b);
		}

		public int CompareTo(SteamNetworkingPOPID other)
		{
			return Value.CompareTo(other.Value);
		}
	}
}
