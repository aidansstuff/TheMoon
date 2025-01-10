using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RendererUtils;

namespace UnityEngine.Rendering.HighDefinition
{
	public static class CustomPassUtils
	{
		private struct OverrideRTHandleScale : IDisposable
		{
			private static int overrideCounter;

			private CustomPassInjectionPoint injectionPoint;

			public OverrideRTHandleScale(in CustomPassContext ctx)
			{
				injectionPoint = ctx.injectionPoint;
				if (injectionPoint == CustomPassInjectionPoint.AfterPostProcess)
				{
					if (overrideCounter == 0)
					{
						propertyBlock.SetVector(HDShaderIDs._OverrideRTHandleScale, RTHandles.rtHandleProperties.rtHandleScale);
					}
					overrideCounter++;
				}
			}

			public void Dispose()
			{
				if (injectionPoint == CustomPassInjectionPoint.AfterPostProcess)
				{
					if (overrideCounter == 1)
					{
						propertyBlock.SetVector(HDShaderIDs._OverrideRTHandleScale, Vector4.zero);
					}
					overrideCounter--;
				}
			}
		}

		public struct DisableSinglePassRendering : IDisposable
		{
			private CustomPassContext m_Context;

			public DisableSinglePassRendering(in CustomPassContext ctx)
			{
				m_Context = ctx;
				if (ctx.hdCamera.xr.enabled)
				{
					m_Context.hdCamera.xr.StopSinglePass(ctx.cmd);
				}
			}

			void IDisposable.Dispose()
			{
				if (m_Context.hdCamera.xr.enabled)
				{
					m_Context.hdCamera.xr.StartSinglePass(m_Context.cmd);
				}
			}
		}

		public struct OverrideCameraRendering : IDisposable
		{
			private CustomPassContext ctx;

			private Camera overrideCamera;

			private HDCamera overrideHDCamera;

			private float originalAspect;

			private static Stack<HDCamera> overrideCameraStack = new Stack<HDCamera>();

			public OverrideCameraRendering(CustomPassContext ctx, Camera overrideCamera)
			{
				this.ctx = ctx;
				this.overrideCamera = overrideCamera;
				overrideHDCamera = HDCamera.GetOrCreate(overrideCamera);
				originalAspect = overrideCamera.aspect;
				float aspect = overrideCamera.aspect;
				Init(overrideAspectRatio: (!(overrideCamera.targetTexture == null)) ? ((float)overrideCamera.pixelWidth / (float)overrideCamera.pixelHeight) : (ctx.hdCamera.camera.pixelRect.width / ctx.hdCamera.camera.pixelRect.height), ctx: ctx, overrideCamera: overrideCamera);
			}

			public OverrideCameraRendering(CustomPassContext ctx, Camera overrideCamera, float overrideAspectRatio)
			{
				this.ctx = ctx;
				this.overrideCamera = overrideCamera;
				overrideHDCamera = HDCamera.GetOrCreate(overrideCamera);
				originalAspect = overrideCamera.aspect;
				Init(ctx, overrideCamera, overrideAspectRatio);
			}

			private void Init(CustomPassContext ctx, Camera overrideCamera, float overrideAspectRatio)
			{
				if (IsContextValid(ctx, overrideCamera))
				{
					overrideHDCamera.isPersistent = true;
					overrideCamera.aspect = overrideAspectRatio;
					if (overrideCamera.targetTexture == null)
					{
						overrideHDCamera.OverridePixelRect(ctx.hdCamera.camera.pixelRect);
					}
					HDRenderPipeline currentPipeline = HDRenderPipeline.currentPipeline;
					overrideHDCamera.Update(overrideHDCamera.frameSettings, currentPipeline, XRSystem.emptyPass, allocateHistoryBuffers: false);
					ctx.hdCamera.SetReferenceSize();
					ShaderVariablesGlobal cb = currentPipeline.GetShaderVariablesGlobalCB();
					overrideHDCamera.UpdateShaderVariablesGlobalCB(ref cb);
					ConstantBuffer.PushGlobal(ctx.cmd, in cb, HDShaderIDs._ShaderVariablesGlobal);
					overrideCameraStack.Push(overrideHDCamera);
				}
			}

			private static bool IsContextValid(CustomPassContext ctx, Camera overrideCamera)
			{
				if (overrideCamera == ctx.hdCamera.camera)
				{
					return false;
				}
				return true;
			}

