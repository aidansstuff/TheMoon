using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSPolicyResponse_t : ICallbackData
	{
		internal byte Secure;

		public static int _datasize = Marshal.SizeOf(typeof(GSPolicyResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSPolicyResponse;
	}
}
