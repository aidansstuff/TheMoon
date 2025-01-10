using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDDynamicShadowAtlas : HDShadowAtlas
	{
		private class BlitCachedShadowPassData
		{
			public List<HDShadowRequest> requestsWaitingBlits;

			public Material blitMaterial;

			public Vector2Int cachedShadowAtlasSize;

			public TextureHandle sourceCachedAtlas;

			public TextureHandle atlasTexture;
		}

		private readonly List<HDShadowResolutionRequest> m_ShadowResolutionRequests = new List<HDShadowResolutionRequest>();

		private readonly List<HDShadowRequest> m_MixedRequestsPendingBlits = new List<HDShadowRequest>();

		private float m_RcpScaleFactor = 1f;

		private HDShadowResolutionRequest[] m_SortedRequestsCache;

		public HDDynamicShadowAtlas(HDShadowAtlasInitParameters atlaInitParams)
			: base(atlaInitParams)
		{
			m_SortedRequestsCache = new HDShadowResolutionRequest[Mathf.CeilToInt(atlaInitParams.maxShadowRequests)];
		}

		internal void ReserveResolution(HDShadowResolutionRequest shadowRequest)
		{
			m_ShadowResolutionRequests.Add(shadowRequest);
		}

		private void InsertionSort(HDShadowResolutionRequest[] array, int startIndex, int lastIndex)
		{
			for (int i = startIndex + 1; i < lastIndex; i++)
			{
				HDShadowResolutionRequest hDShadowResolutionRequest = array[i];
				int num = i - 1;
				while (num >= 0 && (hDShadowResolutionRequest.resolution.x > array[num].resolution.x || hDShadowResolutionRequest.resolution.y > array[num].resolution.y))
				{
					array[num + 1] = array[num];
					num--;
				}
				array[num + 1] = hDShadowResolutionRequest;
			}
		}

		private bool AtlasLayout(bool allowResize, HDShadowResolutionRequest[] fullShadowList, int requestsCount)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = base.width;
			float num5 = base.height;
			m_RcpScaleFactor = 1f;
			for (int i = 0; i < requestsCount; i++)
			{
				HDShadowResolutionRequest hDShadowResolutionRequest = fullShadowList[i];
				Rect dynamicAtlasViewport = new Rect(Vector2.zero, hDShadowResolutionRequest.resolution);
				num3 = Mathf.Max(num3, dynamicAtlasViewport.height);
				if (num + dynamicAtlasViewport.width > num4)
				{
					num = 0f;
					num2 += num3;
					num3 = dynamicAtlasViewport.height;
				}
				if (num2 + num3 > num5)
				{
					if (allowResize)
					{
						LayoutResize();
						return true;
					}
					return false;
				}
				dynamicAtlasViewport.x = num;
				dynamicAtlasViewport.y = num2;
				hDShadowResolutionRequest.dynamicAtlasViewport = dynamicAtlasViewport;
				hDShadowResolutionRequest.resolution = dynamicAtlasViewport.size;
				num += dynamicAtlasViewport.width;
			}
			return true;
		}

		internal bool Layout(bool allowResize = true)
		{
			if (m_ShadowResolutionRequests != null)
			{
				_ = m_ShadowResolutionRequests.Count;
			}
			int i;
			for (i = 0; i < m_ShadowResolutionRequests.Count; i++)
			{
				m_SortedRequestsCache[i] = m_ShadowResolutionRequests[i];
			}
			InsertionSort(m_SortedRequestsCache, 0, i);
			return AtlasLayout(allowResize, m_SortedRequestsCache, i);
		}

		private void LayoutResize()
		{
			int num = 0;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			while (num < m_ShadowResolutionRequests.Count)
			{
				float num6 = 0f;
				float num7 = num5;
				do
				{
					Rect dynamicAtlasViewport = new Rect(Vector2.zero, m_ShadowResolutionRequests[num].resolution);
					dynamicAtlasViewport.x = num5;
					dynamicAtlasViewport.y = num6;
					num6 += dynamicAtlasViewport.height;
					num3 = Mathf.Max(num3, num6);
					num7 = Mathf.Max(num7, num5 + dynamicAtlasViewport.width);
					m_ShadowResolutionRequests[num].dynamicAtlasViewport = dynamicAtlasViewport;
					num++;
				}
				while (num6 < num4 && num < m_ShadowResolutionRequests.Count);
				num4 = Mathf.Max(num4, num3);
				num5 = num7;
				if (num < m_ShadowResolutionRequests.Count)
				{
					float num8 = 0f;
					float num9 = num4;
					do
					{
						Rect dynamicAtlasViewport2 = new Rect(Vector2.zero, m_ShadowResolutionRequests[num].resolution);
						dynamicAtlasViewport2.x = num8;
						dynamicAtlasViewport2.y = num4;
						num8 += dynamicAtlasViewport2.width;
						num2 = Mathf.Max(num2, num8);
						num9 = Mathf.Max(num9, num4 + dynamicAtlasViewport2.height);
						m_ShadowResolutionRequests[num].dynamicAtlasViewport = dynamicAtlasViewport2;
						num++;
					}
					while (num8 < num5 && num < m_ShadowResolutionRequests.Count);
					num5 = Mathf.Max(num5, num2);
					num4 = num9;
				}
			}
			float num10 = Math.Max(num5, num4);
			Vector4 b = new Vector4((float)base.width / num10, (float)base.height / num10, (float)base.width / num10, (float)base.height / num10);
			m_RcpScaleFactor = Mathf.Min(b.x, b.y);
			foreach (HDShadowResolutionRequest shadowResolutionRequest in m_ShadowResolutionRequests)
			{
				Vector4 vector = Vector4.Scale(new Vector4(shadowResolutionRequest.dynamicAtlasViewport.x, shadowResolutionRequest.dynamicAtlasViewport.y, shadowResolutionRequest.dynamicAtlasViewport.width, shadowResolutionRequest.dynamicAtlasViewport.height), b);
				shadowResolutionRequest.dynamicAtlasViewport = new Rect(vector.x, vector.y, vector.z, vector.w);
				shadowResolutionRequest.resolution = shadowResolutionRequest.dynamicAtlasViewport.size;
			}
		}

		public void DisplayAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, Rect atlasViewport, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			base.DisplayAtlas(atlasTexture, cmd, debugMaterial, atlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb, m_RcpScaleFactor);
		}

		public void AddRequestToPendingBlitFromCache(HDShadowRequest request)
		{
			if (request.isMixedCached)
			{
				m_MixedRequestsPendingBlits.Add(request);
			}
		}

		public void BlitCachedIntoAtlas(RenderGraph renderGraph, TextureHandle cachedAtlasTexture, int cachedAtlasSize, Material blitMaterial, string passName, HDProfileId profileID)
		{
			if (m_MixedRequestsPendingBlits.Count <= 0)
			{
				return;
			}
			BlitCachedShadowPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<BlitCachedShadowPassData>(passName, out passData, ProfilingSampler.Get(profileID));
			try
			{
				passData.requestsWaitingBlits = m_MixedRequestsPendingBlits;
				passData.blitMaterial = blitMaterial;
				passData.cachedShadowAtlasSize = new Vector2Int(cachedAtlasSize, cachedAtlasSize);
				passData.sourceCachedAtlas = renderGraphBuilder.ReadTexture(in cachedAtlasTexture);
				BlitCachedShadowPassData blitCachedShadowPassData = passData;
				TextureHandle input = GetShadowMapDepthTexture(renderGraph);
				blitCachedShadowPassData.atlasTexture = renderGraphBuilder.WriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(BlitCachedShadowPassData data, RenderGraphContext ctx)
				{
					foreach (HDShadowRequest requestsWaitingBlit in data.requestsWaitingBlits)
					{
						MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
						ctx.cmd.SetRenderTarget(data.atlasTexture);
						ctx.cmd.SetViewport(requestsWaitingBlit.dynamicAtlasViewport);
						Vector4 value = new Vector4(requestsWaitingBlit.cachedAtlasViewport.width / (float)data.cachedShadowAtlasSize.x, requestsWaitingBlit.cachedAtlasViewport.height / (float)data.cachedShadowAtlasSize.y, requestsWaitingBlit.cachedAtlasViewport.x / (float)data.cachedShadowAtlasSize.x, requestsWaitingBlit.cachedAtlasViewport.y / (float)data.cachedShadowAtlasSize.y);
						tempMaterialPropertyBlock.SetTexture(HDShaderIDs._CachedShadowmapAtlas, data.sourceCachedAtlas);
						tempMaterialPropertyBlock.SetVector(HDShaderIDs._BlitScaleBias, value);
						CoreUtils.DrawFullScreen(ctx.cmd, data.blitMaterial, tempMaterialPropertyBlock);
					}
					data.requestsWaitingBlits.Clear();
				});
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		public override void Clear()
		{
			base.Clear();
			m_ShadowResolutionRequests.Clear();
			m_MixedRequestsPendingBlits.Clear();
		}
	}
}
