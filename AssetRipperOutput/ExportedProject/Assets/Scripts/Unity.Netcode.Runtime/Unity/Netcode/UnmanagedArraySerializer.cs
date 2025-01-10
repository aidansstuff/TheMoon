using Unity.Collections;

namespace Unity.Netcode
{
	internal class UnmanagedArraySerializer<T> : INetworkVariableSerializer<NativeArray<T>> where T : unmanaged
	{
		public void Write(FastBufferWriter writer, ref NativeArray<T> value)
		{
			writer.WriteUnmanagedSafe(value);
		}

		public void Read(FastBufferReader reader, ref NativeArray<T> value)
		{
			value.Dispose();
			reader.ReadUnmanagedSafe(out value, Allocator.Persistent);
		}

		void INetworkVariableSerializer<NativeArray<T>>.ReadWithAllocator(FastBufferReader reader, out NativeArray<T> value, Allocator allocator)
		{
			reader.ReadUnmanagedSafe(out value, allocator);
		}

		public void Duplicate(in NativeArray<T> value, ref NativeArray<T> duplicatedValue)
		{
			if (!duplicatedValue.IsCreated || duplicatedValue.Length != value.Length)
			{
				if (duplicatedValue.IsCreated)
				{
					duplicatedValue.Dispose();
				}
				duplicatedValue = new NativeArray<T>(value.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			}
			duplicatedValue.CopyFrom(value);
		}

		void INetworkVariableSerializer<NativeArray<T>>.Duplicate(in NativeArray<T> value, ref NativeArray<T> duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
