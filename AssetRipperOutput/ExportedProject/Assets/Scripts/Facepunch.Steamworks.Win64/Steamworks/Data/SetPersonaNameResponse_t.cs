using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SetPersonaNameResponse_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool Success;

		[MarshalAs(UnmanagedType.I1)]
		internal bool LocalSuccess;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(SetPersonaNameResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SetPersonaNameResponse;
	}
}
