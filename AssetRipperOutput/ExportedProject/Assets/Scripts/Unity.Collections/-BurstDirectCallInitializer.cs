using Unity.Collections;
using UnityEngine;

internal static class _0024BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Initialize()
	{
		AllocatorManager.Initialize_0024StackAllocator_Try_000000A2_0024BurstDirectCall();
		AllocatorManager.Initialize_0024SlabAllocator_Try_000000B0_0024BurstDirectCall();
		RewindableAllocator.Try_000008B3_0024BurstDirectCall.Initialize();
		xxHash3.Hash64Long_000008F4_0024BurstDirectCall.Initialize();
		xxHash3.Hash128Long_000008FB_0024BurstDirectCall.Initialize();
	}
}
