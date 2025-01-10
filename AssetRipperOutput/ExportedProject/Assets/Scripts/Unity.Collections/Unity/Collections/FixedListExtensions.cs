using System;
using System.Collections.Generic;

namespace Unity.Collections
{
	public static class FixedListExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this ref FixedList32Bytes<T> list) where T : unmanaged, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this ref FixedList32Bytes<T> list, U comp) where T : unmanaged, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this ref FixedList64Bytes<T> list) where T : unmanaged, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this ref FixedList64Bytes<T> list, U comp) where T : unmanaged, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this ref FixedList128Bytes<T> list) where T : unmanaged, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this ref FixedList128Bytes<T> list, U comp) where T : unmanaged, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this ref FixedList512Bytes<T> list) where T : unmanaged, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this ref FixedList512Bytes<T> list, U comp) where T : unmanaged, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length, comp);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void Sort<T>(this ref FixedList4096Bytes<T> list) where T : unmanaged, IComparable<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(NativeSortExtension.DefaultComparer<int>)
		})]
		public unsafe static void Sort<T, U>(this ref FixedList4096Bytes<T> list, U comp) where T : unmanaged, IComparable<T> where U : IComparer<T>
		{
			fixed (byte* ptr = &list.buffer.offset0000.byte0000)
			{
				NativeSortExtension.Sort((T*)(ptr + FixedList.PaddingBytes<T>()), list.Length, comp);
			}
		}
	}
}
