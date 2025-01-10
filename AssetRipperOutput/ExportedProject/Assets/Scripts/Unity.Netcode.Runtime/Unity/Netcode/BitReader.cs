using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Netcode
{
	public ref struct BitReader
	{
		private FastBufferReader m_Reader;

		private unsafe readonly byte* m_BufferPointer;

		private readonly int m_Position;

		private int m_BitPosition;

		private const int k_BitsPerByte = 8;

		private int BytePosition => m_BitPosition >> 3;

		public bool BitAligned
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (m_BitPosition & 7) == 0;
			}
		}

		internal unsafe BitReader(FastBufferReader reader)
		{
			m_Reader = reader;
			m_BufferPointer = m_Reader.Handle->BufferPointer + m_Reader.Handle->Position;
			m_Position = m_Reader.Handle->Position;
			m_BitPosition = 0;
		}

		public void Dispose()
		{
			int num = m_BitPosition >> 3;
			if (!BitAligned)
			{
				num++;
			}
			m_Reader.CommitBitwiseReads(num);
		}

		public unsafe bool TryBeginReadBits(uint bitCount)
		{
			long num = m_BitPosition + bitCount;
			long num2 = num >> 3;
			if ((num & 7) != 0L)
			{
				num2++;
			}
			if (m_Reader.Handle->Position + num2 > m_Reader.Handle->Length)
			{
				return false;
			}
			return true;
		}

		public unsafe void ReadBits(out ulong value, uint bitCount)
		{
			ulong value2 = 0uL;
			int num = (int)bitCount / 8;
			byte* ptr = (byte*)(&value2);
			if (BitAligned)
			{
				if (num != 0)
				{
					ReadPartialValue<ulong>(out value2, num);
				}
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					ReadMisaligned(out ptr[i]);
				}
			}
			value2 = (value = value2 | ((ulong)ReadByteBits((int)(bitCount & 7)) << (int)(bitCount & 0xFFFFFFF8u)));
		}

		public void ReadBits(out byte value, uint bitCount)
		{
			value = ReadByteBits((int)bitCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadBit(out bool bit)
		{
			int num = m_BitPosition & 7;
			int bytePosition = BytePosition;
			bit = (m_BufferPointer[bytePosition] & (1 << num)) != 0;
			m_BitPosition++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ReadPartialValue<T>(out T value, int bytesToRead, int offsetBytes = 0) where T : unmanaged
		{
			T val = new T();
			byte* destination = (byte*)(&val) + offsetBytes;
			byte* source = m_BufferPointer + BytePosition;
			UnsafeUtility.MemCpy(destination, source, bytesToRead);
			m_BitPosition += bytesToRead * 8;
			value = val;
		}

		private byte ReadByteBits(int bitCount)
		{
			if (bitCount > 8)
			{
				throw new ArgumentOutOfRangeException("bitCount", "Cannot read more than 8 bits into an 8-bit value!");
			}
			if (bitCount < 0)
			{
				throw new ArgumentOutOfRangeException("bitCount", "Cannot read fewer than 0 bits!");
			}
			int num = 0;
			ByteBool byteBool = default(ByteBool);
			for (int i = 0; i < bitCount; i++)
			{
				ReadBit(out var bit);
				num |= byteBool.Collapse(bit) << i;
			}
			return (byte)num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ReadMisaligned(out byte value)
		{
			int num = m_BitPosition & 7;
			int num2 = m_BitPosition >> 3;
			int num3 = 8 - num;
			value = (byte)((m_BufferPointer[num2] >> num) | (m_BufferPointer[(m_BitPosition += 8) >> 3] << num3));
		}
	}
}
