using Unity.Collections;

namespace Unity.Netcode
{
	internal interface INetworkVariableSerializer<T>
	{
		void Write(FastBufferWriter writer, ref T value);

		void Read(FastBufferReader reader, ref T value);

		internal void ReadWithAllocator(FastBufferReader reader, out T value, Allocator allocator);

		void Duplicate(in T value, ref T duplicatedValue);
	}
}
