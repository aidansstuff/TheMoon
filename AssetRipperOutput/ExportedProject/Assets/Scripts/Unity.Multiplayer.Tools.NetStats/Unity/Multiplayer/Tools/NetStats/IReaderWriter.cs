namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IReaderWriter
	{
		bool IsReader { get; }

		bool IsWriter { get; }

		FastBufferReader GetFastBufferReader();

		FastBufferWriter GetFastBufferWriter();

		void SerializeValue(ref string s, bool oneByteChars = false);

		void SerializeValue<T>(ref T[] array) where T : unmanaged;

		void SerializeValue(ref byte value);

		void SerializeValue<T>(ref T value) where T : unmanaged;

		void SerializeNetworkSerializable<T>(ref T value) where T : INetworkSerializable, new();

		bool PreCheck(int amount);

		void SerializeValuePreChecked(ref string s, bool oneByteChars = false);

		void SerializeValuePreChecked<T>(ref T[] array) where T : unmanaged;

		void SerializeValuePreChecked(ref byte value);

		void SerializeValuePreChecked<T>(ref T value) where T : unmanaged;
	}
}
