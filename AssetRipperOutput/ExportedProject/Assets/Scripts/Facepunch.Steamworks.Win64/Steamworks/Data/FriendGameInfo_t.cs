using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct FriendGameInfo_t
	{
		internal GameId GameID;

		internal uint GameIP;

		internal ushort GamePort;

		internal ushort QueryPort;

		internal ulong SteamIDLobby;
	}
}
