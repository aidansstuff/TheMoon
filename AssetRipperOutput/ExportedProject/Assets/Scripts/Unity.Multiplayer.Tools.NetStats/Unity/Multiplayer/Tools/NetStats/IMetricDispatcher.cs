namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IMetricDispatcher
	{
		void RegisterObserver(IMetricObserver observer);

		void SetConnectionId(ulong connectionId);

		void Dispatch();
	}
}
