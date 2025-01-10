using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}, IsCreated = {IsCreated}")]
	[BurstCompatible]
	public struct NativeBitArray : INativeDisposable, IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		internal UnsafeBitArray m_BitArray;

		public bool IsCreated => m_BitArray.IsCreated;

		public int Length => CollectionHelper.AssumePositive(m_BitArray.Length);

		public NativeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
			: this(numBits, allocator, options, 2)
		{
		}

		private NativeBitArray(int numBits, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options, int disposeSentinelStackDepth)
		{
			m_BitArray = new UnsafeBitArray(numBits, allocator, options);
		}

		public void Dispose()
		{
			m_BitArray.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return m_BitArray.Dispose(inputDeps);
		}

		public void Clear()
		{
			m_BitArray.Clear();
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> AsNativeArray<T>() where T : unmanaged
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int length = m_BitArray.Length / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(m_BitArray.Ptr, length, Allocator.None);
		}

		public void Set(int pos, bool value)
		{
			m_BitArray.Set(pos, value);
		}

		public void SetBits(int pos, bool value, int numBits)
		{
			m_BitArray.SetBits(pos, value, numBits);
		}

		public void SetBits(int pos, ulong value, int numBits = 1)
		{
			m_BitArray.SetBits(pos, value, numBits);
		}

		public ulong GetBits(int pos, int numBits = 1)
		{
			return m_BitArray.GetBits(pos, numBits);
		}

		public bool IsSet(int pos)
		{
			return m_BitArray.IsSet(pos);
		}

		public void Copy(int dstPos, int srcPos, int numBits)
		{
			m_BitArray.Copy(dstPos, srcPos, numBits);
		}

		public void Copy(int dstPos, ref NativeBitArray srcBitArray, int srcPos, int numBits)
		{
			m_BitArray.Copy(dstPos, ref srcBitArray.m_BitArray, srcPos, numBits);
		}

		public int Find(int pos, int numBits)
		{
			return m_BitArray.Find(pos, numBits);
		}

		public int Find(int pos, int count, int numBits)
		{
			return m_BitArray.Find(pos, count, numBits);
		}

		public bool TestNone(int pos, int numBits = 1)
		{
			return m_BitArray.TestNone(pos, numBits);
		}

		public bool TestAny(int pos, int numBits = 1)
		{
			return m_BitArray.TestAny(pos, numBits);
		}

		public bool TestAll(int pos, int numBits = 1)
		{
			return m_BitArray.TestAll(pos, numBits);
		}

		public int CountBits(int pos, int numBits = 1)
		{
			return m_BitArray.CountBits(pos, numBits);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReadBounds<T>() where T : unmanaged
		{
			int num = UnsafeUtility.SizeOf<T>() * 8;
			int num2 = m_BitArray.Length / num;
			if (num2 == 0)
			{
				throw new InvalidOperationException($"Number of bits in the NativeBitArray {m_BitArray.Length} is not sufficient to cast to NativeArray<T> {UnsafeUtility.SizeOf<T>() * 8}.");
			}
			if (m_BitArray.Length != num * num2)
			{
				throw new InvalidOperationException($"Number of bits in the NativeBitArray {m_BitArray.Length} couldn't hold multiple of T {UnsafeUtility.SizeOf<T>()}. Output array would be truncated.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}
	}
}
