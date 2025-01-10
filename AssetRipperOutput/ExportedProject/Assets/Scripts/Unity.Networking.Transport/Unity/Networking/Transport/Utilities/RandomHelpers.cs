using System.Diagnostics;
using Unity.Mathematics;

namespace Unity.Networking.Transport.Utilities
{
	public static class RandomHelpers
	{
		public static ushort GetRandomUShort()
		{
			return (ushort)new Random((uint)Stopwatch.GetTimestamp()).NextUInt(1u, 65534u);
		}

		public static ulong GetRandomULong()
		{
			Random random = new Random((uint)Stopwatch.GetTimestamp());
			uint num = random.NextUInt(0u, 4294967294u);
			uint num2 = random.NextUInt(1u, 4294967294u);
			return ((ulong)num << 32) | num2;
		}
	}
}
