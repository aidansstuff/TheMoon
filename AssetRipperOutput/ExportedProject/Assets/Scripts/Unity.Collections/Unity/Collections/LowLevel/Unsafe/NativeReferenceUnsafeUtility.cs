using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class NativeReferenceUnsafeUtility
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtr<T>(this NativeReference<T> reference) where T : unmanaged
		{
			return reference.m_Data;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafeReadOnlyPtr<T>(this NativeReference<T> reference) where T : unmanaged
		{
			return reference.m_Data;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtrWithoutChecks<T>(this NativeReference<T> reference) where T : unmanaged
		{
			return reference.m_Data;
		}
	}
}
