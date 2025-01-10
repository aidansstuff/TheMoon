namespace UnityEngine.Rendering
{
	internal static class BC6HExtensions
	{
		public static void BC6HEncodeFastCubemap(this CommandBuffer cmb, RenderTargetIdentifier source, int sourceSize, RenderTargetIdentifier target, int fromMip, int toMip, int targetArrayIndex = 0)
		{
			EncodeBC6H.DefaultInstance.EncodeFastCubemap(cmb, source, sourceSize, target, fromMip, toMip, targetArrayIndex);
		}
	}
}
