namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDDebugDisplaySettings : DebugDisplaySettings<HDDebugDisplaySettings>
	{
		internal DebugDisplaySettingsVolume VolumeSettings { get; private set; }

		public override void Reset()
		{
			base.Reset();
			VolumeSettings = Add(new DebugDisplaySettingsVolume(new HDVolumeDebugSettings()));
		}
	}
}
