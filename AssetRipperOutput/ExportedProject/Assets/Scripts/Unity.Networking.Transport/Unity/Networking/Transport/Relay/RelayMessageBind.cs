namespace Unity.Networking.Transport.Relay
{
	internal static class RelayMessageBind
	{
		private const byte k_ConnectionDataLength = byte.MaxValue;

		private const byte k_HMACLength = 32;

		public const int Length = 295;

		internal unsafe static void Write(DataStreamWriter writer, byte acceptMode, ushort nonce, byte* connectionDataPtr, byte* hmac)
		{
			RelayMessageHeader relayMessageHeader = RelayMessageHeader.Create(RelayMessageType.Bind);
			writer.WriteBytes((byte*)(&relayMessageHeader), 4);
			writer.WriteByte(acceptMode);
			writer.WriteUShort(nonce);
			writer.WriteByte(byte.MaxValue);
			writer.WriteBytes(connectionDataPtr, 255);
			writer.WriteBytes(hmac, 32);
		}
	}
}
