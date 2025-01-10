using System;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct XRSinglePassScope : IDisposable
	{
		private readonly RenderGraph m_RenderGraph;

		private readonly HDCamera m_HDCamera;

		private bool m_Disposed;

		public XRSinglePassScope(RenderGraph renderGraph, HDCamera hdCamera)
		{
			m_RenderGraph = renderGraph;
			m_HDCamera = hdCamera;
			m_Disposed = false;
			HDRenderPipeline.StartXRSinglePass(renderGraph, hdCamera);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		private void Dispose(bool disposing)
		{
			if (!m_Disposed)
			{
				if (disposing)
				{
					HDRenderPipeline.StopXRSinglePass(m_RenderGraph, m_HDCamera);
				}
				m_Disposed = true;
			}
		}
	}
}
