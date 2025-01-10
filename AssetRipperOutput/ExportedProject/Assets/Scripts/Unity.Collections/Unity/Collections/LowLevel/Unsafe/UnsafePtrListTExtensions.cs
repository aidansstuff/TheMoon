using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal static class UnsafePtrListTExtensions
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public static ref UnsafeList<IntPtr> ListData<T>(this ref UnsafePtrList<T> from) where T : unmanaged
		{
			return ref UnsafeUtility.As<UnsafePtrList<T>, UnsafeList<IntPtr>>(ref from);
		}
	}
}
