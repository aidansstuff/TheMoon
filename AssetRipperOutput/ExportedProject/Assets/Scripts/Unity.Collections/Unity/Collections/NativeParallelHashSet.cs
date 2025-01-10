using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[DebuggerTypeProxy(typeof(NativeHashSetDebuggerTypeProxy<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeParallelHashSet<T> : INativeDisposable, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged, IEquatable<T>
	{
		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			internal NativeParallelHashMap<T, bool>.ParallelWriter m_Data;

			public int Capacity => m_Data.Capacity;

			public bool Add(T item)
			{
				return m_Data.TryAdd(item, item: false);
			}
		}

		[NativeContainer]
		[NativeContainerIsReadOnly]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			internal UnsafeParallelHashMapDataEnumerator m_Enumerator;

			public T Current => m_Enumerator.GetCurrentKey<T>();

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

		internal NativeParallelHashMap<T, bool> m_Data;

		public bool IsEmpty => m_Data.IsEmpty;

		public int Capacity
		{
			get
			{
				return m_Data.Capacity;
			}
			set
			{
				m_Data.Capacity = value;
			}
		}

		public bool IsCreated => m_Data.IsCreated;

		public NativeParallelHashSet(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			m_Data = new NativeParallelHashMap<T, bool>(capacity, allocator);
		}

		public int Count()
		{
			return m_Data.Count();
		}

		public void Dispose()
		{
			m_Data.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return m_Data.Dispose(inputDeps);
		}

		public void Clear()
		{
			m_Data.Clear();
		}

		public bool Add(T item)
		{
			return m_Data.TryAdd(item, item: false);
		}

		public bool Remove(T item)
		{
			return m_Data.Remove(item);
		}

		public bool Contains(T item)
		{
			return m_Data.ContainsKey(item);
		}

		public NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
		{
			return m_Data.GetKeyArray(allocator);
		}

		public ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_Data = m_Data.AsParallelWriter();
			return result;
		}

		public unsafe Enumerator GetEnumerator()
		{
			Enumerator result = default(Enumerator);
			result.m_Enumerator = new UnsafeParallelHashMapDataEnumerator(m_Data.m_HashMapData.m_Buffer);
			return result;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
