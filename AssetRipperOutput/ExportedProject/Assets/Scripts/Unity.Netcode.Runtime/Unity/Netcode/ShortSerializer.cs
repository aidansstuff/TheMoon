using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class ShortSerializer : INetworkVariableSerializer<short>
	{
		public void Write(FastBufferWriter writer, ref short value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref short value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<short>.ReadWithAllocator(FastBufferReader reader, out short value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in short value, ref short duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<short>.Duplicate(in short value, ref short duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
