using System.Collections.Generic;

namespace Unity.Services.Core.Scheduler.Internal
{
	internal class ScheduledInvocationComparer : IComparer<ScheduledInvocation>
	{
		public int Compare(ScheduledInvocation x, ScheduledInvocation y)
		{
			if (x == y)
			{
				return 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (x == null)
			{
				return -1;
			}
			int num = x.InvocationTime.CompareTo(y.InvocationTime);
			if (num == 0)
			{
				num = x.ActionId.CompareTo(y.ActionId);
			}
			return num;
		}
	}
}
