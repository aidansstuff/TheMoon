namespace UnityEngine.Rendering.HighDefinition
{
	public struct RenderOutputProperties
	{
		public readonly Vector2Int outputSize;

		public readonly Matrix4x4 cameraToWorldMatrixRHS;

		public readonly Matrix4x4 projectionMatrix;

		public RenderOutputProperties(Vector2Int outputSize, Matrix4x4 cameraToWorldMatrixRhs, Matrix4x4 projectionMatrix)
		{
			this.outputSize = outputSize;
			cameraToWorldMatrixRHS = cameraToWorldMatrixRhs;
			this.projectionMatrix = projectionMatrix;
		}

		internal static RenderOutputProperties From(HDCamera hdCamera)
		{
			return new RenderOutputProperties(new Vector2Int(hdCamera.actualWidth, hdCamera.actualHeight), hdCamera.camera.cameraToWorldMatrix, hdCamera.mainViewConstants.projMatrix);
		}
	}
}
