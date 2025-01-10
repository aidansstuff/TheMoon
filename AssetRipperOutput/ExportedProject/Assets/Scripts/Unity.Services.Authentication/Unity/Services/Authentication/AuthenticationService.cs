using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	public static class AuthenticationService
	{
		private static IAuthenticationService s_Instance;

		public static IAuthenticationService Instance
		{
			get
			{
				if (s_Instance == null)
				{
					throw new ServicesInitializationException("Singleton is not initialized. Please call UnityServices.InitializeAsync() to initialize.");
				}
				return s_Instance;
			}
			internal set
			{
				s_Instance = value;
			}
		}
	}
}
