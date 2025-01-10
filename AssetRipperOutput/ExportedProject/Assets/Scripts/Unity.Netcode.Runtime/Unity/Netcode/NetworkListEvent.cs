namespace Unity.Netcode
{
	public struct NetworkListEvent<T>
	{
		public enum EventType : byte
		{
			Add = 0,
			Insert = 1,
			Remove = 2,
			RemoveAt = 3,
			Value = 4,
			Clear = 5,
			Full = 6
		}

		public EventType Type;

		public T Value;

		public T PreviousValue;

		public int Index;
	}
}
