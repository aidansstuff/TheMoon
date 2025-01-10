namespace Unity.Netcode
{
	public enum SceneEventType : byte
	{
		Load = 0,
		Unload = 1,
		Synchronize = 2,
		ReSynchronize = 3,
		LoadEventCompleted = 4,
		UnloadEventCompleted = 5,
		LoadComplete = 6,
		UnloadComplete = 7,
		SynchronizeComplete = 8,
		ActiveSceneChanged = 9,
		ObjectSceneChanged = 10
	}
}
