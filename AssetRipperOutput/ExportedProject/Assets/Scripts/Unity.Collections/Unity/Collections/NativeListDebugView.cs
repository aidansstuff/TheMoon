namespace Unity.Collections
{
	internal sealed class NativeListDebugView<T> where T : unmanaged
	{
		private NativeList<T> m_Array;

		public T[] Items => m_Array.AsArray().ToArray();

		public NativeListDebugView(NativeList<T> array)
		{
			m_Array = array;
		}
	}
}
