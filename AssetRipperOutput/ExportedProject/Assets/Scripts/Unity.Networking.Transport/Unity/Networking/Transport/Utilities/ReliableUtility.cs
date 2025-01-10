using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Networking.Transport.Utilities
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ReliableUtility
	{
		public struct Statistics
		{
			public int PacketsReceived;

			public int PacketsSent;

			public int PacketsDropped;

			public int PacketsOutOfOrder;

			public int PacketsDuplicated;

			public int PacketsStale;

			public int PacketsResent;
		}

		public struct RTTInfo
		{
			public int LastRtt;

			public float SmoothedRtt;

			public float SmoothedVariance;

			public int ResendTimeout;
		}

		public enum ErrorCodes
		{
			Stale_Packet = -1,
			Duplicated_Packet = -2,
			OutgoingQueueIsFull = -7,
			InsufficientMemory = -8
		}

		public enum PacketType : ushort
		{
			Payload = 0,
			Ack = 1
		}

		public struct SharedContext
		{
			public int WindowSize;

			public int MinimumResendTime;

			public SequenceBufferContext SentPackets;

			public SequenceBufferContext ReceivedPackets;

			internal int DuplicatesSinceLastAck;

			public Statistics stats;

			public ErrorCodes errorCode;

			public RTTInfo RttInfo;

			public int TimerDataOffset;

			public int TimerDataStride;

			public int RemoteTimerDataOffset;

			public int RemoteTimerDataStride;
		}

		public struct Context
		{
			public int Capacity;

			public int Resume;

			public int Delivered;

			public int IndexStride;

			public int IndexPtrOffset;

			public int DataStride;

			public int DataPtrOffset;

			public long LastSentTime;

			public long PreviousTimestamp;
		}

		public struct Parameters : INetworkParameter
		{
			public int WindowSize;

			public bool Validate()
			{
				bool result = true;
				if (WindowSize < 0 || WindowSize > 64)
				{
					result = false;
					Debug.LogError(string.Format("{0} value ({1}) must be greater than 0 and smaller or equal to 32", "WindowSize", WindowSize));
				}
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ParameterConstants
		{
			public const int WindowSize = 32;
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public struct PacketHeader
		{
			public ushort Type;

			public ushort ProcessingTime;

			public ushort SequenceId;

			public ushort AckedSequenceId;

			public uint AckMask;
		}

		internal struct ReliableHeader
		{
			public ushort Type;

			public ushort ProcessingTime;

			public ushort SequenceId;

			public ushort AckedSequenceId;

			public ulong AckedMask;
		}

		public struct PacketInformation
		{
			public int SequenceId;

			public ushort Size;

			public ushort HeaderPadding;

			public long SendTime;
		}

		[StructLayout(LayoutKind.Explicit)]
		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public struct Packet
		{
			internal const int Length = 1472;

			[FieldOffset(0)]
			public PacketHeader Header;

			[FieldOffset(0)]
			public unsafe fixed byte Buffer[1472];
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct ReliablePacket
		{
			internal const int Length = 1476;

			[FieldOffset(0)]
			public ReliableHeader Header;

			[FieldOffset(0)]
			public unsafe fixed byte Buffer[1476];
		}

		public struct PacketTimers
		{
			public ushort ProcessingTime;

			public ushort Padding;

			public int SequenceId;

			public long SentTime;

			public long ReceiveTime;
		}

		public const int NullEntry = -1;

		public const int DefaultMinimumResendTime = 64;

		public const int MaximumResendTime = 200;

		internal const int MaxDuplicatesSinceLastAck = 3;

		private static int AlignedSizeOf<T>() where T : struct
		{
			return (UnsafeUtility.SizeOf<T>() + 7) & -8;
		}

		internal static int PacketHeaderWireSize(int windowSize)
		{
			int num = UnsafeUtility.SizeOf<ReliableHeader>();
			if (windowSize <= 32)
			{
				return num - 4;
			}
			return num;
		}

		internal unsafe static int PacketHeaderWireSize(NetworkPipelineContext ctx)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)ctx.internalSharedProcessBuffer;
			return PacketHeaderWireSize(internalSharedProcessBuffer->WindowSize);
		}

		public static int SharedCapacityNeeded(Parameters param)
		{
			int num = AlignedSizeOf<PacketTimers>() * param.WindowSize * 2;
			return AlignedSizeOf<SharedContext>() + num;
		}

		public static int ProcessCapacityNeeded(Parameters param)
		{
			int num = AlignedSizeOf<PacketInformation>();
			int num2 = 1480;
			num *= param.WindowSize;
			num2 *= param.WindowSize;
			return AlignedSizeOf<Context>() + num + num2;
		}

		public unsafe static SharedContext InitializeContext(byte* sharedBuffer, int sharedBufferLength, byte* sendBuffer, int sendBufferLength, byte* recvBuffer, int recvBufferLength, Parameters param)
		{
			InitializeProcessContext(sendBuffer, sendBufferLength, param);
			InitializeProcessContext(recvBuffer, recvBufferLength, param);
			*(SharedContext*)sharedBuffer = new SharedContext
			{
				WindowSize = param.WindowSize,
				SentPackets = new SequenceBufferContext
				{
					Acked = -1,
					AckedMask = ulong.MaxValue,
					LastAckedMask = ulong.MaxValue
				},
				MinimumResendTime = 64,
				ReceivedPackets = new SequenceBufferContext
				{
					Sequence = -1,
					AckedMask = ulong.MaxValue,
					LastAckedMask = ulong.MaxValue
				},
				RttInfo = new RTTInfo
				{
					SmoothedVariance = 5f,
					SmoothedRtt = 50f,
					ResendTimeout = 50,
					LastRtt = 50
				},
				TimerDataOffset = AlignedSizeOf<SharedContext>(),
				TimerDataStride = AlignedSizeOf<PacketTimers>(),
				RemoteTimerDataOffset = AlignedSizeOf<SharedContext>() + AlignedSizeOf<PacketTimers>() * param.WindowSize,
				RemoteTimerDataStride = AlignedSizeOf<PacketTimers>()
			};
			return *(SharedContext*)sharedBuffer;
		}

		public unsafe static int InitializeProcessContext(byte* buffer, int bufferLength, Parameters param)
		{
			int num = ProcessCapacityNeeded(param);
			if (bufferLength != num)
			{
				return -8;
			}
			((Context*)buffer)->Capacity = param.WindowSize;
			((Context*)buffer)->IndexStride = AlignedSizeOf<PacketInformation>();
			((Context*)buffer)->IndexPtrOffset = AlignedSizeOf<Context>();
			((Context*)buffer)->DataStride = 1480;
			((Context*)buffer)->DataPtrOffset = ((Context*)buffer)->IndexPtrOffset + ((Context*)buffer)->IndexStride * ((Context*)buffer)->Capacity;
			((Context*)buffer)->Resume = -1;
			((Context*)buffer)->Delivered = -1;
			Release(buffer, 0, param.WindowSize);
			return 0;
		}

		public unsafe static void SetPacket(byte* self, int sequence, InboundRecvBuffer data)
		{
			SetPacket(self, sequence, data.buffer, data.bufferLength);
		}

		public unsafe static void SetPacket(byte* self, int sequence, void* data, int length)
		{
			if (length <= ((Context*)self)->DataStride)
			{
				int num = sequence % ((Context*)self)->Capacity;
				PacketInformation* packetInformation = GetPacketInformation(self, sequence);
				packetInformation->SequenceId = sequence;
				packetInformation->Size = (ushort)length;
				packetInformation->HeaderPadding = 0;
				packetInformation->SendTime = -1L;
				int num2 = ((Context*)self)->DataPtrOffset + num * ((Context*)self)->DataStride;
				void* destination = self + num2;
				UnsafeUtility.MemCpy(destination, data, length);
			}
		}

		[Obsolete("Internal API that shouldn't be used. Will be removed in Unity Transport 2.0.")]
		public unsafe static void SetHeaderAndPacket(byte* self, int sequence, PacketHeader header, InboundSendBuffer data, long timestamp)
		{
			throw new NotImplementedException("Implementation was moved to other internal APIs.");
		}

		internal unsafe static void SetHeaderAndPacket(byte* self, int sequence, ReliableHeader header, InboundSendBuffer data, long timestamp)
		{
			int num = data.bufferLength + data.headerPadding;
			if (num <= ((Context*)self)->DataStride)
			{
				int num2 = sequence % ((Context*)self)->Capacity;
				PacketInformation* packetInformation = GetPacketInformation(self, sequence);
				packetInformation->SequenceId = sequence;
				packetInformation->Size = (ushort)num;
				packetInformation->HeaderPadding = (ushort)data.headerPadding;
				packetInformation->SendTime = timestamp;
				GetReliablePacket(self, sequence)->Header = header;
				int num3 = ((Context*)self)->DataPtrOffset + num2 * ((Context*)self)->DataStride;
				void* ptr = self + num3;
				if (data.bufferLength > 0)
				{
					UnsafeUtility.MemCpy((byte*)ptr + data.headerPadding, data.buffer, data.bufferLength);
				}
			}
		}

		public unsafe static PacketInformation* GetPacketInformation(byte* self, int sequence)
		{
			int num = sequence % ((Context*)self)->Capacity;
			return (PacketInformation*)(self + ((Context*)self)->IndexPtrOffset + num * ((Context*)self)->IndexStride);
		}

		[Obsolete("Internal API that shouldn't be used. Will be removed in Unity Transport 2.0.")]
		public unsafe static Packet* GetPacket(byte* self, int sequence)
		{
			throw new NotImplementedException("Implementation was moved to other internal APIs.");
		}

		internal unsafe static ReliablePacket* GetReliablePacket(byte* self, int sequence)
		{
			int num = sequence % ((Context*)self)->Capacity;
			int num2 = ((Context*)self)->DataPtrOffset + num * ((Context*)self)->DataStride;
			return (ReliablePacket*)(self + num2);
		}

		public unsafe static bool TryAquire(byte* self, int sequence)
		{
			int index = sequence % ((Context*)self)->Capacity;
			if (GetIndex(self, index) == -1)
			{
				SetIndex(self, index, sequence);
				return true;
			}
			return false;
		}

		public unsafe static void Release(byte* self, int sequence)
		{
			Release(self, sequence, 1);
		}

		public unsafe static void Release(byte* self, int start_sequence, int count)
		{
			for (int i = 0; i < count; i++)
			{
				SetIndex(self, (start_sequence + i) % ((Context*)self)->Capacity, -1);
			}
		}

		private unsafe static void SetIndex(byte* self, int index, int sequence)
		{
			int* ptr = (int*)(self + ((Context*)self)->IndexPtrOffset + index * ((Context*)self)->IndexStride);
			*ptr = sequence;
		}

		private unsafe static int GetIndex(byte* self, int index)
		{
			int* ptr = (int*)(self + ((Context*)self)->IndexPtrOffset + index * ((Context*)self)->IndexStride);
			return *ptr;
		}

		[Obsolete("Internal API that shouldn't be used. Will be removed in Unity Transport 2.0.")]
		public static bool ReleaseOrResumePackets(NetworkPipelineContext context)
		{
			throw new NotImplementedException("Implementation was moved to other internal APIs.");
		}

		private unsafe static ushort GetNonWrappingLastAckedSequenceNumber(NetworkPipelineContext context)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			ushort num = (ushort)internalSharedProcessBuffer->SentPackets.Acked;
			return (ushort)(internalSharedProcessBuffer->WindowSize * (1 - num >> 15));
		}

		internal unsafe static void ReleaseAcknowledgedPackets(NetworkPipelineContext context)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			ulong ackedMask = internalSharedProcessBuffer->SentPackets.AckedMask;
			ushort num = (ushort)internalSharedProcessBuffer->SentPackets.Acked;
			ushort num2 = GetNonWrappingLastAckedSequenceNumber(context);
			for (int i = 0; i < internalSharedProcessBuffer->WindowSize; i++)
			{
				PacketInformation* packetInformation = GetPacketInformation(context.internalProcessBuffer, num2);
				if (packetInformation->SequenceId >= 0)
				{
					ulong num3 = (ulong)(1L << num - packetInformation->SequenceId);
					if (SequenceHelpers.AbsDistance(num, (ushort)packetInformation->SequenceId) < internalSharedProcessBuffer->WindowSize && (num3 & ackedMask) != 0L)
					{
						Release(context.internalProcessBuffer, packetInformation->SequenceId);
						packetInformation->SendTime = -1L;
					}
				}
				num2--;
			}
		}

		internal unsafe static int GetNextSendResumeSequence(NetworkPipelineContext context)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			ushort num = GetNonWrappingLastAckedSequenceNumber(context);
			int result = -1;
			for (int i = 0; i < internalSharedProcessBuffer->WindowSize; i++)
			{
				PacketInformation* packetInformation = GetPacketInformation(context.internalProcessBuffer, num);
				if (packetInformation->SequenceId >= 0)
				{
					int num2 = CurrentResendTime(context.internalSharedProcessBuffer);
					if (context.timestamp > packetInformation->SendTime + num2)
					{
						result = packetInformation->SequenceId;
					}
				}
				num--;
			}
			return result;
		}

		public unsafe static InboundRecvBuffer ResumeReceive(NetworkPipelineContext context, int startSequence, ref bool needsResume)
		{
			if (startSequence == -1)
			{
				return default(InboundRecvBuffer);
			}
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			Context* internalProcessBuffer = (Context*)context.internalProcessBuffer;
			internalProcessBuffer->Resume = -1;
			PacketInformation* packetInformation = GetPacketInformation(context.internalProcessBuffer, startSequence);
			int sequence = internalSharedProcessBuffer->ReceivedPackets.Sequence;
			if (packetInformation->SequenceId == startSequence)
			{
				int num = internalProcessBuffer->DataPtrOffset + startSequence % internalProcessBuffer->Capacity * internalProcessBuffer->DataStride;
				InboundRecvBuffer result = default(InboundRecvBuffer);
				result.buffer = context.internalProcessBuffer + num;
				result.bufferLength = packetInformation->Size;
				internalProcessBuffer->Delivered = startSequence;
				if (SequenceHelpers.LessThan16((ushort)startSequence, (ushort)sequence))
				{
					internalProcessBuffer->Resume = (ushort)(startSequence + 1);
					needsResume = true;
				}
				return result;
			}
			return default(InboundRecvBuffer);
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static InboundSendBuffer ResumeSend(NetworkPipelineContext context, out PacketHeader header, ref bool needsResume)
		{
			throw new NotImplementedException("Implementation moved to an internal method. Shouldn't be used anymore.");
		}

		internal unsafe static InboundSendBuffer ResumeSend(NetworkPipelineContext context, out ReliableHeader header, ref bool needsResume)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			Context* internalProcessBuffer = (Context*)context.internalProcessBuffer;
			ushort num = (ushort)internalProcessBuffer->Resume;
			PacketInformation* packetInformation = GetPacketInformation(context.internalProcessBuffer, num);
			packetInformation->SendTime = context.timestamp;
			ReliablePacket* reliablePacket = GetReliablePacket(context.internalProcessBuffer, num);
			header = reliablePacket->Header;
			header.AckedSequenceId = (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence;
			header.AckedMask = internalSharedProcessBuffer->ReceivedPackets.AckedMask;
			int num2 = internalProcessBuffer->DataPtrOffset + num % internalProcessBuffer->Capacity * internalProcessBuffer->DataStride;
			InboundSendBuffer result = default(InboundSendBuffer);
			result.bufferWithHeaders = context.internalProcessBuffer + num2;
			result.bufferWithHeadersLength = packetInformation->Size;
			result.headerPadding = packetInformation->HeaderPadding;
			result.SetBufferFrombufferWithHeaders();
			internalSharedProcessBuffer->stats.PacketsResent++;
			return result;
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static int Write(NetworkPipelineContext context, InboundSendBuffer inboundBuffer, ref PacketHeader header)
		{
			throw new NotImplementedException("Implementation moved to an internal method. Shouldn't be used anymore.");
		}

		internal unsafe static int Write(NetworkPipelineContext context, InboundSendBuffer inboundBuffer, ref ReliableHeader header)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			ushort num = (ushort)internalSharedProcessBuffer->SentPackets.Sequence;
			if (!TryAquire(context.internalProcessBuffer, num))
			{
				internalSharedProcessBuffer->errorCode = ErrorCodes.OutgoingQueueIsFull;
				return -7;
			}
			internalSharedProcessBuffer->stats.PacketsSent++;
			header.SequenceId = num;
			header.AckedSequenceId = (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence;
			header.AckedMask = internalSharedProcessBuffer->ReceivedPackets.AckedMask;
			internalSharedProcessBuffer->ReceivedPackets.Acked = internalSharedProcessBuffer->ReceivedPackets.Sequence;
			internalSharedProcessBuffer->ReceivedPackets.LastAckedMask = header.AckedMask;
			internalSharedProcessBuffer->DuplicatesSinceLastAck = 0;
			header.ProcessingTime = CalculateProcessingTime(context.internalSharedProcessBuffer, header.AckedSequenceId, context.timestamp);
			internalSharedProcessBuffer->SentPackets.Sequence = (ushort)(internalSharedProcessBuffer->SentPackets.Sequence + 1);
			SetHeaderAndPacket(context.internalProcessBuffer, num, header, inboundBuffer, context.timestamp);
			StoreTimestamp(context.internalSharedProcessBuffer, num, context.timestamp);
			return num;
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static void WriteAckPacket(NetworkPipelineContext context, ref PacketHeader header)
		{
			throw new NotImplementedException("Implementation moved to an internal method. Shouldn't be used anymore.");
		}

		internal unsafe static void WriteAckPacket(NetworkPipelineContext context, ref ReliableHeader header)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			header.Type = 1;
			header.AckedSequenceId = (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence;
			header.AckedMask = internalSharedProcessBuffer->ReceivedPackets.AckedMask;
			header.ProcessingTime = CalculateProcessingTime(context.internalSharedProcessBuffer, header.AckedSequenceId, context.timestamp);
			internalSharedProcessBuffer->ReceivedPackets.Acked = internalSharedProcessBuffer->ReceivedPackets.Sequence;
			internalSharedProcessBuffer->ReceivedPackets.LastAckedMask = header.AckedMask;
			internalSharedProcessBuffer->DuplicatesSinceLastAck = 0;
		}

		public unsafe static void StoreTimestamp(byte* sharedBuffer, ushort sequenceId, long timestamp)
		{
			PacketTimers* localPacketTimer = GetLocalPacketTimer(sharedBuffer, sequenceId);
			localPacketTimer->SequenceId = sequenceId;
			localPacketTimer->SentTime = timestamp;
			localPacketTimer->ProcessingTime = 0;
			localPacketTimer->ReceiveTime = 0L;
		}

		public unsafe static void StoreReceiveTimestamp(byte* sharedBuffer, ushort sequenceId, long timestamp, ushort processingTime)
		{
			RTTInfo rttInfo = ((SharedContext*)sharedBuffer)->RttInfo;
			PacketTimers* localPacketTimer = GetLocalPacketTimer(sharedBuffer, sequenceId);
			if (localPacketTimer != null && localPacketTimer->SequenceId == sequenceId && localPacketTimer->ReceiveTime <= 0)
			{
				localPacketTimer->ReceiveTime = timestamp;
				localPacketTimer->ProcessingTime = processingTime;
				rttInfo.LastRtt = (int)Math.Max(localPacketTimer->ReceiveTime - localPacketTimer->SentTime - localPacketTimer->ProcessingTime, 1L);
				float num = (float)rttInfo.LastRtt - rttInfo.SmoothedRtt;
				rttInfo.SmoothedRtt += num / 8f;
				rttInfo.SmoothedVariance += (math.abs(num) - rttInfo.SmoothedVariance) / 4f;
				rttInfo.ResendTimeout = (int)(rttInfo.SmoothedRtt + 4f * rttInfo.SmoothedVariance);
				((SharedContext*)sharedBuffer)->RttInfo = rttInfo;
			}
		}

		public unsafe static void StoreRemoteReceiveTimestamp(byte* sharedBuffer, ushort sequenceId, long timestamp)
		{
			PacketTimers* remotePacketTimer = GetRemotePacketTimer(sharedBuffer, sequenceId);
			remotePacketTimer->SequenceId = sequenceId;
			remotePacketTimer->ReceiveTime = timestamp;
		}

		private unsafe static int CurrentResendTime(byte* sharedBuffer)
		{
			if (((SharedContext*)sharedBuffer)->RttInfo.ResendTimeout > 200)
			{
				return 200;
			}
			return Math.Max(((SharedContext*)sharedBuffer)->RttInfo.ResendTimeout, ((SharedContext*)sharedBuffer)->MinimumResendTime);
		}

		public unsafe static ushort CalculateProcessingTime(byte* sharedBuffer, ushort sequenceId, long timestamp)
		{
			PacketTimers* remotePacketTimer = GetRemotePacketTimer(sharedBuffer, sequenceId);
			if (remotePacketTimer != null && remotePacketTimer->SequenceId == sequenceId)
			{
				return Math.Min((ushort)(timestamp - remotePacketTimer->ReceiveTime), ushort.MaxValue);
			}
			return 0;
		}

		public unsafe static PacketTimers* GetLocalPacketTimer(byte* sharedBuffer, ushort sequenceId)
		{
			int num = sequenceId % ((SharedContext*)sharedBuffer)->WindowSize;
			return (PacketTimers*)((long)sharedBuffer + (long)((SharedContext*)sharedBuffer)->TimerDataOffset + ((SharedContext*)sharedBuffer)->TimerDataStride * num);
		}

		public unsafe static PacketTimers* GetRemotePacketTimer(byte* sharedBuffer, ushort sequenceId)
		{
			int num = sequenceId % ((SharedContext*)sharedBuffer)->WindowSize;
			return (PacketTimers*)((long)sharedBuffer + (long)((SharedContext*)sharedBuffer)->RemoteTimerDataOffset + ((SharedContext*)sharedBuffer)->RemoteTimerDataStride * num);
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static int Read(NetworkPipelineContext context, PacketHeader header)
		{
			throw new NotImplementedException("Implementation moved to an internal method. Shouldn't be used anymore.");
		}

		internal unsafe static int Read(NetworkPipelineContext context, ReliableHeader header)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			internalSharedProcessBuffer->stats.PacketsReceived++;
			if (SequenceHelpers.StalePacket(header.SequenceId, (ushort)(internalSharedProcessBuffer->ReceivedPackets.Sequence + 1), (ushort)internalSharedProcessBuffer->WindowSize))
			{
				internalSharedProcessBuffer->stats.PacketsStale++;
				return -1;
			}
			int num = internalSharedProcessBuffer->WindowSize - 1;
			if (SequenceHelpers.GreaterThan16(header.SequenceId, (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence))
			{
				int num2 = SequenceHelpers.AbsDistance(header.SequenceId, (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence);
				if (num2 > num)
				{
					internalSharedProcessBuffer->stats.PacketsDropped += num2 - 1;
					internalSharedProcessBuffer->ReceivedPackets.AckedMask = 1uL;
				}
				else
				{
					internalSharedProcessBuffer->ReceivedPackets.AckedMask <<= num2;
					internalSharedProcessBuffer->ReceivedPackets.AckedMask |= 1uL;
					for (int i = 0; i < Math.Min(num2, num); i++)
					{
						if ((internalSharedProcessBuffer->ReceivedPackets.AckedMask & (ulong)(1L << i)) == 0L)
						{
							internalSharedProcessBuffer->stats.PacketsDropped++;
						}
					}
				}
				internalSharedProcessBuffer->ReceivedPackets.Sequence = header.SequenceId;
			}
			else
			{
				int num3 = SequenceHelpers.AbsDistance(header.SequenceId, (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence);
				if (num3 >= 65535 - internalSharedProcessBuffer->WindowSize)
				{
					num3 = internalSharedProcessBuffer->ReceivedPackets.Sequence - header.SequenceId;
				}
				ulong num4 = (ulong)(1L << num3);
				if ((num4 & internalSharedProcessBuffer->ReceivedPackets.AckedMask) != 0L)
				{
					ReadAckPacket(context, header);
					internalSharedProcessBuffer->stats.PacketsDuplicated++;
					return -2;
				}
				internalSharedProcessBuffer->stats.PacketsOutOfOrder++;
				internalSharedProcessBuffer->ReceivedPackets.AckedMask |= num4;
			}
			StoreRemoteReceiveTimestamp(context.internalSharedProcessBuffer, header.SequenceId, context.timestamp);
			ReadAckPacket(context, header);
			return header.SequenceId;
		}

		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static void ReadAckPacket(NetworkPipelineContext context, PacketHeader header)
		{
			throw new NotImplementedException("Implementation moved to an internal method. Shouldn't be used anymore.");
		}

		internal unsafe static void ReadAckPacket(NetworkPipelineContext context, ReliableHeader header)
		{
			SharedContext* internalSharedProcessBuffer = (SharedContext*)context.internalSharedProcessBuffer;
			StoreReceiveTimestamp(context.internalSharedProcessBuffer, header.AckedSequenceId, context.timestamp, header.ProcessingTime);
			if (!SequenceHelpers.GreaterThan16((ushort)internalSharedProcessBuffer->SentPackets.Acked, header.AckedSequenceId))
			{
				if (internalSharedProcessBuffer->SentPackets.Acked == header.AckedSequenceId)
				{
					internalSharedProcessBuffer->SentPackets.AckedMask |= header.AckedMask;
					return;
				}
				internalSharedProcessBuffer->SentPackets.Acked = header.AckedSequenceId;
				internalSharedProcessBuffer->SentPackets.AckedMask = header.AckedMask;
			}
		}

		public unsafe static bool ShouldSendAck(NetworkPipelineContext ctx)
		{
			Context* internalProcessBuffer = (Context*)ctx.internalProcessBuffer;
			SharedContext* internalSharedProcessBuffer = (SharedContext*)ctx.internalSharedProcessBuffer;
			if (internalProcessBuffer->LastSentTime < internalProcessBuffer->PreviousTimestamp && (SequenceHelpers.LessThan16((ushort)internalSharedProcessBuffer->ReceivedPackets.Acked, (ushort)internalSharedProcessBuffer->ReceivedPackets.Sequence) || internalSharedProcessBuffer->ReceivedPackets.AckedMask != internalSharedProcessBuffer->ReceivedPackets.LastAckedMask || internalSharedProcessBuffer->DuplicatesSinceLastAck >= 3))
			{
				return true;
			}
			return false;
		}

		public unsafe static void SetMinimumResendTime(int value, NetworkDriver driver, NetworkPipeline pipeline, NetworkConnection con)
		{
			driver.GetPipelineBuffers(pipeline, NetworkPipelineStageCollection.GetStageId(typeof(ReliableSequencedPipelineStage)), con, out var _, out var _, out var sharedBuffer);
			SharedContext* unsafePtr = (SharedContext*)sharedBuffer.GetUnsafePtr();
			unsafePtr->MinimumResendTime = value;
		}
	}
}
