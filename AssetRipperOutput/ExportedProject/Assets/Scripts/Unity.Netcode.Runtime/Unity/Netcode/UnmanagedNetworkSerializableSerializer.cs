using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class UnmanagedNetworkSerializableSerializer<T> : INetworkVariableSerializer<T> where T : unmanaged, INetworkSerializable
	{
		public void Write(FastBufferWriter writer, ref T value)
		{
			BufferSerializer<BufferSerializerWriter> serializer = new BufferSerializer<BufferSerializerWriter>(new BufferSerializerWriter(writer));
			value.NetworkSerialize(serializer);
		}

		public void Read(FastBufferReader reader, ref T value)
		{
			BufferSerializer<BufferSerializerReader> serializer = new BufferSerializer<BufferSerializerReader>(new BufferSerializerReader(reader));
			value.NetworkSerialize(serializer);
		}

		void INetworkVariableSerializer<T>.ReadWithAllocator(FastBufferReader reader, out T value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in T value, ref T duplicatedValue)
		{
			duplicatedValue = value;
		}

		void INetworkVariableSerializer<T>.Duplicate(in T value, ref T duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
