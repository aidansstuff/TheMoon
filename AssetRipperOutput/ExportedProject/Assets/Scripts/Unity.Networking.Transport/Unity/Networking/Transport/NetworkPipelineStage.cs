using System;
using System.Runtime.InteropServices;

namespace Unity.Networking.Transport
{
	public struct NetworkPipelineStage
	{
		[Flags]
		public enum Requests
		{
			None = 0,
			Resume = 1,
			Update = 2,
			SendUpdate = 4,
			Error = 8
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void ReceiveDelegate(ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref Requests requests, int systemHeadersSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int SendDelegate(ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref Requests requests, int systemHeadersSize);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public unsafe delegate void InitializeConnectionDelegate(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* recvProcessBuffer, int recvProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength);

		public TransportFunctionPointer<ReceiveDelegate> Receive;

		public TransportFunctionPointer<SendDelegate> Send;

		public TransportFunctionPointer<InitializeConnectionDelegate> InitializeConnection;

		public readonly int ReceiveCapacity;

		public readonly int SendCapacity;

		public readonly int HeaderCapacity;

		public readonly int SharedStateCapacity;

		public readonly int PayloadCapacity;

		internal int StaticStateStart;

		internal int StaticStateCapcity;

		public NetworkPipelineStage(TransportFunctionPointer<ReceiveDelegate> Receive, TransportFunctionPointer<SendDelegate> Send, TransportFunctionPointer<InitializeConnectionDelegate> InitializeConnection, int ReceiveCapacity, int SendCapacity, int HeaderCapacity, int SharedStateCapacity, int PayloadCapacity = 0)
		{
			this.Receive = Receive;
			this.Send = Send;
			this.InitializeConnection = InitializeConnection;
			this.ReceiveCapacity = ReceiveCapacity;
			this.SendCapacity = SendCapacity;
			this.HeaderCapacity = HeaderCapacity;
			this.SharedStateCapacity = SharedStateCapacity;
			this.PayloadCapacity = PayloadCapacity;
			StaticStateStart = (StaticStateCapcity = 0);
		}
	}
}
