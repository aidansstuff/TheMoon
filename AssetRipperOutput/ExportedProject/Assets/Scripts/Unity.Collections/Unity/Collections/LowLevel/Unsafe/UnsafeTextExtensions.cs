namespace Unity.Collections.LowLevel.Unsafe
{
	internal static class UnsafeTextExtensions
	{
		public static ref UnsafeList<byte> AsUnsafeListOfBytes(this ref UnsafeText text)
		{
			return ref UnsafeUtility.As<UntypedUnsafeList, UnsafeList<byte>>(ref text.m_UntypedListData);
		}
	}
}
