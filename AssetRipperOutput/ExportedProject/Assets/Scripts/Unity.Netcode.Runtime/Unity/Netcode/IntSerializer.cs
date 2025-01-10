using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class IntSerializer : INetworkVariableSerializer<int>
	{
		public void Write(FastBufferWriter writer, ref int value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref int value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<int>.ReadWithAllocator(FastBufferReader reader, out int value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in int value, ref int duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<int>.Duplicate(in int value, ref int duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
