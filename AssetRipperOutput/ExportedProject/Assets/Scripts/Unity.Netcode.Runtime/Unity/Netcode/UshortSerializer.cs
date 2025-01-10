using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class UshortSerializer : INetworkVariableSerializer<ushort>
	{
		public void Write(FastBufferWriter writer, ref ushort value)
		{
			BytePacker.WriteValueBitPacked(writer, value);
		}

		public void Read(FastBufferReader reader, ref ushort value)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out value);
		}

		void INetworkVariableSerializer<ushort>.ReadWithAllocator(FastBufferReader reader, out ushort value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in ushort value, ref ushort duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<ushort>.Duplicate(in ushort value, ref ushort duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
