using System.Threading.Tasks;
using Unity.Services.Core.Internal.Serialization;

namespace Unity.Services.Core.Configuration
{
	internal class StreamingAssetsConfigurationLoader : IConfigurationLoader
	{
		private readonly IJsonSerializer m_Serializer;

		public StreamingAssetsConfigurationLoader(IJsonSerializer serializer)
		{
			m_Serializer = serializer;
		}

		public async Task<SerializableProjectConfiguration> GetConfigAsync()
		{
			string value = await StreamingAssetsUtils.GetFileTextFromStreamingAssetsAsync("UnityServicesProjectConfiguration.json");
			return m_Serializer.DeserializeObject<SerializableProjectConfiguration>(value);
		}
	}
}
