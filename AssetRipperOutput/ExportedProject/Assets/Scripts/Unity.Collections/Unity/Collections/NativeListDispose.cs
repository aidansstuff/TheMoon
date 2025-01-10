using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible]
	internal struct NativeListDispose
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe UntypedUnsafeList* m_ListData;

		public unsafe void Dispose()
		{
			UnsafeList<int>* listData = (UnsafeList<int>*)m_ListData;
			UnsafeList<int>.Destroy(listData);
		}
	}
}
