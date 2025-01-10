namespace Unity.Services.Authentication
{
	internal interface IJwtDecoder
	{
		T Decode<T>(string token) where T : BaseJwt;
	}
}
