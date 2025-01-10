using System;
using System.Runtime.InteropServices;
using Unity.Baselib.LowLevel;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.QoS
{
	internal class QosResponse
	{
		private const int MinPacketLen = 13;

		private const int MaxPacketLen = 1500;

		private const byte ResponseMagic = 149;

		private const byte ResponseVersion = 0;

		private byte m_Magic;

		private byte m_VerAndFlow;

		private byte m_Sequence;

		private ushort m_Identifier;

		private ulong m_Timestamp;

		private int m_LatencyMs;

		private ushort m_PacketLength;

		internal byte Magic => m_Magic;

		internal byte Version => (byte)((uint)(m_VerAndFlow >> 4) & 0xFu);

		internal byte FlowControl => (byte)(m_VerAndFlow & 0xFu);

		internal byte Sequence => m_Sequence;

		internal ushort Identifier => m_Identifier;

		internal ulong Timestamp => m_Timestamp;

		internal ushort Length => m_PacketLength;

		internal int LatencyMs => m_LatencyMs;

		internal unsafe (int received, int errorCode) Recv(IntPtr socketHandle, bool wait, DateTime expireTimeUtc, ref NetworkEndPoint endPoint)
		{
			Binding.Baselib_Socket_Message baselib_Socket_Message = default(Binding.Baselib_Socket_Message);
			UnsafeAppendBuffer unsafeAppendBuffer = new UnsafeAppendBuffer(2048, 16, Allocator.Persistent);
			_ = DateTime.UtcNow;
			fixed (Binding.Baselib_NetworkAddress* address = &endPoint.rawNetworkAddress)
			{
				baselib_Socket_Message.dataLen = (uint)unsafeAppendBuffer.Capacity;
				baselib_Socket_Message.address = address;
				baselib_Socket_Message.data = new IntPtr(unsafeAppendBuffer.Ptr);
				Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
				Binding.Baselib_Socket_Handle baselib_Socket_Handle = default(Binding.Baselib_Socket_Handle);
				baselib_Socket_Handle.handle = socketHandle;
				Binding.Baselib_Socket_Handle socket = baselib_Socket_Handle;
				uint num = 0u;
				int num2 = 0;
				while (!QosHelper.ExpiredUtc(expireTimeUtc))
				{
					baselib_ErrorState = default(Binding.Baselib_ErrorState);
					num2++;
					num = Binding.Baselib_Socket_UDP_Recv(socket, &baselib_Socket_Message, 1u, &baselib_ErrorState);
					if (num != 0 || !QosHelper.WouldBlock(baselib_ErrorState.nativeErrorCode))
					{
						break;
					}
					if (!wait)
					{
						return (received: 0, errorCode: 0);
					}
				}
				if (num == 0)
				{
					unsafeAppendBuffer.Dispose();
					return (received: 0, errorCode: (int)baselib_ErrorState.code);
				}
				endPoint.rawNetworkAddress = *baselib_Socket_Message.address;
				m_PacketLength = (ushort)baselib_Socket_Message.dataLen;
				Deserialize(baselib_Socket_Message.data);
				m_LatencyMs = (int)((Length >= 13) ? (DateTime.UtcNow.Ticks / 10000 - (long)m_Timestamp) : (-1));
			}
			unsafeAppendBuffer.Dispose();
			return (received: Length, errorCode: 0);
		}

		internal void Deserialize(IntPtr msgData)
		{
			m_Magic = Marshal.ReadByte(msgData);
			m_VerAndFlow = Marshal.ReadByte(msgData, 1);
			m_Sequence = Marshal.ReadByte(msgData, 2);
			ushort num = Marshal.ReadByte(msgData, 3);
			ushort num2 = (ushort)(Marshal.ReadByte(msgData, 4) << 8);
			m_Identifier = (ushort)(num + num2);
			ulong num3 = Marshal.ReadByte(msgData, 5);
			ulong num4 = (ulong)Marshal.ReadByte(msgData, 6) << 8;
			ulong num5 = (ulong)Marshal.ReadByte(msgData, 7) << 16;
			ulong num6 = (ulong)Marshal.ReadByte(msgData, 8) << 24;
			ulong num7 = (ulong)Marshal.ReadByte(msgData, 9) << 32;
			ulong num8 = (ulong)Marshal.ReadByte(msgData, 10) << 40;
			ulong num9 = (ulong)Marshal.ReadByte(msgData, 11) << 48;
			ulong num10 = (ulong)Marshal.ReadByte(msgData, 12) << 56;
			m_Timestamp = num3 + num4 + num5 + num6 + num7 + num8 + num9 + num10;
		}

		internal bool Verify(uint maxSequence, ref string error)
		{
			if (Length < 13)
			{
				error = $"response is too small got {Length} bytes min expected {13} bytes";
				return false;
			}
			if (Magic != 149)
			{
				error = $"response contains an invalid signature 0x{Magic:X} expected 0x{(byte)149:X}";
				return false;
			}
			if (Version != 0)
			{
				error = $"response contains an invalid version {Version} expected {(byte)0}";
				return false;
			}
			if (Sequence > maxSequence)
			{
				error = $"response contains an invalid sequence {Sequence} max expected {maxSequence}";
				return false;
			}
			return true;
		}

		internal (FcType type, byte units) ParseFlowControl()
		{
			if (FlowControl == 0)
			{
				return (type: FcType.None, units: 0);
			}
			int num = (((FlowControl & 8) == 0) ? 1 : 2);
			byte b = (byte)(FlowControl & 7u);
			if (num == 2)
			{
				b++;
			}
			return (type: (FcType)num, units: b);
		}
	}
}
