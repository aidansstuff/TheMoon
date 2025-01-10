namespace UnityEngine.Rendering.HighDefinition
{
	internal class MipGenerator
	{
		private RTHandle[] m_TempColorTargets;

		private RTHandle[] m_TempDownsamplePyramid;

		private ComputeShader m_DepthPyramidCS;

		private Shader m_ColorPyramidPS;

		private Material m_ColorPyramidPSMat;

		private MaterialPropertyBlock m_PropertyBlock;

		private int m_DepthDownsampleKernel;

		private int[] m_SrcOffset;

		private int[] m_DstOffset;

		private int tmpTargetCount
		{
			get
			{
				if (TextureXR.useTexArray)
				{
					return 2;
				}
				return 1;
			}
		}

		public MipGenerator(HDRenderPipelineRuntimeResources defaultResources)
		{
			m_TempColorTargets = new RTHandle[tmpTargetCount];
			m_TempDownsamplePyramid = new RTHandle[tmpTargetCount];
			m_DepthPyramidCS = defaultResources.shaders.depthPyramidCS;
			m_DepthDownsampleKernel = m_DepthPyramidCS.FindKernel("KDepthDownsample8DualUav");
			m_SrcOffset = new int[4];
			m_DstOffset = new int[4];
			m_ColorPyramidPS = defaultResources.shaders.colorPyramidPS;
			m_ColorPyramidPSMat = CoreUtils.CreateEngineMaterial(m_ColorPyramidPS);
			m_PropertyBlock = new MaterialPropertyBlock();
		}

		public void Release()
		{
			for (int i = 0; i < tmpTargetCount; i++)
			{
				RTHandles.Release(m_TempColorTargets[i]);
				m_TempColorTargets[i] = null;
				RTHandles.Release(m_TempDownsamplePyramid[i]);
				m_TempDownsamplePyramid[i] = null;
			}
			CoreUtils.Destroy(m_ColorPyramidPSMat);
		}

		public void RenderMinDepthPyramid(CommandBuffer cmd, RenderTexture texture, HDUtils.PackedMipChainInfo info, bool mip1AlreadyComputed)
		{
			HDUtils.CheckRTCreated(texture);
			ComputeShader depthPyramidCS = m_DepthPyramidCS;
			int depthDownsampleKernel = m_DepthDownsampleKernel;
			for (int i = 1; i < info.mipLevelCount; i++)
			{
				if (!mip1AlreadyComputed || i != 1)
				{
					Vector2Int vector2Int = info.mipLevelSizes[i];
					Vector2Int vector2Int2 = info.mipLevelOffsets[i];
					Vector2Int vector2Int3 = info.mipLevelSizes[i - 1];
					Vector2Int vector2Int4 = info.mipLevelOffsets[i - 1];
					Vector2Int vector2Int5 = vector2Int4 + vector2Int3 - Vector2Int.one;
					m_SrcOffset[0] = vector2Int4.x;
					m_SrcOffset[1] = vector2Int4.y;
					m_SrcOffset[2] = vector2Int5.x;
					m_SrcOffset[3] = vector2Int5.y;
					m_DstOffset[0] = vector2Int2.x;
					m_DstOffset[1] = vector2Int2.y;
					m_DstOffset[2] = 0;
					m_DstOffset[3] = 0;
					cmd.SetComputeIntParams(depthPyramidCS, HDShaderIDs._SrcOffsetAndLimit, m_SrcOffset);
					cmd.SetComputeIntParams(depthPyramidCS, HDShaderIDs._DstOffset, m_DstOffset);
					cmd.SetComputeTextureParam(depthPyramidCS, depthDownsampleKernel, HDShaderIDs._DepthMipChain, texture);
					cmd.DispatchCompute(depthPyramidCS, depthDownsampleKernel, HDUtils.DivRoundUp(vector2Int.x, 8), HDUtils.DivRoundUp(vector2Int.y, 8), texture.volumeDepth);
				}
			}
		}

		public int RenderColorGaussianPyramid(CommandBuffer cmd, Vector2Int size, Texture source, RenderTexture destination)
		{
			bool flag = source.dimension == TextureDimension.Tex2DArray;
			int num = (flag ? 1 : 0);
			if (m_TempColorTargets[num] != null && m_TempColorTargets[num].rt.graphicsFormat != destination.graphicsFormat)
			{
				RTHandles.Release(m_TempColorTargets[num]);
				m_TempColorTargets[num] = null;
			}
			if (m_TempColorTargets[num] == null)
			{
				m_TempColorTargets[num] = RTHandles.Alloc(Vector2.one * 0.5f, (!flag) ? 1 : TextureXR.slices, DepthBits.None, dimension: source.dimension, colorFormat: destination.graphicsFormat, filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Repeat, enableRandomWrite: true, useMipMap: false, autoGenerateMips: true, isShadowMap: false, anisoLevel: 1, mipMapBias: 0f, msaaSamples: MSAASamples.None, bindTextureMS: false, useDynamicScale: true, memoryless: RenderTextureMemoryless.None, vrUsage: VRTextureUsage.None, name: "Temp Gaussian Pyramid Target");
			}
			int num2 = 0;
			int num3 = size.x;
			int num4 = size.y;
			_ = destination.volumeDepth;
			if (m_TempDownsamplePyramid[num] != null && m_TempDownsamplePyramid[num].rt.graphicsFormat != destination.graphicsFormat)
			{
				RTHandles.Release(m_TempDownsamplePyramid[num]);
				m_TempDownsamplePyramid[num] = null;
			}
			if (m_TempDownsamplePyramid[num] == null)
			{
				m_TempDownsamplePyramid[num] = RTHandles.Alloc(Vector2.one * 0.5f, (!flag) ? 1 : TextureXR.slices, DepthBits.None, dimension: source.dimension, colorFormat: destination.graphicsFormat, filterMode: FilterMode.Bilinear, wrapMode: TextureWrapMode.Repeat, enableRandomWrite: false, useMipMap: false, autoGenerateMips: true, isShadowMap: false, anisoLevel: 1, mipMapBias: 0f, msaaSamples: MSAASamples.None, bindTextureMS: false, useDynamicScale: true, memoryless: RenderTextureMemoryless.None, vrUsage: VRTextureUsage.None, name: "Temporary Downsampled Pyramid");
				cmd.SetRenderTarget(m_TempDownsamplePyramid[num]);
				cmd.ClearRenderTarget(clearDepth: false, clearColor: true, Color.black);
			}
			bool flag2 = DynamicResolutionHandler.instance.HardwareDynamicResIsEnabled();
			Vector2Int size2 = new Vector2Int(source.width, source.height);
			if (flag2)
			{
				size2 = DynamicResolutionHandler.instance.ApplyScalesOnSize(size2);
			}
			float x = (float)size.x / (float)size2.x;
			float y = (float)size.y / (float)size2.y;
			m_PropertyBlock.SetTexture(HDShaderIDs._BlitTexture, source);
			m_PropertyBlock.SetVector(HDShaderIDs._BlitScaleBias, new Vector4(x, y, 0f, 0f));
			m_PropertyBlock.SetFloat(HDShaderIDs._BlitMipLevel, 0f);
			cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
			cmd.SetViewport(new Rect(0f, 0f, num3, num4));
			cmd.DrawProcedural(Matrix4x4.identity, HDUtils.GetBlitMaterial(source.dimension), 0, MeshTopology.Triangles, 3, 1, m_PropertyBlock);
			Vector2Int size3 = new Vector2Int(destination.width, destination.height);
			if (destination.useDynamicScale && flag2)
			{
				size3 = DynamicResolutionHandler.instance.ApplyScalesOnSize(size3);
			}
			while (num3 >= 8 || num4 >= 8)
			{
				int num5 = Mathf.Max(1, num3 >> 1);
				int num6 = Mathf.Max(1, num4 >> 1);
				float x2 = (float)num3 / (float)size3.x;
				float y2 = (float)num4 / (float)size3.y;
				m_PropertyBlock.SetTexture(HDShaderIDs._BlitTexture, destination);
				m_PropertyBlock.SetVector(HDShaderIDs._BlitScaleBias, new Vector4(x2, y2, 0f, 0f));
				m_PropertyBlock.SetFloat(HDShaderIDs._BlitMipLevel, num2);
				cmd.SetRenderTarget(m_TempDownsamplePyramid[num], 0, CubemapFace.Unknown, -1);
				cmd.SetViewport(new Rect(0f, 0f, num5, num6));
				cmd.DrawProcedural(Matrix4x4.identity, HDUtils.GetBlitMaterial(source.dimension), 1, MeshTopology.Triangles, 3, 1, m_PropertyBlock);
				Vector2Int size4 = new Vector2Int(m_TempDownsamplePyramid[num].rt.width, m_TempDownsamplePyramid[num].rt.height);
				if (flag2)
				{
					size4 = DynamicResolutionHandler.instance.ApplyScalesOnSize(size4);
				}
				float num7 = size4.x;
				float num8 = size4.y;
				x2 = (float)num5 / num7;
				y2 = (float)num6 / num8;
				m_PropertyBlock.SetTexture(HDShaderIDs._Source, m_TempDownsamplePyramid[num]);
				m_PropertyBlock.SetVector(HDShaderIDs._SrcScaleBias, new Vector4(x2, y2, 0f, 0f));
				m_PropertyBlock.SetVector(HDShaderIDs._SrcUvLimits, new Vector4(((float)num5 - 0.5f) / num7, ((float)num6 - 0.5f) / num8, 1f / num7, 0f));
				m_PropertyBlock.SetFloat(HDShaderIDs._SourceMip, 0f);
				cmd.SetRenderTarget(m_TempColorTargets[num], 0, CubemapFace.Unknown, -1);
				cmd.SetViewport(new Rect(0f, 0f, num5, num6));
				cmd.DrawProcedural(Matrix4x4.identity, m_ColorPyramidPSMat, num, MeshTopology.Triangles, 3, 1, m_PropertyBlock);
				m_PropertyBlock.SetTexture(HDShaderIDs._Source, m_TempColorTargets[num]);
				m_PropertyBlock.SetVector(HDShaderIDs._SrcScaleBias, new Vector4(x2, y2, 0f, 0f));
				m_PropertyBlock.SetVector(HDShaderIDs._SrcUvLimits, new Vector4(((float)num5 - 0.5f) / num7, ((float)num6 - 0.5f) / num8, 0f, 1f / num8));
				m_PropertyBlock.SetFloat(HDShaderIDs._SourceMip, 0f);
				cmd.SetRenderTarget(destination, num2 + 1, CubemapFace.Unknown, -1);
				cmd.SetViewport(new Rect(0f, 0f, num5, num6));
				cmd.DrawProcedural(Matrix4x4.identity, m_ColorPyramidPSMat, num, MeshTopology.Triangles, 3, 1, m_PropertyBlock);
				num2++;
				num3 >>= 1;
				num4 >>= 1;
				size3.x >>= 1;
				size3.y >>= 1;
			}
			return num2 + 1;
		}
	}
}
