using System;
using System.Diagnostics;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, IsCreated = {IsCreated}")]
	[DebuggerTypeProxy(typeof(UnsafeBitArrayDebugView))]
	[BurstCompatible]
	public struct UnsafeBitArray : INativeDisposable, IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe ulong* Ptr;

		public int Length;

		public AllocatorManager.AllocatorHandle Allocator;

		public unsafe bool IsCreated => Ptr != null;

		public unsafe UnsafeBitArray(void* ptr, int sizeInBytes, AllocatorManager.AllocatorHandle allocator = default(AllocatorManager.AllocatorHandle))
		{
			Ptr = (ulong*)ptr;
			Length = sizeInBytes * 8;
			Allocator = allocator;
		}

		public unsafe UnsafeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			Allocator = allocator;
			int num = Bitwise.AlignUp(numBits, 64) / 8;
			Ptr = (ulong*)Memory.Unmanaged.Allocate(num, 16, allocator);
			Length = numBits;
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(Ptr, num);
			}
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				Memory.Unmanaged.Free(Ptr, Allocator);
				Allocator = AllocatorManager.Invalid;
			}
			Ptr = null;
			Length = 0;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				UnsafeDisposeJob jobData = default(UnsafeDisposeJob);
				jobData.Ptr = Ptr;
				jobData.Allocator = Allocator;
				JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
				Ptr = null;
				Allocator = AllocatorManager.Invalid;
				return result;
			}
			Ptr = null;
			return inputDeps;
		}

		public unsafe void Clear()
		{
			int num = Bitwise.AlignUp(Length, 64) / 8;
			UnsafeUtility.MemClear(Ptr, num);
		}

		public unsafe void Set(int pos, bool value)
		{
			int num = pos >> 6;
			int num2 = pos & 0x3F;
			ulong num3 = (ulong)(1L << num2);
			ulong num4 = (Ptr[num] & ~num3) | ((ulong)(-Bitwise.FromBool(value)) & num3);
			Ptr[num] = num4;
		}

		public unsafe void SetBits(int pos, bool value, int numBits)
		{
			int num = math.min(pos + numBits, Length);
			int num2 = pos >> 6;
			int num3 = pos & 0x3F;
			int num4 = num - 1 >> 6;
			int num5 = num & 0x3F;
			ulong num6 = (ulong)(-1L << num3);
			ulong num7 = ulong.MaxValue >> 64 - num5;
			ulong num8 = (ulong)(-Bitwise.FromBool(value));
			ulong num9 = num6 & num8;
			ulong num10 = num7 & num8;
			ulong num11 = ~num6;
			ulong num12 = ~num7;
			if (num2 == num4)
			{
				ulong num13 = ~(num6 & num7);
				ulong num14 = num9 & num10;
				Ptr[num2] = (Ptr[num2] & num13) | num14;
				return;
			}
			Ptr[num2] = (Ptr[num2] & num11) | num9;
			for (int i = num2 + 1; i < num4; i++)
			{
				Ptr[i] = num8;
			}
			Ptr[num4] = (Ptr[num4] & num12) | num10;
		}

		public unsafe void SetBits(int pos, ulong value, int numBits = 1)
		{
			int num = pos >> 6;
			int num2 = pos & 0x3F;
			if (num2 + numBits <= 64)
			{
				ulong mask = ulong.MaxValue >> 64 - numBits;
				Ptr[num] = Bitwise.ReplaceBits(Ptr[num], num2, mask, value);
				return;
			}
			int num3 = math.min(pos + numBits, Length);
			int num4 = num3 - 1 >> 6;
			int num5 = num3 & 0x3F;
			ulong mask2 = ulong.MaxValue >> num2;
			Ptr[num] = Bitwise.ReplaceBits(Ptr[num], num2, mask2, value);
			ulong value2 = value >> 64 - num2;
			ulong mask3 = ulong.MaxValue >> 64 - num5;
			Ptr[num4] = Bitwise.ReplaceBits(Ptr[num4], 0, mask3, value2);
		}

		public unsafe ulong GetBits(int pos, int numBits = 1)
		{
			int num = pos >> 6;
			int num2 = pos & 0x3F;
			if (num2 + numBits <= 64)
			{
				ulong mask = ulong.MaxValue >> 64 - numBits;
				return Bitwise.ExtractBits(Ptr[num], num2, mask);
			}
			int num3 = math.min(pos + numBits, Length);
			int num4 = num3 - 1 >> 6;
			int num5 = num3 & 0x3F;
			ulong mask2 = ulong.MaxValue >> num2;
			ulong num6 = Bitwise.ExtractBits(Ptr[num], num2, mask2);
			ulong mask3 = ulong.MaxValue >> 64 - num5;
			return (Bitwise.ExtractBits(Ptr[num4], 0, mask3) << 64 - num2) | num6;
		}

		public unsafe bool IsSet(int pos)
		{
			int num = pos >> 6;
			int num2 = pos & 0x3F;
			ulong num3 = (ulong)(1L << num2);
			return (Ptr[num] & num3) != 0;
		}

		internal void CopyUlong(int dstPos, ref UnsafeBitArray srcBitArray, int srcPos, int numBits)
		{
			SetBits(dstPos, srcBitArray.GetBits(srcPos, numBits), numBits);
		}

		public void Copy(int dstPos, int srcPos, int numBits)
		{
			if (dstPos != srcPos)
			{
				Copy(dstPos, ref this, srcPos, numBits);
			}
		}

		public unsafe void Copy(int dstPos, ref UnsafeBitArray srcBitArray, int srcPos, int numBits)
		{
			if (numBits == 0)
			{
				return;
			}
			if (numBits <= 64)
			{
				CopyUlong(dstPos, ref srcBitArray, srcPos, numBits);
			}
			else if (numBits <= 128)
			{
				CopyUlong(dstPos, ref srcBitArray, srcPos, 64);
				numBits -= 64;
				if (numBits > 0)
				{
					CopyUlong(dstPos + 64, ref srcBitArray, srcPos + 64, numBits);
				}
			}
			else if ((dstPos & 7) == (srcPos & 7))
			{
				int num = CollectionHelper.Align(dstPos, 8) >> 3;
				int num2 = CollectionHelper.Align(srcPos, 8) >> 3;
				int num3 = num * 8 - dstPos;
				if (num3 > 0)
				{
					CopyUlong(dstPos, ref srcBitArray, srcPos, num3);
				}
				int num4 = numBits - num3;
				int num5 = num4 / 8;
				if (num5 > 0)
				{
					UnsafeUtility.MemMove((byte*)Ptr + num, (byte*)srcBitArray.Ptr + num2, num5);
				}
				int num6 = num4 & 7;
				if (num6 > 0)
				{
					CopyUlong((num + num5) * 8, ref srcBitArray, (num2 + num5) * 8, num6);
				}
			}
			else
			{
				int num7 = CollectionHelper.Align(dstPos, 64) - dstPos;
				if (num7 > 0)
				{
					CopyUlong(dstPos, ref srcBitArray, srcPos, num7);
					numBits -= num7;
					dstPos += num7;
					srcPos += num7;
				}
				while (numBits >= 64)
				{
					Ptr[dstPos >> 6] = srcBitArray.GetBits(srcPos, 64);
					numBits -= 64;
					dstPos += 64;
					srcPos += 64;
				}
				if (numBits > 0)
				{
					CopyUlong(dstPos, ref srcBitArray, srcPos, numBits);
				}
			}
		}

		public unsafe int Find(int pos, int numBits)
		{
			int count = Length - pos;
			return Bitwise.Find(Ptr, pos, count, numBits);
		}

		public unsafe int Find(int pos, int count, int numBits)
		{
			return Bitwise.Find(Ptr, pos, count, numBits);
		}

		public unsafe bool TestNone(int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, Length);
			int num2 = pos >> 6;
			int num3 = pos & 0x3F;
			int num4 = num - 1 >> 6;
			int num5 = num & 0x3F;
			ulong num6 = (ulong)(-1L << num3);
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return (Ptr[num2] & num8) == 0;
			}
			if ((Ptr[num2] & num6) != 0L)
			{
				return false;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (Ptr[i] != 0L)
				{
					return false;
				}
			}
			return (Ptr[num4] & num7) == 0;
		}

		public unsafe bool TestAny(int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, Length);
			int num2 = pos >> 6;
			int num3 = pos & 0x3F;
			int num4 = num - 1 >> 6;
			int num5 = num & 0x3F;
			ulong num6 = (ulong)(-1L << num3);
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return (Ptr[num2] & num8) != 0;
			}
			if ((Ptr[num2] & num6) != 0L)
			{
				return true;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (Ptr[i] != 0L)
				{
					return true;
				}
			}
			return (Ptr[num4] & num7) != 0;
		}

		public unsafe bool TestAll(int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, Length);
			int num2 = pos >> 6;
			int num3 = pos & 0x3F;
			int num4 = num - 1 >> 6;
			int num5 = num & 0x3F;
			ulong num6 = (ulong)(-1L << num3);
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return num8 == (Ptr[num2] & num8);
			}
			if (num6 != (Ptr[num2] & num6))
			{
				return false;
			}
			for (int i = num2 + 1; i < num4; i++)
			{
				if (ulong.MaxValue != Ptr[i])
				{
					return false;
				}
			}
			return num7 == (Ptr[num4] & num7);
		}

		public unsafe int CountBits(int pos, int numBits = 1)
		{
			int num = math.min(pos + numBits, Length);
			int num2 = pos >> 6;
			int num3 = pos & 0x3F;
			int num4 = num - 1 >> 6;
			int num5 = num & 0x3F;
			ulong num6 = (ulong)(-1L << num3);
			ulong num7 = ulong.MaxValue >> 64 - num5;
			if (num2 == num4)
			{
				ulong num8 = num6 & num7;
				return math.countbits(Ptr[num2] & num8);
			}
			int num9 = math.countbits(Ptr[num2] & num6);
			for (int i = num2 + 1; i < num4; i++)
			{
				num9 += math.countbits(Ptr[i]);
			}
			return num9 + math.countbits(Ptr[num4] & num7);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckSizeMultipleOf8(int sizeInBytes)
		{
			if (((uint)sizeInBytes & 7u) != 0)
			{
				throw new ArgumentException($"BitArray invalid arguments: sizeInBytes {sizeInBytes} (must be multiple of 8-bytes, sizeInBytes: {sizeInBytes}).");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckArgs(int pos, int numBits)
		{
			if (pos < 0 || pos >= Length || numBits < 1)
			{
				throw new ArgumentException($"BitArray invalid arguments: pos {pos} (must be 0-{Length - 1}), numBits {numBits} (must be greater than 0).");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckArgsPosCount(int begin, int count, int numBits)
		{
			if (begin < 0 || begin >= Length)
			{
				throw new ArgumentException($"BitArray invalid argument: begin {begin} (must be 0-{Length - 1}).");
			}
			if (count < 0 || count > Length)
			{
				throw new ArgumentException($"BitArray invalid argument: count {count} (must be 0-{Length}).");
			}
			if (numBits < 1 || count < numBits)
			{
				throw new ArgumentException($"BitArray invalid argument: numBits {numBits} (must be greater than 0).");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckArgsUlong(int pos, int numBits)
		{
			if (numBits < 1 || numBits > 64)
			{
				throw new ArgumentException($"BitArray invalid arguments: numBits {numBits} (must be 1-64).");
			}
			if (pos + numBits > Length)
			{
				throw new ArgumentException($"BitArray invalid arguments: Out of bounds pos {pos}, numBits {numBits}, Length {Length}.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckArgsCopy(ref UnsafeBitArray dstBitArray, int dstPos, ref UnsafeBitArray srcBitArray, int srcPos, int numBits)
		{
			if (srcPos + numBits > srcBitArray.Length)
			{
				throw new ArgumentException($"BitArray invalid arguments: Out of bounds - source position {srcPos}, numBits {numBits}, source bit array Length {srcBitArray.Length}.");
			}
			if (dstPos + numBits > dstBitArray.Length)
			{
				throw new ArgumentException($"BitArray invalid arguments: Out of bounds - destination position {dstPos}, numBits {numBits}, destination bit array Length {dstBitArray.Length}.");
			}
		}
	}
}
