using System;
using System.Collections.Generic;

namespace Unity.Services.Core.Scheduler.Internal
{
	internal abstract class MinimumBinaryHeap
	{
		internal const float IncreaseFactor = 1.5f;

		internal const float DecreaseFactor = 2f;
	}
	internal class MinimumBinaryHeap<T> : MinimumBinaryHeap
	{
		private readonly object m_Lock = new object();

		private readonly IComparer<T> m_Comparer;

		private readonly int m_MinimumCapacity;

		private T[] m_HeapArray;

		internal IReadOnlyList<T> HeapArray => m_HeapArray;

		public int Count { get; private set; }

		public T Min => m_HeapArray[0];

		public MinimumBinaryHeap(int minimumCapacity = 10)
			: this((IComparer<T>)Comparer<T>.Default, minimumCapacity)
		{
		}

		public MinimumBinaryHeap(IComparer<T> comparer, int minimumCapacity = 10)
			: this((ICollection<T>)null, comparer, minimumCapacity)
		{
		}

		internal MinimumBinaryHeap(ICollection<T> collection, IComparer<T> comparer, int minimumCapacity = 10)
		{
			if (minimumCapacity <= 0)
			{
				throw new ArgumentException("capacity must be more than 0");
			}
			m_MinimumCapacity = minimumCapacity;
			m_Comparer = comparer;
			lock (m_Lock)
			{
				Count = collection?.Count ?? 0;
				int num = Math.Max(Count, minimumCapacity);
				m_HeapArray = new T[num];
				if (collection == null)
				{
					return;
				}
				Count = 0;
				foreach (T item in collection)
				{
					Insert(item);
				}
			}
		}

		public void Insert(T item)
		{
			lock (m_Lock)
			{
				IncreaseHeapCapacityWhenFull();
				int num = Count;
				m_HeapArray[Count] = item;
				Count++;
				while (num != 0 && m_Comparer.Compare(m_HeapArray[num], m_HeapArray[GetParentIndex(num)]) < 0)
				{
					Swap(ref m_HeapArray[num], ref m_HeapArray[GetParentIndex(num)]);
					num = GetParentIndex(num);
				}
			}
		}

		private void IncreaseHeapCapacityWhenFull()
		{
			if (Count == m_HeapArray.Length)
			{
				T[] array = new T[(int)Math.Ceiling((float)Count * 1.5f)];
				Array.Copy(m_HeapArray, array, Count);
				m_HeapArray = array;
			}
		}

		public void Remove(T item)
		{
			lock (m_Lock)
			{
				int num = IndexOf(item);
				if (num >= 0)
				{
					while (num != 0)
					{
						Swap(ref m_HeapArray[num], ref m_HeapArray[GetParentIndex(num)]);
						num = GetParentIndex(num);
					}
					ExtractMin();
				}
			}
		}

		private int IndexOf(T item)
		{
			for (int i = 0; i < Count; i++)
			{
				if (m_HeapArray[i].Equals(item))
				{
					return i;
				}
			}
			return -1;
		}

		public T ExtractMin()
		{
			lock (m_Lock)
			{
				if (Count <= 0)
				{
					throw new InvalidOperationException("Can not ExtractMin: BinaryHeap is empty.");
				}
				T result = m_HeapArray[0];
				if (Count == 1)
				{
					Count--;
					m_HeapArray[0] = default(T);
					return result;
				}
				Count--;
				m_HeapArray[0] = m_HeapArray[Count];
				m_HeapArray[Count] = default(T);
				MinHeapify();
				DecreaseHeapCapacityWhenSpare();
				return result;
			}
		}

		private void DecreaseHeapCapacityWhenSpare()
		{
			int num = (int)Math.Ceiling((float)m_HeapArray.Length / 2f);
			if (Count > m_MinimumCapacity && Count <= num)
			{
				T[] array = new T[Count];
				Array.Copy(m_HeapArray, array, Count);
				m_HeapArray = array;
			}
		}

		private void MinHeapify()
		{
			int smallest = 0;
			int currentIndex;
			do
			{
				currentIndex = smallest;
				UpdateSmallestIndex();
				if (smallest == currentIndex)
				{
					break;
				}
				Swap(ref m_HeapArray[currentIndex], ref m_HeapArray[smallest]);
			}
			while (smallest != currentIndex);
			void UpdateSmallestIfCandidateIsSmaller(int candidate)
			{
				if (candidate < Count && m_Comparer.Compare(m_HeapArray[candidate], m_HeapArray[smallest]) < 0)
				{
					smallest = candidate;
				}
			}
			void UpdateSmallestIndex()
			{
				smallest = currentIndex;
				int leftChildIndex = GetLeftChildIndex(currentIndex);
				int rightChildIndex = GetRightChildIndex(currentIndex);
				UpdateSmallestIfCandidateIsSmaller(leftChildIndex);
				UpdateSmallestIfCandidateIsSmaller(rightChildIndex);
			}
		}

		private static void Swap(ref T lhs, ref T rhs)
		{
			T val = rhs;
			T val2 = lhs;
			lhs = val;
			rhs = val2;
		}

		private static int GetParentIndex(int index)
		{
			return (index - 1) / 2;
		}

		private static int GetLeftChildIndex(int index)
		{
			return 2 * index + 1;
		}

		private static int GetRightChildIndex(int index)
		{
			return 2 * index + 2;
		}
	}
}
