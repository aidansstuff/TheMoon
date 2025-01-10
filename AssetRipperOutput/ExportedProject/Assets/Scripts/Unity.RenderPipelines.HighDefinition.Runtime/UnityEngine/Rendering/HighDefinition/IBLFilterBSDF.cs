namespace UnityEngine.Rendering.HighDefinition
{
	internal abstract class IBLFilterBSDF
	{
		internal struct PlanarTextureFilteringParameters
		{
			public bool smoothPlanarReflection;

			public RenderTexture captureCameraDepthBuffer;

			public Matrix4x4 captureCameraIVP;

			public Matrix4x4 captureCameraVP_NonOblique;

			public Matrix4x4 captureCameraIVP_NonOblique;

			public Vector3 captureCameraPosition;

			public Vector4 captureCameraScreenSize;

			public Vector3 probePosition;

			public Vector3 probeNormal;

			public float captureFOV;

			public float captureNearPlane;

			public float captureFarPlane;
		}

		protected Material m_convolveMaterial;

		protected Matrix4x4[] m_faceWorldToViewMatrixMatrices = new Matrix4x4[6];

		protected HDRenderPipelineRuntimeResources m_RenderPipelineResources;

		protected MipGenerator m_MipGenerator;

		public abstract bool IsInitialized();

		public abstract void Initialize(CommandBuffer cmd);

		public abstract void Cleanup();

		public abstract void FilterCubemap(CommandBuffer cmd, Texture source, RenderTexture target);

		public abstract void FilterPlanarTexture(CommandBuffer cmd, RenderTexture source, ref PlanarTextureFilteringParameters planarTextureFilteringParameters, RenderTexture target);

		public abstract void FilterCubemapMIS(CommandBuffer cmd, Texture source, RenderTexture target, RenderTexture conditionalCdf, RenderTexture marginalRowCdf);
	}
}
