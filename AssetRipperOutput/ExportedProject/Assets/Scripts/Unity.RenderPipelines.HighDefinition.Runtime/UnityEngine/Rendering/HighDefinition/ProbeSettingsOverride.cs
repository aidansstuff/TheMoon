using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	internal struct ProbeSettingsOverride
	{
		public ProbeSettingsFields probe;

		public CameraSettingsOverride camera;
	}
}
