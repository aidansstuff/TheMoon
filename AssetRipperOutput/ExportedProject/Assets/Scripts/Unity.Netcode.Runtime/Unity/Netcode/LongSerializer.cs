using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class LongSerializer : INetworkVariableSerializer<long>
	{
		public void Write(FastBufferWriter writer, ref long value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref long value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<long>.ReadWithAllocator(FastBufferReader reader, out long value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in long value, ref long duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<long>.Duplicate(in long value, ref long duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
