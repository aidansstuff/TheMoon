using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public struct GlobalDynamicResolutionSettings
	{
		public bool enabled;

		public bool useMipBias;

		public bool enableDLSS;

		public uint DLSSPerfQualitySetting;

		public DynamicResolutionHandler.UpsamplerScheduleType DLSSInjectionPoint;

		public bool DLSSUseOptimalSettings;

		[Range(0f, 1f)]
		public float DLSSSharpness;

		public bool fsrOverrideSharpness;

		[Range(0f, 1f)]
		public float fsrSharpness;

		public float maxPercentage;

		public float minPercentage;

		public DynamicResolutionType dynResType;

		public DynamicResUpscaleFilter upsampleFilter;

		public bool forceResolution;

		public float forcedPercentage;

		public float lowResTransparencyMinimumThreshold;

		public float rayTracingHalfResThreshold;

		public static GlobalDynamicResolutionSettings NewDefault()
		{
			GlobalDynamicResolutionSettings result = default(GlobalDynamicResolutionSettings);
			result.useMipBias = false;
			result.maxPercentage = 100f;
			result.minPercentage = 100f;
			result.dynResType = DynamicResolutionType.Hardware;
			result.upsampleFilter = DynamicResUpscaleFilter.CatmullRom;
			result.forcedPercentage = 100f;
			result.lowResTransparencyMinimumThreshold = 0f;
			result.rayTracingHalfResThreshold = 50f;
			result.enableDLSS = false;
			result.DLSSUseOptimalSettings = true;
			result.DLSSPerfQualitySetting = 0u;
			result.DLSSSharpness = 0.5f;
			result.DLSSInjectionPoint = DynamicResolutionHandler.UpsamplerScheduleType.BeforePost;
			result.fsrOverrideSharpness = false;
			result.fsrSharpness = 0.92f;
			return result;
		}
	}
}
