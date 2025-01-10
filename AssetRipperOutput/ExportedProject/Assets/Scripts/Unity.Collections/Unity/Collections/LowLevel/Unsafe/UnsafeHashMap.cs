using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("UnsafeHashMap is renamed to UnsafeParallelHashMap. (UnityUpgradable) -> UnsafeParallelHashMap<TKey, TValue>", false)]
	public struct UnsafeHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		[NativeContainerIsAtomicWriteOnly]
		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeParallelHashMapData* m_Buffer;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;

			public unsafe int Capacity => m_Buffer->keyCapacity;

			public unsafe bool TryAdd(TKey key, TValue item)
			{
				return UnsafeParallelHashMapBase<TKey, TValue>.TryAddAtomic(m_Buffer, key, item, m_ThreadIndex);
			}
		}

		public struct Enumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
		{
			internal UnsafeParallelHashMapDataEnumerator m_Enumerator;

			public KeyValue<TKey, TValue> Current => m_Enumerator.GetCurrent<TKey, TValue>();

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return m_Enumerator.MoveNext();
			}

			public void Reset()
			{
				m_Enumerator.Reset();
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeParallelHashMapData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		public unsafe bool IsEmpty
		{
			get
			{
				if (IsCreated)
				{
					return UnsafeParallelHashMapData.IsEmpty(m_Buffer);
				}
				return true;
			}
		}

		public unsafe int Capacity
		{
			get
			{
				return m_Buffer->keyCapacity;
			}
			set
			{
				UnsafeParallelHashMapData.ReallocateHashMap<TKey, TValue>(m_Buffer, value, UnsafeParallelHashMapData.GetBucketSize(value), m_AllocatorLabel);
			}
		}

		public unsafe TValue this[TKey key]
		{
			get
			{
				TryGetValue(key, out var item);
				return item;
			}
			set
			{
				if (UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out var _, out var it))
				{
					UnsafeParallelHashMapBase<TKey, TValue>.SetValue(m_Buffer, ref it, ref value);
				}
				else
				{
					UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, value, isMultiHashMap: false, m_AllocatorLabel);
				}
			}
		}

		public unsafe bool IsCreated => m_Buffer != null;

		public unsafe UnsafeHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			m_AllocatorLabel = allocator;
			UnsafeParallelHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out m_Buffer);
			Clear();
		}

		public unsafe int Count()
		{
			return UnsafeParallelHashMapData.GetCount(m_Buffer);
		}

		public unsafe void Clear()
		{
			UnsafeParallelHashMapBase<TKey, TValue>.Clear(m_Buffer);
		}

		public unsafe bool TryAdd(TKey key, TValue item)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, item, isMultiHashMap: false, m_AllocatorLabel);
		}

		public void Add(TKey key, TValue item)
		{
			TryAdd(key, item);
		}

		public unsafe bool Remove(TKey key)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.Remove(m_Buffer, key, isMultiHashMap: false) != 0;
		}

		public unsafe bool TryGetValue(TKey key, out TValue item)
		{
			NativeParallelMultiHashMapIterator<TKey> it;
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out it);
		}

		public unsafe bool ContainsKey(TKey key)
		{
			TValue item;
			NativeParallelMultiHashMapIterator<TKey> it;
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out it);
		}

		public unsafe void Dispose()
		{
			UnsafeParallelHashMapData.DeallocateHashMap(m_Buffer, m_AllocatorLabel);
			m_Buffer = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			UnsafeParallelHashMapDisposeJob jobData = default(UnsafeParallelHashMapDisposeJob);
			jobData.Data = m_Buffer;
			jobData.Allocator = m_AllocatorLabel;
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_Buffer = null;
			return result;
		}

		public unsafe NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TKey> result = CollectionHelper.CreateNativeArray<TKey>(UnsafeParallelHashMapData.GetCount(m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyArray(m_Buffer, result);
			return result;
		}

		public unsafe NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TValue> result = CollectionHelper.CreateNativeArray<TValue>(UnsafeParallelHashMapData.GetCount(m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetValueArray(m_Buffer, result);
			return result;
		}

		public unsafe NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			NativeKeyValueArrays<TKey, TValue> result = new NativeKeyValueArrays<TKey, TValue>(UnsafeParallelHashMapData.GetCount(m_Buffer), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyValueArrays(m_Buffer, result);
			return result;
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_ThreadIndex = 0;
			result.m_Buffer = m_Buffer;
			return result;
		}

		public unsafe Enumerator GetEnumerator()
		{
			Enumerator result = default(Enumerator);
			result.m_Enumerator = new UnsafeParallelHashMapDataEnumerator(m_Buffer);
			return result;
		}

		IEnumerator<KeyValue<TKey, TValue>> IEnumerable<KeyValue<TKey, TValue>>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
