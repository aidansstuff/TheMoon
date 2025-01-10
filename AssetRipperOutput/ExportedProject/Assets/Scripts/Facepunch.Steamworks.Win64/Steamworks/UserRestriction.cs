namespace Steamworks
{
	internal enum UserRestriction
	{
		None = 0,
		Unknown = 1,
		AnyChat = 2,
		VoiceChat = 4,
		GroupChat = 8,
		Rating = 0x10,
		GameInvites = 0x20,
		Trading = 0x40
	}
}
