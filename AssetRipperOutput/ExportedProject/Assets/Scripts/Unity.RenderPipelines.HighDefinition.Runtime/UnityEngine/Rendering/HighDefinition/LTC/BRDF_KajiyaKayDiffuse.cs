using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_KajiyaKayDiffuse : IBRDF
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
			_pdf = num / Math.PI;
			return num / 9.869604401089358;
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
			return LTCLightingModel.KajiyaKayDiffuse;
		}
	}
}
