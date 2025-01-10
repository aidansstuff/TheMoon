namespace Steamworks
{
	internal enum PersonaChange
	{
		Name = 1,
		Status = 2,
		ComeOnline = 4,
		GoneOffline = 8,
		GamePlayed = 0x10,
		GameServer = 0x20,
		Avatar = 0x40,
		JoinedSource = 0x80,
		LeftSource = 0x100,
		RelationshipChanged = 0x200,
		NameFirstSet = 0x400,
		Broadcast = 0x800,
		Nickname = 0x1000,
		SteamLevel = 0x2000,
		RichPresence = 0x4000
	}
}
