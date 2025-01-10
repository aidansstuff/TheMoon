using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.NVIDIA;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	public class DebugDisplaySettings : IDebugData
	{
		private class AccumulatedTiming
		{
			public float accumulatedValue;

			public float lastAverage;

			internal void UpdateLastAverage(int frameCount)
			{
				lastAverage = accumulatedValue / (float)frameCount;
				accumulatedValue = 0f;
			}
		}

		private enum DebugProfilingType
		{
			CPU = 0,
			GPU = 1,
			InlineCPU = 2
		}

		public class DebugData
		{
			public float debugOverlayRatio = 0.33f;

			public FullScreenDebugMode fullScreenDebugMode;

			public bool enableDebugDepthRemap;

			public Vector4 fullScreenDebugDepthRemap = new Vector4(0f, 1f, 0f, 0f);

			public float fullscreenDebugMip;

			public int fullScreenContactShadowLightIndex;

			[Obsolete]
			public bool xrSinglePassTestMode;

			public bool averageProfilerTimingsOverASecond;

			public MaterialDebugSettings materialDebugSettings = new MaterialDebugSettings();

			public LightingDebugSettings lightingDebugSettings = new LightingDebugSettings();

			public MipMapDebugSettings mipMapDebugSettings = new MipMapDebugSettings();

			public ColorPickerDebugSettings colorPickerDebugSettings = new ColorPickerDebugSettings();

			public MonitorsDebugSettings monitorsDebugSettings = new MonitorsDebugSettings();

			public FalseColorDebugSettings falseColorDebugSettings = new FalseColorDebugSettings();

			public DecalsDebugSettings decalsDebugSettings = new DecalsDebugSettings();

			public TransparencyDebugSettings transparencyDebugSettings = new TransparencyDebugSettings();

			public uint screenSpaceShadowIndex;

			public uint maxQuadCost = 5u;

			public uint maxVertexDensity = 10u;

			public bool countRays;

			public bool showLensFlareDataDrivenOnly;

			public int debugCameraToFreeze;

			internal RTASDebugView rtasDebugView;

			internal RTASDebugMode rtasDebugMode;

			public float minMotionVectorLength;

			internal int lightingDebugModeEnumIndex;

			internal int lightingFulscreenDebugModeEnumIndex;

			internal int materialValidatorDebugModeEnumIndex;

			internal int tileClusterDebugEnumIndex;

			internal int mipMapsEnumIndex;

			internal int engineEnumIndex;

			internal int attributesEnumIndex;

			internal int propertiesEnumIndex;

			internal int gBufferEnumIndex;

			internal int shadowDebugModeEnumIndex;

			internal int tileClusterDebugByCategoryEnumIndex;

			internal int clusterDebugModeEnumIndex;

			internal int lightVolumeDebugTypeEnumIndex;

			internal int renderingFulscreenDebugModeEnumIndex;

			internal int terrainTextureEnumIndex;

			internal int colorPickerDebugModeEnumIndex;

			internal int exposureDebugModeEnumIndex;

			internal int hdrDebugModeEnumIndex;

			internal int msaaSampleDebugModeEnumIndex;

			internal int debugCameraToFreezeEnumIndex;

			internal int rtasDebugViewEnumIndex;

			internal int rtasDebugModeEnumIndex;

			private float m_DebugGlobalMipBiasOverride;

			private bool m_UseDebugGlobalMipBiasOverride;

			[Obsolete("Moved to HDDebugDisplaySettings.Instance. Will be removed soon.")]
			public IVolumeDebugSettings volumeDebugSettings = new HDVolumeDebugSettings();

			public float GetDebugGlobalMipBiasOverride()
			{
				return m_DebugGlobalMipBiasOverride;
			}

			public void SetDebugGlobalMipBiasOverride(float value)
			{
				m_DebugGlobalMipBiasOverride = value;
			}

			internal bool UseDebugGlobalMipBiasOverride()
			{
				return m_UseDebugGlobalMipBiasOverride;
			}

			internal void SetUseDebugGlobalMipBiasOverride(bool value)
			{
				m_UseDebugGlobalMipBiasOverride = value;
			}

			internal void ResetExclusiveEnumIndices()
			{
				materialDebugSettings.materialEnumIndex = 0;
				lightingDebugModeEnumIndex = 0;
				mipMapsEnumIndex = 0;
				engineEnumIndex = 0;
				attributesEnumIndex = 0;
				propertiesEnumIndex = 0;
				gBufferEnumIndex = 0;
				lightingFulscreenDebugModeEnumIndex = 0;
				renderingFulscreenDebugModeEnumIndex = 0;
			}
		}

		private static class MaterialStrings
		{
			public static readonly DebugUI.Widget.NameAndTooltip CommonMaterialProperties = new DebugUI.Widget.NameAndTooltip
			{
				name = "Common Material Properties",
				tooltip = "Use the drop-down to select and debug a Material property to visualize on every GameObject on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Material = new DebugUI.Widget.NameAndTooltip
			{
				name = "Material",
				tooltip = "Use the drop-down to select a Material property to visualize on every GameObject on screen using a specific Shader."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Engine = new DebugUI.Widget.NameAndTooltip
			{
				name = "Engine",
				tooltip = "Use the drop-down to select a Material property to visualize on every GameObject on screen that uses a specific Shader."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Attributes = new DebugUI.Widget.NameAndTooltip
			{
				name = "Attributes",
				tooltip = "Use the drop-down to select a 3D GameObject attribute, like Texture Coordinates or Vertex Color, to visualize on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Properties = new DebugUI.Widget.NameAndTooltip
			{
				name = "Properties",
				tooltip = "Use the drop-down to select a property that the debugger uses to highlight GameObjects on screen. The debugger highlights GameObjects that use a Material with the property that you select."
			};

			public static readonly DebugUI.Widget.NameAndTooltip GBuffer = new DebugUI.Widget.NameAndTooltip
			{
				name = "GBuffer",
				tooltip = "Use the drop-down to select a property from the GBuffer to visualize for deferred Materials."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaterialValidator = new DebugUI.Widget.NameAndTooltip
			{
				name = "Material Validator",
				tooltip = "Use the drop-down to select which properties show validation colors."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValidatorTooHighColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Too High Color",
				tooltip = "Select the color that the debugger displays when a Material's diffuse color is above the acceptable PBR range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValidatorTooLowColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Too Low Color",
				tooltip = "Select the color that the debugger displays when a Material's diffuse color is below the acceptable PBR range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValidatorNotAPureMetalColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Not A Pure Metal Color",
				tooltip = "Select the color that the debugger displays if a pixel defined as metallic has a non-zero albedo value."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValidatorPureMetals = new DebugUI.Widget.NameAndTooltip
			{
				name = "Pure Metals",
				tooltip = "Enable to make the debugger highlight any pixels which Unity defines as metallic, but which have a non-zero albedo value."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideGlobalMaterialTextureMipBias = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Global Material Texture Mip Bias",
				tooltip = "Enable to override the mipmap level bias of texture samplers in material shaders."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DebugGlobalMaterialTextureMipBiasValue = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Global Material Texture Mip Bias Value",
				tooltip = "Use the slider to control the amount of mip bias of texture samplers in material shaders."
			};
		}

		private static class LightingStrings
		{
			public static readonly DebugUI.Widget.NameAndTooltip ShadowDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Shadow Debug Mode",
				tooltip = "Use the drop-down to select which shadow debug information to overlay on the screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ShadowDebugUseSelection = new DebugUI.Widget.NameAndTooltip
			{
				name = "Use Selection",
				tooltip = "Enable the checkbox to display the shadow map for the Light you have selected in the Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ShadowDebugShadowMapIndex = new DebugUI.Widget.NameAndTooltip
			{
				name = "Shadow Map Index",
				tooltip = "Use the slider to view a specific index of the shadow map. To use this property, your scene must include a Light that uses a shadow map."
			};

			public static readonly DebugUI.Widget.NameAndTooltip GlobalShadowScaleFactor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Global Shadow Scale Factor",
				tooltip = "Use the slider to set the global scale that HDRP applies to the shadow rendering resolution."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClearShadowAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Clear Shadow Atlas",
				tooltip = "Enable the checkbox to clear the shadow atlas every frame."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ShadowRangeMinimumValue = new DebugUI.Widget.NameAndTooltip
			{
				name = "Shadow Range Minimum Value",
				tooltip = "Set the minimum shadow value to display in the various shadow debug overlays."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ShadowRangeMaximumValue = new DebugUI.Widget.NameAndTooltip
			{
				name = "Shadow Range Maximum Value",
				tooltip = "Set the maximum shadow value to display in the various shadow debug overlays."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LogCachedShadowAtlasStatus = new DebugUI.Widget.NameAndTooltip
			{
				name = "Log Cached Shadow Atlas Status",
				tooltip = "Displays a list of the Lights currently in the cached shadow atlas in the Console."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ShowLightsByType = new DebugUI.Widget.NameAndTooltip
			{
				name = "Show Lights By Type",
				tooltip = "Allows the user to enable or disable lights in the scene based on their type. This will not change the actual settings of the light."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DirectionalLights = new DebugUI.Widget.NameAndTooltip
			{
				name = "Directional Lights",
				tooltip = "Temporarily enables or disables Directional Lights in your Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip PunctualLights = new DebugUI.Widget.NameAndTooltip
			{
				name = "Punctual Lights",
				tooltip = "Temporarily enables or disables Punctual Lights in your Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AreaLights = new DebugUI.Widget.NameAndTooltip
			{
				name = "Area Lights",
				tooltip = "Temporarily enables or disables Area Lights in your Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ReflectionProbes = new DebugUI.Widget.NameAndTooltip
			{
				name = "Reflection Probes",
				tooltip = "Temporarily enables or disables Reflection Probes in your Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Exposure = new DebugUI.Widget.NameAndTooltip
			{
				name = "Exposure",
				tooltip = "Allows the selection of an Exposure debug mode to use."
			};

			public static readonly DebugUI.Widget.NameAndTooltip HDROutput = new DebugUI.Widget.NameAndTooltip
			{
				name = "HDR",
				tooltip = "Allows the selection of an HDR debug mode to use."
			};

			public static readonly DebugUI.Widget.NameAndTooltip HDROutputDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "DebugMode",
				tooltip = "Use the drop-down to select a debug mode for HDR Output."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ExposureDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "DebugMode",
				tooltip = "Use the drop-down to select a debug mode to validate the exposure."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ExposureDisplayMaskOnly = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Mask Only",
				tooltip = "Display only the metering mask in the picture-in-picture. When disabled, the mask is visible after weighting the scene color instead."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ExposureShowTonemapCurve = new DebugUI.Widget.NameAndTooltip
			{
				name = "Show Tonemap Curve",
				tooltip = "Overlay the tonemap curve to the histogram debug view."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisplayHistogramSceneOverlay = new DebugUI.Widget.NameAndTooltip
			{
				name = "Show Scene Overlay",
				tooltip = "Display the scene overlay showing pixels excluded by the exposure computation via histogram."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ExposureCenterAroundExposure = new DebugUI.Widget.NameAndTooltip
			{
				name = "Center Around Exposure",
				tooltip = "Center the histogram around the current exposure value."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ExposureDisplayRGBHistogram = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display RGB Histogram",
				tooltip = "Display the Final Image Histogram as an RGB histogram instead of just luminance."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DebugExposureCompensation = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Exposure Compensation",
				tooltip = "Set an additional exposure on top of your current exposure for debug purposes."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightingDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Lighting Debug Mode",
				tooltip = "Use the drop-down to select a lighting mode to debug."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightHierarchyDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Light Hierarchy Debug Mode",
				tooltip = "Use the drop-down to select a light type to show the direct lighting for or a Reflection Probe type to show the indirect lighting for."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightLayersVisualization = new DebugUI.Widget.NameAndTooltip
			{
				name = "Light Layers Visualization",
				tooltip = "Visualize the light layers of GameObjects in your Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightLayersUseSelectedLight = new DebugUI.Widget.NameAndTooltip
			{
				name = "Use Selected Light",
				tooltip = "Visualize GameObjects affected by the selected light."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightLayersSwitchToLightShadowLayers = new DebugUI.Widget.NameAndTooltip
			{
				name = "Switch To Light's Shadow Layers",
				tooltip = "Visualize GameObjects that cast shadows for the selected light."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightLayersFilterLayers = new DebugUI.Widget.NameAndTooltip
			{
				name = "Filter Layers",
				tooltip = "Use the drop-down to filter light layers that you want to visialize."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightLayersColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Layers Color",
				tooltip = "Select the display color of each light layer."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideSmoothness = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Smoothness",
				tooltip = "Enable the checkbox to override the smoothness for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideAlbedo = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Albedo",
				tooltip = "Enable the checkbox to override the albedo for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideNormal = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Normal",
				tooltip = "Enable the checkbox to override the normals for the entire Scene with object normals for lighting debug."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideSpecularColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Specular Color",
				tooltip = "Enable the checkbox to override the specular color for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideAmbientOcclusion = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Ambient Occlusion",
				tooltip = "Enable the checkbox to override the ambient occlusion for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverrideEmissiveColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Emissive Color",
				tooltip = "Enable the checkbox to override the emissive color for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Smoothness = new DebugUI.Widget.NameAndTooltip
			{
				name = "Smoothness",
				tooltip = "Use the slider to set the smoothness override value that HDRP uses for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Albedo = new DebugUI.Widget.NameAndTooltip
			{
				name = "Albedo",
				tooltip = "Use the color picker to set the albedo color that HDRP uses for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip SpecularColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Specular Color",
				tooltip = "Use the color picker to set the specular color that HDRP uses for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AmbientOcclusion = new DebugUI.Widget.NameAndTooltip
			{
				name = "Ambient Occlusion",
				tooltip = "Use the slider to set the Ambient Occlusion override value that HDRP uses for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip EmissiveColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Emissive Color",
				tooltip = "Use the color picker to set the emissive color that HDRP uses for the entire Scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FullscreenDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Fullscreen Debug Mode",
				tooltip = "Use the drop-down to select a rendering mode to display as an overlay on the screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ScreenSpaceShadowIndex = new DebugUI.Widget.NameAndTooltip
			{
				name = "Screen Space Shadow Index",
				tooltip = "Select the index of the screen space shadows to view with the slider. There must be a Light in the scene that uses Screen Space Shadows."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DepthPyramidDebugMip = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Mip",
				tooltip = "Enable to view a lower-resolution mipmap."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DepthPyramidEnableRemap = new DebugUI.Widget.NameAndTooltip
			{
				name = "Enable Depth Remap",
				tooltip = "Enable remapping of displayed depth values for better vizualization."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DepthPyramidRangeMin = new DebugUI.Widget.NameAndTooltip
			{
				name = "Depth Range Min Value",
				tooltip = "Distance at which depth values remap starts (0 is near plane, 1 is far plane)"
			};

			public static readonly DebugUI.Widget.NameAndTooltip DepthPyramidRangeMax = new DebugUI.Widget.NameAndTooltip
			{
				name = "Depth Range Max Value",
				tooltip = "Distance at which depth values remap ends (0 is near plane, 1 is far plane)"
			};

			public static readonly DebugUI.Widget.NameAndTooltip ContactShadowsLightIndex = new DebugUI.Widget.NameAndTooltip
			{
				name = "Light Index",
				tooltip = "Enable to display Contact shadows for each Light individually."
			};

			public static readonly DebugUI.Widget.NameAndTooltip RTASDebugView = new DebugUI.Widget.NameAndTooltip
			{
				name = "Ray Tracing Acceleration Structure View",
				tooltip = "Use the drop-down to select a rendering view to display the ray tracing acceleration structure."
			};

			public static readonly DebugUI.Widget.NameAndTooltip RTASDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Ray Tracing Acceleration Structure Mode",
				tooltip = "Use the drop-down to select a rendering mode to display the ray tracing acceleration structure."
			};

			public static readonly DebugUI.Widget.NameAndTooltip TileClusterDebug = new DebugUI.Widget.NameAndTooltip
			{
				name = "Tile/Cluster Debug",
				tooltip = "Use the drop-down to select the Light type that you want to show the Tile/Cluster debug information for."
			};

			public static readonly DebugUI.Widget.NameAndTooltip TileClusterDebugByCategory = new DebugUI.Widget.NameAndTooltip
			{
				name = "Tile/Cluster Debug By Category",
				tooltip = "Use the drop-down to select the visualization mode for the cluster."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClusterDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Cluster Debug Mode",
				tooltip = "Select the debug visualization mode for the Cluster."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClusterDistance = new DebugUI.Widget.NameAndTooltip
			{
				name = "Cluster Distance",
				tooltip = "Set the distance from the camera that HDRP displays the Cluster slice."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisplaySkyReflection = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Sky Reflection",
				tooltip = "Enable the checkbox to display an overlay of the cube map that the current sky generates and HDRP uses for lighting."
			};

			public static readonly DebugUI.Widget.NameAndTooltip SkyReflectionMipmap = new DebugUI.Widget.NameAndTooltip
			{
				name = "Sky Reflection Mipmap",
				tooltip = "Use the slider to set the mipmap level of the sky reflection cubemap. Use this to view the sky reflection cubemap's different mipmap levels."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisplayLightVolumes = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Light Volumes",
				tooltip = "Enable the checkbox to show an overlay of all light bounding volumes."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightVolumeDebugType = new DebugUI.Widget.NameAndTooltip
			{
				name = "Light Volume Debug Type",
				tooltip = "Use the drop-down to select the method HDRP uses to display the light volumes."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaxDebugLightCount = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Debug Light Count",
				tooltip = "Use this property to change the maximum acceptable number of lights for your application and still see areas in red."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisplayCookieAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Cookie Atlas",
				tooltip = "Enable the checkbox to display an overlay of the cookie atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip CookieAtlasMipLevel = new DebugUI.Widget.NameAndTooltip
			{
				name = "Mip Level",
				tooltip = "Use the slider to set the mipmap level of the cookie atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClearCookieAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Clear Cookie Atlas",
				tooltip = "Enable to clear the cookie atlas at each frame."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisplayReflectionProbeAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Reflection Probe Atlas",
				tooltip = "Enable the checkbox to display an overlay of the reflection probe atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ReflectionProbeAtlasMipLevel = new DebugUI.Widget.NameAndTooltip
			{
				name = "Mip Level",
				tooltip = "Use the slider to set the mipmap level of the reflection probe atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ReflectionProbeAtlasSlice = new DebugUI.Widget.NameAndTooltip
			{
				name = "Slice",
				tooltip = "Use the slider to set the slice of the reflection probe atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ReflectionProbeApplyExposure = new DebugUI.Widget.NameAndTooltip
			{
				name = "Apply Exposure",
				tooltip = "Apply exposure to displayed atlas."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClearReflectionProbeAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Clear Reflection Probe Atlas",
				tooltip = "Enable to clear the reflection probe atlas each frame."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DebugOverlayScreenRatio = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Overlay Screen Ratio",
				tooltip = "Set the size of the debug overlay textures with a ratio of the screen size."
			};
		}

		private static class RenderingStrings
		{
			public static readonly DebugUI.Widget.NameAndTooltip FullscreenDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Fullscreen Debug Mode",
				tooltip = "Use the drop-down to select a rendering mode to display as an overlay on the screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaxOverdrawCount = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Overdraw Count",
				tooltip = "Maximum overdraw count allowed for a single pixel."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaxQuadCost = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Quad Cost",
				tooltip = "The scale of the quad mode overdraw heat map."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaxVertexDensity = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Vertex Density",
				tooltip = "The scale of the vertex density mode overdraw heat map."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMaps = new DebugUI.Widget.NameAndTooltip
			{
				name = "Mip Maps",
				tooltip = "Use the drop-down to select a mipmap property to debug."
			};

			public static readonly DebugUI.Widget.NameAndTooltip TerrainTexture = new DebugUI.Widget.NameAndTooltip
			{
				name = "Terrain Texture",
				tooltip = "Use the drop-down to select the terrain Texture to debug the mipmap for."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ColorPickerDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Mode",
				tooltip = "Use the drop-down to select the format of the color picker display."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ColorPickerFontColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Font Color",
				tooltip = "Use the color picker to select a color for the font that the Color Picker uses for its display."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FalseColorMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "False Color Mode",
				tooltip = "Enable the checkbox to define intensity ranges that the debugger uses to show a color temperature gradient for the current frame."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FalseColorRangeThreshold0 = new DebugUI.Widget.NameAndTooltip
			{
				name = "Range Threshold 0",
				tooltip = "Set the split for the intensity range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FalseColorRangeThreshold1 = new DebugUI.Widget.NameAndTooltip
			{
				name = "Range Threshold 1",
				tooltip = "Set the split for the intensity range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FalseColorRangeThreshold2 = new DebugUI.Widget.NameAndTooltip
			{
				name = "Range Threshold 2",
				tooltip = "Set the split for the intensity range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FalseColorRangeThreshold3 = new DebugUI.Widget.NameAndTooltip
			{
				name = "Range Threshold 3",
				tooltip = "Set the split for the intensity range."
			};

			public static readonly DebugUI.Widget.NameAndTooltip FreezeCameraForCulling = new DebugUI.Widget.NameAndTooltip
			{
				name = "Freeze Camera For Culling",
				tooltip = "Use the drop-down to select a Camera to freeze in order to check its culling. To check if the Camera's culling works correctly, freeze the Camera and move occluders around it."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MonitorsSize = new DebugUI.Widget.NameAndTooltip
			{
				name = "Size",
				tooltip = "Sets the size ratio of the displayed monitors"
			};

			public static readonly DebugUI.Widget.NameAndTooltip WaveformToggle = new DebugUI.Widget.NameAndTooltip
			{
				name = "Waveform",
				tooltip = "Toggles the waveform monitor, displaying the full range of luma information in the render."
			};

			public static readonly DebugUI.Widget.NameAndTooltip WaveformExposure = new DebugUI.Widget.NameAndTooltip
			{
				name = "Exposure",
				tooltip = "Set the exposure of the waveform monitor."
			};

			public static readonly DebugUI.Widget.NameAndTooltip WaveformParade = new DebugUI.Widget.NameAndTooltip
			{
				name = "Parade mode",
				tooltip = "Toggles the parade mode of the waveform monitor, splitting the waveform into the red, green and blue channels separately."
			};

			public static readonly DebugUI.Widget.NameAndTooltip VectorscopeToggle = new DebugUI.Widget.NameAndTooltip
			{
				name = "Vectorscope",
				tooltip = "Toggles the vectorscope monitor, allowing to measure the overall range of hue and saturation within the image."
			};

			public static readonly DebugUI.Widget.NameAndTooltip VectorscopeExposure = new DebugUI.Widget.NameAndTooltip
			{
				name = "Exposure",
				tooltip = "Set the exposure of the vectorscope monitor."
			};
		}

		private static class DecalStrings
		{
			public static readonly DebugUI.Widget.NameAndTooltip DisplayAtlas = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Atlas",
				tooltip = "Enable the checkbox to debug and display the decal atlas for a Camera in the top left of that Camera's view."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipLevel = new DebugUI.Widget.NameAndTooltip
			{
				name = "Mip Level",
				tooltip = "Use the slider to select the mip level for the decal atlas."
			};
		}

		private static string k_PanelDisplayStats = "Display Stats";

		private static string k_PanelMaterials = "Material";

		private static string k_PanelLighting = "Lighting";

		private static string k_PanelRendering = "Rendering";

		private static string k_PanelDecals = "Decals";

		private DebugUI.Widget[] m_DebugDisplayStatsItems;

		private DebugUI.Widget[] m_DebugMaterialItems;

		private DebugUI.Widget[] m_DebugLightingItems;

		private DebugUI.Widget[] m_DebugRenderingItems;

		private DebugUI.Widget[] m_DebugDecalsItems;

		private static GUIContent[] s_LightingFullScreenDebugStrings = null;

		private static int[] s_LightingFullScreenDebugValues = null;

		private static GUIContent[] s_RenderingFullScreenDebugStrings = null;

		private static int[] s_RenderingFullScreenDebugValues = null;

		private static GUIContent[] s_MaterialFullScreenDebugStrings = null;

		private static int[] s_MaterialFullScreenDebugValues = null;

		private static List<GUIContent> s_CameraNames = new List<GUIContent>();

		private static GUIContent[] s_CameraNamesStrings = new GUIContent[1]
		{
			new GUIContent("No Visible Camera")
		};

		private static int[] s_CameraNamesValues = new int[1];

		private static bool needsRefreshingCameraFreezeList = true;

		private List<HDProfileId> m_RecordedSamplers = new List<HDProfileId>();

		private Dictionary<int, AccumulatedTiming> m_AccumulatedGPUTiming = new Dictionary<int, AccumulatedTiming>();

		private Dictionary<int, AccumulatedTiming> m_AccumulatedCPUTiming = new Dictionary<int, AccumulatedTiming>();

		private Dictionary<int, AccumulatedTiming> m_AccumulatedInlineCPUTiming = new Dictionary<int, AccumulatedTiming>();

		private float m_TimeSinceLastAvgValue;

		private int m_AccumulatedFrames;

		private const float k_AccumulationTimeInSeconds = 1f;

		private List<HDProfileId> m_RecordedSamplersRT = new List<HDProfileId>();

		internal DebugFrameTiming debugFrameTiming = new DebugFrameTiming();

		private DebugData m_Data;

		internal DebugView nvidiaDebugView { get; } = new DebugView();


		public DebugData data => m_Data;

		public static GUIContent[] renderingFullScreenDebugStrings => s_RenderingFullScreenDebugStrings;

		public static int[] renderingFullScreenDebugValues => s_RenderingFullScreenDebugValues;

		public static GUIContent[] lightingFullScreenDebugStrings => s_LightingFullScreenDebugStrings;

		public static int[] lightingFullScreenDebugValues => s_LightingFullScreenDebugValues;

		[Obsolete("Use autoenum instead @from(2022.2)")]
		public static GUIContent[] lightingFullScreenRTASDebugViewStrings => (from t in Enum.GetNames(typeof(RTASDebugView))
			select new GUIContent(t)).ToArray();

		[Obsolete("Use autoenum instead @from(2022.2)")]
		public static int[] lightingFullScreenRTASDebugViewValues => (int[])Enum.GetValues(typeof(RTASDebugView));

		[Obsolete("Use autoenum instead @from(2022.2)")]
		public static GUIContent[] lightingFullScreenRTASDebugModeStrings => (from t in Enum.GetNames(typeof(RTASDebugMode))
			select new GUIContent(t)).ToArray();

		[Obsolete("Use autoenum instead @from(2022.2)")]
		public static int[] lightingFullScreenRTASDebugModeValues => (int[])Enum.GetValues(typeof(RTASDebugMode));

		internal DebugDisplaySettings()
		{
			FillFullScreenDebugEnum(ref s_LightingFullScreenDebugStrings, ref s_LightingFullScreenDebugValues, FullScreenDebugMode.MinLightingFullScreenDebug, FullScreenDebugMode.MaxLightingFullScreenDebug);
			FillFullScreenDebugEnum(ref s_RenderingFullScreenDebugStrings, ref s_RenderingFullScreenDebugValues, FullScreenDebugMode.MinRenderingFullScreenDebug, FullScreenDebugMode.MaxRenderingFullScreenDebug);
			FillFullScreenDebugEnum(ref s_MaterialFullScreenDebugStrings, ref s_MaterialFullScreenDebugValues, FullScreenDebugMode.MinMaterialFullScreenDebug, FullScreenDebugMode.MaxMaterialFullScreenDebug);
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			if (graphicsDeviceType == GraphicsDeviceType.Metal || graphicsDeviceType == GraphicsDeviceType.PlayStation4 || graphicsDeviceType == GraphicsDeviceType.PlayStation5 || graphicsDeviceType == GraphicsDeviceType.PlayStation5NGGC)
			{
				s_RenderingFullScreenDebugStrings = s_RenderingFullScreenDebugStrings.Where((GUIContent val, int idx) => idx + 22 != 31).ToArray();
				s_RenderingFullScreenDebugValues = s_RenderingFullScreenDebugValues.Where((int val, int idx) => idx + 22 != 31).ToArray();
				s_RenderingFullScreenDebugStrings = s_RenderingFullScreenDebugStrings.Where((GUIContent val, int idx) => idx + 22 != 29).ToArray();
				s_RenderingFullScreenDebugValues = s_RenderingFullScreenDebugValues.Where((int val, int idx) => idx + 22 != 29).ToArray();
			}
			s_MaterialFullScreenDebugStrings[1] = new GUIContent("Diffuse Color");
			s_MaterialFullScreenDebugStrings[2] = new GUIContent("Metal or SpecularColor");
			m_Data = new DebugData();
		}

		Action IDebugData.GetReset()
		{
			return delegate
			{
				m_Data = new DebugData();
			};
		}

		internal float[] GetDebugMaterialIndexes()
		{
			return data.materialDebugSettings.GetDebugMaterialIndexes();
		}

		public DebugLightFilterMode GetDebugLightFilterMode()
		{
			return data.lightingDebugSettings.debugLightFilterMode;
		}

		public DebugLightingMode GetDebugLightingMode()
		{
			return data.lightingDebugSettings.debugLightingMode;
		}

		public DebugLightLayersMask GetDebugLightLayersMask()
		{
			LightingDebugSettings lightingDebugSettings = data.lightingDebugSettings;
			if (!lightingDebugSettings.debugLightLayers)
			{
				return DebugLightLayersMask.None;
			}
			return lightingDebugSettings.debugLightLayersFilterMask;
		}

		public ShadowMapDebugMode GetDebugShadowMapMode()
		{
			return data.lightingDebugSettings.shadowDebugMode;
		}

		public DebugMipMapMode GetDebugMipMapMode()
		{
			return data.mipMapDebugSettings.debugMipMapMode;
		}

		public DebugMipMapModeTerrainTexture GetDebugMipMapModeTerrainTexture()
		{
			return data.mipMapDebugSettings.terrainTexture;
		}

		public ColorPickerDebugMode GetDebugColorPickerMode()
		{
			return data.colorPickerDebugSettings.colorPickerMode;
		}

		public bool IsCameraFreezeEnabled()
		{
			return data.debugCameraToFreeze != 0;
		}

		public bool IsCameraFrozen(Camera camera)
		{
			if (IsCameraFreezeEnabled())
			{
				return camera.name.Equals(s_CameraNamesStrings[data.debugCameraToFreeze].text);
			}
			return false;
		}

		public bool IsDebugDisplayEnabled()
		{
			if (!data.materialDebugSettings.IsDebugDisplayEnabled() && !data.lightingDebugSettings.IsDebugDisplayEnabled() && !data.mipMapDebugSettings.IsDebugDisplayEnabled())
			{
				return IsDebugFullScreenEnabled();
			}
			return true;
		}

		public bool IsDebugMaterialDisplayEnabled()
		{
			return data.materialDebugSettings.IsDebugDisplayEnabled();
		}

		public bool IsDebugFullScreenEnabled()
		{
			return data.fullScreenDebugMode != FullScreenDebugMode.None;
		}

		internal bool IsFullScreenDebugPassEnabled()
		{
			if (data.fullScreenDebugMode != FullScreenDebugMode.QuadOverdraw)
			{
				return data.fullScreenDebugMode == FullScreenDebugMode.VertexDensity;
			}
			return true;
		}

		public bool IsDebugExposureModeEnabled()
		{
			return data.lightingDebugSettings.exposureDebugMode != ExposureDebugMode.None;
		}

		public bool IsHDRDebugModeEnabled()
		{
			return data.lightingDebugSettings.hdrDebugMode != HDRDebugMode.None;
		}

		public bool IsMaterialValidationEnabled()
		{
			if (data.fullScreenDebugMode != FullScreenDebugMode.ValidateDiffuseColor)
			{
				return data.fullScreenDebugMode == FullScreenDebugMode.ValidateSpecularColor;
			}
			return true;
		}

		public bool IsDebugMipMapDisplayEnabled()
		{
			return data.mipMapDebugSettings.IsDebugDisplayEnabled();
		}

		public bool IsMatcapViewEnabled(HDCamera camera)
		{
			if (!CoreUtils.IsSceneLightingDisabled(camera.camera))
			{
				return GetDebugLightingMode() == DebugLightingMode.MatcapView;
			}
			return true;
		}

		private void DisableNonMaterialDebugSettings()
		{
			data.fullScreenDebugMode = FullScreenDebugMode.None;
			data.lightingDebugSettings.debugLightingMode = DebugLightingMode.None;
			data.mipMapDebugSettings.debugMipMapMode = DebugMipMapMode.None;
			data.lightingDebugSettings.debugLightLayers = false;
		}

		public void SetDebugViewCommonMaterialProperty(MaterialSharedProperty value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewCommonMaterialProperty(value);
		}

		public void SetDebugViewMaterial(int value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewMaterial(value);
		}

		public void SetDebugViewEngine(int value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewEngine(value);
		}

		public void SetDebugViewVarying(DebugViewVarying value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewVarying(value);
		}

		public void SetDebugViewProperties(DebugViewProperties value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewProperties(value);
		}

		public void SetDebugViewGBuffer(int value)
		{
			if (value != 0)
			{
				DisableNonMaterialDebugSettings();
			}
			data.materialDebugSettings.SetDebugViewGBuffer(value);
		}

		public void SetFullScreenDebugMode(FullScreenDebugMode value)
		{
			if (data.lightingDebugSettings.shadowDebugMode == ShadowMapDebugMode.SingleShadow)
			{
				value = FullScreenDebugMode.None;
			}
			if (value != 0)
			{
				data.lightingDebugSettings.debugLightingMode = DebugLightingMode.None;
				data.lightingDebugSettings.debugLightLayers = false;
				data.materialDebugSettings.DisableMaterialDebug();
				data.mipMapDebugSettings.debugMipMapMode = DebugMipMapMode.None;
			}
			data.fullScreenDebugMode = value;
		}

		public void SetRTASDebugView(RTASDebugView value)
		{
			data.rtasDebugView = value;
		}

		public void SetRTASDebugMode(RTASDebugMode value)
		{
			data.rtasDebugMode = value;
		}

		public void SetShadowDebugMode(ShadowMapDebugMode value)
		{
			if (value == ShadowMapDebugMode.SingleShadow)
			{
				data.fullScreenDebugMode = FullScreenDebugMode.None;
			}
			data.lightingDebugSettings.shadowDebugMode = value;
		}

		public void SetDebugLightFilterMode(DebugLightFilterMode value)
		{
			if (value != 0)
			{
				data.materialDebugSettings.DisableMaterialDebug();
				data.mipMapDebugSettings.debugMipMapMode = DebugMipMapMode.None;
				data.lightingDebugSettings.debugLightLayers = false;
			}
			data.lightingDebugSettings.debugLightFilterMode = value;
		}

		public void SetDebugLightLayersMode(bool value)
		{
			if (value)
			{
				data.ResetExclusiveEnumIndices();
				data.lightingDebugSettings.debugLightFilterMode = DebugLightFilterMode.None;
				Type typeFromHandle = typeof(Builtin.BuiltinData);
				GenerateHLSL generateHLSL = typeFromHandle.GetCustomAttributes(inherit: true)[0] as GenerateHLSL;
				int num = Array.IndexOf(typeFromHandle.GetFields(), typeFromHandle.GetField("renderingLayers"));
				SetDebugViewMaterial(generateHLSL.paramDefinesStart + num);
			}
			else
			{
				SetDebugViewMaterial(0);
			}
			data.lightingDebugSettings.debugLightLayers = value;
		}

		public void SetDebugLightingMode(DebugLightingMode value)
		{
			if (value != 0)
			{
				data.fullScreenDebugMode = FullScreenDebugMode.None;
				data.materialDebugSettings.DisableMaterialDebug();
				data.mipMapDebugSettings.debugMipMapMode = DebugMipMapMode.None;
				data.lightingDebugSettings.debugLightLayers = false;
			}
			data.lightingDebugSettings.debugLightingMode = value;
		}

		internal void SetExposureDebugMode(ExposureDebugMode value)
		{
			data.lightingDebugSettings.exposureDebugMode = value;
		}

		internal void SetHDRDebugMode(HDRDebugMode value)
		{
			data.lightingDebugSettings.hdrDebugMode = value;
		}

		public void SetMipMapMode(DebugMipMapMode value)
		{
			if (value != 0)
			{
				data.materialDebugSettings.DisableMaterialDebug();
				data.lightingDebugSettings.debugLightingMode = DebugLightingMode.None;
				data.lightingDebugSettings.debugLightLayers = false;
				data.fullScreenDebugMode = FullScreenDebugMode.None;
			}
			data.mipMapDebugSettings.debugMipMapMode = value;
		}

		private void EnableProfilingRecorders()
		{
			m_RecordedSamplers.Add(HDProfileId.HDRenderPipelineAllRenderRequest);
			m_RecordedSamplers.Add(HDProfileId.VolumeUpdate);
			m_RecordedSamplers.Add(HDProfileId.RenderShadowMaps);
			m_RecordedSamplers.Add(HDProfileId.GBuffer);
			m_RecordedSamplers.Add(HDProfileId.PrepareLightsForGPU);
			m_RecordedSamplers.Add(HDProfileId.VolumeVoxelization);
			m_RecordedSamplers.Add(HDProfileId.VolumetricLighting);
			m_RecordedSamplers.Add(HDProfileId.VolumetricClouds);
			m_RecordedSamplers.Add(HDProfileId.VolumetricCloudsTrace);
			m_RecordedSamplers.Add(HDProfileId.VolumetricCloudsReproject);
			m_RecordedSamplers.Add(HDProfileId.VolumetricCloudsUpscaleAndCombine);
			m_RecordedSamplers.Add(HDProfileId.RenderDeferredLightingCompute);
			m_RecordedSamplers.Add(HDProfileId.ForwardOpaque);
			m_RecordedSamplers.Add(HDProfileId.ForwardTransparent);
			m_RecordedSamplers.Add(HDProfileId.ForwardPreRefraction);
			m_RecordedSamplers.Add(HDProfileId.ColorPyramid);
			m_RecordedSamplers.Add(HDProfileId.DepthPyramid);
			m_RecordedSamplers.Add(HDProfileId.PostProcessing);
		}

		private void DisableProfilingRecorders(List<HDProfileId> samplers)
		{
			foreach (HDProfileId sampler in samplers)
			{
				ProfilingSampler.Get(sampler).enableRecording = false;
			}
			samplers.Clear();
		}

		private void EnableProfilingRecordersRT()
		{
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingBuildCluster);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingCullLights);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingBuildAccelerationStructure);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingReflectionDirectionGeneration);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingReflectionEvaluation);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingReflectionAdjustWeight);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingReflectionUpscale);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingReflectionFilter);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingAmbientOcclusion);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingFilterAmbientOcclusion);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingDirectionalLightShadow);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingLightShadow);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingIndirectDiffuseDirectionGeneration);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingIndirectDiffuseEvaluation);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingIndirectDiffuseUpscale);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingFilterIndirectDiffuse);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingDebugOverlay);
			m_RecordedSamplersRT.Add(HDProfileId.ForwardPreRefraction);
			m_RecordedSamplersRT.Add(HDProfileId.RayTracingRecursiveRendering);
			m_RecordedSamplersRT.Add(HDProfileId.RayTracingDepthPrepass);
			m_RecordedSamplersRT.Add(HDProfileId.RayTracingFlagMask);
			m_RecordedSamplersRT.Add(HDProfileId.RaytracingDeferredLighting);
		}

		private float GetSamplerTiming(HDProfileId samplerId, ProfilingSampler sampler, DebugProfilingType type)
		{
			if (!data.averageProfilerTimingsOverASecond)
			{
				return type switch
				{
					DebugProfilingType.GPU => sampler.gpuElapsedTime, 
					DebugProfilingType.CPU => sampler.cpuElapsedTime, 
					_ => sampler.inlineCpuElapsedTime, 
				};
			}
			object obj = type switch
			{
				DebugProfilingType.InlineCPU => m_AccumulatedInlineCPUTiming, 
				DebugProfilingType.CPU => m_AccumulatedCPUTiming, 
				_ => m_AccumulatedGPUTiming, 
			};
			AccumulatedTiming value = null;
			if (((Dictionary<int, AccumulatedTiming>)obj).TryGetValue((int)samplerId, out value))
			{
				return value.lastAverage;
			}
			return 0f;
		}

		private ObservableList<DebugUI.Widget> BuildProfilingSamplerWidgetList(List<HDProfileId> samplerList)
		{
			ObservableList<DebugUI.Widget> observableList = new ObservableList<DebugUI.Widget>();
			foreach (HDProfileId sampler in samplerList)
			{
				ProfilingSampler profilingSampler = ProfilingSampler.Get(sampler);
				profilingSampler.enableRecording = true;
				observableList.Add(new DebugUI.ValueTuple
				{
					displayName = profilingSampler.name,
					values = new DebugUI.Value[3]
					{
						CreateWidgetForSampler(sampler, profilingSampler, DebugProfilingType.CPU),
						CreateWidgetForSampler(sampler, profilingSampler, DebugProfilingType.InlineCPU),
						CreateWidgetForSampler(sampler, profilingSampler, DebugProfilingType.GPU)
					}
				});
			}
			return observableList;
			DebugUI.Value CreateWidgetForSampler(HDProfileId samplerId, ProfilingSampler sampler, DebugProfilingType type)
			{
				Dictionary<int, AccumulatedTiming> dictionary = ((type == DebugProfilingType.CPU) ? m_AccumulatedCPUTiming : ((type == DebugProfilingType.InlineCPU) ? m_AccumulatedInlineCPUTiming : m_AccumulatedGPUTiming));
				if (!dictionary.ContainsKey((int)samplerId))
				{
					dictionary.Add((int)samplerId, new AccumulatedTiming());
				}
				return new DebugUI.Value
				{
					formatString = "{0:F2}ms",
					refreshRate = 0.2f,
					getter = () => GetSamplerTiming(samplerId, sampler, type)
				};
			}
		}

		private void UpdateListOfAveragedProfilerTimings(List<HDProfileId> samplers, bool needUpdatingAverages)
		{
			foreach (HDProfileId sampler in samplers)
			{
				ProfilingSampler profilingSampler = ProfilingSampler.Get(sampler);
				AccumulatedTiming value = null;
				if (m_AccumulatedCPUTiming.TryGetValue((int)sampler, out value))
				{
					value.accumulatedValue += profilingSampler.cpuElapsedTime;
				}
				AccumulatedTiming value2 = null;
				if (m_AccumulatedInlineCPUTiming.TryGetValue((int)sampler, out value2))
				{
					value2.accumulatedValue += profilingSampler.inlineCpuElapsedTime;
				}
				AccumulatedTiming value3 = null;
				if (m_AccumulatedGPUTiming.TryGetValue((int)sampler, out value3))
				{
					value3.accumulatedValue += profilingSampler.gpuElapsedTime;
				}
				if (needUpdatingAverages)
				{
					value?.UpdateLastAverage(m_AccumulatedFrames);
					value2?.UpdateLastAverage(m_AccumulatedFrames);
					value3?.UpdateLastAverage(m_AccumulatedFrames);
				}
			}
		}

		internal void UpdateAveragedProfilerTimings()
		{
			m_TimeSinceLastAvgValue += Time.unscaledDeltaTime;
			m_AccumulatedFrames++;
			bool flag = m_TimeSinceLastAvgValue >= 1f;
			UpdateListOfAveragedProfilerTimings(m_RecordedSamplers, flag);
			UpdateListOfAveragedProfilerTimings(m_RecordedSamplersRT, flag);
			if (flag)
			{
				m_TimeSinceLastAvgValue = 0f;
				m_AccumulatedFrames = 0;
			}
		}

		private void RegisterDisplayStatsDebug()
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			debugFrameTiming.RegisterDebugUI(list);
			EnableProfilingRecorders();
			list.Add(new DebugUI.BoolField
			{
				displayName = "Update every second with average",
				getter = () => data.averageProfilerTimingsOverASecond,
				setter = delegate(bool value)
				{
					data.averageProfilerTimingsOverASecond = value;
				}
			});
			list.Add(new DebugUI.Foldout("Detailed Stats", BuildProfilingSamplerWidgetList(m_RecordedSamplers), new string[3] { "CPU", "CPUInline", "GPU" }));
			HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
			if ((object)currentAsset == null || currentAsset.currentPlatformRenderPipelineSettings.supportRayTracing)
			{
				EnableProfilingRecordersRT();
				list.Add(new DebugUI.Foldout("Ray Tracing Stats", BuildProfilingSamplerWidgetList(m_RecordedSamplersRT), new string[3] { "CPU", "CPUInline", "GPU" }));
			}
			list.Add(new DebugUI.BoolField
			{
				displayName = "Count Rays (MRays/Frame)",
				getter = () => data.countRays,
				setter = delegate(bool value)
				{
					data.countRays = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.countRays,
				children = 
				{
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Ambient Occlusion",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.AmbientOcclusion) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Shadows Directional",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.ShadowDirectional) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Shadows Area",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.ShadowAreaLight) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Shadows Point/Spot",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.ShadowPointSpot) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Reflections Forward ",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.ReflectionForward) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Reflections Deferred",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.ReflectionDeferred) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Diffuse GI Forward",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.DiffuseGI_Forward) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Diffuse GI Deferred",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.DiffuseGI_Deferred) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Recursive Rendering",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.Recursive) / 1000000f,
						refreshRate = 1f / 30f
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Total",
						getter = () => (float)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetRaysPerFrame(RayCountValues.Total) / 1000000f,
						refreshRate = 1f / 30f
					}
				}
			});
			m_DebugDisplayStatsItems = list.ToArray();
			DebugUI.Panel panel = DebugManager.instance.GetPanel(k_PanelDisplayStats, createIfNull: true, int.MinValue);
			panel.flags = DebugUI.Flags.RuntimeOnly;
			panel.children.Add(m_DebugDisplayStatsItems);
		}

		private DebugUI.Widget CreateMissingDebugShadersWarning()
		{
			return new DebugUI.MessageBox
			{
				displayName = "Warning: the debug shader variants are missing. Ensure that the \"Runtime Debug Shaders\" option is enabled in HDRP Global Settings.",
				style = DebugUI.MessageBox.Style.Warning,
				isHiddenCallback = () => !(HDRenderPipelineGlobalSettings.instance != null) || HDRenderPipelineGlobalSettings.instance.supportRuntimeDebugDisplay
			};
		}

		private void UnregisterDisplayStatsDebug()
		{
			DisableProfilingRecorders(m_RecordedSamplers);
			HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
			if ((object)currentAsset == null || currentAsset.currentPlatformRenderPipelineSettings.supportRayTracing)
			{
				DisableProfilingRecorders(m_RecordedSamplersRT);
			}
			UnregisterDebugItems(k_PanelDisplayStats, m_DebugDisplayStatsItems);
		}

		private void RegisterMaterialDebug()
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			list.Add(CreateMissingDebugShadersWarning());
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.CommonMaterialProperties,
				getter = () => (int)data.materialDebugSettings.debugViewMaterialCommonValue,
				setter = delegate(int value)
				{
					SetDebugViewCommonMaterialProperty((MaterialSharedProperty)value);
				},
				autoEnum = typeof(MaterialSharedProperty),
				getIndex = () => (int)data.materialDebugSettings.debugViewMaterialCommonValue,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.materialDebugSettings.debugViewMaterialCommonValue = (MaterialSharedProperty)value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.Material,
				getter = () => (data.materialDebugSettings.debugViewMaterial[0] != 0) ? data.materialDebugSettings.debugViewMaterial[1] : 0,
				setter = delegate(int value)
				{
					SetDebugViewMaterial(value);
				},
				enumNames = MaterialDebugSettings.debugViewMaterialStrings,
				enumValues = MaterialDebugSettings.debugViewMaterialValues,
				getIndex = () => data.materialDebugSettings.materialEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.materialDebugSettings.materialEnumIndex = value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.Engine,
				getter = () => data.materialDebugSettings.debugViewEngine,
				setter = delegate(int value)
				{
					SetDebugViewEngine(value);
				},
				enumNames = MaterialDebugSettings.debugViewEngineStrings,
				enumValues = MaterialDebugSettings.debugViewEngineValues,
				getIndex = () => data.engineEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.engineEnumIndex = value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.Attributes,
				getter = () => (int)data.materialDebugSettings.debugViewVarying,
				setter = delegate(int value)
				{
					SetDebugViewVarying((DebugViewVarying)value);
				},
				autoEnum = typeof(DebugViewVarying),
				getIndex = () => data.attributesEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.attributesEnumIndex = value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.Properties,
				getter = () => (int)data.materialDebugSettings.debugViewProperties,
				setter = delegate(int value)
				{
					SetDebugViewProperties((DebugViewProperties)value);
				},
				autoEnum = typeof(DebugViewProperties),
				getIndex = () => data.propertiesEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.propertiesEnumIndex = value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.GBuffer,
				getter = () => data.materialDebugSettings.debugViewGBuffer,
				setter = delegate(int value)
				{
					SetDebugViewGBuffer(value);
				},
				enumNames = MaterialDebugSettings.debugViewMaterialGBufferStrings,
				enumValues = MaterialDebugSettings.debugViewMaterialGBufferValues,
				getIndex = () => data.gBufferEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.gBufferEnumIndex = value;
				}
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = MaterialStrings.MaterialValidator,
				getter = () => (int)data.fullScreenDebugMode,
				setter = delegate(int value)
				{
					SetFullScreenDebugMode((FullScreenDebugMode)value);
				},
				enumNames = s_MaterialFullScreenDebugStrings,
				enumValues = s_MaterialFullScreenDebugValues,
				getIndex = () => data.materialValidatorDebugModeEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.materialValidatorDebugModeEnumIndex = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.ValidateDiffuseColor && data.fullScreenDebugMode != FullScreenDebugMode.ValidateSpecularColor,
				children = 
				{
					(DebugUI.Widget)new DebugUI.ColorField
					{
						nameAndTooltip = MaterialStrings.ValidatorTooHighColor,
						getter = () => data.materialDebugSettings.materialValidateHighColor,
						setter = delegate(Color value)
						{
							data.materialDebugSettings.materialValidateHighColor = value;
						},
						showAlpha = false,
						hdr = true
					},
					(DebugUI.Widget)new DebugUI.ColorField
					{
						nameAndTooltip = MaterialStrings.ValidatorTooLowColor,
						getter = () => data.materialDebugSettings.materialValidateLowColor,
						setter = delegate(Color value)
						{
							data.materialDebugSettings.materialValidateLowColor = value;
						},
						showAlpha = false,
						hdr = true
					},
					(DebugUI.Widget)new DebugUI.ColorField
					{
						nameAndTooltip = MaterialStrings.ValidatorNotAPureMetalColor,
						getter = () => data.materialDebugSettings.materialValidateTrueMetalColor,
						setter = delegate(Color value)
						{
							data.materialDebugSettings.materialValidateTrueMetalColor = value;
						},
						showAlpha = false,
						hdr = true
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = MaterialStrings.ValidatorPureMetals,
						getter = () => data.materialDebugSettings.materialValidateTrueMetal,
						setter = delegate(bool v)
						{
							data.materialDebugSettings.materialValidateTrueMetal = v;
						}
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !ShaderConfig.s_GlobalMipBias,
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = MaterialStrings.OverrideGlobalMaterialTextureMipBias,
						getter = () => data.UseDebugGlobalMipBiasOverride(),
						setter = delegate(bool value)
						{
							data.SetUseDebugGlobalMipBiasOverride(value);
						}
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = MaterialStrings.DebugGlobalMaterialTextureMipBiasValue,
						getter = () => data.GetDebugGlobalMipBiasOverride(),
						setter = delegate(float value)
						{
							data.SetDebugGlobalMipBiasOverride(value);
						},
						isHiddenCallback = () => !data.UseDebugGlobalMipBiasOverride()
					}
				}
			});
			m_DebugMaterialItems = list.ToArray();
			DebugManager.instance.GetPanel(k_PanelMaterials, createIfNull: true).children.Add(m_DebugMaterialItems);
		}

		private void RefreshDisplayStatsDebug<T>(DebugUI.Field<T> field, T value)
		{
			UnregisterDisplayStatsDebug();
			RegisterDisplayStatsDebug();
		}

		private void RefreshLightingDebug<T>(DebugUI.Field<T> field, T value)
		{
			UnregisterDebugItems(k_PanelLighting, m_DebugLightingItems);
			RegisterLightingDebug();
		}

		private void RefreshDecalsDebug<T>(DebugUI.Field<T> field, T value)
		{
			UnregisterDebugItems(k_PanelDecals, m_DebugDecalsItems);
			RegisterDecalsDebug();
		}

		private void RefreshRenderingDebug<T>(DebugUI.Field<T> field, T value)
		{
			UnregisterRenderingDebug();
			RegisterRenderingDebug();
		}

		private void RefreshMaterialDebug<T>(DebugUI.Field<T> field, T value)
		{
			UnregisterDebugItems(k_PanelMaterials, m_DebugMaterialItems);
			RegisterMaterialDebug();
		}

		private void RegisterLightingDebug()
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			list.Add(CreateMissingDebugShadersWarning());
			DebugUI.Container container = new DebugUI.Container
			{
				displayName = "Shadows"
			};
			container.children.Add(new DebugUI.EnumField
			{
				nameAndTooltip = LightingStrings.ShadowDebugMode,
				getter = () => (int)data.lightingDebugSettings.shadowDebugMode,
				setter = delegate(int value)
				{
					SetShadowDebugMode((ShadowMapDebugMode)value);
				},
				autoEnum = typeof(ShadowMapDebugMode),
				getIndex = () => data.shadowDebugModeEnumIndex,
				setIndex = delegate(int value)
				{
					data.shadowDebugModeEnumIndex = value;
				}
			});
			container.children.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.lightingDebugSettings.shadowDebugMode != ShadowMapDebugMode.VisualizeShadowMap && data.lightingDebugSettings.shadowDebugMode != ShadowMapDebugMode.SingleShadow,
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ShadowDebugUseSelection,
						getter = () => data.lightingDebugSettings.shadowDebugUseSelection,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.shadowDebugUseSelection = value;
						},
						flags = DebugUI.Flags.EditorOnly
					},
					(DebugUI.Widget)new DebugUI.UIntField
					{
						nameAndTooltip = LightingStrings.ShadowDebugShadowMapIndex,
						getter = () => data.lightingDebugSettings.shadowMapIndex,
						setter = delegate(uint value)
						{
							data.lightingDebugSettings.shadowMapIndex = value;
						},
						min = () => 0u,
						max = () => (uint)Math.Max(0L, (long)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetCurrentShadowCount() - 1L),
						isHiddenCallback = () => data.lightingDebugSettings.shadowDebugUseSelection
					}
				}
			});
			container.children.Add(new DebugUI.FloatField
			{
				nameAndTooltip = LightingStrings.GlobalShadowScaleFactor,
				getter = () => data.lightingDebugSettings.shadowResolutionScaleFactor,
				setter = delegate(float v)
				{
					data.lightingDebugSettings.shadowResolutionScaleFactor = v;
				},
				min = () => 0.01f,
				max = () => 4f
			});
			container.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.ClearShadowAtlas,
				getter = () => data.lightingDebugSettings.clearShadowAtlas,
				setter = delegate(bool v)
				{
					data.lightingDebugSettings.clearShadowAtlas = v;
				}
			});
			container.children.Add(new DebugUI.FloatField
			{
				nameAndTooltip = LightingStrings.ShadowRangeMinimumValue,
				getter = () => data.lightingDebugSettings.shadowMinValue,
				setter = delegate(float value)
				{
					data.lightingDebugSettings.shadowMinValue = value;
				}
			});
			container.children.Add(new DebugUI.FloatField
			{
				nameAndTooltip = LightingStrings.ShadowRangeMaximumValue,
				getter = () => data.lightingDebugSettings.shadowMaxValue,
				setter = delegate(float value)
				{
					data.lightingDebugSettings.shadowMaxValue = value;
				}
			});
			list.Add(container);
			DebugUI.Container container2 = new DebugUI.Container
			{
				displayName = "Lighting"
			};
			container2.children.Add(new DebugUI.Foldout
			{
				nameAndTooltip = LightingStrings.ShowLightsByType,
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.DirectionalLights,
						getter = () => data.lightingDebugSettings.showDirectionalLight,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.showDirectionalLight = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.PunctualLights,
						getter = () => data.lightingDebugSettings.showPunctualLight,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.showPunctualLight = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.AreaLights,
						getter = () => data.lightingDebugSettings.showAreaLight,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.showAreaLight = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ReflectionProbes,
						getter = () => data.lightingDebugSettings.showReflectionProbe,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.showReflectionProbe = value;
						}
					}
				}
			});
			DebugUI.Foldout item = new DebugUI.Foldout
			{
				nameAndTooltip = LightingStrings.Exposure,
				children = 
				{
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.ExposureDebugMode,
						getter = () => (int)data.lightingDebugSettings.exposureDebugMode,
						setter = delegate(int value)
						{
							SetExposureDebugMode((ExposureDebugMode)value);
						},
						autoEnum = typeof(ExposureDebugMode),
						getIndex = () => data.exposureDebugModeEnumIndex,
						setIndex = delegate(int value)
						{
							data.exposureDebugModeEnumIndex = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ExposureDisplayMaskOnly,
						getter = () => data.lightingDebugSettings.displayMaskOnly,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.displayMaskOnly = value;
						},
						isHiddenCallback = () => data.lightingDebugSettings.exposureDebugMode != ExposureDebugMode.MeteringWeighted
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => data.lightingDebugSettings.exposureDebugMode != ExposureDebugMode.HistogramView,
						children = 
						{
							(DebugUI.Widget)new DebugUI.BoolField
							{
								nameAndTooltip = LightingStrings.DisplayHistogramSceneOverlay,
								getter = () => data.lightingDebugSettings.displayOnSceneOverlay,
								setter = delegate(bool value)
								{
									data.lightingDebugSettings.displayOnSceneOverlay = value;
								}
							},
							(DebugUI.Widget)new DebugUI.BoolField
							{
								nameAndTooltip = LightingStrings.ExposureShowTonemapCurve,
								getter = () => data.lightingDebugSettings.showTonemapCurveAlongHistogramView,
								setter = delegate(bool value)
								{
									data.lightingDebugSettings.showTonemapCurveAlongHistogramView = value;
								}
							},
							(DebugUI.Widget)new DebugUI.BoolField
							{
								nameAndTooltip = LightingStrings.ExposureCenterAroundExposure,
								getter = () => data.lightingDebugSettings.centerHistogramAroundMiddleGrey,
								setter = delegate(bool value)
								{
									data.lightingDebugSettings.centerHistogramAroundMiddleGrey = value;
								}
							}
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ExposureDisplayRGBHistogram,
						getter = () => data.lightingDebugSettings.displayFinalImageHistogramAsRGB,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.displayFinalImageHistogramAsRGB = value;
						},
						isHiddenCallback = () => data.lightingDebugSettings.exposureDebugMode != ExposureDebugMode.FinalImageHistogramView
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = LightingStrings.DebugExposureCompensation,
						getter = () => data.lightingDebugSettings.debugExposure,
						setter = delegate(float value)
						{
							data.lightingDebugSettings.debugExposure = value;
						}
					}
				}
			};
			container2.children.Add(item);
			DebugUI.Foldout item2 = new DebugUI.Foldout
			{
				nameAndTooltip = LightingStrings.HDROutput,
				children = 
				{
					(DebugUI.Widget)new DebugUI.MessageBox
					{
						displayName = "No HDR monitor detected.",
						style = DebugUI.MessageBox.Style.Warning,
						isHiddenCallback = () => HDRenderPipeline.HDROutputIsActive()
					},
					(DebugUI.Widget)new DebugUI.MessageBox
					{
						displayName = "To display the Gamut View, Gamut Clip, Paper White modes without affecting them, the overlay will be hidden.",
						style = DebugUI.MessageBox.Style.Info,
						isHiddenCallback = () => !HDRenderPipeline.HDROutputIsActive()
					},
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.HDROutputDebugMode,
						getter = () => (int)data.lightingDebugSettings.hdrDebugMode,
						setter = delegate(int value)
						{
							SetHDRDebugMode((HDRDebugMode)value);
						},
						autoEnum = typeof(HDRDebugMode),
						getIndex = () => data.hdrDebugModeEnumIndex,
						setIndex = delegate(int value)
						{
							data.hdrDebugModeEnumIndex = value;
						}
					}
				}
			};
			container2.children.Add(item2);
			container2.children.Add(new DebugUI.EnumField
			{
				nameAndTooltip = LightingStrings.LightingDebugMode,
				getter = () => (int)data.lightingDebugSettings.debugLightingMode,
				setter = delegate(int value)
				{
					SetDebugLightingMode((DebugLightingMode)value);
				},
				autoEnum = typeof(DebugLightingMode),
				getIndex = () => data.lightingDebugModeEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.lightingDebugModeEnumIndex = value;
				}
			});
			container2.children.Add(new DebugUI.BitField
			{
				nameAndTooltip = LightingStrings.LightHierarchyDebugMode,
				getter = () => data.lightingDebugSettings.debugLightFilterMode,
				setter = delegate(Enum value)
				{
					SetDebugLightFilterMode((DebugLightFilterMode)(object)value);
				},
				enumType = typeof(DebugLightFilterMode)
			});
			container2.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.LightLayersVisualization,
				getter = () => data.lightingDebugSettings.debugLightLayers,
				setter = delegate(bool value)
				{
					SetDebugLightLayersMode(value);
				}
			});
			DebugUI.Container container3 = new DebugUI.Container
			{
				isHiddenCallback = () => !data.lightingDebugSettings.debugLightLayers,
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.LightLayersUseSelectedLight,
						getter = () => data.lightingDebugSettings.debugSelectionLightLayers,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.debugSelectionLightLayers = value;
						},
						flags = DebugUI.Flags.EditorOnly
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.LightLayersSwitchToLightShadowLayers,
						getter = () => data.lightingDebugSettings.debugSelectionShadowLayers,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.debugSelectionShadowLayers = value;
						},
						flags = DebugUI.Flags.EditorOnly,
						isHiddenCallback = () => !data.lightingDebugSettings.debugSelectionLightLayers
					}
				}
			};
			DebugUI.BitField bitField = new DebugUI.BitField
			{
				nameAndTooltip = LightingStrings.LightLayersFilterLayers,
				getter = () => data.lightingDebugSettings.debugLightLayersFilterMask,
				setter = delegate(Enum value)
				{
					data.lightingDebugSettings.debugLightLayersFilterMask = (DebugLightLayersMask)(object)value;
				},
				enumType = typeof(DebugLightLayersMask),
				isHiddenCallback = () => data.lightingDebugSettings.debugSelectionLightLayers
			};
			for (int i = 0; i < 8; i++)
			{
				bitField.enumNames[i + 1].text = HDRenderPipelineGlobalSettings.instance.prefixedRenderingLayerMaskNames[i];
			}
			container3.children.Add(bitField);
			DebugUI.Foldout foldout = new DebugUI.Foldout
			{
				nameAndTooltip = LightingStrings.LightLayersColor,
				flags = DebugUI.Flags.EditorOnly
			};
			for (int j = 0; j < 8; j++)
			{
				int index = j;
				foldout.children.Add(new DebugUI.ColorField
				{
					displayName = HDRenderPipelineGlobalSettings.instance.prefixedRenderingLayerMaskNames[j],
					flags = DebugUI.Flags.EditorOnly,
					getter = () => data.lightingDebugSettings.debugRenderingLayersColors[index],
					setter = delegate(Color value)
					{
						data.lightingDebugSettings.debugRenderingLayersColors[index] = value;
					}
				});
			}
			container3.children.Add(foldout);
			container2.children.Add(container3);
			list.Add(container2);
			DebugUI.Container item3 = new DebugUI.Container
			{
				displayName = "Material Overrides",
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideSmoothness,
						getter = () => data.lightingDebugSettings.overrideSmoothness,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideSmoothness = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.lightingDebugSettings.overrideSmoothness,
						children = { (DebugUI.Widget)new DebugUI.FloatField
						{
							nameAndTooltip = LightingStrings.Smoothness,
							getter = () => data.lightingDebugSettings.overrideSmoothnessValue,
							setter = delegate(float value)
							{
								data.lightingDebugSettings.overrideSmoothnessValue = value;
							},
							min = () => 0f,
							max = () => 1f,
							incStep = 0.025f
						} }
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideAlbedo,
						getter = () => data.lightingDebugSettings.overrideAlbedo,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideAlbedo = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.lightingDebugSettings.overrideAlbedo,
						children = { (DebugUI.Widget)new DebugUI.ColorField
						{
							nameAndTooltip = LightingStrings.Albedo,
							getter = () => data.lightingDebugSettings.overrideAlbedoValue,
							setter = delegate(Color value)
							{
								data.lightingDebugSettings.overrideAlbedoValue = value;
							},
							showAlpha = false,
							hdr = false
						} }
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideNormal,
						getter = () => data.lightingDebugSettings.overrideNormal,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideNormal = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideSpecularColor,
						getter = () => data.lightingDebugSettings.overrideSpecularColor,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideSpecularColor = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.lightingDebugSettings.overrideSpecularColor,
						children = { (DebugUI.Widget)new DebugUI.ColorField
						{
							nameAndTooltip = LightingStrings.SpecularColor,
							getter = () => data.lightingDebugSettings.overrideSpecularColorValue,
							setter = delegate(Color value)
							{
								data.lightingDebugSettings.overrideSpecularColorValue = value;
							},
							showAlpha = false,
							hdr = false
						} }
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideAmbientOcclusion,
						getter = () => data.lightingDebugSettings.overrideAmbientOcclusion,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideAmbientOcclusion = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.lightingDebugSettings.overrideAmbientOcclusion,
						children = { (DebugUI.Widget)new DebugUI.FloatField
						{
							nameAndTooltip = LightingStrings.AmbientOcclusion,
							getter = () => data.lightingDebugSettings.overrideAmbientOcclusionValue,
							setter = delegate(float value)
							{
								data.lightingDebugSettings.overrideAmbientOcclusionValue = value;
							},
							min = () => 0f,
							max = () => 1f,
							incStep = 0.025f
						} }
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.OverrideEmissiveColor,
						getter = () => data.lightingDebugSettings.overrideEmissiveColor,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.overrideEmissiveColor = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.lightingDebugSettings.overrideEmissiveColor,
						children = { (DebugUI.Widget)new DebugUI.ColorField
						{
							nameAndTooltip = LightingStrings.EmissiveColor,
							getter = () => data.lightingDebugSettings.overrideEmissiveColorValue,
							setter = delegate(Color value)
							{
								data.lightingDebugSettings.overrideEmissiveColorValue = value;
							},
							showAlpha = false,
							hdr = true
						} }
					}
				}
			};
			list.Add(item3);
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = LightingStrings.FullscreenDebugMode,
				getter = () => (int)data.fullScreenDebugMode,
				setter = delegate(int value)
				{
					SetFullScreenDebugMode((FullScreenDebugMode)value);
				},
				enumNames = s_LightingFullScreenDebugStrings,
				enumValues = s_LightingFullScreenDebugValues,
				getIndex = () => data.lightingFulscreenDebugModeEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.lightingFulscreenDebugModeEnumIndex = value;
				},
				onValueChanged = delegate
				{
					FullScreenDebugMode fullScreenDebugMode = data.fullScreenDebugMode;
					if (fullScreenDebugMode != FullScreenDebugMode.ContactShadows && (uint)(fullScreenDebugMode - 11) > 2u)
					{
						data.fullscreenDebugMip = 0f;
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				children = { (DebugUI.Widget)new DebugUI.UIntField
				{
					nameAndTooltip = LightingStrings.ScreenSpaceShadowIndex,
					getter = () => data.screenSpaceShadowIndex,
					setter = delegate(uint value)
					{
						data.screenSpaceShadowIndex = value;
					},
					min = () => 0u,
					max = () => (uint)((RenderPipelineManager.currentPipeline as HDRenderPipeline).GetMaxScreenSpaceShadows() - 1),
					isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceShadows
				} }
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.RayTracingAccelerationStructure,
				children = 
				{
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.RTASDebugView,
						getter = () => (int)data.rtasDebugView,
						setter = delegate(int value)
						{
							SetRTASDebugView((RTASDebugView)value);
						},
						autoEnum = typeof(RTASDebugView),
						getIndex = () => data.rtasDebugViewEnumIndex,
						setIndex = delegate(int value)
						{
							data.rtasDebugViewEnumIndex = value;
						}
					},
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.RTASDebugMode,
						getter = () => (int)data.rtasDebugMode,
						setter = delegate(int value)
						{
							SetRTASDebugMode((RTASDebugMode)value);
						},
						autoEnum = typeof(RTASDebugMode),
						getIndex = () => data.rtasDebugModeEnumIndex,
						setIndex = delegate(int value)
						{
							data.rtasDebugModeEnumIndex = value;
						}
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.PreRefractionColorPyramid && data.fullScreenDebugMode != FullScreenDebugMode.FinalColorPyramid && data.fullScreenDebugMode != FullScreenDebugMode.DepthPyramid,
				children = 
				{
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = LightingStrings.DepthPyramidDebugMip,
						getter = () => data.fullscreenDebugMip,
						setter = delegate(float value)
						{
							data.fullscreenDebugMip = value;
						},
						min = () => 0f,
						max = () => 1f,
						incStep = 0.05f
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.DepthPyramidEnableRemap,
						getter = () => data.enableDebugDepthRemap,
						setter = delegate(bool value)
						{
							data.enableDebugDepthRemap = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => !data.enableDebugDepthRemap,
						children = 
						{
							(DebugUI.Widget)new DebugUI.FloatField
							{
								nameAndTooltip = LightingStrings.DepthPyramidRangeMin,
								getter = () => data.fullScreenDebugDepthRemap.x,
								setter = delegate(float value)
								{
									data.fullScreenDebugDepthRemap.x = Mathf.Min(value, data.fullScreenDebugDepthRemap.y);
								},
								min = () => 0f,
								max = () => 1f,
								incStep = 0.01f
							},
							(DebugUI.Widget)new DebugUI.FloatField
							{
								nameAndTooltip = LightingStrings.DepthPyramidRangeMax,
								getter = () => data.fullScreenDebugDepthRemap.y,
								setter = delegate(float value)
								{
									data.fullScreenDebugDepthRemap.y = Mathf.Max(value, data.fullScreenDebugDepthRemap.x);
								},
								min = () => 0.01f,
								max = () => 1f,
								incStep = 0.01f
							}
						}
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.ContactShadows,
				children = { (DebugUI.Widget)new DebugUI.IntField
				{
					nameAndTooltip = LightingStrings.ContactShadowsLightIndex,
					getter = () => data.fullScreenContactShadowLightIndex,
					setter = delegate(int value)
					{
						data.fullScreenContactShadowLightIndex = value;
					},
					min = () => -1,
					max = () => ShaderConfig.FPTLMaxLightCount - 1
				} }
			});
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = LightingStrings.TileClusterDebug,
				getter = () => (int)data.lightingDebugSettings.tileClusterDebug,
				setter = delegate(int value)
				{
					data.lightingDebugSettings.tileClusterDebug = (TileClusterDebug)value;
				},
				autoEnum = typeof(TileClusterDebug),
				getIndex = () => data.tileClusterDebugEnumIndex,
				setIndex = delegate(int value)
				{
					data.tileClusterDebugEnumIndex = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.lightingDebugSettings.tileClusterDebug == TileClusterDebug.None || data.lightingDebugSettings.tileClusterDebug == TileClusterDebug.MaterialFeatureVariants,
				children = 
				{
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.TileClusterDebugByCategory,
						getter = () => (int)data.lightingDebugSettings.tileClusterDebugByCategory,
						setter = delegate(int value)
						{
							data.lightingDebugSettings.tileClusterDebugByCategory = (TileClusterCategoryDebug)value;
						},
						autoEnum = typeof(TileClusterCategoryDebug),
						getIndex = () => data.tileClusterDebugByCategoryEnumIndex,
						setIndex = delegate(int value)
						{
							data.tileClusterDebugByCategoryEnumIndex = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						isHiddenCallback = () => data.lightingDebugSettings.tileClusterDebug != TileClusterDebug.Cluster,
						children = 
						{
							(DebugUI.Widget)new DebugUI.EnumField
							{
								nameAndTooltip = LightingStrings.ClusterDebugMode,
								getter = () => (int)data.lightingDebugSettings.clusterDebugMode,
								setter = delegate(int value)
								{
									data.lightingDebugSettings.clusterDebugMode = (ClusterDebugMode)value;
								},
								autoEnum = typeof(ClusterDebugMode),
								getIndex = () => data.clusterDebugModeEnumIndex,
								setIndex = delegate(int value)
								{
									data.clusterDebugModeEnumIndex = value;
								}
							},
							(DebugUI.Widget)new DebugUI.FloatField
							{
								isHiddenCallback = () => data.lightingDebugSettings.clusterDebugMode != ClusterDebugMode.VisualizeSlice,
								nameAndTooltip = LightingStrings.ClusterDistance,
								getter = () => data.lightingDebugSettings.clusterDebugDistance,
								setter = delegate(float value)
								{
									data.lightingDebugSettings.clusterDebugDistance = value;
								},
								min = () => 0f,
								max = () => 100f,
								incStep = 0.05f
							}
						}
					}
				}
			});
			list.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.DisplaySkyReflection,
				getter = () => data.lightingDebugSettings.displaySkyReflection,
				setter = delegate(bool value)
				{
					data.lightingDebugSettings.displaySkyReflection = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.lightingDebugSettings.displaySkyReflection,
				children = { (DebugUI.Widget)new DebugUI.FloatField
				{
					nameAndTooltip = LightingStrings.SkyReflectionMipmap,
					getter = () => data.lightingDebugSettings.skyReflectionMipmap,
					setter = delegate(float value)
					{
						data.lightingDebugSettings.skyReflectionMipmap = value;
					},
					min = () => 0f,
					max = () => 1f,
					incStep = 0.05f
				} }
			});
			list.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.DisplayLightVolumes,
				getter = () => data.lightingDebugSettings.displayLightVolumes,
				setter = delegate(bool value)
				{
					data.lightingDebugSettings.displayLightVolumes = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.lightingDebugSettings.displayLightVolumes,
				children = 
				{
					(DebugUI.Widget)new DebugUI.EnumField
					{
						nameAndTooltip = LightingStrings.LightVolumeDebugType,
						getter = () => (int)data.lightingDebugSettings.lightVolumeDebugByCategory,
						setter = delegate(int value)
						{
							data.lightingDebugSettings.lightVolumeDebugByCategory = (LightVolumeDebug)value;
						},
						autoEnum = typeof(LightVolumeDebug),
						getIndex = () => data.lightVolumeDebugTypeEnumIndex,
						setIndex = delegate(int value)
						{
							data.lightVolumeDebugTypeEnumIndex = value;
						}
					},
					(DebugUI.Widget)new DebugUI.UIntField
					{
						isHiddenCallback = () => data.lightingDebugSettings.lightVolumeDebugByCategory != LightVolumeDebug.Gradient,
						nameAndTooltip = LightingStrings.MaxDebugLightCount,
						getter = () => data.lightingDebugSettings.maxDebugLightCount,
						setter = delegate(uint value)
						{
							data.lightingDebugSettings.maxDebugLightCount = value;
						},
						min = () => 0u,
						max = () => 24u,
						incStep = 1u
					}
				}
			});
			list.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.DisplayCookieAtlas,
				getter = () => data.lightingDebugSettings.displayCookieAtlas,
				setter = delegate(bool value)
				{
					data.lightingDebugSettings.displayCookieAtlas = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.lightingDebugSettings.displayCookieAtlas,
				children = 
				{
					(DebugUI.Widget)new DebugUI.UIntField
					{
						nameAndTooltip = LightingStrings.CookieAtlasMipLevel,
						getter = () => data.lightingDebugSettings.cookieAtlasMipLevel,
						setter = delegate(uint value)
						{
							data.lightingDebugSettings.cookieAtlasMipLevel = value;
						},
						min = () => 0u,
						max = () => (uint)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetCookieAtlasMipCount()
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ClearCookieAtlas,
						getter = () => data.lightingDebugSettings.clearCookieAtlas,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.clearCookieAtlas = value;
						}
					}
				}
			});
			list.Add(new DebugUI.BoolField
			{
				nameAndTooltip = LightingStrings.DisplayReflectionProbeAtlas,
				getter = () => data.lightingDebugSettings.displayReflectionProbeAtlas,
				setter = delegate(bool value)
				{
					data.lightingDebugSettings.displayReflectionProbeAtlas = value;
				},
				onValueChanged = RefreshLightingDebug
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.lightingDebugSettings.displayReflectionProbeAtlas,
				children = 
				{
					(DebugUI.Widget)new DebugUI.UIntField
					{
						nameAndTooltip = LightingStrings.ReflectionProbeAtlasSlice,
						getter = () => data.lightingDebugSettings.reflectionProbeSlice,
						setter = delegate(uint value)
						{
							data.lightingDebugSettings.reflectionProbeSlice = value;
						},
						min = () => 0u,
						max = () => (uint)((RenderPipelineManager.currentPipeline as HDRenderPipeline).GetReflectionProbeArraySize() - 1),
						isHiddenCallback = () => (RenderPipelineManager.currentPipeline as HDRenderPipeline).GetReflectionProbeArraySize() == 1
					},
					(DebugUI.Widget)new DebugUI.UIntField
					{
						nameAndTooltip = LightingStrings.ReflectionProbeAtlasMipLevel,
						getter = () => data.lightingDebugSettings.reflectionProbeMipLevel,
						setter = delegate(uint value)
						{
							data.lightingDebugSettings.reflectionProbeMipLevel = value;
						},
						min = () => 0u,
						max = () => (uint)(RenderPipelineManager.currentPipeline as HDRenderPipeline).GetReflectionProbeMipCount()
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ClearReflectionProbeAtlas,
						getter = () => data.lightingDebugSettings.clearReflectionProbeAtlas,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.clearReflectionProbeAtlas = value;
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = LightingStrings.ReflectionProbeApplyExposure,
						getter = () => data.lightingDebugSettings.reflectionProbeApplyExposure,
						setter = delegate(bool value)
						{
							data.lightingDebugSettings.reflectionProbeApplyExposure = value;
						}
					}
				}
			});
			list.Add(new DebugUI.FloatField
			{
				nameAndTooltip = LightingStrings.DebugOverlayScreenRatio,
				getter = () => data.debugOverlayRatio,
				setter = delegate(float v)
				{
					data.debugOverlayRatio = v;
				},
				min = () => 0.1f,
				max = () => 1f
			});
			m_DebugLightingItems = list.ToArray();
			DebugManager.instance.GetPanel(k_PanelLighting, createIfNull: true).children.Add(m_DebugLightingItems);
		}

		private void RegisterRenderingDebug()
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			list.Add(CreateMissingDebugShadersWarning());
			list.Add(new DebugUI.EnumField
			{
				nameAndTooltip = RenderingStrings.FullscreenDebugMode,
				getter = () => (int)data.fullScreenDebugMode,
				setter = delegate(int value)
				{
					SetFullScreenDebugMode((FullScreenDebugMode)value);
				},
				enumNames = s_RenderingFullScreenDebugStrings,
				enumValues = s_RenderingFullScreenDebugValues,
				getIndex = () => data.renderingFulscreenDebugModeEnumIndex,
				setIndex = delegate(int value)
				{
					data.ResetExclusiveEnumIndices();
					data.renderingFulscreenDebugModeEnumIndex = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.TransparencyOverdraw,
				children = { (DebugUI.Widget)new DebugUI.FloatField
				{
					nameAndTooltip = RenderingStrings.MaxOverdrawCount,
					getter = () => data.transparencyDebugSettings.maxPixelCost,
					setter = delegate(float value)
					{
						data.transparencyDebugSettings.maxPixelCost = value;
					},
					min = () => 0.25f,
					max = () => 2048f
				} }
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.QuadOverdraw,
				children = { (DebugUI.Widget)new DebugUI.UIntField
				{
					nameAndTooltip = RenderingStrings.MaxQuadCost,
					getter = () => data.maxQuadCost,
					setter = delegate(uint value)
					{
						data.maxQuadCost = value;
					},
					min = () => 1u,
					max = () => 10u
				} }
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.VertexDensity,
				children = { (DebugUI.Widget)new DebugUI.UIntField
				{
					nameAndTooltip = RenderingStrings.MaxVertexDensity,
					getter = () => data.maxVertexDensity,
					setter = delegate(uint value)
					{
						data.maxVertexDensity = value;
					},
					min = () => 1u,
					max = () => 100u
				} }
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode != FullScreenDebugMode.MotionVectors,
				children = { (DebugUI.Widget)new DebugUI.FloatField
				{
					displayName = "Min Motion Vector Length (in pixels)",
					getter = () => data.minMotionVectorLength,
					setter = delegate(float value)
					{
						data.minMotionVectorLength = value;
					},
					min = () => 0f
				} }
			});
			list.AddRange(new DebugUI.Widget[1]
			{
				new DebugUI.EnumField
				{
					nameAndTooltip = RenderingStrings.MipMaps,
					getter = () => (int)data.mipMapDebugSettings.debugMipMapMode,
					setter = delegate(int value)
					{
						SetMipMapMode((DebugMipMapMode)value);
					},
					autoEnum = typeof(DebugMipMapMode),
					getIndex = () => data.mipMapsEnumIndex,
					setIndex = delegate(int value)
					{
						data.ResetExclusiveEnumIndices();
						data.mipMapsEnumIndex = value;
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => data.fullScreenDebugMode == FullScreenDebugMode.None,
				children = { (DebugUI.Widget)new DebugUI.EnumField
				{
					nameAndTooltip = RenderingStrings.TerrainTexture,
					getter = () => (int)data.mipMapDebugSettings.terrainTexture,
					setter = delegate(int value)
					{
						data.mipMapDebugSettings.terrainTexture = (DebugMipMapModeTerrainTexture)value;
					},
					autoEnum = typeof(DebugMipMapModeTerrainTexture),
					getIndex = () => data.terrainTextureEnumIndex,
					setIndex = delegate(int value)
					{
						data.terrainTextureEnumIndex = value;
					}
				} }
			});
			list.AddRange(new DebugUI.Container[1]
			{
				new DebugUI.Container
				{
					displayName = "Color Picker",
					flags = DebugUI.Flags.EditorOnly,
					children = 
					{
						(DebugUI.Widget)new DebugUI.EnumField
						{
							nameAndTooltip = RenderingStrings.ColorPickerDebugMode,
							getter = () => (int)data.colorPickerDebugSettings.colorPickerMode,
							setter = delegate(int value)
							{
								data.colorPickerDebugSettings.colorPickerMode = (ColorPickerDebugMode)value;
							},
							autoEnum = typeof(ColorPickerDebugMode),
							getIndex = () => data.colorPickerDebugModeEnumIndex,
							setIndex = delegate(int value)
							{
								data.colorPickerDebugModeEnumIndex = value;
							}
						},
						(DebugUI.Widget)new DebugUI.ColorField
						{
							nameAndTooltip = RenderingStrings.ColorPickerFontColor,
							flags = DebugUI.Flags.EditorOnly,
							getter = () => data.colorPickerDebugSettings.fontColor,
							setter = delegate(Color value)
							{
								data.colorPickerDebugSettings.fontColor = value;
							}
						}
					}
				}
			});
			list.Add(new DebugUI.BoolField
			{
				nameAndTooltip = RenderingStrings.FalseColorMode,
				getter = () => data.falseColorDebugSettings.falseColor,
				setter = delegate(bool value)
				{
					data.falseColorDebugSettings.falseColor = value;
				}
			});
			list.Add(new DebugUI.Container
			{
				isHiddenCallback = () => !data.falseColorDebugSettings.falseColor,
				flags = DebugUI.Flags.EditorOnly,
				children = 
				{
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = RenderingStrings.FalseColorRangeThreshold0,
						getter = () => data.falseColorDebugSettings.colorThreshold0,
						setter = delegate(float value)
						{
							data.falseColorDebugSettings.colorThreshold0 = Mathf.Min(value, data.falseColorDebugSettings.colorThreshold1);
						}
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = RenderingStrings.FalseColorRangeThreshold1,
						getter = () => data.falseColorDebugSettings.colorThreshold1,
						setter = delegate(float value)
						{
							data.falseColorDebugSettings.colorThreshold1 = Mathf.Clamp(value, data.falseColorDebugSettings.colorThreshold0, data.falseColorDebugSettings.colorThreshold2);
						}
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = RenderingStrings.FalseColorRangeThreshold2,
						getter = () => data.falseColorDebugSettings.colorThreshold2,
						setter = delegate(float value)
						{
							data.falseColorDebugSettings.colorThreshold2 = Mathf.Clamp(value, data.falseColorDebugSettings.colorThreshold1, data.falseColorDebugSettings.colorThreshold3);
						}
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = RenderingStrings.FalseColorRangeThreshold3,
						getter = () => data.falseColorDebugSettings.colorThreshold3,
						setter = delegate(float value)
						{
							data.falseColorDebugSettings.colorThreshold3 = Mathf.Max(value, data.falseColorDebugSettings.colorThreshold2);
						}
					}
				}
			});
			list.AddRange(new DebugUI.Widget[1]
			{
				new DebugUI.EnumField
				{
					nameAndTooltip = RenderingStrings.FreezeCameraForCulling,
					getter = () => data.debugCameraToFreeze,
					setter = delegate(int value)
					{
						data.debugCameraToFreeze = value;
					},
					enumNames = s_CameraNamesStrings,
					enumValues = s_CameraNamesValues,
					getIndex = () => data.debugCameraToFreezeEnumIndex,
					setIndex = delegate(int value)
					{
						data.debugCameraToFreezeEnumIndex = value;
					}
				}
			});
			list.Add(new DebugUI.Container
			{
				displayName = "Color Monitors",
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = RenderingStrings.WaveformToggle,
						getter = () => data.monitorsDebugSettings.waveformToggle,
						setter = delegate(bool value)
						{
							data.monitorsDebugSettings.waveformToggle = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container("WaveformContainer")
					{
						isHiddenCallback = () => !data.monitorsDebugSettings.waveformToggle,
						children = 
						{
							(DebugUI.Widget)new DebugUI.FloatField
							{
								nameAndTooltip = RenderingStrings.WaveformExposure,
								getter = () => data.monitorsDebugSettings.waveformExposure,
								setter = delegate(float value)
								{
									data.monitorsDebugSettings.waveformExposure = value;
								},
								min = () => 0f
							},
							(DebugUI.Widget)new DebugUI.BoolField
							{
								nameAndTooltip = RenderingStrings.WaveformParade,
								getter = () => data.monitorsDebugSettings.waveformParade,
								setter = delegate(bool value)
								{
									data.monitorsDebugSettings.waveformParade = value;
								}
							}
						}
					},
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = RenderingStrings.VectorscopeToggle,
						getter = () => data.monitorsDebugSettings.vectorscopeToggle,
						setter = delegate(bool value)
						{
							data.monitorsDebugSettings.vectorscopeToggle = value;
						}
					},
					(DebugUI.Widget)new DebugUI.Container("VectorscopeContainer")
					{
						isHiddenCallback = () => !data.monitorsDebugSettings.vectorscopeToggle,
						children = { (DebugUI.Widget)new DebugUI.FloatField
						{
							nameAndTooltip = RenderingStrings.VectorscopeExposure,
							getter = () => data.monitorsDebugSettings.vectorscopeExposure,
							setter = delegate(float value)
							{
								data.monitorsDebugSettings.vectorscopeExposure = value;
							},
							min = () => 0f
						} }
					},
					(DebugUI.Widget)new DebugUI.FloatField
					{
						nameAndTooltip = RenderingStrings.MonitorsSize,
						getter = () => data.monitorsDebugSettings.monitorsSize,
						setter = delegate(float value)
						{
							data.monitorsDebugSettings.monitorsSize = value;
						},
						min = () => 0.1f,
						max = () => 0.8f
					}
				}
			});
			list.Add(nvidiaDebugView.CreateWidget());
			m_DebugRenderingItems = list.ToArray();
			DebugUI.Panel panel = DebugManager.instance.GetPanel(k_PanelRendering, createIfNull: true);
			panel.children.Add(m_DebugRenderingItems);
			foreach (RenderGraph registeredRenderGraph in RenderGraph.GetRegisteredRenderGraphs())
			{
				registeredRenderGraph.RegisterDebug(panel);
			}
		}

		private void UnregisterRenderingDebug()
		{
			UnregisterDebugItems(k_PanelRendering, m_DebugRenderingItems);
			foreach (RenderGraph registeredRenderGraph in RenderGraph.GetRegisteredRenderGraphs())
			{
				registeredRenderGraph.UnRegisterDebug();
			}
		}

		private void RegisterDecalsDebug()
		{
			DebugUI.Container container = new DebugUI.Container
			{
				displayName = "Decals Affecting Transparent Objects",
				children = 
				{
					(DebugUI.Widget)new DebugUI.BoolField
					{
						nameAndTooltip = DecalStrings.DisplayAtlas,
						getter = () => data.decalsDebugSettings.displayAtlas,
						setter = delegate(bool value)
						{
							data.decalsDebugSettings.displayAtlas = value;
						}
					},
					(DebugUI.Widget)new DebugUI.UIntField
					{
						nameAndTooltip = DecalStrings.MipLevel,
						getter = () => data.decalsDebugSettings.mipLevel,
						setter = delegate(uint value)
						{
							data.decalsDebugSettings.mipLevel = value;
						},
						min = () => 0u,
						max = () => (uint)((RenderPipelineManager.currentPipeline as HDRenderPipeline)?.GetDecalAtlasMipCount()).Value
					}
				}
			};
			m_DebugDecalsItems = new DebugUI.Widget[2]
			{
				CreateMissingDebugShadersWarning(),
				container
			};
			DebugManager.instance.GetPanel(k_PanelDecals, createIfNull: true).children.Add(m_DebugDecalsItems);
		}

		internal void RegisterDebug()
		{
			RegisterDecalsDebug();
			RegisterDisplayStatsDebug();
			RegisterMaterialDebug();
			RegisterLightingDebug();
			RegisterRenderingDebug();
			DebugManager.instance.RegisterData(this);
		}

		internal void UnregisterDebug()
		{
			UnregisterDebugItems(k_PanelDecals, m_DebugDecalsItems);
			UnregisterDisplayStatsDebug();
			UnregisterDebugItems(k_PanelMaterials, m_DebugMaterialItems);
			UnregisterDebugItems(k_PanelLighting, m_DebugLightingItems);
			UnregisterRenderingDebug();
			DebugManager.instance.UnregisterData(this);
		}

		private void UnregisterDebugItems(string panelName, DebugUI.Widget[] items)
		{
			DebugManager.instance.GetPanel(panelName)?.children.Remove(items);
		}

		private void FillFullScreenDebugEnum(ref GUIContent[] strings, ref int[] values, FullScreenDebugMode min, FullScreenDebugMode max)
		{
			int num = max - min - 1;
			strings = new GUIContent[num + 1];
			values = new int[num + 1];
			strings[0] = new GUIContent(FullScreenDebugMode.None.ToString());
			values[0] = 0;
			int num2 = 1;
			for (int i = (int)(min + 1); i < (int)max; i++)
			{
				GUIContent[] obj = strings;
				int num3 = num2;
				FullScreenDebugMode fullScreenDebugMode = (FullScreenDebugMode)i;
				obj[num3] = new GUIContent(fullScreenDebugMode.ToString());
				values[num2] = i;
				num2++;
			}
		}

		internal static void RegisterCamera(IFrameSettingsHistoryContainer container)
		{
			string name = container.panelName;
			if (s_CameraNames.FindIndex((GUIContent x) => x.text.Equals(name)) < 0)
			{
				s_CameraNames.Add(new GUIContent(name));
				needsRefreshingCameraFreezeList = true;
			}
			if (!FrameSettingsHistory.IsRegistered(container))
			{
				IDebugData debugData = FrameSettingsHistory.RegisterDebug(container);
				DebugManager.instance.RegisterData(debugData);
			}
		}

		internal static void UnRegisterCamera(IFrameSettingsHistoryContainer container)
		{
			string name = container.panelName;
			int num = s_CameraNames.FindIndex((GUIContent x) => x.text.Equals(name));
			if (num > 0)
			{
				s_CameraNames.RemoveAt(num);
				needsRefreshingCameraFreezeList = true;
			}
			if (FrameSettingsHistory.IsRegistered(container))
			{
				DebugManager.instance.UnregisterData(container);
				FrameSettingsHistory.UnRegisterDebug(container);
			}
		}

		internal bool IsDebugDisplayRemovePostprocess()
		{
			if (!data.materialDebugSettings.IsDebugDisplayEnabled() && !data.lightingDebugSettings.IsDebugDisplayRemovePostprocess())
			{
				return data.mipMapDebugSettings.IsDebugDisplayEnabled();
			}
			return true;
		}

		internal void UpdateMaterials()
		{
			if (data.mipMapDebugSettings.debugMipMapMode != 0)
			{
				Texture.SetStreamingTextureMaterialDebugProperties();
			}
		}

		internal void UpdateCameraFreezeOptions()
		{
			if (needsRefreshingCameraFreezeList)
			{
				s_CameraNames.Insert(0, new GUIContent("None"));
				s_CameraNamesStrings = s_CameraNames.ToArray();
				s_CameraNamesValues = Enumerable.Range(0, s_CameraNames.Count()).ToArray();
				UnregisterRenderingDebug();
				RegisterRenderingDebug();
				needsRefreshingCameraFreezeList = false;
			}
		}

		internal bool DebugHideSky(HDCamera hdCamera)
		{
			if (!IsMatcapViewEnabled(hdCamera) && GetDebugLightingMode() != DebugLightingMode.DiffuseLighting && GetDebugLightingMode() != DebugLightingMode.SpecularLighting && GetDebugLightingMode() != DebugLightingMode.DirectDiffuseLighting && GetDebugLightingMode() != DebugLightingMode.DirectSpecularLighting && GetDebugLightingMode() != DebugLightingMode.IndirectDiffuseLighting && GetDebugLightingMode() != DebugLightingMode.ReflectionLighting && GetDebugLightingMode() != DebugLightingMode.RefractionLighting)
			{
				return GetDebugLightingMode() == DebugLightingMode.ProbeVolumeSampledSubdivision;
			}
			return true;
		}

		internal bool DebugNeedsExposure()
		{
			DebugLightingMode debugLightingMode = data.lightingDebugSettings.debugLightingMode;
			DebugViewGbuffer debugViewGBuffer = (DebugViewGbuffer)data.materialDebugSettings.debugViewGBuffer;
			if (debugLightingMode != DebugLightingMode.DirectDiffuseLighting && debugLightingMode != DebugLightingMode.DirectSpecularLighting && debugLightingMode != DebugLightingMode.IndirectDiffuseLighting && debugLightingMode != DebugLightingMode.ReflectionLighting && debugLightingMode != DebugLightingMode.RefractionLighting && debugLightingMode != DebugLightingMode.EmissiveLighting && debugLightingMode != DebugLightingMode.DiffuseLighting && debugLightingMode != DebugLightingMode.SpecularLighting && debugLightingMode != DebugLightingMode.VisualizeCascade && debugLightingMode != DebugLightingMode.ProbeVolumeSampledSubdivision && !data.lightingDebugSettings.overrideAlbedo && !data.lightingDebugSettings.overrideNormal && !data.lightingDebugSettings.overrideSmoothness && !data.lightingDebugSettings.overrideSpecularColor && !data.lightingDebugSettings.overrideEmissiveColor && !data.lightingDebugSettings.overrideAmbientOcclusion && debugViewGBuffer != DebugViewGbuffer.BakeDiffuseLightingWithAlbedoPlusEmissive && data.lightingDebugSettings.debugLightFilterMode == DebugLightFilterMode.None && data.fullScreenDebugMode != FullScreenDebugMode.PreRefractionColorPyramid && data.fullScreenDebugMode != FullScreenDebugMode.FinalColorPyramid && data.fullScreenDebugMode != FullScreenDebugMode.VolumetricClouds && data.fullScreenDebugMode != FullScreenDebugMode.TransparentScreenSpaceReflections && data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceReflections && data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceReflectionsPrev && data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceReflectionsAccum && data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceReflectionSpeedRejection && data.fullScreenDebugMode != FullScreenDebugMode.LightCluster && data.fullScreenDebugMode != FullScreenDebugMode.ScreenSpaceShadows && data.fullScreenDebugMode != FullScreenDebugMode.NanTracker && data.fullScreenDebugMode != FullScreenDebugMode.ColorLog)
			{
				return data.fullScreenDebugMode == FullScreenDebugMode.ScreenSpaceGlobalIllumination;
			}
			return true;
		}
	}
}
