using Unity.Collections;

namespace Unity.Networking.Transport.Utilities
{
	public static class NativeListExt
	{
		public static void ResizeUninitializedTillPowerOf2<T>(this NativeList<T> list, int sizeToFit) where T : unmanaged
		{
			int length = list.Length;
			if (sizeToFit >= length)
			{
				sizeToFit |= sizeToFit >> 1;
				sizeToFit |= sizeToFit >> 2;
				sizeToFit |= sizeToFit >> 4;
				sizeToFit |= sizeToFit >> 8;
				sizeToFit |= sizeToFit >> 16;
				sizeToFit++;
				list.ResizeUninitialized(sizeToFit);
			}
		}
	}
}
