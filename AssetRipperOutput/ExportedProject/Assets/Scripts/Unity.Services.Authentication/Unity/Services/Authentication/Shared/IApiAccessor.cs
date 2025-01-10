namespace Unity.Services.Authentication.Shared
{
	internal interface IApiAccessor
	{
		IApiConfiguration Configuration { get; }

		string GetBasePath();
	}
}
