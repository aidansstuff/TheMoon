using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class UnmanagedTypeSerializer<T> : INetworkVariableSerializer<T> where T : unmanaged
	{
		public void Write(FastBufferWriter writer, ref T value)
		{
			writer.WriteUnmanagedSafe(in value);
		}

		public void Read(FastBufferReader reader, ref T value)
		{
			reader.ReadUnmanagedSafe(out value);
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