			void IDisposable.Dispose()
			{
				if (IsContextValid(ctx, overrideCamera))
				{
					if (overrideCamera.targetTexture == null)
					{
						overrideHDCamera.ResetPixelRect();
					}
					overrideCamera.aspect = originalAspect;
					ShaderVariablesGlobal cb = HDRenderPipeline.currentPipeline.GetShaderVariablesGlobalCB();
					overrideCameraStack.Pop();
					if (overrideCameraStack.Count > 0)
					{
						HDCamera hDCamera = overrideCameraStack.Peek();
						hDCamera.SetReferenceSize();
						hDCamera.UpdateShaderVariablesGlobalCB(ref cb);
					}
					else
					{
						ctx.hdCamera.SetReferenceSize();
						ctx.hdCamera.UpdateShaderVariablesGlobalCB(ref cb);
					}
					ConstantBuffer.PushGlobal(ctx.cmd, in cb, HDShaderIDs._ShaderVariablesGlobal);
				}
			}
		}

		public static Vector4 fullScreenScaleBias = new Vector4(1f, 1f, 0f, 0f);

		private static ShaderTagId[] litForwardTags = new ShaderTagId[3]
		{
			HDShaderPassNames.s_ForwardOnlyName,
			HDShaderPassNames.s_ForwardName,
			HDShaderPassNames.s_SRPDefaultUnlitName
		};

		private static ShaderTagId[] depthTags = new ShaderTagId[2]
		{
			HDShaderPassNames.s_DepthForwardOnlyName,
			HDShaderPassNames.s_DepthOnlyName
		};

		private static ProfilingSampler downSampleSampler = new ProfilingSampler("DownSample");

		private static ProfilingSampler verticalBlurSampler = new ProfilingSampler("Vertical Blur");

		private static ProfilingSampler horizontalBlurSampler = new ProfilingSampler("Horizontal Blur");

		private static ProfilingSampler gaussianblurSampler = new ProfilingSampler("Gaussian Blur");

		private static ProfilingSampler copySampler = new ProfilingSampler("Copy");

		private static ProfilingSampler renderFromCameraSampler = new ProfilingSampler("Render From Camera");

		private static ProfilingSampler renderDepthFromCameraSampler = new ProfilingSampler("Render Depth");

		private static ProfilingSampler renderNormalFromCameraSampler = new ProfilingSampler("Render Normal");

		private static ProfilingSampler renderTangentFromCameraSampler = new ProfilingSampler("Render Tangent");

		private static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

		private static Material customPassUtilsMaterial;

		private static Material customPassRenderersUtilsMaterial;

		private static Dictionary<int, ComputeBuffer> gaussianWeightsCache = new Dictionary<int, ComputeBuffer>();

		private static int downSamplePassIndex;

		private static int verticalBlurPassIndex;

		private static int horizontalBlurPassIndex;

		private static int copyPassIndex;

		private static int copyDepthPassIndex;

		private static int depthToColorPassIndex;

		private static int depthPassIndex;

		private static int normalToColorPassIndex;

		private static int tangentToColorPassIndex;

