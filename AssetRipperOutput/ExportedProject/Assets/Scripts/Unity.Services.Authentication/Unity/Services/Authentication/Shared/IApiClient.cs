using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Authentication.Shared
{
	internal interface IApiClient
	{
		Task<ApiResponse<T>> GetAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> GetAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> PostAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> PostAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> PutAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> PutAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> DeleteAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> DeleteAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> HeadAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> HeadAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> OptionsAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> OptionsAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse<T>> PatchAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<ApiResponse> PatchAsync(string path, ApiRequestOptions options, IApiConfiguration configuration = null, CancellationToken cancellationToken = default(CancellationToken));
	}
}
