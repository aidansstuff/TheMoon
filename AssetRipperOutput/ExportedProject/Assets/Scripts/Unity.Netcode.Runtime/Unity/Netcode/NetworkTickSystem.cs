using System;

namespace Unity.Netcode
{
	public class NetworkTickSystem
	{
		public const int NoTick = int.MinValue;

		public uint TickRate { get; internal set; }

		public NetworkTime LocalTime { get; internal set; }

		public NetworkTime ServerTime { get; internal set; }

		public event Action Tick;

		public NetworkTickSystem(uint tickRate, double localTimeSec, double serverTimeSec)
		{
			if (tickRate == 0)
			{
				throw new ArgumentException("Tick rate must be a positive value.", "tickRate");
			}
			TickRate = tickRate;
			this.Tick = null;
			LocalTime = new NetworkTime(tickRate, localTimeSec);
			ServerTime = new NetworkTime(tickRate, serverTimeSec);
		}

		public void Reset(double localTimeSec, double serverTimeSec)
		{
			LocalTime = new NetworkTime(TickRate, localTimeSec);
			ServerTime = new NetworkTime(TickRate, serverTimeSec);
		}

		public void UpdateTick(double localTimeSec, double serverTimeSec)
		{
			int tick = LocalTime.Tick;
			LocalTime = new NetworkTime(TickRate, localTimeSec);
			ServerTime = new NetworkTime(TickRate, serverTimeSec);
			NetworkTime localTime = LocalTime;
			NetworkTime serverTime = ServerTime;
			int tick2 = LocalTime.Tick;
			int num = tick2 - ServerTime.Tick;
			for (int i = tick + 1; i <= tick2; i++)
			{
				LocalTime = new NetworkTime(TickRate, i);
				ServerTime = new NetworkTime(TickRate, i - num);
				this.Tick?.Invoke();
			}
			LocalTime = localTime;
			ServerTime = serverTime;
		}
	}
}
