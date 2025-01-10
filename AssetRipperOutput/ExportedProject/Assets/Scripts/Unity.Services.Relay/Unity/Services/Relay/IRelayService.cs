using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;

namespace Unity.Services.Relay
{
	public interface IRelayService
	{
		Task<Allocation> CreateAllocationAsync(int maxConnections, string region = null);

		Task<string> GetJoinCodeAsync(Guid allocationId);

		Task<JoinAllocation> JoinAllocationAsync(string joinCode);

		Task<List<Region>> ListRegionsAsync();
	}
}
