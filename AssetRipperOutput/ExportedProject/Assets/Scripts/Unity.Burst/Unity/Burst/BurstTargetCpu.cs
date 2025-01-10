namespace Unity.Burst
{
	internal enum BurstTargetCpu
	{
		Auto = 0,
		X86_SSE2 = 1,
		X86_SSE4 = 2,
		X64_SSE2 = 3,
		X64_SSE4 = 4,
		AVX = 5,
		AVX2 = 6,
		WASM32 = 7,
		ARMV7A_NEON32 = 8,
		ARMV8A_AARCH64 = 9,
		THUMB2_NEON32 = 10,
		ARMV8A_AARCH64_HALFFP = 11,
		ARMV9A = 12
	}
}
