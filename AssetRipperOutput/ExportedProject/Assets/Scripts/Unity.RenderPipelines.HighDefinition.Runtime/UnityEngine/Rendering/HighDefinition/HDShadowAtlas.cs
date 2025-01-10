using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDShadowAtlas
	{
		internal struct HDShadowAtlasInitParameters
		{
			internal HDRenderPipelineRuntimeResources renderPipelineResources;

			internal RenderGraph renderGraph;

			internal bool useSharedTexture;

			internal int width;

			internal int height;

			internal int atlasShaderID;

			internal int maxShadowRequests;

			internal string name;

			internal bool isShadowCache;

			internal Material clearMaterial;

			internal HDShadowInitParameters initParams;

			internal BlurAlgorithm blurAlgorithm;

			internal FilterMode filterMode;

			internal DepthBits depthBufferBits;

			internal RenderTextureFormat format;

			internal ConstantBuffer<ShaderVariablesGlobal> cb;

			internal HDShadowAtlasInitParameters(HDRenderPipelineRuntimeResources renderPipelineResources, RenderGraph renderGraph, bool useSharedTexture, int width, int height, int atlasShaderID, Material clearMaterial, int maxShadowRequests, HDShadowInitParameters initParams, ConstantBuffer<ShaderVariablesGlobal> cb)
			{
				this.renderPipelineResources = renderPipelineResources;
				this.renderGraph = renderGraph;
				this.useSharedTexture = useSharedTexture;
				this.width = width;
				this.height = height;
				this.atlasShaderID = atlasShaderID;
				this.clearMaterial = clearMaterial;
				this.maxShadowRequests = maxShadowRequests;
				this.initParams = initParams;
				blurAlgorithm = BlurAlgorithm.None;
				filterMode = FilterMode.Bilinear;
				depthBufferBits = DepthBits.Depth16;
				format = RenderTextureFormat.Shadowmap;
				name = "";
				isShadowCache = false;
				this.cb = cb;
			}
		}

		public enum BlurAlgorithm
		{
			None = 0,
			EVSM = 1,
			IM = 2
		}

		private class RenderShadowMapsPassData
		{
			public TextureHandle atlasTexture;

			public ShaderVariablesGlobal globalCBData;

			public ConstantBuffer<ShaderVariablesGlobal> globalCB;

			public ShadowDrawingSettings shadowDrawSettings;

			public List<HDShadowRequest> shadowRequests;

			public Material clearMaterial;

			public bool debugClearAtlas;

			public bool isRenderingOnACache;
		}

		private class EVSMBlurMomentsPassData
		{
			public TextureHandle atlasTexture;

			public TextureHandle momentAtlasTexture1;

			public TextureHandle momentAtlasTexture2;

			public ComputeShader evsmShadowBlurMomentsCS;

			public List<HDShadowRequest> shadowRequests;

			public bool isRenderingOnACache;
		}

		private class IMBlurMomentPassData
		{
			public TextureHandle atlasTexture;

			public TextureHandle momentAtlasTexture;

			public TextureHandle intermediateSummedAreaTexture;

			public TextureHandle summedAreaTexture;

			public List<HDShadowRequest> shadowRequests;

			public ComputeShader imShadowBlurMomentsCS;

			public bool isRenderingOnACache;
		}

		protected List<HDShadowRequest> m_ShadowRequests = new List<HDShadowRequest>();

		private Material m_ClearMaterial;

		private LightingDebugSettings m_LightingDebugSettings;

		private FilterMode m_FilterMode;

		private DepthBits m_DepthBufferBits;

		private RenderTextureFormat m_Format;

		private string m_Name;

		private string m_MomentName;

		private string m_MomentCopyName;

		private string m_IntermediateSummedAreaName;

		private string m_SummedAreaName;

		private int m_AtlasShaderID;

		private HDRenderPipelineRuntimeResources m_RenderPipelineResources;

		private BlurAlgorithm m_BlurAlgorithm;

		private ConstantBuffer<ShaderVariablesGlobal> m_GlobalConstantBuffer;

		protected bool m_IsACacheForShadows;

		private bool m_UseSharedTexture;

		protected TextureHandle m_Output;

		protected TextureHandle m_ShadowMapOutput;

		private static readonly Vector4[] evsmBlurWeights = new Vector4[2]
		{
			new Vector4(0.1531703f, 0.1448929f, 0.1226492f, 0.0929025f),
			new Vector4(0.06297021f, 0f, 0f, 0f)
		};

		public int width { get; private set; }

		public int height { get; private set; }

		internal bool HasShadowRequests()
		{
			return m_ShadowRequests.Count > 0;
		}

		public TextureDesc GetShadowMapTextureDesc()
		{
			TextureDesc result = new TextureDesc(width, height);
			result.filterMode = m_FilterMode;
			result.depthBufferBits = m_DepthBufferBits;
			result.isShadowMap = true;
			result.name = m_Name;
			return result;
		}

		public HDShadowAtlas()
		{
		}

		public virtual void InitAtlas(HDShadowAtlasInitParameters initParams)
		{
			width = initParams.width;
			height = initParams.height;
			m_FilterMode = initParams.filterMode;
			m_DepthBufferBits = initParams.depthBufferBits;
			m_Format = initParams.format;
			m_Name = initParams.name;
			m_MomentName = m_Name + "Moment";
			m_MomentCopyName = m_Name + "MomentCopy";
			m_IntermediateSummedAreaName = m_Name + "IntermediateSummedArea";
			m_SummedAreaName = m_Name + "SummedAreaFinal";
			m_AtlasShaderID = initParams.atlasShaderID;
			m_ClearMaterial = initParams.clearMaterial;
			m_BlurAlgorithm = initParams.blurAlgorithm;
			m_RenderPipelineResources = initParams.renderPipelineResources;
			m_IsACacheForShadows = initParams.isShadowCache;
			m_GlobalConstantBuffer = initParams.cb;
			InitializeRenderGraphOutput(initParams.renderGraph, initParams.useSharedTexture);
		}

		public HDShadowAtlas(HDShadowAtlasInitParameters initParams)
		{
			InitAtlas(initParams);
		}

		private TextureDesc GetMomentAtlasDesc(string name)
		{
			TextureDesc result = new TextureDesc(width / 2, height / 2);
			result.colorFormat = GraphicsFormat.R32G32_SFloat;
			result.useMipMap = true;
			result.autoGenerateMips = false;
			result.name = name;
			result.enableRandomWrite = true;
			return result;
		}

		private TextureDesc GetImprovedMomentAtlasDesc()
		{
			TextureDesc result = new TextureDesc(width, height);
			result.colorFormat = GraphicsFormat.R32G32B32A32_SFloat;
			result.name = m_MomentName;
			result.enableRandomWrite = true;
			result.clearColor = Color.black;
			return result;
		}

		internal TextureDesc GetAtlasDesc()
		{
			return m_BlurAlgorithm switch
			{
				BlurAlgorithm.None => GetShadowMapTextureDesc(), 
				BlurAlgorithm.EVSM => GetMomentAtlasDesc(m_MomentName), 
				BlurAlgorithm.IM => GetImprovedMomentAtlasDesc(), 
				_ => default(TextureDesc), 
			};
		}

		public void UpdateSize(Vector2Int size)
		{
			width = size.x;
			height = size.y;
		}

		internal void AddShadowRequest(HDShadowRequest shadowRequest)
		{
			m_ShadowRequests.Add(shadowRequest);
		}

		public void UpdateDebugSettings(LightingDebugSettings lightingDebugSettings)
		{
			m_LightingDebugSettings = lightingDebugSettings;
		}

		public void InvalidateOutputIfNeeded()
		{
			if (!m_UseSharedTexture)
			{
				m_Output = TextureHandle.nullHandle;
			}
		}

		public TextureHandle GetOutputTexture(RenderGraph renderGraph)
		{
			if (m_UseSharedTexture)
			{
				TextureDesc desc = GetAtlasDesc();
				TextureDesc textureDesc = renderGraph.GetTextureDesc(m_Output);
				if (textureDesc.width != desc.width || textureDesc.height != desc.height)
				{
					renderGraph.RefreshSharedTextureDesc(m_Output, in desc);
				}
				return m_Output;
			}
			TextureDesc desc2 = GetAtlasDesc();
			renderGraph.CreateTextureIfInvalid(in desc2, ref m_Output);
			return m_Output;
		}

		public TextureHandle GetShadowMapDepthTexture(RenderGraph renderGraph)
		{
			if (m_BlurAlgorithm == BlurAlgorithm.None)
			{
				return GetOutputTexture(renderGraph);
			}
			TextureDesc desc = GetShadowMapTextureDesc();
			renderGraph.CreateTextureIfInvalid(in desc, ref m_ShadowMapOutput);
			return m_ShadowMapOutput;
		}

		protected void InitializeRenderGraphOutput(RenderGraph renderGraph, bool useSharedTexture)
		{
			_ = m_UseSharedTexture;
			m_UseSharedTexture = useSharedTexture;
			if (m_UseSharedTexture)
			{
				TextureDesc desc = GetAtlasDesc();
				m_Output = renderGraph.CreateSharedTexture(in desc, explicitRelease: true);
			}
		}

		internal void CleanupRenderGraphOutput(RenderGraph renderGraph)
		{
			if (m_UseSharedTexture && renderGraph != null && m_Output.IsValid())
			{
				renderGraph.ReleaseSharedTexture(m_Output);
				m_UseSharedTexture = false;
				m_Output = TextureHandle.nullHandle;
			}
		}

		public bool HasBlurredEVSM()
		{
			return m_BlurAlgorithm == BlurAlgorithm.EVSM;
		}

		internal TextureHandle RenderShadowMaps(RenderGraph renderGraph, CullingResults cullResults, in ShaderVariablesGlobal globalCBData, FrameSettings frameSettings, string shadowPassName)
		{
			RenderShadowMapsPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<RenderShadowMapsPassData>("Render Shadow Maps", out passData, ProfilingSampler.Get(HDProfileId.RenderShadowMaps));
			try
			{
				passData.globalCBData = globalCBData;
				passData.globalCB = m_GlobalConstantBuffer;
				passData.shadowRequests = m_ShadowRequests;
				passData.clearMaterial = m_ClearMaterial;
				passData.debugClearAtlas = m_LightingDebugSettings.clearShadowAtlas;
				passData.shadowDrawSettings = new ShadowDrawingSettings(cullResults, 0, BatchCullingProjectionType.Perspective);
				passData.shadowDrawSettings.useRenderingLayerMaskTest = frameSettings.IsEnabled(FrameSettingsField.LightLayers);
				passData.isRenderingOnACache = m_IsACacheForShadows;
				if (m_BlurAlgorithm == BlurAlgorithm.EVSM || m_BlurAlgorithm == BlurAlgorithm.IM)
				{
					RenderShadowMapsPassData renderShadowMapsPassData = passData;
					TextureHandle input = GetShadowMapDepthTexture(renderGraph);
					renderShadowMapsPassData.atlasTexture = renderGraphBuilder.WriteTexture(in input);
				}
				else
				{
					RenderShadowMapsPassData renderShadowMapsPassData2 = passData;
					TextureHandle input = GetOutputTexture(renderGraph);
					renderShadowMapsPassData2.atlasTexture = renderGraphBuilder.WriteTexture(in input);
				}
				renderGraphBuilder.SetRenderFunc(delegate(RenderShadowMapsPassData data, RenderGraphContext ctx)
				{
					ctx.cmd.SetRenderTarget(data.atlasTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
					if (data.debugClearAtlas)
					{
						CoreUtils.DrawFullScreen(ctx.cmd, data.clearMaterial);
					}
					foreach (HDShadowRequest shadowRequest in data.shadowRequests)
					{
						bool flag = ((shadowRequest.shadowMapType == ShadowMapType.CascadedDirectional) ? (!shadowRequest.shouldRenderCachedComponent && shadowRequest.shouldUseCachedShadowData) : (!shadowRequest.shouldRenderCachedComponent && data.isRenderingOnACache));
						if (shadowRequest.shadowMapType == ShadowMapType.CascadedDirectional && shadowRequest.isMixedCached)
						{
							flag = !shadowRequest.shouldRenderCachedComponent && data.isRenderingOnACache;
						}
						if (!flag)
						{
							bool flag2 = false;
							if (shadowRequest.isMixedCached)
							{
								flag2 = !data.isRenderingOnACache;
								data.shadowDrawSettings.objectsFilter = (flag2 ? ShadowObjectsFilter.DynamicOnly : ShadowObjectsFilter.StaticOnly);
							}
							else
							{
								data.shadowDrawSettings.objectsFilter = ShadowObjectsFilter.AllObjects;
							}
							ctx.cmd.SetGlobalDepthBias(1f, shadowRequest.slopeBias);
							ctx.cmd.SetViewport(data.isRenderingOnACache ? shadowRequest.cachedAtlasViewport : shadowRequest.dynamicAtlasViewport);
							ctx.cmd.SetGlobalFloat(HDShaderIDs._ZClip, shadowRequest.zClip ? 1f : 0f);
							if (!flag2)
							{
								CoreUtils.DrawFullScreen(ctx.cmd, data.clearMaterial);
							}
							data.shadowDrawSettings.lightIndex = shadowRequest.lightIndex;
							data.shadowDrawSettings.splitData = shadowRequest.splitData;
							data.shadowDrawSettings.projectionType = shadowRequest.projectionType;
							Matrix4x4 view = shadowRequest.view;
							if (flag2 && shadowRequest.shadowMapType == ShadowMapType.CascadedDirectional)
							{
								view *= Matrix4x4.Translate(shadowRequest.cachedShadowData.cacheTranslationDelta);
							}
							Matrix4x4 viewProjMatrix = shadowRequest.deviceProjectionYFlip * view;
							data.globalCBData._ViewMatrix = view;
							data.globalCBData._InvViewMatrix = view.inverse;
							data.globalCBData._ProjMatrix = shadowRequest.deviceProjectionYFlip;
							data.globalCBData._InvProjMatrix = shadowRequest.deviceProjectionYFlip.inverse;
							data.globalCBData._ViewProjMatrix = viewProjMatrix;
							data.globalCBData._InvViewProjMatrix = viewProjMatrix.inverse;
							data.globalCBData._SlopeScaleDepthBias = 0f - shadowRequest.slopeBias;
							data.globalCBData._GlobalMipBias = 0f;
							data.globalCBData._GlobalMipBiasPow2 = 1f;
							data.globalCB.PushGlobal(ctx.cmd, in data.globalCBData, HDShaderIDs._ShaderVariablesGlobal);
							ctx.cmd.SetGlobalVectorArray(HDShaderIDs._ShadowFrustumPlanes, shadowRequest.frustumPlanes);
							ctx.renderContext.ExecuteCommandBuffer(ctx.cmd);
							ctx.cmd.Clear();
							ctx.renderContext.DrawShadows(ref data.shadowDrawSettings);
						}
					}
					ctx.cmd.SetGlobalFloat(HDShaderIDs._ZClip, 1f);
					ctx.cmd.SetGlobalDepthBias(0f, 0f);
				});
				m_ShadowMapOutput = passData.atlasTexture;
				return passData.atlasTexture;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		private unsafe TextureHandle EVSMBlurMoments(RenderGraph renderGraph, TextureHandle inputAtlas)
		{
			EVSMBlurMomentsPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<EVSMBlurMomentsPassData>("EVSM Blur Moments", out passData, ProfilingSampler.Get(HDProfileId.RenderEVSMShadowMaps));
			try
			{
				passData.evsmShadowBlurMomentsCS = m_RenderPipelineResources.shaders.evsmBlurCS;
				passData.shadowRequests = m_ShadowRequests;
				passData.isRenderingOnACache = m_IsACacheForShadows;
				passData.atlasTexture = renderGraphBuilder.ReadTexture(in inputAtlas);
				EVSMBlurMomentsPassData eVSMBlurMomentsPassData = passData;
				TextureHandle input = GetOutputTexture(renderGraph);
				eVSMBlurMomentsPassData.momentAtlasTexture1 = renderGraphBuilder.WriteTexture(in input);
				EVSMBlurMomentsPassData eVSMBlurMomentsPassData2 = passData;
				TextureDesc desc = GetMomentAtlasDesc(m_MomentCopyName);
				input = renderGraph.CreateTexture(in desc);
				eVSMBlurMomentsPassData2.momentAtlasTexture2 = renderGraphBuilder.WriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(EVSMBlurMomentsPassData data, RenderGraphContext ctx)
				{
					ComputeShader evsmShadowBlurMomentsCS = data.evsmShadowBlurMomentsCS;
					RTHandle[] momentAtlasRenderTextures = ctx.renderGraphPool.GetTempArray<RTHandle>(2);
					momentAtlasRenderTextures[0] = data.momentAtlasTexture1;
					momentAtlasRenderTextures[1] = data.momentAtlasTexture2;
					int kernelIndex = evsmShadowBlurMomentsCS.FindKernel("ConvertAndBlur");
					int kernelIndex2 = evsmShadowBlurMomentsCS.FindKernel("Blur");
					int kernelIndex3 = evsmShadowBlurMomentsCS.FindKernel("CopyMoments");
					RTHandle rTHandle = data.atlasTexture;
					ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex, HDShaderIDs._DepthTexture, rTHandle);
					ctx.cmd.SetComputeVectorArrayParam(evsmShadowBlurMomentsCS, HDShaderIDs._BlurWeightsStorage, evsmBlurWeights);
					int* ptr = stackalloc int[data.shadowRequests.Count];
					int num = 0;
					foreach (HDShadowRequest shadowRequest in data.shadowRequests)
					{
						bool num2;
						if (shadowRequest.shadowMapType == ShadowMapType.CascadedDirectional)
						{
							if (!shadowRequest.shouldRenderCachedComponent)
							{
								num2 = shadowRequest.shouldUseCachedShadowData;
								goto IL_0101;
							}
						}
						else if (!shadowRequest.shouldRenderCachedComponent)
						{
							num2 = data.isRenderingOnACache;
							goto IL_0101;
						}
						goto IL_0106;
						IL_0101:
						if (num2)
						{
							continue;
						}
						goto IL_0106;
						IL_0106:
						Rect rect = (data.isRenderingOnACache ? shadowRequest.cachedAtlasViewport : shadowRequest.dynamicAtlasViewport);
						using (new ProfilingScope(ctx.cmd, ProfilingSampler.Get(HDProfileId.RenderEVSMShadowMapsBlur)))
						{
							int num3 = Mathf.CeilToInt(rect.width * 0.5f);
							int num4 = Mathf.CeilToInt(rect.height * 0.5f);
							Vector2 vector = new Vector2(rect.min.x * 0.5f, rect.min.y * 0.5f);
							ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex, HDShaderIDs._OutputTexture, momentAtlasRenderTextures[0]);
							ctx.cmd.SetComputeVectorParam(evsmShadowBlurMomentsCS, HDShaderIDs._SrcRect, new Vector4(rect.min.x, rect.min.y, rect.width, rect.height));
							ctx.cmd.SetComputeVectorParam(evsmShadowBlurMomentsCS, HDShaderIDs._DstRect, new Vector4(vector.x, vector.y, 1f / (float)rTHandle.rt.width, 1f / (float)rTHandle.rt.height));
							ctx.cmd.SetComputeFloatParam(evsmShadowBlurMomentsCS, HDShaderIDs._EVSMExponent, shadowRequest.evsmParams.x);
							int threadGroupsX = (num3 + 7) / 8;
							int threadGroupsY = (num4 + 7) / 8;
							ctx.cmd.DispatchCompute(evsmShadowBlurMomentsCS, kernelIndex, threadGroupsX, threadGroupsY, 1);
							int currentAtlasMomentSurface = 0;
							ctx.cmd.SetComputeVectorParam(evsmShadowBlurMomentsCS, HDShaderIDs._SrcRect, new Vector4(vector.x, vector.y, num3, num4));
							for (int i = 0; (float)i < shadowRequest.evsmParams.w; i++)
							{
								currentAtlasMomentSurface = (currentAtlasMomentSurface + 1) & 1;
								ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex2, HDShaderIDs._InputTexture, GetMomentRTCopy());
								ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex2, HDShaderIDs._OutputTexture, GetMomentRT());
								ctx.cmd.DispatchCompute(evsmShadowBlurMomentsCS, kernelIndex2, threadGroupsX, threadGroupsY, 1);
							}
							ptr[num++] = currentAtlasMomentSurface;
							RTHandle GetMomentRT()
							{
								return momentAtlasRenderTextures[currentAtlasMomentSurface];
							}
							RTHandle GetMomentRTCopy()
							{
								return momentAtlasRenderTextures[(currentAtlasMomentSurface + 1) & 1];
							}
						}
					}
					for (int j = 0; j < data.shadowRequests.Count; j++)
					{
						if (ptr[j] != 0)
						{
							using (new ProfilingScope(ctx.cmd, ProfilingSampler.Get(HDProfileId.RenderEVSMShadowMapsCopyToAtlas)))
							{
								HDShadowRequest hDShadowRequest = data.shadowRequests[j];
								Rect rect2 = (data.isRenderingOnACache ? hDShadowRequest.cachedAtlasViewport : hDShadowRequest.dynamicAtlasViewport);
								int num5 = Mathf.CeilToInt(rect2.width * 0.5f);
								int num6 = Mathf.CeilToInt(rect2.height * 0.5f);
								ctx.cmd.SetComputeVectorParam(evsmShadowBlurMomentsCS, HDShaderIDs._SrcRect, new Vector4(rect2.min.x * 0.5f, rect2.min.y * 0.5f, num5, num6));
								ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex3, HDShaderIDs._InputTexture, momentAtlasRenderTextures[1]);
								ctx.cmd.SetComputeTextureParam(evsmShadowBlurMomentsCS, kernelIndex3, HDShaderIDs._OutputTexture, momentAtlasRenderTextures[0]);
								int threadGroupsX2 = (num5 + 7) / 8;
								int threadGroupsY2 = (num6 + 7) / 8;
								ctx.cmd.DispatchCompute(evsmShadowBlurMomentsCS, kernelIndex3, threadGroupsX2, threadGroupsY2, 1);
							}
						}
					}
				});
				return passData.momentAtlasTexture1;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		private TextureHandle IMBlurMoment(RenderGraph renderGraph, TextureHandle atlasTexture)
		{
			IMBlurMomentPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<IMBlurMomentPassData>("EVSM Blur Moments", out passData, ProfilingSampler.Get(HDProfileId.RenderMomentShadowMaps));
			try
			{
				passData.shadowRequests = m_ShadowRequests;
				passData.isRenderingOnACache = m_IsACacheForShadows;
				passData.imShadowBlurMomentsCS = m_RenderPipelineResources.shaders.momentShadowsCS;
				passData.atlasTexture = renderGraphBuilder.ReadTexture(in atlasTexture);
				IMBlurMomentPassData iMBlurMomentPassData = passData;
				TextureHandle input = GetOutputTexture(renderGraph);
				iMBlurMomentPassData.momentAtlasTexture = renderGraphBuilder.WriteTexture(in input);
				IMBlurMomentPassData iMBlurMomentPassData2 = passData;
				TextureDesc desc = new TextureDesc(width, height)
				{
					colorFormat = GraphicsFormat.R32G32B32A32_SInt,
					name = m_IntermediateSummedAreaName,
					enableRandomWrite = true,
					clearBuffer = true,
					clearColor = Color.black
				};
				input = renderGraph.CreateTexture(in desc);
				iMBlurMomentPassData2.intermediateSummedAreaTexture = renderGraphBuilder.WriteTexture(in input);
				IMBlurMomentPassData iMBlurMomentPassData3 = passData;
				desc = new TextureDesc(width, height)
				{
					colorFormat = GraphicsFormat.R32G32B32A32_SInt,
					name = m_SummedAreaName,
					enableRandomWrite = true,
					clearColor = Color.black
				};
				input = renderGraph.CreateTexture(in desc);
				iMBlurMomentPassData3.summedAreaTexture = renderGraphBuilder.WriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(IMBlurMomentPassData data, RenderGraphContext ctx)
				{
					ComputeShader imShadowBlurMomentsCS = data.imShadowBlurMomentsCS;
					if (imShadowBlurMomentsCS == null)
					{
						return;
					}
					int kernelIndex = imShadowBlurMomentsCS.FindKernel("ComputeMomentShadows");
					int kernelIndex2 = imShadowBlurMomentsCS.FindKernel("MomentSummedAreaTableHorizontal");
					int kernelIndex3 = imShadowBlurMomentsCS.FindKernel("MomentSummedAreaTableVertical");
					RTHandle rTHandle = data.atlasTexture;
					RTHandle rTHandle2 = data.momentAtlasTexture;
					RTHandle rTHandle3 = data.intermediateSummedAreaTexture;
					RTHandle rTHandle4 = data.summedAreaTexture;
					foreach (HDShadowRequest shadowRequest in data.shadowRequests)
					{
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex, HDShaderIDs._ShadowmapAtlas, rTHandle);
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex, HDShaderIDs._MomentShadowAtlas, rTHandle2);
						ctx.cmd.SetComputeVectorParam(imShadowBlurMomentsCS, HDShaderIDs._MomentShadowmapSlotST, new Vector4(shadowRequest.dynamicAtlasViewport.width, shadowRequest.dynamicAtlasViewport.height, shadowRequest.dynamicAtlasViewport.min.x, shadowRequest.dynamicAtlasViewport.min.y));
						int threadGroupsX = Math.Max((int)shadowRequest.dynamicAtlasViewport.width / 8, 1);
						int threadGroupsY = Math.Max((int)shadowRequest.dynamicAtlasViewport.height / 8, 1);
						ctx.cmd.DispatchCompute(imShadowBlurMomentsCS, kernelIndex, threadGroupsX, threadGroupsY, 1);
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex2, HDShaderIDs._SummedAreaTableInputFloat, rTHandle2);
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex2, HDShaderIDs._SummedAreaTableOutputInt, rTHandle3);
						ctx.cmd.SetComputeFloatParam(imShadowBlurMomentsCS, HDShaderIDs._IMSKernelSize, shadowRequest.kernelSize);
						ctx.cmd.SetComputeVectorParam(imShadowBlurMomentsCS, HDShaderIDs._MomentShadowmapSize, new Vector2(rTHandle2.referenceSize.x, rTHandle2.referenceSize.y));
						int threadGroupsX2 = Math.Max((int)shadowRequest.dynamicAtlasViewport.width / 64, 1);
						ctx.cmd.DispatchCompute(imShadowBlurMomentsCS, kernelIndex2, threadGroupsX2, 1, 1);
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex3, HDShaderIDs._SummedAreaTableInputInt, rTHandle3);
						ctx.cmd.SetComputeTextureParam(imShadowBlurMomentsCS, kernelIndex3, HDShaderIDs._SummedAreaTableOutputInt, rTHandle4);
						ctx.cmd.SetComputeVectorParam(imShadowBlurMomentsCS, HDShaderIDs._MomentShadowmapSize, new Vector2(rTHandle2.referenceSize.x, rTHandle2.referenceSize.y));
						ctx.cmd.SetComputeFloatParam(imShadowBlurMomentsCS, HDShaderIDs._IMSKernelSize, shadowRequest.kernelSize);
						int threadGroupsX3 = Math.Max((int)shadowRequest.dynamicAtlasViewport.height / 64, 1);
						ctx.cmd.DispatchCompute(imShadowBlurMomentsCS, kernelIndex3, threadGroupsX3, 1, 1);
						ctx.cmd.SetGlobalTexture(HDShaderIDs._SummedAreaTableInputInt, rTHandle4);
					}
				});
				return passData.momentAtlasTexture;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		internal TextureHandle BlurShadows(RenderGraph renderGraph)
		{
			if (m_ShadowRequests.Count == 0)
			{
				return renderGraph.defaultResources.defaultShadowTexture;
			}
			if (m_BlurAlgorithm == BlurAlgorithm.EVSM)
			{
				return EVSMBlurMoments(renderGraph, m_ShadowMapOutput);
			}
			if (m_BlurAlgorithm == BlurAlgorithm.IM)
			{
				return IMBlurMoment(renderGraph, m_ShadowMapOutput);
			}
			return m_ShadowMapOutput;
		}

		internal TextureHandle RenderShadows(RenderGraph renderGraph, CullingResults cullResults, in ShaderVariablesGlobal globalCB, FrameSettings frameSettings, string shadowPassName)
		{
			if (m_ShadowRequests.Count == 0)
			{
				return renderGraph.defaultResources.defaultShadowTexture;
			}
			RenderShadowMaps(renderGraph, cullResults, in globalCB, frameSettings, shadowPassName);
			if (m_BlurAlgorithm == BlurAlgorithm.EVSM)
			{
				return EVSMBlurMoments(renderGraph, m_ShadowMapOutput);
			}
			if (m_BlurAlgorithm == BlurAlgorithm.IM)
			{
				return IMBlurMoment(renderGraph, m_ShadowMapOutput);
			}
			return m_ShadowMapOutput;
		}

		public void AddBlitRequestsForUpdatedShadows(HDDynamicShadowAtlas dynamicAtlas)
		{
			if (!m_IsACacheForShadows)
			{
				return;
			}
			foreach (HDShadowRequest shadowRequest in m_ShadowRequests)
			{
				if (shadowRequest.shouldRenderCachedComponent)
				{
					dynamicAtlas.AddRequestToPendingBlitFromCache(shadowRequest);
				}
			}
		}

		public virtual void DisplayAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, Rect atlasViewport, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb, float scaleFactor = 1f)
		{
			if (atlasTexture != null)
			{
				Vector4 value = new Vector4(minValue, 1f / (maxValue - minValue));
				float num = 1f / (float)width;
				float num2 = 1f / (float)height;
				Vector4 value2 = Vector4.Scale(new Vector4(num, num2, num, num2), new Vector4(atlasViewport.width, atlasViewport.height, atlasViewport.x, atlasViewport.y));
				mpb.SetTexture("_AtlasTexture", atlasTexture);
				mpb.SetVector("_TextureScaleBias", value2);
				mpb.SetVector("_ValidRange", value);
				mpb.SetFloat("_RcpGlobalScaleFactor", scaleFactor);
				cmd.SetViewport(new Rect(screenX, screenY, screenSizeX, screenSizeY));
				cmd.DrawProcedural(Matrix4x4.identity, debugMaterial, debugMaterial.FindPass("RegularShadow"), MeshTopology.Triangles, 3, 1, mpb);
			}
		}

		public virtual void Clear()
		{
			m_ShadowRequests.Clear();
		}

		public void Release(RenderGraph renderGraph)
		{
			CleanupRenderGraphOutput(renderGraph);
		}
	}
}
