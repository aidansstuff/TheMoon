using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.HighDefinition.LTC
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BRDF_Marschner : IBRDF
	{
		public double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf)
		{
			_pdf = 1.0 / (4.0 * Math.PI);
			return 0.0;
		}

		public void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction)
		{
			_direction = Vector3.up;
		}

		public LTCLightingModel GetLightingModel()
		{
			return LTCLightingModel.Marschner;
		}
	}
}
