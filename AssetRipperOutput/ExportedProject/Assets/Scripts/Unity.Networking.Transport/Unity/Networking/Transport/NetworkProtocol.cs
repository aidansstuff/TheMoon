using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	internal struct NetworkProtocol
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ComputePacketOverheadDelegate(ref NetworkDriver.Connection connection, out int payloadOffset);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ProcessReceiveDelegate(IntPtr stream, ref NetworkInterfaceEndPoint address, int size, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData, ref ProcessPacketCommand command);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ProcessSendDelegate(ref NetworkDriver.Connection connection, bool hasPipeline, ref NetworkSendInterface sendInterface, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ProcessSendConnectionAcceptDelegate(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ConnectDelegate(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void DisconnectDelegate(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ProcessSendPingDelegate(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ProcessSendPongDelegate(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void UpdateDelegate(long updateTime, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData);

		public TransportFunctionPointer<ComputePacketOverheadDelegate> ComputePacketOverhead;

		public TransportFunctionPointer<ProcessReceiveDelegate> ProcessReceive;

		public TransportFunctionPointer<ProcessSendDelegate> ProcessSend;

		public TransportFunctionPointer<ProcessSendConnectionAcceptDelegate> ProcessSendConnectionAccept;

		public TransportFunctionPointer<ConnectDelegate> Connect;

		public TransportFunctionPointer<DisconnectDelegate> Disconnect;

		public TransportFunctionPointer<ProcessSendPingDelegate> ProcessSendPing;

		public TransportFunctionPointer<ProcessSendPongDelegate> ProcessSendPong;

		public TransportFunctionPointer<UpdateDelegate> Update;

		[NativeDisableUnsafePtrRestriction]
		public IntPtr UserData;

		public int MaxHeaderSize;

		public int MaxFooterSize;

		public bool NeedsUpdate;

		public int PaddingSize => MaxHeaderSize + MaxFooterSize;

		public NetworkProtocol(TransportFunctionPointer<ComputePacketOverheadDelegate> computePacketOverhead, TransportFunctionPointer<ProcessReceiveDelegate> processReceive, TransportFunctionPointer<ProcessSendDelegate> processSend, TransportFunctionPointer<ProcessSendConnectionAcceptDelegate> processSendConnectionAccept, TransportFunctionPointer<ConnectDelegate> connect, TransportFunctionPointer<DisconnectDelegate> disconnect, TransportFunctionPointer<ProcessSendPingDelegate> processSendPing, TransportFunctionPointer<ProcessSendPongDelegate> processSendPong, TransportFunctionPointer<UpdateDelegate> update, bool needsUpdate, IntPtr userData, int maxHeaderSize, int maxFooterSize)
		{
			ComputePacketOverhead = computePacketOverhead;
			ProcessReceive = processReceive;
			ProcessSend = processSend;
			ProcessSendConnectionAccept = processSendConnectionAccept;
			Connect = connect;
			Disconnect = disconnect;
			ProcessSendPing = processSendPing;
			ProcessSendPong = processSendPong;
			Update = update;
			NeedsUpdate = needsUpdate;
			UserData = userData;
			MaxHeaderSize = maxHeaderSize;
			MaxFooterSize = maxFooterSize;
		}
	}
}
