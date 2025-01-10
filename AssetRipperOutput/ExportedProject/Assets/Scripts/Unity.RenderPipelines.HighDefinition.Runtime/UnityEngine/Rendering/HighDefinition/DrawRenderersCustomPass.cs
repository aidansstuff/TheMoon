using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RendererUtils;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class DrawRenderersCustomPass : CustomPass
	{
		public enum ShaderPass
		{
			DepthPrepass = 1,
			Forward = 0
		}

		public enum OverrideMaterialMode
		{
			None = 0,
			Material = 1,
			Shader = 2
		}

		[SerializeField]
		internal bool filterFoldout;

		[SerializeField]
		internal bool rendererFoldout;

		public RenderQueueType renderQueueType = RenderQueueType.AllOpaque;

		public LayerMask layerMask = 1;

		public SortingCriteria sortingCriteria = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder;

		public OverrideMaterialMode overrideMode = OverrideMaterialMode.Material;

		public Material overrideMaterial;

		[SerializeField]
		private int overrideMaterialPassIndex;

		public string overrideMaterialPassName = "Forward";

		public Shader overrideShader;

		[SerializeField]
		private int overrideShaderPassIndex;

		public string overrideShaderPassName = "Forward";

		public bool overrideDepthState;

		public CompareFunction depthCompareFunction = CompareFunction.LessEqual;

		public bool depthWrite = true;

		public bool overrideStencil;

		public int stencilReferenceValue = 64;

		public int stencilWriteMask = 192;

		public int stencilReadMask = 192;

		public CompareFunction stencilCompareFunction = CompareFunction.Always;

		public StencilOp stencilPassOperation;

		public StencilOp stencilFailOperation;

		public StencilOp stencilDepthFailOperation;

		public ShaderPass shaderPass;

		private int fadeValueId;

		private static ShaderTagId[] forwardShaderTags;

		private static ShaderTagId[] depthShaderTags;

		private ShaderTagId[] cachedShaderTagIDs;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			fadeValueId = Shader.PropertyToID("_FadeValue");
			if (string.IsNullOrEmpty(overrideMaterialPassName) && overrideMaterial != null)
			{
				overrideMaterialPassName = overrideMaterial.GetPassName(overrideMaterialPassIndex);
			}
			if (string.IsNullOrEmpty(overrideShaderPassName) && overrideShader != null)
			{
				overrideShaderPassName = new Material(overrideShader).GetPassName(overrideShaderPassIndex);
			}
			forwardShaderTags = new ShaderTagId[4]
			{
				HDShaderPassNames.s_ForwardName,
				HDShaderPassNames.s_ForwardOnlyName,
				HDShaderPassNames.s_SRPDefaultUnlitName,
				HDShaderPassNames.s_EmptyName
			};
			depthShaderTags = new ShaderTagId[3]
			{
				HDShaderPassNames.s_DepthForwardOnlyName,
				HDShaderPassNames.s_DepthOnlyName,
				HDShaderPassNames.s_EmptyName
			};
		}

		protected override void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
		{
			cullingParameters.cullingMask |= (uint)(int)layerMask;
		}

		private ShaderTagId[] GetShaderTagIds()
		{
			if (shaderPass == ShaderPass.DepthPrepass)
			{
				return depthShaderTags;
			}
			return forwardShaderTags;
		}

		protected override void Execute(CustomPassContext ctx)
		{
			ShaderTagId[] shaderTagIds = GetShaderTagIds();
			if (overrideMaterial != null)
			{
				shaderTagIds[^1] = new ShaderTagId(overrideMaterialPassName);
				overrideMaterial.SetFloat(fadeValueId, base.fadeValue);
			}
			if (shaderTagIds.Length == 0)
			{
				Debug.LogWarning("Attempt to call DrawRenderers with an empty shader passes. Skipping the call to avoid errors");
				return;
			}
			RenderStateMask renderStateMask = (overrideDepthState ? RenderStateMask.Depth : RenderStateMask.Nothing);
			renderStateMask |= ((overrideDepthState && !depthWrite) ? RenderStateMask.Stencil : RenderStateMask.Nothing);
			if (overrideStencil)
			{
				renderStateMask |= RenderStateMask.Stencil;
			}
			RenderStateBlock renderStateBlock = new RenderStateBlock(renderStateMask);
			renderStateBlock.depthState = new DepthState(depthWrite, depthCompareFunction);
			renderStateBlock.stencilState = new StencilState(overrideStencil, (byte)stencilReadMask, (byte)stencilWriteMask, stencilCompareFunction, stencilPassOperation, stencilFailOperation, stencilDepthFailOperation);
			renderStateBlock.stencilReference = (overrideStencil ? stencilReferenceValue : 0);
			RenderStateBlock value = renderStateBlock;
			PerObjectData rendererConfiguration = HDUtils.GetRendererConfiguration(ctx.hdCamera.frameSettings.IsEnabled(FrameSettingsField.ProbeVolume), ctx.hdCamera.frameSettings.IsEnabled(FrameSettingsField.Shadowmask));
			Material material = ((overrideShader != null) ? new Material(overrideShader) : null);
			RendererListDesc rendererListDesc = new RendererListDesc(shaderTagIds, ctx.cullingResults, ctx.hdCamera.camera);
			rendererListDesc.rendererConfiguration = rendererConfiguration;
			rendererListDesc.renderQueueRange = GetRenderQueueRange(renderQueueType);
			rendererListDesc.sortingCriteria = sortingCriteria;
			rendererListDesc.excludeObjectMotionVectors = false;
			rendererListDesc.overrideShader = ((overrideMode == OverrideMaterialMode.Shader) ? overrideShader : null);
			rendererListDesc.overrideMaterial = ((overrideMode == OverrideMaterialMode.Material) ? overrideMaterial : null);
			rendererListDesc.overrideMaterialPassIndex = ((overrideMaterial != null) ? overrideMaterial.FindPass(overrideMaterialPassName) : 0);
			rendererListDesc.overrideShaderPassIndex = ((overrideShader != null) ? material.FindPass(overrideShaderPassName) : 0);
			rendererListDesc.stateBlock = value;
			rendererListDesc.layerMask = layerMask;
			RendererListDesc desc = rendererListDesc;
			Object.DestroyImmediate(material);
			RendererList rendererList = ctx.renderContext.CreateRendererList(desc);
			bool opaque = renderQueueType == RenderQueueType.AllOpaque || renderQueueType == RenderQueueType.OpaqueAlphaTest || renderQueueType == RenderQueueType.OpaqueNoAlphaTest;
			HDRenderPipeline.RenderForwardRendererList(ctx.hdCamera.frameSettings, rendererList, opaque, ctx.renderContext, ctx.cmd);
		}

		public override IEnumerable<Material> RegisterMaterialForInspector()
		{
			yield return overrideMaterial;
		}
	}
}
