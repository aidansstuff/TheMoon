using System;

namespace UnityEngine.InputSystem
{
	public enum InputDeviceChange
	{
		Added = 0,
		Removed = 1,
		Disconnected = 2,
		Reconnected = 3,
		Enabled = 4,
		Disabled = 5,
		UsageChanged = 6,
		ConfigurationChanged = 7,
		SoftReset = 8,
		HardReset = 9,
		[Obsolete("Destroyed enum has been deprecated.")]
		Destroyed = 10
	}
}
