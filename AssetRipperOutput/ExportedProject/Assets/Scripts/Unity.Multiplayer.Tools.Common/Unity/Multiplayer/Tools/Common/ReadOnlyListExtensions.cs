using System;
using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.Common
{
	internal static class ReadOnlyListExtensions
	{
		public static int IndexOf<T>(this IReadOnlyList<T> list, T elementToFind) where T : IEquatable<T>
		{
			int num = 0;
			foreach (T item in list)
			{
				if (item.Equals(elementToFind))
				{
					return num;
				}
				num++;
			}
			return -1;
		}
	}
}
