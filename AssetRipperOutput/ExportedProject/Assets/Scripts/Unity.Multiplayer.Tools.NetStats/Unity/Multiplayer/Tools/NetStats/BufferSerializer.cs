namespace Unity.Multiplayer.Tools.NetStats
{
	internal ref struct BufferSerializer<TReaderWriter> where TReaderWriter : IReaderWriter
	{
		private TReaderWriter m_Implementation;

		public bool IsReader => m_Implementation.IsReader;

		public bool IsWriter => m_Implementation.IsWriter;

		internal BufferSerializer(TReaderWriter implementation)
		{
			m_Implementation = implementation;
		}

		public FastBufferReader GetFastBufferReader()
		{
			return m_Implementation.GetFastBufferReader();
		}

		public FastBufferWriter GetFastBufferWriter()
		{
			return m_Implementation.GetFastBufferWriter();
		}

		public void SerializeNetworkSerializable<T>(ref T value) where T : INetworkSerializable, new()
		{
			m_Implementation.SerializeNetworkSerializable(ref value);
		}

		public void SerializeValue(ref string s, bool oneByteChars = false)
		{
			m_Implementation.SerializeValue(ref s, oneByteChars);
		}

		public void SerializeValue<T>(ref T[] array) where T : unmanaged
		{
			m_Implementation.SerializeValue(ref array);
		}

		public void SerializeValue(ref byte value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue<T>(ref T value) where T : unmanaged
		{
			m_Implementation.SerializeValue(ref value);
		}

		public bool PreCheck(int amount)
		{
			return m_Implementation.PreCheck(amount);
		}

		public void SerializeValuePreChecked(ref string s, bool oneByteChars = false)
		{
			m_Implementation.SerializeValuePreChecked(ref s, oneByteChars);
		}

		public void SerializeValuePreChecked<T>(ref T[] array) where T : unmanaged
		{
			m_Implementation.SerializeValuePreChecked(ref array);
		}

		public void SerializeValuePreChecked(ref byte value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked<T>(ref T value) where T : unmanaged
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}
	}
}
