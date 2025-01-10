using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class UlongSerializer : INetworkVariableSerializer<ulong>
	{
		public void Write(FastBufferWriter writer, ref ulong value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref ulong value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<ulong>.ReadWithAllocator(FastBufferReader reader, out ulong value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in ulong value, ref ulong duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<ulong>.Duplicate(in ulong value, ref ulong duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
