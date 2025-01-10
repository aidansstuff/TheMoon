using System;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	[Serializable]
	internal struct BytesSentAndReceived : IEquatable<BytesSentAndReceived>
	{
		public long Sent { get; }

		public long Received { get; }

		public NetworkDirection Direction => (NetworkDirection)((int)(((float)Sent > 0f) ? NetworkDirection.Sent : NetworkDirection.None) | (((float)Received > 0f) ? 1 : 0));

		public long Total => Sent + Received;

		public BytesSentAndReceived(long sent = 0L, long received = 0L)
		{
			Sent = sent;
			Received = received;
		}

		public bool Equals(BytesSentAndReceived other)
		{
			if (Sent == other.Sent)
			{
				return Received == other.Received;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is BytesSentAndReceived other)
			{
				return Equals(other);
			}
			return false;
		}

		public static BytesSentAndReceived operator +(BytesSentAndReceived a, BytesSentAndReceived b)
		{
			return new BytesSentAndReceived(a.Sent + b.Sent, a.Received + b.Received);
		}

		public override int GetHashCode()
		{
			return (Sent.GetHashCode() * 397) ^ Received.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}={2} {3}={4}", "BytesSentAndReceived", "Sent", Sent, "Received", Received);
		}
	}
}
