using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class GlobalPostProcessingQualitySettings
	{
		private static int s_QualitySettingCount = 3;

		[Range(3f, 8f)]
		public int[] NearBlurSampleCount = new int[s_QualitySettingCount];

		[Range(0f, 8f)]
		public float[] NearBlurMaxRadius = new float[s_QualitySettingCount];

		[Range(3f, 16f)]
		public int[] FarBlurSampleCount = new int[s_QualitySettingCount];

		[Range(0f, 16f)]
		public float[] FarBlurMaxRadius = new float[s_QualitySettingCount];

		public DepthOfFieldResolution[] DoFResolution = new DepthOfFieldResolution[s_QualitySettingCount];

		public bool[] DoFHighQualityFiltering = new bool[s_QualitySettingCount];

		public bool[] DoFPhysicallyBased = new bool[s_QualitySettingCount];

		public bool[] LimitManualRangeNearBlur = new bool[s_QualitySettingCount];

		[Min(2f)]
		public int[] MotionBlurSampleCount = new int[s_QualitySettingCount];

		public BloomResolution[] BloomRes = new BloomResolution[s_QualitySettingCount];

		public bool[] BloomHighQualityFiltering = new bool[s_QualitySettingCount];

		public bool[] BloomHighQualityPrefiltering = new bool[s_QualitySettingCount];

		[Range(3f, 24f)]
		public int[] ChromaticAberrationMaxSamples = new int[s_QualitySettingCount];

		internal GlobalPostProcessingQualitySettings()
		{
			NearBlurSampleCount[0] = 3;
			NearBlurSampleCount[1] = 5;
			NearBlurSampleCount[2] = 8;
			NearBlurMaxRadius[0] = 2f;
			NearBlurMaxRadius[1] = 4f;
			NearBlurMaxRadius[2] = 7f;
			FarBlurSampleCount[0] = 4;
			FarBlurSampleCount[1] = 7;
			FarBlurSampleCount[2] = 14;
			FarBlurMaxRadius[0] = 5f;
			FarBlurMaxRadius[1] = 8f;
			FarBlurMaxRadius[2] = 13f;
			DoFResolution[0] = DepthOfFieldResolution.Quarter;
			DoFResolution[1] = DepthOfFieldResolution.Half;
			DoFResolution[2] = DepthOfFieldResolution.Full;
			DoFHighQualityFiltering[0] = false;
			DoFHighQualityFiltering[1] = true;
			DoFHighQualityFiltering[2] = true;
			LimitManualRangeNearBlur[0] = false;
			LimitManualRangeNearBlur[1] = false;
			LimitManualRangeNearBlur[2] = false;
			MotionBlurSampleCount[0] = 4;
			MotionBlurSampleCount[1] = 8;
			MotionBlurSampleCount[2] = 12;
			BloomRes[0] = BloomResolution.Quarter;
			BloomRes[1] = BloomResolution.Half;
			BloomRes[2] = BloomResolution.Half;
			BloomHighQualityFiltering[0] = false;
			BloomHighQualityFiltering[1] = true;
			BloomHighQualityFiltering[2] = true;
			BloomHighQualityPrefiltering[0] = false;
			BloomHighQualityPrefiltering[1] = false;
			BloomHighQualityPrefiltering[2] = true;
			ChromaticAberrationMaxSamples[0] = 3;
			ChromaticAberrationMaxSamples[1] = 6;
			ChromaticAberrationMaxSamples[2] = 12;
		}

		internal static GlobalPostProcessingQualitySettings NewDefault()
		{
			return new GlobalPostProcessingQualitySettings();
		}
	}
}
