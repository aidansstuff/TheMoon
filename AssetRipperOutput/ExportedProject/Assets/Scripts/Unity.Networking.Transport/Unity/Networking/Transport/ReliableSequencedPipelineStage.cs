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
	public struct ReliableSequencedPipelineStage : INetworkPipelineStage
	{
		private static TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate> ReceiveFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.ReceiveDelegate>(Receive);

		private static TransportFunctionPointer<NetworkPipelineStage.SendDelegate> SendFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.SendDelegate>(Send);

		private unsafe static TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate> InitializeConnectionFunctionPointer = new TransportFunctionPointer<NetworkPipelineStage.InitializeConnectionDelegate>(InitializeConnection);

		public int StaticSize => UnsafeUtility.SizeOf<ReliableUtility.Parameters>();

		public unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings)
		{
			ReliableUtility.Parameters reliableStageParameters = settings.GetReliableStageParameters();
			UnsafeUtility.MemCpy(staticInstanceBuffer, &reliableStageParameters, UnsafeUtility.SizeOf<ReliableUtility.Parameters>());
			return new NetworkPipelineStage(ReceiveFunctionPointer, SendFunctionPointer, InitializeConnectionFunctionPointer, ReliableUtility.ProcessCapacityNeeded(reliableStageParameters), ReliableUtility.ProcessCapacityNeeded(reliableStageParameters), ReliableUtility.PacketHeaderWireSize(reliableStageParameters.WindowSize), ReliableUtility.SharedCapacityNeeded(reliableStageParameters));
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.ReceiveDelegate))]
		private unsafe static void Receive(ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			requests = NetworkPipelineStage.Requests.SendUpdate;
			bool needsResume = false;
			ReliableUtility.ReliableHeader header = default(ReliableUtility.ReliableHeader);
			InboundRecvBuffer inboundRecvBuffer = default(InboundRecvBuffer);
			ReliableUtility.Context* internalProcessBuffer = (ReliableUtility.Context*)ctx.internalProcessBuffer;
			ReliableUtility.SharedContext* internalSharedProcessBuffer = (ReliableUtility.SharedContext*)ctx.internalSharedProcessBuffer;
			internalSharedProcessBuffer->errorCode = (ReliableUtility.ErrorCodes)0;
			if (internalProcessBuffer->Resume == -1)
			{
				if (inboundBuffer.bufferLength <= 0)
				{
					inboundBuffer = inboundRecvBuffer;
					return;
				}
				NativeArray<byte> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(inboundBuffer.buffer, inboundBuffer.bufferLength, Allocator.Invalid);
				new DataStreamReader(array).ReadBytes((byte*)(&header), ReliableUtility.PacketHeaderWireSize(ctx));
				if (header.Type == 1)
				{
					ReliableUtility.ReadAckPacket(ctx, header);
					inboundBuffer = default(InboundRecvBuffer);
					return;
				}
				int num = ReliableUtility.Read(ctx, header);
				if (num >= 0)
				{
					ushort num2 = (ushort)(internalProcessBuffer->Delivered + 1);
					if (num == num2)
					{
						internalProcessBuffer->Delivered = num;
						inboundRecvBuffer = inboundBuffer.Slice(ReliableUtility.PacketHeaderWireSize(ctx));
						if (needsResume = SequenceHelpers.GreaterThan16((ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence, (ushort)num))
						{
							internalProcessBuffer->Resume = (ushort)(num + 1);
						}
					}
					else
					{
						ReliableUtility.SetPacket(ctx.internalProcessBuffer, num, inboundBuffer.Slice(ReliableUtility.PacketHeaderWireSize(ctx)));
						inboundRecvBuffer = ReliableUtility.ResumeReceive(ctx, internalProcessBuffer->Delivered + 1, ref needsResume);
					}
				}
				else if (num == -2)
				{
					internalSharedProcessBuffer->DuplicatesSinceLastAck++;
				}
			}
			else
			{
				inboundRecvBuffer = ReliableUtility.ResumeReceive(ctx, internalProcessBuffer->Resume, ref needsResume);
			}
			if (needsResume)
			{
				requests |= NetworkPipelineStage.Requests.Resume;
			}
			inboundBuffer = inboundRecvBuffer;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.SendDelegate))]
		private unsafe static int Send(ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
		{
			requests = NetworkPipelineStage.Requests.Update;
			ReliableUtility.ReliableHeader header = default(ReliableUtility.ReliableHeader);
			ReliableUtility.Context* internalProcessBuffer = (ReliableUtility.Context*)ctx.internalProcessBuffer;
			ReliableUtility.ReleaseAcknowledgedPackets(ctx);
			if (inboundBuffer.bufferLength > 0)
			{
				internalProcessBuffer->LastSentTime = ctx.timestamp;
				if (ReliableUtility.Write(ctx, inboundBuffer, ref header) < 0)
				{
					inboundBuffer = default(InboundSendBuffer);
					requests |= NetworkPipelineStage.Requests.Error;
					return -5;
				}
				ctx.header.Clear();
				ctx.header.WriteBytes((byte*)(&header), ReliableUtility.PacketHeaderWireSize(ctx));
				internalProcessBuffer->PreviousTimestamp = ctx.timestamp;
				return 0;
			}
			if (internalProcessBuffer->Resume != -1)
			{
				internalProcessBuffer->LastSentTime = ctx.timestamp;
				bool needsResume = false;
				inboundBuffer = ReliableUtility.ResumeSend(ctx, out header, ref needsResume);
				internalProcessBuffer->Resume = ReliableUtility.GetNextSendResumeSequence(ctx);
				if (internalProcessBuffer->Resume != -1)
				{
					requests |= NetworkPipelineStage.Requests.Resume;
				}
				ctx.header.Clear();
				ctx.header.WriteBytes((byte*)(&header), ReliableUtility.PacketHeaderWireSize(ctx));
				internalProcessBuffer->PreviousTimestamp = ctx.timestamp;
				return 0;
			}
			internalProcessBuffer->Resume = ReliableUtility.GetNextSendResumeSequence(ctx);
			if (internalProcessBuffer->Resume != -1)
			{
				requests |= NetworkPipelineStage.Requests.Resume;
			}
			if (ReliableUtility.ShouldSendAck(ctx))
			{
				internalProcessBuffer->LastSentTime = ctx.timestamp;
				ReliableUtility.WriteAckPacket(ctx, ref header);
				ctx.header.WriteBytes((byte*)(&header), ReliableUtility.PacketHeaderWireSize(ctx));
				internalProcessBuffer->PreviousTimestamp = ctx.timestamp;
				inboundBuffer.bufferWithHeadersLength = inboundBuffer.headerPadding + 1;
				inboundBuffer.bufferWithHeaders = (byte*)UnsafeUtility.Malloc(inboundBuffer.bufferWithHeadersLength, 8, Allocator.Temp);
				inboundBuffer.SetBufferFrombufferWithHeaders();
				return 0;
			}
			internalProcessBuffer->PreviousTimestamp = ctx.timestamp;
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkPipelineStage.InitializeConnectionDelegate))]
		private unsafe static void InitializeConnection(byte* staticInstanceBuffer, int staticInstanceBufferLength, byte* sendProcessBuffer, int sendProcessBufferLength, byte* recvProcessBuffer, int recvProcessBufferLength, byte* sharedProcessBuffer, int sharedProcessBufferLength)
		{
			ReliableUtility.Parameters param = default(ReliableUtility.Parameters);
			UnsafeUtility.MemCpy(&param, staticInstanceBuffer, UnsafeUtility.SizeOf<ReliableUtility.Parameters>());
			if (sharedProcessBufferLength >= ReliableUtility.SharedCapacityNeeded(param) && sendProcessBufferLength + recvProcessBufferLength >= ReliableUtility.ProcessCapacityNeeded(param) * 2)
			{
				ReliableUtility.InitializeContext(sharedProcessBuffer, sharedProcessBufferLength, sendProcessBuffer, sendProcessBufferLength, recvProcessBuffer, recvProcessBufferLength, param);
			}
		}
	}
}
