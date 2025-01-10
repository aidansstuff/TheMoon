namespace UnityEngine.Rendering
{
	public enum ShaderVariantLogLevel
	{
		[Tooltip("No shader variants are logged")]
		Disabled = 0,
		[Tooltip("Only shaders that are compatible with SRPs (e.g., URP, HDRP) are logged")]
		OnlySRPShaders = 1,
		[Tooltip("All shader variants are logged")]
		AllShaders = 2
	}
}
