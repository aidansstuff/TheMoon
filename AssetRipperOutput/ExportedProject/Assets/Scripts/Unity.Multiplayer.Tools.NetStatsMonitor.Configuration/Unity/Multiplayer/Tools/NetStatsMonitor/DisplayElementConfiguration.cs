using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.Common;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Multiplayer.Tools.NetStatsMonitor.Configuration;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[Serializable]
	public sealed class DisplayElementConfiguration : ISerializationCallbackReceiver
	{
		[Serializable]
		private struct SerializedStat
		{
			[field: HideInInspector]
			[field: SerializeField]
			public string TypeName { get; set; }

			[field: HideInInspector]
			[field: SerializeField]
			public string ValueName { get; set; }
		}

		private int m_PreviousStatsHash;

		private string m_PreviousGeneratedLabel = "";

		private bool m_SerializedStatsLoaded;

		[field: HideInInspector]
		[field: SerializeField]
		internal bool FieldsInitialized { get; private set; } = true;


		[Tooltip("The label to display for this visual element in the on-screen display. For graphs this field is optional, as the variables displayed in the graph are shown in the legend. Consider leaving this field blank for graphs if you would like to make them more compact.")]
		[field: SerializeField]
		public DisplayElementType Type { get; set; }

		[field: SerializeField]
		public string Label { get; set; } = "";


		[field: SerializeField]
		public List<MetricId> Stats { get; set; } = new List<MetricId>();


		[field: SerializeField]
		public CounterConfiguration CounterConfiguration { get; set; } = new CounterConfiguration();


		[field: SerializeField]
		public GraphConfiguration GraphConfiguration { get; set; } = new GraphConfiguration();


		internal int SampleCount
		{
			get
			{
				switch (Type)
				{
				case DisplayElementType.Counter:
					return CounterConfiguration.SampleCount;
				case DisplayElementType.LineGraph:
				case DisplayElementType.StackedAreaGraph:
					return GraphConfiguration.SampleCount;
				default:
					throw new NotSupportedException(string.Format("Unhandled {0} {1}", "DisplayElementType", Type));
				}
			}
		}

		internal double? HalfLife
		{
			get
			{
				switch (Type)
				{
				case DisplayElementType.Counter:
				{
					SmoothingMethod smoothingMethod = CounterConfiguration.SmoothingMethod;
					return smoothingMethod switch
					{
						SmoothingMethod.ExponentialMovingAverage => CounterConfiguration.ExponentialMovingAverageParams.HalfLife, 
						SmoothingMethod.SimpleMovingAverage => null, 
						_ => throw new NotSupportedException(string.Format("Unhandled {0} {1}", "SmoothingMethod", smoothingMethod)), 
					};
				}
				case DisplayElementType.LineGraph:
				case DisplayElementType.StackedAreaGraph:
					return null;
				default:
					throw new NotSupportedException(string.Format("Unhandled {0} {1}", "DisplayElementType", Type));
				}
			}
		}

		internal double? DecayConstant
		{
			get
			{
				if (!HalfLife.HasValue)
				{
					return null;
				}
				return ContinuousExponentialMovingAverage.GetDecayConstantForHalfLife(HalfLife.Value);
			}
		}

		[field: HideInInspector]
		[field: SerializeField]
		private List<SerializedStat> SerializedStats { get; set; } = new List<SerializedStat>();


		internal void OnValidate()
		{
			RefreshGenerateLabel();
			ValidateColors();
		}

		private void RefreshGenerateLabel()
		{
			if (Type != 0)
			{
				return;
			}
			int num = ComputeStatsHashCode();
			if (m_PreviousStatsHash == 0)
			{
				m_PreviousStatsHash = num;
				m_PreviousGeneratedLabel = LabelGeneration.GenerateLabel(Stats);
			}
			else if (num != m_PreviousStatsHash)
			{
				m_PreviousStatsHash = num;
				string text = LabelGeneration.GenerateLabel(Stats);
				if (Label == m_PreviousGeneratedLabel)
				{
					Label = text;
				}
				m_PreviousGeneratedLabel = text;
			}
		}

		private void ValidateColors()
		{
			List<Color> list = GraphConfiguration?.VariableColors;
			if (list == null)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < list.Count; i++)
			{
				Color color = list[i];
				if (color.a != 0f || color.r != 0f || color.g != 0f || color.b != 0f)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				for (int j = 0; j < list.Count; j++)
				{
					Color value = list[j];
					value.a = 1f;
					list[j] = value;
				}
			}
		}

		public void OnBeforeSerialize()
		{
			int count = Stats.Count;
			SerializedStats.Resize(count);
			for (int i = 0; i < count; i++)
			{
				MetricId metricId = Stats[i];
				SerializedStats[i] = new SerializedStat
				{
					TypeName = metricId.EnumType.AssemblyQualifiedName,
					ValueName = metricId.Name
				};
			}
		}

		public void OnAfterDeserialize()
		{
			if (m_SerializedStatsLoaded)
			{
				return;
			}
			m_SerializedStatsLoaded = true;
			int count = SerializedStats.Count;
			Stats.Resize(count);
			for (int i = 0; i < count; i++)
			{
				SerializedStat serializedStat = SerializedStats[i];
				Type type = System.Type.GetType(serializedStat.TypeName);
				if (!(type == null))
				{
					int typeIndex = MetricIdTypeLibrary.GetTypeIndex(type);
					IReadOnlyList<string> enumNames = MetricIdTypeLibrary.GetEnumNames(typeIndex);
					string valueName = serializedStat.ValueName;
					int num = enumNames.IndexOf(valueName);
					if (num != -1)
					{
						int enumValue = MetricIdTypeLibrary.GetEnumValues(typeIndex)[num];
						Stats[i] = new MetricId(typeIndex, enumValue);
					}
				}
			}
		}

		internal int ComputeStatsHashCode()
		{
			int num = 0;
			foreach (MetricId stat in Stats)
			{
				num = HashCode.Combine(num, stat);
			}
			return num;
		}

		internal int ComputeHashCode()
		{
			int value = HashCode.Combine(Type, Label, ComputeStatsHashCode());
			switch (Type)
			{
			case DisplayElementType.Counter:
				return HashCode.Combine(value, CounterConfiguration.ComputeHashCode());
			case DisplayElementType.LineGraph:
			case DisplayElementType.StackedAreaGraph:
				return HashCode.Combine(value, GraphConfiguration.ComputeHashCode());
			default:
				throw new ArgumentOutOfRangeException(string.Format("Unknow {0} {1}", "DisplayElementType", Type));
			}
		}
	}
}
