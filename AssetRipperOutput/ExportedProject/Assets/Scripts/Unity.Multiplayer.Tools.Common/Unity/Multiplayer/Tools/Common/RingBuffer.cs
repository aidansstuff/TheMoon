using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Multiplayer.Tools.Common
{
	internal class RingBuffer<T> : IEnumerable<T>, IEnumerable
	{
		[CanBeNull]
		private T[] m_Buffer;

		private int m_Begin;

		public int Length { get; set; }

		public int Capacity
		{
			get
			{
				T[] buffer = m_Buffer;
				if (buffer == null)
				{
					return 0;
				}
				return buffer.Length;
			}
			set
			{
				ThrowIfCapacityLessThanZero(value);
				UpdateCapacity(value);
			}
		}

		public T this[int index]
		{
			get
			{
				ThrowIfIndexOutOfRange(index);
				return m_Buffer[GetBufferIndex(index)];
			}
			set
			{
				ThrowIfIndexOutOfRange(index);
				m_Buffer[GetBufferIndex(index)] = value;
			}
		}

		public T this[Index index]
		{
			get
			{
				ThrowIfIndexOutOfRange(index);
				return m_Buffer[GetBufferIndex(index)];
			}
			set
			{
				ThrowIfIndexOutOfRange(index);
				m_Buffer[GetBufferIndex(index)] = value;
			}
		}

		public T LeastRecent => this[0];

		public T LeastRecentOrDefault
		{
			get
			{
				if (Length <= 0)
				{
					return default(T);
				}
				return LeastRecent;
			}
		}

		public T MostRecent => this[^1];

		public T MostRecentOrDefault
		{
			get
			{
				if (Length <= 0)
				{
					return default(T);
				}
				return MostRecent;
			}
		}

		public RingBuffer(int capacity)
		{
			ThrowIfCapacityLessThanZero(capacity);
			if (capacity > 0)
			{
				m_Buffer = new T[capacity];
			}
			else
			{
				m_Buffer = null;
			}
			m_Begin = 0;
			Length = 0;
		}

		public RingBuffer(T[] values)
		{
			m_Buffer = values;
			m_Begin = 0;
			Length = values.Length;
		}

		public void PushBack(T value)
		{
			int capacity = Capacity;
			if (capacity > 0)
			{
				int num = (m_Begin + Length) % capacity;
				m_Buffer[num] = value;
				if (Length < capacity)
				{
					Length++;
				}
				else
				{
					m_Begin = (m_Begin + 1) % capacity;
				}
			}
		}

		public void Clear()
		{
			Length = 0;
		}

		private void UpdateCapacity(int newCapacity)
		{
			int capacity = Capacity;
			if (newCapacity == capacity)
			{
				return;
			}
			if (newCapacity == 0)
			{
				m_Buffer = null;
				m_Begin = 0;
				Length = 0;
				return;
			}
			T[] buffer = m_Buffer;
			int begin = m_Begin;
			int length = Length;
			m_Buffer = new T[newCapacity];
			m_Begin = 0;
			Length = Math.Min(length, newCapacity);
			int num = begin + (length - Length);
			for (int i = 0; i < Length; i++)
			{
				int num2 = (i + num) % capacity;
				m_Buffer[i] = buffer[num2];
			}
		}

		private bool ContainsIndex(int index)
		{
			if (0 <= index)
			{
				return index < Length;
			}
			return false;
		}

		private bool ContainsIndex(Index index)
		{
			return ContainsIndex(index.IsFromEnd ? (index.Value - 1) : index.Value);
		}

		private void ThrowIfIndexOutOfRange(int index)
		{
			if (!ContainsIndex(index))
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Length})");
			}
		}

		private void ThrowIfCapacityLessThanZero(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentException($"RingBuffer capacity argument {capacity} is < 0");
			}
		}

		private void ThrowIfIndexOutOfRange(Index index)
		{
			if (!ContainsIndex(index))
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range [0, {Length})");
			}
		}

		private int GetBufferIndex(int index)
		{
			return (index + m_Begin) % Capacity;
		}

		private int GetBufferIndexFromEnd(int index)
		{
			return GetBufferIndex(Length - 1 - index);
		}

		private int GetBufferIndex(Index index)
		{
			if (index.IsFromEnd)
			{
				return GetBufferIndexFromEnd(index.Value - 1);
			}
			return GetBufferIndex(index.Value);
		}

		public T GetValueOrDefault(int index)
		{
			if (ContainsIndex(index))
			{
				return this[index];
			}
			return default(T);
		}

		public T GetValueOrDefault(Index index)
		{
			if (ContainsIndex(index))
			{
				return this[index];
			}
			return default(T);
		}

		public IEnumerator<T> GetEnumerator()
		{
			int i = 0;
			while (i < Length)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
