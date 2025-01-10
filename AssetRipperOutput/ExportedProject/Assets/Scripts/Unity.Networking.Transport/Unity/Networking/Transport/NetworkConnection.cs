using System;

namespace Unity.Networking.Transport
{
	public struct NetworkConnection : IEquatable<NetworkConnection>
	{
		public enum State
		{
			Disconnected = 0,
			Connecting = 1,
			Connected = 2
		}

		internal int m_NetworkId;

		internal int m_NetworkVersion;

		public bool IsCreated => m_NetworkVersion != 0;

		public int InternalId => m_NetworkId;

		public int Disconnect(NetworkDriver driver)
		{
			return driver.Disconnect(this);
		}

		public NetworkEvent.Type PopEvent(NetworkDriver driver, out DataStreamReader stream)
		{
			return driver.PopEventForConnection(this, out stream);
		}

		public NetworkEvent.Type PopEvent(NetworkDriver driver, out DataStreamReader stream, out NetworkPipeline pipeline)
		{
			return driver.PopEventForConnection(this, out stream, out pipeline);
		}

		public int Close(NetworkDriver driver)
		{
			if (m_NetworkId >= 0)
			{
				return driver.Disconnect(this);
			}
			return -1;
		}

		public State GetState(NetworkDriver driver)
		{
			return driver.GetConnectionState(this);
		}

		public static bool operator ==(NetworkConnection lhs, NetworkConnection rhs)
		{
			if (lhs.m_NetworkId == rhs.m_NetworkId)
			{
				return lhs.m_NetworkVersion == rhs.m_NetworkVersion;
			}
			return false;
		}

		public static bool operator !=(NetworkConnection lhs, NetworkConnection rhs)
		{
			if (lhs.m_NetworkId == rhs.m_NetworkId)
			{
				return lhs.m_NetworkVersion != rhs.m_NetworkVersion;
			}
			return true;
		}

		public override bool Equals(object o)
		{
			return this == (NetworkConnection)o;
		}

		public bool Equals(NetworkConnection o)
		{
			return this == o;
		}

		public override int GetHashCode()
		{
			return (m_NetworkId << 8) ^ m_NetworkVersion;
		}
	}
}
