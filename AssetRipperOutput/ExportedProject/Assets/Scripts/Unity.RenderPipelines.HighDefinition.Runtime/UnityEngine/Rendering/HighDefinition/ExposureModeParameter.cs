using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class ExposureModeParameter : VolumeParameter<ExposureMode>
	{
		public ExposureModeParameter(ExposureMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
