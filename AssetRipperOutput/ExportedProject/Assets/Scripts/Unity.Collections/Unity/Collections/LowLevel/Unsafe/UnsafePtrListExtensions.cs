namespace Unity.Collections.LowLevel.Unsafe
{
	internal static class UnsafePtrListExtensions
	{
		public static ref UnsafeList ListData(this ref UnsafePtrList from)
		{
			return ref UnsafeUtility.As<UnsafePtrList, UnsafeList>(ref from);
		}
	}
}
