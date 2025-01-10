using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	internal class DiffusionProfile : IEquatable<DiffusionProfile>
	{
		public enum TexturingMode : uint
		{
			PreAndPostScatter = 0u,
			PostScatter = 1u
		}

		public enum TransmissionMode : uint
		{
			Regular = 0u,
			ThinObject = 1u
		}

		[ColorUsage(false, false)]
		public Color scatteringDistance;

		[Min(0f)]
		public float scatteringDistanceMultiplier = 1f;

		[ColorUsage(false, true)]
		public Color transmissionTint;

		public TexturingMode texturingMode;

		public TransmissionMode transmissionMode;

		public Vector2 thicknessRemap;

		public float worldScale;

		public float ior;

		public uint hash;

		public Vector3 shapeParam { get; private set; }

		public float filterRadius { get; private set; }

		public float maxScatteringDistance { get; private set; }

		public DiffusionProfile(bool dontUseDefaultConstructor)
		{
			ResetToDefault();
		}

		public void ResetToDefault()
		{
			scatteringDistance = Color.grey;
			scatteringDistanceMultiplier = 1f;
			transmissionTint = Color.white;
			texturingMode = TexturingMode.PreAndPostScatter;
			transmissionMode = TransmissionMode.ThinObject;
			thicknessRemap = new Vector2(0f, 5f);
			worldScale = 1f;
			ior = 1.4f;
		}

		internal void Validate()
		{
			thicknessRemap.y = Mathf.Max(thicknessRemap.y, 0f);
			thicknessRemap.x = Mathf.Clamp(thicknessRemap.x, 0f, thicknessRemap.y);
			worldScale = Mathf.Max(worldScale, 0.001f);
			ior = Mathf.Clamp(ior, 1f, 2f);
			UpdateKernel();
		}

		private void UpdateKernel()
		{
			Vector3 vector = scatteringDistanceMultiplier * (Vector3)(Vector4)scatteringDistance;
			shapeParam = new Vector3(Mathf.Min(16777216f, 1f / vector.x), Mathf.Min(16777216f, 1f / vector.y), Mathf.Min(16777216f, 1f / vector.z));
			float u = 0.997f;
			maxScatteringDistance = Mathf.Max(vector.x, vector.y, vector.z);
			filterRadius = SampleBurleyDiffusionProfile(u, maxScatteringDistance);
		}

		private static float DisneyProfile(float r, float s)
		{
			return s * (Mathf.Exp((0f - r) * s) + Mathf.Exp((0f - r) * s * (1f / 3f))) / (MathF.PI * 8f * r);
		}

		private static float DisneyProfilePdf(float r, float s)
		{
			return r * DisneyProfile(r, s);
		}

		private static float DisneyProfileCdf(float r, float s)
		{
			return 1f - 0.25f * Mathf.Exp((0f - r) * s) - 0.75f * Mathf.Exp((0f - r) * s * (1f / 3f));
		}

		private static float DisneyProfileCdfDerivative1(float r, float s)
		{
			return 0.25f * s * Mathf.Exp((0f - r) * s) * (1f + Mathf.Exp(r * s * (2f / 3f)));
		}

		private static float DisneyProfileCdfDerivative2(float r, float s)
		{
			return -1f / 12f * s * s * Mathf.Exp((0f - r) * s) * (3f + Mathf.Exp(r * s * (2f / 3f)));
		}

		private static float DisneyProfileCdfInverse(float p, float s)
		{
			float num = (Mathf.Pow(10f, p) - 1f) / s;
			float num2 = float.MaxValue;
			while (true)
			{
				float num3 = DisneyProfileCdf(num, s) - p;
				float num4 = DisneyProfileCdfDerivative1(num, s);
				float num5 = DisneyProfileCdfDerivative2(num, s);
				float num6 = num3 / (num4 * (1f - num3 * num5 / (2f * num4 * num4)));
				if (!(Mathf.Abs(num6) < num2))
				{
					break;
				}
				num -= num6;
				num2 = Mathf.Abs(num6);
			}
			return num;
		}

		private static float SampleBurleyDiffusionProfile(float u, float rcpS)
		{
			u = 1f - u;
			float num = 1f + 4f * u * (2f * u + Mathf.Sqrt(1f + 4f * u * u));
			float num2 = Mathf.Pow(num, -1f / 3f);
			float num3 = num * num2 * num2;
			float num4 = 1f + num3 + num2;
			return 3f * Mathf.Log(num4 / (4f * u)) * rcpS;
		}

		public bool Equals(DiffusionProfile other)
		{
			if (other == null)
			{
				return false;
			}
			if (scatteringDistance == other.scatteringDistance && scatteringDistanceMultiplier == other.scatteringDistanceMultiplier && transmissionTint == other.transmissionTint && texturingMode == other.texturingMode && transmissionMode == other.transmissionMode && thicknessRemap == other.thicknessRemap && worldScale == other.worldScale)
			{
				return ior == other.ior;
			}
			return false;
		}
	}
}
