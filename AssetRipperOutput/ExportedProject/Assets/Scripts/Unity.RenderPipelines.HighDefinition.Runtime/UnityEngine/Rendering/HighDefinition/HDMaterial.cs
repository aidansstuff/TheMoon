using System;
using System.Collections.Generic;
using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDMaterial
	{
		internal enum ShaderID
		{
			Lit = 0,
			LitTesselation = 1,
			LayeredLit = 2,
			LayeredLitTesselation = 3,
			Unlit = 4,
			Decal = 5,
			TerrainLit = 6,
			AxF = 7,
			Count_Standard = 8,
			SG_Unlit = 8,
			SG_Lit = 9,
			SG_Hair = 10,
			SG_Fabric = 11,
			SG_StackLit = 12,
			SG_Decal = 13,
			SG_Eye = 14,
			SG_Water = 15,
			SG_FogVolume = 16,
			Count_All = 17,
			Count_ShaderGraph = 9,
			SG_External = -1
		}

		internal delegate void MaterialResetter(Material material);

		public enum RenderingPass
		{
			BeforeRefraction = 0,
			Default = 1,
			AfterPostProcess = 2,
			LowResolution = 3
		}

		internal static readonly string[] s_ShaderPaths = new string[8] { "HDRP/Lit", "HDRP/LitTessellation", "HDRP/LayeredLit", "HDRP/LayeredLitTessellation", "HDRP/Unlit", "HDRP/Decal", "HDRP/TerrainLit", "HDRP/AxF" };

		internal static readonly string[] s_SubTargetIds = new string[9] { "HDUnlitSubTarget", "HDLitSubTarget", "HairSubTarget", "FabricSubTarget", "StackLitSubTarget", "DecalSubTarget", "EyeSubTarget", "WaterSubTarget", "FogVolumeSubTarget" };

		internal static Dictionary<ShaderID, MaterialResetter> k_PlainShadersMaterialResetters = new Dictionary<ShaderID, MaterialResetter>
		{
			{
				ShaderID.Lit,
				LitAPI.ValidateMaterial
			},
			{
				ShaderID.LitTesselation,
				LitAPI.ValidateMaterial
			},
			{
				ShaderID.LayeredLit,
				LayeredLitAPI.ValidateMaterial
			},
			{
				ShaderID.LayeredLitTesselation,
				LayeredLitAPI.ValidateMaterial
			},
			{
				ShaderID.Unlit,
				UnlitAPI.ValidateMaterial
			},
			{
				ShaderID.Decal,
				DecalAPI.ValidateMaterial
			},
			{
				ShaderID.TerrainLit,
				TerrainLitAPI.ValidateMaterial
			},
			{
				ShaderID.AxF,
				AxFAPI.ValidateMaterial
			},
			{
				ShaderID.Count_Standard,
				ShaderGraphAPI.ValidateUnlitMaterial
			},
			{
				ShaderID.SG_Lit,
				ShaderGraphAPI.ValidateLightingMaterial
			},
			{
				ShaderID.SG_Hair,
				ShaderGraphAPI.ValidateLightingMaterial
			},
			{
				ShaderID.SG_Fabric,
				ShaderGraphAPI.ValidateLightingMaterial
			},
			{
				ShaderID.SG_StackLit,
				ShaderGraphAPI.ValidateLightingMaterial
			},
			{
				ShaderID.SG_Decal,
				ShaderGraphAPI.ValidateDecalMaterial
			},
			{
				ShaderID.SG_Eye,
				ShaderGraphAPI.ValidateLightingMaterial
			},
			{
				ShaderID.SG_FogVolume,
				ShaderGraphAPI.ValidateFogVolumeMaterial
			}
		};

		internal static ShaderID GetShaderID(Material material)
		{
			if (!IsShaderGraph(material))
			{
				string shaderName = material.shader.name;
				return (ShaderID)Array.FindIndex(s_ShaderPaths, (string m) => m == shaderName);
			}
			string subTarget = material.GetTag("ShaderGraphTargetId", searchFallbacks: false, null);
			int num = Array.FindIndex(s_SubTargetIds, (string m) => m == subTarget);
			if (num != -1)
			{
				return (ShaderID)(num + 8);
			}
			return ShaderID.SG_External;
		}

		internal static void RemoveMaterialKeyword(Material material, ShaderID shaderID)
		{
			if (ShaderID.Lit <= shaderID && shaderID < ShaderID.Count_Standard)
			{
				material.shaderKeywords = null;
			}
		}

		public static bool ValidateMaterial(Material material)
		{
			ShaderID shaderID = GetShaderID(material);
			k_PlainShadersMaterialResetters.TryGetValue(shaderID, out var value);
			if (value == null)
			{
				return false;
			}
			RemoveMaterialKeyword(material, shaderID);
			value(material);
			return true;
		}

		private static HDRenderQueue.RenderQueueType RenderingPassToQueue(RenderingPass pass, bool isTransparent)
		{
			switch (pass)
			{
			case RenderingPass.Default:
				if (!isTransparent)
				{
					return HDRenderQueue.RenderQueueType.Opaque;
				}
				return HDRenderQueue.RenderQueueType.Transparent;
			case RenderingPass.AfterPostProcess:
				if (!isTransparent)
				{
					return HDRenderQueue.RenderQueueType.AfterPostProcessOpaque;
				}
				return HDRenderQueue.RenderQueueType.AfterPostprocessTransparent;
			case RenderingPass.BeforeRefraction:
				if (!isTransparent)
				{
					return HDRenderQueue.RenderQueueType.Opaque;
				}
				return HDRenderQueue.RenderQueueType.PreRefraction;
			case RenderingPass.LowResolution:
				if (!isTransparent)
				{
					return HDRenderQueue.RenderQueueType.Opaque;
				}
				return HDRenderQueue.RenderQueueType.LowTransparent;
			default:
				return HDRenderQueue.RenderQueueType.Unknown;
			}
		}

		public static void SetSurfaceType(Material material, bool transparent)
		{
			SurfaceType surfaceType = (transparent ? SurfaceType.Transparent : SurfaceType.Opaque);
			material.SetFloat("_SurfaceType", (float)surfaceType);
			ValidateMaterial(material);
		}

		public static void SetRenderingPass(Material material, RenderingPass value)
		{
			bool isTransparent = (int)material.GetFloat("_SurfaceType") == 1;
			HDRenderQueue.RenderQueueType targetType = RenderingPassToQueue(value, isTransparent);
			int offset = (material.HasProperty("_TransparentSortPriority") ? ((int)material.GetFloat("_TransparentSortPriority")) : 0);
			bool alphaTest = material.HasProperty("_AlphaCutoffEnable") && material.GetFloat("_AlphaCutoffEnable") > 0f;
			bool receiveDecal = material.HasProperty("_SupportDecals") && material.GetFloat("_SupportDecals") > 0f;
			material.renderQueue = HDRenderQueue.ChangeType(targetType, offset, alphaTest, receiveDecal);
		}

		public static void SetEmissiveColor(Material material, Color value)
		{
			if (material.GetFloat("_UseEmissiveIntensity") > 0f)
			{
				material.SetColor("_EmissiveColorLDR", value);
				material.SetColor("_EmissiveColor", value.linear * material.GetFloat("_EmissiveIntensity"));
				return;
			}
			if (material.HasProperty("_EmissiveColorHDR"))
			{
				material.SetColor("_EmissiveColorHDR", value);
			}
			material.SetColor("_EmissiveColor", value);
		}

		public static void SetUseEmissiveIntensity(Material material, bool value)
		{
			material.SetFloat("_UseEmissiveIntensity", value ? 1f : 0f);
			if (value)
			{
				material.UpdateEmissiveColorFromIntensityAndEmissiveColorLDR();
			}
			else if (material.HasProperty("_EmissiveColorHDR"))
			{
				material.SetColor("_EmissiveColor", material.GetColor("_EmissiveColorHDR"));
			}
		}

		public static bool GetUseEmissiveIntensity(Material material)
		{
			return material.GetFloat("_UseEmissiveIntensity") > 0f;
		}

		public static void SetEmissiveIntensity(Material material, float intensity, EmissiveIntensityUnit unit)
		{
			if (unit == EmissiveIntensityUnit.EV100)
			{
				intensity = LightUtils.ConvertEvToLuminance(intensity);
			}
			material.SetFloat("_EmissiveIntensity", intensity);
			material.SetFloat("_EmissiveIntensityUnit", (float)unit);
			if (material.GetFloat("_UseEmissiveIntensity") > 0f)
			{
				material.SetColor("_EmissiveColor", material.GetColor("_EmissiveColorLDR").linear * intensity);
			}
		}

		public static void SetAlphaClipping(Material material, bool value)
		{
			material.SetFloat("_AlphaCutoffEnable", value ? 1f : 0f);
			material.SetupBaseUnlitKeywords();
		}

		public static void SetAlphaCutoff(Material material, float cutoff)
		{
			material.SetFloat("_AlphaCutoff", cutoff);
			material.SetFloat("_Cutoff", cutoff);
		}

		public static void SetDiffusionProfile(Material material, DiffusionProfileSettings profile)
		{
			float value = ((profile != null) ? HDShadowUtils.Asfloat(profile.profile.hash) : 0f);
			material.SetFloat(HDShaderIDs._DiffusionProfileHash, value);
		}

		public static void SetDiffusionProfileShaderGraph(Material material, DiffusionProfileSettings profile, string referenceName)
		{
			float value = ((profile != null) ? HDShadowUtils.Asfloat(profile.profile.hash) : 0f);
			material.SetFloat(referenceName, value);
		}

		internal static bool IsShaderGraph(Material material)
		{
			return !string.IsNullOrEmpty(material.GetTag("ShaderGraphShader", searchFallbacks: false, null));
		}
	}
}
