namespace Steamworks.Data
{
	public struct RemotePlaySession
	{
		public uint Id { get; set; }

		public bool IsValid => Id != 0;

		public SteamId SteamId => SteamRemotePlay.Internal.GetSessionSteamID(Id);

		public string ClientName => SteamRemotePlay.Internal.GetSessionClientName(Id);

		public SteamDeviceFormFactor FormFactor => SteamRemotePlay.Internal.GetSessionClientFormFactor(Id);

		public override string ToString()
		{
			return Id.ToString();
		}

		public static implicit operator RemotePlaySession(uint value)
		{
			RemotePlaySession result = default(RemotePlaySession);
			result.Id = value;
			return result;
		}

		public static implicit operator uint(RemotePlaySession value)
		{
			return value.Id;
		}
	}
}
