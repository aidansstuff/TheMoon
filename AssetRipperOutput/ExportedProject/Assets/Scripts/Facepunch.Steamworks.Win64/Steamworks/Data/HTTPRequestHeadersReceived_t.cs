using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTTPRequestHeadersReceived_t : ICallbackData
	{
		internal uint Request;

		internal ulong ContextValue;

		public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestHeadersReceived_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTTPRequestHeadersReceived;
	}
}
