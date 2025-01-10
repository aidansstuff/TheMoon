namespace Unity.Networking.Transport.Utilities
{
	public struct SequenceBufferContext
	{
		public int Sequence;

		public int Acked;

		internal ulong AckedMask;

		internal ulong LastAckedMask;

		public uint AckMask;

		public uint LastAckMask;
	}
}
