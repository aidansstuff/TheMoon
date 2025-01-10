using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeSortExtension
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct DefaultComparer<T> : IComparer<T> where T : IComparable<T>
		{
			public int Compare(T x, T y)
			{
				return x.CompareTo(y);
			}
		}

		[BurstCompile]
		private struct SegmentSort<T, U> : IJobParallelFor where T : unmanaged where U : IComparer<T>
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe T* Data;

			public U Comp;

			public int Length;

			public int SegmentWidth;

			public unsafe void Execute(int index)
			{
				int num = index * SegmentWidth;
				int length = ((Length - num < SegmentWidth) ? (Length - num) : SegmentWidth);
				Sort(Data + num, length, Comp);
			}
		}

		[BurstCompile]
		private struct SegmentSortMerge<T, U> : IJob where T : unmanaged where U : IComparer<T>
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe T* Data;

			public U Comp;

			public int Length;

			public int SegmentWidth;

			public unsafe void Execute()
			{
				int num = (Length + (SegmentWidth - 1)) / SegmentWidth;
				int* ptr = stackalloc int[num];
				T* ptr2 = (T*)Memory.Unmanaged.Allocate(UnsafeUtility.SizeOf<T>() * Length, 16, Allocator.Temp);
				for (int i = 0; i < Length; i++)
				{
					int num2 = -1;
					T val = default(T);
					for (int j = 0; j < num; j++)
					{
						int num3 = j * SegmentWidth;
						int num4 = ptr[j];
						int num5 = ((Length - num3 < SegmentWidth) ? (Length - num3) : SegmentWidth);
						if (num4 != num5)
						{
							T val2 = Data[num3 + num4];
							if (num2 == -1 || Comp.Compare(val2, val) <= 0)
							{
								val = val2;
								num2 = j;
							}
						}
					}
					ptr[num2]++;
					ptr2[i] = val;
				}
				UnsafeUtility.MemCpy(Data, ptr2, UnsafeUtility.SizeOf<T>() * Length);
			}
		}

		private const int k_IntrosortSizeThreshold = 16;

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(T* array, int length) where T : unmanaged, IComparable<T>
		{
			IntroSort<T, DefaultComparer<T>>(array, length, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(T* array, int length, U comp) where T : unmanaged where U : IComparer<T>
		{
			IntroSort<T, U>(array, length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(T*, int).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T>(T* array, int length, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return Sort(array, length, default(DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static SortJob<T, DefaultComparer<T>> SortJob<T>(T* array, int length) where T : unmanaged, IComparable<T>
		{
			SortJob<T, DefaultComparer<T>> result = default(SortJob<T, DefaultComparer<T>>);
			result.Data = array;
			result.Length = length;
			result.Comp = default(DefaultComparer<T>);
			return result;
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(T*, int, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(T* array, int length, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			if (length == 0)
			{
				return inputDeps;
			}
			int num = (length + 1023) / 1024;
			int num2 = math.max(1, 128);
			int innerloopBatchCount = num / num2;
			SegmentSort<T, U> jobData = default(SegmentSort<T, U>);
			jobData.Data = array;
			jobData.Comp = comp;
			jobData.Length = length;
			jobData.SegmentWidth = 1024;
			JobHandle dependsOn = IJobParallelForExtensions.Schedule(jobData, num, innerloopBatchCount, inputDeps);
			SegmentSortMerge<T, U> jobData2 = default(SegmentSortMerge<T, U>);
			jobData2.Data = array;
			jobData2.Comp = comp;
			jobData2.Length = length;
			jobData2.SegmentWidth = 1024;
			return IJobExtensions.Schedule(jobData2, dependsOn);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static SortJob<T, U> SortJob<T, U>(T* array, int length, U comp) where T : unmanaged where U : IComparer<T>
		{
			SortJob<T, U> result = default(SortJob<T, U>);
			result.Data = array;
			result.Length = length;
			result.Comp = comp;
			return result;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static int BinarySearch<T>(T* ptr, int length, T value) where T : unmanaged, IComparable<T>
		{
			return BinarySearch(ptr, length, value, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<T, U>(T* ptr, int length, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			int num = 0;
			for (int num2 = length; num2 != 0; num2 >>= 1)
			{
				int num3 = num + (num2 >> 1);
				T y = ptr[num3];
				int num4 = comp.Compare(value, y);
				if (num4 == 0)
				{
					return num3;
				}
				if (num4 > 0)
				{
					num = num3 + 1;
					num2--;
				}
			}
			return ~num;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this NativeArray<T> array) where T : struct, IComparable<T>
		{
			IntroSortStruct<T, DefaultComparer<T>>(array.GetUnsafePtr(), array.Length, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this NativeArray<T> array, U comp) where T : struct where U : IComparer<T>
		{
			IntroSortStruct<T, U>(array.GetUnsafePtr(), array.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeArray<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T>(this NativeArray<T> array, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return Sort((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), array.Length, default(DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static SortJob<T, DefaultComparer<T>> SortJob<T>(this NativeArray<T> array) where T : unmanaged, IComparable<T>
		{
			return SortJob((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), array.Length, default(DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeArray<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(this NativeArray<T> array, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			return Sort((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), array.Length, comp, inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static SortJob<T, U> SortJob<T, U>(this NativeArray<T> array, U comp) where T : unmanaged where U : IComparer<T>
		{
			SortJob<T, U> result = default(SortJob<T, U>);
			result.Data = (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);
			result.Length = array.Length;
			result.Comp = comp;
			return result;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<T>(this NativeArray<T> array, T value) where T : unmanaged, IComparable<T>
		{
			return array.BinarySearch(value, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<T, U>(this NativeArray<T> array, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			return BinarySearch((T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), array.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<T>(this NativeList<T> list) where T : unmanaged, IComparable<T>
		{
			list.Sort(default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this NativeList<T> list, U comp) where T : unmanaged where U : IComparer<T>
		{
			IntroSort<T, U>(list.GetUnsafePtr(), list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeList<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<T>(this NativeList<T> array, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return array.Sort(default(DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static SortJob<T, DefaultComparer<T>> SortJob<T>(this NativeList<T> list) where T : unmanaged, IComparable<T>
		{
			return SortJob((T*)list.GetUnsafePtr(), list.Length, default(DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeList<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(this NativeList<T> list, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			return Sort((T*)list.GetUnsafePtr(), list.Length, comp, inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static SortJob<T, U> SortJob<T, U>(this NativeList<T> list, U comp) where T : unmanaged where U : IComparer<T>
		{
			return SortJob((T*)list.GetUnsafePtr(), list.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<T>(this NativeList<T> list, T value) where T : unmanaged, IComparable<T>
		{
			return list.BinarySearch(value, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<T, U>(this NativeList<T> list, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			return BinarySearch((T*)list.GetUnsafePtr(), list.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<T>(this UnsafeList<T> list) where T : unmanaged, IComparable<T>
		{
			list.Sort(default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this UnsafeList<T> list, U comp) where T : unmanaged where U : IComparer<T>
		{
			IntroSort<T, U>(list.Ptr, list.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<T>(this UnsafeList<T> list, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return list.Sort(default(DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static SortJob<T, DefaultComparer<T>> SortJob<T>(this UnsafeList<T> list) where T : unmanaged, IComparable<T>
		{
			return SortJob(list.Ptr, list.Length, default(DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this UnsafeList<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(this UnsafeList<T> list, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			return Sort(list.Ptr, list.Length, comp, inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static SortJob<T, U> SortJob<T, U>(this UnsafeList<T> list, U comp) where T : unmanaged where U : IComparer<T>
		{
			return SortJob(list.Ptr, list.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<T>(this UnsafeList<T> list, T value) where T : unmanaged, IComparable<T>
		{
			return list.BinarySearch(value, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<T, U>(this UnsafeList<T> list, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			return BinarySearch(list.Ptr, list.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static void Sort<T>(this NativeSlice<T> slice) where T : struct, IComparable<T>
		{
			slice.Sort(default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this NativeSlice<T> slice, U comp) where T : struct where U : IComparer<T>
		{
			IntroSortStruct<T, U>(slice.GetUnsafePtr(), slice.Length, comp);
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeSlice<T>).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public static JobHandle Sort<T>(this NativeSlice<T> slice, JobHandle inputDeps) where T : unmanaged, IComparable<T>
		{
			return slice.Sort(default(DefaultComparer<T>), inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static SortJob<T, DefaultComparer<T>> SortJob<T>(this NativeSlice<T> slice) where T : unmanaged, IComparable<T>
		{
			return SortJob((T*)slice.GetUnsafePtr(), slice.Length, default(DefaultComparer<T>));
		}

		[NotBurstCompatible]
		[Obsolete("Instead call SortJob(this NativeSlice<T>, U).Schedule(JobHandle). (RemovedAfter 2021-06-20)", false)]
		public unsafe static JobHandle Sort<T, U>(this NativeSlice<T> slice, U comp, JobHandle inputDeps) where T : unmanaged where U : IComparer<T>
		{
			return Sort((T*)slice.GetUnsafePtr(), slice.Length, comp, inputDeps);
		}

		[BurstCompatible(RequiredUnityDefine = "UNITY_2020_2_OR_NEWER", GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static SortJob<T, U> SortJob<T, U>(this NativeSlice<T> slice, U comp) where T : unmanaged where U : IComparer<T>
		{
			return SortJob((T*)slice.GetUnsafePtr(), slice.Length, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static int BinarySearch<T>(this NativeSlice<T> slice, T value) where T : unmanaged, IComparable<T>
		{
			return slice.BinarySearch(value, default(DefaultComparer<T>));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		public unsafe static int BinarySearch<T, U>(this NativeSlice<T> slice, T value, U comp) where T : unmanaged where U : IComparer<T>
		{
			return BinarySearch((T*)slice.GetUnsafePtr(), slice.Length, value, comp);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(DefaultComparer<int>)
		})]
		internal unsafe static void IntroSort<T, U>(void* array, int length, U comp) where T : unmanaged where U : IComparer<T>
		{
			IntroSort<T, U>(array, 0, length - 1, 2 * CollectionHelper.Log2Floor(length), comp);
		}

		private unsafe static void IntroSort<T, U>(void* array, int lo, int hi, int depth, U comp) where T : unmanaged where U : IComparer<T>
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				if (num <= 16)
				{
					switch (num)
					{
					case 1:
						break;
					case 2:
						SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
						break;
					case 3:
						SwapIfGreaterWithItems<T, U>(array, lo, hi - 1, comp);
						SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
						SwapIfGreaterWithItems<T, U>(array, hi - 1, hi, comp);
						break;
					default:
						InsertionSort<T, U>(array, lo, hi, comp);
						break;
					}
					break;
				}
				if (depth == 0)
				{
					HeapSort<T, U>(array, lo, hi, comp);
					break;
				}
				depth--;
				int num2 = Partition<T, U>(array, lo, hi, comp);
				IntroSort<T, U>(array, num2 + 1, hi, depth, comp);
				hi = num2 - 1;
			}
		}

		private unsafe static void InsertionSort<T, U>(void* array, int lo, int hi, U comp) where T : unmanaged where U : IComparer<T>
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				T val = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
				while (num >= lo && comp.Compare(val, UnsafeUtility.ReadArrayElement<T>(array, num)) < 0)
				{
					UnsafeUtility.WriteArrayElement(array, num + 1, UnsafeUtility.ReadArrayElement<T>(array, num));
					num--;
				}
				UnsafeUtility.WriteArrayElement(array, num + 1, val);
			}
		}

		private unsafe static int Partition<T, U>(void* array, int lo, int hi, U comp) where T : unmanaged where U : IComparer<T>
		{
			int num = lo + (hi - lo) / 2;
			SwapIfGreaterWithItems<T, U>(array, lo, num, comp);
			SwapIfGreaterWithItems<T, U>(array, lo, hi, comp);
			SwapIfGreaterWithItems<T, U>(array, num, hi, comp);
			T x = UnsafeUtility.ReadArrayElement<T>(array, num);
			Swap<T>(array, num, hi - 1);
			int num2 = lo;
			int num3 = hi - 1;
			while (num2 < num3)
			{
				while (comp.Compare(x, UnsafeUtility.ReadArrayElement<T>(array, ++num2)) > 0)
				{
				}
				while (comp.Compare(x, UnsafeUtility.ReadArrayElement<T>(array, --num3)) < 0)
				{
				}
				if (num2 >= num3)
				{
					break;
				}
				Swap<T>(array, num2, num3);
			}
			Swap<T>(array, num2, hi - 1);
			return num2;
		}

		private unsafe static void HeapSort<T, U>(void* array, int lo, int hi, U comp) where T : unmanaged where U : IComparer<T>
		{
			int num = hi - lo + 1;
			for (int num2 = num / 2; num2 >= 1; num2--)
			{
				Heapify<T, U>(array, num2, num, lo, comp);
			}
			for (int num3 = num; num3 > 1; num3--)
			{
				Swap<T>(array, lo, lo + num3 - 1);
				Heapify<T, U>(array, 1, num3 - 1, lo, comp);
			}
		}

		private unsafe static void Heapify<T, U>(void* array, int i, int n, int lo, U comp) where T : unmanaged where U : IComparer<T>
		{
			T val = UnsafeUtility.ReadArrayElement<T>(array, lo + i - 1);
			while (i <= n / 2)
			{
				int num = 2 * i;
				if (num < n && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), UnsafeUtility.ReadArrayElement<T>(array, lo + num)) < 0)
				{
					num++;
				}
				if (comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), val) < 0)
				{
					break;
				}
				UnsafeUtility.WriteArrayElement(array, lo + i - 1, UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1));
				i = num;
			}
			UnsafeUtility.WriteArrayElement(array, lo + i - 1, val);
		}

		private unsafe static void Swap<T>(void* array, int lhs, int rhs) where T : unmanaged
		{
			T value = UnsafeUtility.ReadArrayElement<T>(array, lhs);
			UnsafeUtility.WriteArrayElement(array, lhs, UnsafeUtility.ReadArrayElement<T>(array, rhs));
			UnsafeUtility.WriteArrayElement(array, rhs, value);
		}

		private unsafe static void SwapIfGreaterWithItems<T, U>(void* array, int lhs, int rhs, U comp) where T : unmanaged where U : IComparer<T>
		{
			if (lhs != rhs && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lhs), UnsafeUtility.ReadArrayElement<T>(array, rhs)) > 0)
			{
				Swap<T>(array, lhs, rhs);
			}
		}

		private unsafe static void IntroSortStruct<T, U>(void* array, int length, U comp) where T : struct where U : IComparer<T>
		{
			IntroSortStruct<T, U>(array, 0, length - 1, 2 * CollectionHelper.Log2Floor(length), comp);
		}

		private unsafe static void IntroSortStruct<T, U>(void* array, int lo, int hi, int depth, U comp) where T : struct where U : IComparer<T>
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				if (num <= 16)
				{
					switch (num)
					{
					case 1:
						break;
					case 2:
						SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
						break;
					case 3:
						SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi - 1, comp);
						SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
						SwapIfGreaterWithItemsStruct<T, U>(array, hi - 1, hi, comp);
						break;
					default:
						InsertionSortStruct<T, U>(array, lo, hi, comp);
						break;
					}
					break;
				}
				if (depth == 0)
				{
					HeapSortStruct<T, U>(array, lo, hi, comp);
					break;
				}
				depth--;
				int num2 = PartitionStruct<T, U>(array, lo, hi, comp);
				IntroSortStruct<T, U>(array, num2 + 1, hi, depth, comp);
				hi = num2 - 1;
			}
		}

		private unsafe static void InsertionSortStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				T val = UnsafeUtility.ReadArrayElement<T>(array, i + 1);
				while (num >= lo && comp.Compare(val, UnsafeUtility.ReadArrayElement<T>(array, num)) < 0)
				{
					UnsafeUtility.WriteArrayElement(array, num + 1, UnsafeUtility.ReadArrayElement<T>(array, num));
					num--;
				}
				UnsafeUtility.WriteArrayElement(array, num + 1, val);
			}
		}

		private unsafe static int PartitionStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			int num = lo + (hi - lo) / 2;
			SwapIfGreaterWithItemsStruct<T, U>(array, lo, num, comp);
			SwapIfGreaterWithItemsStruct<T, U>(array, lo, hi, comp);
			SwapIfGreaterWithItemsStruct<T, U>(array, num, hi, comp);
			T x = UnsafeUtility.ReadArrayElement<T>(array, num);
			SwapStruct<T>(array, num, hi - 1);
			int num2 = lo;
			int num3 = hi - 1;
			while (num2 < num3)
			{
				while (comp.Compare(x, UnsafeUtility.ReadArrayElement<T>(array, ++num2)) > 0)
				{
				}
				while (comp.Compare(x, UnsafeUtility.ReadArrayElement<T>(array, --num3)) < 0)
				{
				}
				if (num2 >= num3)
				{
					break;
				}
				SwapStruct<T>(array, num2, num3);
			}
			SwapStruct<T>(array, num2, hi - 1);
			return num2;
		}

		private unsafe static void HeapSortStruct<T, U>(void* array, int lo, int hi, U comp) where T : struct where U : IComparer<T>
		{
			int num = hi - lo + 1;
			for (int num2 = num / 2; num2 >= 1; num2--)
			{
				HeapifyStruct<T, U>(array, num2, num, lo, comp);
			}
			for (int num3 = num; num3 > 1; num3--)
			{
				SwapStruct<T>(array, lo, lo + num3 - 1);
				HeapifyStruct<T, U>(array, 1, num3 - 1, lo, comp);
			}
		}

		private unsafe static void HeapifyStruct<T, U>(void* array, int i, int n, int lo, U comp) where T : struct where U : IComparer<T>
		{
			T val = UnsafeUtility.ReadArrayElement<T>(array, lo + i - 1);
			while (i <= n / 2)
			{
				int num = 2 * i;
				if (num < n && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), UnsafeUtility.ReadArrayElement<T>(array, lo + num)) < 0)
				{
					num++;
				}
				if (comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1), val) < 0)
				{
					break;
				}
				UnsafeUtility.WriteArrayElement(array, lo + i - 1, UnsafeUtility.ReadArrayElement<T>(array, lo + num - 1));
				i = num;
			}
			UnsafeUtility.WriteArrayElement(array, lo + i - 1, val);
		}

		private unsafe static void SwapStruct<T>(void* array, int lhs, int rhs) where T : struct
		{
			T value = UnsafeUtility.ReadArrayElement<T>(array, lhs);
			UnsafeUtility.WriteArrayElement(array, lhs, UnsafeUtility.ReadArrayElement<T>(array, rhs));
			UnsafeUtility.WriteArrayElement(array, rhs, value);
		}

		private unsafe static void SwapIfGreaterWithItemsStruct<T, U>(void* array, int lhs, int rhs, U comp) where T : struct where U : IComparer<T>
		{
			if (lhs != rhs && comp.Compare(UnsafeUtility.ReadArrayElement<T>(array, lhs), UnsafeUtility.ReadArrayElement<T>(array, rhs)) > 0)
			{
				SwapStruct<T>(array, lhs, rhs);
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckStrideMatchesSize<T>(int stride) where T : struct
		{
			if (stride != UnsafeUtility.SizeOf<T>())
			{
				throw new InvalidOperationException("Sort requires that stride matches the size of the source type");
			}
		}
	}
}
