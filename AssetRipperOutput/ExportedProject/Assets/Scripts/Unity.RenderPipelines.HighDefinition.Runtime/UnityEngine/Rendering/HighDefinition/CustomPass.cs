using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public abstract class CustomPass : IVersionable<CustomPass.Version>
	{
		public enum TargetBuffer
		{
			Camera = 0,
			Custom = 1,
			None = 2
		}

		public enum RenderQueueType
		{
			OpaqueNoAlphaTest = 0,
			OpaqueAlphaTest = 1,
			AllOpaque = 2,
			AfterPostProcessOpaque = 3,
			PreRefraction = 4,
			Transparent = 5,
			LowTransparent = 6,
			AllTransparent = 7,
			AllTransparentWithLowRes = 8,
			AfterPostProcessTransparent = 9,
			Overlay = 11,
			All = 10
		}

		internal struct RenderTargets
		{
			public Lazy<RTHandle> customColorBuffer;

			public Lazy<RTHandle> customDepthBuffer;

			public TextureHandle colorBufferRG;

			public TextureHandle nonMSAAColorBufferRG;

			public TextureHandle depthBufferRG;

			public TextureHandle normalBufferRG;

			public TextureHandle motionVectorBufferRG;
		}

		private enum Version
		{
			Initial = 0
		}

		private class ExecutePassData
		{
			public CustomPass customPass;

			public CullingResults cullingResult;

			public CullingResults cameraCullingResult;

			public HDCamera hdCamera;
		}

		[SerializeField]
		[FormerlySerializedAs("name")]
		private string m_Name = "Custom Pass";

		private ProfilingSampler m_ProfilingSampler;

		public bool enabled = true;

		public TargetBuffer targetColorBuffer;

		public TargetBuffer targetDepthBuffer;

		public ClearFlag clearFlags;

		[SerializeField]
		private bool passFoldout;

		[NonSerialized]
		private bool isSetup;

		private bool isExecuting;

		private RenderTargets currentRenderTarget;

		private CustomPassVolume owner;

		private HDCamera currentHDCamera;

		private MaterialPropertyBlock m_MSAAResolveMPB;

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		public string name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
				m_ProfilingSampler = new ProfilingSampler(m_Name);
			}
		}

		internal ProfilingSampler profilingSampler
		{
			get
			{
				if (m_ProfilingSampler == null)
				{
					m_ProfilingSampler = new ProfilingSampler(m_Name ?? "Custom Pass");
				}
				return m_ProfilingSampler;
			}
		}

		protected float fadeValue => owner.fadeValue;

		protected CustomPassInjectionPoint injectionPoint => owner.injectionPoint;

		protected virtual bool executeInSceneView => true;

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

		private void Awake()
		{
			if (m_MSAAResolveMPB == null)
			{
				m_MSAAResolveMPB = new MaterialPropertyBlock();
			}
		}

		internal bool WillBeExecuted(HDCamera hdCamera)
		{
			if (!enabled)
			{
				return false;
			}
			if (hdCamera.camera.cameraType == CameraType.SceneView && !executeInSceneView)
			{
				return false;
			}
			return true;
		}

		private RenderTargets ReadRenderTargets(in RenderGraphBuilder builder, in RenderTargets targets)
		{
			RenderTargets result = default(RenderTargets);
			result.customColorBuffer = targets.customColorBuffer;
			result.customDepthBuffer = targets.customDepthBuffer;
			if (targets.colorBufferRG.IsValid())
			{
				result.colorBufferRG = builder.ReadWriteTexture(in targets.colorBufferRG);
			}
			if (targets.nonMSAAColorBufferRG.IsValid())
			{
				result.nonMSAAColorBufferRG = builder.ReadWriteTexture(in targets.nonMSAAColorBufferRG);
			}
			if (targets.depthBufferRG.IsValid())
			{
				result.depthBufferRG = builder.ReadWriteTexture(in targets.depthBufferRG);
			}
			if (targets.normalBufferRG.IsValid())
			{
				result.normalBufferRG = builder.ReadWriteTexture(in targets.normalBufferRG);
			}
			if (targets.motionVectorBufferRG.IsValid())
			{
				result.motionVectorBufferRG = builder.ReadWriteTexture(in targets.motionVectorBufferRG);
			}
			return result;
		}

		internal void ExecuteInternal(RenderGraph renderGraph, HDCamera hdCamera, CullingResults cullingResult, CullingResults cameraCullingResult, in RenderTargets targets, CustomPassVolume owner)
		{
			this.owner = owner;
			currentRenderTarget = targets;
			currentHDCamera = hdCamera;
			ExecutePassData passData;
			RenderGraphBuilder builder = renderGraph.AddRenderPass<ExecutePassData>(name, out passData, profilingSampler);
			try
			{
				passData.customPass = this;
				passData.cullingResult = cullingResult;
				passData.cameraCullingResult = cameraCullingResult;
				passData.hdCamera = hdCamera;
				currentRenderTarget = ReadRenderTargets(in builder, in targets);
				builder.SetRenderFunc(delegate(ExecutePassData data, RenderGraphContext ctx)
				{
					CustomPass customPass = data.customPass;
					ctx.cmd.SetGlobalFloat(HDShaderIDs._CustomPassInjectionPoint, (float)customPass.injectionPoint);
					if (customPass.currentRenderTarget.colorBufferRG.IsValid() && customPass.injectionPoint == CustomPassInjectionPoint.AfterPostProcess)
					{
						ctx.cmd.SetGlobalTexture(HDShaderIDs._AfterPostProcessColorBuffer, customPass.currentRenderTarget.colorBufferRG);
					}
					if (customPass.currentRenderTarget.motionVectorBufferRG.IsValid() && customPass.injectionPoint != 0)
					{
						ctx.cmd.SetGlobalTexture(HDShaderIDs._CameraMotionVectorsTexture, customPass.currentRenderTarget.motionVectorBufferRG);
					}
					if (customPass.currentRenderTarget.normalBufferRG.IsValid() && customPass.injectionPoint != CustomPassInjectionPoint.AfterPostProcess)
					{
						ctx.cmd.SetGlobalTexture(HDShaderIDs._NormalBufferTexture, customPass.currentRenderTarget.normalBufferRG);
					}
					if (customPass.currentRenderTarget.customColorBuffer.IsValueCreated)
					{
						ctx.cmd.SetGlobalTexture(HDShaderIDs._CustomColorTexture, customPass.currentRenderTarget.customColorBuffer.Value);
					}
					if (customPass.currentRenderTarget.customDepthBuffer.IsValueCreated)
					{
						ctx.cmd.SetGlobalTexture(HDShaderIDs._CustomDepthTexture, customPass.currentRenderTarget.customDepthBuffer.Value);
					}
					if (!customPass.isSetup)
					{
						customPass.Setup(ctx.renderContext, ctx.cmd);
						customPass.isSetup = true;
					}
					customPass.SetCustomPassTarget(ctx.cmd);
					TextureHandle colorBufferRG = customPass.currentRenderTarget.colorBufferRG;
					CustomPassContext ctx2 = new CustomPassContext(ctx.renderContext, ctx.cmd, data.hdCamera, data.cullingResult, data.cameraCullingResult, colorBufferRG, customPass.currentRenderTarget.depthBufferRG, customPass.currentRenderTarget.normalBufferRG, customPass.currentRenderTarget.motionVectorBufferRG, customPass.currentRenderTarget.customColorBuffer, customPass.currentRenderTarget.customDepthBuffer, ctx.renderGraphPool.GetTempMaterialPropertyBlock(), customPass.injectionPoint);
					customPass.isExecuting = true;
					customPass.Execute(ctx2);
					customPass.isExecuting = false;
					if (customPass.targetDepthBuffer != 0)
					{
						CoreUtils.SetRenderTarget(ctx.cmd, colorBufferRG);
					}
				});
			}
			finally
			{
				((IDisposable)builder).Dispose();
			}
		}

		internal void InternalAggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
		{
			AggregateCullingParameters(ref cullingParameters, hdCamera);
		}

		~CustomPass()
		{
			CleanupPassInternal();
		}

		internal void CleanupPassInternal()
		{
			if (isSetup)
			{
				Cleanup();
				isSetup = false;
			}
		}

		private bool IsMSAAEnabled(HDCamera hdCamera)
		{
			return hdCamera.msaaEnabled & (injectionPoint == CustomPassInjectionPoint.BeforePreRefraction || injectionPoint == CustomPassInjectionPoint.BeforeTransparent || injectionPoint == CustomPassInjectionPoint.AfterOpaqueDepthAndNormal);
		}

		private void SetCustomPassTarget(CommandBuffer cmd)
		{
			if (targetColorBuffer == TargetBuffer.None && targetDepthBuffer == TargetBuffer.None)
			{
				return;
			}
			RTHandle rTHandle = ((targetColorBuffer == TargetBuffer.Custom) ? currentRenderTarget.customColorBuffer.Value : ((RTHandle)currentRenderTarget.colorBufferRG));
			RTHandle rTHandle2 = ((targetDepthBuffer == TargetBuffer.Custom) ? currentRenderTarget.customDepthBuffer.Value : ((RTHandle)currentRenderTarget.depthBufferRG));
			if (targetColorBuffer == TargetBuffer.None && targetDepthBuffer != TargetBuffer.None)
			{
				CoreUtils.SetRenderTarget(cmd, rTHandle2, clearFlags);
				return;
			}
			if (targetColorBuffer != TargetBuffer.None && targetDepthBuffer == TargetBuffer.None)
			{
				CoreUtils.SetRenderTarget(cmd, rTHandle, clearFlags);
				return;
			}
			if (rTHandle.isMSAAEnabled != rTHandle2.isMSAAEnabled)
			{
				Debug.LogError("Color and Depth buffer MSAA flags doesn't match, no rendering will occur.");
			}
			CoreUtils.SetRenderTarget(cmd, rTHandle, rTHandle2, clearFlags);
		}

		protected virtual void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
		{
		}

		[Obsolete("This Execute signature is obsolete and will be removed in the future. Please use Execute(CustomPassContext) instead")]
		protected virtual void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
		{
		}

		protected virtual void Execute(CustomPassContext ctx)
		{
			Execute(ctx.renderContext, ctx.cmd, ctx.hdCamera, ctx.cullingResults);
		}

		protected virtual void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
		}

		protected virtual void Cleanup()
		{
		}

		[Obsolete("Use directly CoreUtils.SetRenderTarget with the render target of your choice.")]
		protected void SetCameraRenderTarget(CommandBuffer cmd, bool bindDepth = true, ClearFlag clearFlags = ClearFlag.None)
		{
			if (!isExecuting)
			{
				throw new Exception("SetCameraRenderTarget can only be called inside the CustomPass.Execute function");
			}
			RTHandle rTHandle = currentRenderTarget.colorBufferRG;
			RTHandle depthBuffer = currentRenderTarget.depthBufferRG;
			if (bindDepth)
			{
				CoreUtils.SetRenderTarget(cmd, rTHandle, depthBuffer, clearFlags);
			}
			else
			{
				CoreUtils.SetRenderTarget(cmd, rTHandle, clearFlags);
			}
		}

		[Obsolete("Use directly CoreUtils.SetRenderTarget with the render target of your choice.")]
		protected void SetCustomRenderTarget(CommandBuffer cmd, bool bindDepth = true, ClearFlag clearFlags = ClearFlag.None)
		{
			if (!isExecuting)
			{
				throw new Exception("SetCameraRenderTarget can only be called inside the CustomPass.Execute function");
			}
			if (bindDepth)
			{
				CoreUtils.SetRenderTarget(cmd, currentRenderTarget.customColorBuffer.Value, currentRenderTarget.customDepthBuffer.Value, clearFlags);
			}
			else
			{
				CoreUtils.SetRenderTarget(cmd, currentRenderTarget.customColorBuffer.Value, clearFlags);
			}
		}

		protected void SetRenderTargetAuto(CommandBuffer cmd)
		{
			SetCustomPassTarget(cmd);
		}

		protected void ResolveMSAAColorBuffer(CommandBuffer cmd, HDCamera hdCamera)
		{
			if (!isExecuting)
			{
				throw new Exception("ResolveMSAAColorBuffer can only be called inside the CustomPass.Execute function");
			}
			if (IsMSAAEnabled(hdCamera))
			{
				CoreUtils.SetRenderTarget(cmd, currentRenderTarget.nonMSAAColorBufferRG);
				m_MSAAResolveMPB.SetTexture(HDShaderIDs._ColorTextureMS, currentRenderTarget.colorBufferRG);
				cmd.DrawProcedural(Matrix4x4.identity, HDRenderPipeline.currentPipeline.GetMSAAColorResolveMaterial(), HDRenderPipeline.SampleCountToPassIndex(hdCamera.msaaSamples), MeshTopology.Triangles, 3, 1, m_MSAAResolveMPB);
			}
		}

		protected void ResolveMSAAColorBuffer(CustomPassContext ctx)
		{
			ResolveMSAAColorBuffer(ctx.cmd, ctx.hdCamera);
		}

		[Obsolete("GetCameraBuffers is obsolete and will be removed in the future. All camera buffers are now avaliable directly in the CustomPassContext in parameter of the Execute function")]
		protected void GetCameraBuffers(out RTHandle colorBuffer, out RTHandle depthBuffer)
		{
			if (!isExecuting)
			{
				throw new Exception("GetCameraBuffers can only be called inside the CustomPass.Execute function");
			}
			colorBuffer = currentRenderTarget.colorBufferRG;
			depthBuffer = currentRenderTarget.depthBufferRG;
		}

		[Obsolete("GetCustomBuffers is obsolete and will be removed in the future. All custom buffers are now avaliable directly in the CustomPassContext in parameter of the Execute function")]
		protected void GetCustomBuffers(out RTHandle colorBuffer, out RTHandle depthBuffer)
		{
			if (!isExecuting)
			{
				throw new Exception("GetCustomBuffers can only be called inside the CustomPass.Execute function");
			}
			colorBuffer = currentRenderTarget.customColorBuffer.Value;
			depthBuffer = currentRenderTarget.customDepthBuffer.Value;
		}

		[Obsolete("GetNormalBuffer is obsolete and will be removed in the future. Normal buffer is now avaliable directly in the CustomPassContext in parameter of the Execute function")]
		protected RTHandle GetNormalBuffer()
		{
			if (!isExecuting)
			{
				throw new Exception("GetNormalBuffer can only be called inside the CustomPass.Execute function");
			}
			return currentRenderTarget.normalBufferRG;
		}

		public virtual IEnumerable<Material> RegisterMaterialForInspector()
		{
			yield break;
		}

		protected RenderQueueRange GetRenderQueueRange(RenderQueueType type)
		{
			return CustomPassUtils.GetRenderQueueRangeFromRenderQueueType(type);
		}

		public static FullScreenCustomPass CreateFullScreenPass(Material fullScreenMaterial, TargetBuffer targetColorBuffer = TargetBuffer.Camera, TargetBuffer targetDepthBuffer = TargetBuffer.Camera)
		{
			return new FullScreenCustomPass
			{
				name = "FullScreen Pass",
				targetColorBuffer = targetColorBuffer,
				targetDepthBuffer = targetDepthBuffer,
				fullscreenPassMaterial = fullScreenMaterial
			};
		}

		public static DrawRenderersCustomPass CreateDrawRenderersPass(RenderQueueType queue, LayerMask mask, Material overrideMaterial, string overrideMaterialPassName = "Forward", SortingCriteria sorting = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder, ClearFlag clearFlags = ClearFlag.None, TargetBuffer targetColorBuffer = TargetBuffer.Camera, TargetBuffer targetDepthBuffer = TargetBuffer.Camera)
		{
			return new DrawRenderersCustomPass
			{
				name = "DrawRenderers Pass",
				renderQueueType = queue,
				layerMask = mask,
				overrideMaterial = overrideMaterial,
				overrideMaterialPassName = overrideMaterialPassName,
				sortingCriteria = sorting,
				clearFlags = clearFlags,
				targetColorBuffer = targetColorBuffer,
				targetDepthBuffer = targetDepthBuffer
			};
		}
	}
}
