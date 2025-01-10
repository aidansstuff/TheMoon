using System;
using Steamworks.Data;

namespace Steamworks.ServerList
{
	public class Favourites : Base
	{
		internal override void LaunchQuery()
		{
			MatchMakingKeyValuePair[] ppchFilters = GetFilters();
			request = Base.Internal.RequestFavoritesServerList(base.AppId.Value, ref ppchFilters, (uint)ppchFilters.Length, IntPtr.Zero);
		}
	}
}
