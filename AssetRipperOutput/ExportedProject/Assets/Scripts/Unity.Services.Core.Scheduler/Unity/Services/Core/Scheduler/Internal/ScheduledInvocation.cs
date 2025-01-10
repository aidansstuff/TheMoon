using System;

namespace Unity.Services.Core.Scheduler.Internal
{
	internal class ScheduledInvocation
	{
		public Action Action;

		public DateTime InvocationTime;

		public long ActionId;
	}
}
