using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal struct FixedBitArray3 : IEnumerable<bool>, IEnumerable
	{
		public bool _0;

		public bool _1;

		public bool _2;

		public bool this[int index]
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

		public bool Contains(bool value)
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

		public int IndexOf(bool value)
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
			_0 = (_1 = (_2 = false));
		}

		public void Clear(bool value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					this[i] = false;
				}
			}
		}

		private IEnumerable<bool> Enumerate()
		{
			int i = 0;
			while (i < 3)
			{
				yield return this[i];
				int num = i + 1;
				i = num;
			}
		}

		public IEnumerator<bool> GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
