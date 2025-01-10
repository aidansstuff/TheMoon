using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class WaterSurface : MonoBehaviour
	{
		public enum WaterCausticsResolution
		{
			Caustics256 = 0x100,
			Caustics512 = 0x200,
			Caustics1024 = 0x400
		}

		internal static HashSet<WaterSurface> instances = new HashSet<WaterSurface>();

		internal static WaterSurface[] instancesAsArray = null;

		internal static int instanceCount = 0;

		public WaterSurfaceType surfaceType;

		public WaterGeometryType geometryType = WaterGeometryType.Infinite;

		public Mesh mesh;

		public bool cpuSimulation;

		public bool cpuFullResolution;

		public bool cpuEvaluateRipples;

		public float timeMultiplier = 1f;

		[Tooltip("")]
		public float repetitionSize = 500f;

		public float largeWindSpeed = 30f;

		public float largeWindOrientationValue;

		public float largeCurrentSpeedValue;

		public float largeCurrentOrientationValue;

		public float largeChaos = 0.8f;

		public float largeBand0Multiplier = 1f;

		public bool largeBand0FadeToggle = true;

		[Tooltip("")]
		public float largeBand0FadeStart = 1500f;

		[Tooltip("")]
		public float largeBand0FadeDistance = 3000f;

		[Tooltip("")]
		public float largeBand1Multiplier = 1f;

		[Tooltip("")]
		public bool largeBand1FadeToggle = true;

		[Tooltip("")]
		public float largeBand1FadeStart = 300f;

		[Tooltip("")]
		public float largeBand1FadeDistance = 800f;

		public bool ripples = true;

		[Tooltip("")]
		public float ripplesWindSpeed = 8f;

		[Tooltip("")]
		public WaterPropertyOverrideMode ripplesWindOrientationMode;

		[Tooltip("")]
		public float ripplesWindOrientationValue;

		[Tooltip("")]
		public WaterPropertyOverrideMode ripplesCurrentMode;

		[Tooltip("")]
		public float ripplesCurrentSpeedValue;

		[Tooltip("")]
		public float ripplesCurrentOrientationValue;

		[Tooltip("")]
		public float ripplesChaos = 0.8f;

		[Tooltip("")]
		public bool ripplesFadeToggle = true;

		[Tooltip("")]
		public float ripplesFadeStart = 50f;

		[Tooltip("")]
		public float ripplesFadeDistance = 200f;

		public Material customMaterial;

		[Tooltip("")]
		public float startSmoothness = 0.95f;

		[Tooltip("")]
		public float endSmoothness = 0.85f;

		[Tooltip("")]
		public float smoothnessFadeStart = 100f;

		[Tooltip("")]
		public float smoothnessFadeDistance = 500f;

		[Tooltip("Sets the color that is used to simulate the under-water refraction.")]
		[ColorUsage(false)]
		public Color refractionColor = new Color(0f, 0.45f, 0.65f);

		[Tooltip("Controls the maximum distance in meters used to clamp the under water refraction depth. Higher value increases the distortion amount.")]
		public float maxRefractionDistance = 1f;

		[Tooltip("Controls the approximative distance in meters that the camera can perceive through a water surface. This distance can vary widely depending on the intensity of the light the object receives.")]
		public float absorptionDistance = 5f;

		[Tooltip("Sets the color that is used to simulate the under-water scattering.")]
		[ColorUsage(false)]
		public Color scatteringColor = new Color(0f, 0.27f, 0.23f);

		[Tooltip("Controls the intensity of the height based scattering. The higher the vertical displacement, the more the water receives scattering. This can be adjusted for artistic purposes.")]
		public float ambientScattering = 0.1f;

		[Tooltip("Controls the intensity of the height based scattering. The higher the vertical displacement, the more the water receives scattering. This can be adjusted for artistic purposes.")]
		public float heightScattering = 0.1f;

		[Tooltip("Controls the intensity of the displacement based scattering. The bigger horizontal displacement, the more the water receives scattering. This can be adjusted for artistic purposes.")]
		public float displacementScattering = 0.3f;

		[Tooltip("Controls the intensity of the direct light scattering on the tip of the waves. The effect is more perceivable at grazing angles.")]
		public float directLightTipScattering = 0.6f;

		[Tooltip("Controls the intensity of the direct light scattering on the body of the waves. The effect is more perceivable at grazing angles.")]
		public float directLightBodyScattering = 0.4f;

		[Tooltip("When enabled, the water surface will render caustics.")]
		public bool caustics = true;

		[Tooltip("Sets the intensity of the under-water caustics.")]
		public float causticsIntensity = 0.5f;

		[Tooltip("Sets the vertical blending distance for the water caustics.")]
		public float causticsPlaneBlendDistance = 1f;

		[Tooltip("Specifies the resolution at which the water caustics are rendered (simulation only).")]
		public WaterCausticsResolution causticsResolution = WaterCausticsResolution.Caustics256;

		[Tooltip("Controls which band is used for the caustics evaluation.")]
		public int causticsBand = 1;

		public float virtualPlaneDistance = 5f;

		[Tooltip("")]
		public bool foam = true;

		[Tooltip("Controls the simulation foam amount. Higher values generate larger foam patches. Foam presence is highly dependent on the wind speed and chopiness values.")]
		public float simulationFoamAmount = 0.3f;

		[Tooltip("Controls the life span of the surface foam. A higher value will cause the foam to persist longer and leave a trail.")]
		public float simulationFoamDrag;

		[Tooltip("Controls the surface foam smoothness.")]
		public float simulationFoamSmoothness = 1f;

		public Texture2D foamMask;

		[Tooltip("Sets the extent of the foam mask in meters.")]
		public Vector2 foamMaskExtent = new Vector2(100f, 100f);

		[Tooltip("Sets the offset of the foam mask in meters.")]
		public Vector2 foamMaskOffset = new Vector2(0f, 0f);

		public AnimationCurve windFoamCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 1f));

		[Tooltip("Set the texture used to attenuate or suppress the simulation foam.")]
		public Texture2D foamTexture;

		[Tooltip("Set the per meter tiling for the foam texture.")]
		public float foamTextureTiling = 0.2f;

		public Texture2D waterMask;

		[Tooltip("Sets the extent of the water mask in meters.")]
		public Vector2 waterMaskExtent = new Vector2(100f, 100f);

		[Tooltip("Sets the offset of the water mask in meters.")]
		public Vector2 waterMaskOffset = new Vector2(0f, 0f);

		[Tooltip("Specifies the decal layers that affect the water surface.")]
		public DecalLayerEnum decalLayerMask = DecalLayerEnum.DecalLayerDefault;

		[Tooltip("Specifies the light layers that affect the water surface.")]
		public LightLayerEnum lightLayerMask = LightLayerEnum.LightLayerDefault;

		[Tooltip("When enabled, HDRP will apply a fog and color shift to the final image when the camera is under the surface. This feature has a cost even when the camera is above the water surface.")]
		public bool underWater;

		[Tooltip("Sets a box collider that will be used to define the volume where the under water effect is applied for non infinite surfaces.")]
		public BoxCollider volumeBounds;

		[Tooltip("Sets maximum depth at which the under water effect is evaluated for infinite surfaces.")]
		public float volumeDepth = 50f;

		[Tooltip("Sets a priority value that is used to define which surface should be considered for under water rendering in the case of multiple overlapping surfaces.")]
		public int volumePrority;

		[Tooltip("Sets a vertical distance to the water surface at which the blending between above and under water starts.")]
		public float transitionSize = 0.1f;

		[Tooltip("Sets the multiplier for the  Absorption Distance when the camera is under water. A value of 2.0 means you will see twice as far underwater.")]
		public float absorbtionDistanceMultiplier = 1f;

		internal WaterSimulationResources simulation;

		internal static void RegisterInstance(WaterSurface surface)
		{
			instances.Add(surface);
			instanceCount = instances.Count;
			if (instanceCount > 0)
			{
				instancesAsArray = new WaterSurface[instanceCount];
				instances.CopyTo(instancesAsArray);
			}
			else
			{
				instancesAsArray = null;
			}
		}

		internal static void UnregisterInstance(WaterSurface surface)
		{
			instances.Remove(surface);
			instanceCount = instances.Count;
			if (instanceCount > 0)
			{
				instancesAsArray = new WaterSurface[instanceCount];
				instances.CopyTo(instancesAsArray);
			}
			else
			{
				instancesAsArray = null;
			}
		}

		internal void CheckResources(int bandResolution, int bandCount, bool cpuSimActive, out bool gpuSpectrumValid, out bool cpuSpectrumValid, out bool historyValid)
		{
			gpuSpectrumValid = true;
			cpuSpectrumValid = true;
			historyValid = true;
			if (simulation != null && !simulation.ValidResources(bandResolution, bandCount))
			{
				simulation.ReleaseSimulationResources();
				simulation = null;
			}
			bool flag = cpuSimActive && cpuSimulation;
			if (simulation == null)
			{
				gpuSpectrumValid = false;
				cpuSpectrumValid = false;
				historyValid = false;
				simulation = new WaterSimulationResources();
				simulation.InitializeSimulationResources(bandResolution, bandCount);
				simulation.AllocateSimulationBuffersGPU();
				if (flag)
				{
					simulation.AllocateSimulationBuffersCPU();
				}
			}
			if (!flag && simulation.cpuBuffers != null)
			{
				simulation.ReleaseSimulationBuffersCPU();
				cpuSpectrumValid = false;
			}
			if (flag && simulation.cpuBuffers == null)
			{
				simulation.AllocateSimulationBuffersCPU();
				cpuSpectrumValid = false;
			}
			WaterSpectrumParameters waterSpectrumParameters = EvaluateSpectrumParams(surfaceType);
			if (simulation.spectrum.numActiveBands != waterSpectrumParameters.numActiveBands)
			{
				historyValid = false;
			}
			if (simulation.spectrum != waterSpectrumParameters)
			{
				gpuSpectrumValid = false;
				cpuSpectrumValid = false;
				simulation.spectrum = waterSpectrumParameters;
			}
			cpuSpectrumValid = false;
			simulation.rendering = EvaluateRenderingParams(surfaceType);
		}

		public WaterSimulationResolution GetSimulationResolutionCPU()
		{
			if (simulation.simulationResolution != 64)
			{
				return (WaterSimulationResolution)(cpuFullResolution ? simulation.simulationResolution : (simulation.simulationResolution / 2));
			}
			return (WaterSimulationResolution)simulation.simulationResolution;
		}

		public bool FillWaterSearchData(ref WaterSimSearchData wsd)
		{
			if (simulation != null && simulation.cpuBuffers != null)
			{
				wsd.displacementData = simulation.cpuBuffers.displacementBufferCPU;
				wsd.waterSurfaceElevation = base.transform.position.y;
				wsd.simulationRes = (int)GetSimulationResolutionCPU();
				wsd.spectrum = simulation.spectrum;
				wsd.rendering = simulation.rendering;
				wsd.activeBandCount = HDRenderPipeline.EvaluateCPUBandCount(surfaceType, ripples, cpuEvaluateRipples);
				return true;
			}
			return false;
		}

		public bool FindWaterSurfaceHeight(WaterSearchParameters wsp, out WaterSearchResult wsr)
		{
			wsr.error = float.MaxValue;
			wsr.height = 0f;
			wsr.candidateLocation = float3.zero;
			wsr.numIterations = wsp.maxIterations;
			WaterSimSearchData wsd = default(WaterSimSearchData);
			if (FillWaterSearchData(ref wsd))
			{
				HDRenderPipeline.FindWaterSurfaceHeight(wsd, wsp, out wsr);
				return true;
			}
			return false;
		}

		private void Start()
		{
			RegisterInstance(this);
		}

		private void Awake()
		{
			RegisterInstance(this);
		}

		private void OnEnable()
		{
			RegisterInstance(this);
		}

		private void OnDisable()
		{
			UnregisterInstance(this);
		}

		private bool SpectrumParametersAreValid(WaterSpectrumParameters spectrum)
		{
			return simulation.spectrum == spectrum;
		}

		private WaterSpectrumParameters EvaluateSpectrumParams(WaterSurfaceType type)
		{
			WaterSpectrumParameters result = default(WaterSpectrumParameters);
			switch (type)
			{
			case WaterSurfaceType.OceanSeaLake:
			{
				float num = repetitionSize;
				float num2 = HDRenderPipeline.EvaluateSwellSecondPatchSize(num);
				result.numActiveBands = (ripples ? 3 : 2);
				result.patchSizes.x = num;
				result.patchSizes.y = num / num2;
				result.patchSizes.z = 10f;
				result.patchWindSpeed.x = largeWindSpeed * (5f / 18f);
				result.patchWindSpeed.y = largeWindSpeed * (5f / 18f);
				result.patchWindSpeed.z = ripplesWindSpeed * (5f / 18f);
				result.patchWindOrientation.x = largeWindOrientationValue;
				result.patchWindOrientation.y = largeWindOrientationValue;
				result.patchWindOrientation.z = ((ripplesWindOrientationMode == WaterPropertyOverrideMode.Inherit) ? largeWindOrientationValue : ripplesWindOrientationValue);
				result.patchWindDirDampener.x = largeChaos;
				result.patchWindDirDampener.y = largeChaos;
				result.patchWindDirDampener.z = ripplesChaos;
				break;
			}
			case WaterSurfaceType.River:
				result.numActiveBands = ((!ripples) ? 1 : 2);
				result.patchSizes.x = repetitionSize;
				result.patchSizes.y = 10f;
				result.patchWindSpeed.x = largeWindSpeed * (5f / 18f);
				result.patchWindSpeed.y = ripplesWindSpeed * (5f / 18f);
				result.patchWindOrientation.x = largeWindOrientationValue;
				result.patchWindOrientation.y = ((ripplesWindOrientationMode == WaterPropertyOverrideMode.Inherit) ? largeWindOrientationValue : ripplesWindOrientationValue);
				result.patchWindDirDampener.x = largeChaos;
				result.patchWindDirDampener.y = ripplesChaos;
				break;
			case WaterSurfaceType.Pool:
				result.numActiveBands = 1;
				result.patchSizes.x = 10f;
				result.patchWindSpeed.x = ripplesWindSpeed * (5f / 18f);
				result.patchWindOrientation.x = ripplesWindOrientationValue;
				result.patchWindDirDampener.x = ripplesChaos;
				break;
			}
			return result;
		}

		private WaterRenderingParameters EvaluateRenderingParams(WaterSurfaceType type)
		{
			WaterRenderingParameters result = default(WaterRenderingParameters);
			result.simulationTime = simulation.simulationTime;
			switch (type)
			{
			case WaterSurfaceType.OceanSeaLake:
			{
				result.patchAmplitudeMultiplier.x = largeBand0Multiplier;
				result.patchAmplitudeMultiplier.y = largeBand1Multiplier;
				result.patchAmplitudeMultiplier.z = 1f;
				float num = largeCurrentSpeedValue * (5f / 18f);
				result.patchCurrentSpeed.x = num;
				result.patchCurrentSpeed.y = num;
				result.patchCurrentSpeed.z = ((ripplesCurrentMode == WaterPropertyOverrideMode.Inherit) ? num : (ripplesCurrentSpeedValue * (5f / 18f)));
				result.patchCurrentOrientation.x = largeCurrentOrientationValue;
				result.patchCurrentOrientation.y = largeCurrentOrientationValue;
				result.patchCurrentOrientation.z = ((ripplesCurrentMode == WaterPropertyOverrideMode.Inherit) ? largeCurrentOrientationValue : ripplesCurrentOrientationValue);
				result.patchFadeStart.x = largeBand0FadeStart;
				result.patchFadeStart.y = largeBand1FadeStart;
				result.patchFadeStart.z = ripplesFadeStart;
				result.patchFadeDistance.x = largeBand0FadeDistance;
				result.patchFadeDistance.y = largeBand1FadeDistance;
				result.patchFadeDistance.z = ripplesFadeDistance;
				result.patchFadeValue.x = (largeBand0FadeToggle ? 0f : 1f);
				result.patchFadeValue.y = (largeBand1FadeToggle ? 0f : 1f);
				result.patchFadeValue.z = (ripplesFadeToggle ? 0f : 1f);
				break;
			}
			case WaterSurfaceType.River:
				result.patchAmplitudeMultiplier.x = largeBand0Multiplier;
				result.patchAmplitudeMultiplier.y = (ripples ? 1f : 0f);
				result.patchCurrentSpeed.x = largeCurrentSpeedValue * (5f / 18f);
				result.patchCurrentSpeed.y = ((ripplesCurrentMode == WaterPropertyOverrideMode.Inherit) ? result.patchCurrentSpeed.x : (ripplesCurrentSpeedValue * (5f / 18f)));
				result.patchCurrentOrientation.x = largeCurrentOrientationValue;
				result.patchCurrentOrientation.y = ((ripplesCurrentMode == WaterPropertyOverrideMode.Inherit) ? result.patchCurrentOrientation.x : ripplesCurrentOrientationValue);
				result.patchFadeStart.x = largeBand0FadeStart;
				result.patchFadeStart.y = ripplesFadeStart;
				result.patchFadeDistance.x = largeBand0FadeDistance;
				result.patchFadeDistance.y = ripplesFadeDistance;
				result.patchFadeValue.x = (largeBand0FadeToggle ? 0f : 1f);
				result.patchFadeValue.y = (ripplesFadeToggle ? 0f : 1f);
				break;
			case WaterSurfaceType.Pool:
				result.patchAmplitudeMultiplier.x = 1f;
				result.patchAmplitudeMultiplier.y = 0f;
				result.patchCurrentSpeed.x = ripplesCurrentSpeedValue * (5f / 18f);
				result.patchCurrentOrientation.x = ripplesCurrentOrientationValue;
				result.patchFadeStart.x = ripplesFadeStart;
				result.patchFadeDistance.x = ripplesFadeDistance;
				result.patchFadeValue.x = (ripplesFadeToggle ? 0f : 1f);
				break;
			}
			return result;
		}

		internal bool IsInfinite()
		{
			if (surfaceType != 0)
			{
				return false;
			}
			return geometryType == WaterGeometryType.Infinite;
		}

		private void OnDestroy()
		{
			UnregisterInstance(this);
			if (simulation != null && simulation.AllocatedTextures())
			{
				simulation.ReleaseSimulationResources();
			}
		}
	}
}
