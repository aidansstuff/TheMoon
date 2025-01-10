using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class FixedStringSerializer<T> : INetworkVariableSerializer<T> where T : unmanaged, INativeList<byte>, IUTF8Bytes
	{
		public void Write(FastBufferWriter writer, ref T value)
		{
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForFixedStrings));
		}

		public void Read(FastBufferReader reader, ref T value)
		{
			reader.ReadValueSafeInPlace(ref value);
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
