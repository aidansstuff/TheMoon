using System;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeParallelMultiHashMapIterator<TKey> where TKey : struct
	{
		internal TKey key;

		internal int NextEntryIndex;

		internal int EntryIndex;

		public int GetEntryIndex()
		{
			return EntryIndex;
		}
	}
}
