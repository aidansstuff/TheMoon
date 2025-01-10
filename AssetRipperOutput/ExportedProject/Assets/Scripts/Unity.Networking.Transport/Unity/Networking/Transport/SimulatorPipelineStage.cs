using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompile]
	public struct SimulatorPipelineStage : INetworkPipelineStage
	{
		private static TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate> ReceiveFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate>(Receive);

		private static TransportFunctionPointer<NetworkPipelineStage.SendDelegate> SendFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.SendDelegate>(Send);

		private unsafe static TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate> InitializeConnectionFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate>(InitializeConnection);

		public int StaticSize => UnsafeUtility.SizeOf<SimulatorUtility.Parameters>();

		public unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings)
		{
			SimulatorUtility.Parameters simulatorStageParameters = settings.GetSimulatorStageParameters();
			UnsafeUtility.MemCpy(staticInstanceBuffer, &simulatorStageParameters, UnsafeUtility.SizeOf<SimulatorUtility.Parameters>());
			return new NetworkPipelineStage(ReceiveFunctionPointer, SendFunctionPointer, InitializeConnectionFunctionPointer, simulatorStageParameters.MaxPacketCount * (simulatorStageParameters.MaxPacketSize + UnsafeUtility.SizeOf<SimulatorUtility.DelayedPacket>()), 0, 0, UnsafeUtility.SizeOf<SimulatorUtility.Context>());
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.InitializeConnectionDelegate))]
		private unsafe static void InitializeConnection(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* recvProcessBuffer, int recvProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength)
		{
			SimulatorUtility.Parameters param = default(SimulatorUtility.Parameters);
			UnsafeUtility.MemCpy(&param, staticInstanceBuffer, UnsafeUtility.SizeOf<SimulatorUtility.Parameters>());
			if (sharedProcessBufferLength >= UnsafeUtility.SizeOf<SimulatorUtility.Parameters>())
			{
				SimulatorUtility.InitializeContext(param, sharedProcessBuffer);
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.SendDelegate))]
		private static int Send(ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.ReceiveDelegate))]
		private unsafe static void Receive(ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			SimulatorUtility.Context* internalSharedProcessBuffer = (SimulatorUtility.Context*)ctx.internalSharedProcessBuffer;
			SimulatorUtility.Parameters staticInstanceBuffer = *(SimulatorUtility.Parameters*)ctx.staticInstanceBuffer;
			SimulatorUtility simulatorUtility = new SimulatorUtility(staticInstanceBuffer.MaxPacketCount, staticInstanceBuffer.MaxPacketSize, staticInstanceBuffer.PacketDelayMs, staticInstanceBuffer.PacketJitterMs);
			if (inboundBuffer.bufferLength > staticInstanceBuffer.MaxPacketSize)
			{
				return;
			}
			long timestamp = ctx.timestamp;
			if (inboundBuffer.bufferLength > 0)
			{
				internalSharedProcessBuffer->PacketCount++;
				if (simulatorUtility.ShouldDropPacket(internalSharedProcessBuffer, staticInstanceBuffer, timestamp))
				{
					internalSharedProcessBuffer->PacketDropCount++;
					inboundBuffer = default(InboundRecvBuffer);
					return;
				}
				InboundSendBuffer inboundBuffer2 = default(InboundSendBuffer);
				inboundBuffer2.bufferWithHeaders = inboundBuffer.buffer;
				inboundBuffer2.bufferWithHeadersLength = inboundBuffer.bufferLength;
				inboundBuffer2.buffer = inboundBuffer.buffer;
				inboundBuffer2.bufferLength = inboundBuffer.bufferLength;
				inboundBuffer2.headerPadding = 0;
				if (internalSharedProcessBuffer->PacketDelayMs == 0 || !simulatorUtility.DelayPacket(ref ctx, inboundBuffer2, ref requests, timestamp))
				{
					return;
				}
			}
			InboundSendBuffer delayedPacket = default(InboundSendBuffer);
			if (simulatorUtility.GetDelayedPacket(ref ctx, ref delayedPacket, ref requests, timestamp))
			{
				inboundBuffer.buffer = delayedPacket.bufferWithHeaders;
				inboundBuffer.bufferLength = delayedPacket.bufferWithHeadersLength;
			}
			else
			{
				inboundBuffer = default(InboundRecvBuffer);
			}
		}
	}
}
