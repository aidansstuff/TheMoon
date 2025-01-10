using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class DebugLightVolumes
	{
		private class RenderLightVolumesPassData
		{
			public HDCamera hdCamera;

			public CullingResults cullResults;

			public Material debugLightVolumeMaterial;

			public ComputeShader debugLightVolumeCS;

			public int debugLightVolumeKernel;

			public int maxDebugLightCount;

			public float borderRadius;

			public Texture2D colorGradientTexture;

			public bool lightOverlapEnabled;

			public TextureHandle lightCountBuffer;

			public TextureHandle colorAccumulationBuffer;

			public TextureHandle debugLightVolumesTexture;

			public TextureHandle depthBuffer;

			public TextureHandle destination;
		}

		private Material m_Blit;

		private Material m_DebugLightVolumeMaterial;

		private ComputeShader m_DebugLightVolumeCompute;

		private int m_DebugLightVolumeGradientKernel;

		private int m_DebugLightVolumeColorsKernel;

		private Texture2D m_ColorGradientTexture;

		public static readonly int _ColorShaderID = Shader.PropertyToID("_Color");

		public static readonly int _OffsetShaderID = Shader.PropertyToID("_Offset");

		public static readonly int _RangeShaderID = Shader.PropertyToID("_Range");

		public static readonly int _DebugLightCountBufferShaderID = Shader.PropertyToID("_DebugLightCountBuffer");

		public static readonly int _DebugColorAccumulationBufferShaderID = Shader.PropertyToID("_DebugColorAccumulationBuffer");

		public static readonly int _DebugLightVolumesTextureShaderID = Shader.PropertyToID("_DebugLightVolumesTexture");

		public static readonly int _ColorGradientTextureShaderID = Shader.PropertyToID("_ColorGradientTexture");

		public static readonly int _MaxDebugLightCountShaderID = Shader.PropertyToID("_MaxDebugLightCount");

		public static readonly int _BorderRadiusShaderID = Shader.PropertyToID("_BorderRadius");

		private MaterialPropertyBlock m_MaterialProperty = new MaterialPropertyBlock();

		public void InitData(HDRenderPipelineRuntimeResources renderPipelineResources)
		{
			m_DebugLightVolumeMaterial = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.debugLightVolumePS);
			m_DebugLightVolumeCompute = renderPipelineResources.shaders.debugLightVolumeCS;
			m_DebugLightVolumeGradientKernel = m_DebugLightVolumeCompute.FindKernel("LightVolumeGradient");
			m_DebugLightVolumeColorsKernel = m_DebugLightVolumeCompute.FindKernel("LightVolumeColors");
			m_ColorGradientTexture = renderPipelineResources.textures.colorGradient;
			m_Blit = Blitter.GetBlitMaterial(TextureDimension.Tex2D);
		}

		public void ReleaseData()
		{
			CoreUtils.Destroy(m_DebugLightVolumeMaterial);
		}

		public void RenderLightVolumes(RenderGraph renderGraph, LightingDebugSettings lightingDebugSettings, TextureHandle destination, TextureHandle depthBuffer, CullingResults cullResults, HDCamera hdCamera)
		{
			RenderLightVolumesPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<RenderLightVolumesPassData>("LightVolumes", out passData);
			try
			{
				bool flag = CoreUtils.IsLightOverlapDebugEnabled(hdCamera.camera);
				bool flag2 = lightingDebugSettings.lightVolumeDebugByCategory == LightVolumeDebug.ColorAndEdge || flag;
				passData.hdCamera = hdCamera;
				passData.cullResults = cullResults;
				passData.debugLightVolumeMaterial = m_DebugLightVolumeMaterial;
				passData.debugLightVolumeCS = m_DebugLightVolumeCompute;
				passData.debugLightVolumeKernel = (flag2 ? m_DebugLightVolumeColorsKernel : m_DebugLightVolumeGradientKernel);
				passData.maxDebugLightCount = (int)lightingDebugSettings.maxDebugLightCount;
				passData.borderRadius = (flag ? 0.5f : 1f);
				passData.colorGradientTexture = m_ColorGradientTexture;
				passData.lightOverlapEnabled = flag;
				RenderLightVolumesPassData renderLightVolumesPassData = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R32_SFloat,
					clearBuffer = true,
					clearColor = Color.black,
					name = "LightVolumeCount"
				};
				renderLightVolumesPassData.lightCountBuffer = renderGraphBuilder.CreateTransientTexture(in desc);
				RenderLightVolumesPassData renderLightVolumesPassData2 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					clearBuffer = true,
					clearColor = Color.black,
					name = "LightVolumeColorAccumulation"
				};
				renderLightVolumesPassData2.colorAccumulationBuffer = renderGraphBuilder.CreateTransientTexture(in desc);
				RenderLightVolumesPassData renderLightVolumesPassData3 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					clearBuffer = true,
					clearColor = Color.black,
					enableRandomWrite = true,
					name = "LightVolumeDebugLightVolumesTexture"
				};
				renderLightVolumesPassData3.debugLightVolumesTexture = renderGraphBuilder.CreateTransientTexture(in desc);
				passData.depthBuffer = renderGraphBuilder.UseDepthBuffer(in depthBuffer, DepthAccess.ReadWrite);
				passData.destination = renderGraphBuilder.WriteTexture(in destination);
				renderGraphBuilder.SetRenderFunc(delegate(RenderLightVolumesPassData data, RenderGraphContext ctx)
				{
					MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
					RenderTargetIdentifier[] tempArray = ctx.renderGraphPool.GetTempArray<RenderTargetIdentifier>(2);
					tempArray[0] = data.lightCountBuffer;
					tempArray[1] = data.colorAccumulationBuffer;
					if (data.lightOverlapEnabled)
					{
						CoreUtils.SetRenderTarget(ctx.cmd, tempArray[0], depthBuffer);
						foreach (HDAdditionalLightData s_overlappingHDLight in HDAdditionalLightData.s_overlappingHDLights)
						{
							RenderLightVolume(ctx.cmd, data.debugLightVolumeMaterial, s_overlappingHDLight, s_overlappingHDLight.legacyLight, tempMaterialPropertyBlock);
						}
					}
					else
					{
						CoreUtils.SetRenderTarget(ctx.cmd, tempArray, depthBuffer);
						int length = data.cullResults.visibleLights.Length;
						for (int i = 0; i < length; i++)
						{
							Light light = data.cullResults.visibleLights[i].light;
							if (!(light == null))
							{
								HDAdditionalLightData component = light.GetComponent<HDAdditionalLightData>();
								if (!(component == null))
								{
									RenderLightVolume(ctx.cmd, data.debugLightVolumeMaterial, component, light, tempMaterialPropertyBlock);
								}
							}
						}
						if (!data.lightOverlapEnabled)
						{
							int length2 = data.cullResults.visibleReflectionProbes.Length;
							for (int j = 0; j < length2; j++)
							{
								ReflectionProbe reflectionProbe = data.cullResults.visibleReflectionProbes[j].reflectionProbe;
								HDAdditionalReflectionData component2 = reflectionProbe.GetComponent<HDAdditionalReflectionData>();
								if ((bool)component2)
								{
									MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
									Mesh mesh = null;
									if (component2.influenceVolume.shape == InfluenceShape.Sphere)
									{
										materialPropertyBlock.SetVector(_RangeShaderID, new Vector3(component2.influenceVolume.sphereRadius, component2.influenceVolume.sphereRadius, component2.influenceVolume.sphereRadius));
										mesh = DebugShapes.instance.RequestSphereMesh();
									}
									else
									{
										materialPropertyBlock.SetVector(_RangeShaderID, new Vector3(component2.influenceVolume.boxSize.x, component2.influenceVolume.boxSize.y, component2.influenceVolume.boxSize.z));
										mesh = DebugShapes.instance.RequestBoxMesh();
									}
									materialPropertyBlock.SetColor(_ColorShaderID, new Color(1f, 1f, 0f, 1f));
									materialPropertyBlock.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
									Matrix4x4 matrix = Matrix4x4.Translate(reflectionProbe.transform.position);
									ctx.cmd.DrawMesh(mesh, matrix, data.debugLightVolumeMaterial, 0, 0, materialPropertyBlock);
								}
							}
						}
					}
					ctx.cmd.SetComputeTextureParam(data.debugLightVolumeCS, data.debugLightVolumeKernel, _DebugLightCountBufferShaderID, data.lightCountBuffer);
					ctx.cmd.SetComputeTextureParam(data.debugLightVolumeCS, data.debugLightVolumeKernel, _DebugColorAccumulationBufferShaderID, data.colorAccumulationBuffer);
					ctx.cmd.SetComputeTextureParam(data.debugLightVolumeCS, data.debugLightVolumeKernel, _DebugLightVolumesTextureShaderID, data.debugLightVolumesTexture);
					ctx.cmd.SetComputeTextureParam(data.debugLightVolumeCS, data.debugLightVolumeKernel, _ColorGradientTextureShaderID, data.colorGradientTexture);
					ctx.cmd.SetComputeIntParam(data.debugLightVolumeCS, _MaxDebugLightCountShaderID, data.maxDebugLightCount);
					ctx.cmd.SetComputeFloatParam(data.debugLightVolumeCS, _BorderRadiusShaderID, data.borderRadius);
					int actualWidth = data.hdCamera.actualWidth;
					int actualHeight = data.hdCamera.actualHeight;
					int num = 8;
					int threadGroupsX = (actualWidth + (num - 1)) / num;
					int threadGroupsY = (actualHeight + (num - 1)) / num;
					ctx.cmd.DispatchCompute(data.debugLightVolumeCS, data.debugLightVolumeKernel, threadGroupsX, threadGroupsY, data.hdCamera.viewCount);
					CoreUtils.SetRenderTarget(ctx.cmd, destination);
					tempMaterialPropertyBlock.SetTexture(HDShaderIDs._BlitTexture, data.debugLightVolumesTexture);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.debugLightVolumeMaterial, 1, MeshTopology.Triangles, 3, 1, tempMaterialPropertyBlock);
				});
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		private static void RenderLightVolume(CommandBuffer cmd, Material debugLightVolumeMaterial, HDAdditionalLightData currentHDRLight, Light currentLegacyLight, MaterialPropertyBlock mpb)
		{
			Matrix4x4 matrix = Matrix4x4.Translate(currentLegacyLight.transform.position);
			switch (currentHDRLight.ComputeLightType(currentLegacyLight))
			{
			case HDLightType.Point:
				mpb.SetColor(_ColorShaderID, new Color(0f, 0.5f, 0f, 1f));
				mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
				mpb.SetVector(_RangeShaderID, new Vector3(currentLegacyLight.range, currentLegacyLight.range, currentLegacyLight.range));
				cmd.DrawMesh(DebugShapes.instance.RequestSphereMesh(), matrix, debugLightVolumeMaterial, 0, 0, mpb);
				break;
			case HDLightType.Spot:
				switch (currentHDRLight.spotLightShape)
				{
				case SpotLightShape.Cone:
				{
					float num2 = Mathf.Tan(currentLegacyLight.spotAngle * MathF.PI / 360f) * currentLegacyLight.range;
					mpb.SetColor(_ColorShaderID, new Color(1f, 0.5f, 0f, 1f));
					mpb.SetVector(_RangeShaderID, new Vector3(num2, num2, currentLegacyLight.range));
					mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
					cmd.DrawMesh(DebugShapes.instance.RequestConeMesh(), currentLegacyLight.gameObject.transform.localToWorldMatrix, debugLightVolumeMaterial, 0, 0, mpb);
					break;
				}
				case SpotLightShape.Box:
					mpb.SetColor(_ColorShaderID, new Color(1f, 0.5f, 0f, 1f));
					mpb.SetVector(_RangeShaderID, new Vector3(currentHDRLight.shapeWidth, currentHDRLight.shapeHeight, currentLegacyLight.range));
					mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, currentLegacyLight.range / 2f));
					cmd.DrawMesh(DebugShapes.instance.RequestBoxMesh(), currentLegacyLight.gameObject.transform.localToWorldMatrix, debugLightVolumeMaterial, 0, 0, mpb);
					break;
				case SpotLightShape.Pyramid:
				{
					float num = Mathf.Tan(currentLegacyLight.spotAngle * MathF.PI / 360f) * currentLegacyLight.range;
					mpb.SetColor(_ColorShaderID, new Color(1f, 0.5f, 0f, 1f));
					mpb.SetVector(_RangeShaderID, new Vector3(currentHDRLight.aspectRatio * num * 2f, num * 2f, currentLegacyLight.range));
					mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
					cmd.DrawMesh(DebugShapes.instance.RequestPyramidMesh(), currentLegacyLight.gameObject.transform.localToWorldMatrix, debugLightVolumeMaterial, 0, 0, mpb);
					break;
				}
				}
				break;
			case HDLightType.Area:
				switch (currentHDRLight.areaLightShape)
				{
				case AreaLightShape.Rectangle:
					mpb.SetColor(_ColorShaderID, new Color(0f, 1f, 1f, 1f));
					mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
					mpb.SetVector(_RangeShaderID, new Vector3(currentLegacyLight.range, currentLegacyLight.range, currentLegacyLight.range));
					cmd.DrawMesh(DebugShapes.instance.RequestSphereMesh(), matrix, debugLightVolumeMaterial, 0, 0, mpb);
					break;
				case AreaLightShape.Tube:
					mpb.SetColor(_ColorShaderID, new Color(1f, 0f, 0.5f, 1f));
					mpb.SetVector(_OffsetShaderID, new Vector3(0f, 0f, 0f));
					mpb.SetVector(_RangeShaderID, new Vector3(currentLegacyLight.range, currentLegacyLight.range, currentLegacyLight.range));
					cmd.DrawMesh(DebugShapes.instance.RequestSphereMesh(), matrix, debugLightVolumeMaterial, 0, 0, mpb);
					break;
				}
				break;
			case HDLightType.Directional:
				break;
			}
		}
	}
}
