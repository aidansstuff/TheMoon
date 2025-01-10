using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamAppUninstalled_t : ICallbackData
	{
		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(SteamAppUninstalled_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamAppUninstalled;
	}
}
