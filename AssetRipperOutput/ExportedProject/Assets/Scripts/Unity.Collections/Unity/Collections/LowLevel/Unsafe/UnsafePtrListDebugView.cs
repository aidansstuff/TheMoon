using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafePtrListDebugView
	{
		private UnsafePtrList Data;

		public unsafe IntPtr[] Items
		{
			get
			{
				IntPtr[] array = new IntPtr[Data.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (IntPtr)Data.Ptr[i];
				}
				return array;
			}
		}

		public UnsafePtrListDebugView(UnsafePtrList data)
		{
			Data = data;
		}
	}
}
