using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	internal class Utf8StringToNative : ICustomMarshaler
	{
		public unsafe IntPtr MarshalManagedToNative(object managedObj)
		{
			if (managedObj == null)
			{
				return IntPtr.Zero;
			}
			if (managedObj is string text)
			{
				fixed (char* chars = text)
				{
					int byteCount = Encoding.UTF8.GetByteCount(text);
					IntPtr intPtr = Marshal.AllocHGlobal(byteCount + 1);
					int bytes = Encoding.UTF8.GetBytes(chars, text.Length, (byte*)(void*)intPtr, byteCount + 1);
					((byte*)(void*)intPtr)[bytes] = 0;
					return intPtr;
				}
			}
			return IntPtr.Zero;
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			throw new NotImplementedException();
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			Marshal.FreeHGlobal(pNativeData);
		}

		public void CleanUpManagedData(object managedObj)
		{
			throw new NotImplementedException();
		}

		public int GetNativeDataSize()
		{
			return -1;
		}

		[Preserve]
		public static ICustomMarshaler GetInstance(string cookie)
		{
			return new Utf8StringToNative();
		}
	}
}
