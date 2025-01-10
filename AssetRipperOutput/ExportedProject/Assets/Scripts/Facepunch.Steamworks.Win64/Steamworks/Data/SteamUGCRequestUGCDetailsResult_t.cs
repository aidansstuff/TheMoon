using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamUGCRequestUGCDetailsResult_t : ICallbackData
	{
		internal SteamUGCDetails_t Details;

		[MarshalAs(UnmanagedType.I1)]
		internal bool CachedData;

		public static int _datasize = Marshal.SizeOf(typeof(SteamUGCRequestUGCDetailsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamUGCRequestUGCDetailsResult;
	}
}
