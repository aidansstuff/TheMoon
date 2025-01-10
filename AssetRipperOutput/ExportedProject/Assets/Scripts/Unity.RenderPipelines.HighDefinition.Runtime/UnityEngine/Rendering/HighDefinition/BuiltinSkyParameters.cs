namespace UnityEngine.Rendering.HighDefinition
{
	public class BuiltinSkyParameters
	{
		public HDCamera hdCamera;

		public Matrix4x4 pixelCoordToViewDirMatrix;

		public Vector3 worldSpaceCameraPos;

		public Matrix4x4 viewMatrix;

		public Vector4 screenSize;

		public CommandBuffer commandBuffer;

		public Light sunLight;

		public RTHandle colorBuffer;

		public RTHandle depthBuffer;

		public RTHandle cloudOpacity;

		public ComputeBuffer cloudAmbientProbe;

		public int frameIndex;

		public SkySettings skySettings;

		public CloudSettings cloudSettings;

		public VolumetricClouds volumetricClouds;

		public DebugDisplaySettings debugSettings;

		public static RenderTargetIdentifier nullRT = -1;

		public CubemapFace cubemapFace = CubemapFace.Unknown;

		public void CopyTo(BuiltinSkyParameters other)
		{
			other.hdCamera = hdCamera;
			other.pixelCoordToViewDirMatrix = pixelCoordToViewDirMatrix;
			other.worldSpaceCameraPos = worldSpaceCameraPos;
			other.viewMatrix = viewMatrix;
			other.screenSize = screenSize;
			other.commandBuffer = commandBuffer;
			other.sunLight = sunLight;
			other.colorBuffer = colorBuffer;
			other.depthBuffer = depthBuffer;
			other.frameIndex = frameIndex;
			other.skySettings = skySettings;
			other.cloudSettings = cloudSettings;
			other.volumetricClouds = volumetricClouds;
			other.debugSettings = debugSettings;
			other.cubemapFace = cubemapFace;
		}
	}
}
