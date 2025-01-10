using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_Charlie : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			_alpha = Mathf.Max(0.002f, _alpha);
			Vector3 vector = Vector3.Normalize(_tsView + _tsLight);
			double num = _tsLight.z;
			double ndotV = _tsView.z;
			double ndotH = vector.z;
			double num2 = CharlieD(_alpha, ndotH);
			double num3 = V_Charlie(ndotV, num, _alpha);
			double result = num2 * num3 * num;
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

		private double CharlieD(float _roughness, double _NdotH)
		{
			double num = 1.0 / (double)_roughness;
			double num2 = _NdotH * _NdotH;
			double x = 1.0 - num2;
			return (2.0 + num) * Math.Pow(x, num * 0.5) / (Math.PI * 2.0);
		}

		private double V_Ashikhmin(double _NdotV, double _NdotL)
		{
			return 1.0 / (4.0 * (_NdotL + _NdotV - _NdotL * _NdotV));
		}

		private double V_Charlie(double _NdotV, double _NdotL, double _roughness)
		{
			double num = ((_NdotV < 0.5) ? Math.Exp(CharlieL(_NdotV, _roughness)) : Math.Exp(2.0 * CharlieL(0.5, _roughness) - CharlieL(1.0 - _NdotV, _roughness)));
			double num2 = ((_NdotL < 0.5) ? Math.Exp(CharlieL(_NdotL, _roughness)) : Math.Exp(2.0 * CharlieL(0.5, _roughness) - CharlieL(1.0 - _NdotL, _roughness)));
			return 1.0 / ((1.0 + num + num2) * (4.0 * _NdotV * _NdotL));
		}

		private double CharlieL(double x, double _roughness)
		{
			float num = Mathf.Clamp01((float)_roughness);
			num = 1f - num * num;
			float num2 = Mathf.Lerp(25.3245f, 21.5473f, num);
			float num3 = Mathf.Lerp(3.32435f, 3.82987f, num);
			float num4 = Mathf.Lerp(0.16801f, 0.19823f, num);
			float num5 = Mathf.Lerp(-1.27393f, -1.9776f, num);
			float num6 = Mathf.Lerp(-4.85967f, -4.32054f, num);
			return (double)num2 / (1.0 + (double)num3 * Math.Pow(Math.Max(0.0, x), num4)) + (double)num5 * x + (double)num6;
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.Charlie;
		}
	}
}
