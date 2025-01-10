using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SkyManager
	{
		private class SetGlobalSkyDataPassData
		{
			public BuiltinSkyParameters builtinParameters = new BuiltinSkyParameters();

			public SkyRenderer skyRenderer;
		}

		private class RenderSkyToCubemapPassData
		{
			public BuiltinSkyParameters builtinParameters = new BuiltinSkyParameters();

			public SkyRenderer skyRenderer;

			public CloudRenderer cloudRenderer;

			public Matrix4x4[] cameraViewMatrices;

			public Matrix4x4[] facePixelCoordToViewDirMatrices;

			public bool includeSunInBaking;

			public TextureHandle output;
		}

		private class UpdateAmbientProbePassData
		{
			public ComputeShader computeAmbientProbeCS;

			public int computeAmbientProbeKernel;

			public TextureHandle skyCubemap;

			public ComputeBuffer ambientProbeResult;

			public ComputeBuffer diffuseAmbientProbeResult;

			public ComputeBuffer volumetricAmbientProbeResult;

			public ComputeBufferHandle scratchBuffer;

			public Vector4 fogParameters;

			public Action<AsyncGPUReadbackRequest> callback;
		}

		private class SkyEnvironmentConvolutionPassData
		{
			public TextureHandle input;

			public TextureHandle intermediateTexture;

			public CubemapArray output;

			public IBLFilterBSDF[] bsdfs;
		}

		private class RenderSkyPassData
		{
			public BuiltinSkyParameters builtinParameters = new BuiltinSkyParameters();

			public TextureHandle colorBuffer;

			public TextureHandle cloudOpacityBuffer;

			public TextureHandle depthBuffer;

			public SkyUpdateContext skyContext;

			public bool renderSunDisk;
		}

		private class OpaqueAtmosphericScatteringPassData
		{
			public TextureHandle colorBuffer;

			public TextureHandle depthTexture;

			public TextureHandle volumetricLighting;

			public TextureHandle depthBuffer;

			public TextureHandle intermediateTexture;

			public Matrix4x4 pixelCoordToViewDirWS;

			public Material opaqueAtmosphericalScatteringMaterial;

			public bool pbrFog;

			public bool msaa;
		}

		private Material m_StandardSkyboxMaterial;

		private Material m_BlitCubemapMaterial;

		private Material m_OpaqueAtmScatteringMaterial;

		private SphericalHarmonicsL2 m_BlackAmbientProbe;

		private bool m_UpdateRequired;

		private bool m_StaticSkyUpdateRequired;

		private int m_Resolution;

		private int m_LowResolution;

		private SkyUpdateContext m_StaticLightingSky = new SkyUpdateContext();

		private static Dictionary<int, Type> m_SkyTypesDict = null;

		private static Dictionary<int, Type> m_CloudTypesDict = null;

		private static List<StaticLightingSky> m_StaticLightingSkies = new List<StaticLightingSky>();

		private static bool logOnce = true;

		private IBLFilterBSDF[] m_IBLFilterArray;

		private Vector4 m_CubemapScreenSize;

		private Vector4 m_LowResCubemapScreenSize;

		private Matrix4x4[] m_FacePixelCoordToViewDirMatrices = new Matrix4x4[6];

		private Matrix4x4[] m_FacePixelCoordToViewDirMatricesLowRes = new Matrix4x4[6];

		private Matrix4x4[] m_CameraRelativeViewMatrices = new Matrix4x4[6];

		private BuiltinSkyParameters m_BuiltinParameters = new BuiltinSkyParameters();

		private ComputeShader m_ComputeAmbientProbeCS;

		private static readonly int s_AmbientProbeOutputBufferParam = Shader.PropertyToID("_AmbientProbeOutputBuffer");

		private static readonly int s_VolumetricAmbientProbeOutputBufferParam = Shader.PropertyToID("_VolumetricAmbientProbeOutputBuffer");

		private static readonly int s_DiffuseAmbientProbeOutputBufferParam = Shader.PropertyToID("_DiffuseAmbientProbeOutputBuffer");

		private static readonly int s_ScratchBufferParam = Shader.PropertyToID("_ScratchBuffer");

		private static readonly int s_AmbientProbeInputCubemap = Shader.PropertyToID("_AmbientProbeInputCubemap");

		private static readonly int s_FogParameters = Shader.PropertyToID("_FogParameters");

		private int m_ComputeAmbientProbeKernel;

		private int m_ComputeAmbientProbeVolumetricKernel;

		private int m_ComputeAmbientProbeCloudsKernel;

		private CubemapArray m_BlackCubemapArray;

		private ComputeBuffer m_BlackAmbientProbeBuffer;

		private DynamicArray<CachedSkyContext> m_CachedSkyContexts = new DynamicArray<CachedSkyContext>(2);

		private DebugDisplaySettings m_CurrentDebugDisplaySettings;

		private Light m_CurrentSunLight;

		private TextureHandle m_CloudOpacity;

		public VolumeStack lightingOverrideVolumeStack { get; private set; }

		public LayerMask lightingOverrideLayerMask { get; private set; } = -1;


		public static Dictionary<int, Type> skyTypesDict
		{
			get
			{
				if (m_SkyTypesDict == null)
				{
					UpdateSkyTypes();
				}
				return m_SkyTypesDict;
			}
		}

		public static Dictionary<int, Type> cloudTypesDict
		{
			get
			{
				if (m_CloudTypesDict == null)
				{
					UpdateCloudTypes();
				}
				return m_CloudTypesDict;
			}
		}

		public TextureHandle cloudOpacity => m_CloudOpacity;

		~SkyManager()
		{
		}

		internal static SkySettings GetSkySetting(VolumeStack stack)
		{
			int value = stack.GetComponent<VisualEnvironment>().skyType.value;
			if (skyTypesDict.TryGetValue(value, out var value2))
			{
				return (SkySettings)stack.GetComponent(value2);
			}
			if (value == 2 && logOnce)
			{
				Debug.LogError("You are using the deprecated Procedural Sky in your Scene. You can still use it but, to do so, you must install it separately. To do this, open the Package Manager window and import the 'Procedural Sky' sample from the HDRP package page, then close and re-open your project without saving.");
				logOnce = false;
			}
			return null;
		}

		internal static CloudSettings GetCloudSetting(VolumeStack stack)
		{
			int value = stack.GetComponent<VisualEnvironment>().cloudType.value;
			if (cloudTypesDict.TryGetValue(value, out var value2))
			{
				return (CloudSettings)stack.GetComponent(value2);
			}
			return null;
		}

		internal static VolumetricClouds GetVolumetricClouds(VolumeStack stack)
		{
			return stack.GetComponent<VolumetricClouds>();
		}

		private static void UpdateSkyTypes()
		{
			if (m_SkyTypesDict != null)
			{
				return;
			}
			m_SkyTypesDict = new Dictionary<int, Type>();
			foreach (Type item in from t in CoreUtils.GetAllTypesDerivedFrom<SkySettings>()
				where !t.IsAbstract
				select t)
			{
				object[] customAttributes = item.GetCustomAttributes(typeof(SkyUniqueID), inherit: false);
				if (customAttributes.Length == 0)
				{
					Debug.LogWarningFormat("Missing attribute SkyUniqueID on class {0}. Class won't be registered as an available sky.", item);
					continue;
				}
				int uniqueID = ((SkyUniqueID)customAttributes[0]).uniqueID;
				Type value;
				if (uniqueID == 0)
				{
					Debug.LogWarningFormat("0 is a reserved SkyUniqueID and is used in class {0}. Class won't be registered as an available sky.", item);
				}
				else if (m_SkyTypesDict.TryGetValue(uniqueID, out value))
				{
					Debug.LogWarningFormat("SkyUniqueID {0} used in class {1} is already used in class {2}. Class won't be registered as an available sky.", uniqueID, item, value);
				}
				else
				{
					m_SkyTypesDict.Add(uniqueID, item);
				}
			}
		}

		private static void UpdateCloudTypes()
		{
			if (m_CloudTypesDict != null)
			{
				return;
			}
			m_CloudTypesDict = new Dictionary<int, Type>();
			foreach (Type item in from t in CoreUtils.GetAllTypesDerivedFrom<CloudSettings>()
				where !t.IsAbstract
				select t)
			{
				object[] customAttributes = item.GetCustomAttributes(typeof(CloudUniqueID), inherit: false);
				if (customAttributes.Length == 0)
				{
					Debug.LogWarningFormat("Missing attribute CloudUniqueID on class {0}. Class won't be registered as an available cloud type.", item);
					continue;
				}
				int uniqueID = ((CloudUniqueID)customAttributes[0]).uniqueID;
				Type value;
				if (uniqueID == 0)
				{
					Debug.LogWarningFormat("0 is a reserved CloudUniqueID and is used in class {0}. Class won't be registered as an available cloud type.", item);
				}
				else if (m_CloudTypesDict.TryGetValue(uniqueID, out value))
				{
					Debug.LogWarningFormat("CloudUniqueID {0} used in class {1} is already used in class {2}. Class won't be registered as an available cloud type.", uniqueID, item, value);
				}
				else
				{
					m_CloudTypesDict.Add(uniqueID, item);
				}
			}
		}

		public void UpdateCurrentSkySettings(HDCamera hdCamera)
		{
			hdCamera.UpdateCurrentSky(this);
		}

		private void SetGlobalSkyData(RenderGraph renderGraph, SkyUpdateContext skyContext, BuiltinSkyParameters builtinParameters)
		{
			if (!IsCachedContextValid(skyContext) || skyContext.skyRenderer == null)
			{
				return;
			}
			SetGlobalSkyDataPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<SetGlobalSkyDataPassData>("SetGlobalSkyData", out passData);
			renderGraphBuilder.AllowPassCulling(value: false);
			builtinParameters.CopyTo(passData.builtinParameters);
			passData.builtinParameters.skySettings = skyContext.skySettings;
			passData.builtinParameters.cloudSettings = skyContext.cloudSettings;
			passData.builtinParameters.volumetricClouds = skyContext.volumetricClouds;
			passData.skyRenderer = skyContext.skyRenderer;
			renderGraphBuilder.SetRenderFunc(delegate(SetGlobalSkyDataPassData data, RenderGraphContext ctx)
			{
				data.builtinParameters.commandBuffer = ctx.cmd;
				data.skyRenderer.SetGlobalSkyData(ctx.cmd, data.builtinParameters);
			});
		}

		public void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources, IBLFilterBSDF[] iblFilterBSDFArray)
		{
			m_LowResolution = 16;
			m_Resolution = (int)hdAsset.currentPlatformRenderPipelineSettings.lightLoopSettings.skyReflectionSize;
			m_IBLFilterArray = iblFilterBSDFArray;
			m_StandardSkyboxMaterial = CoreUtils.CreateEngineMaterial(defaultResources.shaders.skyboxCubemapPS);
			m_BlitCubemapMaterial = CoreUtils.CreateEngineMaterial(defaultResources.shaders.blitCubemapPS);
			m_OpaqueAtmScatteringMaterial = CoreUtils.CreateEngineMaterial(defaultResources.shaders.opaqueAtmosphericScatteringPS);
			m_ComputeAmbientProbeCS = HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.ambientProbeConvolutionCS;
			m_ComputeAmbientProbeKernel = m_ComputeAmbientProbeCS.FindKernel("AmbientProbeConvolutionDiffuse");
			m_ComputeAmbientProbeVolumetricKernel = m_ComputeAmbientProbeCS.FindKernel("AmbientProbeConvolutionDiffuseVolumetric");
			m_ComputeAmbientProbeCloudsKernel = m_ComputeAmbientProbeCS.FindKernel("AmbientProbeConvolutionClouds");
			lightingOverrideVolumeStack = VolumeManager.instance.CreateStack();
			lightingOverrideLayerMask = hdAsset.currentPlatformRenderPipelineSettings.lightLoopSettings.skyLightingOverrideLayerMask;
			m_CubemapScreenSize = new Vector4(m_Resolution, m_Resolution, 1f / (float)m_Resolution, 1f / (float)m_Resolution);
			m_LowResCubemapScreenSize = new Vector4(m_LowResolution, m_LowResolution, 1f / (float)m_LowResolution, 1f / (float)m_LowResolution);
			for (int i = 0; i < 6; i++)
			{
				Matrix4x4 matrix4x = Matrix4x4.LookAt(Vector3.zero, CoreUtils.lookAtList[i], CoreUtils.upVectorList[i]) * Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
				m_FacePixelCoordToViewDirMatrices[i] = HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(MathF.PI / 2f, Vector2.zero, m_CubemapScreenSize, matrix4x, renderToCubemap: true);
				m_FacePixelCoordToViewDirMatricesLowRes[i] = HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(MathF.PI / 2f, Vector2.zero, m_LowResCubemapScreenSize, matrix4x, renderToCubemap: true);
				m_CameraRelativeViewMatrices[i] = matrix4x;
			}
			InitializeBlackCubemapArray();
			if (m_BlackAmbientProbeBuffer == null)
			{
				m_BlackAmbientProbeBuffer = new ComputeBuffer(7, 16);
				float[] array = new float[28];
				for (int j = 0; j < 28; j++)
				{
					array[j] = 0f;
				}
				m_BlackAmbientProbeBuffer.SetData(array);
			}
		}

		private void InitializeBlackCubemapArray()
		{
			if (!(m_BlackCubemapArray == null))
			{
				return;
			}
			m_BlackCubemapArray = new CubemapArray(1, m_IBLFilterArray.Length, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
			{
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Repeat,
				wrapModeV = TextureWrapMode.Clamp,
				filterMode = FilterMode.Trilinear,
				anisoLevel = 0,
				name = "BlackCubemapArray"
			};
			Color32[] colors = new Color32[1]
			{
				new Color32(0, 0, 0, 0)
			};
			for (int i = 0; i < m_IBLFilterArray.Length; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					m_BlackCubemapArray.SetPixels32(colors, (CubemapFace)j, i);
				}
			}
			m_BlackCubemapArray.Apply();
		}

		public void Cleanup()
		{
			CoreUtils.Destroy(m_StandardSkyboxMaterial);
			CoreUtils.Destroy(m_BlitCubemapMaterial);
			CoreUtils.Destroy(m_OpaqueAtmScatteringMaterial);
			CoreUtils.Destroy(m_BlackCubemapArray);
			m_BlackAmbientProbeBuffer.Release();
			for (int i = 0; i < m_CachedSkyContexts.size; i++)
			{
				m_CachedSkyContexts[i].Cleanup();
			}
			m_StaticLightingSky.Cleanup();
			lightingOverrideVolumeStack.Dispose();
		}

		public bool IsLightingSkyValid(HDCamera hdCamera)
		{
			return hdCamera.lightingSky.IsValid();
		}

		public bool IsVisualSkyValid(HDCamera hdCamera)
		{
			return hdCamera.visualSky.IsValid();
		}

		private SphericalHarmonicsL2 GetAmbientProbe(SkyUpdateContext skyContext)
		{
			if (skyContext.IsValid() && IsCachedContextValid(skyContext))
			{
				return m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext.ambientProbe;
			}
			return m_BlackAmbientProbe;
		}

		private ComputeBuffer GetDiffuseAmbientProbeBuffer(SkyUpdateContext skyContext)
		{
			if (skyContext.IsValid() && IsCachedContextValid(skyContext))
			{
				return m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext.diffuseAmbientProbeBuffer;
			}
			return m_BlackAmbientProbeBuffer;
		}

		private ComputeBuffer GetVolumetricAmbientProbeBuffer(SkyUpdateContext skyContext)
		{
			if (skyContext.IsValid() && IsCachedContextValid(skyContext))
			{
				return m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext.volumetricAmbientProbeBuffer;
			}
			return m_BlackAmbientProbeBuffer;
		}

		private Texture GetSkyCubemap(SkyUpdateContext skyContext)
		{
			if (skyContext.IsValid() && IsCachedContextValid(skyContext))
			{
				return m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext.skyboxCubemapRT;
			}
			return CoreUtils.blackCubeTexture;
		}

		private Texture GetReflectionTexture(SkyUpdateContext skyContext)
		{
			if (skyContext.IsValid() && IsCachedContextValid(skyContext))
			{
				return m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext.skyboxBSDFCubemapArray;
			}
			return m_BlackCubemapArray;
		}

		public Texture GetSkyReflection(HDCamera hdCamera)
		{
			return GetReflectionTexture(hdCamera.lightingSky);
		}

		private SkyUpdateContext GetLightingSky(HDCamera hdCamera)
		{
			if (hdCamera.skyAmbientMode == SkyAmbientMode.Static || (hdCamera.camera.cameraType == CameraType.Reflection && HDRenderPipeline.currentPipeline.reflectionProbeBaking))
			{
				return m_StaticLightingSky;
			}
			return hdCamera.lightingSky;
		}

		internal SphericalHarmonicsL2 GetAmbientProbe(HDCamera hdCamera)
		{
			if (hdCamera.lightingSky == null && hdCamera.skyAmbientMode == SkyAmbientMode.Dynamic)
			{
				return m_BlackAmbientProbe;
			}
			return GetAmbientProbe(GetLightingSky(hdCamera));
		}

		internal ComputeBuffer GetDiffuseAmbientProbeBuffer(HDCamera hdCamera)
		{
			if (hdCamera.lightingSky == null && hdCamera.skyAmbientMode == SkyAmbientMode.Dynamic)
			{
				return m_BlackAmbientProbeBuffer;
			}
			return GetDiffuseAmbientProbeBuffer(GetLightingSky(hdCamera));
		}

		internal ComputeBuffer GetVolumetricAmbientProbeBuffer(HDCamera hdCamera)
		{
			if (hdCamera.lightingSky == null && hdCamera.skyAmbientMode == SkyAmbientMode.Dynamic)
			{
				return m_BlackAmbientProbeBuffer;
			}
			return GetVolumetricAmbientProbeBuffer(GetLightingSky(hdCamera));
		}

		internal bool HasSetValidAmbientProbe(HDCamera hdCamera)
		{
			VisualEnvironment component = hdCamera.volumeStack.GetComponent<VisualEnvironment>();
			if (component.skyAmbientMode.value == SkyAmbientMode.Static)
			{
				return true;
			}
			if (component.skyType.value == 0)
			{
				return true;
			}
			if (hdCamera.skyAmbientMode == SkyAmbientMode.Dynamic && hdCamera.lightingSky != null && hdCamera.lightingSky.IsValid() && IsCachedContextValid(hdCamera.lightingSky))
			{
				return m_CachedSkyContexts[hdCamera.lightingSky.cachedSkyRenderingContextId].renderingContext.ambientProbeIsReady;
			}
			return false;
		}

		internal void SetupAmbientProbe(HDCamera hdCamera)
		{
			RenderSettings.ambientMode = AmbientMode.Custom;
			RenderSettings.ambientProbe = GetAmbientProbe(hdCamera);
			if ((hdCamera.lightingSky != null || hdCamera.skyAmbientMode != SkyAmbientMode.Dynamic) && hdCamera.camera.cameraType != CameraType.Preview)
			{
				bool flag = true;
				m_StandardSkyboxMaterial.SetTexture("_Tex", GetSkyCubemap((hdCamera.skyAmbientMode != SkyAmbientMode.Static && flag) ? hdCamera.lightingSky : m_StaticLightingSky));
				RenderSettings.skybox = m_StandardSkyboxMaterial;
				RenderSettings.ambientIntensity = 1f;
				RenderSettings.ambientMode = AmbientMode.Skybox;
				RenderSettings.reflectionIntensity = 1f;
				RenderSettings.customReflectionTexture = null;
			}
		}

		private void BlitCubemap(CommandBuffer cmd, Cubemap source, RenderTexture dest)
		{
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			for (int i = 0; i < 6; i++)
			{
				CoreUtils.SetRenderTarget(cmd, dest, ClearFlag.None, 0, (CubemapFace)i);
				materialPropertyBlock.SetTexture("_MainTex", source);
				materialPropertyBlock.SetFloat("_faceIndex", i);
				cmd.DrawProcedural(Matrix4x4.identity, m_BlitCubemapMaterial, 0, MeshTopology.Triangles, 3, 1, materialPropertyBlock);
			}
			cmd.GenerateMips(dest);
		}

		private void RenderSkyToCubemap(RenderGraph renderGraph, SkyUpdateContext skyContext, HDCamera hdCamera, TextureHandle cubemap, Matrix4x4[] pixelCoordToViewDir, bool renderBackgroundClouds, HDProfileId profileId)
		{
			RenderSkyToCubemapPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<RenderSkyToCubemapPassData>("RenderSkyToCubemap", out passData, ProfilingSampler.Get(profileId));
			UpdateBuiltinParameters(ref passData.builtinParameters, skyContext, hdCamera, m_CurrentSunLight, m_CurrentDebugDisplaySettings);
			ref CachedSkyContext reference = ref m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId];
			passData.builtinParameters.cloudAmbientProbe = reference.renderingContext.cloudAmbientProbeBuffer;
			passData.skyRenderer = skyContext.skyRenderer;
			passData.cloudRenderer = (renderBackgroundClouds ? skyContext.cloudRenderer : null);
			passData.cameraViewMatrices = m_CameraRelativeViewMatrices;
			passData.facePixelCoordToViewDirMatrices = pixelCoordToViewDir;
			passData.includeSunInBaking = skyContext.skySettings.includeSunInBaking.value;
			passData.output = renderGraphBuilder.WriteTexture(in cubemap);
			renderGraphBuilder.SetRenderFunc(delegate(RenderSkyToCubemapPassData data, RenderGraphContext ctx)
			{
				data.builtinParameters.commandBuffer = ctx.cmd;
				for (int i = 0; i < 6; i++)
				{
					data.builtinParameters.pixelCoordToViewDirMatrix = data.facePixelCoordToViewDirMatrices[i];
					data.builtinParameters.viewMatrix = data.cameraViewMatrices[i];
					data.builtinParameters.colorBuffer = data.output;
					data.builtinParameters.depthBuffer = null;
					data.builtinParameters.cubemapFace = (CubemapFace)i;
					CoreUtils.SetRenderTarget(ctx.cmd, data.output, ClearFlag.None, 0, (CubemapFace)i);
					data.skyRenderer.RenderSky(data.builtinParameters, renderForCubemap: true, data.includeSunInBaking);
					if (data.cloudRenderer != null)
					{
						data.cloudRenderer.RenderClouds(data.builtinParameters, renderForCubemap: true);
					}
				}
			});
		}

		internal void RenderSkyAmbientProbe(RenderGraph renderGraph, SkyUpdateContext skyContext, HDCamera hdCamera, ComputeBuffer probeBuffer, bool renderBackgroundClouds, HDProfileId profileId, float dimmer = 1f, float anisotropy = 0.7f)
		{
			TextureDesc desc = new TextureDesc(m_LowResolution, m_LowResolution)
			{
				slices = TextureXR.slices,
				dimension = TextureDimension.Cube,
				colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
				enableRandomWrite = true
			};
			TextureHandle textureHandle = renderGraph.CreateTexture(in desc);
			RenderSkyToCubemap(renderGraph, skyContext, hdCamera, textureHandle, m_FacePixelCoordToViewDirMatricesLowRes, renderBackgroundClouds, profileId);
			UpdateAmbientProbe(renderGraph, textureHandle, outputForClouds: true, null, null, probeBuffer, new Vector4(dimmer, anisotropy, 0f, 0f), null);
		}

		internal void UpdateAmbientProbe(RenderGraph renderGraph, TextureHandle skyCubemap, bool outputForClouds, ComputeBuffer ambientProbeResult, ComputeBuffer diffuseAmbientProbeResult, ComputeBuffer volumetricAmbientProbeResult, Vector4 fogParameters, Action<AsyncGPUReadbackRequest> callback)
		{
			UpdateAmbientProbePassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<UpdateAmbientProbePassData>("UpdateAmbientProbe", out passData, ProfilingSampler.Get(HDProfileId.UpdateSkyAmbientProbe));
			try
			{
				passData.computeAmbientProbeCS = m_ComputeAmbientProbeCS;
				if (outputForClouds)
				{
					passData.computeAmbientProbeKernel = m_ComputeAmbientProbeCloudsKernel;
				}
				else
				{
					passData.computeAmbientProbeKernel = ((volumetricAmbientProbeResult != null) ? m_ComputeAmbientProbeVolumetricKernel : m_ComputeAmbientProbeKernel);
				}
				passData.skyCubemap = renderGraphBuilder.ReadTexture(in skyCubemap);
				passData.ambientProbeResult = ambientProbeResult;
				passData.diffuseAmbientProbeResult = diffuseAmbientProbeResult;
				UpdateAmbientProbePassData updateAmbientProbePassData = passData;
				ComputeBufferDesc desc = new ComputeBufferDesc(27, 4);
				updateAmbientProbePassData.scratchBuffer = renderGraphBuilder.CreateTransientComputeBuffer(in desc);
				passData.volumetricAmbientProbeResult = volumetricAmbientProbeResult;
				passData.fogParameters = fogParameters;
				passData.callback = callback;
				renderGraphBuilder.SetRenderFunc(delegate(UpdateAmbientProbePassData data, RenderGraphContext ctx)
				{
					if (data.ambientProbeResult != null)
					{
						ctx.cmd.SetComputeBufferParam(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, s_AmbientProbeOutputBufferParam, data.ambientProbeResult);
					}
					ctx.cmd.SetComputeBufferParam(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, s_ScratchBufferParam, data.scratchBuffer);
					ctx.cmd.SetComputeTextureParam(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, s_AmbientProbeInputCubemap, data.skyCubemap);
					if (data.diffuseAmbientProbeResult != null)
					{
						ctx.cmd.SetComputeBufferParam(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, s_DiffuseAmbientProbeOutputBufferParam, data.diffuseAmbientProbeResult);
					}
					if (data.volumetricAmbientProbeResult != null)
					{
						ctx.cmd.SetComputeBufferParam(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, s_VolumetricAmbientProbeOutputBufferParam, data.volumetricAmbientProbeResult);
						ctx.cmd.SetComputeVectorParam(data.computeAmbientProbeCS, s_FogParameters, data.fogParameters);
					}
					Hammersley.BindConstants(ctx.cmd, data.computeAmbientProbeCS);
					ctx.cmd.DispatchCompute(data.computeAmbientProbeCS, data.computeAmbientProbeKernel, 1, 1, 1);
					if (data.ambientProbeResult != null)
					{
						ctx.cmd.RequestAsyncReadback(data.ambientProbeResult, data.callback);
					}
				});
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		private TextureHandle GenerateSkyCubemap(RenderGraph renderGraph, HDCamera hdCamera, SkyUpdateContext skyContext, ComputeBuffer cloudsProbeBuffer)
		{
			SkyRenderingContext renderingContext = m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext;
			TextureHandle textureHandle = renderGraph.ImportTexture(renderingContext.skyboxCubemapRT);
			RenderSkyToCubemap(renderGraph, skyContext, hdCamera, textureHandle, m_FacePixelCoordToViewDirMatrices, renderBackgroundClouds: true, HDProfileId.RenderSkyToCubemap);
			if (skyContext.volumetricClouds != null)
			{
				SetGlobalSkyData(renderGraph, skyContext, m_BuiltinParameters);
				textureHandle = HDRenderPipeline.currentPipeline.RenderVolumetricClouds_Sky(renderGraph, hdCamera, m_FacePixelCoordToViewDirMatrices, skyContext.volumetricClouds, (int)m_BuiltinParameters.screenSize.x, (int)m_BuiltinParameters.screenSize.y, cloudsProbeBuffer, textureHandle);
			}
			HDRenderPipeline.GenerateMipmaps(renderGraph, textureHandle);
			return textureHandle;
		}

		private void RenderCubemapGGXConvolution(RenderGraph renderGraph, TextureHandle input, CubemapArray output)
		{
			SkyEnvironmentConvolutionPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<SkyEnvironmentConvolutionPassData>("UpdateSkyEnvironmentConvolution", out passData, ProfilingSampler.Get(HDProfileId.UpdateSkyEnvironmentConvolution));
			try
			{
				passData.bsdfs = m_IBLFilterArray;
				passData.input = renderGraphBuilder.ReadTexture(in input);
				passData.output = output;
				SkyEnvironmentConvolutionPassData skyEnvironmentConvolutionPassData = passData;
				TextureDesc desc = new TextureDesc(m_Resolution, m_Resolution)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					dimension = TextureDimension.Cube,
					useMipMap = true,
					autoGenerateMips = false,
					filterMode = FilterMode.Trilinear,
					name = "SkyboxBSDFIntermediate"
				};
				skyEnvironmentConvolutionPassData.intermediateTexture = renderGraphBuilder.CreateTransientTexture(in desc);
				renderGraphBuilder.SetRenderFunc(delegate(SkyEnvironmentConvolutionPassData data, RenderGraphContext ctx)
				{
					for (int i = 0; i < data.bsdfs.Length; i++)
					{
						data.bsdfs[i].FilterCubemap(ctx.cmd, data.input, data.intermediateTexture);
						for (int j = 0; j < 6; j++)
						{
							ctx.cmd.CopyTexture(data.intermediateTexture, j, data.output, 6 * i + j);
						}
					}
				});
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		private int GetSunLightHashCode(Light light)
		{
			HDAdditionalLightData component = light.GetComponent<HDAdditionalLightData>();
			int num = 13;
			num = num * 23 + light.transform.position.GetHashCode();
			num = num * 23 + light.transform.rotation.GetHashCode();
			num = num * 23 + light.color.GetHashCode();
			num = num * 23 + light.colorTemperature.GetHashCode();
			num = num * 23 + light.intensity.GetHashCode();
			if (component != null)
			{
				num = num * 23 + component.lightDimmer.GetHashCode();
			}
			return num;
		}

		private void AllocateNewRenderingContext(SkyUpdateContext skyContext, int slot, int newHash, bool supportConvolution, in SphericalHarmonicsL2 previousAmbientProbe, string name)
		{
			ref CachedSkyContext reference = ref m_CachedSkyContexts[slot];
			reference.hash = newHash;
			reference.refCount = 1;
			reference.type = skyContext.skySettings.GetSkyRendererType();
			if (reference.renderingContext != null && reference.renderingContext.supportsConvolution != supportConvolution)
			{
				reference.renderingContext.Cleanup();
				reference.renderingContext = null;
			}
			if (reference.renderingContext == null)
			{
				reference.renderingContext = new SkyRenderingContext(m_Resolution, m_IBLFilterArray.Length, supportConvolution, previousAmbientProbe, name);
			}
			if (skyContext.settingsHadBigDifferenceWithPrev)
			{
				reference.renderingContext.ClearAmbientProbe();
			}
			skyContext.cachedSkyRenderingContextId = slot;
		}

		private bool AcquireSkyRenderingContext(SkyUpdateContext updateContext, int newHash, string name = "", bool supportConvolution = true)
		{
			SphericalHarmonicsL2 previousAmbientProbe = default(SphericalHarmonicsL2);
			if (IsCachedContextValid(updateContext))
			{
				ref CachedSkyContext reference = ref m_CachedSkyContexts[updateContext.cachedSkyRenderingContextId];
				if (newHash == reference.hash && !(updateContext.skySettings.GetSkyRendererType() != reference.type))
				{
					return false;
				}
				if (updateContext.skySettings.GetSkyRendererType() == reference.type)
				{
					previousAmbientProbe = reference.renderingContext.ambientProbe;
				}
				ReleaseCachedContext(updateContext.cachedSkyRenderingContextId);
			}
			int num = -1;
			for (int i = 0; i < m_CachedSkyContexts.size; i++)
			{
				if (m_CachedSkyContexts[i].hash == newHash)
				{
					m_CachedSkyContexts[i].refCount++;
					updateContext.cachedSkyRenderingContextId = i;
					updateContext.skyParametersHash = newHash;
					return false;
				}
				if (num == -1 && m_CachedSkyContexts[i].hash == 0)
				{
					num = i;
				}
			}
			if (name == "")
			{
				name = "SkyboxCubemap";
			}
			if (num != -1)
			{
				AllocateNewRenderingContext(updateContext, num, newHash, supportConvolution, in previousAmbientProbe, name);
			}
			else
			{
				DynamicArray<CachedSkyContext> cachedSkyContexts = m_CachedSkyContexts;
				CachedSkyContext value = default(CachedSkyContext);
				int slot = cachedSkyContexts.Add(in value);
				AllocateNewRenderingContext(updateContext, slot, newHash, supportConvolution, in previousAmbientProbe, name);
			}
			return true;
		}

		internal void ReleaseCachedContext(int id)
		{
			if (id == -1)
			{
				return;
			}
			ref CachedSkyContext reference = ref m_CachedSkyContexts[id];
			if (reference.refCount != 0)
			{
				reference.refCount--;
				if (reference.refCount == 0)
				{
					reference.Reset();
				}
			}
		}

		private bool IsCachedContextValid(SkyUpdateContext skyContext)
		{
			if (skyContext.skySettings == null)
			{
				return false;
			}
			int cachedSkyRenderingContextId = skyContext.cachedSkyRenderingContextId;
			if (cachedSkyRenderingContextId != -1 && skyContext.skySettings.GetSkyRendererType() == m_CachedSkyContexts[cachedSkyRenderingContextId].type)
			{
				return m_CachedSkyContexts[cachedSkyRenderingContextId].hash != 0;
			}
			return false;
		}

		private int ComputeSkyHash(HDCamera camera, SkyUpdateContext skyContext, Light sunLight, SkyAmbientMode ambientMode, bool staticSky = false)
		{
			int num = 0;
			if (sunLight != null && skyContext.skyRenderer.SupportDynamicSunLight)
			{
				num = GetSunLightHashCode(sunLight);
			}
			Camera camera2 = camera.camera;
			if (camera.camera.cameraType == CameraType.Reflection && camera.parentCamera != null)
			{
				camera2 = camera.parentCamera;
			}
			int num2 = num * 23 + skyContext.skySettings.GetHashCode(camera2);
			if (skyContext.HasClouds())
			{
				num2 = num2 * 23 + skyContext.cloudSettings.GetHashCode(camera2);
			}
			if (skyContext.HasVolumetricClouds())
			{
				num2 = num2 * 23 + skyContext.volumetricClouds.GetHashCode();
				num2 = num2 * 23 + camera.frameSettings.IsEnabled(FrameSettingsField.FullResolutionCloudsForSky).GetHashCode();
			}
			num2 = num2 * 23 + (staticSky ? 1 : 0);
			num2 = num2 * 23 + ((ambientMode == SkyAmbientMode.Static) ? 1 : 0);
			if (camera.frameSettings.IsEnabled(FrameSettingsField.Volumetrics))
			{
				Fog component = camera.volumeStack.GetComponent<Fog>();
				num2 = num2 * 23 + component.globalLightProbeDimmer.GetHashCode();
				num2 = num2 * 23 + component.anisotropy.GetHashCode();
			}
			return num2;
		}

		public void RequestEnvironmentUpdate()
		{
			m_UpdateRequired = true;
		}

		internal void RequestStaticEnvironmentUpdate()
		{
			m_StaticSkyUpdateRequired = true;
		}

		private void UpdateEnvironment(RenderGraph renderGraph, HDCamera hdCamera, SkyUpdateContext skyContext, Light sunLight, bool updateRequired, bool updateAmbientProbe, bool staticSky, SkyAmbientMode ambientMode)
		{
			if (skyContext.IsValid())
			{
				using (new RenderGraphProfilingScope(renderGraph, ProfilingSampler.Get(HDProfileId.UpdateEnvironment)))
				{
					skyContext.currentUpdateTime += hdCamera.deltaTime;
					UpdateBuiltinParameters(ref m_BuiltinParameters, skyContext, hdCamera, m_CurrentSunLight, null);
					if (hdCamera.camera.cameraType == CameraType.Reflection && hdCamera.parentCamera != null)
					{
						m_BuiltinParameters.worldSpaceCameraPos = hdCamera.parentCamera.transform.position;
					}
					m_BuiltinParameters.screenSize = m_CubemapScreenSize;
					if (!IsCachedContextValid(skyContext) || updateRequired || (skyContext.skySettings.updateMode.value != EnvironmentUpdateMode.OnDemand && (skyContext.skySettings.updateMode.value != EnvironmentUpdateMode.Realtime || !(skyContext.currentUpdateTime < skyContext.skySettings.updatePeriod.value))))
					{
						int num = ComputeSkyHash(hdCamera, skyContext, sunLight, ambientMode, staticSky);
						bool flag = updateRequired;
						flag |= AcquireSkyRenderingContext(skyContext, num, staticSky ? "SkyboxCubemap_Static" : "SkyboxCubemap", !staticSky);
						SkyRenderingContext renderingContext = m_CachedSkyContexts[skyContext.cachedSkyRenderingContextId].renderingContext;
						if (IsCachedContextValid(skyContext))
						{
							flag |= skyContext.skyRenderer.DoUpdate(m_BuiltinParameters);
							flag |= skyContext.HasClouds() && skyContext.cloudRenderer.DoUpdate(m_BuiltinParameters);
						}
						flag |= skyContext.skySettings.updateMode.value == EnvironmentUpdateMode.OnChanged && num != skyContext.skyParametersHash;
						flag |= skyContext.skySettings.updateMode.value == EnvironmentUpdateMode.Realtime && skyContext.currentUpdateTime > skyContext.skySettings.updatePeriod.value;
						if (flag && skyContext.cloudRenderer != null)
						{
							RenderSkyAmbientProbe(renderGraph, skyContext, hdCamera, renderingContext.cloudAmbientProbeBuffer, renderBackgroundClouds: false, HDProfileId.BackgroundCloudsAmbientProbe);
						}
						ComputeBuffer cloudsProbeBuffer = HDRenderPipeline.currentPipeline.RenderVolumetricCloudsAmbientProbe(renderGraph, hdCamera, skyContext, staticSky);
						if (flag)
						{
							TextureHandle textureHandle = GenerateSkyCubemap(renderGraph, hdCamera, skyContext, cloudsProbeBuffer);
							if (updateAmbientProbe)
							{
								Fog component = hdCamera.volumeStack.GetComponent<Fog>();
								UpdateAmbientProbe(renderGraph, textureHandle, outputForClouds: false, renderingContext.ambientProbeResult, renderingContext.diffuseAmbientProbeBuffer, renderingContext.volumetricAmbientProbeBuffer, new Vector4(component.globalLightProbeDimmer.value, component.anisotropy.value, 0f, 0f), renderingContext.OnComputeAmbientProbeDone);
							}
							if (renderingContext.supportsConvolution)
							{
								RenderCubemapGGXConvolution(renderGraph, textureHandle, renderingContext.skyboxBSDFCubemapArray);
							}
							skyContext.skyParametersHash = num;
							skyContext.currentUpdateTime = 0f;
						}
					}
					return;
				}
			}
			if (skyContext.cachedSkyRenderingContextId != -1)
			{
				ReleaseCachedContext(skyContext.cachedSkyRenderingContextId);
				skyContext.cachedSkyRenderingContextId = -1;
			}
		}

		public void UpdateEnvironment(RenderGraph renderGraph, HDCamera hdCamera, Light sunLight, DebugDisplaySettings debugSettings)
		{
			m_CurrentDebugDisplaySettings = debugSettings;
			m_CurrentSunLight = sunLight;
			SkyAmbientMode value = hdCamera.volumeStack.GetComponent<VisualEnvironment>().skyAmbientMode.value;
			UpdateEnvironment(renderGraph, hdCamera, hdCamera.lightingSky, sunLight, m_UpdateRequired, value == SkyAmbientMode.Dynamic, staticSky: false, value);
			bool flag = false;
			StaticLightingSky staticLightingSky = GetStaticLightingSky();
			if ((value == SkyAmbientMode.Static || flag) && hdCamera.camera.cameraType != CameraType.Preview)
			{
				m_StaticLightingSky.skySettings = ((staticLightingSky != null) ? staticLightingSky.skySettings : null);
				m_StaticLightingSky.cloudSettings = ((staticLightingSky != null) ? staticLightingSky.cloudSettings : null);
				m_StaticLightingSky.volumetricClouds = ((staticLightingSky != null) ? staticLightingSky.volumetricClouds : null);
				UpdateEnvironment(renderGraph, hdCamera, m_StaticLightingSky, sunLight, m_StaticSkyUpdateRequired || m_UpdateRequired, updateAmbientProbe: true, staticSky: true, SkyAmbientMode.Static);
				m_StaticSkyUpdateRequired = false;
			}
			m_UpdateRequired = false;
			SetGlobalSkyData(renderGraph, hdCamera.lightingSky, m_BuiltinParameters);
			HDRenderPipeline.SetGlobalTexture(renderGraph, HDShaderIDs._SkyTexture, GetReflectionTexture(hdCamera.lightingSky));
			HDRenderPipeline.SetGlobalBuffer(renderGraph, HDShaderIDs._AmbientProbeData, GetDiffuseAmbientProbeBuffer(hdCamera));
		}

		private static void UpdateBuiltinParameters(ref BuiltinSkyParameters builtinParameters, SkyUpdateContext skyContext, HDCamera hdCamera, Light sunLight, DebugDisplaySettings debugSettings)
		{
			builtinParameters.hdCamera = hdCamera;
			builtinParameters.sunLight = sunLight;
			builtinParameters.pixelCoordToViewDirMatrix = hdCamera.mainViewConstants.pixelCoordToViewDirWS;
			builtinParameters.worldSpaceCameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
			builtinParameters.viewMatrix = hdCamera.mainViewConstants.viewMatrix;
			builtinParameters.screenSize = hdCamera.screenSize;
			builtinParameters.debugSettings = debugSettings;
			builtinParameters.frameIndex = (int)hdCamera.GetCameraFrameCount();
			builtinParameters.skySettings = skyContext.skySettings;
			builtinParameters.cloudSettings = skyContext.cloudSettings;
			builtinParameters.volumetricClouds = skyContext.volumetricClouds;
			builtinParameters.commandBuffer = null;
			builtinParameters.colorBuffer = null;
			builtinParameters.depthBuffer = null;
		}

		public bool TryGetCloudSettings(HDCamera hdCamera, out CloudSettings cloudSettings, out CloudRenderer cloudRenderer)
		{
			SkyUpdateContext visualSky = hdCamera.visualSky;
			cloudSettings = visualSky.cloudSettings;
			cloudRenderer = visualSky.cloudRenderer;
			return visualSky.HasClouds();
		}

		private bool RequiresPreRenderSky(HDCamera hdCamera)
		{
			SkyUpdateContext visualSky = hdCamera.visualSky;
			if (visualSky.IsValid())
			{
				if (!visualSky.skyRenderer.RequiresPreRender(visualSky.skySettings))
				{
					if (visualSky.HasClouds())
					{
						return visualSky.cloudRenderer.RequiresPreRenderClouds(m_BuiltinParameters);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public void PreRenderSky(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle normalBuffer, TextureHandle depthBuffer)
		{
			SkyUpdateContext visualSky = hdCamera.visualSky;
			if (!visualSky.IsValid() || !RequiresPreRenderSky(hdCamera))
			{
				return;
			}
			RenderSkyPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<RenderSkyPassData>("Pre Render Sky", out passData, ProfilingSampler.Get(HDProfileId.PreRenderSky));
			passData.colorBuffer = renderGraphBuilder.WriteTexture(in normalBuffer);
			passData.depthBuffer = renderGraphBuilder.WriteTexture(in depthBuffer);
			passData.skyContext = visualSky;
			passData.renderSunDisk = hdCamera.camera.cameraType != CameraType.Reflection || visualSky.skySettings.includeSunInBaking.value;
			UpdateBuiltinParameters(ref passData.builtinParameters, visualSky, hdCamera, m_CurrentSunLight, m_CurrentDebugDisplaySettings);
			renderGraphBuilder.SetRenderFunc(delegate(RenderSkyPassData data, RenderGraphContext ctx)
			{
				data.builtinParameters.colorBuffer = data.colorBuffer;
				data.builtinParameters.depthBuffer = data.depthBuffer;
				data.builtinParameters.commandBuffer = ctx.cmd;
				CoreUtils.SetRenderTarget(ctx.cmd, data.colorBuffer, data.depthBuffer);
				if (data.skyContext.skyRenderer.RequiresPreRender(data.skyContext.skySettings))
				{
					data.skyContext.skyRenderer.DoUpdate(data.builtinParameters);
					data.skyContext.skyRenderer.PreRenderSky(data.builtinParameters);
				}
				if (data.skyContext.HasClouds() && data.skyContext.cloudRenderer.RequiresPreRenderClouds(data.builtinParameters))
				{
					data.skyContext.cloudRenderer.DoUpdate(data.builtinParameters);
					data.skyContext.cloudRenderer.PreRenderClouds(data.builtinParameters, renderForCubemap: false);
				}
			});
		}

		public void RenderSky(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle colorBuffer, TextureHandle depthBuffer, string passName, ProfilingSampler sampler = null)
		{
			if (hdCamera.clearColorMode != 0 || m_CurrentDebugDisplaySettings.data.lightingDebugSettings.debugLightingMode == DebugLightingMode.LuxMeter)
			{
				return;
			}
			SkyUpdateContext visualSky = hdCamera.visualSky;
			if (!visualSky.IsValid())
			{
				return;
			}
			RenderSkyPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<RenderSkyPassData>("Render Sky", out passData, sampler);
			passData.colorBuffer = renderGraphBuilder.WriteTexture(in colorBuffer);
			passData.depthBuffer = renderGraphBuilder.WriteTexture(in depthBuffer);
			if (LensFlareCommonSRP.IsCloudLayerOpacityNeeded(hdCamera.camera))
			{
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R8_UNorm,
					clearBuffer = true,
					clearColor = Color.black,
					name = "Cloud Occlusion"
				};
				TextureHandle input = renderGraph.CreateTexture(in desc);
				m_CloudOpacity = renderGraphBuilder.WriteTexture(in input);
			}
			else
			{
				m_CloudOpacity = TextureHandle.nullHandle;
			}
			passData.skyContext = visualSky;
			bool flag = false;
			if (passData.skyContext.HasClouds())
			{
				CloudLayer cloudLayer = passData.skyContext.cloudSettings as CloudLayer;
				if ((bool)cloudLayer)
				{
					flag = cloudLayer.active && cloudLayer.opacity.value > 0f;
				}
			}
			if (flag && LensFlareCommonSRP.IsCloudLayerOpacityNeeded(hdCamera.camera))
			{
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R8_UNorm,
					clearBuffer = true,
					clearColor = Color.black,
					name = "Cloud Occlusion"
				};
				TextureHandle input2 = renderGraph.CreateTexture(in desc);
				m_CloudOpacity = renderGraphBuilder.WriteTexture(in input2);
			}
			else
			{
				m_CloudOpacity = TextureHandle.nullHandle;
			}
			passData.renderSunDisk = hdCamera.camera.cameraType != CameraType.Reflection || visualSky.skySettings.includeSunInBaking.value;
			UpdateBuiltinParameters(ref passData.builtinParameters, visualSky, hdCamera, m_CurrentSunLight, m_CurrentDebugDisplaySettings);
			passData.cloudOpacityBuffer = m_CloudOpacity;
			if (visualSky.HasClouds())
			{
				ref CachedSkyContext reference = ref m_CachedSkyContexts[visualSky.cachedSkyRenderingContextId];
				passData.builtinParameters.cloudAmbientProbe = reference.renderingContext.cloudAmbientProbeBuffer;
			}
			renderGraphBuilder.SetRenderFunc(delegate(RenderSkyPassData data, RenderGraphContext ctx)
			{
				data.builtinParameters.colorBuffer = data.colorBuffer;
				data.builtinParameters.depthBuffer = data.depthBuffer;
				data.builtinParameters.cloudOpacity = data.cloudOpacityBuffer;
				data.builtinParameters.commandBuffer = ctx.cmd;
				CoreUtils.SetRenderTarget(ctx.cmd, data.colorBuffer, data.depthBuffer);
				data.skyContext.skyRenderer.DoUpdate(data.builtinParameters);
				data.skyContext.skyRenderer.RenderSky(data.builtinParameters, renderForCubemap: false, data.renderSunDisk);
				if (data.skyContext.HasClouds())
				{
					using (new ProfilingScope(ctx.cmd, ProfilingSampler.Get(HDProfileId.RenderClouds)))
					{
						data.skyContext.cloudRenderer.DoUpdate(data.builtinParameters);
						data.skyContext.cloudRenderer.RenderClouds(data.builtinParameters, renderForCubemap: false);
					}
				}
			});
		}

		public void RenderOpaqueAtmosphericScattering(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle colorBuffer, TextureHandle depthTexture, TextureHandle volumetricLighting, TextureHandle depthBuffer)
		{
			if (!Fog.IsFogEnabled(hdCamera) && !Fog.IsPBRFogEnabled(hdCamera))
			{
				return;
			}
			OpaqueAtmosphericScatteringPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<OpaqueAtmosphericScatteringPassData>("Opaque Atmospheric Scattering", out passData, ProfilingSampler.Get(HDProfileId.OpaqueAtmosphericScattering));
			passData.opaqueAtmosphericalScatteringMaterial = m_OpaqueAtmScatteringMaterial;
			passData.msaa = hdCamera.msaaEnabled;
			passData.pbrFog = Fog.IsPBRFogEnabled(hdCamera);
			passData.pixelCoordToViewDirWS = hdCamera.mainViewConstants.pixelCoordToViewDirWS;
			if (volumetricLighting.IsValid())
			{
				passData.volumetricLighting = renderGraphBuilder.ReadTexture(in volumetricLighting);
			}
			else
			{
				passData.volumetricLighting = TextureHandle.nullHandle;
			}
			passData.colorBuffer = renderGraphBuilder.WriteTexture(in colorBuffer);
			passData.depthTexture = renderGraphBuilder.ReadTexture(in depthTexture);
			passData.depthBuffer = renderGraphBuilder.ReadTexture(in depthBuffer);
			if (Fog.IsPBRFogEnabled(hdCamera))
			{
				passData.intermediateTexture = renderGraphBuilder.CreateTransientTexture(in colorBuffer);
			}
			renderGraphBuilder.SetRenderFunc(delegate(OpaqueAtmosphericScatteringPassData data, RenderGraphContext ctx)
			{
				MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
				tempMaterialPropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, data.pixelCoordToViewDirWS);
				tempMaterialPropertyBlock.SetTexture(data.msaa ? HDShaderIDs._DepthTextureMS : HDShaderIDs._CameraDepthTexture, data.depthTexture);
				if (data.volumetricLighting.IsValid())
				{
					tempMaterialPropertyBlock.SetTexture(HDShaderIDs._VBufferLighting, data.volumetricLighting);
				}
				if (data.pbrFog)
				{
					tempMaterialPropertyBlock.SetTexture(data.msaa ? HDShaderIDs._ColorTextureMS : HDShaderIDs._ColorTexture, data.colorBuffer);
					HDUtils.DrawFullScreen(ctx.cmd, data.opaqueAtmosphericalScatteringMaterial, data.intermediateTexture, data.depthBuffer, tempMaterialPropertyBlock, data.msaa ? 3 : 2);
					ctx.cmd.CopyTexture(data.intermediateTexture, data.colorBuffer);
				}
				else
				{
					HDUtils.DrawFullScreen(ctx.cmd, data.opaqueAtmosphericalScatteringMaterial, data.colorBuffer, data.depthBuffer, tempMaterialPropertyBlock, data.msaa ? 1 : 0);
				}
			});
		}

		public static StaticLightingSky GetStaticLightingSky()
		{
			if (m_StaticLightingSkies.Count == 0)
			{
				return null;
			}
			return m_StaticLightingSkies[m_StaticLightingSkies.Count - 1];
		}

		public static void RegisterStaticLightingSky(StaticLightingSky staticLightingSky)
		{
			if (!m_StaticLightingSkies.Contains(staticLightingSky))
			{
				if (m_StaticLightingSkies.Count != 0)
				{
					Debug.LogWarning("One Static Lighting Sky component was already set for baking, only the latest one will be used.");
				}
				if (staticLightingSky.staticLightingSkyUniqueID == 2 && !skyTypesDict.TryGetValue(2, out var _))
				{
					Debug.LogError("You are using the deprecated Procedural Sky for static lighting in your Scene. You can still use it but, to do so, you must install it separately. To do this, open the Package Manager window and import the 'Procedural Sky' sample from the HDRP package page, then close and re-open your project without saving.");
				}
				else
				{
					m_StaticLightingSkies.Add(staticLightingSky);
				}
			}
		}

		public static void UnRegisterStaticLightingSky(StaticLightingSky staticLightingSky)
		{
			m_StaticLightingSkies.Remove(staticLightingSky);
		}

		public Texture2D ExportSkyToTexture(Camera camera)
		{
			HDCamera orCreate = HDCamera.GetOrCreate(camera);
			if (!orCreate.visualSky.IsValid() || !IsCachedContextValid(orCreate.visualSky))
			{
				Debug.LogError("Cannot export sky to a texture, no valid Sky is setup (Also make sure the game view has been rendered at least once).");
				return null;
			}
			RenderTexture renderTexture = m_CachedSkyContexts[orCreate.visualSky.cachedSkyRenderingContextId].renderingContext.skyboxCubemapRT;
			int width = renderTexture.width;
			RenderTexture renderTexture2 = new RenderTexture(width * 6, width, 0, GraphicsFormat.R16G16B16A16_SFloat)
			{
				dimension = TextureDimension.Tex2D,
				useMipMap = false,
				autoGenerateMips = false,
				filterMode = FilterMode.Trilinear
			};
			renderTexture2.Create();
			Texture2D texture2D = new Texture2D(width * 6, width, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
			Texture2D texture2D2 = new Texture2D(width * 6, width, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
			int num = 0;
			for (int i = 0; i < 6; i++)
			{
				Graphics.SetRenderTarget(renderTexture, 0, (CubemapFace)i);
				texture2D.ReadPixels(new Rect(0f, 0f, width, width), num, 0);
				texture2D.Apply();
				num += width;
			}
			Graphics.Blit(texture2D, renderTexture2, new Vector2(1f, -1f), new Vector2(0f, 0f));
			texture2D2.ReadPixels(new Rect(0f, 0f, width * 6, width), 0, 0);
			texture2D2.Apply();
			Graphics.SetRenderTarget(null);
			CoreUtils.Destroy(texture2D);
			CoreUtils.Destroy(renderTexture2);
			return texture2D2;
		}
	}
}
