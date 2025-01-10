using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[Obsolete("NativeMultiHashMap is renamed to NativeParallelMultiHashMap. (UnityUpgradable) -> NativeParallelMultiHashMap<TKey, TValue>", false)]
	[NativeContainer]
	public struct NativeMultiHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		public struct ParallelWriter
		{
			internal UnsafeParallelMultiHashMap<TKey, TValue>.ParallelWriter m_Writer;

			public int m_ThreadIndex => m_Writer.m_ThreadIndex;

			public int Capacity => m_Writer.Capacity;

			public void Add(TKey key, TValue item)
			{
				m_Writer.Add(key, item);
			}
		}

		public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			internal NativeMultiHashMap<TKey, TValue> hashmap;

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

		[NativeContainer]
		[NativeContainerIsReadOnly]
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

		internal UnsafeParallelMultiHashMap<TKey, TValue> m_MultiHashMapData;

		public bool IsEmpty => m_MultiHashMapData.IsEmpty;

		public int Capacity
		{
			get
			{
				return m_MultiHashMapData.Capacity;
			}
			set
			{
				m_MultiHashMapData.Capacity = value;
			}
		}

		public bool IsCreated => m_MultiHashMapData.IsCreated;

		public NativeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
			: this(capacity, allocator, 2)
		{
		}

		internal void Initialize<U>(int capacity, ref U allocator, int disposeSentinelStackDepth) where U : unmanaged, AllocatorManager.IAllocator
		{
			m_MultiHashMapData = new UnsafeParallelMultiHashMap<TKey, TValue>(capacity, allocator.Handle);
		}

		private NativeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this = default(NativeMultiHashMap<TKey, TValue>);
			Initialize(capacity, ref allocator, disposeSentinelStackDepth);
		}

		public int Count()
		{
			return m_MultiHashMapData.Count();
		}

		public void Clear()
		{
			m_MultiHashMapData.Clear();
		}

		public void Add(TKey key, TValue item)
		{
			m_MultiHashMapData.Add(key, item);
		}

		public int Remove(TKey key)
		{
			return m_MultiHashMapData.Remove(key);
		}

		public void Remove(NativeParallelMultiHashMapIterator<TKey> it)
		{
			m_MultiHashMapData.Remove(it);
		}

		public bool TryGetFirstValue(TKey key, out TValue item, out NativeParallelMultiHashMapIterator<TKey> it)
		{
			return m_MultiHashMapData.TryGetFirstValue(key, out item, out it);
		}

		public bool TryGetNextValue(out TValue item, ref NativeParallelMultiHashMapIterator<TKey> it)
		{
			return m_MultiHashMapData.TryGetNextValue(out item, ref it);
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

		public bool SetValue(TValue item, NativeParallelMultiHashMapIterator<TKey> it)
		{
			return m_MultiHashMapData.SetValue(item, it);
		}

		public void Dispose()
		{
			m_MultiHashMapData.Dispose();
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			UnsafeParallelHashMapDataDisposeJob jobData = default(UnsafeParallelHashMapDataDisposeJob);
			jobData.Data = new UnsafeParallelHashMapDataDispose
			{
				m_Buffer = m_MultiHashMapData.m_Buffer,
				m_AllocatorLabel = m_MultiHashMapData.m_AllocatorLabel
			};
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_MultiHashMapData.m_Buffer = null;
			return result;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return m_MultiHashMapData.GetKeyArray(allocator);
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return m_MultiHashMapData.GetValueArray(allocator);
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return m_MultiHashMapData.GetKeyValueArrays(allocator);
		}

		public ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_Writer = m_MultiHashMapData.AsParallelWriter();
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

		public unsafe KeyValueEnumerator GetEnumerator()
		{
			KeyValueEnumerator result = default(KeyValueEnumerator);
			result.m_Enumerator = new UnsafeParallelHashMapDataEnumerator(m_MultiHashMapData.m_Buffer);
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

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}
	}
}
