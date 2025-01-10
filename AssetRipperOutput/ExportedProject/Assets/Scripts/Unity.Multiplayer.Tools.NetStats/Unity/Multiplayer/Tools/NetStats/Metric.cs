using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	internal abstract class Metric<TValue> : IMetric<TValue>, IMetric, IResettable where TValue : unmanaged
	{
		public string Name => Id.ToString();

		public MetricId Id { get; }

		public abstract MetricContainerType MetricContainerType { get; }

		public FixedString128Bytes FactoryTypeName => default(FixedString128Bytes);

		public TValue Value { get; protected set; }

		protected TValue DefaultValue { get; }

		public bool ShouldResetOnDispatch { get; set; } = true;


		protected Metric(MetricId metricId, TValue defaultValue = default(TValue))
		{
			Id = metricId;
			DefaultValue = defaultValue;
			Value = defaultValue;
		}

		public int GetWriteSize()
		{
			return FastBufferWriter.GetWriteSize<TValue>();
		}

		public void Write(FastBufferWriter writer)
		{
			TValue value = Value;
			writer.TryBeginWriteValue(in value);
			value = Value;
			writer.WriteValue(in value);
		}

		public void Read(FastBufferReader reader)
		{
			TValue value = default(TValue);
			reader.TryBeginReadValue(in value);
			reader.ReadValue(out TValue value2);
			Value = value2;
		}

		public void Reset()
		{
			Value = DefaultValue;
		}
	}
}
