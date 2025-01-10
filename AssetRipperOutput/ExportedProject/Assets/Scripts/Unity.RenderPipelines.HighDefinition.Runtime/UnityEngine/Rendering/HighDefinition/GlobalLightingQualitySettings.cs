using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class GlobalLightingQualitySettings
	{
		private static int s_QualitySettingCount = Enum.GetNames(typeof(ScalableSettingLevelParameter.Level)).Length;

		[Range(2f, 32f)]
		public int[] AOStepCount = new int[s_QualitySettingCount];

		public bool[] AOFullRes = new bool[s_QualitySettingCount];

		[Range(16f, 256f)]
		public int[] AOMaximumRadiusPixels = new int[s_QualitySettingCount];

		public bool[] AOBilateralUpsample = new bool[s_QualitySettingCount];

		[Range(1f, 6f)]
		public int[] AODirectionCount = new int[s_QualitySettingCount];

		[Range(4f, 64f)]
		public int[] ContactShadowSampleCount = new int[s_QualitySettingCount];

		[Min(0f)]
		public int[] SSRMaxRaySteps = new int[s_QualitySettingCount];

		[Min(0f)]
		public int[] SSGIRaySteps = new int[s_QualitySettingCount];

		public bool[] SSGIDenoise = new bool[s_QualitySettingCount];

		public bool[] SSGIHalfResDenoise = new bool[s_QualitySettingCount];

		public float[] SSGIDenoiserRadius = new float[s_QualitySettingCount];

		public bool[] SSGISecondDenoise = new bool[s_QualitySettingCount];

		[Min(0.01f)]
		public float[] RTAORayLength = new float[s_QualitySettingCount];

		[Range(1f, 64f)]
		public int[] RTAOSampleCount = new int[s_QualitySettingCount];

		public bool[] RTAODenoise = new bool[s_QualitySettingCount];

		[Range(0.001f, 1f)]
		public float[] RTAODenoiserRadius = new float[s_QualitySettingCount];

		[Min(0.01f)]
		public float[] RTGIRayLength = new float[s_QualitySettingCount];

		public bool[] RTGIFullResolution = new bool[s_QualitySettingCount];

		[Range(0.001f, 10f)]
		public float[] RTGIClampValue = new float[s_QualitySettingCount];

		[Min(0f)]
		public int[] RTGIRaySteps = new int[s_QualitySettingCount];

		public bool[] RTGIDenoise = new bool[s_QualitySettingCount];

		public bool[] RTGIHalfResDenoise = new bool[s_QualitySettingCount];

		[Range(0.001f, 1f)]
		public float[] RTGIDenoiserRadius = new float[s_QualitySettingCount];

		public bool[] RTGISecondDenoise = new bool[s_QualitySettingCount];

		[Range(0f, 1f)]
		public float[] RTRMinSmoothness = new float[s_QualitySettingCount];

		[Range(0f, 1f)]
		public float[] RTRSmoothnessFadeStart = new float[s_QualitySettingCount];

		[Min(0.01f)]
		public float[] RTRRayLength = new float[s_QualitySettingCount];

		[Range(0.001f, 10f)]
		public float[] RTRClampValue = new float[s_QualitySettingCount];

		public bool[] RTRFullResolution = new bool[s_QualitySettingCount];

		[Min(0f)]
		public int[] RTRRayMaxIterations = new int[s_QualitySettingCount];

		public bool[] RTRDenoise = new bool[s_QualitySettingCount];

		[Range(1f, 32f)]
		public int[] RTRDenoiserRadius = new int[s_QualitySettingCount];

		public bool[] RTRSmoothDenoising = new bool[s_QualitySettingCount];

		public FogControl[] Fog_ControlMode = new FogControl[s_QualitySettingCount];

		[Range(0f, 1f)]
		public float[] Fog_Budget = new float[s_QualitySettingCount];

		[Range(0f, 1f)]
		public float[] Fog_DepthRatio = new float[s_QualitySettingCount];

		internal GlobalLightingQualitySettings()
		{
			AOStepCount[0] = 4;
			AOStepCount[1] = 6;
			AOStepCount[2] = 16;
			AOFullRes[0] = false;
			AOFullRes[1] = false;
			AOFullRes[2] = true;
			AOBilateralUpsample[0] = false;
			AOBilateralUpsample[1] = true;
			AOBilateralUpsample[2] = true;
			AODirectionCount[0] = 1;
			AODirectionCount[1] = 2;
			AODirectionCount[2] = 4;
			AOMaximumRadiusPixels[0] = 32;
			AOMaximumRadiusPixels[1] = 40;
			AOMaximumRadiusPixels[2] = 80;
			ContactShadowSampleCount[0] = 6;
			ContactShadowSampleCount[1] = 10;
			ContactShadowSampleCount[2] = 16;
			SSRMaxRaySteps[0] = 16;
			SSRMaxRaySteps[1] = 32;
			SSRMaxRaySteps[2] = 64;
			SSGIRaySteps[0] = 32;
			SSGIRaySteps[1] = 64;
			SSGIRaySteps[2] = 128;
			SSGIDenoise[0] = true;
			SSGIDenoise[1] = true;
			SSGIDenoise[2] = true;
			SSGIHalfResDenoise[0] = true;
			SSGIHalfResDenoise[1] = false;
			SSGIHalfResDenoise[2] = false;
			SSGIDenoiserRadius[0] = 0.75f;
			SSGIDenoiserRadius[1] = 0.5f;
			SSGIDenoiserRadius[2] = 0.5f;
			SSGISecondDenoise[0] = true;
			SSGISecondDenoise[1] = true;
			SSGISecondDenoise[2] = true;
			RTAORayLength[0] = 0.5f;
			RTAORayLength[1] = 3f;
			RTAORayLength[2] = 20f;
			RTAOSampleCount[0] = 1;
			RTAOSampleCount[1] = 2;
			RTAOSampleCount[2] = 8;
			RTAODenoise[0] = true;
			RTAODenoise[1] = true;
			RTAODenoise[2] = true;
			RTAODenoiserRadius[0] = 0.25f;
			RTAODenoiserRadius[1] = 0.5f;
			RTAODenoiserRadius[2] = 0.65f;
			RTGIRayLength[0] = 50f;
			RTGIRayLength[1] = 50f;
			RTGIRayLength[2] = 50f;
			RTGIFullResolution[0] = false;
			RTGIFullResolution[1] = false;
			RTGIFullResolution[2] = true;
			RTGIClampValue[0] = 2f;
			RTGIClampValue[1] = 3f;
			RTGIClampValue[2] = 5f;
			RTGIRaySteps[0] = 32;
			RTGIRaySteps[1] = 48;
			RTGIRaySteps[2] = 64;
			RTGIDenoise[0] = true;
			RTGIDenoise[1] = true;
			RTGIDenoise[2] = true;
			RTGIHalfResDenoise[0] = true;
			RTGIHalfResDenoise[1] = false;
			RTGIHalfResDenoise[2] = false;
			RTGIDenoiserRadius[0] = 1f;
			RTGIDenoiserRadius[1] = 1f;
			RTGIDenoiserRadius[2] = 1f;
			RTGISecondDenoise[0] = true;
			RTGISecondDenoise[1] = true;
			RTGISecondDenoise[2] = true;
			RTRMinSmoothness[0] = 0.6f;
			RTRMinSmoothness[1] = 0.4f;
			RTRMinSmoothness[2] = 0f;
			RTRSmoothnessFadeStart[0] = 0.7f;
			RTRSmoothnessFadeStart[1] = 0.5f;
			RTRSmoothnessFadeStart[2] = 0f;
			RTRRayLength[0] = 50f;
			RTRRayLength[1] = 50f;
			RTRRayLength[2] = 50f;
			RTRClampValue[0] = 0.8f;
			RTRClampValue[1] = 1f;
			RTRClampValue[2] = 1.2f;
			RTRFullResolution[0] = false;
			RTRFullResolution[1] = false;
			RTRFullResolution[2] = true;
			RTRRayMaxIterations[0] = 32;
			RTRRayMaxIterations[1] = 48;
			RTRRayMaxIterations[2] = 64;
			RTRDenoise[0] = true;
			RTRDenoise[1] = true;
			RTRDenoise[2] = true;
			RTRDenoiserRadius[0] = 8;
			RTRDenoiserRadius[1] = 12;
			RTRDenoiserRadius[2] = 16;
			RTRSmoothDenoising[0] = true;
			RTRSmoothDenoising[1] = false;
			RTRSmoothDenoising[2] = false;
			Fog_ControlMode[0] = FogControl.Balance;
			Fog_ControlMode[1] = FogControl.Balance;
			Fog_ControlMode[2] = FogControl.Balance;
			Fog_Budget[0] = 0.166f;
			Fog_Budget[1] = 0.33f;
			Fog_Budget[2] = 0.666f;
			Fog_DepthRatio[0] = 0.666f;
			Fog_DepthRatio[1] = 0.666f;
			Fog_DepthRatio[2] = 0.5f;
		}

		internal static GlobalLightingQualitySettings NewDefault()
		{
			return new GlobalLightingQualitySettings();
		}
	}
}
