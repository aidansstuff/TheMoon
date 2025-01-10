using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections.NotBurstCompatible
{
	public static class Extensions
	{
		[NotBurstCompatible]
		public static T[] ToArray<T>(this NativeParallelHashSet<T> set) where T : unmanaged, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] result = nativeArray.ToArray();
			nativeArray.Dispose();
			return result;
		}

		[NotBurstCompatible]
		public static T[] ToArrayNBC<T>(this NativeList<T> list) where T : unmanaged
		{
			return list.AsArray().ToArray();
		}

		[NotBurstCompatible]
		public static void CopyFromNBC<T>(this NativeList<T> list, T[] array) where T : unmanaged
		{
			list.Clear();
			list.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			list.AsArray().CopyFrom(array);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Obsolete("Burst now supports tuple, please use `GetUniqueKeyArray` method from `Unity.Collections.UnsafeParallelMultiHashMap` instead.", false)]
		public static (NativeArray<TKey>, int) GetUniqueKeyArrayNBC<TKey, TValue>(this UnsafeParallelMultiHashMap<TKey, TValue> hashmap, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			return hashmap.GetUniqueKeyArray(allocator);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Obsolete("Burst now supports tuple, please use `GetUniqueKeyArray` method from `Unity.Collections.NativeParallelMultiHashMap` instead.", false)]
		public static (NativeArray<TKey>, int) GetUniqueKeyArrayNBC<TKey, TValue>(this NativeParallelMultiHashMap<TKey, TValue> hashmap, AllocatorManager.AllocatorHandle allocator) where TKey : struct, IEquatable<TKey>, IComparable<TKey> where TValue : struct
		{
			return hashmap.GetUniqueKeyArray(allocator);
		}
	}
}
