using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamInventoryFullUpdate_t : ICallbackData
	{
		internal int Handle;

		public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryFullUpdate_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamInventoryFullUpdate;
	}
}
