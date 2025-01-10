namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\VolumetricLighting\\HDRenderPipeline.VolumetricLighting.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesVolumetric
	{
		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _VBufferCoordToViewDirWS[32];

		public float _VBufferUnitDepthTexelSpacing;

		public uint _NumVisibleLocalVolumetricFog;

		public float _CornetteShanksConstant;

		public uint _VBufferHistoryIsValid;

		public Vector4 _VBufferSampleOffset;

		public float _VBufferVoxelSize;

		public float _HaveToPad;

		public float _OtherwiseTheBuffer;

		public float _IsFilledWithGarbage;

		public Vector4 _VBufferPrevViewportSize;

		public Vector4 _VBufferHistoryViewportScale;

		public Vector4 _VBufferHistoryViewportLimit;

		public Vector4 _VBufferPrevDistanceEncodingParams;

		public Vector4 _VBufferPrevDistanceDecodingParams;

		public uint _NumTileBigTileX;

		public uint _NumTileBigTileY;

		public uint _Pad0_SVV;

		public uint _Pad1_SVV;
	}
}
