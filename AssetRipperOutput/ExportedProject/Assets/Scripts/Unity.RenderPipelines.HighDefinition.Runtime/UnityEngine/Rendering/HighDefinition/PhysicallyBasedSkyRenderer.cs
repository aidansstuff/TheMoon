using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class PhysicallyBasedSkyRenderer : SkyRenderer
	{
		private class PrecomputationCache
		{
			private class RefCountedData
			{
				public int refCount;

				public PrecomputationData data = new PrecomputationData();
			}

			private ObjectPool<RefCountedData> m_DataPool = new ObjectPool<RefCountedData>(null, null);

			private Dictionary<int, RefCountedData> m_CachedData = new Dictionary<int, RefCountedData>();

			public PrecomputationData Get(int hash)
			{
				if (m_CachedData.TryGetValue(hash, out var value))
				{
					value.refCount++;
					return value.data;
				}
				value = m_DataPool.Get();
				value.refCount = 1;
				value.data.Allocate();
				m_CachedData.Add(hash, value);
				return value.data;
			}

			public void Release(int hash)
			{
				if (m_CachedData.TryGetValue(hash, out var value))
				{
					value.refCount--;
					if (value.refCount == 0)
					{
						value.data.Release();
						m_CachedData.Remove(hash);
						m_DataPool.Release(value);
					}
				}
			}
		}

		private class PrecomputationData
		{
			private int m_LastPrecomputedBounce;

			private int m_LastFrameComputation;

			private RTHandle[] m_GroundIrradianceTables;

			private RTHandle[] m_InScatteredRadianceTables;

			private RTHandle AllocateGroundIrradianceTable(int index)
			{
				return RTHandles.Alloc(256, 1, 1, DepthBits.None, s_ColorFormat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2D, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, $"GroundIrradianceTable{index}");
			}

			private RTHandle AllocateInScatteredRadianceTable(int index)
			{
				return RTHandles.Alloc(128, 32, 1024, DepthBits.None, s_ColorFormat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex3D, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, $"InScatteredRadianceTable{index}");
			}

			public void Allocate()
			{
				m_LastFrameComputation = -1;
				m_LastPrecomputedBounce = 0;
				m_GroundIrradianceTables = new RTHandle[2];
				m_GroundIrradianceTables[0] = AllocateGroundIrradianceTable(0);
				m_InScatteredRadianceTables = new RTHandle[5];
				m_InScatteredRadianceTables[0] = AllocateInScatteredRadianceTable(0);
				m_InScatteredRadianceTables[1] = AllocateInScatteredRadianceTable(1);
				m_InScatteredRadianceTables[2] = AllocateInScatteredRadianceTable(2);
			}

			public void Release()
			{
				RTHandles.Release(m_GroundIrradianceTables[0]);
				m_GroundIrradianceTables[0] = null;
				RTHandles.Release(m_GroundIrradianceTables[1]);
				m_GroundIrradianceTables[1] = null;
				RTHandles.Release(m_InScatteredRadianceTables[0]);
				m_InScatteredRadianceTables[0] = null;
				RTHandles.Release(m_InScatteredRadianceTables[1]);
				m_InScatteredRadianceTables[1] = null;
				RTHandles.Release(m_InScatteredRadianceTables[2]);
				m_InScatteredRadianceTables[2] = null;
				RTHandles.Release(m_InScatteredRadianceTables[3]);
				m_InScatteredRadianceTables[3] = null;
				RTHandles.Release(m_InScatteredRadianceTables[4]);
				m_InScatteredRadianceTables[4] = null;
			}

			private void PrecomputeTables(CommandBuffer cmd)
			{
				using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.InScatteredRadiancePrecomputation)))
				{
					int num = m_LastPrecomputedBounce + 1;
					int num2 = Math.Min(num - 1, 2);
					int num3 = 3;
					int num4 = Math.Min(num, 2);
					for (int i = 0; i < num4; i++)
					{
						int num5 = ((i == 0) ? num2 : num3);
						switch (num5)
						{
						case 0:
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._AirSingleScatteringTable, m_InScatteredRadianceTables[0]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._AerosolSingleScatteringTable, m_InScatteredRadianceTables[1]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTable, m_InScatteredRadianceTables[2]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTableOrder, m_InScatteredRadianceTables[3]);
							break;
						case 1:
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._AirSingleScatteringTexture, m_InScatteredRadianceTables[0]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._AerosolSingleScatteringTexture, m_InScatteredRadianceTables[1]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._GroundIrradianceTexture, m_GroundIrradianceTables[1]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTable, m_InScatteredRadianceTables[4]);
							break;
						case 2:
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTexture, m_InScatteredRadianceTables[3]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._GroundIrradianceTexture, m_GroundIrradianceTables[1]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTable, m_InScatteredRadianceTables[4]);
							break;
						case 3:
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTexture, m_InScatteredRadianceTables[4]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTableOrder, m_InScatteredRadianceTables[3]);
							cmd.SetComputeTextureParam(s_InScatteredRadiancePrecomputationCS, num5, HDShaderIDs._MultipleScatteringTable, m_InScatteredRadianceTables[2]);
							break;
						}
						cmd.DispatchCompute(s_InScatteredRadiancePrecomputationCS, num5, 32, 8, 256);
					}
					cmd.SetComputeTextureParam(s_GroundIrradiancePrecomputationCS, num2, HDShaderIDs._GroundIrradianceTable, m_GroundIrradianceTables[0]);
					cmd.SetComputeTextureParam(s_GroundIrradiancePrecomputationCS, num2, HDShaderIDs._GroundIrradianceTableOrder, m_GroundIrradianceTables[1]);
					switch (num2)
					{
					case 1:
						cmd.SetComputeTextureParam(s_GroundIrradiancePrecomputationCS, num2, HDShaderIDs._AirSingleScatteringTexture, m_InScatteredRadianceTables[0]);
						cmd.SetComputeTextureParam(s_GroundIrradiancePrecomputationCS, num2, HDShaderIDs._AerosolSingleScatteringTexture, m_InScatteredRadianceTables[1]);
						break;
					case 2:
						cmd.SetComputeTextureParam(s_GroundIrradiancePrecomputationCS, num2, HDShaderIDs._MultipleScatteringTexture, m_InScatteredRadianceTables[3]);
						break;
					}
					cmd.DispatchCompute(s_GroundIrradiancePrecomputationCS, num2, 4, 1, 1);
				}
			}

			public void BindGlobalBuffers(CommandBuffer cmd)
			{
				if (m_LastPrecomputedBounce > 0)
				{
					cmd.SetGlobalTexture(HDShaderIDs._AirSingleScatteringTexture, m_InScatteredRadianceTables[0]);
					cmd.SetGlobalTexture(HDShaderIDs._AerosolSingleScatteringTexture, m_InScatteredRadianceTables[1]);
					cmd.SetGlobalTexture(HDShaderIDs._MultipleScatteringTexture, m_InScatteredRadianceTables[2]);
				}
				else
				{
					cmd.SetGlobalTexture(HDShaderIDs._AirSingleScatteringTexture, CoreUtils.blackVolumeTexture);
					cmd.SetGlobalTexture(HDShaderIDs._AerosolSingleScatteringTexture, CoreUtils.blackVolumeTexture);
					cmd.SetGlobalTexture(HDShaderIDs._MultipleScatteringTexture, CoreUtils.blackVolumeTexture);
				}
			}

			public void BindBuffers(CommandBuffer cmd, MaterialPropertyBlock mpb)
			{
				if (m_LastPrecomputedBounce != 0)
				{
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._GroundIrradianceTexture, m_GroundIrradianceTables[0]);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._AirSingleScatteringTexture, m_InScatteredRadianceTables[0]);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._AerosolSingleScatteringTexture, m_InScatteredRadianceTables[1]);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._MultipleScatteringTexture, m_InScatteredRadianceTables[2]);
				}
				else
				{
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._GroundIrradianceTexture, Texture2D.blackTexture);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._AirSingleScatteringTexture, CoreUtils.blackVolumeTexture);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._AerosolSingleScatteringTexture, CoreUtils.blackVolumeTexture);
					s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._MultipleScatteringTexture, CoreUtils.blackVolumeTexture);
				}
			}

			public bool Update(BuiltinSkyParameters builtinParams, PhysicallyBasedSky pbrSky)
			{
				if (builtinParams.frameIndex <= m_LastFrameComputation)
				{
					return false;
				}
				m_LastFrameComputation = builtinParams.frameIndex;
				if (m_LastPrecomputedBounce == 0)
				{
					if (m_GroundIrradianceTables[1] == null)
					{
						m_GroundIrradianceTables[1] = AllocateGroundIrradianceTable(1);
					}
					if (m_InScatteredRadianceTables[3] == null)
					{
						m_InScatteredRadianceTables[3] = AllocateInScatteredRadianceTable(3);
					}
					if (m_InScatteredRadianceTables[4] == null)
					{
						m_InScatteredRadianceTables[4] = AllocateInScatteredRadianceTable(4);
					}
				}
				if (m_LastPrecomputedBounce == pbrSky.numberOfBounces.value)
				{
					RTHandles.Release(m_GroundIrradianceTables[1]);
					RTHandles.Release(m_InScatteredRadianceTables[3]);
					RTHandles.Release(m_InScatteredRadianceTables[4]);
					m_GroundIrradianceTables[1] = null;
					m_InScatteredRadianceTables[3] = null;
					m_InScatteredRadianceTables[4] = null;
				}
				if (m_LastPrecomputedBounce < pbrSky.numberOfBounces.value)
				{
					PrecomputeTables(builtinParams.commandBuffer);
					m_LastPrecomputedBounce++;
					return builtinParams.skySettings.updateMode != EnvironmentUpdateMode.Realtime;
				}
				return false;
			}
		}

		private int m_LastPrecomputationParamHash;

		private PrecomputationData m_PrecomputedData;

		private static ComputeShader s_GroundIrradiancePrecomputationCS;

		private static ComputeShader s_InScatteredRadiancePrecomputationCS;

		private Material m_PbrSkyMaterial;

		private static MaterialPropertyBlock s_PbrSkyMaterialProperties;

		private static PrecomputationCache s_PrecomputaionCache = new PrecomputationCache();

		private ShaderVariablesPhysicallyBasedSky m_ConstantBuffer;

		private int m_ShaderVariablesPhysicallyBasedSkyID = Shader.PropertyToID("ShaderVariablesPhysicallyBasedSky");

		private static GraphicsFormat s_ColorFormat = GraphicsFormat.R16G16B16A16_SFloat;

		public override void Build()
		{
			HDRenderPipelineRuntimeResources renderPipelineResources = HDRenderPipelineGlobalSettings.instance.renderPipelineResources;
			s_GroundIrradiancePrecomputationCS = renderPipelineResources.shaders.groundIrradiancePrecomputationCS;
			s_InScatteredRadiancePrecomputationCS = renderPipelineResources.shaders.inScatteredRadiancePrecomputationCS;
			s_PbrSkyMaterialProperties = new MaterialPropertyBlock();
			m_PbrSkyMaterial = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.physicallyBasedSkyPS);
		}

		public override void SetGlobalSkyData(CommandBuffer cmd, BuiltinSkyParameters builtinParams)
		{
			UpdateGlobalConstantBuffer(cmd, builtinParams);
			if (m_PrecomputedData != null)
			{
				m_PrecomputedData.BindGlobalBuffers(builtinParams.commandBuffer);
			}
		}

		public override void Cleanup()
		{
			if (m_PrecomputedData != null)
			{
				s_PrecomputaionCache.Release(m_LastPrecomputationParamHash);
				m_LastPrecomputationParamHash = 0;
				m_PrecomputedData = null;
			}
			CoreUtils.Destroy(m_PbrSkyMaterial);
		}

		private static float CornetteShanksPhasePartConstant(float anisotropy)
		{
			return 3f / (8f * MathF.PI) * (1f - anisotropy * anisotropy) / (2f + anisotropy * anisotropy);
		}

		private static Vector2 ComputeExponentialInterpolationParams(float k)
		{
			if (k == 0f)
			{
				k = 1E-06f;
			}
			float num = 10f * k;
			float y = 1f / (Mathf.Exp(num) - 1f);
			return new Vector2(num, y);
		}

		private void UpdateGlobalConstantBuffer(CommandBuffer cmd, BuiltinSkyParameters builtinParams)
		{
			PhysicallyBasedSky physicallyBasedSky = builtinParams.skySettings as PhysicallyBasedSky;
			float planetaryRadius = physicallyBasedSky.GetPlanetaryRadius();
			float maximumAltitude = physicallyBasedSky.GetMaximumAltitude();
			float airScaleHeight = physicallyBasedSky.GetAirScaleHeight();
			float aerosolScaleHeight = physicallyBasedSky.GetAerosolScaleHeight();
			float aerosolAnisotropy = physicallyBasedSky.GetAerosolAnisotropy();
			float skyIntensity = SkyRenderer.GetSkyIntensity(physicallyBasedSky, builtinParams.debugSettings);
			Vector2 vector = ComputeExponentialInterpolationParams(physicallyBasedSky.horizonZenithShift.value);
			m_ConstantBuffer._PlanetaryRadius = planetaryRadius;
			m_ConstantBuffer._RcpPlanetaryRadius = 1f / planetaryRadius;
			m_ConstantBuffer._AtmosphericDepth = maximumAltitude;
			m_ConstantBuffer._RcpAtmosphericDepth = 1f / maximumAltitude;
			m_ConstantBuffer._AtmosphericRadius = planetaryRadius + maximumAltitude;
			m_ConstantBuffer._AerosolAnisotropy = aerosolAnisotropy;
			m_ConstantBuffer._AerosolPhasePartConstant = CornetteShanksPhasePartConstant(aerosolAnisotropy);
			m_ConstantBuffer._Unused = 0f;
			m_ConstantBuffer._Unused2 = 0f;
			m_ConstantBuffer._AirDensityFalloff = 1f / airScaleHeight;
			m_ConstantBuffer._AirScaleHeight = airScaleHeight;
			m_ConstantBuffer._AerosolDensityFalloff = 1f / aerosolScaleHeight;
			m_ConstantBuffer._AerosolScaleHeight = aerosolScaleHeight;
			m_ConstantBuffer._AirSeaLevelExtinction = physicallyBasedSky.GetAirExtinctionCoefficient();
			m_ConstantBuffer._AerosolSeaLevelExtinction = physicallyBasedSky.GetAerosolExtinctionCoefficient();
			m_ConstantBuffer._AirSeaLevelScattering = physicallyBasedSky.GetAirScatteringCoefficient();
			m_ConstantBuffer._IntensityMultiplier = skyIntensity;
			m_ConstantBuffer._AerosolSeaLevelScattering = physicallyBasedSky.GetAerosolScatteringCoefficient();
			m_ConstantBuffer._ColorSaturation = physicallyBasedSky.colorSaturation.value;
			Vector3 vector2 = new Vector3(physicallyBasedSky.groundTint.value.r, physicallyBasedSky.groundTint.value.g, physicallyBasedSky.groundTint.value.b);
			m_ConstantBuffer._GroundAlbedo = vector2;
			m_ConstantBuffer._AlphaSaturation = physicallyBasedSky.alphaSaturation.value;
			m_ConstantBuffer._PlanetCenterPosition = physicallyBasedSky.GetPlanetCenterPosition(builtinParams.worldSpaceCameraPos);
			m_ConstantBuffer._AlphaMultiplier = physicallyBasedSky.alphaMultiplier.value;
			Vector3 vector3 = new Vector3(physicallyBasedSky.horizonTint.value.r, physicallyBasedSky.horizonTint.value.g, physicallyBasedSky.horizonTint.value.b);
			m_ConstantBuffer._HorizonTint = vector3;
			m_ConstantBuffer._HorizonZenithShiftPower = vector.x;
			Vector3 vector4 = new Vector3(physicallyBasedSky.zenithTint.value.r, physicallyBasedSky.zenithTint.value.g, physicallyBasedSky.zenithTint.value.b);
			m_ConstantBuffer._ZenithTint = vector4;
			m_ConstantBuffer._HorizonZenithShiftScale = vector.y;
			ConstantBuffer.PushGlobal(cmd, in m_ConstantBuffer, m_ShaderVariablesPhysicallyBasedSkyID);
		}

		protected override bool Update(BuiltinSkyParameters builtinParams)
		{
			UpdateGlobalConstantBuffer(builtinParams.commandBuffer, builtinParams);
			PhysicallyBasedSky physicallyBasedSky = builtinParams.skySettings as PhysicallyBasedSky;
			int precomputationHashCode = physicallyBasedSky.GetPrecomputationHashCode();
			if (precomputationHashCode != m_LastPrecomputationParamHash)
			{
				if (m_LastPrecomputationParamHash != 0)
				{
					s_PrecomputaionCache.Release(m_LastPrecomputationParamHash);
				}
				m_PrecomputedData = s_PrecomputaionCache.Get(precomputationHashCode);
				m_LastPrecomputationParamHash = precomputationHashCode;
			}
			return m_PrecomputedData.Update(builtinParams, physicallyBasedSky);
		}

		public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
		{
			PhysicallyBasedSky physicallyBasedSky = builtinParams.skySettings as PhysicallyBasedSky;
			Vector3 worldSpaceCameraPos = builtinParams.worldSpaceCameraPos;
			Vector3 planetCenterPosition = physicallyBasedSky.GetPlanetCenterPosition(worldSpaceCameraPos);
			float planetaryRadius = physicallyBasedSky.GetPlanetaryRadius();
			Vector3 vector = planetCenterPosition - worldSpaceCameraPos;
			float magnitude = vector.magnitude;
			worldSpaceCameraPos = planetCenterPosition - Mathf.Max(planetaryRadius, magnitude) * vector.normalized;
			bool flag = physicallyBasedSky.type.value == PhysicallyBasedSkyModel.EarthSimple;
			CommandBuffer commandBuffer = builtinParams.commandBuffer;
			Quaternion q = Quaternion.Euler(physicallyBasedSky.planetRotation.value.x, physicallyBasedSky.planetRotation.value.y, physicallyBasedSky.planetRotation.value.z);
			Quaternion q2 = Quaternion.Euler(physicallyBasedSky.spaceRotation.value.x, physicallyBasedSky.spaceRotation.value.y, physicallyBasedSky.spaceRotation.value.z);
			Matrix4x4 value = Matrix4x4.Rotate(q);
			value[0] *= -1f;
			value[1] *= -1f;
			value[2] *= -1f;
			s_PbrSkyMaterialProperties.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
			s_PbrSkyMaterialProperties.SetVector(HDShaderIDs._WorldSpaceCameraPos1, worldSpaceCameraPos);
			s_PbrSkyMaterialProperties.SetMatrix(HDShaderIDs._ViewMatrix1, builtinParams.viewMatrix);
			s_PbrSkyMaterialProperties.SetMatrix(HDShaderIDs._PlanetRotation, value);
			s_PbrSkyMaterialProperties.SetMatrix(HDShaderIDs._SpaceRotation, Matrix4x4.Rotate(q2));
			m_PrecomputedData.BindBuffers(commandBuffer, s_PbrSkyMaterialProperties);
			int value2 = 0;
			if (physicallyBasedSky.groundColorTexture.value != null && !flag)
			{
				value2 = 1;
				s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._GroundAlbedoTexture, physicallyBasedSky.groundColorTexture.value);
			}
			s_PbrSkyMaterialProperties.SetInt(HDShaderIDs._HasGroundAlbedoTexture, value2);
			int value3 = 0;
			if (physicallyBasedSky.groundEmissionTexture.value != null && !flag)
			{
				value3 = 1;
				s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._GroundEmissionTexture, physicallyBasedSky.groundEmissionTexture.value);
				s_PbrSkyMaterialProperties.SetFloat(HDShaderIDs._GroundEmissionMultiplier, physicallyBasedSky.groundEmissionMultiplier.value);
			}
			s_PbrSkyMaterialProperties.SetInt(HDShaderIDs._HasGroundEmissionTexture, value3);
			int value4 = 0;
			if (physicallyBasedSky.spaceEmissionTexture.value != null && !flag)
			{
				value4 = 1;
				s_PbrSkyMaterialProperties.SetTexture(HDShaderIDs._SpaceEmissionTexture, physicallyBasedSky.spaceEmissionTexture.value);
				s_PbrSkyMaterialProperties.SetFloat(HDShaderIDs._SpaceEmissionMultiplier, physicallyBasedSky.spaceEmissionMultiplier.value);
			}
			s_PbrSkyMaterialProperties.SetInt(HDShaderIDs._HasSpaceEmissionTexture, value4);
			s_PbrSkyMaterialProperties.SetInt(HDShaderIDs._RenderSunDisk, renderSunDisk ? 1 : 0);
			int shaderPassId = ((!renderForCubemap) ? 2 : 0);
			CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_PbrSkyMaterial, s_PbrSkyMaterialProperties, shaderPassId);
		}
	}
}
