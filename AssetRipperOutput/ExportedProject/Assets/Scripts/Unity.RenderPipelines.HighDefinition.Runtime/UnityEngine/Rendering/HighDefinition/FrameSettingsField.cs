using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public enum FrameSettingsField
	{
		None = -1,
		[FrameSettingsField(0, FrameSettingsField.LitShaderMode, null, "Specifies the Lit Shader Mode for Cameras using these Frame Settings use to render the Scene (Depends on \"Lit Shader Mode\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsEnumPopup, typeof(LitShaderMode), null, null, 0)]
		LitShaderMode = 0,
		[FrameSettingsField(0, FrameSettingsField.None, "Depth Prepass within Deferred", "When enabled, HDRP processes a depth prepass for Cameras using these Frame Settings. Set Lit Shader Mode to Deferred to access this option.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.LitShaderMode }, null, -1)]
		DepthPrepassWithDeferredRendering = 1,
		[FrameSettingsField(0, FrameSettingsField.None, "Clear GBuffers", "When enabled, HDRP clear GBuffers for Cameras using these Frame Settings. Set Lit Shader Mode to Deferred to access this option.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.LitShaderMode }, null, 2)]
		ClearGBuffers = 5,
		[Obsolete]
		MSAA = 31,
		[FrameSettingsField(0, FrameSettingsField.None, "MSAA Within Forward", "Specifies the MSAA mode for Cameras using these Frame Settings. Set Lit Shader Mode to Forward to access this option. Note that MSAA is disabled when using ray tracing.", FrameSettingsFieldAttribute.DisplayType.Others, typeof(MSAAMode), null, null, 3)]
		MSAAMode = 4,
		[Obsolete]
		[FrameSettingsField(0, FrameSettingsField.None, "Alpha To Mask", "When enabled, Cameras using these Frame Settings use Alpha To Mask. Activate MSAA to access this option.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 3)]
		AlphaToMask = 56,
		[FrameSettingsField(0, FrameSettingsField.OpaqueObjects, null, "When enabled, Cameras using these Frame Settings render opaque GameObjects.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 4)]
		OpaqueObjects = 2,
		[FrameSettingsField(0, FrameSettingsField.TransparentObjects, null, "When enabled, Cameras using these Frame Settings render Transparent GameObjects.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 5)]
		TransparentObjects = 3,
		[FrameSettingsField(0, FrameSettingsField.Decals, null, "When enabled, HDRP processes a decal render pass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 6)]
		Decals = 12,
		[FrameSettingsField(0, FrameSettingsField.DecalLayers, null, "When enabled, Cameras that use these Frame Settings make use of DecalLayers (Depends on \"Decal Layers\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Decals }, null, 6)]
		DecalLayers = 96,
		[FrameSettingsField(0, FrameSettingsField.TransparentPrepass, null, "When enabled, HDRP processes a transparent prepass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 7)]
		TransparentPrepass = 8,
		[FrameSettingsField(0, FrameSettingsField.TransparentPostpass, null, "When enabled, HDRP processes a transparent postpass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 8)]
		TransparentPostpass = 9,
		[FrameSettingsField(0, FrameSettingsField.None, "Low Resolution Transparent", "When enabled, HDRP processes a transparent pass in a lower resolution for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 9)]
		LowResTransparent = 18,
		[FrameSettingsField(0, FrameSettingsField.None, "Ray Tracing", "When enabled, HDRP updates ray tracing for Cameras using these Frame Settings (Depends on \"Realtime RayTracing\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 10)]
		RayTracing = 92,
		[FrameSettingsField(0, FrameSettingsField.CustomPass, null, "When enabled, HDRP renders custom passes contained in CustomPassVolume components.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 11)]
		CustomPass = 6,
		[FrameSettingsField(0, FrameSettingsField.VirtualTexturing, null, "Virtual Texturing needs to be enabled first in Project Settings > Player > Other Settings > Virtual Texturing.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 105)]
		VirtualTexturing = 68,
		[FrameSettingsField(0, FrameSettingsField.Water, null, "When enabled, Cameras using these Frame Settings render water surfaces.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 106)]
		Water = 99,
		[FrameSettingsField(0, FrameSettingsField.None, "Asymmetric Projection", "When enabled HDRP will account for asymmetric projection when evaluating the view direction based on pixel coordinates.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 107)]
		AsymmetricProjection = 78,
		[FrameSettingsField(0, FrameSettingsField.None, "Screen Coordinates Override", "When enabled HDRP will use Screen Coordinates Override for post processing and custom passes. This allows post effects to be compatible with Cluster Display for example.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 108)]
		ScreenCoordOverride = 77,
		[FrameSettingsField(0, FrameSettingsField.MotionVectors, null, "When enabled, HDRP processes a motion vector pass for Cameras using these Frame Settings (Depends on \"Motion Vectors\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 12)]
		MotionVectors = 10,
		[FrameSettingsField(0, FrameSettingsField.None, "Opaque Object Motion", "When enabled, HDRP processes an object motion vector pass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.MotionVectors }, null, 13)]
		ObjectMotionVectors = 11,
		[FrameSettingsField(0, FrameSettingsField.None, "Transparent Object Motion", "When enabled, transparent GameObjects use Motion Vectors. You must also enable TransparentWritesVelocity for Materials that you want to use motion vectors with.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.MotionVectors }, null, 14)]
		TransparentsWriteMotionVector = 16,
		[FrameSettingsField(0, FrameSettingsField.Refraction, null, "When enabled, HDRP processes a refraction render pass for Cameras using these Frame Settings. This add a resolve of ColorBuffer after the drawing of opaque materials to be use for Refraction effect during transparent pass.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 15)]
		Refraction = 13,
		[Obsolete]
		RoughRefraction = 13,
		[FrameSettingsField(0, FrameSettingsField.Distortion, null, "When enabled, HDRP processes a distortion render pass for Cameras using these Frame Settings (Depends on \"Distortion\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 16)]
		Distortion = 14,
		[FrameSettingsField(0, FrameSettingsField.RoughDistortion, null, "When enabled, HDRP processes a distortion render pass for Cameras using these Frame Settings (Depends on \"Distortion\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Distortion }, null, 17)]
		RoughDistortion = 67,
		[FrameSettingsField(0, FrameSettingsField.None, "Post-process", "When enabled, HDRP processes a post-processing render pass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 18)]
		Postprocess = 15,
		[FrameSettingsField(0, FrameSettingsField.None, "Custom Post-process", "When enabled on a Camera, HDRP renders user-written post processes.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		CustomPostProcess = 39,
		[FrameSettingsField(0, FrameSettingsField.None, "Stop NaN", "When enabled, HDRP replace NaN values with black pixels for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		StopNaN = 80,
		[FrameSettingsField(0, FrameSettingsField.DepthOfField, null, "When enabled, HDRP adds depth of field to Cameras affected by a Volume containing the Depth Of Field override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		DepthOfField = 81,
		[FrameSettingsField(0, FrameSettingsField.MotionBlur, null, "When enabled, HDRP adds motion blur to Cameras affected by a Volume containing the Blur override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		MotionBlur = 82,
		[FrameSettingsField(0, FrameSettingsField.PaniniProjection, null, "When enabled, HDRP adds panini projection to Cameras affected by a Volume containing the Panini Projection override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		PaniniProjection = 83,
		[FrameSettingsField(0, FrameSettingsField.Bloom, null, "When enabled, HDRP adds bloom to Cameras affected by a Volume containing the Bloom override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		Bloom = 84,
		[FrameSettingsField(0, FrameSettingsField.LensFlareDataDriven, null, "When enabled, HDRP adds lens flare to Cameras.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		LensFlareDataDriven = 97,
		[FrameSettingsField(0, FrameSettingsField.LensDistortion, null, "When enabled, HDRP adds lens distortion to Cameras affected by a Volume containing the Lens Distortion override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		LensDistortion = 85,
		[FrameSettingsField(0, FrameSettingsField.ChromaticAberration, null, "When enabled, HDRP adds chromatic aberration to Cameras affected by a Volume containing the Chromatic Aberration override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		ChromaticAberration = 86,
		[FrameSettingsField(0, FrameSettingsField.Vignette, null, "When enabled, HDRP adds vignette to Cameras affected by a Volume containing the Vignette override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		Vignette = 87,
		[FrameSettingsField(0, FrameSettingsField.ColorGrading, null, "When enabled, HDRP processes color grading for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		ColorGrading = 88,
		[FrameSettingsField(0, FrameSettingsField.Tonemapping, null, "When enabled, HDRP processes tonemapping for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		Tonemapping = 93,
		[FrameSettingsField(0, FrameSettingsField.FilmGrain, null, "When enabled, HDRP adds film grain to Cameras affected by a Volume containing the Film Grain override.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		FilmGrain = 89,
		[FrameSettingsField(0, FrameSettingsField.Dithering, null, "When enabled, HDRP processes dithering for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		Dithering = 90,
		[FrameSettingsField(0, FrameSettingsField.None, "Anti-aliasing", "When enabled, HDRP processes anti-aliasing for camera using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.Postprocess }, null, 19)]
		Antialiasing = 91,
		[FrameSettingsField(0, FrameSettingsField.None, "After Post-process", "When enabled, HDRP processes a post-processing render pass for Cameras using these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 20)]
		AfterPostprocess = 17,
		[FrameSettingsField(0, FrameSettingsField.None, "Depth Test", "When enabled, Cameras that don't use TAA process a depth test for Materials in the AfterPostProcess rendering pass.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AfterPostprocess }, null, 20)]
		ZTestAfterPostProcessTAA = 19,
		[FrameSettingsField(0, FrameSettingsField.LODBiasMode, null, "Specifies the Level Of Detail Mode for Cameras using these Frame Settings use to render the Scene. Scale will allow to add a scale factor while Override will allow to set a specific value.", FrameSettingsFieldAttribute.DisplayType.Others, typeof(LODBiasMode), null, null, 100)]
		LODBiasMode = 60,
		[FrameSettingsField(0, FrameSettingsField.LODBias, null, "Sets the Level Of Detail Bias or the Scale on it.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.LODBiasMode }, null, -1)]
		LODBias = 61,
		[FrameSettingsField(0, FrameSettingsField.None, "Quality Level", "The quality level to use when fetching the value from the quality settings.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.LODBiasMode }, null, 100)]
		LODBiasQualityLevel = 64,
		[FrameSettingsField(0, FrameSettingsField.MaximumLODLevelMode, null, "Specifies the Maximum Level Of Detail Mode for Cameras using these Frame Settings to use to render the Scene. Offset allows you to add an offset factor while Override allows you to set a specific value.", FrameSettingsFieldAttribute.DisplayType.Others, typeof(MaximumLODLevelMode), null, null, -1)]
		MaximumLODLevelMode = 62,
		[FrameSettingsField(0, FrameSettingsField.MaximumLODLevel, null, "Sets the Maximum Level Of Detail Level or the Offset on it.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.MaximumLODLevelMode }, null, -1)]
		MaximumLODLevel = 63,
		[FrameSettingsField(0, FrameSettingsField.None, "Quality Level", "The quality level to use when fetching the value from the quality settings.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.MaximumLODLevelMode }, null, 102)]
		MaximumLODLevelQualityLevel = 65,
		[FrameSettingsField(0, FrameSettingsField.MaterialQualityLevel, null, "The material quality level to use.", FrameSettingsFieldAttribute.DisplayType.Others, null, null, null, -1)]
		MaterialQualityLevel = 66,
		[FrameSettingsField(1, FrameSettingsField.ShadowMaps, null, "When enabled, Cameras using these Frame Settings render shadows.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 1)]
		ShadowMaps = 20,
		[FrameSettingsField(1, FrameSettingsField.ContactShadows, null, "When enabled, Cameras using these Frame Settings render Contact Shadows", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		ContactShadows = 21,
		[FrameSettingsField(1, FrameSettingsField.ScreenSpaceShadows, null, "When enabled, Cameras using these Frame Settings render Screen Space Shadows (Depends on \"Screen Space Shadows\" in current HDRP Asset). Note that Screen Space Shadows are disabled when MSAA is enabled.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 23)]
		ScreenSpaceShadows = 34,
		[FrameSettingsField(1, FrameSettingsField.Shadowmask, null, "When enabled, Cameras using these Frame Settings render shadows from Shadow Masks (Depends on \"Shadowmask\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 24)]
		Shadowmask = 22,
		[FrameSettingsField(1, FrameSettingsField.None, "Screen Space Reflection", "When enabled, Cameras using these Frame Settings calculate Screen Space Reflections (Depends on \"Screen Space Reflection\" in current HDRP Asset). Note that Screen Space Reflections are disabled when MSAA is enabled.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		SSR = 23,
		[FrameSettingsField(1, FrameSettingsField.None, "Transparents", "When enabled, Cameras using these Frame Settings calculate Screen Space Reflections on transparent objects.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.SSR }, null, 25)]
		TransparentSSR = 94,
		[FrameSettingsField(1, FrameSettingsField.None, "Screen Space Ambient Occlusion", "When enabled, Cameras using these Frame Settings calculate Screen Space Ambient Occlusion (Depends on \"Screen Space Ambient Occlusion\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		SSAO = 24,
		[FrameSettingsField(1, FrameSettingsField.None, "Screen Space Global Illumination", "When enabled, Cameras using these Frame Settings calculate Screen Space Global Illumination (Depends on \"Screen Space Global Illumination\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 25)]
		SSGI = 95,
		[FrameSettingsField(1, FrameSettingsField.SubsurfaceScattering, null, "When enabled, Cameras using these Frame Settings render subsurface scattering (SSS) effects for GameObjects that use a SSS Material (Depends on \"Subsurface Scattering\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 46)]
		SubsurfaceScattering = 46,
		[FrameSettingsField(1, FrameSettingsField.None, "Quality Mode", "Configures the way the sample budget of the Subsurface Scattering algorithm is determined. You can either pick from one of the existing values in the Quality Settings, or request a custom number of samples.", FrameSettingsFieldAttribute.DisplayType.Others, typeof(SssQualityMode), new FrameSettingsField[] { FrameSettingsField.SubsurfaceScattering }, null, 47)]
		SssQualityMode = 47,
		[FrameSettingsField(1, FrameSettingsField.None, "Quality Level", "Sets the Quality Level of the Subsurface Scattering algorithm.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.SubsurfaceScattering }, null, 48)]
		SssQualityLevel = 48,
		[FrameSettingsField(1, FrameSettingsField.None, "Custom Sample Budget", "Sets the custom sample budget of the Subsurface Scattering algorithm.", FrameSettingsFieldAttribute.DisplayType.Others, null, new FrameSettingsField[] { FrameSettingsField.SubsurfaceScattering }, null, 49)]
		SssCustomSampleBudget = 49,
		[FrameSettingsField(1, FrameSettingsField.VolumetricClouds, null, "When enabled, Cameras using these Frame Settings calculate Volumetric Clouds.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 50)]
		VolumetricClouds = 79,
		[FrameSettingsField(1, FrameSettingsField.FullResolutionCloudsForSky, null, "When enabled, Cameras using these Frame Settings calculate Volumetric Clouds at full resolution when evaluating the sky texture.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.VolumetricClouds }, null, 51)]
		FullResolutionCloudsForSky = 98,
		[FrameSettingsField(1, FrameSettingsField.Transmission, null, "When enabled, Cameras using these Frame Settings render subsurface scattering (SSS) Materials with an added transmission effect (only if you enable Transmission on the SSS Material in the Material's Inspector).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		Transmission = 26,
		[FrameSettingsField(1, FrameSettingsField.None, "Fog", "When enabled, Cameras using these Frame Settings render fog effects.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		AtmosphericScattering = 27,
		[FrameSettingsField(1, FrameSettingsField.Volumetrics, null, "When enabled, Cameras using these Frame Settings render volumetric effects such as volumetric fog and lighting (Depends on \"Volumetrics\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AtmosphericScattering }, null, -1)]
		Volumetrics = 28,
		[FrameSettingsField(1, FrameSettingsField.None, "Reprojection", "When enabled, Cameras using these Frame Settings use several previous frames to calculate volumetric effects which increases their overall quality at run time.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[]
		{
			FrameSettingsField.AtmosphericScattering,
			FrameSettingsField.Volumetrics
		}, null, -1)]
		ReprojectionForVolumetrics = 29,
		[FrameSettingsField(1, FrameSettingsField.LightLayers, null, "When enabled, Cameras that use these Frame Settings make use of LightLayers (Depends on \"Light Layers\" in current HDRP Asset).", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		LightLayers = 30,
		[FrameSettingsField(1, FrameSettingsField.ExposureControl, null, "When enabled, Cameras that use these Frame Settings use exposure values defined in relevant components.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 33)]
		ExposureControl = 32,
		[FrameSettingsField(1, FrameSettingsField.ReflectionProbe, null, "When enabled, Cameras that use these Frame Settings calculate reflection from Reflection Probes.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		ReflectionProbe = 33,
		[FrameSettingsField(1, FrameSettingsField.None, "Planar Reflection Probe", "When enabled, Cameras that use these Frame Settings calculate reflection from Planar Reflection Probes.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 36)]
		PlanarProbe = 35,
		[FrameSettingsField(1, FrameSettingsField.None, "Metallic Indirect Fallback", "When enabled, Cameras that use these Frame Settings render Materials with base color as diffuse. This is a useful Frame Setting to use for real-time Reflection Probes because it renders metals as diffuse Materials to stop them appearing black when Unity can't calculate several bounces of specular lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		ReplaceDiffuseForIndirect = 36,
		[FrameSettingsField(1, FrameSettingsField.SkyReflection, null, "When enabled, the Sky affects specular lighting for Cameras that use these Frame Settings.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		SkyReflection = 37,
		[FrameSettingsField(1, FrameSettingsField.DirectSpecularLighting, null, "When enabled, Cameras that use these Frame Settings render Direct Specular lighting. This is a useful Frame Setting to use for baked Reflection Probes to remove view dependent lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		DirectSpecularLighting = 38,
		[FrameSettingsField(1, FrameSettingsField.ProbeVolume, null, "Enable to debug and make HDRP process Probe Volumes. Enabling this feature causes HDRP to process Probe Volumes for this Camera/Reflection Probe.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, 3)]
		ProbeVolume = 127,
		[FrameSettingsField(1, FrameSettingsField.None, "Normalize Reflection Probes", null, FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.ProbeVolume }, null, 4)]
		NormalizeReflectionProbeWithProbeVolume = 126,
		[FrameSettingsField(2, FrameSettingsField.None, "Asynchronous Execution", "When enabled, HDRP executes certain Compute Shader commands in parallel. This is only supported on DX12 and Vulkan. If Asynchronous execution is disabled or not supported the effects will fallback on a synchronous version.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		AsyncCompute = 40,
		[FrameSettingsField(2, FrameSettingsField.None, "Light List", "When enabled, HDRP builds the Light List asynchronously.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AsyncCompute }, null, -1)]
		LightListAsync = 41,
		[FrameSettingsField(2, FrameSettingsField.None, "SS Reflection", "When enabled, HDRP calculates screen space reflection asynchronously.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AsyncCompute }, null, -1)]
		SSRAsync = 42,
		[FrameSettingsField(2, FrameSettingsField.None, "SS Ambient Occlusion", "When enabled, HDRP calculates screen space ambient occlusion asynchronously.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AsyncCompute }, null, -1)]
		SSAOAsync = 43,
		[FrameSettingsField(2, FrameSettingsField.None, "Contact Shadows", "When enabled, HDRP calculates Contact Shadows asynchronously.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AsyncCompute }, null, -1)]
		ContactShadowsAsync = 44,
		[FrameSettingsField(2, FrameSettingsField.None, "Volume Voxelizations", "When enabled, HDRP calculates volumetric voxelization asynchronously.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.AsyncCompute }, null, -1)]
		VolumeVoxelizationsAsync = 45,
		[FrameSettingsField(3, FrameSettingsField.FPTLForForwardOpaque, null, "When enabled, HDRP uses FPTL for forward opaque.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		FPTLForForwardOpaque = 120,
		[FrameSettingsField(3, FrameSettingsField.BigTilePrepass, null, "When enabled, HDRP uses a big tile prepass for light visibility.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		BigTilePrepass = 121,
		[FrameSettingsField(3, FrameSettingsField.DeferredTile, null, "When enabled, HDRP uses tiles to compute deferred lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, null, null, -1)]
		DeferredTile = 122,
		[FrameSettingsField(3, FrameSettingsField.ComputeLightEvaluation, null, "When enabled, HDRP uses a compute shader to compute deferred lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.DeferredTile }, null, -1)]
		ComputeLightEvaluation = 123,
		[FrameSettingsField(3, FrameSettingsField.ComputeLightVariants, null, "When enabled, HDRP uses light variant classification to compute lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.DeferredTile }, null, -1)]
		ComputeLightVariants = 124,
		[FrameSettingsField(3, FrameSettingsField.ComputeMaterialVariants, null, "When enabled, HDRP uses material variant classification to compute lighting.", FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox, null, new FrameSettingsField[] { FrameSettingsField.DeferredTile }, null, -1)]
		ComputeMaterialVariants = 125
	}
}
