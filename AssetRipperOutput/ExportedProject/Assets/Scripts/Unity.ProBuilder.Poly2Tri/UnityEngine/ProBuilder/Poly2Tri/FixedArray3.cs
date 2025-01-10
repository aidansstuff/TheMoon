using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal struct FixedArray3<T> : IEnumerable<T>, IEnumerable where T : class
	{
		public T _0;

		public T _1;

		public T _2;

		public T this[int index]
		{
			get
			{
				return index switch
				{
					0 => _0, 
					1 => _1, 
					2 => _2, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			set
			{
				switch (index)
				{
				case 0:
					_0 = value;
					break;
				case 1:
					_1 = value;
					break;
				case 2:
					_2 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool Contains(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		public void Clear()
		{
			_0 = (_1 = (_2 = null));
		}

		public void Clear(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					this[i] = null;
				}
			}
		}

		private IEnumerable<T> Enumerate()
		{
			int i = 0;
			while (i < 3)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
