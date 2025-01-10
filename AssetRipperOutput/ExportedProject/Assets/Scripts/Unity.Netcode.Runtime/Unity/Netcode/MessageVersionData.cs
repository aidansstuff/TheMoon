namespace Unity.Netcode
{
	internal struct MessageVersionData
	{
		public uint Hash;

		public int Version;

		public void Serialize(FastBufferWriter writer)
		{
			writer.WriteValueSafe(in Hash, default(FastBufferWriter.ForPrimitives));
			BytePacker.WriteValueBitPacked(writer, Version);
		}

		public void Deserialize(FastBufferReader reader)
		{
			reader.ReadValueSafe(out Hash, default(FastBufferWriter.ForPrimitives));
			ByteUnpacker.ReadValueBitPacked(reader, out Version);
		}
	}
}
