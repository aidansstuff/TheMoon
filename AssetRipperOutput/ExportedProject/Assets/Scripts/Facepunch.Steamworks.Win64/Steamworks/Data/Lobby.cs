using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Lobby
	{
		public SteamId Id { get; internal set; }

		public int MemberCount => SteamMatchmaking.Internal.GetNumLobbyMembers(Id);

		public IEnumerable<Friend> Members
		{
			get
			{
				for (int i = 0; i < MemberCount; i++)
				{
					yield return new Friend(SteamMatchmaking.Internal.GetLobbyMemberByIndex(Id, i));
				}
			}
		}

		public IEnumerable<KeyValuePair<string, string>> Data
		{
			get
			{
				int cnt = SteamMatchmaking.Internal.GetLobbyDataCount(Id);
				for (int i = 0; i < cnt; i++)
				{
					if (SteamMatchmaking.Internal.GetLobbyDataByIndex(Id, i, out var a, out var b))
					{
						yield return new KeyValuePair<string, string>(a, b);
					}
					a = null;
					b = null;
				}
			}
		}

		public int MaxMembers
		{
			get
			{
				return SteamMatchmaking.Internal.GetLobbyMemberLimit(Id);
			}
			set
			{
				SteamMatchmaking.Internal.SetLobbyMemberLimit(Id, value);
			}
		}

		public Friend Owner
		{
			get
			{
				return new Friend(SteamMatchmaking.Internal.GetLobbyOwner(Id));
			}
			set
			{
				SteamMatchmaking.Internal.SetLobbyOwner(Id, value.Id);
			}
		}

		public Lobby(SteamId id)
		{
			Id = id;
		}

		public async Task<RoomEnter> Join()
		{
			LobbyEnter_t? result = await SteamMatchmaking.Internal.JoinLobby(Id);
			if (!result.HasValue)
			{
				return RoomEnter.Error;
			}
			return (RoomEnter)result.Value.EChatRoomEnterResponse;
		}

		public void Leave()
		{
			SteamMatchmaking.Internal.LeaveLobby(Id);
		}

		public bool InviteFriend(SteamId steamid)
		{
			return SteamMatchmaking.Internal.InviteUserToLobby(Id, steamid);
		}

		public string GetData(string key)
		{
			return SteamMatchmaking.Internal.GetLobbyData(Id, key);
		}

		public bool SetData(string key, string value)
		{
			if (key.Length > 255)
			{
				throw new ArgumentException("Key should be < 255 chars", "key");
			}
			if (value.Length > 8192)
			{
				throw new ArgumentException("Value should be < 8192 chars", "key");
			}
			return SteamMatchmaking.Internal.SetLobbyData(Id, key, value);
		}

		public bool DeleteData(string key)
		{
			return SteamMatchmaking.Internal.DeleteLobbyData(Id, key);
		}

		public string GetMemberData(Friend member, string key)
		{
			return SteamMatchmaking.Internal.GetLobbyMemberData(Id, member.Id, key);
		}

		public void SetMemberData(string key, string value)
		{
			SteamMatchmaking.Internal.SetLobbyMemberData(Id, key, value);
		}

		public bool SendChatString(string message)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(message);
			return SendChatBytes(bytes);
		}

		internal unsafe bool SendChatBytes(byte[] data)
		{
			fixed (byte* ptr = data)
			{
				return SteamMatchmaking.Internal.SendLobbyChatMsg(Id, (IntPtr)ptr, data.Length);
			}
		}

		public bool Refresh()
		{
			return SteamMatchmaking.Internal.RequestLobbyData(Id);
		}

		public bool SetPublic()
		{
			return SteamMatchmaking.Internal.SetLobbyType(Id, LobbyType.Public);
		}

		public bool SetPrivate()
		{
			return SteamMatchmaking.Internal.SetLobbyType(Id, LobbyType.Private);
		}

		public bool SetInvisible()
		{
			return SteamMatchmaking.Internal.SetLobbyType(Id, LobbyType.Invisible);
		}

		public bool SetFriendsOnly()
		{
			return SteamMatchmaking.Internal.SetLobbyType(Id, LobbyType.FriendsOnly);
		}

		public bool SetJoinable(bool b)
		{
			return SteamMatchmaking.Internal.SetLobbyJoinable(Id, b);
		}

		public void SetGameServer(SteamId steamServer)
		{
			if (!steamServer.IsValid)
			{
				throw new ArgumentException("SteamId for server is invalid");
			}
			SteamMatchmaking.Internal.SetLobbyGameServer(Id, 0u, 0, steamServer);
		}

		public void SetGameServer(string ip, ushort port)
		{
			if (!IPAddress.TryParse(ip, out var address))
			{
				throw new ArgumentException("IP address for server is invalid");
			}
			SteamMatchmaking.Internal.SetLobbyGameServer(Id, address.IpToInt32(), port, default(SteamId));
		}

		public bool GetGameServer(ref uint ip, ref ushort port, ref SteamId serverId)
		{
			return SteamMatchmaking.Internal.GetLobbyGameServer(Id, ref ip, ref port, ref serverId);
		}

		public bool IsOwnedBy(SteamId k)
		{
			return (ulong)Owner.Id == (ulong)k;
		}
	}
}
