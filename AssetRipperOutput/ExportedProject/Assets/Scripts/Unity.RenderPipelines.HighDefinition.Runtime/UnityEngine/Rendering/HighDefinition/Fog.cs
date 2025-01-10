using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Fog", new Type[] { typeof(HDRenderPipeline) })]
	public class Fog : VolumeComponentWithQuality
	{
		[Tooltip("Enables the fog.")]
		public BoolParameter enabled = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		public FogColorParameter colorMode = new FogColorParameter(FogColorMode.SkyColor);

		[Tooltip("Specifies the constant color of the fog.")]
		public ColorParameter color = new ColorParameter(Color.grey, hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Specifies the tint of the fog.")]
		public ColorParameter tint = new ColorParameter(Color.white, hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Sets the maximum fog distance HDRP uses when it shades the skybox or the Far Clipping Plane of the Camera.")]
		public MinFloatParameter maxFogDistance = new MinFloatParameter(5000f, 0f);

		[AdditionalProperty]
		[Tooltip("Controls the maximum mip map HDRP uses for mip fog (0 is the lowest mip and 1 is the highest mip).")]
		public ClampedFloatParameter mipFogMaxMip = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		[Tooltip("Sets the distance at which HDRP uses the minimum mip image of the blurred sky texture as the fog color.")]
		public MinFloatParameter mipFogNear = new MinFloatParameter(0f, 0f);

		[AdditionalProperty]
		[Tooltip("Sets the distance at which HDRP uses the maximum mip image of the blurred sky texture as the fog color.")]
		public MinFloatParameter mipFogFar = new MinFloatParameter(1000f, 0f);

		public FloatParameter baseHeight = new FloatParameter(0f);

		public FloatParameter maximumHeight = new FloatParameter(50f);

		[DisplayInfo(name = "Fog Attenuation Distance")]
		public MinFloatParameter meanFreePath = new MinFloatParameter(400f, 1f);

		[DisplayInfo(name = "Volumetric Fog")]
		public BoolParameter enableVolumetricFog = new BoolParameter(value: false);

		public ColorParameter albedo = new ColorParameter(Color.white);

		[DisplayInfo(name = "GI Dimmer")]
		public ClampedFloatParameter globalLightProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		public MinFloatParameter depthExtent = new MinFloatParameter(64f, 0.1f);

		[Tooltip("Specifies the denoising technique to use for the volumetric effect.")]
		public FogDenoisingModeParameter denoisingMode = new FogDenoisingModeParameter(FogDenoisingMode.Gaussian);

		[AdditionalProperty]
		public ClampedFloatParameter anisotropy = new ClampedFloatParameter(0f, -1f, 1f);

		[AdditionalProperty]
		[Tooltip("Controls the distribution of slices along the Camera's focal axis. 0 is exponential distribution and 1 is linear distribution.")]
		public ClampedFloatParameter sliceDistributionUniformity = new ClampedFloatParameter(0.75f, 0f, 1f);

		internal const float minFogScreenResolutionPercentage = 6.25f;

		internal const float optimalFogScreenResolutionPercentage = 12.5f;

		internal const float maxFogScreenResolutionPercentage = 50f;

		internal const int maxFogSliceCount = 512;

		[AdditionalProperty]
		[SerializeField]
		[FormerlySerializedAs("fogControlMode")]
		[Tooltip("Specifies which method to use to control the performance and quality of the volumetric fog.")]
		private FogControlParameter m_FogControlMode = new FogControlParameter(FogControl.Balance);

		[AdditionalProperty]
		[Tooltip("Controls the resolution of the volumetric buffer (3D texture) along the x-axis and y-axis relative to the resolution of the screen.")]
		public ClampedFloatParameter screenResolutionPercentage = new ClampedFloatParameter(12.5f, 6.25f, 50f);

		[AdditionalProperty]
		[Tooltip("Controls the number of slices to use the volumetric buffer (3D texture) along the camera's focal axis.")]
		public ClampedIntParameter volumeSliceCount = new ClampedIntParameter(64, 1, 512);

		[AdditionalProperty]
		[SerializeField]
		[FormerlySerializedAs("volumetricFogBudget")]
		[Tooltip("Controls the performance to quality ratio of the volumetric fog. A value of 0 being the least resource-intensive and a value of 1 being the highest quality.")]
		private ClampedFloatParameter m_VolumetricFogBudget = new ClampedFloatParameter(0.25f, 0f, 1f);

		[AdditionalProperty]
		[SerializeField]
		[FormerlySerializedAs("resolutionDepthRatio")]
		[Tooltip("Controls how Unity shares resources between Screen (x-axis and y-axis) and Depth (z-axis) resolutions.")]
		public ClampedFloatParameter m_ResolutionDepthRatio = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		[Tooltip("When enabled, HDRP only includes directional Lights when it evaluates volumetric fog.")]
		public BoolParameter directionalLightsOnly = new BoolParameter(value: false);

		public FogControl fogControlMode
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_FogControlMode.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().Fog_ControlMode[quality.value];
			}
			set
			{
				m_FogControlMode.value = value;
			}
		}

		public float volumetricFogBudget
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_VolumetricFogBudget.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().Fog_Budget[quality.value];
			}
			set
			{
				m_VolumetricFogBudget.value = value;
			}
		}

		public float resolutionDepthRatio
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_ResolutionDepthRatio.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().Fog_DepthRatio[quality.value];
			}
			set
			{
				m_ResolutionDepthRatio.value = value;
			}
		}

		internal static bool IsFogEnabled(HDCamera hdCamera)
		{
			if (hdCamera.frameSettings.IsEnabled(FrameSettingsField.AtmosphericScattering))
			{
				return hdCamera.volumeStack.GetComponent<Fog>().enabled.value;
			}
			return false;
		}

		internal static bool IsVolumetricFogEnabled(HDCamera hdCamera)
		{
			Fog component = hdCamera.volumeStack.GetComponent<Fog>();
			bool value = component.enableVolumetricFog.value;
			bool flag = hdCamera.frameSettings.IsEnabled(FrameSettingsField.Volumetrics);
			bool flag2 = CoreUtils.IsSceneViewFogEnabled(hdCamera.camera);
			bool value2 = component.enabled.value;
			return value && flag && flag2 && value2;
		}

		internal static bool IsPBRFogEnabled(HDCamera hdCamera)
		{
			hdCamera.volumeStack.GetComponent<VisualEnvironment>();
			return false;
		}

		private static float ScaleHeightFromLayerDepth(float d)
		{
			return d * 0.144765f;
		}

		private static void UpdateShaderVariablesGlobalCBNeutralParameters(ref ShaderVariablesGlobal cb)
		{
			cb._FogEnabled = 0;
			cb._EnableVolumetricFog = 0;
			cb._HeightFogBaseScattering = Vector3.zero;
			cb._HeightFogBaseExtinction = 0f;
			cb._HeightFogExponents = Vector2.one;
			cb._HeightFogBaseHeight = 0f;
			cb._GlobalFogAnisotropy = 0f;
		}

		internal static void UpdateShaderVariablesGlobalCB(ref ShaderVariablesGlobal cb, HDCamera hdCamera)
		{
			Fog component = hdCamera.volumeStack.GetComponent<Fog>();
			if (!hdCamera.frameSettings.IsEnabled(FrameSettingsField.AtmosphericScattering) || !component.enabled.value)
			{
				UpdateShaderVariablesGlobalCBNeutralParameters(ref cb);
			}
			else
			{
				component.UpdateShaderVariablesGlobalCBFogParameters(ref cb, hdCamera);
			}
		}

		private void UpdateShaderVariablesGlobalCBFogParameters(ref ShaderVariablesGlobal cb, HDCamera hdCamera)
		{
			bool flag = enableVolumetricFog.value && hdCamera.frameSettings.IsEnabled(FrameSettingsField.Volumetrics);
			cb._FogEnabled = 1;
			cb._PBRFogEnabled = (IsPBRFogEnabled(hdCamera) ? 1 : 0);
			cb._EnableVolumetricFog = (flag ? 1 : 0);
			cb._MaxFogDistance = maxFogDistance.value;
			Color color = ((colorMode.value == FogColorMode.ConstantColor) ? this.color.value : tint.value);
			cb._FogColorMode = (float)colorMode.value;
			cb._FogColor = new Color(color.r, color.g, color.b, 0f);
			cb._MipFogParameters = new Vector4(mipFogNear.value, mipFogFar.value, mipFogMaxMip.value, 0f);
			LocalVolumetricFogEngineData localVolumetricFogEngineData = new LocalVolumetricFogArtistParameters(albedo.value, meanFreePath.value, anisotropy.value).ConvertToEngineData();
			cb._HeightFogBaseScattering = (flag ? ((Vector4)localVolumetricFogEngineData.scattering) : (Vector4.one * localVolumetricFogEngineData.extinction));
			cb._HeightFogBaseExtinction = localVolumetricFogEngineData.extinction;
			float num = baseHeight.value;
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				num -= hdCamera.camera.transform.position.y;
			}
			float num2 = ScaleHeightFromLayerDepth(Mathf.Max(0.01f, maximumHeight.value - baseHeight.value));
			cb._HeightFogExponents = new Vector2(1f / num2, num2);
			cb._HeightFogBaseHeight = num;
			cb._GlobalFogAnisotropy = anisotropy.value;
			cb._VolumetricFilteringEnabled = (((denoisingMode.value & FogDenoisingMode.Gaussian) != 0) ? 1 : 0);
			cb._FogDirectionalOnly = (directionalLightsOnly.value ? 1 : 0);
		}
	}
}
