using System;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct NativeKeyValueArrays<TKey, TValue> : INativeDisposable, IDisposable where TKey : struct where TValue : struct
	{
		public NativeArray<TKey> Keys;

		public NativeArray<TValue> Values;

		public int Length => Keys.Length;

		public NativeKeyValueArrays(int length, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options)
		{
			Keys = CollectionHelper.CreateNativeArray<TKey>(length, allocator, options);
			Values = CollectionHelper.CreateNativeArray<TValue>(length, allocator, options);
		}

		public void Dispose()
		{
			Keys.Dispose();
			Values.Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return Keys.Dispose(Values.Dispose(inputDeps));
		}
	}
}
