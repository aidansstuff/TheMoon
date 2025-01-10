using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport;

namespace Unity.Netcode.Transports.UTP
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompile]
	internal struct NetworkMetricsPipelineStage : INetworkPipelineStage
	{
		private static TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate> s_ReceiveFunction = new TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate>(Receive);

		private static TransportFunctionPointer<NetworkPipelineStage.SendDelegate> s_SendFunction = new TransportFunctionPointer<NetworkPipelineStage.SendDelegate>(Send);

		private unsafe static TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate> s_InitializeConnectionFunction = new TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate>(InitializeConnection);

		public int StaticSize => 0;

		public unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings)
		{
			return new NetworkPipelineStage(s_ReceiveFunction, s_SendFunction, s_InitializeConnectionFunction, 0, 0, 0, UnsafeUtility.SizeOf<NetworkMetricsContext>());
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.ReceiveDelegate))]
		private unsafe static void Receive(ref NetworkPipelineContext networkPipelineContext, ref InboundRecvBuffer inboundReceiveBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			NetworkMetricsContext* internalSharedProcessBuffer = (NetworkMetricsContext*)networkPipelineContext.internalSharedProcessBuffer;
			internalSharedProcessBuffer->PacketReceivedCount++;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.SendDelegate))]
		private unsafe static int Send(ref NetworkPipelineContext networkPipelineContext, ref InboundSendBuffer inboundSendBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			NetworkMetricsContext* internalSharedProcessBuffer = (NetworkMetricsContext*)networkPipelineContext.internalSharedProcessBuffer;
			internalSharedProcessBuffer->PacketSentCount++;
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.InitializeConnectionDelegate))]
		private unsafe static void InitializeConnection(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* receiveProcessBuffer, int receiveProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength)
		{
			((NetworkMetricsContext*)sharedProcessBuffer)->PacketSentCount = 0u;
			((NetworkMetricsContext*)sharedProcessBuffer)->PacketReceivedCount = 0u;
		}
	}
}
