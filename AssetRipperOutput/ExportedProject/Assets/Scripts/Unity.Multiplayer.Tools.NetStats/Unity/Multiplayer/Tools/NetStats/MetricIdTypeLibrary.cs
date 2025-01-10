using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.Common;

namespace Unity.Multiplayer.Tools.NetStats
{
	public static class MetricIdTypeLibrary
	{
		private static readonly List<Type> k_Types;

		private static readonly List<string> k_TypeDisplayNames;

		private static readonly List<int[]> k_EnumValues;

		private static readonly List<string[]> k_EnumNames;

		private static readonly List<string[]> k_EnumDisplayNames;

		private static readonly List<MetricKind[]> k_MetricKinds;

		private static readonly List<BaseUnits[]> k_Units;

		private static readonly List<bool[]> k_DisplayAsPercentage;

		internal static IReadOnlyList<Type> Types => k_Types;

		internal static IReadOnlyList<string> TypeDisplayNames => k_TypeDisplayNames;

		static MetricIdTypeLibrary()
		{
			k_Types = new List<Type>();
			k_TypeDisplayNames = new List<string>();
			k_EnumValues = new List<int[]>();
			k_EnumNames = new List<string[]>();
			k_EnumDisplayNames = new List<string[]>();
			k_MetricKinds = new List<MetricKind[]>();
			k_Units = new List<BaseUnits[]>();
			k_DisplayAsPercentage = new List<bool[]>();
			TypeRegistration.RunIfNeeded();
		}

		public static void RegisterType<TEnumType>()
		{
			k_Types.Add(typeof(TEnumType));
		}

		internal static void TypeRegistrationPostProcess()
		{
			k_Types.Sort(delegate(Type a, Type b)
			{
				SortPriority sortPriority = a.GetCustomAttribute<MetricTypeSortPriorityAttribute>()?.SortPriority ?? SortPriority.Neutral;
				SortPriority sortPriority2 = b.GetCustomAttribute<MetricTypeSortPriorityAttribute>()?.SortPriority ?? SortPriority.Neutral;
				int num = sortPriority.CompareTo(sortPriority2);
				return (num != 0) ? num : StringComparer.InvariantCulture.Compare(a.FullName, b.FullName);
			});
			foreach (Type k_Type in k_Types)
			{
				string item = k_Type.GetCustomAttribute<MetricTypeEnumAttribute>()?.DisplayName ?? k_Type.Name;
				int[] array = k_Type.GetEnumValues().Cast<int>().ToArray();
				string[] enumNames = k_Type.GetEnumNames();
				Array.Sort(enumNames, array);
				string[] array2 = new string[array.Length];
				MetricKind[] array3 = new MetricKind[array.Length];
				BaseUnits[] array4 = new BaseUnits[array.Length];
				bool[] array5 = new bool[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text = enumNames[i];
					MetricMetadataAttribute metricMetadataAttribute = k_Type.GetMember(text).FirstOrDefault()?.GetCustomAttribute<MetricMetadataAttribute>();
					if (metricMetadataAttribute != null)
					{
						array2[i] = metricMetadataAttribute.DisplayName ?? StringUtil.AddSpacesToCamelCase(text);
						array3[i] = metricMetadataAttribute.MetricKind;
						array4[i] = metricMetadataAttribute.Units.GetBaseUnits();
						array5[i] = metricMetadataAttribute.DisplayAsPercentage;
					}
					ref string reference = ref array2[i];
					if (reference == null)
					{
						reference = StringUtil.AddSpacesToCamelCase(text);
					}
					if (array3[i] == MetricKind.Counter)
					{
						BaseUnits baseUnits = array4[i];
						array4[i] = baseUnits.WithSeconds((sbyte)(baseUnits.SecondsExponent - 1));
					}
				}
				k_TypeDisplayNames.Add(item);
				k_EnumValues.Add(array);
				k_EnumNames.Add(enumNames);
				k_EnumDisplayNames.Add(array2);
				k_MetricKinds.Add(array3);
				k_Units.Add(array4);
				k_DisplayAsPercentage.Add(array5);
			}
		}

		internal static bool IsValidTypeIndex(int index)
		{
			if (0 <= index)
			{
				return index < k_Types.Count;
			}
			return false;
		}

		internal static int GetTypeIndex(Type type)
		{
			return k_Types.IndexOf(type);
		}

		internal static Type GetType(int typeIndex)
		{
			return k_Types[typeIndex];
		}

		internal static bool ContainsType(Type type)
		{
			return k_Types.Contains(type);
		}

		internal static IReadOnlyList<int> GetEnumValues(int typeIndex)
		{
			return k_EnumValues[typeIndex];
		}

		internal static IReadOnlyList<string> GetEnumNames(int typeIndex)
		{
			return k_EnumNames[typeIndex];
		}

		[NotNull]
		internal static string GetEnumName(int typeIndex, int enumValue)
		{
			return GetEnumMetadata(k_EnumNames, typeIndex, enumValue) ?? enumValue.ToString();
		}

		internal static MetricKind GetEnumMetricKind(int typeIndex, int enumValue)
		{
			return GetEnumMetadata(k_MetricKinds, typeIndex, enumValue);
		}

		internal static IReadOnlyList<string> GetEnumDisplayNames(int typeIndex)
		{
			return k_EnumDisplayNames[typeIndex];
		}

		[NotNull]
		internal static string GetEnumDisplayName(int typeIndex, int enumValue)
		{
			return GetEnumMetadata(k_EnumDisplayNames, typeIndex, enumValue) ?? "";
		}

		internal static BaseUnits GetEnumUnit(int typeIndex, int enumValue)
		{
			return GetEnumMetadata(k_Units, typeIndex, enumValue);
		}

		internal static bool GetDisplayAsPercentage(int typeIndex, int enumValue)
		{
			return GetEnumMetadata(k_DisplayAsPercentage, typeIndex, enumValue);
		}

		private static T GetEnumMetadata<T>(List<T[]> data, int typeIndex, int enumValue)
		{
			if (typeIndex > k_EnumValues.Count)
			{
				return default(T);
			}
			int num = Array.IndexOf(k_EnumValues[typeIndex], enumValue);
			if (num != -1)
			{
				return data[typeIndex][num];
			}
			return default(T);
		}
	}
}
