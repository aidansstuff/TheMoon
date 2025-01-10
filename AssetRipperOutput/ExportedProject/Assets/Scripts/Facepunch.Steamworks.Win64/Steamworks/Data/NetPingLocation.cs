using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	public struct NetPingLocation
	{
		public static NetPingLocation? TryParseFromString(string str)
		{
			NetPingLocation result = default(NetPingLocation);
			if (!SteamNetworkingUtils.Internal.ParsePingLocationString(str, ref result))
			{
				return null;
			}
			return result;
		}

		public override string ToString()
		{
			SteamNetworkingUtils.Internal.ConvertPingLocationToString(ref this, out var pszBuf);
			return pszBuf;
		}

		public int EstimatePingTo(NetPingLocation target)
		{
			return SteamNetworkingUtils.Internal.EstimatePingTimeBetweenTwoLocations(ref this, ref target);
		}
	}
}
