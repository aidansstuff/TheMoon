namespace Unity.Networking.Transport.Utilities
{
	public static class ReliableStageParameterExtensions
	{
		public static ref NetworkSettings WithReliableStageParameters(this ref NetworkSettings settings, int windowSize = 32)
		{
			ReliableUtility.Parameters parameters = default(ReliableUtility.Parameters);
			parameters.WindowSize = windowSize;
			ReliableUtility.Parameters parameter = parameters;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ReliableUtility.Parameters GetReliableStageParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<ReliableUtility.Parameters>(out var parameter))
			{
				parameter.WindowSize = 32;
			}
			return parameter;
		}
	}
}
