using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	public struct DataStreamWriter
	{
		private struct StreamData
		{
			public unsafe byte* buffer;

			public int length;

			public int capacity;

			public ulong bitBuffer;

			public int bitIndex;

			public int failedWrites;
		}

		[NativeDisableUnsafePtrRestriction]
		private StreamData m_Data;

		internal IntPtr m_SendHandleData;

		public unsafe static bool IsLittleEndian
		{
			get
			{
				uint num = 1u;
				byte* ptr = (byte*)(&num);
				return *ptr == 1;
			}
		}

		public unsafe bool IsCreated => m_Data.buffer != null;

		public bool HasFailedWrites => m_Data.failedWrites > 0;

		public int Capacity => m_Data.capacity;

		public int Length
		{
			get
			{
				SyncBitData();
				return m_Data.length + (m_Data.bitIndex + 7 >> 3);
			}
		}

		public int LengthInBits
		{
			get
			{
				SyncBitData();
				return m_Data.length * 8 + m_Data.bitIndex;
			}
		}

		public DataStreamWriter(int length, Allocator allocator)
		{
			Initialize(out this, new NativeArray<byte>(length, allocator));
		}

		public DataStreamWriter(NativeArray<byte> data)
		{
			Initialize(out this, data);
		}

		public unsafe DataStreamWriter(byte* data, int length)
		{
			NativeArray<byte> data2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(data, length, Allocator.Invalid);
			Initialize(out this, data2);
		}

		public unsafe NativeArray<byte> AsNativeArray()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(m_Data.buffer, Length, Allocator.Invalid);
		}

		private unsafe static void Initialize(out DataStreamWriter self, NativeArray<byte> data)
		{
			self.m_SendHandleData = IntPtr.Zero;
			self.m_Data.capacity = data.Length;
			self.m_Data.length = 0;
			self.m_Data.buffer = (byte*)data.GetUnsafePtr();
			self.m_Data.bitBuffer = 0uL;
			self.m_Data.bitIndex = 0;
			self.m_Data.failedWrites = 0;
		}

		private static short ByteSwap(short val)
		{
			return (short)(((val & 0xFF) << 8) | ((val >> 8) & 0xFF));
		}

		private static int ByteSwap(int val)
		{
			return ((val & 0xFF) << 24) | ((val & 0xFF00) << 8) | ((val >> 8) & 0xFF00) | ((val >> 24) & 0xFF);
		}

		private unsafe void SyncBitData()
		{
			int num = m_Data.bitIndex;
			if (num > 0)
			{
				ulong num2 = m_Data.bitBuffer;
				int num3 = 0;
				while (num > 0)
				{
					m_Data.buffer[m_Data.length + num3] = (byte)num2;
					num -= 8;
					num2 >>= 8;
					num3++;
				}
			}
		}

		public unsafe void Flush()
		{
			while (m_Data.bitIndex > 0)
			{
				m_Data.buffer[m_Data.length++] = (byte)m_Data.bitBuffer;
				m_Data.bitIndex -= 8;
				m_Data.bitBuffer >>= 8;
			}
			m_Data.bitIndex = 0;
		}

		public unsafe bool WriteBytes(byte* data, int bytes)
		{
			if (m_Data.length + (m_Data.bitIndex + 7 >> 3) + bytes > m_Data.capacity)
			{
				m_Data.failedWrites++;
				return false;
			}
			Flush();
			UnsafeUtility.MemCpy(m_Data.buffer + m_Data.length, data, bytes);
			m_Data.length += bytes;
			return true;
		}

		public unsafe bool WriteByte(byte value)
		{
			return WriteBytes(&value, 1);
		}

		public unsafe bool WriteBytes(NativeArray<byte> value)
		{
			return WriteBytes((byte*)value.GetUnsafeReadOnlyPtr(), value.Length);
		}

		public unsafe bool WriteShort(short value)
		{
			return WriteBytes((byte*)(&value), 2);
		}

		public unsafe bool WriteUShort(ushort value)
		{
			return WriteBytes((byte*)(&value), 2);
		}

		public unsafe bool WriteInt(int value)
		{
			return WriteBytes((byte*)(&value), 4);
		}

		public unsafe bool WriteUInt(uint value)
		{
			return WriteBytes((byte*)(&value), 4);
		}

		public unsafe bool WriteLong(long value)
		{
			return WriteBytes((byte*)(&value), 8);
		}

		public unsafe bool WriteULong(ulong value)
		{
			return WriteBytes((byte*)(&value), 8);
		}

		public unsafe bool WriteShortNetworkByteOrder(short value)
		{
			short num = (IsLittleEndian ? ByteSwap(value) : value);
			return WriteBytes((byte*)(&num), 2);
		}

		public bool WriteUShortNetworkByteOrder(ushort value)
		{
			return WriteShortNetworkByteOrder((short)value);
		}

		public unsafe bool WriteIntNetworkByteOrder(int value)
		{
			int num = (IsLittleEndian ? ByteSwap(value) : value);
			return WriteBytes((byte*)(&num), 4);
		}

		public bool WriteUIntNetworkByteOrder(uint value)
		{
			return WriteIntNetworkByteOrder((int)value);
		}

		public bool WriteFloat(float value)
		{
			UIntFloat uIntFloat = default(UIntFloat);
			uIntFloat.floatValue = value;
			return WriteInt((int)uIntFloat.intValue);
		}

		private unsafe void FlushBits()
		{
			while (m_Data.bitIndex >= 8)
			{
				m_Data.buffer[m_Data.length++] = (byte)m_Data.bitBuffer;
				m_Data.bitIndex -= 8;
				m_Data.bitBuffer >>= 8;
			}
		}

		private void WriteRawBitsInternal(uint value, int numbits)
		{
			m_Data.bitBuffer |= (ulong)value << m_Data.bitIndex;
			m_Data.bitIndex += numbits;
		}

		public bool WriteRawBits(uint value, int numbits)
		{
			if (m_Data.length + (m_Data.bitIndex + numbits + 7 >> 3) > m_Data.capacity)
			{
				m_Data.failedWrites++;
				return false;
			}
			WriteRawBitsInternal(value, numbits);
			FlushBits();
			return true;
		}

		public unsafe bool WritePackedUInt(uint value, NetworkCompressionModel model)
		{
			int num = model.CalculateBucket(value);
			uint num2 = model.bucketOffsets[num];
			int num3 = model.bucketSizes[num];
			ushort num4 = model.encodeTable[num];
			if (m_Data.length + (m_Data.bitIndex + (num4 & 0xFF) + num3 + 7 >> 3) > m_Data.capacity)
			{
				m_Data.failedWrites++;
				return false;
			}
			WriteRawBitsInternal((uint)(num4 >> 8), num4 & 0xFF);
			WriteRawBitsInternal(value - num2, num3);
			FlushBits();
			return true;
		}

		public bool WritePackedULong(ulong value, NetworkCompressionModel model)
		{
			return WritePackedUInt((uint)(value >> 32), model) & WritePackedUInt((uint)(value & 0xFFFFFFFFu), model);
		}

		public bool WritePackedInt(int value, NetworkCompressionModel model)
		{
			uint value2 = (uint)((value >> 31) ^ (value << 1));
			return WritePackedUInt(value2, model);
		}

		public bool WritePackedLong(long value, NetworkCompressionModel model)
		{
			ulong value2 = (ulong)((value >> 63) ^ (value << 1));
			return WritePackedULong(value2, model);
		}

		public bool WritePackedFloat(float value, NetworkCompressionModel model)
		{
			return WritePackedFloatDelta(value, 0f, model);
		}

		public bool WritePackedUIntDelta(uint value, uint baseline, NetworkCompressionModel model)
		{
			int value2 = (int)(baseline - value);
			return WritePackedInt(value2, model);
		}

		public bool WritePackedIntDelta(int value, int baseline, NetworkCompressionModel model)
		{
			int value2 = baseline - value;
			return WritePackedInt(value2, model);
		}

		public bool WritePackedLongDelta(long value, long baseline, NetworkCompressionModel model)
		{
			long value2 = baseline - value;
			return WritePackedLong(value2, model);
		}

		public bool WritePackedULongDelta(ulong value, ulong baseline, NetworkCompressionModel model)
		{
			long value2 = (long)(baseline - value);
			return WritePackedLong(value2, model);
		}

		public bool WritePackedFloatDelta(float value, float baseline, NetworkCompressionModel model)
		{
			int num = 0;
			if (value != baseline)
			{
				num = 32;
			}
			if (m_Data.length + (m_Data.bitIndex + 1 + num + 7 >> 3) > m_Data.capacity)
			{
				m_Data.failedWrites++;
				return false;
			}
			if (num == 0)
			{
				WriteRawBitsInternal(0u, 1);
			}
			else
			{
				WriteRawBitsInternal(1u, 1);
				UIntFloat uIntFloat = default(UIntFloat);
				uIntFloat.floatValue = value;
				WriteRawBitsInternal(uIntFloat.intValue, num);
			}
			FlushBits();
			return true;
		}

		public unsafe bool WriteFixedString32(FixedString32Bytes str)
		{
			int bytes = *(ushort*)(&str) + 2;
			byte* data = (byte*)(&str);
			return WriteBytes(data, bytes);
		}

		public unsafe bool WriteFixedString64(FixedString64Bytes str)
		{
			int bytes = *(ushort*)(&str) + 2;
			byte* data = (byte*)(&str);
			return WriteBytes(data, bytes);
		}

		public unsafe bool WriteFixedString128(FixedString128Bytes str)
		{
			int bytes = *(ushort*)(&str) + 2;
			byte* data = (byte*)(&str);
			return WriteBytes(data, bytes);
		}

		public unsafe bool WriteFixedString512(FixedString512Bytes str)
		{
			int bytes = *(ushort*)(&str) + 2;
			byte* data = (byte*)(&str);
			return WriteBytes(data, bytes);
		}

		public unsafe bool WriteFixedString4096(FixedString4096Bytes str)
		{
			int bytes = *(ushort*)(&str) + 2;
			byte* data = (byte*)(&str);
			return WriteBytes(data, bytes);
		}

		public unsafe bool WritePackedFixedString32Delta(FixedString32Bytes str, FixedString32Bytes baseline, NetworkCompressionModel model)
		{
			ushort length = *(ushort*)(&str);
			byte* data = (byte*)(&str) + 2;
			return WritePackedFixedStringDelta(data, length, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
		}

		public unsafe bool WritePackedFixedString64Delta(FixedString64Bytes str, FixedString64Bytes baseline, NetworkCompressionModel model)
		{
			ushort length = *(ushort*)(&str);
			byte* data = (byte*)(&str) + 2;
			return WritePackedFixedStringDelta(data, length, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
		}

		public unsafe bool WritePackedFixedString128Delta(FixedString128Bytes str, FixedString128Bytes baseline, NetworkCompressionModel model)
		{
			ushort length = *(ushort*)(&str);
			byte* data = (byte*)(&str) + 2;
			return WritePackedFixedStringDelta(data, length, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
		}

		public unsafe bool WritePackedFixedString512Delta(FixedString512Bytes str, FixedString512Bytes baseline, NetworkCompressionModel model)
		{
			ushort length = *(ushort*)(&str);
			byte* data = (byte*)(&str) + 2;
			return WritePackedFixedStringDelta(data, length, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
		}

		public unsafe bool WritePackedFixedString4096Delta(FixedString4096Bytes str, FixedString4096Bytes baseline, NetworkCompressionModel model)
		{
			ushort length = *(ushort*)(&str);
			byte* data = (byte*)(&str) + 2;
			return WritePackedFixedStringDelta(data, length, (byte*)(&baseline) + 2, *(ushort*)(&baseline), model);
		}

		private unsafe bool WritePackedFixedStringDelta(byte* data, uint length, byte* baseData, uint baseLength, NetworkCompressionModel model)
		{
			StreamData data2 = m_Data;
			if (!WritePackedUIntDelta(length, baseLength, model))
			{
				return false;
			}
			bool flag = false;
			if (length <= baseLength)
			{
				for (uint num = 0u; num < length; num++)
				{
					flag |= !WritePackedUIntDelta(data[num], baseData[num], model);
				}
			}
			else
			{
				for (uint num2 = 0u; num2 < baseLength; num2++)
				{
					flag |= !WritePackedUIntDelta(data[num2], baseData[num2], model);
				}
				for (uint num3 = baseLength; num3 < length; num3++)
				{
					flag |= !WritePackedUInt(data[num3], model);
				}
			}
			if (flag)
			{
				m_Data = data2;
				m_Data.failedWrites++;
			}
			return !flag;
		}

		public void Clear()
		{
			m_Data.length = 0;
			m_Data.bitIndex = 0;
			m_Data.bitBuffer = 0uL;
			m_Data.failedWrites = 0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAllocator(Allocator allocator)
		{
			if (allocator != Allocator.Temp)
			{
				throw new InvalidOperationException("DataStreamWriters can only be created with temp memory");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckBits(uint value, int numbits)
		{
			if (numbits < 0 || numbits > 32)
			{
				throw new ArgumentOutOfRangeException("Invalid number of bits");
			}
			if ((ulong)value >= (ulong)(1L << numbits))
			{
				throw new ArgumentOutOfRangeException("Value does not fit in the specified number of bits");
			}
		}
	}
}
