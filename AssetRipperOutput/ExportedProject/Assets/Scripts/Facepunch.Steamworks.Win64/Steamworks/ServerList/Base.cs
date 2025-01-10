using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks.ServerList
{
	public abstract class Base : IDisposable
	{
		public List<ServerInfo> Responsive = new List<ServerInfo>();

		public List<ServerInfo> Unresponsive = new List<ServerInfo>();

		internal HServerListRequest request;

		internal List<MatchMakingKeyValuePair> filters = new List<MatchMakingKeyValuePair>();

		internal List<int> watchList = new List<int>();

		internal int LastCount = 0;

		internal static ISteamMatchmakingServers Internal => SteamMatchmakingServers.Internal;

		public AppId AppId { get; set; }

		internal int Count => Internal.GetServerCount(request);

		internal bool IsRefreshing => request.Value != IntPtr.Zero && Internal.IsRefreshing(request);

		public event Action OnChanges;

		public event Action<ServerInfo> OnResponsiveServer;

		public Base()
		{
			AppId = SteamClient.AppId;
		}

		public virtual async Task<bool> RunQueryAsync(float timeoutSeconds = 10f)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Reset();
			LaunchQuery();
			HServerListRequest thisRequest = request;
			while (IsRefreshing)
			{
				await Task.Delay(33);
				if (request.Value == IntPtr.Zero || thisRequest.Value != request.Value)
				{
					return false;
				}
				if (!SteamClient.IsValid)
				{
					return false;
				}
				int r = Responsive.Count;
				UpdatePending();
				UpdateResponsive();
				if (r != Responsive.Count)
				{
					InvokeChanges();
				}
				if (stopwatch.Elapsed.TotalSeconds > (double)timeoutSeconds)
				{
					break;
				}
			}
			MovePendingToUnresponsive();
			InvokeChanges();
			return true;
		}

		public virtual void Cancel()
		{
			Internal.CancelQuery(request);
		}

		internal abstract void LaunchQuery();

		internal virtual MatchMakingKeyValuePair[] GetFilters()
		{
			return filters.ToArray();
		}

		public void AddFilter(string key, string value)
		{
			filters.Add(new MatchMakingKeyValuePair
			{
				Key = key,
				Value = value
			});
		}

		private void Reset()
		{
			ReleaseQuery();
			LastCount = 0;
			watchList.Clear();
		}

		private void ReleaseQuery()
		{
			if (request.Value != IntPtr.Zero)
			{
				Cancel();
				Internal.ReleaseRequest(request);
				request = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			ReleaseQuery();
		}

		internal void InvokeChanges()
		{
			this.OnChanges?.Invoke();
		}

		private void UpdatePending()
		{
			int count = Count;
			if (count != LastCount)
			{
				for (int i = LastCount; i < count; i++)
				{
					watchList.Add(i);
				}
				LastCount = count;
			}
		}

		public void UpdateResponsive()
		{
			watchList.RemoveAll(delegate(int x)
			{
				gameserveritem_t serverDetails = Internal.GetServerDetails(request, x);
				if (serverDetails.HadSuccessfulResponse)
				{
					OnServer(ServerInfo.From(serverDetails), serverDetails.HadSuccessfulResponse);
					return true;
				}
				return false;
			});
		}

		private void MovePendingToUnresponsive()
		{
			watchList.RemoveAll(delegate(int x)
			{
				gameserveritem_t serverDetails = Internal.GetServerDetails(request, x);
				OnServer(ServerInfo.From(serverDetails), serverDetails.HadSuccessfulResponse);
				return true;
			});
		}

		private void OnServer(ServerInfo serverInfo, bool responded)
		{
			if (responded)
			{
				Responsive.Add(serverInfo);
				this.OnResponsiveServer?.Invoke(serverInfo);
			}
			else
			{
				Unresponsive.Add(serverInfo);
			}
		}
	}
}
