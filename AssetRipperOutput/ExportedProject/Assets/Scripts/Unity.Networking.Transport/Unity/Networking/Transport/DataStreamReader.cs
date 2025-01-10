using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	public struct DataStreamReader
	{
		private struct Context
		{
			public int m_ReadByteIndex;

			public int m_BitIndex;

			public ulong m_BitBuffer;

			public int m_FailedReads;
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe byte* m_bufferPtr;

		private Context m_Context;

		private int m_Length;

		public bool IsLittleEndian => DataStreamWriter.IsLittleEndian;

		public bool HasFailedReads => m_Context.m_FailedReads > 0;

		public int Length => m_Length;

		public unsafe bool IsCreated => m_bufferPtr != null;

		public DataStreamReader(NativeArray<byte> array)
		{
			Initialize(out this, array);
		}

		public unsafe DataStreamReader(byte* data, int length)
		{
			NativeArray<byte> array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(data, length, Allocator.Invalid);
			Initialize(out this, array);
		}

		private unsafe static void Initialize(out DataStreamReader self, NativeArray<byte> array)
		{
			self.m_bufferPtr = (byte*)array.GetUnsafeReadOnlyPtr();
			self.m_Length = array.Length;
			self.m_Context = default(Context);
		}

		private static short ByteSwap(short val)
		{
			return (short)(((val & 0xFF) << 8) | ((val >> 8) & 0xFF));
		}

		private static int ByteSwap(int val)
		{
			return ((val & 0xFF) << 24) | ((val & 0xFF00) << 8) | ((val >> 8) & 0xFF00) | ((val >> 24) & 0xFF);
		}

		public unsafe void ReadBytes(byte* data, int length)
		{
			if (GetBytesRead() + length > m_Length)
			{
				m_Context.m_FailedReads++;
				UnsafeUtility.MemClear(data, length);
				return;
			}
			m_Context.m_ReadByteIndex -= m_Context.m_BitIndex >> 3;
			m_Context.m_BitIndex = 0;
			m_Context.m_BitBuffer = 0uL;
			UnsafeUtility.MemCpy(data, m_bufferPtr + m_Context.m_ReadByteIndex, length);
			m_Context.m_ReadByteIndex += length;
		}

		public unsafe void ReadBytes(NativeArray<byte> array)
		{
			ReadBytes((byte*)array.GetUnsafePtr(), array.Length);
		}

		public int GetBytesRead()
		{
			return m_Context.m_ReadByteIndex - (m_Context.m_BitIndex >> 3);
		}

		public int GetBitsRead()
		{
			return (m_Context.m_ReadByteIndex << 3) - m_Context.m_BitIndex;
		}

		public void SeekSet(int pos)
		{
			if (pos > m_Length)
			{
				m_Context.m_FailedReads++;
				return;
			}
			m_Context.m_ReadByteIndex = pos;
			m_Context.m_BitIndex = 0;
			m_Context.m_BitBuffer = 0uL;
		}

		public unsafe byte ReadByte()
		{
			byte result = default(byte);
			ReadBytes(&result, 1);
			return result;
		}

		public unsafe short ReadShort()
		{
			short result = default(short);
			ReadBytes((byte*)(&result), 2);
			return result;
		}

		public unsafe ushort ReadUShort()
		{
			ushort result = default(ushort);
			ReadBytes((byte*)(&result), 2);
			return result;
		}

		public unsafe int ReadInt()
		{
			int result = default(int);
			ReadBytes((byte*)(&result), 4);
			return result;
		}

		public unsafe uint ReadUInt()
		{
			uint result = default(uint);
			ReadBytes((byte*)(&result), 4);
			return result;
		}

		public unsafe long ReadLong()
		{
			long result = default(long);
			ReadBytes((byte*)(&result), 8);
			return result;
		}

		public unsafe ulong ReadULong()
		{
			ulong result = default(ulong);
			ReadBytes((byte*)(&result), 8);
			return result;
		}

		public unsafe short ReadShortNetworkByteOrder()
		{
			short num = default(short);
			ReadBytes((byte*)(&num), 2);
			if (!IsLittleEndian)
			{
				return num;
			}
			return ByteSwap(num);
		}

		public ushort ReadUShortNetworkByteOrder()
		{
			return (ushort)ReadShortNetworkByteOrder();
		}

		public unsafe int ReadIntNetworkByteOrder()
		{
			int num = default(int);
			ReadBytes((byte*)(&num), 4);
			if (!IsLittleEndian)
			{
				return num;
			}
			return ByteSwap(num);
		}

		public uint ReadUIntNetworkByteOrder()
		{
			return (uint)ReadIntNetworkByteOrder();
		}

		public float ReadFloat()
		{
			UIntFloat uIntFloat = default(UIntFloat);
			uIntFloat.intValue = (uint)ReadInt();
			return uIntFloat.floatValue;
		}

		public unsafe uint ReadPackedUInt(NetworkCompressionModel model)
		{
			FillBitBuffer();
			uint num = 63u;
			uint num2 = (uint)(int)m_Context.m_BitBuffer & num;
			ushort num3 = model.decodeTable[(int)num2];
			int num4 = num3 >> 8;
			int num5 = num3 & 0xFF;
			if (m_Context.m_BitIndex < num5)
			{
				m_Context.m_FailedReads++;
				return 0u;
			}
			m_Context.m_BitBuffer >>= num5;
			m_Context.m_BitIndex -= num5;
			uint num6 = model.bucketOffsets[num4];
			int numbits = model.bucketSizes[num4];
			return ReadRawBitsInternal(numbits) + num6;
		}

		private unsafe void FillBitBuffer()
		{
			while (m_Context.m_BitIndex <= 56 && m_Context.m_ReadByteIndex < m_Length)
			{
				m_Context.m_BitBuffer |= (ulong)m_bufferPtr[m_Context.m_ReadByteIndex++] << m_Context.m_BitIndex;
				m_Context.m_BitIndex += 8;
			}
		}

		private uint ReadRawBitsInternal(int numbits)
		{
			if (m_Context.m_BitIndex < numbits)
			{
				m_Context.m_FailedReads++;
				return 0u;
			}
			int result = (int)((long)m_Context.m_BitBuffer & ((1L << numbits) - 1));
			m_Context.m_BitBuffer >>= numbits;
			m_Context.m_BitIndex -= numbits;
			return (uint)result;
		}

		public uint ReadRawBits(int numbits)
		{
			FillBitBuffer();
			return ReadRawBitsInternal(numbits);
		}

		public ulong ReadPackedULong(NetworkCompressionModel model)
		{
			return ((ulong)ReadPackedUInt(model) << 32) | ReadPackedUInt(model);
		}

		public int ReadPackedInt(NetworkCompressionModel model)
		{
			uint num = ReadPackedUInt(model);
			return (int)((num >> 1) ^ (0 - (num & 1)));
		}

		public long ReadPackedLong(NetworkCompressionModel model)
		{
			ulong num = ReadPackedULong(model);
			return (long)((num >> 1) ^ (0L - (num & 1)));
		}

		public float ReadPackedFloat(NetworkCompressionModel model)
		{
			return ReadPackedFloatDelta(0f, model);
		}

		public int ReadPackedIntDelta(int baseline, NetworkCompressionModel model)
		{
			int num = ReadPackedInt(model);
			return baseline - num;
		}

		public uint ReadPackedUIntDelta(uint baseline, NetworkCompressionModel model)
		{
			uint num = (uint)ReadPackedInt(model);
			return baseline - num;
		}

		public long ReadPackedLongDelta(long baseline, NetworkCompressionModel model)
		{
			long num = ReadPackedLong(model);
			return baseline - num;
		}

		public ulong ReadPackedULongDelta(ulong baseline, NetworkCompressionModel model)
		{
			ulong num = (ulong)ReadPackedLong(model);
			return baseline - num;
		}

		public float ReadPackedFloatDelta(float baseline, NetworkCompressionModel model)
		{
			FillBitBuffer();
			if (ReadRawBitsInternal(1) == 0)
			{
				return baseline;
			}
			int numbits = 32;
			UIntFloat uIntFloat = default(UIntFloat);
			uIntFloat.intValue = ReadRawBitsInternal(numbits);
			return uIntFloat.floatValue;
		}

		public unsafe FixedString32Bytes ReadFixedString32()
		{
			FixedString32Bytes result = default(FixedString32Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadFixedString(data, result.Capacity);
			return result;
		}

		public unsafe FixedString64Bytes ReadFixedString64()
		{
			FixedString64Bytes result = default(FixedString64Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadFixedString(data, result.Capacity);
			return result;
		}

		public unsafe FixedString128Bytes ReadFixedString128()
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadFixedString(data, result.Capacity);
			return result;
		}

		public unsafe FixedString512Bytes ReadFixedString512()
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadFixedString(data, result.Capacity);
			return result;
		}

		public unsafe FixedString4096Bytes ReadFixedString4096()
		{
			FixedString4096Bytes result = default(FixedString4096Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadFixedString(data, result.Capacity);
			return result;
		}

		public unsafe ushort ReadFixedString(byte* data, int maxLength)
		{
			ushort num = ReadUShort();
			if (num > maxLength)
			{
				return 0;
			}
			ReadBytes(data, num);
			return num;
		}

		public unsafe FixedString32Bytes ReadPackedFixedString32Delta(FixedString32Bytes baseline, NetworkCompressionModel model)
		{
			FixedString32Bytes result = default(FixedString32Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadPackedFixedStringDelta(data, result.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
			return result;
		}

		public unsafe FixedString64Bytes ReadPackedFixedString64Delta(FixedString64Bytes baseline, NetworkCompressionModel model)
		{
			FixedString64Bytes result = default(FixedString64Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadPackedFixedStringDelta(data, result.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
			return result;
		}

		public unsafe FixedString128Bytes ReadPackedFixedString128Delta(FixedString128Bytes baseline, NetworkCompressionModel model)
		{
			FixedString128Bytes result = default(FixedString128Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadPackedFixedStringDelta(data, result.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
			return result;
		}

		public unsafe FixedString512Bytes ReadPackedFixedString512Delta(FixedString512Bytes baseline, NetworkCompressionModel model)
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadPackedFixedStringDelta(data, result.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
			return result;
		}

		public unsafe FixedString4096Bytes ReadPackedFixedString4096Delta(FixedString4096Bytes baseline, NetworkCompressionModel model)
		{
			FixedString4096Bytes result = default(FixedString4096Bytes);
			byte* data = (byte*)(&result) + 2;
			*(ushort*)(&result) = ReadPackedFixedStringDelta(data, result.Capacity, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
			return result;
		}

		public unsafe ushort ReadPackedFixedStringDelta(byte* data, int maxLength, byte* baseData, ushort baseLength, NetworkCompressionModel model)
		{
			uint num = ReadPackedUIntDelta(baseLength, model);
			if (num > (uint)maxLength)
			{
				return 0;
			}
			if (num <= baseLength)
			{
				for (int i = 0; i < num; i++)
				{
					data[i] = (byte)ReadPackedUIntDelta(baseData[i], model);
				}
			}
			else
			{
				for (int j = 0; j < baseLength; j++)
				{
					data[j] = (byte)ReadPackedUIntDelta(baseData[j], model);
				}
				for (int k = baseLength; k < num; k++)
				{
					data[k] = (byte)ReadPackedUInt(model);
				}
			}
			return (ushort)num;
		}

		public unsafe void* GetUnsafeReadOnlyPtr()
		{
			return m_bufferPtr;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckBits(int numbits)
		{
			if (numbits < 0 || numbits > 32)
			{
				throw new ArgumentOutOfRangeException("Invalid number of bits");
			}
		}
	}
}