		internal static void Initialize()
		{
			customPassUtilsMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.customPassUtils);
			downSamplePassIndex = customPassUtilsMaterial.FindPass("Downsample");
			verticalBlurPassIndex = customPassUtilsMaterial.FindPass("VerticalBlur");
			horizontalBlurPassIndex = customPassUtilsMaterial.FindPass("HorizontalBlur");
			copyPassIndex = customPassUtilsMaterial.FindPass("Copy");
			copyDepthPassIndex = customPassUtilsMaterial.FindPass("CopyDepth");
			customPassRenderersUtilsMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.customPassRenderersUtils);
			depthToColorPassIndex = customPassRenderersUtilsMaterial.FindPass("DepthToColorPass");
			depthPassIndex = customPassRenderersUtilsMaterial.FindPass("DepthPass");
			normalToColorPassIndex = customPassRenderersUtilsMaterial.FindPass("NormalToColorPass");
			tangentToColorPassIndex = customPassRenderersUtilsMaterial.FindPass("TangentToColorPass");
		}

		public static void DownSample(in CustomPassContext ctx, RTHandle source, RTHandle destination, int sourceMip = 0, int destMip = 0)
		{
			DownSample(in ctx, source, destination, fullScreenScaleBias, fullScreenScaleBias, sourceMip, destMip);
		}

		public static void DownSample(in CustomPassContext ctx, RTHandle source, RTHandle destination, Vector4 sourceScaleBias, Vector4 destScaleBias, int sourceMip = 0, int destMip = 0)
		{
			if (destination.rt.width < source.rt.width / 2 || destination.rt.height < source.rt.height / 2)
			{
				Debug.LogError("Destination for DownSample is too small, it needs to be at least half as big as source.");
			}
			if (source.rt.antiAliasing > 1 || destination.rt.antiAliasing > 1)
			{
				Debug.LogError("DownSample is not supported with MSAA buffers");
			}
			using (new ProfilingScope(ctx.cmd, downSampleSampler))
			{
				using (new OverrideRTHandleScale(in ctx))
				{
					SetRenderTargetWithScaleBias(in ctx, propertyBlock, destination, destScaleBias, ClearFlag.None, destMip);
					propertyBlock.SetTexture(HDShaderIDs._Source, source);
					propertyBlock.SetVector(HDShaderIDs._SourceScaleBias, sourceScaleBias);
					SetSourceSize(propertyBlock, source);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, customPassUtilsMaterial, downSamplePassIndex, MeshTopology.Triangles, 3, 1, propertyBlock);
				}
			}
		}

		public static void Copy(in CustomPassContext ctx, RTHandle source, RTHandle destination, int sourceMip = 0, int destMip = 0)
		{
			Copy(in ctx, source, destination, fullScreenScaleBias, fullScreenScaleBias, sourceMip, destMip);
		}

		public static void Copy(in CustomPassContext ctx, RTHandle source, RTHandle destination, Vector4 sourceScaleBias, Vector4 destScaleBias, int sourceMip = 0, int destMip = 0)
		{
			if (source == destination)
			{
				Debug.LogError("Can't copy the buffer. Source has to be different from the destination.");
			}
			if (source.rt.antiAliasing > 1 || destination.rt.antiAliasing > 1)
			{
				Debug.LogError("Copy is not supported with MSAA buffers");
			}
			using (new ProfilingScope(ctx.cmd, copySampler))
			{
				using (new OverrideRTHandleScale(in ctx))
				{
					SetRenderTargetWithScaleBias(in ctx, propertyBlock, destination, destScaleBias, ClearFlag.None, destMip);
					propertyBlock.SetTexture(HDShaderIDs._Source, source);
					propertyBlock.SetVector(HDShaderIDs._SourceScaleBias, sourceScaleBias);
					SetSourceSize(propertyBlock, source);
					if (source.rt.graphicsFormat != 0 && destination.rt.graphicsFormat != 0)
					{
						ctx.cmd.DrawProcedural(Matrix4x4.identity, customPassUtilsMaterial, copyPassIndex, MeshTopology.Triangles, 3, 1, propertyBlock);
					}
					if (source.rt.depthStencilFormat != 0 && destination.rt.depthStencilFormat != 0)
					{
						ctx.cmd.DrawProcedural(Matrix4x4.identity, customPassUtilsMaterial, copyDepthPassIndex, MeshTopology.Triangles, 3, 1, propertyBlock);
					}
				}
			}
		}

		public static void VerticalGaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, int sampleCount = 8, float radius = 5f, int sourceMip = 0, int destMip = 0)
		{
			VerticalGaussianBlur(in ctx, source, destination, fullScreenScaleBias, fullScreenScaleBias, sampleCount, radius, sourceMip, destMip);
		}

		public static void VerticalGaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, Vector4 sourceScaleBias, Vector4 destScaleBias, int sampleCount = 8, float radius = 5f, int sourceMip = 0, int destMip = 0)
		{
			if (source == destination)
			{
				Debug.LogError("Can't blur the buffer. Source has to be different from the destination.");
			}
			if (source.rt.antiAliasing > 1 || destination.rt.antiAliasing > 1)
			{
				Debug.LogError("GaussianBlur is not supported with MSAA buffers");
			}
			using (new ProfilingScope(ctx.cmd, verticalBlurSampler))
			{
				using (new OverrideRTHandleScale(in ctx))
				{
					SetRenderTargetWithScaleBias(in ctx, propertyBlock, destination, destScaleBias, ClearFlag.None, destMip);
					propertyBlock.SetTexture(HDShaderIDs._Source, source);
					propertyBlock.SetVector(HDShaderIDs._SourceScaleBias, sourceScaleBias);
					propertyBlock.SetBuffer(HDShaderIDs._GaussianWeights, GetGaussianWeights(sampleCount));
					propertyBlock.SetFloat(HDShaderIDs._SampleCount, sampleCount);
					propertyBlock.SetFloat(HDShaderIDs._Radius, radius);
					SetSourceSize(propertyBlock, source);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, customPassUtilsMaterial, verticalBlurPassIndex, MeshTopology.Triangles, 3, 1, propertyBlock);
				}
			}
		}

		public static void HorizontalGaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, int sampleCount = 8, float radius = 5f, int sourceMip = 0, int destMip = 0)
		{
			HorizontalGaussianBlur(in ctx, source, destination, fullScreenScaleBias, fullScreenScaleBias, sampleCount, radius, sourceMip, destMip);
		}

		public static void HorizontalGaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, Vector4 sourceScaleBias, Vector4 destScaleBias, int sampleCount = 8, float radius = 5f, int sourceMip = 0, int destMip = 0)
		{
			if (source == destination)
			{
				Debug.LogError("Can't blur the buffer. Source has to be different from the destination.");
			}
			if (source.rt.antiAliasing > 1 || destination.rt.antiAliasing > 1)
			{
				Debug.LogError("GaussianBlur is not supported with MSAA buffers");
			}
			using (new ProfilingScope(ctx.cmd, horizontalBlurSampler))
			{
				using (new OverrideRTHandleScale(in ctx))
				{
					SetRenderTargetWithScaleBias(in ctx, propertyBlock, destination, destScaleBias, ClearFlag.None, destMip);
					propertyBlock.SetTexture(HDShaderIDs._Source, source);
					propertyBlock.SetVector(HDShaderIDs._SourceScaleBias, sourceScaleBias);
					propertyBlock.SetBuffer(HDShaderIDs._GaussianWeights, GetGaussianWeights(sampleCount));
					propertyBlock.SetFloat(HDShaderIDs._SampleCount, sampleCount);
					propertyBlock.SetFloat(HDShaderIDs._Radius, radius);
					SetSourceSize(propertyBlock, source);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, customPassUtilsMaterial, horizontalBlurPassIndex, MeshTopology.Triangles, 3, 1, propertyBlock);
				}
			}
		}

		public static void GaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, RTHandle tempTarget, int sampleCount = 9, float radius = 5f, int sourceMip = 0, int destMip = 0, bool downSample = true)
		{
			GaussianBlur(in ctx, source, destination, tempTarget, fullScreenScaleBias, fullScreenScaleBias, sampleCount, radius, sourceMip, destMip, downSample);
		}

		public static void GaussianBlur(in CustomPassContext ctx, RTHandle source, RTHandle destination, RTHandle tempTarget, Vector4 sourceScaleBias, Vector4 destScaleBias, int sampleCount = 9, float radius = 5f, int sourceMip = 0, int destMip = 0, bool downSample = true)
		{
			if (source == tempTarget || destination == tempTarget)
			{
				Debug.LogError("Can't blur the buffer. tempTarget has to be different from both source or destination.");
			}
			if (tempTarget.scaleFactor.x != tempTarget.scaleFactor.y || (tempTarget.scaleFactor.x != 0.5f && tempTarget.scaleFactor.x != 1f))
			{
				Debug.LogError($"Can't blur the buffer. Only a scaleFactor of 0.5 or 1.0 is supported on tempTarget. Current scaleFactor: {tempTarget.scaleFactor}");
			}
			if (source.rt.antiAliasing > 1 || destination.rt.antiAliasing > 1 || tempTarget.rt.antiAliasing > 1)
			{
				Debug.LogError("GaussianBlur is not supported with MSAA buffers");
			}
			if (sampleCount % 2 == 0)
			{
				sampleCount++;
			}
			using (new ProfilingScope(ctx.cmd, gaussianblurSampler))
			{
				if (downSample)
				{
					using (new OverrideRTHandleScale(in ctx))
					{
						DownSample(in ctx, source, tempTarget, sourceScaleBias, sourceScaleBias, sourceMip);
						VerticalGaussianBlur(in ctx, tempTarget, destination, sourceScaleBias, sourceScaleBias, sampleCount, radius, 0, destMip);
						Copy(in ctx, destination, tempTarget, sourceScaleBias, sourceScaleBias, 0, destMip);
						HorizontalGaussianBlur(in ctx, tempTarget, destination, sourceScaleBias, destScaleBias, sampleCount, radius, sourceMip, destMip);
						return;
					}
				}
				using (new OverrideRTHandleScale(in ctx))
				{
					VerticalGaussianBlur(in ctx, source, tempTarget, sourceScaleBias, sourceScaleBias, sampleCount, radius, sourceMip, destMip);
					HorizontalGaussianBlur(in ctx, tempTarget, destination, sourceScaleBias, destScaleBias, sampleCount, radius, sourceMip, destMip);
				}
			}
		}

		public static void DrawRenderers(in CustomPassContext ctx, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, Material overrideMaterial = null, int overrideMaterialIndex = 0, RenderStateBlock overrideRenderState = default(RenderStateBlock), SortingCriteria sorting = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder)
		{
			DrawRenderers(in ctx, litForwardTags, layerMask, renderQueueFilter, overrideMaterial, overrideMaterialIndex, overrideRenderState, sorting);
		}

		public static void DrawRenderers(in CustomPassContext ctx, ShaderTagId[] shaderTags, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, Material overrideMaterial = null, int overrideMaterialIndex = 0, RenderStateBlock overrideRenderState = default(RenderStateBlock), SortingCriteria sorting = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder)
		{
			PerObjectData rendererConfiguration = HDUtils.GetRendererConfiguration(ctx.hdCamera.frameSettings.IsEnabled(FrameSettingsField.ProbeVolume), ctx.hdCamera.frameSettings.IsEnabled(FrameSettingsField.Shadowmask));
			RendererListDesc rendererListDesc = new RendererListDesc(shaderTags, ctx.cullingResults, ctx.hdCamera.camera);
			rendererListDesc.rendererConfiguration = rendererConfiguration;
			rendererListDesc.renderQueueRange = GetRenderQueueRangeFromRenderQueueType(renderQueueFilter);
			rendererListDesc.sortingCriteria = sorting;
			rendererListDesc.overrideMaterial = overrideMaterial;
			rendererListDesc.overrideMaterialPassIndex = overrideMaterialIndex;
			rendererListDesc.excludeObjectMotionVectors = false;
			rendererListDesc.layerMask = layerMask;
			rendererListDesc.stateBlock = overrideRenderState;
			RendererListDesc desc = rendererListDesc;
			ScriptableRenderContext renderContext = ctx.renderContext;
			CoreUtils.DrawRendererList(ctx.renderContext, ctx.cmd, renderContext.CreateRendererList(desc));
		}

		internal static ComputeBuffer GetGaussianWeights(int weightCount)
		{
			if (gaussianWeightsCache.TryGetValue(weightCount, out var value))
			{
				return value;
			}
			float[] array = new float[weightCount];
			float num = 3f;
			float num2 = 0f - num;
			float num3 = 0f;
			float num4 = 1f / (float)weightCount * num * 2f;
			for (int i = 0; i < weightCount; i++)
			{
				float num5 = (array[i] = Gaussian(num2) / (float)weightCount * num * 2f);
				num2 += num4;
				num3 += num5;
			}
			value = new ComputeBuffer(array.Length, 4);
			value.SetData(array);
			gaussianWeightsCache[weightCount] = value;
			return value;
			static float Gaussian(float x, float sigma = 1f)
			{
				float num6 = 1f / Mathf.Sqrt(MathF.PI * 2f * sigma * sigma);
				float num7 = Mathf.Exp((0f - x * x) / (2f * sigma * sigma));
				return num6 * num7;
			}
		}

		public static RenderQueueRange GetRenderQueueRangeFromRenderQueueType(CustomPass.RenderQueueType type)
		{
			return type switch
			{
				CustomPass.RenderQueueType.OpaqueNoAlphaTest => HDRenderQueue.k_RenderQueue_OpaqueNoAlphaTest, 
				CustomPass.RenderQueueType.OpaqueAlphaTest => HDRenderQueue.k_RenderQueue_OpaqueAlphaTest, 
				CustomPass.RenderQueueType.AllOpaque => HDRenderQueue.k_RenderQueue_AllOpaque, 
				CustomPass.RenderQueueType.AfterPostProcessOpaque => HDRenderQueue.k_RenderQueue_AfterPostProcessOpaque, 
				CustomPass.RenderQueueType.PreRefraction => HDRenderQueue.k_RenderQueue_PreRefraction, 
				CustomPass.RenderQueueType.Transparent => HDRenderQueue.k_RenderQueue_Transparent, 
				CustomPass.RenderQueueType.LowTransparent => HDRenderQueue.k_RenderQueue_LowTransparent, 
				CustomPass.RenderQueueType.AllTransparent => HDRenderQueue.k_RenderQueue_AllTransparent, 
				CustomPass.RenderQueueType.AllTransparentWithLowRes => HDRenderQueue.k_RenderQueue_AllTransparentWithLowRes, 
				CustomPass.RenderQueueType.AfterPostProcessTransparent => HDRenderQueue.k_RenderQueue_AfterPostProcessTransparent, 
				CustomPass.RenderQueueType.Overlay => HDRenderQueue.k_RenderQueue_Overlay, 
				_ => HDRenderQueue.k_RenderQueue_All, 
			};
		}

		public static void RenderFromCamera(in CustomPassContext ctx, Camera view, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, Material overrideMaterial = null, int overrideMaterialIndex = 0, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			RenderFromCamera(in ctx, view, null, null, ClearFlag.None, layerMask, renderQueueFilter, overrideMaterial, overrideMaterialIndex, overrideRenderState);
		}

		public static void RenderFromCamera(in CustomPassContext ctx, Camera view, RenderTexture targetRenderTexture, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, Material overrideMaterial = null, int overrideMaterialIndex = 0, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			CoreUtils.SetRenderTarget(ctx.cmd, targetRenderTexture.colorBuffer, targetRenderTexture.depthBuffer, clearFlag);
			float overrideAspectRatio = (float)targetRenderTexture.width / (float)targetRenderTexture.height;
			using (new DisableSinglePassRendering(in ctx))
			{
				using (new OverrideCameraRendering(ctx, view, overrideAspectRatio))
				{
					using (new ProfilingScope(ctx.cmd, renderFromCameraSampler))
					{
						DrawRenderers(in ctx, layerMask, renderQueueFilter, overrideMaterial, overrideMaterialIndex, overrideRenderState);
					}
				}
			}
		}

		public static void RenderFromCamera(in CustomPassContext ctx, Camera view, RTHandle targetColor, RTHandle targetDepth, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, Material overrideMaterial = null, int overrideMaterialIndex = 0, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			if (targetColor != null && targetDepth != null)
			{
				CoreUtils.SetRenderTarget(ctx.cmd, targetColor, targetDepth, clearFlag);
			}
			else if (targetColor != null)
			{
				CoreUtils.SetRenderTarget(ctx.cmd, targetColor, clearFlag);
			}
			else if (targetDepth != null)
			{
				CoreUtils.SetRenderTarget(ctx.cmd, targetDepth, clearFlag);
			}
			using (new DisableSinglePassRendering(in ctx))
			{
				using (new OverrideCameraRendering(ctx, view))
				{
					using (new ProfilingScope(ctx.cmd, renderFromCameraSampler))
					{
						DrawRenderers(in ctx, layerMask, renderQueueFilter, overrideMaterial, overrideMaterialIndex, overrideRenderState);
					}
				}
			}
		}

		public static void RenderDepthFromCamera(in CustomPassContext ctx, Camera view, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			RenderDepthFromCamera(in ctx, view, null, null, ClearFlag.None, layerMask, renderQueueFilter, overrideRenderState);
		}

		public static void RenderDepthFromCamera(in CustomPassContext ctx, Camera view, RTHandle targetColor, RTHandle targetDepth, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderDepthFromCameraSampler))
			{
				if (targetColor == null && targetDepth != null)
				{
					RenderFromCamera(in ctx, view, targetColor, targetDepth, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, depthPassIndex, overrideRenderState);
				}
				else
				{
					RenderFromCamera(in ctx, view, targetColor, targetDepth, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, depthToColorPassIndex, overrideRenderState);
				}
			}
		}

		public static void RenderDepthFromCamera(in CustomPassContext ctx, Camera view, RenderTexture targetRenderTexture, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderDepthFromCameraSampler))
			{
				if (targetRenderTexture.format == RenderTextureFormat.Depth)
				{
					RenderFromCamera(in ctx, view, targetRenderTexture, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, depthPassIndex, overrideRenderState);
				}
				else
				{
					RenderFromCamera(in ctx, view, targetRenderTexture, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, depthToColorPassIndex, overrideRenderState);
				}
			}
		}

		public static void RenderNormalFromCamera(in CustomPassContext ctx, Camera view, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			RenderNormalFromCamera(in ctx, view, null, null, ClearFlag.None, layerMask, renderQueueFilter, overrideRenderState);
		}

		public static void RenderNormalFromCamera(in CustomPassContext ctx, Camera view, RTHandle targetColor, RTHandle targetDepth, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderNormalFromCameraSampler))
			{
				RenderFromCamera(in ctx, view, targetColor, targetDepth, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, normalToColorPassIndex, overrideRenderState);
			}
		}

		public static void RenderNormalFromCamera(in CustomPassContext ctx, Camera view, RenderTexture targetRenderTexture, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderNormalFromCameraSampler))
			{
				RenderFromCamera(in ctx, view, targetRenderTexture, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, normalToColorPassIndex, overrideRenderState);
			}
		}

		public static void RenderTangentFromCamera(in CustomPassContext ctx, Camera view, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			RenderTangentFromCamera(in ctx, view, null, null, ClearFlag.None, layerMask, renderQueueFilter, overrideRenderState);
		}

		public static void RenderTangentFromCamera(in CustomPassContext ctx, Camera view, RTHandle targetColor, RTHandle targetDepth, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderTangentFromCameraSampler))
			{
				RenderFromCamera(in ctx, view, targetColor, targetDepth, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, tangentToColorPassIndex, overrideRenderState);
			}
		}

		public static void RenderTangentFromCamera(in CustomPassContext ctx, Camera view, RenderTexture targetRenderTexture, ClearFlag clearFlag, LayerMask layerMask, CustomPass.RenderQueueType renderQueueFilter = CustomPass.RenderQueueType.All, RenderStateBlock overrideRenderState = default(RenderStateBlock))
		{
			using (new ProfilingScope(ctx.cmd, renderTangentFromCameraSampler))
			{
				RenderFromCamera(in ctx, view, targetRenderTexture, clearFlag, layerMask, renderQueueFilter, customPassRenderersUtilsMaterial, tangentToColorPassIndex, overrideRenderState);
			}
		}

		internal static void Cleanup()
		{
			foreach (KeyValuePair<int, ComputeBuffer> item in gaussianWeightsCache)
			{
				item.Value.Release();
			}
			gaussianWeightsCache.Clear();
		}

		internal static void SetRenderTargetWithScaleBias(in CustomPassContext ctx, MaterialPropertyBlock block, RTHandle destination, Vector4 destScaleBias, ClearFlag clearFlag, int miplevel)
		{
			Rect viewport = default(Rect);
			if (destination.useScaling)
			{
				viewport.size = destination.GetScaledSize(destination.rtHandleProperties.currentViewportSize);
			}
			else
			{
				viewport.size = new Vector2Int(destination.rt.width, destination.rt.height);
			}
			Vector2 size = viewport.size;
			viewport.position = new Vector2(viewport.size.x * destScaleBias.z, viewport.size.y * destScaleBias.w);
			viewport.size *= new Vector2(destScaleBias.x, destScaleBias.y);
			CoreUtils.SetRenderTarget(ctx.cmd, destination, clearFlag, Color.black, miplevel);
			ctx.cmd.SetViewport(viewport);
			block.SetVector(HDShaderIDs._ViewPortSize, new Vector4(size.x, size.y, 1f / size.x, 1f / size.y));
			block.SetVector(HDShaderIDs._ViewportScaleBias, new Vector4(1f / destScaleBias.x, 1f / destScaleBias.y, destScaleBias.z, destScaleBias.w));
		}

		private static void SetSourceSize(MaterialPropertyBlock block, RTHandle source)
		{
			Vector2 vector = source.GetScaledSize(source.rtHandleProperties.currentViewportSize);
			block.SetVector(HDShaderIDs._SourceSize, new Vector4(vector.x, vector.y, 1f / vector.x, 1f / vector.y));
			block.SetVector(HDShaderIDs._SourceScaleFactor, new Vector4(source.scaleFactor.x, source.scaleFactor.y, 1f / source.scaleFactor.x, 1f / source.scaleFactor.y));
		}
	}
}
