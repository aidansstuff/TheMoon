using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Lighting/Screen Space Reflection", new Type[] { typeof(HDRenderPipeline) })]
	public class ScreenSpaceReflection : VolumeComponentWithQuality
	{
		[Tooltip("Enable Screen Space Reflections.")]
		public BoolParameter enabled = new BoolParameter(value: true, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Enable Transparent Screen Space Reflections.")]
		public BoolParameter enabledTransparent = new BoolParameter(value: true, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Controls the casting technique used to evaluate the effect.")]
		public RayCastingModeParameter tracing = new RayCastingModeParameter(RayCastingMode.RayMarching);

		[SerializeField]
		[FormerlySerializedAs("minSmoothness")]
		private ClampedFloatParameter m_MinSmoothness = new ClampedFloatParameter(0.9f, 0f, 1f);

		[SerializeField]
		[FormerlySerializedAs("smoothnessFadeStart")]
		private ClampedFloatParameter m_SmoothnessFadeStart = new ClampedFloatParameter(0.9f, 0f, 1f);

		public BoolParameter reflectSky = new BoolParameter(value: true);

		public SSRAlgoParameter usedAlgorithm = new SSRAlgoParameter(ScreenSpaceReflectionAlgorithm.Approximation);

		public ClampedFloatParameter depthBufferThickness = new ClampedFloatParameter(0.01f, 0f, 1f);

		public ClampedFloatParameter screenFadeDistance = new ClampedFloatParameter(0.1f, 0f, 1f);

		public ClampedFloatParameter accumulationFactor = new ClampedFloatParameter(0.75f, 0f, 1f);

		[AdditionalProperty]
		public ClampedFloatParameter biasFactor = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		public FloatParameter speedRejectionParam = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		public ClampedFloatParameter speedRejectionScalerFactor = new ClampedFloatParameter(0.2f, 0.001f, 1f);

		[AdditionalProperty]
		public BoolParameter speedSmoothReject = new BoolParameter(value: false);

		[AdditionalProperty]
		public BoolParameter speedSurfaceOnly = new BoolParameter(value: true);

		[AdditionalProperty]
		public BoolParameter speedTargetOnly = new BoolParameter(value: true);

		public BoolParameter enableWorldSpeedRejection = new BoolParameter(value: false);

		[SerializeField]
		[FormerlySerializedAs("rayMaxIterations")]
		private MinIntParameter m_RayMaxIterations = new MinIntParameter(64, 0);

		[FormerlySerializedAs("fallbackHierachy")]
		[AdditionalProperty]
		public RayTracingFallbackHierachyParameter rayMiss = new RayTracingFallbackHierachyParameter(RayTracingFallbackHierachy.ReflectionProbesAndSky);

		[AdditionalProperty]
		public RayTracingFallbackHierachyParameter lastBounceFallbackHierarchy = new RayTracingFallbackHierachyParameter(RayTracingFallbackHierachy.ReflectionProbesAndSky);

		[Tooltip("Controls the dimmer applied to the ambient and legacy light probes.")]
		[AdditionalProperty]
		public ClampedFloatParameter ambientProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		public LayerMaskParameter layerMask = new LayerMaskParameter(-1);

		public ClampedIntParameter textureLodBias = new ClampedIntParameter(1, 0, 7);

		[SerializeField]
		[FormerlySerializedAs("rayLength")]
		private MinFloatParameter m_RayLength = new MinFloatParameter(50f, 0.01f);

		[SerializeField]
		[FormerlySerializedAs("clampValue")]
		[Tooltip("Clamps the exposed intensity, this only affects reflections on opaque objects.")]
		private ClampedFloatParameter m_ClampValue = new ClampedFloatParameter(1f, 0.001f, 10f);

		[SerializeField]
		[FormerlySerializedAs("denoise")]
		[Tooltip("Denoise the ray-traced reflection.")]
		private BoolParameter m_Denoise = new BoolParameter(value: true);

		[SerializeField]
		[FormerlySerializedAs("denoiserRadius")]
		[Tooltip("Controls the radius of the ray traced reflection denoiser.")]
		private ClampedIntParameter m_DenoiserRadius = new ClampedIntParameter(8, 1, 32);

		[SerializeField]
		[Tooltip("Denoiser affects smooth surfaces.")]
		private BoolParameter m_AffectSmoothSurfaces = new BoolParameter(value: false);

		public RayTracingModeParameter mode = new RayTracingModeParameter(RayTracingMode.Quality);

		[SerializeField]
		[FormerlySerializedAs("fullResolution")]
		[Tooltip("Full Resolution")]
		private BoolParameter m_FullResolution = new BoolParameter(value: false);

		public ClampedIntParameter sampleCount = new ClampedIntParameter(1, 1, 32);

		public ClampedIntParameter bounceCount = new ClampedIntParameter(1, 1, 8);

		[SerializeField]
		[FormerlySerializedAs("rayMaxIterations")]
		private MinIntParameter m_RayMaxIterationsRT = new MinIntParameter(48, 0);

		public float minSmoothness
		{
			get
			{
				if ((UsesRayTracing() && (UsesRayTracingQualityMode() || !UsesQualitySettings())) || !UsesRayTracing())
				{
					return m_MinSmoothness.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRMinSmoothness[quality.value];
			}
			set
			{
				m_MinSmoothness.value = value;
			}
		}

		public float smoothnessFadeStart
		{
			get
			{
				if ((UsesRayTracing() && (UsesRayTracingQualityMode() || !UsesQualitySettings())) || !UsesRayTracing())
				{
					return m_SmoothnessFadeStart.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRSmoothnessFadeStart[quality.value];
			}
			set
			{
				m_SmoothnessFadeStart.value = value;
			}
		}

		public int rayMaxIterations
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_RayMaxIterations.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().SSRMaxRaySteps[quality.value];
			}
			set
			{
				m_RayMaxIterations.value = value;
			}
		}

		public float rayLength
		{
			get
			{
				if (!UsesQualitySettings() || UsesRayTracingQualityMode())
				{
					return m_RayLength.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRRayLength[quality.value];
			}
			set
			{
				m_RayLength.value = value;
			}
		}

		public float clampValue
		{
			get
			{
				if (!UsesQualitySettings() || UsesRayTracingQualityMode())
				{
					return m_ClampValue.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRClampValue[quality.value];
			}
			set
			{
				m_ClampValue.value = value;
			}
		}

		public bool denoise
		{
			get
			{
				if (!UsesQualitySettings() || UsesRayTracingQualityMode())
				{
					return m_Denoise.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRDenoise[quality.value];
			}
			set
			{
				m_Denoise.value = value;
			}
		}

		public int denoiserRadius
		{
			get
			{
				if (!UsesQualitySettings() || UsesRayTracingQualityMode())
				{
					return m_DenoiserRadius.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRDenoiserRadius[quality.value];
			}
			set
			{
				m_DenoiserRadius.value = value;
			}
		}

		public bool affectSmoothSurfaces
		{
			get
			{
				if (!UsesQualitySettings() || UsesRayTracingQualityMode())
				{
					return m_AffectSmoothSurfaces.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRSmoothDenoising[quality.value];
			}
			set
			{
				m_AffectSmoothSurfaces.value = value;
			}
		}

		public bool fullResolution
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_FullResolution.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRFullResolution[quality.value];
			}
			set
			{
				m_FullResolution.value = value;
			}
		}

		public int rayMaxIterationsRT
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_RayMaxIterationsRT.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTRRayMaxIterations[quality.value];
			}
			set
			{
				m_RayMaxIterationsRT.value = value;
			}
		}

		private bool UsesRayTracingQualityMode()
		{
			if (tracing.overrideState && tracing == RayCastingMode.RayTracing)
			{
				if (mode.overrideState)
				{
					if (mode.overrideState)
					{
						return mode == RayTracingMode.Quality;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		private bool UsesRayTracing()
		{
			HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
			if (currentAsset != null && currentAsset.currentPlatformRenderPipelineSettings.supportRayTracing && tracing.overrideState)
			{
				return tracing.value != RayCastingMode.RayMarching;
			}
			return false;
		}

		internal static bool RayTracingActive(ScreenSpaceReflection volume)
		{
			return volume.tracing.value != RayCastingMode.RayMarching;
		}
	}
}
