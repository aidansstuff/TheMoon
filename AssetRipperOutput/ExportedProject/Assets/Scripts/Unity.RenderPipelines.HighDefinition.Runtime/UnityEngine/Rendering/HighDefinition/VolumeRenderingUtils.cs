namespace UnityEngine.Rendering.HighDefinition
{
	internal class VolumeRenderingUtils
	{
		public static float MeanFreePathFromExtinction(float extinction)
		{
			return 1f / extinction;
		}

		public static float ExtinctionFromMeanFreePath(float meanFreePath)
		{
			return 1f / meanFreePath;
		}

		public static Vector3 AbsorptionFromExtinctionAndScattering(float extinction, Vector3 scattering)
		{
			return new Vector3(extinction, extinction, extinction) - scattering;
		}

		public static Vector3 ScatteringFromExtinctionAndAlbedo(float extinction, Vector3 albedo)
		{
			return extinction * albedo;
		}

		public static Vector3 AlbedoFromMeanFreePathAndScattering(float meanFreePath, Vector3 scattering)
		{
			return meanFreePath * scattering;
		}
	}
}
