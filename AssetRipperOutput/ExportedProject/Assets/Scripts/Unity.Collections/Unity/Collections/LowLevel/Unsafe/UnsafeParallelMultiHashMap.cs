using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerTypeProxy(typeof(UnsafeParallelMultiHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct UnsafeParallelMultiHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			internal UnsafeParallelMultiHashMap<TKey, TValue> hashmap;

			internal TKey key;

			internal bool isFirst;

			private TValue value;

			private NativeParallelMultiHashMapIterator<TKey> iterator;

			public TValue Current => value;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				if (isFirst)
				{
					isFirst = false;
					return hashmap.TryGetFirstValue(key, out value, out iterator);
				}
				return hashmap.TryGetNextValue(out value, ref iterator);
			}

			public void Reset()
			{
				isFirst = true;
			}

			public Enumerator GetEnumerator()
			{
				return this;
			}
		}

		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeParallelHashMapData* m_Buffer;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;

			public unsafe int Capacity => m_Buffer->keyCapacity;

			public unsafe void Add(TKey key, TValue item)
			{
				UnsafeParallelHashMapBase<TKey, TValue>.AddAtomicMulti(m_Buffer, key, item, m_ThreadIndex);
			}
		}

		public struct KeyValueEnumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
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

		public unsafe bool IsCreated => m_Buffer != null;

		public unsafe UnsafeParallelMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			m_AllocatorLabel = allocator;
			UnsafeParallelHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out m_Buffer);
			Clear();
		}

		public unsafe int Count()
		{
			if (m_Buffer->allocatedIndexLength <= 0)
			{
				return 0;
			}
			return UnsafeParallelHashMapData.GetCount(m_Buffer);
		}

		public unsafe void Clear()
		{
			UnsafeParallelHashMapBase<TKey, TValue>.Clear(m_Buffer);
		}

		public unsafe void Add(TKey key, TValue item)
		{
			UnsafeParallelHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, item, isMultiHashMap: true, m_AllocatorLabel);
		}

		public unsafe int Remove(TKey key)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.Remove(m_Buffer, key, isMultiHashMap: true);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void Remove<TValueEQ>(TKey key, TValueEQ value) where TValueEQ : struct, IEquatable<TValueEQ>
		{
			UnsafeParallelHashMapBase<TKey, TValueEQ>.RemoveKeyValue(m_Buffer, key, value);
		}

		public unsafe void Remove(NativeParallelMultiHashMapIterator<TKey> it)
		{
			UnsafeParallelHashMapBase<TKey, TValue>.Remove(m_Buffer, it);
		}

		public unsafe bool TryGetFirstValue(TKey key, out TValue item, out NativeParallelMultiHashMapIterator<TKey> it)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out it);
		}

		public unsafe bool TryGetNextValue(out TValue item, ref NativeParallelMultiHashMapIterator<TKey> it)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.TryGetNextValueAtomic(m_Buffer, out item, ref it);
		}

		public bool ContainsKey(TKey key)
		{
			TValue item;
			NativeParallelMultiHashMapIterator<TKey> it;
			return TryGetFirstValue(key, out item, out it);
		}

		public int CountValuesForKey(TKey key)
		{
			if (!TryGetFirstValue(key, out var item, out var it))
			{
				return 0;
			}
			int num = 1;
			while (TryGetNextValue(out item, ref it))
			{
				num++;
			}
			return num;
		}

		public unsafe bool SetValue(TValue item, NativeParallelMultiHashMapIterator<TKey> it)
		{
			return UnsafeParallelHashMapBase<TKey, TValue>.SetValue(m_Buffer, ref it, ref item);
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
			NativeArray<TKey> result = CollectionHelper.CreateNativeArray<TKey>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyArray(m_Buffer, result);
			return result;
		}

		public unsafe NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<TValue> result = CollectionHelper.CreateNativeArray<TValue>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetValueArray(m_Buffer, result);
			return result;
		}

		public unsafe NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			NativeKeyValueArrays<TKey, TValue> result = new NativeKeyValueArrays<TKey, TValue>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeParallelHashMapData.GetKeyValueArrays(m_Buffer, result);
			return result;
		}

		public Enumerator GetValuesForKey(TKey key)
		{
			Enumerator result = default(Enumerator);
			result.hashmap = this;
			result.key = key;
			result.isFirst = true;
			return result;
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_ThreadIndex = 0;
			result.m_Buffer = m_Buffer;
			return result;
		}

		public unsafe KeyValueEnumerator GetEnumerator()
		{
			KeyValueEnumerator result = default(KeyValueEnumerator);
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
