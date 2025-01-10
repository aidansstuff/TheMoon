namespace Unity.Networking.Transport.Utilities
{
	public static class FragmentationStageParameterExtensions
	{
		public static ref NetworkSettings WithFragmentationStageParameters(this ref NetworkSettings settings, int payloadCapacity = 4096)
		{
			FragmentationUtility.Parameters parameters = default(FragmentationUtility.Parameters);
			parameters.PayloadCapacity = payloadCapacity;
			FragmentationUtility.Parameters parameter = parameters;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static FragmentationUtility.Parameters GetFragmentationStageParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<FragmentationUtility.Parameters>(out var parameter))
			{
				parameter.PayloadCapacity = 4096;
			}
			return parameter;
		}
	}
}
