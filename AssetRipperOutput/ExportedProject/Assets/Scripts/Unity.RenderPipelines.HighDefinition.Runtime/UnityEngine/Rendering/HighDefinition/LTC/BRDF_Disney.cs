using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_Disney : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			if (_tsView.z <= 0f)
			{
				_pdf = 0.0;
				return 0.0;
			}
			_alpha = Mathf.Max(0.002f, _alpha);
			double num = Math.Max(0f, _tsLight.z);
			double cosTheta = Math.Max(0f, _tsView.z);
			double num2 = Math.Max(0f, Vector3.Dot(_tsLight, _tsView));
			double num3 = Math.Sqrt(_alpha);
			double f = 0.5 + (num3 + num3 * num2);
			double num4 = F_Schlick(1.0, f, num);
			double num5 = F_Schlick(1.0, f, cosTheta);
			double result = num4 * num5 / Math.PI / 1.03571 * num;
			_pdf = num / Math.PI;
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

		private double F_Schlick(double _F0, double _F90, double _cosTheta)
		{
			double num = 1.0 - _cosTheta;
			double num2 = num * num;
			double num3 = num * num2 * num2;
			return (_F90 - _F0) * num3 + _F0;
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.DisneyDiffuse;
		}
	}
}
