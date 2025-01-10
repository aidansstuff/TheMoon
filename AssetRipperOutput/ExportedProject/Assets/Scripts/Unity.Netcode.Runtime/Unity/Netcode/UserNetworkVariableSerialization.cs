namespace Unity.Netcode
{
	public class UserNetworkVariableSerialization<T>
	{
		public delegate void WriteValueDelegate(FastBufferWriter writer, in T value);

		public delegate void ReadValueDelegate(FastBufferReader reader, out T value);

		public delegate void DuplicateValueDelegate(in T value, ref T duplicatedValue);

		public static WriteValueDelegate WriteValue;

		public static ReadValueDelegate ReadValue;

		public static DuplicateValueDelegate DuplicateValue;
	}
}
