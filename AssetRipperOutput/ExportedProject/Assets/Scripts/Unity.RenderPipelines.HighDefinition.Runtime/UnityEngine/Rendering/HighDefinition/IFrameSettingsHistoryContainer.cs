namespace UnityEngine.Rendering.HighDefinition
{
	internal interface IFrameSettingsHistoryContainer : IDebugData
	{
		FrameSettingsHistory frameSettingsHistory { get; set; }

		FrameSettingsOverrideMask frameSettingsMask { get; }

		FrameSettings frameSettings { get; }

		bool hasCustomFrameSettings { get; }

		string panelName { get; }
	}
}
