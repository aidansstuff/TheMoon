using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct RingControl
	{
		internal readonly int Capacity;

		internal int Current;

		internal int Write;

		internal int Read;

		internal int Length => Distance(Read, Write);

		internal RingControl(int capacity)
		{
			Capacity = capacity;
			Current = 0;
			Write = 0;
			Read = 0;
		}

		internal void Reset()
		{
			Current = 0;
			Write = 0;
			Read = 0;
		}

		internal int Distance(int from, int to)
		{
			int num = to - from;
			if (num >= 0)
			{
				return num;
			}
			return Capacity - math.abs(num);
		}

		internal int Available()
		{
			return Distance(Read, Current);
		}

		internal int Reserve(int count)
		{
			int num = Distance(Write, Read) - 1;
			int num2 = ((num < 0) ? (Capacity - 1) : num);
			count = ((math.abs(count) - num2 < 0) ? count : num2);
			Write = (Write + count) % Capacity;
			return count;
		}

		internal int Commit(int count)
		{
			int num = Distance(Current, Write);
			count = ((math.abs(count) - num < 0) ? count : num);
			Current = (Current + count) % Capacity;
			return count;
		}

		internal int Consume(int count)
		{
			int num = Distance(Read, Current);
			count = ((math.abs(count) - num < 0) ? count : num);
			Read = (Read + count) % Capacity;
			return count;
		}
	}
}
