using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	public struct NetworkSendInterface
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int BeginSendMessageDelegate(out NetworkInterfaceSendHandle handle, IntPtr userData, int requiredPayloadSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int EndSendMessageDelegate(ref NetworkInterfaceSendHandle handle, ref NetworkInterfaceEndPoint address, IntPtr userData, ref NetworkSendQueueHandle sendQueue);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void AbortSendMessageDelegate(ref NetworkInterfaceSendHandle handle, IntPtr userData);

		public TransportFunctionPointer<BeginSendMessageDelegate> BeginSendMessage;

		public TransportFunctionPointer<EndSendMessageDelegate> EndSendMessage;

		public TransportFunctionPointer<AbortSendMessageDelegate> AbortSendMessage;

		[NativeDisableUnsafePtrRestriction]
		public IntPtr UserData;
	}
}
