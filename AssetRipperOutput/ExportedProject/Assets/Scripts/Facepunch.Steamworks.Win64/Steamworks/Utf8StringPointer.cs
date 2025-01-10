using System;
using System.Text;

namespace Steamworks
{
	internal struct Utf8StringPointer
	{
		internal IntPtr ptr;

		public unsafe static implicit operator string(Utf8StringPointer p)
		{
			if (p.ptr == IntPtr.Zero)
			{
				return null;
			}
			byte* ptr = (byte*)(void*)p.ptr;
			int i;
			for (i = 0; i < 67108864 && ptr[i] != 0; i++)
			{
			}
			return Encoding.UTF8.GetString(ptr, i);
		}
	}
}
