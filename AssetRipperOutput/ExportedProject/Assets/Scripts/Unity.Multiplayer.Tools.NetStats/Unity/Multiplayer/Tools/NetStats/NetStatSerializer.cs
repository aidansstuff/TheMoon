using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal class NetStatSerializer : INetStatSerializer
	{
		private MetricFactory m_MetricFactory = new MetricFactory();

		public NativeArray<byte> Serialize(MetricCollection metricCollection)
		{
			int num = 0;
			for (int i = 0; i < metricCollection.Metrics.Count; i++)
			{
				IMetric metric = metricCollection.Metrics[i];
				num += FastBufferWriter.GetWriteSize<MetricHeader>();
				num += metric.GetWriteSize();
			}
			num += FastBufferWriter.GetWriteSize<ulong>();
			FastBufferWriter writer = new FastBufferWriter(num, Allocator.Temp, int.MaxValue);
			try
			{
				ulong value = metricCollection.ConnectionId;
				writer.WriteValueSafe(in value);
				int value2 = metricCollection.Metrics.Count;
				writer.WriteValueSafe(in value2);
				for (int j = 0; j < metricCollection.Metrics.Count; j++)
				{
					IMetric metric2 = metricCollection.Metrics[j];
					MetricHeader value3 = new MetricHeader(metric2.FactoryTypeName, metric2.MetricContainerType, metric2.Id);
					writer.WriteValueSafe(in value3);
					writer.TryBeginWrite(metric2.GetWriteSize());
					metric2.Write(writer);
				}
				return writer.ToNativeArray(Allocator.Temp);
			}
			finally
			{
				((IDisposable)writer).Dispose();
			}
		}

		public MetricCollection Deserialize(NativeArray<byte> bytes)
		{
			List<IMetric> list = new List<IMetric>();
			ulong value;
			using (FastBufferReader reader = new FastBufferReader(bytes, Allocator.Temp))
			{
				reader.ReadValueSafe(out value);
				reader.ReadValueSafe(out int value2);
				for (int i = 0; i < value2; i++)
				{
					reader.ReadValueSafe(out MetricHeader value3);
					if (m_MetricFactory.TryConstruct(value3, out var metric))
					{
						metric.Read(reader);
						list.Add(metric);
						continue;
					}
					throw new InvalidOperationException($"Failed to construct metric from serialized data. Metric Header: {value3}");
				}
			}
			return new MetricCollection(list, value);
		}
	}
}
