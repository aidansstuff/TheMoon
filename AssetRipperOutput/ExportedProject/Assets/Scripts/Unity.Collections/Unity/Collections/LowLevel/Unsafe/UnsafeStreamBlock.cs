namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct UnsafeStreamBlock
	{
		internal unsafe UnsafeStreamBlock* Next;

		internal unsafe fixed byte Data[1];
	}
}
