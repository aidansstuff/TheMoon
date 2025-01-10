using Unity.Collections;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SkyRenderingContext
	{
		private SphericalHarmonicsL2 m_AmbientProbe;

		internal bool ambientProbeIsReady;

		public SphericalHarmonicsL2 ambientProbe => m_AmbientProbe;

		public ComputeBuffer ambientProbeResult { get; private set; }

		public ComputeBuffer diffuseAmbientProbeBuffer { get; private set; }

		public ComputeBuffer volumetricAmbientProbeBuffer { get; private set; }

		public ComputeBuffer cloudAmbientProbeBuffer { get; private set; }

		public RTHandle skyboxCubemapRT { get; private set; }

		public CubemapArray skyboxBSDFCubemapArray { get; private set; }

		public bool supportsConvolution { get; private set; }

		public SkyRenderingContext(int resolution, int bsdfCount, bool supportsConvolution, SphericalHarmonicsL2 ambientProbe, string name)
		{
			m_AmbientProbe = ambientProbe;
			this.supportsConvolution = supportsConvolution;
			ambientProbeResult = new ComputeBuffer(27, 4);
			volumetricAmbientProbeBuffer = new ComputeBuffer(7, 16);
			diffuseAmbientProbeBuffer = new ComputeBuffer(7, 16);
			cloudAmbientProbeBuffer = new ComputeBuffer(7, 16);
			skyboxCubemapRT = RTHandles.Alloc(resolution, resolution, 1, DepthBits.None, GraphicsFormat.R16G16B16A16_SFloat, FilterMode.Trilinear, TextureWrapMode.Repeat, TextureDimension.Cube, enableRandomWrite: false, useMipMap: true, autoGenerateMips: false, isShadowMap: false, 1, 0f, MSAASamples.None, bindTextureMS: false, useDynamicScale: false, RenderTextureMemoryless.None, VRTextureUsage.None, name);
			if (supportsConvolution)
			{
				skyboxBSDFCubemapArray = new CubemapArray(resolution, bsdfCount, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.MipChain)
				{
					hideFlags = HideFlags.HideAndDontSave,
					wrapMode = TextureWrapMode.Repeat,
					wrapModeV = TextureWrapMode.Clamp,
					filterMode = FilterMode.Trilinear,
					anisoLevel = 0,
					name = "SkyboxCubemapConvolution"
				};
			}
		}

		public void Reset()
		{
			ambientProbeIsReady = false;
		}

		public void Cleanup()
		{
			RTHandles.Release(skyboxCubemapRT);
			if (skyboxBSDFCubemapArray != null)
			{
				CoreUtils.Destroy(skyboxBSDFCubemapArray);
			}
			ambientProbeResult.Release();
			diffuseAmbientProbeBuffer.Release();
			volumetricAmbientProbeBuffer.Release();
			cloudAmbientProbeBuffer.Release();
		}

		public void ClearAmbientProbe()
		{
			m_AmbientProbe = default(SphericalHarmonicsL2);
		}

		public void UpdateAmbientProbe(in SphericalHarmonicsL2 probe)
		{
			m_AmbientProbe = probe;
		}

		public void OnComputeAmbientProbeDone(AsyncGPUReadbackRequest request)
		{
			if (request.hasError)
			{
				return;
			}
			NativeArray<float> data = request.GetData<float>();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					m_AmbientProbe[i, j] = data[i * 9 + j];
				}
			}
			ambientProbeIsReady = true;
		}
	}
}
