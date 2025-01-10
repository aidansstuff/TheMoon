using System;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamRemotePlay : SteamClientClass<SteamRemotePlay>
	{
		internal static ISteamRemotePlay Internal => SteamClientClass<SteamRemotePlay>.Interface as ISteamRemotePlay;

		public static int SessionCount => (int)Internal.GetSessionCount();

		public static event Action<RemotePlaySession> OnSessionConnected;

		public static event Action<RemotePlaySession> OnSessionDisconnected;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamRemotePlay(server));
			InstallEvents(server);
		}

		internal void InstallEvents(bool server)
		{
			Dispatch.Install(delegate(SteamRemotePlaySessionConnected_t x)
			{
				SteamRemotePlay.OnSessionConnected?.Invoke(x.SessionID);
			}, server);
			Dispatch.Install(delegate(SteamRemotePlaySessionDisconnected_t x)
			{
				SteamRemotePlay.OnSessionDisconnected?.Invoke(x.SessionID);
			}, server);
		}

		public static RemotePlaySession GetSession(int index)
		{
			return Internal.GetSessionID(index).Value;
		}

		public static bool SendInvite(SteamId steamid)
		{
			return Internal.BSendRemotePlayTogetherInvite(steamid);
		}
	}
}
