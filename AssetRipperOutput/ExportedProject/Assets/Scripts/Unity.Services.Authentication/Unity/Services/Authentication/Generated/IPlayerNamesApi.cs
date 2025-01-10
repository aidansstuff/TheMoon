using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication.Shared;

namespace Unity.Services.Authentication.Generated
{
	internal interface IPlayerNamesApi : IApiAccessor
	{
		Task<ApiResponse<Player>> GetNameAsync(string playerId, bool? autoGenerate = null, bool? showMetadata = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<Player>> UpdateNameAsync(string playerId, UpdateNameRequest updateNameRequest, CancellationToken cancellationToken = default(CancellationToken));
	}
}
