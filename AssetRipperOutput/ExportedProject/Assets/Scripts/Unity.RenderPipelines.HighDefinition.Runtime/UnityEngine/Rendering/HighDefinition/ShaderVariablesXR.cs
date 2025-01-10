namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\ShaderLibrary\\ShaderVariablesXR.cs", needAccessors = false, generateCBuffer = true, constantRegister = 1)]
	internal struct ShaderVariablesXR
	{
		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRViewMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRInvViewMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRInvProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRViewProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRInvViewProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRNonJitteredViewProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRPrevViewProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRPrevInvViewProjMatrix[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRPrevViewProjMatrixNoCameraTrans[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRViewProjMatrixNoCameraTrans[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float _XRPixelCoordToViewDirWS[32];

		[HLSLArray(2, typeof(Vector4))]
		public unsafe fixed float _XRWorldSpaceCameraPos[8];

		[HLSLArray(2, typeof(Vector4))]
		public unsafe fixed float _XRWorldSpaceCameraPosViewOffset[8];

		[HLSLArray(2, typeof(Vector4))]
		public unsafe fixed float _XRPrevWorldSpaceCameraPos[8];
	}
}
