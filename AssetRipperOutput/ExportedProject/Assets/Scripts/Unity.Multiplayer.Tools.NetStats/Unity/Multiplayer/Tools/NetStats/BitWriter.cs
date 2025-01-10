using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal ref struct BitWriter
	{
		private FastBufferWriter m_Writer;

		private unsafe byte* m_BufferPointer;

		private readonly int m_Position;

		private int m_BitPosition;

		private const int k_BitsPerByte = 8;

		public bool BitAligned
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (m_BitPosition & 7) == 0;
			}
		}

		internal unsafe BitWriter(FastBufferWriter writer)
		{
			m_Writer = writer;
			m_BufferPointer = writer.Handle->BufferPointer + writer.Handle->Position;
			m_Position = writer.Handle->Position;
			m_BitPosition = 0;
		}

		public void Dispose()
		{
			int num = m_BitPosition >> 3;
			if (!BitAligned)
			{
				num++;
			}
			m_Writer.CommitBitwiseWrites(num);
		}

		public unsafe bool TryBeginWriteBits(int bitCount)
		{
			int num = m_BitPosition + bitCount;
			int num2 = num >> 3;
			if (((uint)num & 7u) != 0)
			{
				num2++;
			}
			if (m_Position + num2 > m_Writer.Handle->Capacity)
			{
				if (m_Position + num2 > m_Writer.Handle->MaxCapacity)
				{
					return false;
				}
				if (m_Writer.Handle->Capacity >= m_Writer.Handle->MaxCapacity)
				{
					return false;
				}
				m_Writer.Grow(num2);
				m_BufferPointer = m_Writer.Handle->BufferPointer + m_Writer.Handle->Position;
			}
			return true;
		}

		public unsafe void WriteBits(ulong value, uint bitCount)
		{
			int num = (int)bitCount / 8;
			byte* ptr = (byte*)(&value);
			if (BitAligned)
			{
				if (num != 0)
				{
					WritePartialValue(value, num);
				}
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					WriteMisaligned(ptr[i]);
				}
			}
			for (int j = num * 8; j < bitCount; j++)
			{
				WriteBit((value & (ulong)(1L << j)) != 0);
			}
		}

		public void WriteBits(byte value, uint bitCount)
		{
			for (int i = 0; i < bitCount; i++)
			{
				WriteBit(((value >> i) & 1) != 0);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBit(bool bit)
		{
			int num = m_BitPosition & 7;
			int num2 = m_BitPosition >> 3;
			m_BitPosition++;
			m_BufferPointer[num2] = (byte)(bit ? ((m_BufferPointer[num2] & ~(1 << num)) | (1 << num)) : (m_BufferPointer[num2] & ~(1 << num)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WritePartialValue<T>(T value, int bytesToWrite, int offsetBytes = 0) where T : unmanaged
		{
			byte* source = (byte*)(&value) + offsetBytes;
			UnsafeUtility.MemCpy(m_BufferPointer + m_Position, source, bytesToWrite);
			m_BitPosition += bytesToWrite * 8;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteMisaligned(byte value)
		{
			int num = m_BitPosition & 7;
			int num2 = m_BitPosition >> 3;
			int num3 = 8 - num;
			m_BufferPointer[num2 + 1] = (byte)((m_BufferPointer[num2 + 1] & (255 << num)) | (value >> num3));
			m_BufferPointer[num2] = (byte)((m_BufferPointer[num2] & (255 >> num3)) | (value << num));
			m_BitPosition += 8;
		}
	}
}
