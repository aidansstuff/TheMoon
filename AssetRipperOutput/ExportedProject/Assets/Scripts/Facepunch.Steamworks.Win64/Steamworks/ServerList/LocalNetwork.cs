using System;

namespace Steamworks.ServerList
{
	public class LocalNetwork : Base
	{
		internal override void LaunchQuery()
		{
			request = Base.Internal.RequestLANServerList(base.AppId.Value, IntPtr.Zero);
		}
	}
}
