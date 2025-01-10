namespace Unity.Netcode
{
	public enum SceneEventProgressStatus
	{
		None = 0,
		Started = 1,
		SceneNotLoaded = 2,
		SceneEventInProgress = 3,
		InvalidSceneName = 4,
		SceneFailedVerification = 5,
		InternalNetcodeError = 6
	}
}
