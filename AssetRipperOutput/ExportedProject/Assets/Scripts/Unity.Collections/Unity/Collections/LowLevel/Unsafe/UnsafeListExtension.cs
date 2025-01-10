using System;
using System.Collections.Generic;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	public static class UnsafeListExtension
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static ref UnsafeList ListData<T>(this ref UnsafeList<T> from) where T : unmanaged
		{
			return ref UnsafeUtility.As<UnsafeList<T>, UnsafeList>(ref from);
		}

		public static void Sort<T>(this UnsafeList list) where T : unmanaged, IComparable<T>
		{
			list.Sort<T, NativeSortExtension.DefaultComparer<T>>(default(NativeSortExtension.DefaultComparer<T>));
		}

		public unsafe static void Sort<T, U>(this UnsafeList list, U comp) where T : unmanaged where U : IComparer<T>
		{
			NativeSortExtension.IntroSort<T, U>(list.Ptr, list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<T>(this UnsafeList container, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return container.Sort<T, NativeSortExtension.DefaultComparer<T>>(default(NativeSortExtension.DefaultComparer<T>), inputDeps);
		}

		public unsafe static SortJob<T, NativeSortExtension.DefaultComparer<T>> SortJob<T>(this UnsafeList list) where T : unmanaged, IComparable<T>
		{
			return NativeSortExtension.SortJob((T*)list.Ptr, list.Length, default(NativeSortExtension.DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(this UnsafeList container, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			return NativeSortExtension.Sort((T*)container.Ptr, container.Length, comp, inputDeps);
		}

		public unsafe static SortJob<T, U> SortJob<T, U>(this UnsafeList list, U comp) where T : unmanaged where U : IComparer<T>
		{
			return NativeSortExtension.SortJob((T*)list.Ptr, list.Length, comp);
		}

		public static int BinarySearch<T>(this UnsafeList container, T value) where T : unmanaged, IComparable<T>
		{
			return container.BinarySearch(value, default(NativeSortExtension.DefaultComparer<T>));
		}

		public unsafe static int BinarySearch<T, U>(this UnsafeList container, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			return NativeSortExtension.BinarySearch((T*)container.Ptr, container.Length, value, comp);
		}
	}
}
