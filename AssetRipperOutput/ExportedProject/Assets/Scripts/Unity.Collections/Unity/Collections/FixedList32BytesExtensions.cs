using System;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class FixedList32BytesExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this ref FixedList32Bytes<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(list.Buffer, list.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<T, U>(this ref FixedList32Bytes<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return IndexOf(ref list, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Remove<T, U>(this ref FixedList32Bytes<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			int num = IndexOf(ref list, value);
			if (num < 0)
			{
				return false;
			}
			list.RemoveAt(num);
			return true;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool RemoveSwapBack<T, U>(this ref FixedList32Bytes<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			int num = IndexOf(ref list, value);
			if (num == -1)
			{
				return false;
			}
			list.RemoveAtSwapBack(num);
			return true;
		}
	}
}
