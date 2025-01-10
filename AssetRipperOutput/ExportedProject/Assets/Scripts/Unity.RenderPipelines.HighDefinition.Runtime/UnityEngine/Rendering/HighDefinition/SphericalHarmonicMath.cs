using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SphericalHarmonicMath
	{
		private const float c0 = 0.2820948f;

		private const float c1 = 0.325735f;

		private const float c2 = 0.27313712f;

		private const float c3 = 0.07884789f;

		private const float c4 = 0.13656856f;

		private static float[] invNormConsts = new float[9] { 3.5449076f, -3.0699801f, 3.0699801f, -3.0699801f, 3.6611648f, -3.6611648f, 12.682647f, -3.6611648f, 7.3223295f };

		private const float k0 = 0.2820948f;

		private const float k1 = 0.48860252f;

		private const float k2 = 1.0925485f;

		private const float k3 = 0.31539157f;

		private const float k4 = 0.54627424f;

		private static float[] ks = new float[9] { 0.2820948f, -0.48860252f, 0.48860252f, -0.48860252f, 1.0925485f, -1.0925485f, 0.31539157f, -1.0925485f, 0.54627424f };

		public static SphericalHarmonicsL2 Convolve(SphericalHarmonicsL2 sh, ZonalHarmonicsL2 zh)
		{
			for (int i = 0; i <= 2; i++)
			{
				float num = Mathf.Sqrt(MathF.PI * 4f / (float)(2 * i + 1));
				float num2 = zh.coeffs[i];
				float num3 = num * num2;
				for (int j = -i; j <= i; j++)
				{
					int coefficient = i * (i + 1) + j;
					for (int k = 0; k < 3; k++)
					{
						sh[k, coefficient] *= num3;
					}
				}
			}
			return sh;
		}

		public static SphericalHarmonicsL2 UndoCosineRescaling(SphericalHarmonicsL2 sh)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					sh[i, j] *= invNormConsts[j];
				}
			}
			return sh;
		}

		public static SphericalHarmonicsL2 PremultiplyCoefficients(SphericalHarmonicsL2 sh)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					sh[i, j] *= ks[j];
				}
			}
			return sh;
		}

		public static SphericalHarmonicsL2 RescaleCoefficients(SphericalHarmonicsL2 sh, float scalar)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					sh[i, j] *= scalar;
				}
			}
			return sh;
		}

		public static void PackCoefficients(Vector4[] packedCoeffs, SphericalHarmonicsL2 sh)
		{
			for (int i = 0; i < 3; i++)
			{
				packedCoeffs[i].Set(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]);
			}
			for (int j = 0; j < 3; j++)
			{
				packedCoeffs[3 + j].Set(sh[j, 4], sh[j, 5], sh[j, 6] * 3f, sh[j, 7]);
			}
			packedCoeffs[6].Set(sh[0, 8], sh[1, 8], sh[2, 8], 1f);
		}
	}
}
