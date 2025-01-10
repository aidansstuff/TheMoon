namespace Unity.Services.Authentication
{
	internal interface IAuthenticationCache : ICache
	{
		string Profile { get; }

		string CloudProjectId { get; }

		void Migrate(string key);
	}
}
