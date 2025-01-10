using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class CameraClampModeParameter : VolumeParameter<CameraClampMode>
	{
		public CameraClampModeParameter(CameraClampMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
