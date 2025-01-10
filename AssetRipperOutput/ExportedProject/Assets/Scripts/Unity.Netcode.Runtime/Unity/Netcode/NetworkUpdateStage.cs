namespace Unity.Netcode
{
	public enum NetworkUpdateStage : byte
	{
		Unset = 0,
		Initialization = 1,
		EarlyUpdate = 2,
		FixedUpdate = 3,
		PreUpdate = 4,
		Update = 5,
		PreLateUpdate = 6,
		PostLateUpdate = 7
	}
}
