using System;

namespace Unity.Netcode
{
	public struct NetworkTime
	{
		private double m_TimeSec;

		private uint m_TickRate;

		private double m_TickInterval;

		private int m_CachedTick;

		private double m_CachedTickOffset;

		public double TickOffset => m_CachedTickOffset;

		public double Time => m_TimeSec;

		public float TimeAsFloat => (float)m_TimeSec;

		public double FixedTime => (double)m_CachedTick * m_TickInterval;

		public float FixedDeltaTime => (float)m_TickInterval;

		public int Tick => m_CachedTick;

		public uint TickRate => m_TickRate;

		public NetworkTime(uint tickRate)
		{
			m_TickRate = tickRate;
			m_TickInterval = 1f / (float)m_TickRate;
			m_CachedTickOffset = 0.0;
			m_CachedTick = 0;
			m_TimeSec = 0.0;
		}

		public NetworkTime(uint tickRate, int tick, double tickOffset = 0.0)
			: this(tickRate)
		{
			m_CachedTickOffset = tickOffset;
			m_CachedTick = tick;
			m_TimeSec = (double)tick * m_TickInterval + tickOffset;
		}

		public NetworkTime(uint tickRate, double timeSec)
			: this(tickRate)
		{
			this += timeSec;
		}

		public NetworkTime ToFixedTime()
		{
			return new NetworkTime(m_TickRate, m_CachedTick);
		}

		public NetworkTime TimeTicksAgo(int ticks)
		{
			return this - new NetworkTime(TickRate, ticks);
		}

		private void UpdateCache()
		{
			double num = m_TimeSec / m_TickInterval;
			m_CachedTick = (int)num;
			if (num - (double)m_CachedTick >= 0.999999999999)
			{
				m_CachedTick++;
			}
			m_CachedTickOffset = (num - Math.Truncate(num)) * m_TickInterval;
			if (m_CachedTick < 0 && m_CachedTickOffset != 0.0)
			{
				m_CachedTick--;
				m_CachedTickOffset = m_TickInterval + m_CachedTickOffset;
			}
		}

		public static NetworkTime operator -(NetworkTime a, NetworkTime b)
		{
			return new NetworkTime(a.TickRate, a.Time - b.Time);
		}

		public static NetworkTime operator +(NetworkTime a, NetworkTime b)
		{
			return new NetworkTime(a.TickRate, a.Time + b.Time);
		}

		public static NetworkTime operator +(NetworkTime a, double b)
		{
			a.m_TimeSec += b;
			a.UpdateCache();
			return a;
		}

		public static NetworkTime operator -(NetworkTime a, double b)
		{
			return a + (0.0 - b);
		}
	}
}
