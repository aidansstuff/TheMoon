using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_Ward : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			_alpha = Mathf.Max(0.002f, _alpha);
			Vector3 normalized = (_tsView + _tsLight).normalized;
			double num = Math.Max(1E-08, _tsLight.z);
			double num2 = Math.Max(1E-08, normalized.z);
			double num3 = Math.Max(1E-08, Vector3.Dot(_tsLight, normalized));
			double num4 = _alpha * _alpha;
			double num5 = num2 * num2;
			double num6 = Math.Exp((0.0 - (1.0 - num5)) / (num4 * num5)) / (Math.PI * num4 * num5 * num5);
			num6 /= 4.0 * num3 * num3;
			double result = num6 * num;
			_pdf = Math.Abs(num6 * num2);
			return result;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			float num = _alpha * Mathf.Sqrt(0f - Mathf.Log(Mathf.Max(1E-06f, _U1)));
			float f = _U2 * 2f * MathF.PI;
			float num2 = 1f / Mathf.Sqrt(1f + num * num);
			float num3 = Mathf.Sqrt(1f - num2 * num2);
			Vector3 vector = new Vector3(num3 * Mathf.Cos(f), num3 * Mathf.Sin(f), num2);
			_direction = 2f * Vector3.Dot(vector, _tsView) * vector - _tsView;
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.Ward;
		}
	}
}
