namespace Unity.Services.Relay.Http
{
	internal abstract class BaseApiClient
	{
		protected readonly IHttpClient HttpClient;

		public BaseApiClient(IHttpClient httpClient)
		{
			HttpClient = httpClient ?? new HttpClient();
		}
	}
}
