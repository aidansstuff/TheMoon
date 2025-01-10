using System;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace UnityEngine.Experimental.Rendering
{
	internal static class XRMirrorView
	{
		private static readonly MaterialPropertyBlock s_MirrorViewMaterialProperty = new MaterialPropertyBlock();

		private static readonly ProfilingSampler k_MirrorViewProfilingSampler = new ProfilingSampler("XR Mirror View");

		private static readonly int k_SourceTex = Shader.PropertyToID("_SourceTex");

		private static readonly int k_SourceTexArraySlice = Shader.PropertyToID("_SourceTexArraySlice");

		private static readonly int k_ScaleBias = Shader.PropertyToID("_ScaleBias");

		private static readonly int k_ScaleBiasRt = Shader.PropertyToID("_ScaleBiasRt");

		private static readonly int k_SRGBRead = Shader.PropertyToID("_SRGBRead");

		private static readonly int k_SRGBWrite = Shader.PropertyToID("_SRGBWrite");

		internal static void RenderMirrorView(CommandBuffer cmd, Camera camera, Material mat, XRDisplaySubsystem display)
		{
			if ((Application.platform == RuntimePlatform.Android && !XRGraphicsAutomatedTests.running) || display == null || !display.running || mat == null)
			{
				return;
			}
			int preferredMirrorBlitMode = display.GetPreferredMirrorBlitMode();
			if (display.GetMirrorViewBlitDesc(null, out var outDesc, preferredMirrorBlitMode))
			{
				using (new ProfilingScope(cmd, k_MirrorViewProfilingSampler))
				{
					cmd.SetRenderTarget((camera.targetTexture != null) ? ((RenderTargetIdentifier)camera.targetTexture) : new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget));
					if (outDesc.nativeBlitAvailable)
					{
						display.AddGraphicsThreadMirrorViewBlit(cmd, outDesc.nativeBlitInvalidStates, preferredMirrorBlitMode);
					}
					else
					{
						for (int i = 0; i < outDesc.blitParamsCount; i++)
						{
							outDesc.GetBlitParameter(i, out var blitParameter);
							Vector4 value = new Vector4(blitParameter.srcRect.width, blitParameter.srcRect.height, blitParameter.srcRect.x, blitParameter.srcRect.y);
							Vector4 value2 = new Vector4(blitParameter.destRect.width, blitParameter.destRect.height, blitParameter.destRect.x, blitParameter.destRect.y);
							if (camera.targetTexture != null || camera.cameraType == CameraType.SceneView || camera.cameraType == CameraType.Preview)
							{
								value.y = 0f - value.y;
								value.w += blitParameter.srcRect.height;
							}
							s_MirrorViewMaterialProperty.SetFloat(k_SRGBRead, blitParameter.srcTex.sRGB ? 0f : 1f);
							s_MirrorViewMaterialProperty.SetFloat(k_SRGBWrite, (QualitySettings.activeColorSpace == ColorSpace.Linear) ? 0f : 1f);
							s_MirrorViewMaterialProperty.SetTexture(k_SourceTex, blitParameter.srcTex);
							s_MirrorViewMaterialProperty.SetVector(k_ScaleBias, value);
							s_MirrorViewMaterialProperty.SetVector(k_ScaleBiasRt, value2);
							s_MirrorViewMaterialProperty.SetFloat(k_SourceTexArraySlice, blitParameter.srcTexArraySlice);
							if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster) && blitParameter.foveatedRenderingInfo != IntPtr.Zero)
							{
								cmd.ConfigureFoveatedRendering(blitParameter.foveatedRenderingInfo);
								cmd.EnableShaderKeyword("_FOVEATED_RENDERING_NON_UNIFORM_RASTER");
							}
							int shaderPass = ((blitParameter.srcTex.dimension == TextureDimension.Tex2DArray) ? 1 : 0);
							cmd.DrawProcedural(Matrix4x4.identity, mat, shaderPass, MeshTopology.Quads, 4, 1, s_MirrorViewMaterialProperty);
						}
					}
				}
			}
			if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
			{
				cmd.DisableShaderKeyword("_FOVEATED_RENDERING_NON_UNIFORM_RASTER");
				cmd.ConfigureFoveatedRendering(IntPtr.Zero);
			}
		}
	}
}
