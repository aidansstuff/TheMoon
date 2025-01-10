using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
	internal static class MaterialExtension
	{
		public static SurfaceType GetSurfaceType(this Material material)
		{
			if (!material.HasProperty("_SurfaceType"))
			{
				return SurfaceType.Opaque;
			}
			return (SurfaceType)material.GetFloat("_SurfaceType");
		}

		public static MaterialId GetMaterialId(this Material material)
		{
			if (!material.HasProperty("_MaterialID"))
			{
				return MaterialId.LitStandard;
			}
			return (MaterialId)material.GetFloat("_MaterialID");
		}

		public static BlendMode GetBlendMode(this Material material)
		{
			if (!material.HasProperty("_BlendMode"))
			{
				return BlendMode.Additive;
			}
			return (BlendMode)material.GetFloat("_BlendMode");
		}

		public static int GetLayerCount(this Material material)
		{
			if (!material.HasProperty("_LayerCount"))
			{
				return 1;
			}
			return material.GetInt("_LayerCount");
		}

		public static bool GetZWrite(this Material material)
		{
			if (!material.HasProperty("_ZWrite"))
			{
				return false;
			}
			return material.GetInt("_ZWrite") == 1;
		}

		public static bool GetTransparentZWrite(this Material material)
		{
			if (!material.HasProperty("_TransparentZWrite"))
			{
				return false;
			}
			return material.GetInt("_TransparentZWrite") == 1;
		}

		public static CullMode GetTransparentCullMode(this Material material)
		{
			if (!material.HasProperty("_TransparentCullMode"))
			{
				return CullMode.Back;
			}
			return (CullMode)material.GetInt("_TransparentCullMode");
		}

		public static CullMode GetOpaqueCullMode(this Material material)
		{
			if (!material.HasProperty("_OpaqueCullMode"))
			{
				return CullMode.Back;
			}
			return (CullMode)material.GetInt("_OpaqueCullMode");
		}

		public static CompareFunction GetTransparentZTest(this Material material)
		{
			if (!material.HasProperty("_ZTestTransparent"))
			{
				return CompareFunction.LessEqual;
			}
			return (CompareFunction)material.GetInt("_ZTestTransparent");
		}

		public static void ResetMaterialCustomRenderQueue(this Material material)
		{
			HDRenderQueue.RenderQueueType targetType = material.GetSurfaceType() switch
			{
				SurfaceType.Opaque => HDRenderQueue.GetOpaqueEquivalent(HDRenderQueue.GetTypeByRenderQueueValue(material.renderQueue)), 
				SurfaceType.Transparent => HDRenderQueue.GetTransparentEquivalent(HDRenderQueue.GetTypeByRenderQueueValue(material.renderQueue)), 
				_ => throw new ArgumentException("Unknown SurfaceType"), 
			};
			float num = (material.HasProperty("_TransparentSortPriority") ? material.GetFloat("_TransparentSortPriority") : 0f);
			bool alphaTest = material.HasProperty("_AlphaCutoffEnable") && material.GetFloat("_AlphaCutoffEnable") > 0f;
			bool receiveDecal = material.HasProperty("_SupportDecals") && material.GetFloat("_SupportDecals") > 0f;
			material.renderQueue = HDRenderQueue.ChangeType(targetType, (int)num, alphaTest, receiveDecal);
		}

		public static void UpdateEmissiveColorFromIntensityAndEmissiveColorLDR(this Material material)
		{
			if (material.HasProperty("_EmissiveColorLDR") && material.HasProperty("_EmissiveIntensity") && material.HasProperty("_EmissiveColor"))
			{
				Color color = material.GetColor("_EmissiveColorLDR");
				Color color2 = new Color(Mathf.GammaToLinearSpace(color.r), Mathf.GammaToLinearSpace(color.g), Mathf.GammaToLinearSpace(color.b));
				material.SetColor("_EmissiveColor", color2 * material.GetFloat("_EmissiveIntensity"));
			}
		}

		public static DisplacementMode GetFilteredDisplacementMode(this Material material, DisplacementMode displacementMode)
		{
			if (material.HasProperty("_TessellationMode"))
			{
				if (displacementMode == DisplacementMode.Pixel || displacementMode == DisplacementMode.Vertex)
				{
					return DisplacementMode.None;
				}
			}
			else if (displacementMode == DisplacementMode.Tessellation)
			{
				return DisplacementMode.None;
			}
			return displacementMode;
		}

		public static bool HasPass(this Material material, string pass)
		{
			int i = 0;
			for (int passCount = material.passCount; i < passCount; i++)
			{
				if (material.GetPassName(i).Equals(pass, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}
}
