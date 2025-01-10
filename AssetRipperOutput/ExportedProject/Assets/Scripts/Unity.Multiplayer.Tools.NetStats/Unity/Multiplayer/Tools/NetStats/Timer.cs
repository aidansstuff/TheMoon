using System;
using System.Diagnostics;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	internal class Timer : Metric<TimeSpan>
	{
		public readonly struct TimerScope : IDisposable
		{
			private readonly Action<TimeSpan> m_Callback;

			private readonly Stopwatch m_Stopwatch;

			internal TimerScope(Action<TimeSpan> callback)
			{
				m_Callback = callback;
				m_Stopwatch = new Stopwatch();
				m_Stopwatch.Start();
			}

			public void Dispose()
			{
				m_Callback?.Invoke(m_Stopwatch.Elapsed);
			}
		}

		public override MetricContainerType MetricContainerType => MetricContainerType.Timer;

		public Timer(MetricId metricId, TimeSpan defaultValue = default(TimeSpan))
			: base(metricId, defaultValue)
		{
		}

		public void Set(TimeSpan value)
		{
			base.Value = value;
		}

		public TimerScope Time()
		{
			return new TimerScope(Set);
		}
	}
}
