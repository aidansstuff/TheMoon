using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public sealed class BlueNoise
	{
		internal struct DitheredTextureSet
		{
			public Texture2D owenScrambled256Tex;

			public Texture2D scramblingTile;

			public Texture2D rankingTile;

			public Texture2D scramblingTex;
		}

		private readonly Texture2D[] m_Textures16L;

		private readonly Texture2D[] m_Textures16RGB;

		private Texture2DArray m_TextureArray16L;

		private Texture2DArray m_TextureArray16RGB;

		private HDRenderPipelineRuntimeResources m_RenderPipelineResources;

		private static readonly System.Random m_Random = new System.Random();

		public Texture2D[] textures16L => m_Textures16L;

		public Texture2D[] textures16RGB => m_Textures16RGB;

		public Texture2DArray textureArray16L => m_TextureArray16L;

		public Texture2DArray textureArray16RGB => m_TextureArray16RGB;

		internal BlueNoise(HDRenderPipelineRuntimeResources resources)
		{
			m_RenderPipelineResources = resources;
			InitTextures(16, TextureFormat.Alpha8, resources.textures.blueNoise16LTex, out m_Textures16L, out m_TextureArray16L);
			InitTextures(16, TextureFormat.RGB24, resources.textures.blueNoise16RGBTex, out m_Textures16RGB, out m_TextureArray16RGB);
		}

		public void Cleanup()
		{
			CoreUtils.Destroy(m_TextureArray16L);
			CoreUtils.Destroy(m_TextureArray16RGB);
			m_TextureArray16L = null;
			m_TextureArray16RGB = null;
		}

		public Texture2D GetRandom16L()
		{
			return textures16L[(int)(m_Random.NextDouble() * (double)(textures16L.Length - 1))];
		}

		public Texture2D GetRandom16RGB()
		{
			return textures16RGB[(int)(m_Random.NextDouble() * (double)(textures16RGB.Length - 1))];
		}

		private static void InitTextures(int size, TextureFormat format, Texture2D[] sourceTextures, out Texture2D[] destination, out Texture2DArray destinationArray)
		{
			int num = sourceTextures.Length;
			destination = new Texture2D[num];
			destinationArray = new Texture2DArray(size, size, num, format, mipChain: false, linear: true);
			destinationArray.hideFlags = HideFlags.HideAndDontSave;
			for (int i = 0; i < num; i++)
			{
				Texture2D texture2D = sourceTextures[i];
				if (texture2D == null)
				{
					destination[i] = Texture2D.whiteTexture;
					continue;
				}
				destination[i] = texture2D;
				Graphics.CopyTexture(texture2D, 0, 0, destinationArray, i, 0);
			}
		}

		internal void BindDitheredRNGData1SPP(CommandBuffer cmd)
		{
			cmd.SetGlobalTexture(HDShaderIDs._OwenScrambledTexture, m_RenderPipelineResources.textures.owenScrambled256Tex);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTileXSPP, m_RenderPipelineResources.textures.scramblingTile1SPP);
			cmd.SetGlobalTexture(HDShaderIDs._RankingTileXSPP, m_RenderPipelineResources.textures.rankingTile1SPP);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTexture, m_RenderPipelineResources.textures.scramblingTex);
		}

		internal void BindDitheredRNGData8SPP(CommandBuffer cmd)
		{
			cmd.SetGlobalTexture(HDShaderIDs._OwenScrambledTexture, m_RenderPipelineResources.textures.owenScrambled256Tex);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTileXSPP, m_RenderPipelineResources.textures.scramblingTile8SPP);
			cmd.SetGlobalTexture(HDShaderIDs._RankingTileXSPP, m_RenderPipelineResources.textures.rankingTile8SPP);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTexture, m_RenderPipelineResources.textures.scramblingTex);
		}

		internal DitheredTextureSet DitheredTextureSet1SPP()
		{
			DitheredTextureSet result = default(DitheredTextureSet);
			result.owenScrambled256Tex = m_RenderPipelineResources.textures.owenScrambled256Tex;
			result.scramblingTile = m_RenderPipelineResources.textures.scramblingTile1SPP;
			result.rankingTile = m_RenderPipelineResources.textures.rankingTile1SPP;
			result.scramblingTex = m_RenderPipelineResources.textures.scramblingTex;
			return result;
		}

		internal DitheredTextureSet DitheredTextureSet8SPP()
		{
			DitheredTextureSet result = default(DitheredTextureSet);
			result.owenScrambled256Tex = m_RenderPipelineResources.textures.owenScrambled256Tex;
			result.scramblingTile = m_RenderPipelineResources.textures.scramblingTile8SPP;
			result.rankingTile = m_RenderPipelineResources.textures.rankingTile8SPP;
			result.scramblingTex = m_RenderPipelineResources.textures.scramblingTex;
			return result;
		}

		internal DitheredTextureSet DitheredTextureSet256SPP()
		{
			DitheredTextureSet result = default(DitheredTextureSet);
			result.owenScrambled256Tex = m_RenderPipelineResources.textures.owenScrambled256Tex;
			result.scramblingTile = m_RenderPipelineResources.textures.scramblingTile256SPP;
			result.rankingTile = m_RenderPipelineResources.textures.rankingTile256SPP;
			result.scramblingTex = m_RenderPipelineResources.textures.scramblingTex;
			return result;
		}

		internal void BindDitheredRNGData256SPP(CommandBuffer cmd)
		{
			cmd.SetGlobalTexture(HDShaderIDs._OwenScrambledTexture, m_RenderPipelineResources.textures.owenScrambled256Tex);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTileXSPP, m_RenderPipelineResources.textures.scramblingTile256SPP);
			cmd.SetGlobalTexture(HDShaderIDs._RankingTileXSPP, m_RenderPipelineResources.textures.rankingTile256SPP);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTexture, m_RenderPipelineResources.textures.scramblingTex);
		}

		internal static void BindDitheredTextureSet(CommandBuffer cmd, DitheredTextureSet ditheredTextureSet)
		{
			cmd.SetGlobalTexture(HDShaderIDs._OwenScrambledTexture, ditheredTextureSet.owenScrambled256Tex);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTileXSPP, ditheredTextureSet.scramblingTile);
			cmd.SetGlobalTexture(HDShaderIDs._RankingTileXSPP, ditheredTextureSet.rankingTile);
			cmd.SetGlobalTexture(HDShaderIDs._ScramblingTexture, ditheredTextureSet.scramblingTex);
		}
	}
}
