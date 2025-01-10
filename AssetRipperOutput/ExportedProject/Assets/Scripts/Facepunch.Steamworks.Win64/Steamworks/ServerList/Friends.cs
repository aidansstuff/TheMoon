using System;
using Steamworks.Data;

namespace Steamworks.ServerList
{
	public class Friends : Base
	{
		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair[] ppchFilters = GetFilters();
			request = Base.Internal.RequestFriendsServerList(base.AppId.Value, ref ppchFilters, (uint)ppchFilters.Length, IntPtr.Zero);
		}
	}
}
