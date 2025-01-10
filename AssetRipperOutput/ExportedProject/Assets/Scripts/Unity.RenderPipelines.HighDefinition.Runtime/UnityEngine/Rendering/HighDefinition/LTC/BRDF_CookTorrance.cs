using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_CookTorrance : IBRDF
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
			double val = Math.Max(1E-08, _tsLight.z);
			double num = Math.Max(1E-08, _tsView.z);
			double num2 = normalized.z;
			double num3 = Math.Max(1E-08, Vector3.Dot(_tsLight, normalized));
			double num4 = num2 * num2;
			double num5 = _alpha * _alpha;
			double num6 = Math.Exp((num4 - 1.0) / (num4 * num5)) / Math.Max(1E-12, Math.PI * num5 * num4 * num4);
			double num7 = Math.Min(1.0, 2.0 * num2 * Math.Min(num, val) / num3);
			double result = num6 * num7 / (4.0 * num);
			_pdf = Math.Abs(num6 * num2 / (4.0 * num3));
			return result;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			float f = MathF.PI * 2f * _U1;
			float num = 1f / Mathf.Sqrt(1f - _alpha * _alpha * Mathf.Log(Mathf.Max(1E-06f, _U2)));
			float num2 = Mathf.Sqrt(1f - num * num);
			Vector3 vector = new Vector3(num2 * Mathf.Cos(f), num2 * Mathf.Sin(f), num);
			_direction = 2f * Vector3.Dot(vector, _tsView) * vector - _tsView;
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.CookTorrance;
		}
	}
}
