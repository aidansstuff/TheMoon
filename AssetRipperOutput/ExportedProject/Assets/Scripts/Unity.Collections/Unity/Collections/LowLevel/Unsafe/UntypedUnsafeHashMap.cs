using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("UntypedUnsafeHashMap is renamed to UntypedUnsafeParallelHashMap. (UnityUpgradable) -> UntypedUnsafeParallelHashMap", false)]
	public struct UntypedUnsafeHashMap
	{
		[NativeDisableUnsafePtrRestriction]
		private unsafe UnsafeParallelHashMapData* m_Buffer;

		private AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
