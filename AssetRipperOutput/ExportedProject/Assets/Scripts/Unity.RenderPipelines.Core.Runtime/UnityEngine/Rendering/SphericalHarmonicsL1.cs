using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public struct SphericalHarmonicsL1
	{
		public Vector4 shAr;

		public Vector4 shAg;

		public Vector4 shAb;

		public static readonly SphericalHarmonicsL1 zero = new SphericalHarmonicsL1
		{
			shAr = Vector4.zero,
			shAg = Vector4.zero,
			shAb = Vector4.zero
		};

		public static SphericalHarmonicsL1 operator +(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			SphericalHarmonicsL1 result = default(SphericalHarmonicsL1);
			result.shAr = lhs.shAr + rhs.shAr;
			result.shAg = lhs.shAg + rhs.shAg;
			result.shAb = lhs.shAb + rhs.shAb;
			return result;
		}

		public static SphericalHarmonicsL1 operator -(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			SphericalHarmonicsL1 result = default(SphericalHarmonicsL1);
			result.shAr = lhs.shAr - rhs.shAr;
			result.shAg = lhs.shAg - rhs.shAg;
			result.shAb = lhs.shAb - rhs.shAb;
			return result;
		}

		public static SphericalHarmonicsL1 operator *(SphericalHarmonicsL1 lhs, float rhs)
		{
			SphericalHarmonicsL1 result = default(SphericalHarmonicsL1);
			result.shAr = lhs.shAr * rhs;
			result.shAg = lhs.shAg * rhs;
			result.shAb = lhs.shAb * rhs;
			return result;
		}

		public static SphericalHarmonicsL1 operator /(SphericalHarmonicsL1 lhs, float rhs)
		{
			SphericalHarmonicsL1 result = default(SphericalHarmonicsL1);
			result.shAr = lhs.shAr / rhs;
			result.shAg = lhs.shAg / rhs;
			result.shAb = lhs.shAb / rhs;
			return result;
		}

		public static bool operator ==(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			if (lhs.shAr == rhs.shAr && lhs.shAg == rhs.shAg)
			{
				return lhs.shAb == rhs.shAb;
			}
			return false;
		}

		public static bool operator !=(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			if (!(other is SphericalHarmonicsL1))
			{
				return false;
			}
			return this == (SphericalHarmonicsL1)other;
		}

		public override int GetHashCode()
		{
			return ((391 + shAr.GetHashCode()) * 23 + shAg.GetHashCode()) * 23 + shAb.GetHashCode();
		}
	}
}
