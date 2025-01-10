using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTTPRequestCompleted_t : ICallbackData
	{
		internal uint Request;

		internal ulong ContextValue;

		[MarshalAs(UnmanagedType.I1)]
		internal bool RequestSuccessful;

		internal HTTPStatusCode StatusCode;

		internal uint BodySize;

		public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestCompleted_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTTPRequestCompleted;
	}
}
