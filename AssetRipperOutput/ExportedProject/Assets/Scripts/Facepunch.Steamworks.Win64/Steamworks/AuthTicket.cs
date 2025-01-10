using System;

namespace Steamworks
{
	public class AuthTicket : IDisposable
	{
		public byte[] Data;

		public uint Handle;

		public void Cancel()
		{
			if (Handle != 0)
			{
				SteamUser.Internal.CancelAuthTicket(Handle);
			}
			Handle = 0u;
			Data = null;
		}

		public void Dispose()
		{
			Cancel();
		}
	}
}
