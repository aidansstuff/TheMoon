using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	internal class EncodeBC6H
	{
		public static EncodeBC6H DefaultInstance;

		private static readonly int _Source = Shader.PropertyToID("_Source");

		private static readonly int _Target = Shader.PropertyToID("_Target");

		private static readonly int _MipIndex = Shader.PropertyToID("_MipIndex");

		private static readonly int[] __Tmp_RT = new int[14]
		{
			Shader.PropertyToID("__Tmp_RT0"),
			Shader.PropertyToID("__Tmp_RT1"),
			Shader.PropertyToID("__Tmp_RT2"),
			Shader.PropertyToID("__Tmp_RT3"),
			Shader.PropertyToID("__Tmp_RT4"),
			Shader.PropertyToID("__Tmp_RT5"),
			Shader.PropertyToID("__Tmp_RT6"),
			Shader.PropertyToID("__Tmp_RT7"),
			Shader.PropertyToID("__Tmp_RT8"),
			Shader.PropertyToID("__Tmp_RT9"),
			Shader.PropertyToID("__Tmp_RT10"),
			Shader.PropertyToID("__Tmp_RT11"),
			Shader.PropertyToID("__Tmp_RT12"),
			Shader.PropertyToID("__Tmp_RT13")
		};

		private readonly ComputeShader m_Shader;

		private readonly int m_KEncodeFastCubemapMip;

		public EncodeBC6H(ComputeShader shader)
		{
			m_Shader = shader;
			m_KEncodeFastCubemapMip = m_Shader.FindKernel("KEncodeFastCubemapMip");
		}

		public void EncodeFastCubemap(CommandBuffer cmb, RenderTargetIdentifier source, int sourceSize, RenderTargetIdentifier target, int fromMip, int toMip, int targetArrayIndex = 0)
		{
			int num = Mathf.Max(0, (int)(Mathf.Log(sourceSize) / Mathf.Log(2f)) - 2);
			int num2 = Mathf.Clamp(fromMip, 0, num);
			int num3 = Mathf.Min(num, Mathf.Max(toMip, num2));
			RenderTextureDescriptor renderTextureDescriptor = default(RenderTextureDescriptor);
			renderTextureDescriptor.autoGenerateMips = false;
			renderTextureDescriptor.bindMS = false;
			renderTextureDescriptor.graphicsFormat = GraphicsFormat.R32G32B32A32_SInt;
			renderTextureDescriptor.depthBufferBits = 0;
			renderTextureDescriptor.dimension = TextureDimension.Tex2DArray;
			renderTextureDescriptor.enableRandomWrite = true;
			renderTextureDescriptor.msaaSamples = 1;
			renderTextureDescriptor.volumeDepth = 6;
			renderTextureDescriptor.sRGB = false;
			renderTextureDescriptor.useMipMap = false;
			RenderTextureDescriptor desc = renderTextureDescriptor;
			cmb.SetComputeTextureParam(m_Shader, m_KEncodeFastCubemapMip, _Source, source);
			for (int i = num2; i <= num3; i++)
			{
				int height = (desc.width = sourceSize >> i >> 2);
				desc.height = height;
				cmb.GetTemporaryRT(__Tmp_RT[i], desc);
			}
			for (int j = num2; j <= num3; j++)
			{
				int num5 = sourceSize >> j >> 2;
				cmb.SetComputeTextureParam(m_Shader, m_KEncodeFastCubemapMip, _Target, __Tmp_RT[j]);
				cmb.SetComputeIntParam(m_Shader, _MipIndex, j);
				cmb.DispatchCompute(m_Shader, m_KEncodeFastCubemapMip, num5, num5, 6);
			}
			int num6 = 6 * targetArrayIndex;
			for (int k = num2; k <= num3; k++)
			{
				int num7 = Mathf.Clamp(k, num2, num3);
				for (int l = 0; l < 6; l++)
				{
					cmb.CopyTexture(__Tmp_RT[num7], l, 0, target, num6 + l, k);
				}
			}
			for (int m = num2; m <= num3; m++)
			{
				cmb.ReleaseTemporaryRT(__Tmp_RT[m]);
			}
		}
	}
}
