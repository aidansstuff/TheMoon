using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTTPRequestDataReceived_t : ICallbackData
	{
		internal uint Request;

		internal ulong ContextValue;

		internal uint COffset;

		internal uint CBytesReceived;

		public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestDataReceived_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTTPRequestDataReceived;
	}
}
