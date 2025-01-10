using System;
using Steamworks.Data;

namespace Steamworks.ServerList
{
	public class History : Base
	{
		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair[] ppchFilters = GetFilters();
			request = Base.Internal.RequestHistoryServerList(base.AppId.Value, ref ppchFilters, (uint)ppchFilters.Length, IntPtr.Zero);
		}
	}
}
