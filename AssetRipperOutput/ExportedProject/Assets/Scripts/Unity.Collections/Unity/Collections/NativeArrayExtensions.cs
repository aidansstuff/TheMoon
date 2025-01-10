using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	public static class NativeArrayExtensions
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NativeArrayStaticId<T> where T : struct
		{
			internal static readonly SharedStatic<int> s_staticSafetyId = SharedStatic<int>.GetOrCreate<NativeArray<T>>();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
		{
			return IndexOf<T, U>(array.GetUnsafeReadOnlyPtr(), array.Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this NativeArray<T> array, U value) where T : struct, IEquatable<U>
		{
			return IndexOf<T, U>(array.GetUnsafeReadOnlyPtr(), array.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, IEquatable<U>
		{
			return IndexOf<T, U>(array.m_Buffer, array.m_Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this NativeArray<T>.ReadOnly array, U value) where T : struct, IEquatable<U>
		{
			return IndexOf<T, U>(array.m_Buffer, array.m_Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<T, U>(this NativeList<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return IndexOf<T, U>(list.GetUnsafeReadOnlyPtr(), list.Length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(this NativeList<T> list, U value) where T : unmanaged, IEquatable<U>
		{
			return IndexOf<T, U>(list.GetUnsafeReadOnlyPtr(), list.Length, value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static bool Contains<T, U>(void* ptr, int length, U value) where T : struct, IEquatable<U>
		{
			return IndexOf<T, U>(ptr, length, value) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static int IndexOf<T, U>(void* ptr, int length, U value) where T : struct, IEquatable<U>
		{
			for (int i = 0; i != length; i++)
			{
				if (UnsafeUtility.ReadArrayElement<T>(ptr, i).Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		public unsafe static NativeArray<U> Reinterpret<T, U>(this NativeArray<T> array) where T : struct where U : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num / num2;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<U>(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array), (int)num3, Allocator.None);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<T>(this NativeArray<T> array, NativeArray<T> other) where T : struct, IEquatable<T>
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

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static bool ArraysEqual<T>(this NativeList<T> array, NativeArray<T> other) where T : unmanaged, IEquatable<T>
		{
			return array.AsArray().ArraysEqual(other);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckReinterpretSize<T, U>(ref NativeArray<T> array) where T : struct where U : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = UnsafeUtility.SizeOf<U>();
			long num3 = (long)array.Length * (long)num;
			if (num3 / num2 * num2 != num3)
			{
				throw new InvalidOperationException($"Types {typeof(T)} (array length {array.Length}) and {typeof(U)} cannot be aliased due to size constraints. The size of the types and lengths involved must line up.");
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal unsafe static void Initialize<T>(this ref NativeArray<T> array, int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where T : struct
		{
			AllocatorManager.AllocatorHandle t = allocator;
			array.m_Buffer = AllocatorManager.AllocateStruct(ref t, default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = Allocator.None;
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, array.m_Length * UnsafeUtility.SizeOf<T>());
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(AllocatorManager.AllocatorHandle)
		})]
		internal unsafe static void Initialize<T, U>(this ref NativeArray<T> array, int length, ref U allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) where T : struct where U : unmanaged, AllocatorManager.IAllocator
		{
			array.m_Buffer = AllocatorManager.AllocateStruct(ref allocator, default(T), length);
			array.m_Length = length;
			array.m_AllocatorLabel = Allocator.None;
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(array.m_Buffer, array.m_Length * UnsafeUtility.SizeOf<T>());
			}
		}
	}
}
