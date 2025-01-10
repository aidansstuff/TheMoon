namespace Steamworks
{
	public class SteamMatchmakingServers : SteamClientClass<SteamMatchmakingServers>
	{
		internal static ISteamMatchmakingServers Internal => SteamClientClass<SteamMatchmakingServers>.Interface as ISteamMatchmakingServers;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamMatchmakingServers(server));
		}
	}
}
