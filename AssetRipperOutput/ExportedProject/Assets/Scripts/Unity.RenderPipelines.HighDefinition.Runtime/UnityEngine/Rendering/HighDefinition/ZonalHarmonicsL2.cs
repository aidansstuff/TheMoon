using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct ZonalHarmonicsL2
	{
		public float[] coeffs;

		public static ZonalHarmonicsL2 GetHenyeyGreensteinPhaseFunction(float anisotropy)
		{
			ZonalHarmonicsL2 result = default(ZonalHarmonicsL2);
			result.coeffs = new float[3];
			result.coeffs[0] = 0.5f * Mathf.Sqrt(1f / MathF.PI);
			result.coeffs[1] = 0.5f * Mathf.Sqrt(3f / MathF.PI) * anisotropy;
			result.coeffs[2] = 0.5f * Mathf.Sqrt(5f / MathF.PI) * anisotropy * anisotropy;
			return result;
		}

		public static void GetCornetteShanksPhaseFunction(ZonalHarmonicsL2 zh, float anisotropy)
		{
			zh.coeffs[0] = 0.282095f;
			zh.coeffs[1] = 0.293162f * anisotropy * (4f + anisotropy * anisotropy) / (2f + anisotropy * anisotropy);
			zh.coeffs[2] = (0.126157f + 1.44179f * (anisotropy * anisotropy) + 0.324403f * (anisotropy * anisotropy) * (anisotropy * anisotropy)) / (2f + anisotropy * anisotropy);
		}
	}
}
