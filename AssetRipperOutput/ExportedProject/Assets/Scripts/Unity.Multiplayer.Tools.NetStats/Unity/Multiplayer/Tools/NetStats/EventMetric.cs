using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	internal class EventMetric<TValue> : IEventMetric<TValue>, IEventMetric, IMetric, IResettable where TValue : unmanaged
	{
		private readonly List<TValue> m_Values = new List<TValue>();

		public int Count => m_Values.Count;

		public string Name => Id.ToString();

		public MetricId Id { get; }

		public MetricContainerType MetricContainerType => MetricContainerType.Event;

		public FixedString128Bytes FactoryTypeName { get; }

		public IReadOnlyList<TValue> Values => m_Values;

		public bool ShouldResetOnDispatch { get; set; } = true;


		public uint MaxNumberOfValues { get; set; } = 100u;


		public bool WentOverLimit { get; private set; }

		public EventMetric(MetricId id)
		{
			Id = id;
			if (EventMetricFactory.TryGetFactoryTypeName(typeof(TValue), out var typeName))
			{
				FactoryTypeName = typeName;
			}
		}

		public int GetWriteSize()
		{
			return 0 + FastBufferWriter.GetWriteSize<int>() + FastBufferWriter.GetWriteSize<TValue>() * m_Values.Count;
		}

		public void Write(FastBufferWriter writer)
		{
			int value = m_Values.Count;
			writer.WriteValue(in value);
			for (int i = 0; i < m_Values.Count; i++)
			{
				TValue value2 = m_Values[i];
				writer.WriteValue(in value2);
			}
		}

		public void Read(FastBufferReader reader)
		{
			m_Values.Clear();
			reader.ReadValueSafe(out int value);
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out TValue value2);
				m_Values.Add(value2);
			}
		}

		public void Mark(TValue value)
		{
			if (m_Values.Count >= MaxNumberOfValues)
			{
				WentOverLimit = true;
			}
			else
			{
				m_Values.Add(value);
			}
		}

		public void Reset()
		{
			m_Values.Clear();
			WentOverLimit = false;
		}
	}
}
