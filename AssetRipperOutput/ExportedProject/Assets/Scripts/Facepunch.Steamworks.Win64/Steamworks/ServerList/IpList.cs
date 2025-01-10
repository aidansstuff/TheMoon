using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steamworks.ServerList
{
	public class IpList : Internet
	{
		public List<string> Ips = new List<string>();

		private bool wantsCancel;

		public IpList(IEnumerable<string> list)
		{
			Ips.AddRange(list);
		}

		public IpList(params string[] list)
		{
			Ips.AddRange(list);
		}

		public override async Task<bool> RunQueryAsync(float timeoutSeconds = 10f)
		{
			int blockSize = 16;
			int pointer = 0;
			string[] ips = Ips.ToArray();
			while (true)
			{
				IEnumerable<string> sublist = ips.Skip(pointer).Take(blockSize);
				if (sublist.Count() == 0)
				{
					break;
				}
				using (Internet list = new Internet())
				{
					list.AddFilter("or", sublist.Count().ToString());
					foreach (string server in sublist)
					{
						list.AddFilter("gameaddr", server);
					}
					await list.RunQueryAsync(timeoutSeconds);
					if (wantsCancel)
					{
						return false;
					}
					Responsive.AddRange(list.Responsive);
					Responsive = Responsive.Distinct().ToList();
					Unresponsive.AddRange(list.Unresponsive);
					Unresponsive = Unresponsive.Distinct().ToList();
				}
				pointer += sublist.Count();
				InvokeChanges();
			}
			return true;
		}

		public override void Cancel()
		{
			wantsCancel = true;
		}
	}
}
