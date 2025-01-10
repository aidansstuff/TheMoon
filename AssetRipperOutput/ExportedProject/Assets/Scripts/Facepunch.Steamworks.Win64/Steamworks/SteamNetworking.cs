using System;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamNetworking : SteamSharedClass<SteamNetworking>
	{
		public static Action<SteamId> OnP2PSessionRequest;

		public static Action<SteamId, P2PSessionError> OnP2PConnectionFailed;

		internal static ISteamNetworking Internal => SteamSharedClass<SteamNetworking>.Interface as ISteamNetworking;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamNetworking(server));
			InstallEvents(server);
		}

		internal static void InstallEvents(bool server)
		{
			Dispatch.Install(delegate(P2PSessionRequest_t x)
			{
				OnP2PSessionRequest?.Invoke(x.SteamIDRemote);
			}, server);
			Dispatch.Install(delegate(P2PSessionConnectFail_t x)
			{
				OnP2PConnectionFailed?.Invoke(x.SteamIDRemote, (P2PSessionError)x.P2PSessionError);
			}, server);
		}

		public static bool AcceptP2PSessionWithUser(SteamId user)
		{
			return Internal.AcceptP2PSessionWithUser(user);
		}

		public static bool AllowP2PPacketRelay(bool allow)
		{
			return Internal.AllowP2PPacketRelay(allow);
		}

		public static bool CloseP2PSessionWithUser(SteamId user)
		{
			return Internal.CloseP2PSessionWithUser(user);
		}

		public static bool IsP2PPacketAvailable(int channel = 0)
		{
			uint pcubMsgSize = 0u;
			return Internal.IsP2PPacketAvailable(ref pcubMsgSize, channel);
		}

		public unsafe static P2Packet? ReadP2PPacket(int channel = 0)
		{
			uint pcubMsgSize = 0u;
			if (!Internal.IsP2PPacketAvailable(ref pcubMsgSize, channel))
			{
				return null;
			}
			byte[] array = Helpers.TakeBuffer((int)pcubMsgSize);
			fixed (byte* ptr = array)
			{
				SteamId psteamIDRemote = 1uL;
				if (!Internal.ReadP2PPacket((IntPtr)ptr, (uint)array.Length, ref pcubMsgSize, ref psteamIDRemote, channel) || pcubMsgSize == 0)
				{
					return null;
				}
				byte[] array2 = new byte[pcubMsgSize];
				Array.Copy(array, 0L, array2, 0L, pcubMsgSize);
				P2Packet value = default(P2Packet);
				value.SteamId = psteamIDRemote;
				value.Data = array2;
				return value;
			}
		}

		public unsafe static bool ReadP2PPacket(byte[] buffer, ref uint size, ref SteamId steamid, int channel = 0)
		{
			fixed (byte* ptr = buffer)
			{
				return Internal.ReadP2PPacket((IntPtr)ptr, (uint)buffer.Length, ref size, ref steamid, channel);
			}
		}

		public unsafe static bool ReadP2PPacket(byte* buffer, uint cbuf, ref uint size, ref SteamId steamid, int channel = 0)
		{
			return Internal.ReadP2PPacket((IntPtr)buffer, cbuf, ref size, ref steamid, channel);
		}

		public unsafe static bool SendP2PPacket(SteamId steamid, byte[] data, int length = -1, int nChannel = 0, P2PSend sendType = P2PSend.Reliable)
		{
			if (length <= 0)
			{
				length = data.Length;
			}
			fixed (byte* ptr = data)
			{
				return Internal.SendP2PPacket(steamid, (IntPtr)ptr, (uint)length, sendType, nChannel);
			}
		}

		public unsafe static bool SendP2PPacket(SteamId steamid, byte* data, uint length, int nChannel = 1, P2PSend sendType = P2PSend.Reliable)
		{
			return Internal.SendP2PPacket(steamid, (IntPtr)data, length, sendType, nChannel);
		}
	}
}
