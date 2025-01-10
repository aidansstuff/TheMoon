using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class UintSerializer : INetworkVariableSerializer<uint>
	{
		public void Write(FastBufferWriter writer, ref uint value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref uint value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<uint>.ReadWithAllocator(FastBufferReader reader, out uint value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in uint value, ref uint duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<uint>.Duplicate(in uint value, ref uint duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
