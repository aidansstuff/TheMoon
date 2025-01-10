namespace Unity.Networking.Transport
{
	public static class BaselibNetworkParameterExtensions
	{
		internal const int k_defaultRxQueueSize = 64;

		internal const int k_defaultTxQueueSize = 64;

		internal const uint k_defaultMaximumPayloadSize = 2000u;

		public static ref NetworkSettings WithBaselibNetworkInterfaceParameters(this ref NetworkSettings settings, int receiveQueueCapacity = 64, int sendQueueCapacity = 64, uint maximumPayloadSize = 2000u)
		{
			BaselibNetworkParameter baselibNetworkParameter = default(BaselibNetworkParameter);
			baselibNetworkParameter.receiveQueueCapacity = receiveQueueCapacity;
			baselibNetworkParameter.sendQueueCapacity = sendQueueCapacity;
			baselibNetworkParameter.maximumPayloadSize = maximumPayloadSize;
			BaselibNetworkParameter parameter = baselibNetworkParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static BaselibNetworkParameter GetBaselibNetworkInterfaceParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<BaselibNetworkParameter>(out var parameter))
			{
				parameter.receiveQueueCapacity = 64;
				parameter.sendQueueCapacity = 64;
				parameter.maximumPayloadSize = 2000u;
			}
			return parameter;
		}
	}
}
