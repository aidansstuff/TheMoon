using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamAPICallCompleted_t : ICallbackData
	{
		internal ulong AsyncCall;

		internal int Callback;

		internal uint ParamCount;

		public static int _datasize = Marshal.SizeOf(typeof(SteamAPICallCompleted_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamAPICallCompleted;
	}
}
