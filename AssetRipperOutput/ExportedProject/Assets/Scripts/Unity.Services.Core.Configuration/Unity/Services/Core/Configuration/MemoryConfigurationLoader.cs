using System.Threading.Tasks;

namespace Unity.Services.Core.Configuration
{
	internal class MemoryConfigurationLoader : IConfigurationLoader
	{
		public SerializableProjectConfiguration Config { get; set; }

		Task<SerializableProjectConfiguration> IConfigurationLoader.GetConfigAsync()
		{
			TaskCompletionSource<SerializableProjectConfiguration> taskCompletionSource = new TaskCompletionSource<SerializableProjectConfiguration>();
			taskCompletionSource.SetResult(Config);
			return taskCompletionSource.Task;
		}
	}
}
