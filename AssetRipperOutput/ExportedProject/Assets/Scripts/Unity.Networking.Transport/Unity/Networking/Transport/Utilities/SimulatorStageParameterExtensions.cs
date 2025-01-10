namespace Unity.Networking.Transport.Utilities
{
	public static class SimulatorStageParameterExtensions
	{
		public static ref NetworkSettings WithSimulatorStageParameters(this ref NetworkSettings settings, int maxPacketCount, int maxPacketSize, int packetDelayMs = 0, int packetJitterMs = 0, int packetDropInterval = 0, int packetDropPercentage = 0, int fuzzFactor = 0, int fuzzOffset = 0, uint randomSeed = 0u)
		{
			SimulatorUtility.Parameters parameters = default(SimulatorUtility.Parameters);
			parameters.MaxPacketCount = maxPacketCount;
			parameters.MaxPacketSize = maxPacketSize;
			parameters.PacketDelayMs = packetDelayMs;
			parameters.PacketJitterMs = packetJitterMs;
			parameters.PacketDropInterval = packetDropInterval;
			parameters.PacketDropPercentage = packetDropPercentage;
			parameters.FuzzFactor = fuzzFactor;
			parameters.FuzzOffset = fuzzOffset;
			parameters.RandomSeed = randomSeed;
			SimulatorUtility.Parameters parameter = parameters;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static SimulatorUtility.Parameters GetSimulatorStageParameters(this ref NetworkSettings settings)
		{
			settings.TryGet<SimulatorUtility.Parameters>(out var parameter);
			return parameter;
		}
	}
}
