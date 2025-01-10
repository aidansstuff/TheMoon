using System;
using Unity.Baselib.LowLevel;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.QoS
{
	internal class QosRequest
	{
		private const int MinPacketLen = 15;

		private const int MaxPacketLen = 1500;

		private const byte RequestMagic = 89;

		private const int ConstructedPacketLen = 14;

		private byte m_Magic = 89;

		private byte m_VerAndFlow;

		private byte m_TitleLen;

		private byte[] m_Title;

		private byte m_Sequence;

		private ushort m_Identifier;

		private ulong m_Timestamp;

		private ushort m_PacketLength = 14;

		internal byte Magic => m_Magic;

		internal byte Version => (byte)((uint)(m_VerAndFlow >> 4) & 0xFu);

		internal byte FlowControl => (byte)(m_VerAndFlow & 0xFu);

		internal byte[] Title
		{
			get
			{
				return m_Title;
			}
			set
			{
				if (15 + value.Length > 1500)
				{
					throw new ArgumentException($"Encoded title would make the QosPacket have size {15 + value.Length}. Max size is {1500}.");
				}
				m_Title = value;
				m_TitleLen = (byte)(m_Title.Length + 1);
				m_PacketLength = (ushort)(14 + m_Title.Length);
			}
		}

		internal byte Sequence
		{
			get
			{
				return m_Sequence;
			}
			set
			{
				m_Sequence = value;
			}
		}

		internal ushort Identifier
		{
			get
			{
				return m_Identifier;
			}
			set
			{
				m_Identifier = value;
			}
		}

		internal ulong Timestamp
		{
			get
			{
				return m_Timestamp;
			}
			set
			{
				m_Timestamp = value;
			}
		}

		internal int Length => m_PacketLength;

		internal unsafe (uint, int) Send(IntPtr socketHandle, NetworkEndPoint endPoint, DateTime expireTimeUtc)
		{
			if (Title == null)
			{
				throw new InvalidOperationException("QosRequest requires a title.");
			}
			UnsafeAppendBuffer unsafeAppendBuffer = Serialize();
			uint length = (uint)unsafeAppendBuffer.Length;
			Binding.Baselib_Socket_Message baselib_Socket_Message = default(Binding.Baselib_Socket_Message);
			baselib_Socket_Message.address = &endPoint.rawNetworkAddress;
			baselib_Socket_Message.data = new IntPtr(unsafeAppendBuffer.Ptr);
			baselib_Socket_Message.dataLen = length;
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_Socket_Handle baselib_Socket_Handle = default(Binding.Baselib_Socket_Handle);
			baselib_Socket_Handle.handle = socketHandle;
			Binding.Baselib_Socket_Handle socket = baselib_Socket_Handle;
			uint num = 0u;
			while (Binding.Baselib_Socket_UDP_Send(socket, &baselib_Socket_Message, 1u, &baselib_ErrorState) == 0 && QosHelper.WouldBlock(baselib_ErrorState.nativeErrorCode) && !QosHelper.ExpiredUtc(expireTimeUtc))
			{
			}
			unsafeAppendBuffer.Dispose();
			return ((uint)Length, (int)baselib_ErrorState.code);
		}

		internal UnsafeAppendBuffer Serialize()
		{
			int initialCapacity = 2048;
			UnsafeAppendBuffer result = new UnsafeAppendBuffer(initialCapacity, 16, Allocator.TempJob);
			result.Add(m_Magic);
			result.Add(m_VerAndFlow);
			result.Add(m_TitleLen);
			for (int i = 0; i < m_TitleLen - 1; i++)
			{
				result.Add(m_Title[i]);
			}
			result.Add(m_Sequence);
			byte value = (byte)(m_Identifier & 0xFFu);
			byte value2 = (byte)((m_Identifier & 0xFF00) >> 8);
			result.Add(value);
			result.Add(value2);
			byte value3 = (byte)(m_Timestamp & 0xFF);
			byte value4 = (byte)((m_Timestamp & 0xFF00) >> 8);
			byte value5 = (byte)((m_Timestamp & 0xFF0000) >> 16);
			byte value6 = (byte)((m_Timestamp & 0xFF000000u) >> 24);
			byte value7 = (byte)((m_Timestamp & 0xFF00000000L) >> 32);
			byte value8 = (byte)((m_Timestamp & 0xFF0000000000L) >> 40);
			byte value9 = (byte)((m_Timestamp & 0xFF000000000000L) >> 48);
			byte value10 = (byte)((m_Timestamp & 0xFF00000000000000uL) >> 56);
			result.Add(value3);
			result.Add(value4);
			result.Add(value5);
			result.Add(value6);
			result.Add(value7);
			result.Add(value8);
			result.Add(value9);
			result.Add(value10);
			return result;
		}
	}
}
