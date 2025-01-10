using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
	internal static class BaseUnlitAPI
	{
		public static void SetupBaseUnlitKeywords(this Material material)
		{
			material.ResetMaterialCustomRenderQueue();
			bool flag = material.HasProperty("_AlphaCutoffEnable") && material.GetFloat("_AlphaCutoffEnable") > 0f;
			CoreUtils.SetKeyword(material, "_ALPHATEST_ON", flag);
			SurfaceType surfaceType = material.GetSurfaceType();
			CoreUtils.SetKeyword(material, "_SURFACE_TYPE_TRANSPARENT", surfaceType == SurfaceType.Transparent);
			bool state = surfaceType == SurfaceType.Transparent && material.HasProperty("_TransparentWritingMotionVec") && material.GetInt("_TransparentWritingMotionVec") > 0;
			CoreUtils.SetKeyword(material, "_TRANSPARENT_WRITES_MOTION_VEC", state);
			if (material.HasProperty("_AddPrecomputedVelocity"))
			{
				CoreUtils.SetKeyword(material, "_ADD_PRECOMPUTED_VELOCITY", material.GetInt("_AddPrecomputedVelocity") != 0);
			}
			HDRenderQueue.RenderQueueType typeByRenderQueueValue = HDRenderQueue.GetTypeByRenderQueueValue(material.renderQueue);
			bool flag2 = typeByRenderQueueValue == HDRenderQueue.RenderQueueType.AfterPostprocessTransparent || typeByRenderQueueValue == HDRenderQueue.RenderQueueType.LowTransparent;
			if (material.HasProperty("_ZTestGBuffer"))
			{
				if (flag)
				{
					material.SetInt("_ZTestGBuffer", 3);
				}
				else
				{
					material.SetInt("_ZTestGBuffer", 4);
				}
			}
			if (material.HasProperty("_ZTestDepthEqualForOpaque"))
			{
				if (surfaceType == SurfaceType.Opaque)
				{
					if (HDRenderQueue.k_RenderQueue_AfterPostProcessOpaque.Contains(material.renderQueue))
					{
						material.SetInt("_ZTestDepthEqualForOpaque", 4);
					}
					else
					{
						material.SetInt("_ZTestDepthEqualForOpaque", 3);
					}
				}
				else
				{
					material.SetInt("_ZTestDepthEqualForOpaque", (int)material.GetTransparentZTest());
				}
			}
			if (surfaceType == SurfaceType.Opaque)
			{
				material.SetOverrideTag("RenderType", flag ? "TransparentCutout" : "");
				material.SetInt("_SrcBlend", 1);
				material.SetInt("_DstBlend", 0);
				material.SetInt("_AlphaSrcBlend", 1);
				material.SetInt("_AlphaDstBlend", 0);
				material.SetInt("_ZWrite", 1);
			}
			else
			{
				material.SetOverrideTag("RenderType", "Transparent");
				material.SetInt("_ZWrite", material.GetTransparentZWrite() ? 1 : 0);
				if (material.HasProperty("_BlendMode"))
				{
					switch (material.GetBlendMode())
					{
					case BlendMode.Alpha:
						material.SetInt("_SrcBlend", 1);
						material.SetInt("_DstBlend", 10);
						if (flag2)
						{
							material.SetInt("_AlphaSrcBlend", 0);
							material.SetInt("_AlphaDstBlend", 10);
						}
						else
						{
							material.SetInt("_AlphaSrcBlend", 1);
							material.SetInt("_AlphaDstBlend", 10);
						}
						break;
					case BlendMode.Additive:
						material.SetInt("_SrcBlend", 1);
						material.SetInt("_DstBlend", 1);
						if (flag2)
						{
							material.SetInt("_AlphaSrcBlend", 0);
							material.SetInt("_AlphaDstBlend", 1);
						}
						else
						{
							material.SetInt("_AlphaSrcBlend", 1);
							material.SetInt("_AlphaDstBlend", 1);
						}
						break;
					case BlendMode.Premultiply:
						material.SetInt("_SrcBlend", 1);
						material.SetInt("_DstBlend", 10);
						if (flag2)
						{
							material.SetInt("_AlphaSrcBlend", 0);
							material.SetInt("_AlphaDstBlend", 10);
						}
						else
						{
							material.SetInt("_AlphaSrcBlend", 1);
							material.SetInt("_AlphaDstBlend", 10);
						}
						break;
					}
				}
			}
			bool state2 = material.HasProperty("_EnableFogOnTransparent") && material.GetFloat("_EnableFogOnTransparent") > 0f && surfaceType == SurfaceType.Transparent;
			CoreUtils.SetKeyword(material, "_ENABLE_FOG_ON_TRANSPARENT", state2);
			if (material.HasProperty("_DistortionEnable") && material.HasProperty("_DistortionBlendMode"))
			{
				bool flag3 = material.GetFloat("_DistortionDepthTest") > 0f;
				if (material.HasProperty("_ZTestModeDistortion"))
				{
					if (flag3)
					{
						material.SetInt("_ZTestModeDistortion", 4);
					}
					else
					{
						material.SetInt("_ZTestModeDistortion", 8);
					}
				}
				switch (material.GetInt("_DistortionBlendMode"))
				{
				default:
					material.SetInt("_DistortionSrcBlend", 1);
					material.SetInt("_DistortionDstBlend", 1);
					material.SetInt("_DistortionBlurSrcBlend", 1);
					material.SetInt("_DistortionBlurDstBlend", 1);
					material.SetInt("_DistortionBlurBlendOp", 0);
					break;
				case 1:
					material.SetInt("_DistortionSrcBlend", 2);
					material.SetInt("_DistortionDstBlend", 0);
					material.SetInt("_DistortionBlurSrcBlend", 7);
					material.SetInt("_DistortionBlurDstBlend", 0);
					material.SetInt("_DistortionBlurBlendOp", 0);
					break;
				case 2:
					material.SetInt("_DistortionSrcBlend", 1);
					material.SetInt("_DistortionDstBlend", 0);
					material.SetInt("_DistortionBlurSrcBlend", 1);
					material.SetInt("_DistortionBlurDstBlend", 0);
					material.SetInt("_DistortionBlurBlendOp", 0);
					break;
				}
			}
			CullMode cullMode = ((surfaceType == SurfaceType.Transparent) ? material.GetTransparentCullMode() : material.GetOpaqueCullMode());
			bool num = material.HasProperty("_TransparentBackfaceEnable") && material.GetFloat("_TransparentBackfaceEnable") > 0f && surfaceType == SurfaceType.Transparent;
			bool flag4 = material.HasProperty("_DoubleSidedEnable") && material.GetFloat("_DoubleSidedEnable") > 0f;
			DoubleSidedGIMode doubleSidedGIMode = DoubleSidedGIMode.Auto;
			if (material.HasProperty("_DoubleSidedGIMode"))
			{
				doubleSidedGIMode = (DoubleSidedGIMode)material.GetFloat("_DoubleSidedGIMode");
			}
			material.SetInt("_CullMode", (int)((!flag4) ? cullMode : CullMode.Off));
			if (num)
			{
				material.SetInt("_CullModeForward", 2);
			}
			else
			{
				material.SetInt("_CullModeForward", (int)((!flag4) ? cullMode : CullMode.Off));
			}
			CoreUtils.SetKeyword(material, "_DOUBLESIDED_ON", flag4);
			if (material.HasProperty("_EmissionColor"))
			{
				material.SetColor("_EmissionColor", Color.white);
			}
			material.SetupMainTexForAlphaTestGI("_UnlitColorMap", "_UnlitColor");
			if (!material.HasProperty("_DisplacementMode") && material.HasProperty("_DepthOffsetEnable"))
			{
				bool state3 = material.GetFloat("_DepthOffsetEnable") > 0f;
				CoreUtils.SetKeyword(material, "_DEPTHOFFSET_ON", state3);
				if (material.HasProperty("_ConservativeDepthOffsetEnable"))
				{
					bool state4 = material.GetFloat("_ConservativeDepthOffsetEnable") > 0f;
					CoreUtils.SetKeyword(material, "_CONSERVATIVE_DEPTH_OFFSET", state4);
				}
			}
			if (material.HasProperty("_TessellationMode"))
			{
				TessellationMode tessellationMode = (TessellationMode)material.GetFloat("_TessellationMode");
				CoreUtils.SetKeyword(material, "_TESSELLATION_PHONG", tessellationMode == TessellationMode.Phong);
			}
			switch (doubleSidedGIMode)
			{
			case DoubleSidedGIMode.Auto:
				material.doubleSidedGI = flag4;
				break;
			case DoubleSidedGIMode.On:
				material.doubleSidedGI = true;
				break;
			case DoubleSidedGIMode.Off:
				material.doubleSidedGI = false;
				break;
			}
		}

		public static void SetupMainTexForAlphaTestGI(this Material material, string colorMapPropertyName, string colorPropertyName)
		{
			if (material.HasProperty(colorMapPropertyName))
			{
				Texture texture = material.GetTexture(colorMapPropertyName);
				Vector2 textureScale = material.GetTextureScale(colorMapPropertyName);
				Vector2 textureOffset = material.GetTextureOffset(colorMapPropertyName);
				material.SetTexture("_MainTex", texture);
				material.SetTextureScale("_MainTex", textureScale);
				material.SetTextureOffset("_MainTex", textureOffset);
			}
			if (material.HasProperty(colorPropertyName))
			{
				Color color = material.GetColor(colorPropertyName);
				material.SetColor("_Color", color);
			}
			if (material.HasProperty("_AlphaCutoff"))
			{
				float @float = material.GetFloat("_AlphaCutoff");
				material.SetFloat("_Cutoff", @float);
			}
		}

		public static void SetupBaseUnlitPass(this Material material)
		{
			if (HDMaterial.IsShaderGraph(material))
			{
				material.SetShaderPassEnabled(HDShaderPassNames.s_DistortionVectorsStr, enabled: true);
			}
			else if (material.HasProperty("_DistortionEnable"))
			{
				bool flag = material.GetFloat("_DistortionEnable") > 0f && (int)material.GetFloat("_SurfaceType") == 1;
				bool flag2 = false;
				if (material.HasProperty("_DistortionOnly"))
				{
					flag2 = material.GetFloat("_DistortionOnly") > 0f;
				}
				bool enabled = !(flag && flag2);
				material.SetShaderPassEnabled(HDShaderPassNames.s_ForwardStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_DepthOnlyStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_DepthForwardOnlyStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_ForwardOnlyStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_GBufferStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_GBufferWithPrepassStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_DistortionVectorsStr, flag);
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentDepthPrepassStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentBackfaceStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentDepthPostpassStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_RayTracingPrepassStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_MetaStr, enabled);
				material.SetShaderPassEnabled(HDShaderPassNames.s_ShadowCasterStr, enabled);
			}
			if (material.HasProperty("_TransparentDepthPrepassEnable"))
			{
				bool flag3 = material.GetFloat("_TransparentDepthPrepassEnable") > 0f && (int)material.GetFloat("_SurfaceType") == 1;
				bool flag4 = material.HasProperty("_ReceivesSSRTransparent") && material.GetFloat("_ReceivesSSRTransparent") > 0f && (int)material.GetFloat("_SurfaceType") == 1;
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentDepthPrepassStr, flag3 || flag4);
			}
			if (material.HasProperty("_TransparentDepthPostpassEnable"))
			{
				bool enabled2 = material.GetFloat("_TransparentDepthPostpassEnable") > 0f && (int)material.GetFloat("_SurfaceType") == 1;
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentDepthPostpassStr, enabled2);
			}
			if (material.HasProperty("_TransparentBackfaceEnable"))
			{
				bool enabled3 = material.GetFloat("_TransparentBackfaceEnable") > 0f && (int)material.GetFloat("_SurfaceType") == 1;
				material.SetShaderPassEnabled(HDShaderPassNames.s_TransparentBackfaceStr, enabled3);
			}
			if (material.HasProperty("_RayTracing"))
			{
				bool enabled4 = material.GetFloat("_RayTracing") > 0f;
				material.SetShaderPassEnabled(HDShaderPassNames.s_RayTracingPrepassStr, enabled4);
			}
			if (!HDMaterial.IsShaderGraph(material))
			{
				bool enabled5 = false;
				if (material.HasProperty("_AddPrecomputedVelocity"))
				{
					enabled5 = material.GetInt("_AddPrecomputedVelocity") != 0;
				}
				material.SetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr, enabled5);
			}
		}
	}
}
