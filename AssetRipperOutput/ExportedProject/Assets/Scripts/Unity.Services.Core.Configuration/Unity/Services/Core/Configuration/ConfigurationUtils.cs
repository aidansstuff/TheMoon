using Unity.Services.Core.Internal.Serialization;

namespace Unity.Services.Core.Configuration
{
	internal static class ConfigurationUtils
	{
		public const string ConfigFileName = "UnityServicesProjectConfiguration.json";

		public static IConfigurationLoader ConfigurationLoader { get; internal set; } = new StreamingAssetsConfigurationLoader(new NewtonsoftSerializer());

	}
}
