namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDDynamicResolutionPlatformCapabilities
	{
		private static bool m_DLSSDetected;

		public static bool DLSSDetected => m_DLSSDetected;

		internal static void ActivateDLSS()
		{
			m_DLSSDetected = true;
		}
	}
}
