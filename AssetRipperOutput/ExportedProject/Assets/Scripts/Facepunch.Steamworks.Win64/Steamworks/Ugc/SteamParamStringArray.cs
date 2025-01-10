using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks.Ugc
{
	internal struct SteamParamStringArray : IDisposable
	{
		public SteamParamStringArray_t Value;

		private IntPtr[] NativeStrings;

		private IntPtr NativeArray;

		public static SteamParamStringArray From(string[] array)
		{
			SteamParamStringArray result = default(SteamParamStringArray);
			result.NativeStrings = new IntPtr[array.Length];
			for (int i = 0; i < result.NativeStrings.Length; i++)
			{
				result.NativeStrings[i] = Marshal.StringToHGlobalAnsi(array[i]);
			}
			int cb = Marshal.SizeOf(typeof(IntPtr)) * result.NativeStrings.Length;
			result.NativeArray = Marshal.AllocHGlobal(cb);
			Marshal.Copy(result.NativeStrings, 0, result.NativeArray, result.NativeStrings.Length);
			result.Value = new SteamParamStringArray_t
			{
				Strings = result.NativeArray,
				NumStrings = array.Length
			};
			return result;
		}

		public void Dispose()
		{
			IntPtr[] nativeStrings = NativeStrings;
			foreach (IntPtr hglobal in nativeStrings)
			{
				Marshal.FreeHGlobal(hglobal);
			}
			Marshal.FreeHGlobal(NativeArray);
		}
	}
}
