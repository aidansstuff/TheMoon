using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Common
{
	internal static class ListUtil
	{
		public static void Resize<T>(this List<T> list, int size, T element = default(T))
		{
			int count = list.Count;
			int num = size - count;
			if (num < 0)
			{
				list.RemoveRange(size, count - size);
			}
			else if (num > 0)
			{
				if (size > list.Capacity)
				{
					list.Capacity = size;
				}
				for (int i = 0; i < num; i++)
				{
					list.Add(element);
				}
			}
		}

		public static void Resize<T>(this List<T> list, int size, Func<T> generator)
		{
			int count = list.Count;
			int num = size - count;
			if (num < 0)
			{
				list.RemoveRange(size, count - size);
			}
			else if (num > 0)
			{
				if (size > list.Capacity)
				{
					list.Capacity = size;
				}
				for (int i = 0; i < num; i++)
				{
					list.Add(generator());
				}
			}
		}
	}
}
