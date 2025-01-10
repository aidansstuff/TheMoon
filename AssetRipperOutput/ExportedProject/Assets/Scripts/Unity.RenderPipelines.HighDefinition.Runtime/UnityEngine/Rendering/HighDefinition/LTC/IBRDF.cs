namespace UnityEngine.Rendering.HighDefinition.LTC
{
	internal interface IBRDF
	{
		double Eval(ref Vector3 _tsView, ref Vector3 _tsLight, float _alpha, out double _pdf);

		void GetSamplingDirection(ref Vector3 _tsView, float _alpha, float _U1, float _U2, ref Vector3 _direction);

		LTCLightingModel GetLightingModel();
	}
}
