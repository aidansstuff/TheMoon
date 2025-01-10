using System;
using Steamworks.Data;

namespace Steamworks.ServerList
{
	public class Internet : Base
	{
		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair[] ppchFilters = GetFilters();
			request = Base.Internal.RequestInternetServerList(base.AppId.Value, ref ppchFilters, (uint)ppchFilters.Length, IntPtr.Zero);
		}
	}
}
