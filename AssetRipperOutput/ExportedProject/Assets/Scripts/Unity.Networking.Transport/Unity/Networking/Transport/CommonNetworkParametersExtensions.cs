using System;

namespace Unity.Networking.Transport
{
	public static class CommonNetworkParametersExtensions
	{
		[Obsolete("In Unity Transport 2.0, the data stream size will always be dynamically-sized and this API will be removed.")]
		public static ref NetworkSettings WithDataStreamParameters(this ref NetworkSettings settings, int size = 0)
		{
			NetworkDataStreamParameter networkDataStreamParameter = default(NetworkDataStreamParameter);
			networkDataStreamParameter.size = size;
			NetworkDataStreamParameter parameter = networkDataStreamParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static NetworkDataStreamParameter GetDataStreamParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<NetworkDataStreamParameter>(out var parameter))
			{
				parameter.size = 0;
			}
			return parameter;
		}

		public static ref NetworkSettings WithNetworkConfigParameters(this ref NetworkSettings settings, int connectTimeoutMS = 1000, int maxConnectAttempts = 60, int disconnectTimeoutMS = 30000, int heartbeatTimeoutMS = 500, int maxFrameTimeMS = 0, int fixedFrameTimeMS = 0, int maxMessageSize = 1400)
		{
			NetworkConfigParameter networkConfigParameter = default(NetworkConfigParameter);
			networkConfigParameter.connectTimeoutMS = connectTimeoutMS;
			networkConfigParameter.maxConnectAttempts = maxConnectAttempts;
			networkConfigParameter.disconnectTimeoutMS = disconnectTimeoutMS;
			networkConfigParameter.heartbeatTimeoutMS = heartbeatTimeoutMS;
			networkConfigParameter.maxFrameTimeMS = maxFrameTimeMS;
			networkConfigParameter.fixedFrameTimeMS = fixedFrameTimeMS;
			networkConfigParameter.maxMessageSize = maxMessageSize;
			NetworkConfigParameter parameter = networkConfigParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithNetworkConfigParameters(this ref NetworkSettings settings, int connectTimeoutMS, int maxConnectAttempts, int disconnectTimeoutMS, int heartbeatTimeoutMS, int maxFrameTimeMS, int fixedFrameTimeMS)
		{
			NetworkConfigParameter networkConfigParameter = default(NetworkConfigParameter);
			networkConfigParameter.connectTimeoutMS = connectTimeoutMS;
			networkConfigParameter.maxConnectAttempts = maxConnectAttempts;
			networkConfigParameter.disconnectTimeoutMS = disconnectTimeoutMS;
			networkConfigParameter.heartbeatTimeoutMS = heartbeatTimeoutMS;
			networkConfigParameter.maxFrameTimeMS = maxFrameTimeMS;
			networkConfigParameter.fixedFrameTimeMS = fixedFrameTimeMS;
			networkConfigParameter.maxMessageSize = 1400;
			NetworkConfigParameter parameter = networkConfigParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static NetworkConfigParameter GetNetworkConfigParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<NetworkConfigParameter>(out var parameter))
			{
				parameter.connectTimeoutMS = 1000;
				parameter.maxConnectAttempts = 60;
				parameter.disconnectTimeoutMS = 30000;
				parameter.heartbeatTimeoutMS = 500;
				parameter.maxFrameTimeMS = 0;
				parameter.fixedFrameTimeMS = 0;
				parameter.maxMessageSize = 1400;
			}
			return parameter;
		}
	}
}
