using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Networking.Transport.Utilities
{
	public struct SimulatorUtility
	{
		public struct Parameters : INetworkParameter
		{
			public int MaxPacketCount;

			public int MaxPacketSize;

			public int PacketDelayMs;

			public int PacketJitterMs;

			public int PacketDropInterval;

			public int PacketDropPercentage;

			public int FuzzFactor;

			public int FuzzOffset;

			public uint RandomSeed;

			public bool Validate()
			{
				return true;
			}
		}

		public struct Context
		{
			public int MaxPacketCount;

			public int MaxPacketSize;

			public int PacketDelayMs;

			public int PacketJitterMs;

			public int PacketDrop;

			public int FuzzOffset;

			public int FuzzFactor;

			public uint RandomSeed;

			public Random Random;

			public int PacketCount;

			public int PacketDropCount;

			public int ReadyPackets;

			public int WaitingPackets;

			public long NextPacketTime;

			public long StatsTime;
		}

		public struct DelayedPacket
		{
			public int processBufferOffset;

			public ushort packetSize;

			public ushort packetHeaderPadding;

			public long delayUntil;
		}

		private int m_PacketCount;

		private int m_MaxPacketSize;

		private int m_PacketDelayMs;

		private int m_PacketJitterMs;

		public SimulatorUtility(int packetCount, int maxPacketSize, int packetDelayMs, int packetJitterMs)
		{
			m_PacketCount = packetCount;
			m_MaxPacketSize = maxPacketSize;
			m_PacketDelayMs = packetDelayMs;
			m_PacketJitterMs = packetJitterMs;
		}

		public unsafe static void InitializeContext(Parameters param, byte* sharedProcessBuffer)
		{
			((Context*)sharedProcessBuffer)->MaxPacketCount = param.MaxPacketCount;
			((Context*)sharedProcessBuffer)->MaxPacketSize = param.MaxPacketSize;
			((Context*)sharedProcessBuffer)->PacketDelayMs = param.PacketDelayMs;
			((Context*)sharedProcessBuffer)->PacketJitterMs = param.PacketJitterMs;
			((Context*)sharedProcessBuffer)->PacketDrop = param.PacketDropInterval;
			((Context*)sharedProcessBuffer)->FuzzFactor = param.FuzzFactor;
			((Context*)sharedProcessBuffer)->FuzzOffset = param.FuzzOffset;
			((Context*)sharedProcessBuffer)->PacketCount = 0;
			((Context*)sharedProcessBuffer)->PacketDropCount = 0;
			((Context*)sharedProcessBuffer)->Random = default(Random);
			if (param.RandomSeed != 0)
			{
				((Context*)sharedProcessBuffer)->Random.InitState(param.RandomSeed);
				((Context*)sharedProcessBuffer)->RandomSeed = param.RandomSeed;
			}
			else
			{
				((Context*)sharedProcessBuffer)->Random.InitState();
			}
		}

		public unsafe bool GetEmptyDataSlot(byte* processBufferPtr, ref int packetPayloadOffset, ref int packetDataOffset)
		{
			int num = UnsafeUtility.SizeOf<DelayedPacket>();
			int num2 = m_PacketCount * num;
			bool result = false;
			for (int i = 0; i < m_PacketCount; i++)
			{
				packetDataOffset = num * i;
				DelayedPacket* ptr = (DelayedPacket*)(processBufferPtr + packetDataOffset);
				if (ptr->delayUntil == 0L)
				{
					result = true;
					packetPayloadOffset = num2 + m_MaxPacketSize * i;
					break;
				}
			}
			return result;
		}

		public unsafe bool GetDelayedPacket(ref NetworkPipelineContext ctx, ref InboundSendBuffer delayedPacket, ref NetworkPipelineStage.Requests requests, long currentTimestamp)
		{
			requests = NetworkPipelineStage.Requests.None;
			int num = UnsafeUtility.SizeOf<DelayedPacket>();
			byte* internalProcessBuffer = ctx.internalProcessBuffer;
			Context* internalSharedProcessBuffer = (Context*)ctx.internalSharedProcessBuffer;
			int num2 = -1;
			long num3 = long.MaxValue;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < m_PacketCount; i++)
			{
				DelayedPacket* ptr = (DelayedPacket*)(internalProcessBuffer + num * i);
				if ((int)ptr->delayUntil == 0)
				{
					continue;
				}
				num5++;
				if (ptr->delayUntil <= currentTimestamp)
				{
					num4++;
					if (num3 > ptr->delayUntil)
					{
						num2 = i;
						num3 = ptr->delayUntil;
					}
				}
			}
			internalSharedProcessBuffer->ReadyPackets = num4;
			internalSharedProcessBuffer->WaitingPackets = num5;
			internalSharedProcessBuffer->NextPacketTime = num3;
			internalSharedProcessBuffer->StatsTime = currentTimestamp;
			if (num4 > 1)
			{
				requests |= NetworkPipelineStage.Requests.Resume;
			}
			else if (num5 > 0)
			{
				requests |= NetworkPipelineStage.Requests.Update;
			}
			if (num2 >= 0)
			{
				DelayedPacket* ptr2 = (DelayedPacket*)(internalProcessBuffer + num * num2);
				ptr2->delayUntil = 0L;
				delayedPacket.bufferWithHeaders = ctx.internalProcessBuffer + ptr2->processBufferOffset;
				delayedPacket.bufferWithHeadersLength = ptr2->packetSize;
				delayedPacket.headerPadding = ptr2->packetHeaderPadding;
				delayedPacket.SetBufferFrombufferWithHeaders();
				return true;
			}
			return false;
		}

		public unsafe void FuzzPacket(Context* ctx, ref InboundSendBuffer inboundBuffer)
		{
			int fuzzFactor = ctx->FuzzFactor;
			int fuzzOffset = ctx->FuzzOffset;
			if (ctx->Random.NextInt(0, 100) > fuzzFactor)
			{
				return;
			}
			int bufferLength = inboundBuffer.bufferLength;
			for (int i = fuzzOffset; i < bufferLength; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					if (fuzzFactor > ctx->Random.NextInt(0, 100))
					{
						byte* num = inboundBuffer.buffer + i;
						*num ^= (byte)(1 << j);
					}
				}
			}
		}

		public unsafe bool DelayPacket(ref NetworkPipelineContext ctx, InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, long timestamp)
		{
			int packetPayloadOffset = 0;
			int packetDataOffset = 0;
			byte* internalProcessBuffer = ctx.internalProcessBuffer;
			if (!GetEmptyDataSlot(internalProcessBuffer, ref packetPayloadOffset, ref packetDataOffset))
			{
				return false;
			}
			UnsafeUtility.MemCpy(ctx.internalProcessBuffer + packetPayloadOffset + inboundBuffer.headerPadding, inboundBuffer.buffer, inboundBuffer.bufferLength);
			Context* internalSharedProcessBuffer = (Context*)ctx.internalSharedProcessBuffer;
			DelayedPacket delayedPacket = default(DelayedPacket);
			delayedPacket.delayUntil = timestamp + m_PacketDelayMs + internalSharedProcessBuffer->Random.NextInt(m_PacketJitterMs * 2) - m_PacketJitterMs;
			delayedPacket.processBufferOffset = packetPayloadOffset;
			delayedPacket.packetSize = (ushort)(inboundBuffer.headerPadding + inboundBuffer.bufferLength);
			delayedPacket.packetHeaderPadding = (ushort)inboundBuffer.headerPadding;
			byte* source = (byte*)(&delayedPacket);
			UnsafeUtility.MemCpy(internalProcessBuffer + packetDataOffset, source, UnsafeUtility.SizeOf<DelayedPacket>());
			requests |= NetworkPipelineStage.Requests.Update;
			return true;
		}

		public unsafe bool ShouldDropPacket(Context* ctx, Parameters param, long timestamp)
		{
			if (param.PacketDropInterval > 0 && (ctx->PacketCount - 1) % param.PacketDropInterval == 0)
			{
				return true;
			}
			if (param.PacketDropPercentage > 0 && ctx->Random.NextInt(0, 100) < param.PacketDropPercentage)
			{
				return true;
			}
			return false;
		}
	}
}
