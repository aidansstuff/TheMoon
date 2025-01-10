using System;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	internal class BRDF_GGX : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			double num = Lambda(_tsView.z, _alpha);
			double num2 = 0.0;
			if (_tsLight.z > 0f)
			{
				double num3 = Lambda(_tsLight.z, _alpha);
				num2 = 1.0 / (1.0 + num + num3);
			}
			Vector3 rhs = _tsView + _tsLight;
			float magnitude = rhs.magnitude;
			if (magnitude > 1E-08f)
			{
				rhs /= magnitude;
			}
			else
			{
				rhs = new Vector3(0f, 0f, 1f);
			}
			double num4 = rhs.x / rhs.z;
			double num5 = rhs.y / rhs.z;
			double num6 = 1.0 / (1.0 + (num4 * num4 + num5 * num5) / (double)_alpha / (double)_alpha);
			num6 *= num6;
			num6 /= Math.PI * (double)_alpha * (double)_alpha * (double)rhs.z * (double)rhs.z * (double)rhs.z * (double)rhs.z;
			double result = num6 * num2 / 4.0 / (double)_tsView.z;
			_pdf = Math.Abs(num6 * (double)rhs.z / 4.0 / (double)Vector3.Dot(_tsView, rhs));
			return result;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			float f = MathF.PI * 2f * _U1;
			float num = _alpha * Mathf.Sqrt(_U2 / (1f - _U2));
			Vector3 normalized = new Vector3(num * Mathf.Cos(f), num * Mathf.Sin(f), 1f).normalized;
			_direction = -_tsView + 2f * normalized * Vector3.Dot(normalized, _tsView);
		}

		private double Lambda(float _cosTheta, float _alpha)
		{
			double num = (double)(1f / _alpha) / Math.Tan(Math.Acos(_cosTheta));
			if (!((double)_cosTheta < 1.0))
			{
				return 0.0;
			}
			return 0.5 * (-1.0 + Math.Sqrt(1.0 + 1.0 / (num * num)));
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.GGX;
		}
	}
}
