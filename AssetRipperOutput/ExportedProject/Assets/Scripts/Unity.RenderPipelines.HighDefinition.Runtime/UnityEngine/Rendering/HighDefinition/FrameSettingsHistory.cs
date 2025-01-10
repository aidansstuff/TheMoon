using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct FrameSettingsHistory
	{
		internal static readonly string[] foldoutNames;

		private static readonly string[] columnNames;

		private static readonly string[] columnTooltips;

		private static readonly Dictionary<FrameSettingsField, FrameSettingsFieldAttribute> attributes;

		private static Dictionary<int, IOrderedEnumerable<KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute>>> attributesGroup;

		internal static HashSet<IFrameSettingsHistoryContainer> containers;

		public FrameSettingsRenderType defaultType;

		public FrameSettings overridden;

		public FrameSettingsOverrideMask customMask;

		public FrameSettings sanitazed;

		public FrameSettings debug;

		private bool hasDebug;

		private static bool s_PossiblyInUse;

		public static bool enabled
		{
			get
			{
				if (!s_PossiblyInUse)
				{
					return s_PossiblyInUse = DebugManager.instance.displayEditorUI || DebugManager.instance.displayRuntimeUI;
				}
				if (!DebugManager.instance.displayEditorUI && !DebugManager.instance.displayRuntimeUI)
				{
					if (s_PossiblyInUse)
					{
						return s_PossiblyInUse = containers.Any((IFrameSettingsHistoryContainer history) => history.frameSettingsHistory.hasDebug);
					}
					return false;
				}
				return true;
			}
		}

		static FrameSettingsHistory()
		{
			foldoutNames = new string[4] { "Rendering", "Lighting", "Async Compute", "Light Loop" };
			columnNames = new string[4] { "Debug", "Sanitized", "Overridden", "Default" };
			columnTooltips = new string[4] { "Displays Frame Setting values you can modify for the selected Camera.", "Displays the Frame Setting values that the selected Camera uses after Unity checks to see if your HDRP Asset supports them.", "Displays the Frame Setting values that the selected Camera overrides.", "Displays the default Frame Setting values in your current HDRP Asset." };
			attributesGroup = new Dictionary<int, IOrderedEnumerable<KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute>>>();
			containers = new HashSet<IFrameSettingsHistoryContainer>();
			attributes = new Dictionary<FrameSettingsField, FrameSettingsFieldAttribute>();
			attributesGroup = new Dictionary<int, IOrderedEnumerable<KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute>>>();
			Dictionary<FrameSettingsField, string> enumNameMap = FrameSettingsFieldAttribute.GetEnumNameMap();
			Type typeFromHandle = typeof(FrameSettingsField);
			foreach (FrameSettingsField key in enumNameMap.Keys)
			{
				attributes[key] = typeFromHandle.GetField(enumNameMap[key]).GetCustomAttribute<FrameSettingsFieldAttribute>();
			}
		}

		public static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, HDAdditionalCameraData additionalData, HDRenderPipelineAsset hdrpAsset, HDRenderPipelineAsset defaultHdrpAsset)
		{
			if (!(hdrpAsset == null) || !(defaultHdrpAsset == null))
			{
				AggregateFrameSettings(ref aggregatedFrameSettings, camera, additionalData, ref HDRenderPipelineGlobalSettings.instance.GetDefaultFrameSettings(additionalData?.defaultFrameSettings ?? FrameSettingsRenderType.Camera), (hdrpAsset != null) ? hdrpAsset.currentPlatformRenderPipelineSettings : defaultHdrpAsset.currentPlatformRenderPipelineSettings);
			}
		}

		public static void AggregateFrameSettings(ref FrameSettings aggregatedFrameSettings, Camera camera, IFrameSettingsHistoryContainer historyContainer, ref FrameSettings defaultFrameSettings, RenderPipelineSettings supportedFeatures)
		{
			FrameSettingsHistory frameSettingsHistory = historyContainer.frameSettingsHistory;
			aggregatedFrameSettings = defaultFrameSettings;
			bool flag = false;
			if (historyContainer.hasCustomFrameSettings)
			{
				FrameSettings.Override(ref aggregatedFrameSettings, historyContainer.frameSettings, historyContainer.frameSettingsMask);
				flag = frameSettingsHistory.customMask.mask != historyContainer.frameSettingsMask.mask;
				frameSettingsHistory.customMask = historyContainer.frameSettingsMask;
			}
			frameSettingsHistory.overridden = aggregatedFrameSettings;
			FrameSettings.Sanitize(ref aggregatedFrameSettings, camera, supportedFeatures);
			frameSettingsHistory.hasDebug = frameSettingsHistory.debug != aggregatedFrameSettings;
			flag |= frameSettingsHistory.sanitazed != aggregatedFrameSettings;
			bool num = !frameSettingsHistory.hasDebug || flag;
			frameSettingsHistory.sanitazed = aggregatedFrameSettings;
			if (num)
			{
				frameSettingsHistory.debug = frameSettingsHistory.sanitazed;
			}
			else
			{
				FrameSettings.Sanitize(ref frameSettingsHistory.debug, camera, supportedFeatures);
			}
			aggregatedFrameSettings = frameSettingsHistory.debug;
			historyContainer.frameSettingsHistory = frameSettingsHistory;
		}

		private static DebugUI.HistoryBoolField GenerateHistoryBoolField(IFrameSettingsHistoryContainer frameSettingsContainer, FrameSettingsField field, FrameSettingsFieldAttribute attribute)
		{
			string text = "";
			for (int i = 0; i < attribute.indentLevel; i++)
			{
				text += "  ";
			}
			DebugUI.HistoryBoolField historyBoolField = new DebugUI.HistoryBoolField();
			historyBoolField.displayName = text + attribute.displayedName;
			historyBoolField.tooltip = attribute.tooltip;
			historyBoolField.getter = () => frameSettingsContainer.frameSettingsHistory.debug.IsEnabled(field);
			historyBoolField.setter = delegate(bool value)
			{
				FrameSettingsHistory frameSettingsHistory = frameSettingsContainer.frameSettingsHistory;
				frameSettingsHistory.debug.SetEnabled(field, value);
				frameSettingsContainer.frameSettingsHistory = frameSettingsHistory;
			};
			historyBoolField.historyGetter = new Func<bool>[3]
			{
				() => frameSettingsContainer.frameSettingsHistory.sanitazed.IsEnabled(field),
				() => frameSettingsContainer.frameSettingsHistory.overridden.IsEnabled(field),
				() => HDRenderPipelineGlobalSettings.instance.GetDefaultFrameSettings(frameSettingsContainer.frameSettingsHistory.defaultType).IsEnabled(field)
			};
			return historyBoolField;
		}

		private static DebugUI.HistoryEnumField GenerateHistoryEnumField(IFrameSettingsHistoryContainer frameSettingsContainer, FrameSettingsField field, FrameSettingsFieldAttribute attribute, Type autoEnum)
		{
			string text = "";
			for (int i = 0; i < attribute.indentLevel; i++)
			{
				text += "  ";
			}
			DebugUI.HistoryEnumField historyEnumField = new DebugUI.HistoryEnumField();
			historyEnumField.displayName = text + attribute.displayedName;
			historyEnumField.tooltip = attribute.tooltip;
			historyEnumField.getter = () => frameSettingsContainer.frameSettingsHistory.debug.IsEnabled(field) ? 1 : 0;
			historyEnumField.setter = delegate(int value)
			{
				FrameSettingsHistory frameSettingsHistory = frameSettingsContainer.frameSettingsHistory;
				frameSettingsHistory.debug.SetEnabled(field, value == 1);
				frameSettingsContainer.frameSettingsHistory = frameSettingsHistory;
			};
			historyEnumField.autoEnum = autoEnum;
			historyEnumField.getIndex = () => frameSettingsContainer.frameSettingsHistory.debug.IsEnabled(field) ? 1 : 0;
			historyEnumField.setIndex = delegate
			{
			};
			historyEnumField.historyIndexGetter = new Func<int>[3]
			{
				() => frameSettingsContainer.frameSettingsHistory.sanitazed.IsEnabled(field) ? 1 : 0,
				() => frameSettingsContainer.frameSettingsHistory.overridden.IsEnabled(field) ? 1 : 0,
				() => HDRenderPipelineGlobalSettings.instance.GetDefaultFrameSettings(frameSettingsContainer.frameSettingsHistory.defaultType).IsEnabled(field) ? 1 : 0
			};
			return historyEnumField;
		}

		private static ObservableList<DebugUI.Widget> GenerateHistoryArea(IFrameSettingsHistoryContainer frameSettingsContainer, int groupIndex)
		{
			if (!attributesGroup.ContainsKey(groupIndex) || attributesGroup[groupIndex] == null)
			{
				attributesGroup[groupIndex] = attributes?.Where(delegate(KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute> pair)
				{
					FrameSettingsFieldAttribute value = pair.Value;
					return value != null && value.group == groupIndex;
				})?.OrderBy((KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute> pair) => pair.Value.orderInGroup);
			}
			if (!attributesGroup.ContainsKey(groupIndex))
			{
				throw new ArgumentException("Unknown groupIndex");
			}
			ObservableList<DebugUI.Widget> observableList = new ObservableList<DebugUI.Widget>();
			foreach (KeyValuePair<FrameSettingsField, FrameSettingsFieldAttribute> item in attributesGroup[groupIndex])
			{
				switch (item.Value.type)
				{
				case FrameSettingsFieldAttribute.DisplayType.BoolAsCheckbox:
					observableList.Add(GenerateHistoryBoolField(frameSettingsContainer, item.Key, item.Value));
					break;
				case FrameSettingsFieldAttribute.DisplayType.BoolAsEnumPopup:
					observableList.Add(GenerateHistoryEnumField(frameSettingsContainer, item.Key, item.Value, RetrieveEnumTypeByField(item.Key)));
					break;
				}
			}
			return observableList;
		}

		private static DebugUI.Widget[] GenerateFrameSettingsPanelContent(IFrameSettingsHistoryContainer frameSettingsContainer)
		{
			DebugUI.Widget[] array = new DebugUI.Widget[foldoutNames.Length];
			for (int i = 0; i < foldoutNames.Length; i++)
			{
				array[i] = new DebugUI.Foldout(foldoutNames[i], GenerateHistoryArea(frameSettingsContainer, i), columnNames, columnTooltips);
			}
			return array;
		}

		private static void GenerateFrameSettingsPanel(string menuName, IFrameSettingsHistoryContainer frameSettingsContainer)
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			list.AddRange(GenerateFrameSettingsPanelContent(frameSettingsContainer));
			DebugManager.instance.GetPanel(menuName, createIfNull: true, 2, overrideIfExist: true).children.Add(list.ToArray());
		}

		private static Type RetrieveEnumTypeByField(FrameSettingsField field)
		{
			if (field == FrameSettingsField.LitShaderMode)
			{
				return typeof(LitShaderMode);
			}
			throw new ArgumentException("Unknown enum type for this field");
		}

		public static IDebugData RegisterDebug(IFrameSettingsHistoryContainer frameSettingsContainer, bool sceneViewCamera = false)
		{
			GenerateFrameSettingsPanel(frameSettingsContainer.panelName, frameSettingsContainer);
			containers.Add(frameSettingsContainer);
			return frameSettingsContainer;
		}

		public static void UnRegisterDebug(IFrameSettingsHistoryContainer container)
		{
			DebugManager.instance.RemovePanel(container.panelName);
			containers.Remove(container);
		}

		public static bool IsRegistered(IFrameSettingsHistoryContainer container, bool sceneViewCamera = false)
		{
			if (sceneViewCamera)
			{
				return true;
			}
			return containers.Contains(container);
		}

		internal void TriggerReset()
		{
			debug = sanitazed;
			hasDebug = false;
		}
	}
}
