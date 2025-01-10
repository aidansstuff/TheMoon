using System;

namespace Unity.Networking.Transport.Relay
{
	public static class RelayParameterExtensions
	{
		public static ref NetworkSettings WithRelayParameters(this ref NetworkSettings settings, ref RelayServerData serverData, int relayConnectionTimeMS = 3000)
		{
			RelayNetworkParameter relayNetworkParameter = default(RelayNetworkParameter);
			relayNetworkParameter.ServerData = serverData;
			relayNetworkParameter.RelayConnectionTimeMS = relayConnectionTimeMS;
			RelayNetworkParameter parameter = relayNetworkParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static RelayNetworkParameter GetRelayParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<RelayNetworkParameter>(out var parameter))
			{
				throw new InvalidOperationException("Can't extract Relay parameters: RelayNetworkParameter must be provided to the NetworkSettings");
			}
			return parameter;
		}
	}
}
