using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRISkyRenderer : SkyRenderer
	{
		private Material m_SkyHDRIMaterial;

		private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();

		private float scrollFactor;

		private float lastTime;

		private int m_RenderCubemapID;

		private int m_RenderFullscreenSkyID;

		private int m_RenderFullscreenSkyWithBackplateID;

		private int m_RenderDepthOnlyFullscreenSkyWithBackplateID;

		public HDRISkyRenderer()
		{
			SupportDynamicSunLight = false;
		}

		public override void Build()
		{
			m_SkyHDRIMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.hdriSkyPS);
			m_RenderCubemapID = m_SkyHDRIMaterial.FindPass("FragBaking");
			m_RenderFullscreenSkyID = m_SkyHDRIMaterial.FindPass("FragRender");
			m_RenderFullscreenSkyWithBackplateID = m_SkyHDRIMaterial.FindPass("FragRenderBackplate");
			m_RenderDepthOnlyFullscreenSkyWithBackplateID = m_SkyHDRIMaterial.FindPass("FragRenderBackplateDepth");
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_SkyHDRIMaterial);
		}

		private void GetParameters(out float intensity, out float phi, out float backplatePhi, BuiltinSkyParameters builtinParams, HDRISky hdriSky)
		{
			intensity = SkyRenderer.GetSkyIntensity(hdriSky, builtinParams.debugSettings);
			phi = -MathF.PI / 180f * hdriSky.rotation.value;
			backplatePhi = phi - MathF.PI / 180f * hdriSky.plateRotation.value;
		}

		private Vector4 GetBackplateParameters0(HDRISky hdriSky)
		{
			float num = Mathf.Abs(hdriSky.scale.value.x);
			float y = Mathf.Abs(hdriSky.scale.value.y);
			if (hdriSky.backplateType.value == BackplateType.Disc)
			{
				y = num;
			}
			return new Vector4(num, y, hdriSky.groundLevel.value, hdriSky.projectionDistance.value);
		}

		private Vector4 GetBackplateParameters1(float backplatePhi, HDRISky hdriSky)
		{
			float x = 3f;
			float y = hdriSky.blendAmount.value / 100f;
			switch (hdriSky.backplateType.value)
			{
			case BackplateType.Disc:
				x = 0f;
				break;
			case BackplateType.Rectangle:
				x = 1f;
				break;
			case BackplateType.Ellipse:
				x = 2f;
				break;
			case BackplateType.Infinite:
				x = 3f;
				y = 0f;
				break;
			}
			return new Vector4(x, y, Mathf.Cos(backplatePhi), Mathf.Sin(backplatePhi));
		}

		private Vector4 GetBackplateParameters2(HDRISky hdriSky)
		{
			float f = -MathF.PI / 180f * hdriSky.plateTexRotation.value;
			return new Vector4(Mathf.Cos(f), Mathf.Sin(f), hdriSky.plateTexOffset.value.x, hdriSky.plateTexOffset.value.y);
		}

		public override bool RequiresPreRender(SkySettings skySettings)
		{
			HDRISky hDRISky = skySettings as HDRISky;
			if (!(hDRISky != null))
			{
				return false;
			}
			return hDRISky.enableBackplate.value;
		}

		public override void PreRenderSky(BuiltinSkyParameters builtinParams)
		{
			HDRISky hDRISky = builtinParams.skySettings as HDRISky;
			GetParameters(out var intensity, out var phi, out var _, builtinParams, hDRISky);
			using (new ProfilingScope(builtinParams.commandBuffer, ProfilingSampler.Get(HDProfileId.PreRenderSky)))
			{
				m_SkyHDRIMaterial.SetTexture(HDShaderIDs._Cubemap, hDRISky.hdriSky.value);
				m_SkyHDRIMaterial.SetVector(HDShaderIDs._SkyParam, new Vector4(intensity, 0f, Mathf.Cos(phi), Mathf.Sin(phi)));
				m_SkyHDRIMaterial.SetVector(HDShaderIDs._BackplateParameters0, GetBackplateParameters0(hDRISky));
				m_PropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
				CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_SkyHDRIMaterial, m_PropertyBlock, m_RenderDepthOnlyFullscreenSkyWithBackplateID);
			}
		}

		public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
		{
			HDRISky hDRISky = builtinParams.skySettings as HDRISky;
			GetParameters(out var intensity, out var phi, out var backplatePhi, builtinParams, hDRISky);
			int shaderPassId = (renderForCubemap ? m_RenderCubemapID : (hDRISky.enableBackplate.value ? m_RenderFullscreenSkyWithBackplateID : m_RenderFullscreenSkyID));
			bool flag = builtinParams.hdCamera.frameSettings.IsEnabled(FrameSettingsField.FPTLForForwardOpaque);
			CoreUtils.SetKeyword(builtinParams.commandBuffer, "USE_FPTL_LIGHTLIST", flag);
			CoreUtils.SetKeyword(builtinParams.commandBuffer, "USE_CLUSTERED_LIGHTLIST", !flag);
			CoreUtils.SetKeyword(m_SkyHDRIMaterial, "DISTORTION_PROCEDURAL", hDRISky.distortionMode.value == HDRISky.DistortionMode.Procedural);
			CoreUtils.SetKeyword(m_SkyHDRIMaterial, "DISTORTION_FLOWMAP", hDRISky.distortionMode.value == HDRISky.DistortionMode.Flowmap);
			if (hDRISky.distortionMode.value != 0)
			{
				if (hDRISky.distortionMode.value == HDRISky.DistortionMode.Flowmap)
				{
					m_SkyHDRIMaterial.SetTexture(HDShaderIDs._Flowmap, hDRISky.flowmap.value);
				}
				HDCamera hdCamera = builtinParams.hdCamera;
				float f = MathF.PI / 180f * (hDRISky.scrollOrientation.GetValue(hdCamera) - hDRISky.rotation.value);
				bool flag2 = hDRISky.upperHemisphereOnly.value || hDRISky.distortionMode.value == HDRISky.DistortionMode.Procedural;
				Vector4 value = new Vector4(flag2 ? 1f : 0f, scrollFactor / 200f, 0f - Mathf.Cos(f), 0f - Mathf.Sin(f));
				m_SkyHDRIMaterial.SetVector(HDShaderIDs._FlowmapParam, value);
				scrollFactor += (hdCamera.animateMaterials ? (hDRISky.scrollSpeed.GetValue(hdCamera) * (hdCamera.time - lastTime) * 0.01f) : 0f);
				lastTime = hdCamera.time;
			}
			m_SkyHDRIMaterial.SetTexture(HDShaderIDs._Cubemap, hDRISky.hdriSky.value);
			m_SkyHDRIMaterial.SetVector(HDShaderIDs._SkyParam, new Vector4(intensity, 0f, Mathf.Cos(phi), Mathf.Sin(phi)));
			m_SkyHDRIMaterial.SetVector(HDShaderIDs._BackplateParameters0, GetBackplateParameters0(hDRISky));
			m_SkyHDRIMaterial.SetVector(HDShaderIDs._BackplateParameters1, GetBackplateParameters1(backplatePhi, hDRISky));
			m_SkyHDRIMaterial.SetVector(HDShaderIDs._BackplateParameters2, GetBackplateParameters2(hDRISky));
			m_SkyHDRIMaterial.SetColor(HDShaderIDs._BackplateShadowTint, hDRISky.shadowTint.value);
			uint num = 0u;
			if (hDRISky.pointLightShadow.value)
			{
				num |= 0x1000u;
			}
			if (hDRISky.dirLightShadow.value)
			{
				num |= 0x4000u;
			}
			if (hDRISky.rectLightShadow.value)
			{
				num |= 0x2000u;
			}
			m_SkyHDRIMaterial.SetInt(HDShaderIDs._BackplateShadowFilter, (int)num);
			m_PropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
			CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_SkyHDRIMaterial, m_PropertyBlock, shaderPassId);
		}
	}
}
