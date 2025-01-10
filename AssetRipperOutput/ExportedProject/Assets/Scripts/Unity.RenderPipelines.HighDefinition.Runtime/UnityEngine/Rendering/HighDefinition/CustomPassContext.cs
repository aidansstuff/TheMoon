using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct CustomPassContext
	{
		public readonly ScriptableRenderContext renderContext;

		public readonly CommandBuffer cmd;

		public readonly HDCamera hdCamera;

		public CullingResults cullingResults;

		public readonly CullingResults cameraCullingResults;

		public readonly RTHandle cameraColorBuffer;

		public readonly RTHandle cameraDepthBuffer;

		public readonly RTHandle cameraNormalBuffer;

		public readonly RTHandle cameraMotionVectorsBuffer;

		public readonly Lazy<RTHandle> customColorBuffer;

		public readonly Lazy<RTHandle> customDepthBuffer;

		public readonly MaterialPropertyBlock propertyBlock;

		internal readonly CustomPassInjectionPoint injectionPoint;

		internal CustomPassContext(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResults, CullingResults cameraCullingResults, RTHandle cameraColorBuffer, RTHandle cameraDepthBuffer, RTHandle cameraNormalBuffer, RTHandle cameraMotionVectorsBuffer, Lazy<RTHandle> customColorBuffer, Lazy<RTHandle> customDepthBuffer, MaterialPropertyBlock propertyBlock, CustomPassInjectionPoint injectionPoint)
		{
			this.renderContext = renderContext;
			this.cmd = cmd;
			this.hdCamera = hdCamera;
			this.cullingResults = cullingResults;
			this.cameraCullingResults = cameraCullingResults;
			this.cameraColorBuffer = cameraColorBuffer;
			this.cameraDepthBuffer = cameraDepthBuffer;
			this.customColorBuffer = customColorBuffer;
			this.cameraNormalBuffer = cameraNormalBuffer;
			this.cameraMotionVectorsBuffer = cameraMotionVectorsBuffer;
			this.customDepthBuffer = customDepthBuffer;
			this.propertyBlock = propertyBlock;
			this.injectionPoint = injectionPoint;
		}
	}
}
