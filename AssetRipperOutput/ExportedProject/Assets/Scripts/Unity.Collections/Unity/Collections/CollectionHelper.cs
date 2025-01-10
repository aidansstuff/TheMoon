using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class CollectionHelper
	{
		[StructLayout(LayoutKind.Explicit)]
		internal struct LongDoubleUnion
		{
			[FieldOffset(0)]
			internal long longValue;

			[FieldOffset(0)]
			internal double doubleValue;
		}

		public const int CacheLineSize = 64;

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void CheckAllocator(AllocatorManager.AllocatorHandle allocator)
		{
			if (!ShouldDeallocate(allocator))
			{
				throw new ArgumentException($"Allocator {allocator} must not be None or Invalid");
			}
		}

		public static int Log2Floor(int value)
		{
			return 31 - math.lzcnt((uint)value);
		}

		public static int Log2Ceil(int value)
		{
			return 32 - math.lzcnt((uint)(value - 1));
		}

		public static int Align(int size, int alignmentPowerOfTwo)
		{
			if (alignmentPowerOfTwo == 0)
			{
				return size;
			}
			return (size + alignmentPowerOfTwo - 1) & ~(alignmentPowerOfTwo - 1);
		}

		public static ulong Align(ulong size, ulong alignmentPowerOfTwo)
		{
			if (alignmentPowerOfTwo == 0L)
			{
				return size;
			}
			return (size + alignmentPowerOfTwo - 1) & ~(alignmentPowerOfTwo - 1);
		}

		public unsafe static bool IsAligned(void* p, int alignmentPowerOfTwo)
		{
			return ((ulong)p & (ulong)((long)alignmentPowerOfTwo - 1L)) == 0;
		}

		public static bool IsAligned(ulong offset, int alignmentPowerOfTwo)
		{
			return (offset & (ulong)((long)alignmentPowerOfTwo - 1L)) == 0;
		}

		public static bool IsPowerOfTwo(int value)
		{
			return (value & (value - 1)) == 0;
		}

		public unsafe static uint Hash(void* ptr, int bytes)
		{
			ulong num = 5381uL;
			while (bytes > 0)
			{
				ulong num2 = ((byte*)ptr)[--bytes];
				num = (num << 5) + num + num2;
			}
			return (uint)num;
		}

		[NotBurstCompatible]
		internal static void WriteLayout(Type type)
		{
			Console.WriteLine($"   Offset | Bytes  | Name     Layout: {0}", type.Name);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				Console.WriteLine("   {0, 6} | {1, 6} | {2}", Marshal.OffsetOf(type, fieldInfo.Name), Marshal.SizeOf(fieldInfo.FieldType), fieldInfo.Name);
			}
		}

		internal static bool ShouldDeallocate(AllocatorManager.AllocatorHandle allocator)
		{
			return allocator.ToAllocator > Allocator.None;
		}

		[return: AssumeRange(0L, 2147483647L)]
		internal static int AssumePositive(int value)
		{
			return value;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[BurstDiscard]
		[NotBurstCompatible]
		internal static void CheckIsUnmanaged<T>()
		{
			if (!UnsafeUtility.IsValidNativeContainerElementType<T>())
			{
				throw new ArgumentException($"{typeof(T)} used in native collection is not blittable, not primitive, or contains a type tagged as NativeContainer");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void CheckIntPositivePowerOfTwo(int value)
		{
			if (value <= 0 || (value & (value - 1)) != 0)
			{
				throw new ArgumentException($"Alignment requested: {value} is not a non-zero, positive power of two.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void CheckUlongPositivePowerOfTwo(ulong value)
		{
			if (value == 0 || (value & (value - 1)) != 0)
			{
				throw new ArgumentException($"Alignment requested: {value} is not a non-zero, positive power of two.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckIndexInRange(int index, int length)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException($"Index {index} must be positive.");
			}
			if (index >= length)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range in container of '{length}' Length.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckCapacityInRange(int capacity, int length)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException($"Capacity {capacity} must be positive.");
			}
			if (capacity < length)
			{
				throw new ArgumentOutOfRangeException($"Capacity {capacity} is out of range in container of '{length}' Length.");
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		public static NativeArray<T> CreateNativeArray<T, U>(int length, ref U allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct where U : unmanaged, AllocatorManager.IAllocator
		{
			NativeArray<T> array;
			if (!allocator.IsCustomAllocator)
			{
				array = new NativeArray<T>(length, allocator.ToAllocator, options);
			}
			else
			{
				array = default(NativeArray<T>);
				NativeArrayExtensions.Initialize(ref array, length, ref allocator, options);
			}
			return array;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static NativeArray<T> CreateNativeArray<T>(int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct
		{
			NativeArray<T> array;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				array = new NativeArray<T>(length, allocator.ToAllocator, options);
			}
			else
			{
				array = default(NativeArray<T>);
				NativeArrayExtensions.Initialize(ref array, length, allocator, options);
			}
			return array;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static NativeArray<T> CreateNativeArray<T>(NativeArray<T> array, AllocatorManager.AllocatorHandle allocator) where T : struct
		{
			NativeArray<T> array2;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				array2 = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				array2 = default(NativeArray<T>);
				NativeArrayExtensions.Initialize(ref array2, array.Length, allocator);
				array2.CopyFrom(array);
			}
			return array2;
		}

		[NotBurstCompatible]
		public static NativeArray<T> CreateNativeArray<T>(T[] array, AllocatorManager.AllocatorHandle allocator) where T : struct
		{
			NativeArray<T> array2;
			if (!AllocatorManager.IsCustomAllocator(allocator))
			{
				array2 = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				array2 = default(NativeArray<T>);
				NativeArrayExtensions.Initialize(ref array2, array.Length, allocator);
				array2.CopyFrom(array);
			}
			return array2;
		}

		[NotBurstCompatible]
		public static NativeArray<T> CreateNativeArray<T, U>(T[] array, ref U allocator) where T : struct where U : unmanaged, AllocatorManager.IAllocator
		{
			NativeArray<T> array2;
			if (!allocator.IsCustomAllocator)
			{
				array2 = new NativeArray<T>(array, allocator.ToAllocator);
			}
			else
			{
				array2 = default(NativeArray<T>);
				NativeArrayExtensions.Initialize(ref array2, array.Length, ref allocator);
				array2.CopyFrom(array);
			}
			return array2;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		public static NativeParallelHashMap<TKey, TValue> CreateNativeParallelHashMap<TKey, TValue, U>(int length, ref U allocator) where TKey : struct, IEquatable<TKey> where TValue : struct where U : unmanaged, AllocatorManager.IAllocator
		{
			return default(NativeParallelHashMap<TKey, TValue>);
		}

		[Obsolete("CreateNativeMultiHashMap is renamed to CreateNativeParallelHashMap. (UnityUpgradable) -> CreateNativeParallelHashMap<TKey, TValue, U>(*)", true)]
		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		public static NativeHashMap<TKey, TValue> CreateNativeMultiHashMap<TKey, TValue, U>(int length, ref U allocator) where TKey : struct, IEquatable<TKey> where TValue : struct where U : unmanaged, AllocatorManager.IAllocator
		{
			return default(NativeHashMap<TKey, TValue>);
		}
	}
}
