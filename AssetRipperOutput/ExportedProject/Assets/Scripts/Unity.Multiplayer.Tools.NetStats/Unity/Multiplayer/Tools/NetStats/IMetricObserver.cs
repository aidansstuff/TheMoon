namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IMetricObserver
	{
		void Observe(MetricCollection collection);
	}
}
