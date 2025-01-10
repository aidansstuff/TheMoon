using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class UnsafeListExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this UnsafeList<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(list.Ptr, list.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<T, U>(this UnsafeList<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return list.IndexOf(value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this UnsafeList<T>.ParallelReader list, U value) where T : unmanaged, IEquatable<U>
		{
			return NativeArrayExtensions.IndexOf<T, U>(list.Ptr, list.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public static bool Contains<T, U>(this UnsafeList<T>.ParallelReader list, U value) where T : unmanaged, IEquatable<U>
		{
			return list.IndexOf(value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<T>(this UnsafeList<T> array, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
		{
			if (array.Length != other.Length)
			{
				return false;
			}
			for (int i = 0; i != array.Length; i++)
			{
				if (!array[i].Equals(other[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
