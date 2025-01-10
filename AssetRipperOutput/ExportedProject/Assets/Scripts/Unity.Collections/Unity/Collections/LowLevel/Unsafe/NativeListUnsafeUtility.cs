using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class NativeListUnsafeUtility
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafePtr<T>(this NativeList<T> list) where T : unmanaged
		{
			return list.m_ListData->Ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetUnsafeReadOnlyPtr<T>(this NativeList<T> list) where T : unmanaged
		{
			return list.m_ListData->Ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void* GetInternalListDataPtrUnchecked<T>(ref NativeList<T> list) where T : unmanaged
		{
			return list.m_ListData;
		}
	}
}
