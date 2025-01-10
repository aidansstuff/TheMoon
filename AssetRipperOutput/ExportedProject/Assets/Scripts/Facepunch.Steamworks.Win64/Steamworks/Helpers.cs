using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal static class Helpers
	{
		public const int MaxStringSize = 32768;

		private static IntPtr[] MemoryPool;

		private static int MemoryPoolIndex;

		private static byte[][] BufferPool;

		private static int BufferPoolIndex;

		public unsafe static IntPtr TakeMemory()
		{
			if (MemoryPool == null)
			{
				MemoryPool = new IntPtr[5];
				for (int i = 0; i < MemoryPool.Length; i++)
				{
					MemoryPool[i] = Marshal.AllocHGlobal(32768);
				}
			}
			MemoryPoolIndex++;
			if (MemoryPoolIndex >= MemoryPool.Length)
			{
				MemoryPoolIndex = 0;
			}
			IntPtr intPtr = MemoryPool[MemoryPoolIndex];
			*(sbyte*)(void*)intPtr = 0;
			return intPtr;
		}

		public static byte[] TakeBuffer(int minSize)
		{
			if (BufferPool == null)
			{
				BufferPool = new byte[8][];
				for (int i = 0; i < BufferPool.Length; i++)
				{
					BufferPool[i] = new byte[131072];
				}
			}
			BufferPoolIndex++;
			if (BufferPoolIndex >= BufferPool.Length)
			{
				BufferPoolIndex = 0;
			}
			if (BufferPool[BufferPoolIndex].Length < minSize)
			{
				BufferPool[BufferPoolIndex] = new byte[minSize + 1024];
			}
			return BufferPool[BufferPoolIndex];
		}

		internal unsafe static string MemoryToString(IntPtr ptr)
		{
			int num = 0;
			for (num = 0; num < 32768 && ((byte*)(void*)ptr)[num] != 0; num++)
			{
			}
			if (num == 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString((byte*)(void*)ptr, num);
		}
	}
}
