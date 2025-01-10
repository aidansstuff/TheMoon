using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct IPCFailure_t : ICallbackData
	{
		internal enum EFailureType
		{
			FlushedCallbackQueue = 0,
			PipeFail = 1
		}

		internal byte FailureType;

		public static int _datasize = Marshal.SizeOf(typeof(IPCFailure_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.IPCFailure;
	}
}
