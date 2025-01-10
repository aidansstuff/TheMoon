using System;

namespace Steamworks.Data
{
	public struct DurationControl
	{
		internal DurationControl_t _inner;

		public AppId Appid => _inner.Appid;

		public bool Applicable => _inner.Applicable;

		internal TimeSpan PlaytimeInLastFiveHours => TimeSpan.FromSeconds(_inner.CsecsLast5h);

		internal TimeSpan PlaytimeToday => TimeSpan.FromSeconds(_inner.CsecsLast5h);

		internal DurationControlProgress Progress => _inner.Progress;
	}
}
