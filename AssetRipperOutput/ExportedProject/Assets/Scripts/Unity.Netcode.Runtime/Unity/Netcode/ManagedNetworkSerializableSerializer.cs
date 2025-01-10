using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class ManagedNetworkSerializableSerializer<T> : INetworkVariableSerializer<T> where T : class, INetworkSerializable, new()
	{
		public void Write(FastBufferWriter writer, ref T value)
		{
			BufferSerializer<BufferSerializerWriter> serializer = new BufferSerializer<BufferSerializerWriter>(new BufferSerializerWriter(writer));
			bool value2 = value == null;
			serializer.SerializeValue(ref value2, default(FastBufferWriter.ForPrimitives));
			if (!value2)
			{
				value.NetworkSerialize(serializer);
			}
		}

		public void Read(FastBufferReader reader, ref T value)
		{
			BufferSerializer<BufferSerializerReader> serializer = new BufferSerializer<BufferSerializerReader>(new BufferSerializerReader(reader));
			bool value2 = false;
			serializer.SerializeValue(ref value2, default(FastBufferWriter.ForPrimitives));
			if (value2)
			{
				value = null;
				return;
			}
			if (value == null)
			{
				value = new T();
			}
			value.NetworkSerialize(serializer);
		}

		void INetworkVariableSerializer<T>.ReadWithAllocator(FastBufferReader reader, out T value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in T value, ref T duplicatedValue)
		{
			using FastBufferWriter writer = new FastBufferWriter(256, Allocator.Temp);
			T value2 = value;
			Write(writer, ref value2);
			using FastBufferReader reader = new FastBufferReader(writer, Allocator.None);
			Read(reader, ref duplicatedValue);
		}

		void INetworkVariableSerializer<T>.Duplicate(in T value, ref T duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
