namespace Unity.Services.Qos.Http
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
