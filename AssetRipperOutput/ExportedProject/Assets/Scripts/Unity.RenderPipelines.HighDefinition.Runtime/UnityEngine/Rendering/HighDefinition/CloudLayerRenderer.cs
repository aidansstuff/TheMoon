using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class CloudLayerRenderer : CloudRenderer
	{
		private class PrecomputationCache
		{
			private class RefCountedData
			{
				public int refCount;

				public PrecomputationData data = new PrecomputationData();
			}

			private ObjectPool<RefCountedData> m_DataPool = new ObjectPool<RefCountedData>(null, null);

			private Dictionary<int, RefCountedData> m_CachedData = new Dictionary<int, RefCountedData>();

			public PrecomputationData Get(CloudLayer cloudLayer, int currentHash)
			{
				if (m_CachedData.TryGetValue(currentHash, out var value))
				{
					value.refCount++;
					return value.data;
				}
				value = m_DataPool.Get();
				value.refCount = 1;
				value.data.Allocate(cloudLayer);
				m_CachedData.Add(currentHash, value);
				return value.data;
			}

			public void Release(int hash)
			{
				if (m_CachedData.TryGetValue(hash, out var value))
				{
					value.refCount--;
					if (value.refCount == 0)
					{
						value.data.Release();
						m_CachedData.Remove(hash);
						m_DataPool.Release(value);
					}
				}
			}
		}

		private class PrecomputationData
		{
			private struct TextureCache
			{
				private int width;

				private int height;

				private RTHandle rt;

				public bool TryGet(int textureWidth, int textureHeight, ref RTHandle texture)
				{
					if (rt == null || textureWidth != width || textureHeight != height)
					{
						return false;
					}
					texture = rt;
					rt = null;
					return true;
				}

				public void Cache(int textureWidth, int textureHeight, RTHandle texture)
				{
					if (texture != null)
					{
						if (rt != null)
						{
							RTHandles.Release(rt);
						}
						width = textureWidth;
						height = textureHeight;
						rt = texture;
					}
				}
			}

			private static TextureCache cloudTextureCache;

			private static TextureCache cloudShadowsCache;

			private bool initialized;

			private int cloudTextureWidth;

			private int cloudTextureHeight;

			private int cloudShadowsResolution;

			public RTHandle cloudTextureRT;

			public RTHandle cloudShadowsRT;

			public void Allocate(CloudLayer cloudLayer)
			{
				initialized = false;
				cloudTextureWidth = (int)cloudLayer.resolution.value;
				cloudTextureHeight = (cloudLayer.upperHemisphereOnly.value ? (cloudTextureWidth / 2) : cloudTextureWidth);
				if (!cloudTextureCache.TryGet(cloudTextureWidth, cloudTextureHeight, ref cloudTextureRT))
				{
					cloudTextureRT = RTHandles.Alloc(cloudTextureWidth, cloudTextureHeight, TextureWrapMode.Repeat, TextureWrapMode.Clamp, TextureWrapMode.Repeat, cloudLayer.NumLayers, DepthBits.None, GraphicsFormat.R16G16_SFloat, FilterMode.Bilinear, TextureDimension.Tex2DArray, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, "Cloud Texture");
				}
				cloudShadowsRT = null;
				cloudShadowsResolution = (int)cloudLayer.shadowResolution.value;
				if (cloudLayer.CastShadows && !cloudShadowsCache.TryGet(cloudShadowsResolution, cloudShadowsResolution, ref cloudShadowsRT))
				{
					cloudShadowsRT = RTHandles.Alloc(cloudShadowsResolution, cloudShadowsResolution, 1, DepthBits.None, GraphicsFormat.B10G11R11_UFloatPack32, FilterMode.Bilinear, TextureWrapMode.Repeat, TextureDimension.Tex2D, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, "Cloud Shadows");
				}
			}

			public void Release()
			{
				cloudTextureCache.Cache(cloudTextureHeight, cloudTextureHeight, cloudTextureRT);
				cloudShadowsCache.Cache(cloudShadowsResolution, cloudShadowsResolution, cloudShadowsRT);
			}

			public bool InitIfNeeded(CloudLayer cloudLayer, Light sunLight, CommandBuffer cmd)
			{
				if (initialized)
				{
					return false;
				}
				Vector4 val = ((sunLight == null) ? Vector3.zero : (-sunLight.transform.forward));
				val.w = (cloudLayer.upperHemisphereOnly.value ? 1f : 0f);
				cmd.SetComputeVectorParam(s_BakeCloudTextureCS, HDShaderIDs._Params, val);
				cmd.SetComputeTextureParam(s_BakeCloudTextureCS, s_BakeCloudTextureKernel, _CloudTexture, cloudTextureRT);
				cmd.SetComputeTextureParam(s_BakeCloudTextureCS, s_BakeCloudTextureKernel, _CloudMapA, cloudLayer.layerA.cloudMap.value);
				(Vector4, Vector4) bakingParameters = cloudLayer.layerA.GetBakingParameters();
				if (cloudLayer.NumLayers == 1)
				{
					s_BakeCloudTextureCS.DisableKeyword("USE_SECOND_CLOUD_LAYER");
					cmd.SetComputeVectorParam(s_BakeCloudTextureCS, HDShaderIDs._Params1, bakingParameters.Item1);
					cmd.SetComputeVectorParam(s_BakeCloudTextureCS, HDShaderIDs._Params2, bakingParameters.Item2);
				}
				else
				{
					cmd.SetComputeTextureParam(s_BakeCloudTextureCS, s_BakeCloudTextureKernel, _CloudMapB, cloudLayer.layerB.cloudMap.value);
					(Vector4, Vector4) bakingParameters2 = cloudLayer.layerB.GetBakingParameters();
					s_BakeCloudTextureCS.EnableKeyword("USE_SECOND_CLOUD_LAYER");
					s_VectorArray[0] = bakingParameters.Item1;
					s_VectorArray[1] = bakingParameters2.Item1;
					cmd.SetComputeVectorArrayParam(s_BakeCloudTextureCS, HDShaderIDs._Params1, s_VectorArray);
					s_VectorArray[0] = bakingParameters.Item2;
					s_VectorArray[1] = bakingParameters2.Item2;
					cmd.SetComputeVectorArrayParam(s_BakeCloudTextureCS, HDShaderIDs._Params2, s_VectorArray);
				}
				cmd.SetComputeFloatParam(s_BakeCloudTextureCS, HDShaderIDs._Resolution, 1f / (float)cloudTextureWidth);
				int threadGroupsX = (cloudTextureWidth + 7) / 8;
				int threadGroupsY = (cloudTextureHeight + 7) / 8;
				cmd.DispatchCompute(s_BakeCloudTextureCS, s_BakeCloudTextureKernel, threadGroupsX, threadGroupsY, 1);
				initialized = true;
				return true;
			}

			public void BakeCloudShadows(CloudLayer cloudLayer, Light sunLight, HDCamera hdCamera, CommandBuffer cmd)
			{
				InitIfNeeded(cloudLayer, sunLight, cmd);
				Vector4 val = cloudLayer.shadowTint.value;
				val.w = cloudLayer.shadowMultiplier.value * 8f;
				cmd.SetComputeFloatParam(s_BakeCloudShadowsCS, HDShaderIDs._Resolution, 1f / (float)cloudShadowsResolution);
				cmd.SetComputeVectorParam(s_BakeCloudShadowsCS, HDShaderIDs._Params, val);
				cmd.SetComputeTextureParam(s_BakeCloudShadowsCS, s_BakeCloudShadowsKernel, _CloudTexture, cloudTextureRT);
				cmd.SetComputeTextureParam(s_BakeCloudShadowsCS, s_BakeCloudShadowsKernel, _CloudShadows, cloudShadowsRT);
				Vector4 renderingParameters = cloudLayer.layerA.GetRenderingParameters(hdCamera);
				Vector4 renderingParameters2 = cloudLayer.layerB.GetRenderingParameters(hdCamera);
				renderingParameters.w = (cloudLayer.upperHemisphereOnly.value ? 1 : 0);
				renderingParameters2.w = cloudLayer.opacity.value;
				s_VectorArray[0] = renderingParameters;
				s_VectorArray[1] = renderingParameters2;
				cmd.SetComputeVectorArrayParam(s_BakeCloudShadowsCS, HDShaderIDs._FlowmapParam, s_VectorArray);
				s_VectorArray[0] = sunLight.transform.right;
				s_VectorArray[1] = sunLight.transform.up;
				s_VectorArray[0].w = cloudLayer.layerA.altitude.value;
				s_VectorArray[1].w = cloudLayer.layerB.altitude.value;
				cmd.SetComputeVectorArrayParam(s_BakeCloudShadowsCS, HDShaderIDs._Params1, s_VectorArray);
				cmd.SetComputeVectorParam(s_BakeCloudShadowsCS, HDShaderIDs._SunDirection, -sunLight.transform.forward);
				cmd.SetComputeTextureParam(s_BakeCloudShadowsCS, s_BakeCloudShadowsKernel, _FlowmapA, cloudLayer.layerA.flowmap.value);
				cmd.SetComputeTextureParam(s_BakeCloudShadowsCS, s_BakeCloudShadowsKernel, _FlowmapB, cloudLayer.layerB.flowmap.value);
				bool value = cloudLayer.layerA.castShadows.value;
				CloudDistortionMode value2 = cloudLayer.layerA.distortionMode.value;
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER1_OFF", !value);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER1_STATIC", value && value2 == CloudDistortionMode.None);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER1_PROCEDURAL", value && value2 == CloudDistortionMode.Procedural);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER1_FLOWMAP", value && value2 == CloudDistortionMode.Flowmap);
				bool flag = cloudLayer.layers.value == CloudMapMode.Double && cloudLayer.layerB.castShadows.value;
				CloudDistortionMode value3 = cloudLayer.layerB.distortionMode.value;
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER2_OFF", !flag);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER2_STATIC", flag && value3 == CloudDistortionMode.None);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER2_PROCEDURAL", flag && value3 == CloudDistortionMode.Procedural);
				CoreUtils.SetKeyword(s_BakeCloudShadowsCS, "LAYER2_FLOWMAP", flag && value3 == CloudDistortionMode.Flowmap);
				int threadGroupsX = (cloudShadowsResolution + 7) / 8;
				int threadGroupsY = (cloudShadowsResolution + 7) / 8;
				cmd.DispatchCompute(s_BakeCloudShadowsCS, s_BakeCloudShadowsKernel, threadGroupsX, threadGroupsY, 1);
				cloudShadowsRT.rt.IncrementUpdateCount();
			}
		}

		private Material m_CloudLayerMaterial;

		private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();

		private float lastTime;

		private int m_LastPrecomputationParamHash;

		private static readonly int _CloudTexture = Shader.PropertyToID("_CloudTexture");

		private static readonly int _CloudShadows = Shader.PropertyToID("_CloudShadows");

		private static readonly int _FlowmapA = Shader.PropertyToID("_FlowmapA");

		private static readonly int _FlowmapB = Shader.PropertyToID("_FlowmapB");

		private static readonly int _CloudMapA = Shader.PropertyToID("_CloudMapA");

		private static readonly int _CloudMapB = Shader.PropertyToID("_CloudMapB");

		private static readonly int _AmbientProbeBuffer = Shader.PropertyToID("_AmbientProbeBuffer");

		private static ComputeShader s_BakeCloudTextureCS;

		private static ComputeShader s_BakeCloudShadowsCS;

		private static int s_BakeCloudTextureKernel;

		private static int s_BakeCloudShadowsKernel;

		private static readonly Vector4[] s_VectorArray = new Vector4[2];

		public RTHandle fullscreenOpacity;

		public RenderTargetIdentifier[] mrtToRenderCloudOcclusion = new RenderTargetIdentifier[2];

		private static PrecomputationCache s_PrecomputationCache = new PrecomputationCache();

		private PrecomputationData m_PrecomputedData;

		public RTHandle cloudTexture => m_PrecomputedData.cloudTextureRT;

		public override void Build()
		{
			HDRenderPipelineGlobalSettings instance = HDRenderPipelineGlobalSettings.instance;
			m_CloudLayerMaterial = CoreUtils.CreateEngineMaterial(instance.renderPipelineResources.shaders.cloudLayerPS);
			s_BakeCloudTextureCS = instance.renderPipelineResources.shaders.bakeCloudTextureCS;
			s_BakeCloudTextureKernel = s_BakeCloudTextureCS.FindKernel("BakeCloudTexture");
			s_BakeCloudShadowsCS = instance.renderPipelineResources.shaders.bakeCloudShadowsCS;
			s_BakeCloudShadowsKernel = s_BakeCloudShadowsCS.FindKernel("BakeCloudShadows");
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_CloudLayerMaterial);
			if (m_PrecomputedData != null)
			{
				s_PrecomputationCache.Release(m_LastPrecomputationParamHash);
				m_LastPrecomputationParamHash = 0;
				m_PrecomputedData = null;
			}
		}

		private bool UpdateCache(CloudLayer cloudLayer, Light sunLight)
		{
			int bakingHashCode = cloudLayer.GetBakingHashCode(sunLight);
			if (bakingHashCode != m_LastPrecomputationParamHash)
			{
				s_PrecomputationCache.Release(m_LastPrecomputationParamHash);
				m_PrecomputedData = s_PrecomputationCache.Get(cloudLayer, bakingHashCode);
				m_LastPrecomputationParamHash = bakingHashCode;
				return true;
			}
			return false;
		}

		protected override bool Update(BuiltinSkyParameters builtinParams)
		{
			return UpdateCache(builtinParams.cloudSettings as CloudLayer, builtinParams.sunLight);
		}

		public override bool GetSunLightCookieParameters(CloudSettings settings, ref CookieParameters cookieParams)
		{
			CloudLayer cloudLayer = (CloudLayer)settings;
			if (cloudLayer.CastShadows)
			{
				if (m_PrecomputedData == null || m_PrecomputedData.cloudShadowsRT == null)
				{
					UpdateCache(cloudLayer, HDRenderPipeline.currentPipeline.GetMainLight());
				}
				cookieParams.texture = m_PrecomputedData.cloudShadowsRT;
				cookieParams.size = new Vector2(cloudLayer.shadowSize.value, cloudLayer.shadowSize.value);
				return true;
			}
			return false;
		}

		public override void RenderSunLightCookie(BuiltinSunCookieParameters builtinParams)
		{
			m_PrecomputedData.BakeCloudShadows((CloudLayer)builtinParams.cloudSettings, builtinParams.sunLight, builtinParams.hdCamera, builtinParams.commandBuffer);
		}

		public override void RenderClouds(BuiltinSkyParameters builtinParams, bool renderForCubemap)
		{
			HDCamera hdCamera = builtinParams.hdCamera;
			CommandBuffer commandBuffer = builtinParams.commandBuffer;
			CloudLayer cloudLayer = builtinParams.cloudSettings as CloudLayer;
			if (cloudLayer.opacity.value == 0f)
			{
				return;
			}
			float num = (hdCamera.animateMaterials ? (hdCamera.time - lastTime) : 0f);
			lastTime = hdCamera.time;
			if (!hdCamera.animateMaterials)
			{
				cloudLayer.layerA.scrollFactor = (cloudLayer.layerB.scrollFactor = 0f);
			}
			m_PrecomputedData.InitIfNeeded(cloudLayer, builtinParams.sunLight, builtinParams.commandBuffer);
			m_CloudLayerMaterial.SetTexture(_CloudTexture, m_PrecomputedData.cloudTextureRT);
			Vector4 renderingParameters = cloudLayer.layerA.GetRenderingParameters(hdCamera);
			Vector4 renderingParameters2 = cloudLayer.layerB.GetRenderingParameters(hdCamera);
			renderingParameters.w = (cloudLayer.upperHemisphereOnly.value ? 1 : 0);
			renderingParameters2.w = cloudLayer.opacity.value;
			s_VectorArray[0] = renderingParameters;
			s_VectorArray[1] = renderingParameters2;
			m_CloudLayerMaterial.SetVectorArray(HDShaderIDs._FlowmapParam, s_VectorArray);
			if (cloudLayer.layerA.distortionMode.value != 0)
			{
				cloudLayer.layerA.scrollFactor += cloudLayer.layerA.scrollSpeed.GetValue(hdCamera) * num * 0.277778f;
				if (cloudLayer.layerA.distortionMode.value == CloudDistortionMode.Flowmap)
				{
					m_CloudLayerMaterial.SetTexture(_FlowmapA, cloudLayer.layerA.flowmap.value);
				}
			}
			if (cloudLayer.layerB.distortionMode.value != 0 && cloudLayer.layers.value == CloudMapMode.Double)
			{
				cloudLayer.layerB.scrollFactor += cloudLayer.layerB.scrollSpeed.GetValue(hdCamera) * num * 0.277778f;
				if (cloudLayer.layerB.distortionMode.value == CloudDistortionMode.Flowmap)
				{
					m_CloudLayerMaterial.SetTexture(_FlowmapB, cloudLayer.layerB.flowmap.value);
				}
			}
			Color color = Color.black;
			if (builtinParams.sunLight != null)
			{
				m_CloudLayerMaterial.SetVector(HDShaderIDs._SunDirection, -builtinParams.sunLight.transform.forward);
				Light component = builtinParams.sunLight.GetComponent<Light>();
				HDAdditionalLightData component2 = builtinParams.sunLight.GetComponent<HDAdditionalLightData>();
				color = component.color.linear * component.intensity;
				if (component2.useColorTemperature)
				{
					color *= Mathf.CorrelatedColorTemperatureToRGB(component.colorTemperature);
				}
			}
			s_VectorArray[0] = cloudLayer.layerA.Color * color;
			s_VectorArray[1] = cloudLayer.layerB.Color * color;
			s_VectorArray[0].w = cloudLayer.layerA.altitude.value;
			s_VectorArray[1].w = cloudLayer.layerB.altitude.value;
			m_CloudLayerMaterial.SetVectorArray(HDShaderIDs._Params1, s_VectorArray);
			Vector4 value = new Vector4(cloudLayer.layerA.ambientProbeDimmer.value, cloudLayer.layerB.ambientProbeDimmer.value, 0f, 0f);
			m_CloudLayerMaterial.SetVector(HDShaderIDs._Params2, value);
			m_CloudLayerMaterial.SetBuffer(_AmbientProbeBuffer, builtinParams.cloudAmbientProbe);
			CloudDistortionMode value2 = cloudLayer.layerA.distortionMode.value;
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER1_STATIC", value2 == CloudDistortionMode.None);
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER1_PROCEDURAL", value2 == CloudDistortionMode.Procedural);
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER1_FLOWMAP", value2 == CloudDistortionMode.Flowmap);
			bool flag = cloudLayer.layers.value == CloudMapMode.Double;
			CloudDistortionMode value3 = cloudLayer.layerB.distortionMode.value;
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER2_OFF", !flag);
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER2_STATIC", flag && value3 == CloudDistortionMode.None);
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER2_PROCEDURAL", flag && value3 == CloudDistortionMode.Procedural);
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "LAYER2_FLOWMAP", flag && value3 == CloudDistortionMode.Flowmap);
			VisualEnvironment component3 = hdCamera.volumeStack.GetComponent<VisualEnvironment>();
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "PHYSICALLY_BASED_SUN", component3.skyType.value == 4);
			m_PropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
			if (renderForCubemap)
			{
				CoreUtils.SetRenderTarget(commandBuffer, builtinParams.colorBuffer, ClearFlag.None, 0, builtinParams.cubemapFace);
				CoreUtils.DrawFullScreen(commandBuffer, m_CloudLayerMaterial, m_PropertyBlock);
				return;
			}
			CoreUtils.SetKeyword(m_CloudLayerMaterial, "CLOUD_RENDER_OPACITY_MRT", builtinParams.cloudOpacity != null);
			if (builtinParams.depthBuffer == BuiltinSkyParameters.nullRT)
			{
				if (builtinParams.cloudOpacity == null)
				{
					CoreUtils.SetRenderTarget(commandBuffer, builtinParams.colorBuffer);
				}
				else
				{
					RenderTargetIdentifier[] colorBuffers = new RenderTargetIdentifier[2] { builtinParams.colorBuffer, builtinParams.cloudOpacity };
					CoreUtils.SetRenderTarget(commandBuffer, colorBuffers, null);
				}
			}
			else if (builtinParams.cloudOpacity == null)
			{
				CoreUtils.SetRenderTarget(commandBuffer, builtinParams.colorBuffer, builtinParams.depthBuffer);
			}
			else
			{
				mrtToRenderCloudOcclusion[0] = builtinParams.colorBuffer;
				mrtToRenderCloudOcclusion[1] = builtinParams.cloudOpacity;
				CoreUtils.SetRenderTarget(commandBuffer, mrtToRenderCloudOcclusion, builtinParams.depthBuffer);
			}
			CoreUtils.DrawFullScreen(commandBuffer, m_CloudLayerMaterial, m_PropertyBlock, 1);
		}
	}
}
