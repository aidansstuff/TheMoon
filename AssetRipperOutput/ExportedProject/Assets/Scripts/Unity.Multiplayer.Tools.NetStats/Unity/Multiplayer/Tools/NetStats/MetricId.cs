using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	public struct MetricId : IEquatable<MetricId>
	{
		[field: SerializeField]
		internal int TypeIndex { get; set; }

		[field: SerializeField]
		internal int EnumValue { get; set; }

		internal Type EnumType => MetricIdTypeLibrary.GetType(TypeIndex);

		[NotNull]
		internal string Name => MetricIdTypeLibrary.GetEnumName(TypeIndex, EnumValue);

		[NotNull]
		internal string DisplayName => MetricIdTypeLibrary.GetEnumDisplayName(TypeIndex, EnumValue);

		internal MetricKind MetricKind => MetricIdTypeLibrary.GetEnumMetricKind(TypeIndex, EnumValue);

		internal BaseUnits Units => MetricIdTypeLibrary.GetEnumUnit(TypeIndex, EnumValue);

		internal bool DisplayAsPercentage => MetricIdTypeLibrary.GetDisplayAsPercentage(TypeIndex, EnumValue);

		internal MetricId(int typeIndex, int enumValue)
		{
			if (!MetricIdTypeLibrary.IsValidTypeIndex(typeIndex))
			{
				throw new ArgumentOutOfRangeException(string.Format("Cannot construct {0} with out-of-range {1} {2}.", "MetricId", "TypeIndex", typeIndex));
			}
			TypeIndex = typeIndex;
			EnumValue = enumValue;
		}

		internal MetricId(Type enumType, int enumValue)
		{
			TypeIndex = MetricIdTypeLibrary.GetTypeIndex(enumType);
			EnumValue = enumValue;
		}

		public static MetricId Create<T>(T value) where T : struct, IConvertible
		{
			Type typeFromHandle = typeof(T);
			int enumValue = value.ToInt32(CultureInfo.InvariantCulture);
			return new MetricId(typeFromHandle, enumValue);
		}

		public bool Equals(MetricId other)
		{
			if (TypeIndex == other.TypeIndex)
			{
				return EnumValue == other.EnumValue;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((MetricId)obj);
		}

		public override int GetHashCode()
		{
			return 173 * TypeIndex + 13 * EnumValue;
		}

		public override string ToString()
		{
			return Name;
		}

		public static implicit operator string(MetricId metricId)
		{
			return metricId.ToString();
		}
	}
}
