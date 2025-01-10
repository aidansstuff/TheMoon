namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public static class NativeBitArrayUnsafeUtility
	{
		public unsafe static NativeBitArray ConvertExistingDataToNativeBitArray(void* ptr, int sizeInBytes, AllocatorManager.AllocatorHandle allocator)
		{
			NativeBitArray result = default(NativeBitArray);
			result.m_BitArray = new UnsafeBitArray(ptr, sizeInBytes, allocator);
			return result;
		}
	}
}
