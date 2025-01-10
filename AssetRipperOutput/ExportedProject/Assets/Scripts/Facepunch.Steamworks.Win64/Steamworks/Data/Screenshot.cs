namespace Steamworks.Data
{
	public struct Screenshot
	{
		internal ScreenshotHandle Value;

		public bool TagUser(SteamId user)
		{
			return SteamScreenshots.Internal.TagUser(Value, user);
		}

		public bool SetLocation(string location)
		{
			return SteamScreenshots.Internal.SetLocation(Value, location);
		}

		public bool TagPublishedFile(PublishedFileId file)
		{
			return SteamScreenshots.Internal.TagPublishedFile(Value, file);
		}
	}
}
