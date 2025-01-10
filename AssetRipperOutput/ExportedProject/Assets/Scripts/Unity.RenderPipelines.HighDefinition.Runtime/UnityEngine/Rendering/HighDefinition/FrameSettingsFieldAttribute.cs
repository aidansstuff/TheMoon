using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Rendering.HighDefinition
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class FrameSettingsFieldAttribute : Attribute
	{
		public enum DisplayType
		{
			BoolAsCheckbox = 0,
			BoolAsEnumPopup = 1,
			Others = 2
		}

		public readonly DisplayType type;

		public readonly string displayedName;

		public readonly string tooltip;

		public readonly int group;

		public readonly int orderInGroup;

		public readonly Type targetType;

		public readonly int indentLevel;

		public readonly FrameSettingsField[] dependencies;

		private readonly int dependencySeparator;

		private static int autoOrder;

		private static Dictionary<FrameSettingsField, string> s_FrameSettingsEnumNameMap;

		public static Dictionary<FrameSettingsField, string> GetEnumNameMap()
		{
			if (s_FrameSettingsEnumNameMap == null)
			{
				s_FrameSettingsEnumNameMap = new Dictionary<FrameSettingsField, string>();
				Type typeFromHandle = typeof(FrameSettingsField);
				string[] names = Enum.GetNames(typeFromHandle);
				foreach (string text in names)
				{
					if (typeFromHandle.GetField(text).GetCustomAttribute<ObsoleteAttribute>() == null)
					{
						s_FrameSettingsEnumNameMap.Add((FrameSettingsField)Enum.Parse(typeFromHandle, text), text);
					}
				}
			}
			return s_FrameSettingsEnumNameMap;
		}

		static FrameSettingsFieldAttribute()
		{
			GetEnumNameMap();
		}

		public FrameSettingsFieldAttribute(int group, FrameSettingsField autoName = FrameSettingsField.None, string displayedName = null, string tooltip = null, DisplayType type = DisplayType.BoolAsCheckbox, Type targetType = null, FrameSettingsField[] positiveDependencies = null, FrameSettingsField[] negativeDependencies = null, int customOrderInGroup = -1)
		{
			if (string.IsNullOrEmpty(displayedName))
			{
				if (!s_FrameSettingsEnumNameMap.TryGetValue(autoName, out displayedName))
				{
					displayedName = autoName.ToString();
				}
				displayedName = displayedName.CamelToPascalCaseWithSpace();
			}
			this.group = group;
			if (customOrderInGroup != -1)
			{
				autoOrder = customOrderInGroup;
			}
			orderInGroup = autoOrder++;
			this.displayedName = displayedName;
			this.type = type;
			this.targetType = targetType;
			dependencySeparator = ((positiveDependencies != null) ? positiveDependencies.Length : 0);
			dependencies = new FrameSettingsField[dependencySeparator + ((negativeDependencies != null) ? negativeDependencies.Length : 0)];
			positiveDependencies?.CopyTo(dependencies, 0);
			negativeDependencies?.CopyTo(dependencies, dependencySeparator);
			FrameSettingsField[] array = dependencies;
			indentLevel = ((array != null) ? array.Length : 0);
		}

		public bool IsNegativeDependency(FrameSettingsField frameSettingsField)
		{
			return Array.FindIndex(dependencies, (FrameSettingsField fsf) => fsf == frameSettingsField) >= dependencySeparator;
		}
	}
}
