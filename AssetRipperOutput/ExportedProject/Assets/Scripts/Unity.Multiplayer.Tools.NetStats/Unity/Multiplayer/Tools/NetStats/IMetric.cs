using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IMetric
	{
		string Name { get; }

		MetricId Id { get; }

		MetricContainerType MetricContainerType { get; }

		FixedString128Bytes FactoryTypeName { get; }

		int GetWriteSize();

		void Write(FastBufferWriter writer);

		void Read(FastBufferReader reader);
	}
	internal interface IMetric<TValue> : IMetric
	{
		TValue Value { get; }
	}
}
