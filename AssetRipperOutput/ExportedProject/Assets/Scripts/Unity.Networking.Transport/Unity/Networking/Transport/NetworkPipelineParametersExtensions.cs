using System;

namespace Unity.Networking.Transport
{
	public static class NetworkPipelineParametersExtensions
	{
		[Obsolete("Will be removed in Unity Transport 2.0.")]
		public static ref NetworkSettings WithPipelineParameters(this ref NetworkSettings settings, int initialCapacity = 0)
		{
			NetworkPipelineParams networkPipelineParams = default(NetworkPipelineParams);
			networkPipelineParams.initialCapacity = initialCapacity;
			NetworkPipelineParams parameter = networkPipelineParams;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static NetworkPipelineParams GetPipelineParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<NetworkPipelineParams>(out var parameter))
			{
				parameter.initialCapacity = 0;
			}
			return parameter;
		}
	}
}
