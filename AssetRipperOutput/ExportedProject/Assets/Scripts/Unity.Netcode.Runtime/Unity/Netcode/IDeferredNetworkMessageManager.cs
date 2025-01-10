namespace Unity.Netcode
{
	internal interface IDeferredNetworkMessageManager
	{
		internal enum TriggerType
		{
			OnSpawn = 0,
			OnAddPrefab = 1
		}

		void DeferMessage(TriggerType trigger, ulong key, FastBufferReader reader, ref NetworkContext context);

		void CleanupStaleTriggers();

		void ProcessTriggers(TriggerType trigger, ulong key);

		void CleanupAllTriggers();
	}
}
