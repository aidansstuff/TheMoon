using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompile]
	public struct FragmentationPipelineStage : INetworkPipelineStage
	{
		public struct FragContext
		{
			public int startIndex;

			public int endIndex;

			public int sequence;

			public bool packetError;
		}

		internal struct FragSharedContext
		{
			public int PayloadCapacity;

			public int MaxMessageSize;
		}

		[Flags]
		private enum FragFlags
		{
			First = 0x8000,
			Last = 0x4000,
			SeqMask = 0x3FFF
		}

		private const int FragHeaderCapacity = 2;

		private static TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate> ReceiveFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate>(Receive);

		private static TransportFunctionPointer<NetworkPipelineStage.SendDelegate> SendFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.SendDelegate>(Send);

		private unsafe static TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate> InitializeConnectionFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate>(InitializeConnection);

		public int StaticSize => UnsafeUtility.SizeOf<FragSharedContext>();

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.SendDelegate))]
		private unsafe static int Send(ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			FragContext* internalProcessBuffer = (FragContext*)ctx.internalProcessBuffer;
			byte* ptr = ctx.internalProcessBuffer + sizeof(FragContext);
			FragSharedContext* staticInstanceBuffer = (FragSharedContext*)ctx.staticInstanceBuffer;
			FragFlags fragFlags = FragFlags.First;
			_ = ctx.header.Capacity;
			int num = systemHeaderSize + 1 + 8;
			int num2 = staticInstanceBuffer->MaxMessageSize - num - inboundBuffer.headerPadding;
			int num3 = num2 - ctx.accumulatedHeaderCapacity;
			if (internalProcessBuffer->endIndex > internalProcessBuffer->startIndex)
			{
				if (inboundBuffer.bufferLength != 0)
				{
					return -3;
				}
				fragFlags &= ~FragFlags.First;
				int num4 = internalProcessBuffer->endIndex - internalProcessBuffer->startIndex;
				if (num4 > num2)
				{
					num4 = num2;
				}
				inboundBuffer.bufferWithHeaders = (inboundBuffer.buffer = ptr + internalProcessBuffer->startIndex) - inboundBuffer.headerPadding;
				inboundBuffer.bufferLength = num4;
				inboundBuffer.bufferWithHeadersLength = num4 + inboundBuffer.headerPadding;
				internalProcessBuffer->startIndex += num4;
			}
			else if (inboundBuffer.bufferLength > num3)
			{
				int payloadCapacity = staticInstanceBuffer->PayloadCapacity;
				int num5 = inboundBuffer.bufferLength - num3;
				byte* source = inboundBuffer.buffer + num3;
				if (num5 + inboundBuffer.headerPadding > payloadCapacity)
				{
					return -4;
				}
				UnsafeUtility.MemCpy(ptr + inboundBuffer.headerPadding, source, num5);
				internalProcessBuffer->startIndex = inboundBuffer.headerPadding;
				internalProcessBuffer->endIndex = num5 + inboundBuffer.headerPadding;
				inboundBuffer.bufferWithHeadersLength -= num5;
				inboundBuffer.bufferLength -= num5;
			}
			if (internalProcessBuffer->endIndex > internalProcessBuffer->startIndex)
			{
				requests |= NetworkPipelineStage.Requests.Resume;
			}
			else
			{
				fragFlags |= FragFlags.Last;
			}
			int num6 = (internalProcessBuffer->sequence++ & 0x3FFF) | (int)fragFlags;
			ctx.header.WriteShort((short)num6);
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.ReceiveDelegate))]
		private unsafe static void Receive(ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			FragContext* internalProcessBuffer = (FragContext*)ctx.internalProcessBuffer;
			byte* ptr = ctx.internalProcessBuffer + sizeof(FragContext);
			FragSharedContext* staticInstanceBuffer = (FragSharedContext*)ctx.staticInstanceBuffer;
			NativeArray<byte> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(inboundBuffer.buffer, inboundBuffer.bufferLength, Allocator.Invalid);
			short num = new DataStreamReader(array).ReadShort();
			int num2 = num & 0x3FFF;
			FragFlags fragFlags = (FragFlags)(num & -16384);
			inboundBuffer = inboundBuffer.Slice(2);
			int num3 = internalProcessBuffer->sequence;
			bool num4 = (fragFlags & FragFlags.First) != 0;
			bool flag = (fragFlags & FragFlags.Last) != 0;
			if (num4)
			{
				num3 = num2;
				internalProcessBuffer->packetError = false;
				internalProcessBuffer->endIndex = 0;
			}
			if (num2 != num3)
			{
				internalProcessBuffer->packetError = true;
				internalProcessBuffer->endIndex = 0;
			}
			if (!internalProcessBuffer->packetError)
			{
				if (!flag || internalProcessBuffer->endIndex > 0)
				{
					if (internalProcessBuffer->endIndex + inboundBuffer.bufferLength > staticInstanceBuffer->PayloadCapacity)
					{
						Debug.LogError("Fragmentation capacity exceeded");
						return;
					}
					UnsafeUtility.MemCpy(ptr + internalProcessBuffer->endIndex, inboundBuffer.buffer, inboundBuffer.bufferLength);
					internalProcessBuffer->endIndex += inboundBuffer.bufferLength;
				}
				if (flag && internalProcessBuffer->endIndex > 0)
				{
					inboundBuffer = new InboundRecvBuffer
					{
						buffer = ptr,
						bufferLength = internalProcessBuffer->endIndex
					};
				}
			}
			if (!flag || internalProcessBuffer->packetError)
			{
				inboundBuffer = default(InboundRecvBuffer);
			}
			internalProcessBuffer->sequence = (num2 + 1) & 0x3FFF;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.InitializeConnectionDelegate))]
		private unsafe static void InitializeConnection(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* recvProcessBuffer, int recvProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength)
		{
		}

		public unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings)
		{
			((FragSharedContext*)staticInstanceBuffer)->PayloadCapacity = settings.GetFragmentationStageParameters().PayloadCapacity;
			((FragSharedContext*)staticInstanceBuffer)->MaxMessageSize = settings.GetNetworkConfigParameters().maxMessageSize;
			return new NetworkPipelineStage(ReceiveFunctionPointer, SendFunctionPointer, InitializeConnectionFunctionPointer, sizeof(FragContext) + ((FragSharedContext*)staticInstanceBuffer)->PayloadCapacity, sizeof(FragContext) + ((FragSharedContext*)staticInstanceBuffer)->PayloadCapacity, 2, 0, ((FragSharedContext*)staticInstanceBuffer)->PayloadCapacity);
		}
	}
}
