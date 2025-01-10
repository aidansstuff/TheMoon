using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct ConnectionInfo
	{
		public ulong Id { get; }

		public ConnectionInfo(ulong id)
		{
			Id = id;
		}

		public static bool operator ==(ConnectionInfo a, ConnectionInfo b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ConnectionInfo a, ConnectionInfo b)
		{
			return !(a == b);
		}

		public bool Equals(ConnectionInfo other)
		{
			return Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is ConnectionInfo other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
