using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MicroTxnAuthorizationResponse_t : ICallbackData
	{
		internal uint AppID;

		internal ulong OrderID;

		internal byte Authorized;

		public static int _datasize = Marshal.SizeOf(typeof(MicroTxnAuthorizationResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MicroTxnAuthorizationResponse;
	}
}
