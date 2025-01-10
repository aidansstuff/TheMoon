namespace Unity.Collections.LowLevel.Unsafe
{
	internal sealed class UnsafeRingQueueDebugView<T> where T : unmanaged
	{
		private UnsafeRingQueue<T> Data;

		public unsafe T[] Items
		{
			get
			{
				T[] array = new T[Data.Length];
				int read = Data.Control.Read;
				int capacity = Data.Control.Capacity;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Data.Ptr[(read + i) % capacity];
				}
				return array;
			}
		}

		public UnsafeRingQueueDebugView(UnsafeRingQueue<T> data)
		{
			Data = data;
		}
	}
}
