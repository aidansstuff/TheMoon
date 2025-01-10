using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRenderPipelineRuntimeResources : HDRenderPipelineResources, IVersionable<HDRenderPipelineRuntimeResources.Version>, IMigratableAsset
	{
		[Serializable]
		[ReloadGroup]
		public sealed class ShaderResources
		{
			[Reload("Runtime/Material/Lit/Lit.shader", ReloadAttribute.Package.Root)]
			public Shader defaultPS;

			[Reload("Runtime/Debug/DebugDisplayLatlong.Shader", ReloadAttribute.Package.Root)]
			public Shader debugDisplayLatlongPS;

			[Reload("Runtime/Debug/DebugViewMaterialGBuffer.Shader", ReloadAttribute.Package.Root)]
			public Shader debugViewMaterialGBufferPS;

			[Reload("Runtime/Debug/DebugViewTiles.Shader", ReloadAttribute.Package.Root)]
			public Shader debugViewTilesPS;

			[Reload("Runtime/Debug/DebugFullScreen.Shader", ReloadAttribute.Package.Root)]
			public Shader debugFullScreenPS;

			[Reload("Runtime/Debug/DebugColorPicker.Shader", ReloadAttribute.Package.Root)]
			public Shader debugColorPickerPS;

			[Reload("Runtime/Debug/DebugExposure.Shader", ReloadAttribute.Package.Root)]
			public Shader debugExposurePS;

			[Reload("Runtime/Debug/DebugHDR.Shader", ReloadAttribute.Package.Root)]
			public Shader debugHDRPS;

			[Reload("Runtime/Debug/DebugLightVolumes.Shader", ReloadAttribute.Package.Root)]
			public Shader debugLightVolumePS;

			[Reload("Runtime/Debug/DebugLightVolumes.compute", ReloadAttribute.Package.Root)]
			public ComputeShader debugLightVolumeCS;

			[Reload("Runtime/Debug/DebugBlitQuad.Shader", ReloadAttribute.Package.Root)]
			public Shader debugBlitQuad;

			[Reload("Runtime/Debug/DebugVTBlit.Shader", ReloadAttribute.Package.Root)]
			public Shader debugViewVirtualTexturingBlit;

			[Reload("Runtime/Debug/MaterialError.Shader", ReloadAttribute.Package.Root)]
			public Shader materialError;

			[Reload("Runtime/Debug/MaterialLoading.shader", ReloadAttribute.Package.Root)]
			public Shader materialLoading;

			[Reload("Runtime/Debug/ClearDebugBuffer.compute", ReloadAttribute.Package.Root)]
			public ComputeShader clearDebugBufferCS;

			[Reload("Runtime/Debug/ProbeVolumeDebug.shader", ReloadAttribute.Package.Root)]
			public Shader probeVolumeDebugShader;

			[Reload("Runtime/Debug/ProbeVolumeOffsetDebug.shader", ReloadAttribute.Package.Root)]
			public Shader probeVolumeOffsetDebugShader;

			[Reload("Runtime/Lighting/ProbeVolume/ProbeVolumeBlendStates.compute", ReloadAttribute.Package.Root)]
			public ComputeShader probeVolumeBlendStatesCS;

			[Reload("Runtime/Debug/DebugWaveform.shader", ReloadAttribute.Package.Root)]
			public Shader debugWaveformPS;

			[Reload("Runtime/Debug/DebugWaveform.compute", ReloadAttribute.Package.Root)]
			public ComputeShader debugWaveformCS;

			[Reload("Runtime/Debug/DebugVectorscope.shader", ReloadAttribute.Package.Root)]
			public Shader debugVectorscopePS;

			[Reload("Runtime/Debug/DebugVectorscope.compute", ReloadAttribute.Package.Root)]
			public ComputeShader debugVectorscopeCS;

			[Reload("Runtime/Lighting/Deferred.Shader", ReloadAttribute.Package.Root)]
			public Shader deferredPS;

			[Reload("Runtime/RenderPipeline/RenderPass/ColorPyramidPS.Shader", ReloadAttribute.Package.Root)]
			public Shader colorPyramidPS;

			[Reload("Runtime/RenderPipeline/RenderPass/DepthPyramid.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthPyramidCS;

			[Reload("Runtime/RenderPipeline/RenderPass/GenerateMaxZ.compute", ReloadAttribute.Package.Root)]
			public ComputeShader maxZCS;

			[Reload("Runtime/Core/CoreResources/GPUCopy.compute", ReloadAttribute.Package.Root)]
			public ComputeShader copyChannelCS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/ScreenSpaceReflections.compute", ReloadAttribute.Package.Root)]
			public ComputeShader screenSpaceReflectionsCS;

			[Reload("Runtime/RenderPipeline/RenderPass/Distortion/ApplyDistortion.shader", ReloadAttribute.Package.Root)]
			public Shader applyDistortionPS;

			[Reload("Runtime/Lighting/LightLoop/cleardispatchindirect.compute", ReloadAttribute.Package.Root)]
			public ComputeShader clearDispatchIndirectCS;

			[Reload("Runtime/Lighting/LightLoop/ClearLightLists.compute", ReloadAttribute.Package.Root)]
			public ComputeShader clearLightListsCS;

			[Reload("Runtime/Lighting/LightLoop/builddispatchindirect.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildDispatchIndirectCS;

			[Reload("Runtime/Lighting/LightLoop/scrbound.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildScreenAABBCS;

			[Reload("Runtime/Lighting/LightLoop/lightlistbuild.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildPerTileLightListCS;

			[Reload("Runtime/Lighting/LightLoop/lightlistbuild-bigtile.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildPerBigTileLightListCS;

			[Reload("Runtime/Lighting/LightLoop/lightlistbuild-clustered.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildPerVoxelLightListCS;

			[Reload("Runtime/Lighting/LightLoop/lightlistbuild-clearatomic.compute", ReloadAttribute.Package.Root)]
			public ComputeShader lightListClusterClearAtomicIndexCS;

			[Reload("Runtime/Lighting/LightLoop/materialflags.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildMaterialFlagsCS;

			[Reload("Runtime/Lighting/LightLoop/Deferred.compute", ReloadAttribute.Package.Root)]
			public ComputeShader deferredCS;

			[Reload("Runtime/Lighting/Shadow/ContactShadows.compute", ReloadAttribute.Package.Root)]
			public ComputeShader contactShadowCS;

			[Reload("Runtime/Lighting/VolumetricLighting/VolumeVoxelization.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumeVoxelizationCS;

			[Reload("Runtime/Lighting/VolumetricLighting/VolumetricLighting.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumetricLightingCS;

			[Reload("Runtime/Lighting/VolumetricLighting/VolumetricLightingFiltering.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumetricLightingFilteringCS;

			[Reload("Runtime/Lighting/LightLoop/DeferredTile.shader", ReloadAttribute.Package.Root)]
			public Shader deferredTilePS;

			[Reload("Runtime/Lighting/Shadow/ScreenSpaceShadows.shader", ReloadAttribute.Package.Root)]
			public Shader screenSpaceShadowPS;

			[Reload("Runtime/Material/SubsurfaceScattering/SubsurfaceScattering.compute", ReloadAttribute.Package.Root)]
			public ComputeShader subsurfaceScatteringCS;

			[Reload("Runtime/Material/SubsurfaceScattering/CombineLighting.shader", ReloadAttribute.Package.Root)]
			public Shader combineLightingPS;

			[Reload("Runtime/Lighting/VolumetricLighting/DebugLocalVolumetricFogAtlas.shader", ReloadAttribute.Package.Root)]
			public Shader debugLocalVolumetricFogAtlasPS;

			[Reload("Runtime/RenderPipeline/RenderPass/MotionVectors/CameraMotionVectors.shader", ReloadAttribute.Package.Root)]
			public Shader cameraMotionVectorsPS;

			[Reload("Runtime/ShaderLibrary/ClearStencilBuffer.shader", ReloadAttribute.Package.Root)]
			public Shader clearStencilBufferPS;

			[Reload("Runtime/ShaderLibrary/CopyStencilBuffer.shader", ReloadAttribute.Package.Root)]
			public Shader copyStencilBufferPS;

			[Reload("Runtime/ShaderLibrary/CopyDepthBuffer.shader", ReloadAttribute.Package.Root)]
			public Shader copyDepthBufferPS;

			[Reload("Runtime/ShaderLibrary/Blit.shader", ReloadAttribute.Package.Root)]
			public Shader blitPS;

			[Reload("Runtime/ShaderLibrary/BlitColorAndDepth.shader", ReloadAttribute.Package.Root)]
			public Shader blitColorAndDepthPS;

			[Reload("Runtime/Core/CoreResources/ClearBuffer2D.compute", ReloadAttribute.Package.Root)]
			public ComputeShader clearBuffer2D;

			[Reload("Runtime/ShaderLibrary/DownsampleDepth.shader", ReloadAttribute.Package.Root)]
			public Shader downsampleDepthPS;

			[Reload("Runtime/ShaderLibrary/UpsampleTransparent.shader", ReloadAttribute.Package.Root)]
			public Shader upsampleTransparentPS;

			[Reload("Runtime/ShaderLibrary/ResolveStencilBuffer.compute", ReloadAttribute.Package.Root)]
			public ComputeShader resolveStencilCS;

			[Reload("Runtime/Sky/BlitCubemap.shader", ReloadAttribute.Package.Root)]
			public Shader blitCubemapPS;

			[Reload("Runtime/Material/GGXConvolution/BuildProbabilityTables.compute", ReloadAttribute.Package.Root)]
			public ComputeShader buildProbabilityTablesCS;

			[Reload("Runtime/Material/GGXConvolution/ComputeGgxIblSampleData.compute", ReloadAttribute.Package.Root)]
			public ComputeShader computeGgxIblSampleDataCS;

			[Reload("Runtime/Material/GGXConvolution/GGXConvolve.shader", ReloadAttribute.Package.Root)]
			public Shader GGXConvolvePS;

			[Reload("Runtime/Material/Fabric/CharlieConvolve.shader", ReloadAttribute.Package.Root)]
			public Shader charlieConvolvePS;

			[Reload("Runtime/Lighting/AtmosphericScattering/OpaqueAtmosphericScattering.shader", ReloadAttribute.Package.Root)]
			public Shader opaqueAtmosphericScatteringPS;

			[Reload("Runtime/Sky/HDRISky/HDRISky.shader", ReloadAttribute.Package.Root)]
			public Shader hdriSkyPS;

			[Reload("Runtime/Sky/HDRISky/IntegrateHDRISky.shader", ReloadAttribute.Package.Root)]
			public Shader integrateHdriSkyPS;

			[Reload("Skybox/Cubemap", ReloadAttribute.Package.Builtin)]
			public Shader skyboxCubemapPS;

			[Reload("Runtime/Sky/GradientSky/GradientSky.shader", ReloadAttribute.Package.Root)]
			public Shader gradientSkyPS;

			[Reload("Runtime/Sky/AmbientProbeConvolution.compute", ReloadAttribute.Package.Root)]
			public ComputeShader ambientProbeConvolutionCS;

			[Reload("Runtime/Sky/PhysicallyBasedSky/GroundIrradiancePrecomputation.compute", ReloadAttribute.Package.Root)]
			public ComputeShader groundIrradiancePrecomputationCS;

			[Reload("Runtime/Sky/PhysicallyBasedSky/InScatteredRadiancePrecomputation.compute", ReloadAttribute.Package.Root)]
			public ComputeShader inScatteredRadiancePrecomputationCS;

			[Reload("Runtime/Sky/PhysicallyBasedSky/PhysicallyBasedSky.shader", ReloadAttribute.Package.Root)]
			public Shader physicallyBasedSkyPS;

			[Reload("Runtime/Lighting/PlanarReflectionFiltering.compute", ReloadAttribute.Package.Root)]
			public ComputeShader planarReflectionFilteringCS;

			[Reload("Runtime/Sky/CloudSystem/CloudLayer/CloudLayer.shader", ReloadAttribute.Package.Root)]
			public Shader cloudLayerPS;

			[Reload("Runtime/Sky/CloudSystem/CloudLayer/BakeCloudTexture.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bakeCloudTextureCS;

			[Reload("Runtime/Sky/CloudSystem/CloudLayer/BakeCloudShadows.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bakeCloudShadowsCS;

			[Reload("Runtime/Lighting/VolumetricLighting/VolumetricClouds.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumetricCloudsCS;

			[Reload("Editor/Lighting/VolumetricClouds/CloudMapGenerator.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumetricCloudMapGeneratorCS;

			[Reload("Runtime/Lighting/VolumetricLighting/VolumetricCloudsCombine.shader", ReloadAttribute.Package.Root)]
			public Shader volumetricCloudsCombinePS;

			[Reload("Runtime/Water/WaterSimulation.compute", ReloadAttribute.Package.Root)]
			public ComputeShader waterSimulationCS;

			[Reload("Runtime/Water/FourierTransform.compute", ReloadAttribute.Package.Root)]
			public ComputeShader fourierTransformCS;

			[Reload("Runtime/RenderPipelineResources/ShaderGraph/Water.shadergraph", ReloadAttribute.Package.Root)]
			public Shader waterPS;

			[Reload("Runtime/Water/WaterLighting.compute", ReloadAttribute.Package.Root)]
			public ComputeShader waterLightingCS;

			[Reload("Runtime/Water/WaterCaustics.shader", ReloadAttribute.Package.Root)]
			public Shader waterCausticsPS;

			[Reload("Runtime/Material/PreIntegratedFGD/PreIntegratedFGD_GGXDisneyDiffuse.shader", ReloadAttribute.Package.Root)]
			public Shader preIntegratedFGD_GGXDisneyDiffusePS;

			[Reload("Runtime/Material/PreIntegratedFGD/PreIntegratedFGD_CharlieFabricLambert.shader", ReloadAttribute.Package.Root)]
			public Shader preIntegratedFGD_CharlieFabricLambertPS;

			[Reload("Runtime/Material/AxF/PreIntegratedFGD_Ward.shader", ReloadAttribute.Package.Root)]
			public Shader preIntegratedFGD_WardPS;

			[Reload("Runtime/Material/AxF/PreIntegratedFGD_CookTorrance.shader", ReloadAttribute.Package.Root)]
			public Shader preIntegratedFGD_CookTorrancePS;

			[Reload("Runtime/Material/PreIntegratedFGD/PreIntegratedFGD_Marschner.shader", ReloadAttribute.Package.Root)]
			public Shader preIntegratedFGD_MarschnerPS;

			[Reload("Runtime/Material/Hair/MultipleScattering/HairMultipleScatteringPreIntegration.compute", ReloadAttribute.Package.Root)]
			public ComputeShader preIntegratedFiberScatteringCS;

			[Reload("Runtime/Material/VolumetricMaterial/VolumetricMaterial.compute", ReloadAttribute.Package.Root)]
			public ComputeShader volumetricMaterialCS;

			[Reload("Runtime/Core/CoreResources/EncodeBC6H.compute", ReloadAttribute.Package.Root)]
			public ComputeShader encodeBC6HCS;

			[Reload("Runtime/Core/CoreResources/CubeToPano.shader", ReloadAttribute.Package.Root)]
			public Shader cubeToPanoPS;

			[Reload("Runtime/Core/CoreResources/BlitCubeTextureFace.shader", ReloadAttribute.Package.Root)]
			public Shader blitCubeTextureFacePS;

			[Reload("Runtime/Material/LTCAreaLight/FilterAreaLightCookies.shader", ReloadAttribute.Package.Root)]
			public Shader filterAreaLightCookiesPS;

			[Reload("Runtime/Core/CoreResources/ClearUIntTextureArray.compute", ReloadAttribute.Package.Root)]
			public ComputeShader clearUIntTextureCS;

			[Reload("Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassUtils.shader", ReloadAttribute.Package.Root)]
			public Shader customPassUtils;

			[Reload("Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassRenderersUtils.shader", ReloadAttribute.Package.Root)]
			public Shader customPassRenderersUtils;

			[Reload("Runtime/RenderPipeline/Utility/Texture3DAtlas.compute", ReloadAttribute.Package.Root)]
			public ComputeShader texture3DAtlasCS;

			[Reload("Runtime/ShaderLibrary/XRMirrorView.shader", ReloadAttribute.Package.Root)]
			public Shader xrMirrorViewPS;

			[Reload("Runtime/ShaderLibrary/XROcclusionMesh.shader", ReloadAttribute.Package.Root)]
			public Shader xrOcclusionMeshPS;

			[Reload("Runtime/Lighting/Shadow/ShadowClear.shader", ReloadAttribute.Package.Root)]
			public Shader shadowClearPS;

			[Reload("Runtime/Lighting/Shadow/EVSMBlur.compute", ReloadAttribute.Package.Root)]
			public ComputeShader evsmBlurCS;

			[Reload("Runtime/Lighting/Shadow/DebugDisplayHDShadowMap.shader", ReloadAttribute.Package.Root)]
			public Shader debugHDShadowMapPS;

			[Reload("Runtime/Lighting/Shadow/MomentShadows.compute", ReloadAttribute.Package.Root)]
			public ComputeShader momentShadowsCS;

			[Reload("Runtime/Lighting/Shadow/ShadowBlit.shader", ReloadAttribute.Package.Root)]
			public Shader shadowBlitPS;

			[Reload("Runtime/Material/Decal/DecalNormalBuffer.shader", ReloadAttribute.Package.Root)]
			public Shader decalNormalBufferPS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/GTAO.compute", ReloadAttribute.Package.Root)]
			public ComputeShader GTAOCS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/GTAOSpatialDenoise.compute", ReloadAttribute.Package.Root)]
			public ComputeShader GTAOSpatialDenoiseCS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/GTAOTemporalDenoise.compute", ReloadAttribute.Package.Root)]
			public ComputeShader GTAOTemporalDenoiseCS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/GTAOCopyHistory.compute", ReloadAttribute.Package.Root)]
			public ComputeShader GTAOCopyHistoryCS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/GTAOBlurAndUpsample.compute", ReloadAttribute.Package.Root)]
			public ComputeShader GTAOBlurAndUpsample;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/ScreenSpaceGlobalIllumination.compute", ReloadAttribute.Package.Root)]
			public ComputeShader screenSpaceGlobalIlluminationCS;

			[Reload("Runtime/RenderPipeline/RenderPass/MSAA/DepthValues.shader", ReloadAttribute.Package.Root)]
			public Shader depthValuesPS;

			[Reload("Runtime/RenderPipeline/RenderPass/MSAA/ColorResolve.shader", ReloadAttribute.Package.Root)]
			public Shader colorResolvePS;

			[Reload("Runtime/RenderPipeline/RenderPass/MSAA/MotionVecResolve.shader", ReloadAttribute.Package.Root)]
			public Shader resolveMotionVecPS;

			[Reload("Runtime/PostProcessing/Shaders/AlphaCopy.compute", ReloadAttribute.Package.Root)]
			public ComputeShader copyAlphaCS;

			[Reload("Runtime/PostProcessing/Shaders/NaNKiller.compute", ReloadAttribute.Package.Root)]
			public ComputeShader nanKillerCS;

			[Reload("Runtime/PostProcessing/Shaders/Exposure.compute", ReloadAttribute.Package.Root)]
			public ComputeShader exposureCS;

			[Reload("Runtime/PostProcessing/Shaders/HistogramExposure.compute", ReloadAttribute.Package.Root)]
			public ComputeShader histogramExposureCS;

			[Reload("Runtime/PostProcessing/Shaders/ApplyExposure.compute", ReloadAttribute.Package.Root)]
			public ComputeShader applyExposureCS;

			[Reload("Runtime/PostProcessing/Shaders/DebugHistogramImage.compute", ReloadAttribute.Package.Root)]
			public ComputeShader debugImageHistogramCS;

			[Reload("Runtime/PostProcessing/Shaders/DebugHDRxyMapping.compute", ReloadAttribute.Package.Root)]
			public ComputeShader debugHDRxyMappingCS;

			[Reload("Runtime/PostProcessing/Shaders/UberPost.compute", ReloadAttribute.Package.Root)]
			public ComputeShader uberPostCS;

			[Reload("Runtime/PostProcessing/Shaders/LutBuilder3D.compute", ReloadAttribute.Package.Root)]
			public ComputeShader lutBuilder3DCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldKernel.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldKernelCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoC.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldCoCCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoCReproject.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldCoCReprojectCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCoCDilate.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldDilateCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldMip.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldMipCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldMipSafe.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldMipSafeCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldPrefilter.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldPrefilterCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldTileMax.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldTileMaxCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldGather.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldGatherCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldCombine.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldCombineCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldPreCombineFar.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldPreCombineFarCS;

			[Reload("Runtime/PostProcessing/Shaders/DepthOfFieldClearIndirectArgs.compute", ReloadAttribute.Package.Root)]
			public ComputeShader depthOfFieldClearIndirectArgsCS;

			[Reload("Runtime/PostProcessing/Shaders/PaniniProjection.compute", ReloadAttribute.Package.Root)]
			public ComputeShader paniniProjectionCS;

			[Reload("Runtime/PostProcessing/Shaders/MotionBlurMotionVecPrep.compute", ReloadAttribute.Package.Root)]
			public ComputeShader motionBlurMotionVecPrepCS;

			[Reload("Runtime/PostProcessing/Shaders/MotionBlurGenTilePass.compute", ReloadAttribute.Package.Root)]
			public ComputeShader motionBlurGenTileCS;

			[Reload("Runtime/PostProcessing/Shaders/MotionBlurMergeTilePass.compute", ReloadAttribute.Package.Root)]
			public ComputeShader motionBlurMergeTileCS;

			[Reload("Runtime/PostProcessing/Shaders/MotionBlurNeighborhoodTilePass.compute", ReloadAttribute.Package.Root)]
			public ComputeShader motionBlurNeighborhoodTileCS;

			[Reload("Runtime/PostProcessing/Shaders/MotionBlur.compute", ReloadAttribute.Package.Root)]
			public ComputeShader motionBlurCS;

			[Reload("Runtime/PostProcessing/Shaders/BloomPrefilter.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bloomPrefilterCS;

			[Reload("Runtime/PostProcessing/Shaders/BloomBlur.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bloomBlurCS;

			[Reload("Runtime/PostProcessing/Shaders/BloomUpsample.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bloomUpsampleCS;

			[Reload("Runtime/PostProcessing/Shaders/FXAA.compute", ReloadAttribute.Package.Root)]
			public ComputeShader FXAACS;

			[Reload("Runtime/PostProcessing/Shaders/FinalPass.shader", ReloadAttribute.Package.Root)]
			public Shader finalPassPS;

			[Reload("Runtime/PostProcessing/Shaders/ClearBlack.shader", ReloadAttribute.Package.Root)]
			public Shader clearBlackPS;

			[Reload("Runtime/PostProcessing/Shaders/SubpixelMorphologicalAntialiasing.shader", ReloadAttribute.Package.Root)]
			public Shader SMAAPS;

			[Reload("Runtime/PostProcessing/Shaders/TemporalAntialiasing.shader", ReloadAttribute.Package.Root)]
			public Shader temporalAntialiasingPS;

			[Reload("Runtime/PostProcessing/Shaders/LensFlareDataDriven.shader", ReloadAttribute.Package.Root)]
			public Shader lensFlareDataDrivenPS;

			[Reload("Runtime/PostProcessing/Shaders/LensFlareMergeOcclusionDataDriven.compute", ReloadAttribute.Package.Root)]
			public ComputeShader lensFlareMergeOcclusionCS;

			[Reload("Runtime/PostProcessing/Shaders/DLSSBiasColorMask.shader", ReloadAttribute.Package.Root)]
			public Shader DLSSBiasColorMaskPS;

			[Reload("Runtime/PostProcessing/Shaders/CompositeWithUIAndOETF.shader", ReloadAttribute.Package.Root)]
			public Shader compositeUIAndOETFApplyPS;

			[Reload("Runtime/PostProcessing/Shaders/DoFCircleOfConfusion.compute", ReloadAttribute.Package.Root)]
			public ComputeShader dofCircleOfConfusion;

			[Reload("Runtime/PostProcessing/Shaders/DoFGather.compute", ReloadAttribute.Package.Root)]
			public ComputeShader dofGatherCS;

			[Reload("Runtime/PostProcessing/Shaders/DoFCoCMinMax.compute", ReloadAttribute.Package.Root)]
			public ComputeShader dofCoCMinMaxCS;

			[Reload("Runtime/PostProcessing/Shaders/DoFMinMaxDilate.compute", ReloadAttribute.Package.Root)]
			public ComputeShader dofMinMaxDilateCS;

			[Reload("Runtime/PostProcessing/Shaders/DoFCombine.compute", ReloadAttribute.Package.Root)]
			public ComputeShader dofCombineCS;

			[Reload("Runtime/PostProcessing/Shaders/ContrastAdaptiveSharpen.compute", ReloadAttribute.Package.Root)]
			public ComputeShader contrastAdaptiveSharpenCS;

			[Reload("Runtime/PostProcessing/Shaders/EdgeAdaptiveSpatialUpsampling.compute", ReloadAttribute.Package.Root)]
			public ComputeShader edgeAdaptiveSpatialUpsamplingCS;

			[Reload("Runtime/VirtualTexturing/Shaders/DownsampleVTFeedback.compute", ReloadAttribute.Package.Root)]
			public ComputeShader VTFeedbackDownsample;

			[Reload("Runtime/RenderPipeline/Accumulation/Shaders/Accumulation.compute", ReloadAttribute.Package.Root)]
			public ComputeShader accumulationCS;

			[Reload("Runtime/RenderPipeline/Accumulation/Shaders/BlitAndExpose.compute", ReloadAttribute.Package.Root)]
			public ComputeShader blitAndExposeCS;

			[Reload("Runtime/Compositor/Shaders/AlphaInjection.shader", ReloadAttribute.Package.Root)]
			public Shader alphaInjectionPS;

			[Reload("Runtime/Compositor/Shaders/ChromaKeying.shader", ReloadAttribute.Package.Root)]
			public Shader chromaKeyingPS;

			[Reload("Runtime/Compositor/Shaders/CustomClear.shader", ReloadAttribute.Package.Root)]
			public Shader customClearPS;

			[Reload("Runtime/Lighting/ScreenSpaceLighting/BilateralUpsample.compute", ReloadAttribute.Package.Root)]
			public ComputeShader bilateralUpsampleCS;

			[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Denoising/TemporalFilter.compute", ReloadAttribute.Package.Root)]
			public ComputeShader temporalFilterCS;

			[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Denoising/DiffuseDenoiser.compute", ReloadAttribute.Package.Root)]
			public ComputeShader diffuseDenoiserCS;
		}

		[Serializable]
		[ReloadGroup]
		public sealed class MaterialResources
		{
			[Reload("Runtime/RenderPipelineResources/Material/AreaLightCookieViewer.mat", ReloadAttribute.Package.Root)]
			public Material areaLightCookieMaterial;
		}

		[Serializable]
		[ReloadGroup]
		public sealed class TextureResources
		{
			[Reload("Runtime/RenderPipelineResources/Texture/DebugFont.tga", ReloadAttribute.Package.Root)]
			public Texture2D debugFontTex;

			[Reload("Runtime/Debug/ColorGradient.png", ReloadAttribute.Package.Root)]
			public Texture2D colorGradient;

			[Reload("Runtime/RenderPipelineResources/Texture/Matcap/DefaultMatcap.png", ReloadAttribute.Package.Root)]
			public Texture2D matcapTex;

			[Reload("Runtime/RenderPipelineResources/Texture/BlueNoise16/L/LDR_LLL1_{0}.png", 0, 32, ReloadAttribute.Package.Root)]
			public Texture2D[] blueNoise16LTex;

			[Reload("Runtime/RenderPipelineResources/Texture/BlueNoise16/RGB/LDR_RGB1_{0}.png", 0, 32, ReloadAttribute.Package.Root)]
			public Texture2D[] blueNoise16RGBTex;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/OwenScrambledNoise4.png", ReloadAttribute.Package.Root)]
			public Texture2D owenScrambledRGBATex;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/OwenScrambledNoise256.png", ReloadAttribute.Package.Root)]
			public Texture2D owenScrambled256Tex;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/ScrambleNoise.png", ReloadAttribute.Package.Root)]
			public Texture2D scramblingTex;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/RankingTile1SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D rankingTile1SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/ScramblingTile1SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D scramblingTile1SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/RankingTile8SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D rankingTile8SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/ScramblingTile8SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D scramblingTile8SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/RankingTile256SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D rankingTile256SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/CoherentNoise/ScramblingTile256SPP.png", ReloadAttribute.Package.Root)]
			public Texture2D scramblingTile256SPP;

			[Reload("Runtime/RenderPipelineResources/Texture/EyeCausticLUT16R.exr", ReloadAttribute.Package.Root)]
			public Texture3D eyeCausticLUT;

			[Reload("Runtime/RenderPipelineResources/Texture/VolumetricClouds/CloudLutRainAO.png", ReloadAttribute.Package.Root)]
			public Texture2D cloudLutRainAO;

			[Reload("Runtime/RenderPipelineResources/Texture/VolumetricClouds/WorleyNoise128RGBA.png", ReloadAttribute.Package.Root)]
			public Texture3D worleyNoise128RGBA;

			[Reload("Runtime/RenderPipelineResources/Texture/VolumetricClouds/WorleyNoise32RGB.png", ReloadAttribute.Package.Root)]
			public Texture3D worleyNoise32RGB;

			[Reload("Runtime/RenderPipelineResources/Texture/VolumetricClouds/PerlinNoise32RGB.png", ReloadAttribute.Package.Root)]
			public Texture3D perlinNoise32RGB;

			[Reload("Runtime/RenderPipelineResources/Texture/Water/FoamSurface.png", ReloadAttribute.Package.Root)]
			public Texture2D foamSurface;

			[Reload(new string[] { "Runtime/RenderPipelineResources/Texture/FilmGrain/Thin01.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Thin02.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium01.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium02.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium03.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium04.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium05.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Medium06.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Large01.png", "Runtime/RenderPipelineResources/Texture/FilmGrain/Large02.png" }, ReloadAttribute.Package.Root)]
			public Texture2D[] filmGrainTex;

			[Reload("Runtime/RenderPipelineResources/Texture/SMAA/SearchTex.tga", ReloadAttribute.Package.Root)]
			public Texture2D SMAASearchTex;

			[Reload("Runtime/RenderPipelineResources/Texture/SMAA/AreaTex.tga", ReloadAttribute.Package.Root)]
			public Texture2D SMAAAreaTex;

			[Reload("Runtime/RenderPipelineResources/Texture/DefaultHDRISky.exr", ReloadAttribute.Package.Root)]
			public Cubemap defaultHDRISky;

			[Reload("Runtime/RenderPipelineResources/Texture/DefaultCloudMap.png", ReloadAttribute.Package.Root)]
			public Texture2D defaultCloudMap;
		}

		[Serializable]
		[ReloadGroup]
		public sealed class ShaderGraphResources
		{
			[Reload("Runtime/ShaderLibrary/SolidColor.shadergraph", ReloadAttribute.Package.Root)]
			public Shader objectIDPS;

			[Reload("Runtime/RenderPipelineResources/ShaderGraph/DefaultFogVolume.shadergraph", ReloadAttribute.Package.Root)]
			public Shader defaultFogVolumeShader;
		}

		[Serializable]
		[ReloadGroup]
		public sealed class AssetResources
		{
			[Reload("Runtime/RenderPipelineResources/defaultDiffusionProfile.asset", ReloadAttribute.Package.Root)]
			public DiffusionProfileSettings defaultDiffusionProfile;

			[Reload("Runtime/RenderPipelineResources/Mesh/Cylinder.fbx", ReloadAttribute.Package.Root)]
			public Mesh emissiveCylinderMesh;

			[Reload("Runtime/RenderPipelineResources/Mesh/Quad.fbx", ReloadAttribute.Package.Root)]
			public Mesh emissiveQuadMesh;

			[Reload("Runtime/RenderPipelineResources/Mesh/Sphere.fbx", ReloadAttribute.Package.Root)]
			public Mesh sphereMesh;

			[Reload("Runtime/RenderPipelineResources/Mesh/ProbeDebugSphere.fbx", ReloadAttribute.Package.Root)]
			public Mesh probeDebugSphere;

			[Reload("Runtime/RenderPipelineResources/Mesh/ProbeDebugPyramid.fbx", ReloadAttribute.Package.Root)]
			public Mesh pyramidMesh;
		}

		private enum Version
		{
			None = 0,
			First = 1,
			RemovedEditorOnlyResources = 4
		}

		public ShaderResources shaders;

		public MaterialResources materials;

		public TextureResources textures;

		public ShaderGraphResources shaderGraphs;

		public AssetResources assets;

		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("version")]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		Version IVersionable<Version>.version
		{
			get
			{
				return m_Version;
			}
			set
			{
				m_Version = value;
			}
		}
	}
}
