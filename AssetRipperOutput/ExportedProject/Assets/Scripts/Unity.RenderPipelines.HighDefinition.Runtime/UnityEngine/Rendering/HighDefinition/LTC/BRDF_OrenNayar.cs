using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_OrenNayar : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			float num = Mathf.Max(0.002f, MathF.PI / 2f * _alpha);
			double num2 = Math.Max(0f, _tsLight.z);
			double num3 = Math.Max(0f, _tsView.z);
			double val = (double)(_tsView.x * _tsLight.x + _tsView.y * _tsLight.y) / Math.Max(1E-20, Math.Sqrt(1.0 - num3 * num3) * Math.Sqrt(1.0 - num2 * num2));
			double num4 = num * num;
			double num5 = 1.0 - 0.5 * (num4 / (num4 + 0.57));
			double num6 = 0.45 * (num4 / (num4 + 0.09));
			double num7 = ((num3 < num2) ? num3 : num2);
			double num8 = ((num3 < num2) ? num2 : num3);
			double num9 = Math.Sqrt(1.0 - num7 * num7);
			double num10 = Math.Sqrt(1.0 - num8 * num8);
			double num11 = num9 * num10 / Math.Max(1E-20, num8);
			double result = (num5 + num6 * Math.Max(0.0, val) * num11) / Math.PI * num2;
			_pdf = num2 / Math.PI;
			return result;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			float num = Mathf.Sqrt(_U1);
			float f = MathF.PI * 2f * _U2;
			_direction.x = num * Mathf.Cos(f);
			_direction.y = num * Mathf.Sin(f);
			_direction.z = Mathf.Sqrt(1f - _U1);
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.OrenNayar;
		}
	}
}
