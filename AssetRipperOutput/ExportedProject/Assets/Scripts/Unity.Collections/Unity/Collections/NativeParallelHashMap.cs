using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Count = {m_HashMapData.Count()}, Capacity = {m_HashMapData.Capacity}, IsCreated = {m_HashMapData.IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(NativeParallelHashMapDebuggerTypeProxy<, >))]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeParallelHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[DebuggerDisplay("Capacity = {m_Writer.Capacity}")]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public struct ParallelWriter
		{
			internal UnsafeParallelHashMap<TKey, TValue>.ParallelWriter m_Writer;

			public int m_ThreadIndex => m_Writer.m_ThreadIndex;

			public int Capacity => m_Writer.Capacity;

			public bool TryAdd(TKey key, TValue item)
			{
				return m_Writer.TryAdd(key, item);
			}
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
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

		internal UnsafeParallelHashMap<TKey, TValue> m_HashMapData;

		public bool IsEmpty
		{
			get
			{
				if (!IsCreated)
				{
					return true;
				}
				return m_HashMapData.IsEmpty;
			}
		}

		public int Capacity
		{
			get
			{
				return m_HashMapData.Capacity;
			}
			set
			{
				m_HashMapData.Capacity = value;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				if (m_HashMapData.TryGetValue(key, out var item))
				{
					return item;
				}
				return default(TValue);
			}
			set
			{
				m_HashMapData[key] = value;
			}
		}

		public bool IsCreated => m_HashMapData.IsCreated;

		public NativeParallelHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
			: this(capacity, allocator, 2)
		{
		}

		private NativeParallelHashMap(int capacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			m_HashMapData = new UnsafeParallelHashMap<TKey, TValue>(capacity, allocator);
		}

		public int Count()
		{
			return m_HashMapData.Count();
		}

		public void Clear()
		{
			m_HashMapData.Clear();
		}

		public bool TryAdd(TKey key, TValue item)
		{
			return m_HashMapData.TryAdd(key, item);
		}

		public void Add(TKey key, TValue item)
		{
			TryAdd(key, item);
		}

		public bool Remove(TKey key)
		{
			return m_HashMapData.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue item)
		{
			return m_HashMapData.TryGetValue(key, out item);
		}

		public bool ContainsKey(TKey key)
		{
			return m_HashMapData.ContainsKey(key);
		}

		public void Dispose()
		{
			m_HashMapData.Dispose();
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			UnsafeParallelHashMapDataDisposeJob jobData = default(UnsafeParallelHashMapDataDisposeJob);
			jobData.Data = new UnsafeParallelHashMapDataDispose
			{
				m_Buffer = m_HashMapData.m_Buffer,
				m_AllocatorLabel = m_HashMapData.m_AllocatorLabel
			};
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_HashMapData.m_Buffer = null;
			return result;
		}

		public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
		{
			return m_HashMapData.GetKeyArray(allocator);
		}

		public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
		{
			return m_HashMapData.GetValueArray(allocator);
		}

		public NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
		{
			return m_HashMapData.GetKeyValueArrays(allocator);
		}

		public ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_Writer = m_HashMapData.AsParallelWriter();
			return result;
		}

		public unsafe Enumerator GetEnumerator()
		{
			Enumerator result = default(Enumerator);
			result.m_Enumerator = new UnsafeParallelHashMapDataEnumerator(m_HashMapData.m_Buffer);
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

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowKeyNotPresent(TKey key)
		{
			throw new ArgumentException($"Key: {key} is not present in the NativeParallelHashMap.");
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowKeyAlreadyAdded(TKey key)
		{
			throw new ArgumentException("An item with the same key has already been added", "key");
		}
	}
}
