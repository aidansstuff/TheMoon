using Newtonsoft.Json;

namespace Unity.Services.Authentication
{
	internal static class SerializerSettings
	{
		private static JsonSerializerSettings s_Instance;

		internal static JsonSerializerSettings DefaultSerializerSettings
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new JsonSerializerSettings();
				}
				return s_Instance;
			}
		}
	}
}
