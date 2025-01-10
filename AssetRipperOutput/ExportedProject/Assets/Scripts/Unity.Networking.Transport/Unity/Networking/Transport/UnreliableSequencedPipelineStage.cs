using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompile]
	public struct UnreliableSequencedPipelineStage : INetworkPipelineStage
	{
		private static TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate> ReceiveFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate>(Receive);

		private static TransportFunctionPointer<NetworkPipelineStage.SendDelegate> SendFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.SendDelegate>(Send);

		private unsafe static TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate> InitializeConnectionFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate>(InitializeConnection);

		public int StaticSize => 0;

		public unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings)
		{
			return new NetworkPipelineStage(ReceiveFunctionPointer, SendFunctionPointer, InitializeConnectionFunctionPointer, UnsafeUtility.SizeOf<int>(), UnsafeUtility.SizeOf<int>(), UnsafeUtility.SizeOf<ushort>(), 0);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.ReceiveDelegate))]
		private unsafe static void Receive(ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			NativeArray<byte> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(inboundBuffer.buffer, inboundBuffer.bufferLength, Allocator.Invalid);
			DataStreamReader dataStreamReader = new DataStreamReader(array);
			int* internalProcessBuffer = (int*)ctx.internalProcessBuffer;
			ushort num = dataStreamReader.ReadUShort();
			if (SequenceHelpers.GreaterThan16(num, (ushort)(*internalProcessBuffer)))
			{
				*internalProcessBuffer = num;
				inboundBuffer = inboundBuffer.Slice(2);
			}
			else
			{
				inboundBuffer = default(InboundRecvBuffer);
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.SendDelegate))]
		private unsafe static int Send(ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			int* internalProcessBuffer = (int*)ctx.internalProcessBuffer;
			ctx.header.WriteUShort((ushort)(*internalProcessBuffer));
			(*internalProcessBuffer)++;
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.InitializeConnectionDelegate))]
		private unsafe static void InitializeConnection(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* recvProcessBuffer, int recvProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength)
		{
			if (recvProcessBufferLength > 0)
			{
				*(int*)recvProcessBuffer = -1;
			}
		}
	}
}
