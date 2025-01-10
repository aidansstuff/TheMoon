using System;

namespace Unity.Netcode
{
	public class NetworkTimeSystem
	{
		private const float k_DefaultBufferSizeSec = 0.05f;

		private const double k_TimeSyncFrequency = 1.0;

		private const double k_HardResetThresholdSeconds = 0.2;

		private const double k_DefaultAdjustmentRatio = 0.01;

		private double m_TimeSec;

		private double m_CurrentLocalTimeOffset;

		private double m_DesiredLocalTimeOffset;

		private double m_CurrentServerTimeOffset;

		private double m_DesiredServerTimeOffset;

		private NetworkConnectionManager m_ConnectionManager;

		private NetworkTransport m_NetworkTransport;

		private NetworkTickSystem m_NetworkTickSystem;

		private NetworkManager m_NetworkManager;

		private int m_TimeSyncFrequencyTicks;

		public double LocalBufferSec { get; set; }

		public double ServerBufferSec { get; set; }

		public double HardResetThresholdSec { get; set; }

		public double AdjustmentRatio { get; set; }

		public double LocalTime => m_TimeSec + m_CurrentLocalTimeOffset;

		public double ServerTime => m_TimeSec + m_CurrentServerTimeOffset;

		internal double LastSyncedServerTimeSec { get; private set; }

		internal double LastSyncedRttSec { get; private set; }

		public NetworkTimeSystem(double localBufferSec, double serverBufferSec = 0.05000000074505806, double hardResetThresholdSec = 0.2, double adjustmentRatio = 0.01)
		{
			LocalBufferSec = localBufferSec;
			ServerBufferSec = serverBufferSec;
			HardResetThresholdSec = hardResetThresholdSec;
			AdjustmentRatio = adjustmentRatio;
		}

		internal NetworkTickSystem Initialize(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
			m_ConnectionManager = networkManager.ConnectionManager;
			m_NetworkTransport = networkManager.NetworkConfig.NetworkTransport;
			m_TimeSyncFrequencyTicks = (int)(1.0 * (double)networkManager.NetworkConfig.TickRate);
			m_NetworkTickSystem = new NetworkTickSystem(networkManager.NetworkConfig.TickRate, 0.0, 0.0);
			if (m_ConnectionManager.LocalClient.IsServer)
			{
				m_NetworkTickSystem.Tick += OnTickSyncTime;
			}
			return m_NetworkTickSystem;
		}

		internal void UpdateTime()
		{
			if (m_ConnectionManager.LocalClient.IsServer || m_ConnectionManager.LocalClient.IsConnected)
			{
				if (Advance(m_NetworkManager.RealTimeProvider.UnscaledDeltaTime))
				{
					m_NetworkTickSystem.Reset(LocalTime, ServerTime);
				}
				m_NetworkTickSystem.UpdateTick(LocalTime, ServerTime);
				if (!m_ConnectionManager.LocalClient.IsServer)
				{
					Sync(LastSyncedServerTimeSec + (double)m_NetworkManager.RealTimeProvider.UnscaledDeltaTime, (double)m_NetworkTransport.GetCurrentRtt(0uL) / 1000.0);
				}
			}
		}

		private void OnTickSyncTime()
		{
			if (m_ConnectionManager.LocalClient.IsServer && m_NetworkTickSystem.ServerTime.Tick % m_TimeSyncFrequencyTicks == 0)
			{
				TimeSyncMessage timeSyncMessage = default(TimeSyncMessage);
				timeSyncMessage.Tick = m_NetworkTickSystem.ServerTime.Tick;
				TimeSyncMessage message = timeSyncMessage;
				m_ConnectionManager.SendMessage(ref message, NetworkDelivery.Unreliable, in m_ConnectionManager.ConnectedClientIds);
			}
		}

		internal void Shutdown()
		{
			if (m_ConnectionManager.LocalClient.IsServer)
			{
				m_NetworkTickSystem.Tick -= OnTickSyncTime;
			}
		}

		public static NetworkTimeSystem ServerTimeSystem()
		{
			return new NetworkTimeSystem(0.0, 0.0, double.MaxValue);
		}

		public bool Advance(double deltaTimeSec)
		{
			m_TimeSec += deltaTimeSec;
			if (Math.Abs(m_DesiredLocalTimeOffset - m_CurrentLocalTimeOffset) > HardResetThresholdSec || Math.Abs(m_DesiredServerTimeOffset - m_CurrentServerTimeOffset) > HardResetThresholdSec)
			{
				m_TimeSec += m_DesiredServerTimeOffset;
				m_DesiredLocalTimeOffset -= m_DesiredServerTimeOffset;
				m_CurrentLocalTimeOffset = m_DesiredLocalTimeOffset;
				m_DesiredServerTimeOffset = 0.0;
				m_CurrentServerTimeOffset = 0.0;
				return true;
			}
			m_CurrentLocalTimeOffset += deltaTimeSec * ((m_DesiredLocalTimeOffset > m_CurrentLocalTimeOffset) ? AdjustmentRatio : (0.0 - AdjustmentRatio));
			m_CurrentServerTimeOffset += deltaTimeSec * ((m_DesiredServerTimeOffset > m_CurrentServerTimeOffset) ? AdjustmentRatio : (0.0 - AdjustmentRatio));
			return false;
		}

		public void Reset(double serverTimeSec, double rttSec)
		{
			Sync(serverTimeSec, rttSec);
			Advance(0.0);
		}

		public void Sync(double serverTimeSec, double rttSec)
		{
			LastSyncedRttSec = rttSec;
			LastSyncedServerTimeSec = serverTimeSec;
			double num = serverTimeSec - m_TimeSec;
			m_DesiredServerTimeOffset = num - ServerBufferSec;
			m_DesiredLocalTimeOffset = num + rttSec + LocalBufferSec;
		}
	}
}
