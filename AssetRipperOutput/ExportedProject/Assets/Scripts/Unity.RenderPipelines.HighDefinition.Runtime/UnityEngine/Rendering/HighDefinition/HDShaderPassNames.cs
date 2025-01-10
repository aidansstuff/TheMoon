namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDShaderPassNames
	{
		public static readonly string s_EmptyStr = "";

		public static readonly string s_ForwardStr = "Forward";

		public static readonly string s_DepthOnlyStr = "DepthOnly";

		public static readonly string s_DepthForwardOnlyStr = "DepthForwardOnly";

		public static readonly string s_ForwardOnlyStr = "ForwardOnly";

		public static readonly string s_GBufferStr = "GBuffer";

		public static readonly string s_GBufferWithPrepassStr = "GBufferWithPrepass";

		public static readonly string s_SRPDefaultUnlitStr = "SRPDefaultUnlit";

		public static readonly string s_MotionVectorsStr = "MotionVectors";

		public static readonly string s_DistortionVectorsStr = "DistortionVectors";

		public static readonly string s_TransparentDepthPrepassStr = "TransparentDepthPrepass";

		public static readonly string s_TransparentBackfaceStr = "TransparentBackface";

		public static readonly string s_TransparentDepthPostpassStr = "TransparentDepthPostpass";

		public static readonly string s_RayTracingPrepassStr = "RayTracingPrepass";

		public static readonly string s_RayTracingGBufferStr = "GBufferDXR";

		public static readonly string s_RayTracingForwardStr = "ForwardDXR";

		public static readonly string s_RayTracingIndirectStr = "IndirectDXR";

		public static readonly string s_RayTracingVisibilityStr = "VisibilityDXR";

		public static readonly string s_PathTracingDXRStr = "PathTracingDXR";

		public static readonly string s_MetaStr = "META";

		public static readonly string s_ShadowCasterStr = "ShadowCaster";

		public static readonly string s_FullScreenDebugStr = "FullScreenDebug";

		public static readonly string s_DBufferProjectorStr = DecalSystem.s_MaterialDecalPassNames[0];

		public static readonly string s_DecalProjectorForwardEmissiveStr = DecalSystem.s_MaterialDecalPassNames[1];

		public static readonly string s_DBufferMeshStr = DecalSystem.s_MaterialDecalPassNames[2];

		public static readonly string s_DecalMeshForwardEmissiveStr = DecalSystem.s_MaterialDecalPassNames[3];

		public static readonly string s_DBufferVFXDecalStr = "DBufferVFX";

		public static readonly string s_FogVolumeVoxelizeStr = "FogVolumeVoxelize";

		public static readonly ShaderTagId s_EmptyName = new ShaderTagId(s_EmptyStr);

		public static readonly ShaderTagId s_ForwardName = new ShaderTagId(s_ForwardStr);

		public static readonly ShaderTagId s_DepthOnlyName = new ShaderTagId(s_DepthOnlyStr);

		public static readonly ShaderTagId s_DepthForwardOnlyName = new ShaderTagId(s_DepthForwardOnlyStr);

		public static readonly ShaderTagId s_ForwardOnlyName = new ShaderTagId(s_ForwardOnlyStr);

		public static readonly ShaderTagId s_GBufferName = new ShaderTagId(s_GBufferStr);

		public static readonly ShaderTagId s_GBufferWithPrepassName = new ShaderTagId(s_GBufferWithPrepassStr);

		public static readonly ShaderTagId s_SRPDefaultUnlitName = new ShaderTagId(s_SRPDefaultUnlitStr);

		public static readonly ShaderTagId s_MotionVectorsName = new ShaderTagId(s_MotionVectorsStr);

		public static readonly ShaderTagId s_DistortionVectorsName = new ShaderTagId(s_DistortionVectorsStr);

		public static readonly ShaderTagId s_TransparentDepthPrepassName = new ShaderTagId(s_TransparentDepthPrepassStr);

		public static readonly ShaderTagId s_TransparentBackfaceName = new ShaderTagId(s_TransparentBackfaceStr);

		public static readonly ShaderTagId s_TransparentDepthPostpassName = new ShaderTagId(s_TransparentDepthPostpassStr);

		public static readonly ShaderTagId s_RayTracingPrepassName = new ShaderTagId(s_RayTracingPrepassStr);

		public static readonly ShaderTagId s_FullScreenDebugName = new ShaderTagId(s_FullScreenDebugStr);

		public static readonly ShaderTagId s_DBufferMeshName = new ShaderTagId(s_DBufferMeshStr);

		public static readonly ShaderTagId s_DecalMeshForwardEmissiveName = new ShaderTagId(s_DecalMeshForwardEmissiveStr);

		public static readonly ShaderTagId s_DBufferVFXDecalName = new ShaderTagId(s_DBufferVFXDecalStr);

		public static readonly ShaderTagId s_FogVolumeVoxelizeName = new ShaderTagId(s_FogVolumeVoxelizeStr);

		internal static readonly ShaderTagId s_AlwaysName = new ShaderTagId("Always");

		internal static readonly ShaderTagId s_ForwardBaseName = new ShaderTagId("ForwardBase");

		internal static readonly ShaderTagId s_DeferredName = new ShaderTagId("Deferred");

		internal static readonly ShaderTagId s_PrepassBaseName = new ShaderTagId("PrepassBase");

		internal static readonly ShaderTagId s_VertexName = new ShaderTagId("Vertex");

		internal static readonly ShaderTagId s_VertexLMRGBMName = new ShaderTagId("VertexLMRGBM");

		internal static readonly ShaderTagId s_VertexLMName = new ShaderTagId("VertexLM");
	}
}
