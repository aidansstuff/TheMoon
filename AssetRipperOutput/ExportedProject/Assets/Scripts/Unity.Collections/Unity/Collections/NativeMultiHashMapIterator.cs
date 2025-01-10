using System;

namespace Unity.Collections
{
	[Obsolete("NativeMultiHashMapIterator is renamed to NativeParallelMultiHashMapIterator. (UnityUpgradable) -> NativeParallelMultiHashMapIterator<TKey>", false)]
	public struct NativeMultiHashMapIterator<TKey> where TKey : struct
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
