using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_KajiyaKaySpecular : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			_alpha = Mathf.Max(0.002f, _alpha);
			double num = Math.Sqrt(_alpha);
			Vector3 right = Vector3.right;
			Vector3 forward = Vector3.forward;
			double num2 = Math.Max(0f, _tsView.z);
			double num3 = Math.Max(0f, _tsLight.z);
			double num4 = Math.Max(0f, Vector3.Dot(_tsLight, _tsView));
			Vector3 vector = Vector3.Normalize(_tsLight + _tsView);
			double cosTheta = Math.Max(0f, Vector3.Dot(_tsLight, vector));
			Vector3 t = ShiftTangent(right, forward, 0f);
			Vector3 t2 = ShiftTangent(right, forward, 0f);
			double specularExponent = RoughnessToBlinnPhongSpecularExponent(_alpha);
			double num5 = D_KajiyaKay(t, vector, specularExponent);
			double num6 = D_KajiyaKay(t2, vector, specularExponent);
			double f = 0.5 + (num + num * num4);
			double num7 = F_Schlick(1.0, f, cosTheta);
			double result = 0.25 * num7 * (num5 + num6) * num3 * Math.Min(Math.Max(num2 * double.MaxValue, 0.0), 1.0);
			_pdf = 1.0 / (2.0 * Math.PI);
			return result;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			float f = MathF.PI * 2f * _U1;
			float num = 1f - _U2;
			float num2 = Mathf.Sqrt(1f - num * num);
			_direction = new Vector3(num2 * Mathf.Cos(f), num2 * Mathf.Sin(f), num);
		}

		private double RoughnessToBlinnPhongSpecularExponent(double roughness)
		{
			return Math.Min(Math.Max(2.0 / (roughness * roughness) - 2.0, 0.0001), 3000.0);
		}

		private double F_Schlick(double _F0, double _F90, double _cosTheta)
		{
			double num = 1.0 - _cosTheta;
			double num2 = num * num;
			double num3 = num * num2 * num2;
			return (_F90 - _F0) * num3 + _F0;
		}

		private Vector3 ShiftTangent(Vector3 T, Vector3 N, float shift)
		{
			return Vector3.Normalize(T + N * shift);
		}

		private double PositivePow(double value, double power)
		{
			return Math.Pow(Math.Max(Math.Abs(value), 1.192092896E-07), power);
		}

		private double D_KajiyaKay(Vector3 T, Vector3 H, double specularExponent)
		{
			float num = Vector3.Dot(T, H);
			float num2 = Mathf.Clamp(1f - num * num, 0f, 1f);
			float num3 = Mathf.Clamp(num + 1f, 0f, 1f);
			double num4 = (specularExponent + 2.0) / (Math.PI * 2.0);
			return (double)num3 * num4 * PositivePow(num2, 0.5 * specularExponent);
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.KajiyaKaySpecular;
		}
	}
}
