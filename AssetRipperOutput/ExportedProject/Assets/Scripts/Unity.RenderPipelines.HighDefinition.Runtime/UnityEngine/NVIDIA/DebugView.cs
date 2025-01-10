using System;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.NVIDIA
{
	internal class DebugView
	{
		private enum DeviceState
		{
			Unknown = 0,
			MissingPluginDLL = 1,
			DeviceCreationFailed = 2,
			Active = 3
		}

		private class Container<T> where T : struct
		{
			public T data = new T();
		}

		private class Data
		{
			public DeviceState deviceState;

			public bool dlssSupported;

			public Container<DLSSDebugFeatureInfos>[] dlssFeatureInfos;
		}

		private GraphicsDeviceDebugView m_DebugView;

		private Data m_Data = new Data();

		private const int MaxDebugRows = 4;

		private DebugUI.Container m_DebugWidget;

		private DebugUI.Table.Row[] m_DlssViewStateTableRows;

		private DebugUI.Container m_DlssViewStateTableHeader;

		private DebugUI.Table m_DlssViewStateTable;

		internal void Reset()
		{
			InternalReset();
		}

		internal void Update()
		{
			InternalUpdate();
		}

		internal DebugUI.Widget CreateWidget()
		{
			return InternalCreateWidget();
		}

		private void InternalReset()
		{
			GraphicsDevice device = GraphicsDevice.device;
			if (device != null && m_DebugView != null)
			{
				device.DeleteDebugView(m_DebugView);
			}
			m_DebugView = null;
		}

		private void InternalUpdate()
		{
			GraphicsDevice device = GraphicsDevice.device;
			bool flag = DebugManager.instance.displayRuntimeUI || DebugManager.instance.displayEditorUI;
			if (device != null)
			{
				if (flag && m_DebugView == null)
				{
					m_DebugView = device.CreateDebugView();
				}
				else if (!flag && m_DebugView != null)
				{
					device.DeleteDebugView(m_DebugView);
					m_DebugView = null;
				}
			}
			if (device != null)
			{
				if (m_DebugView != null)
				{
					m_Data.deviceState = DeviceState.Active;
					m_Data.dlssSupported = device.IsFeatureAvailable(GraphicsDeviceFeature.DLSS);
					device.UpdateDebugView(m_DebugView);
					TranslateDlssFeatureArray(m_Data.dlssFeatureInfos, in m_DebugView);
				}
				else
				{
					m_Data.deviceState = DeviceState.Unknown;
				}
			}
			else if (device == null)
			{
				bool flag2 = NVUnityPlugin.IsLoaded();
				m_Data.deviceState = ((!flag2) ? DeviceState.MissingPluginDLL : DeviceState.DeviceCreationFailed);
				m_Data.dlssSupported = false;
				ClearFeatureStateContainer(m_Data.dlssFeatureInfos);
			}
			UpdateDebugUITable();
		}

		private static void ClearFeatureStateContainer(Container<DLSSDebugFeatureInfos>[] containerArray)
		{
			for (int i = 0; i < containerArray.Length; i++)
			{
				containerArray[i].data = default(DLSSDebugFeatureInfos);
			}
		}

		private static void TranslateDlssFeatureArray(Container<DLSSDebugFeatureInfos>[] containerArray, in GraphicsDeviceDebugView debugView)
		{
			ClearFeatureStateContainer(containerArray);
			if (!debugView.dlssFeatureInfos.Any())
			{
				return;
			}
			int num = 0;
			foreach (DLSSDebugFeatureInfos dlssFeatureInfo in debugView.dlssFeatureInfos)
			{
				if (num == containerArray.Length)
				{
					break;
				}
				containerArray[num++].data = dlssFeatureInfo;
			}
		}

		private DebugUI.Widget InternalCreateWidget()
		{
			if (m_DebugWidget != null)
			{
				return m_DebugWidget;
			}
			m_DlssViewStateTableHeader = new DebugUI.Table.Row
			{
				displayName = "",
				children = 
				{
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Status"
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Input resolution"
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Output resolution"
					},
					(DebugUI.Widget)new DebugUI.Container
					{
						displayName = "Quality"
					}
				}
			};
			m_DlssViewStateTable = new DebugUI.Table
			{
				displayName = "DLSS Slot ID",
				isReadOnly = true
			};
			m_DlssViewStateTable.children.Add(m_DlssViewStateTableHeader);
			m_DebugWidget = new DebugUI.Container
			{
				displayName = "NVIDIA device debug view",
				children = 
				{
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "NVUnityPlugin Version",
						getter = () => (m_DebugView != null) ? m_DebugView.deviceVersion.ToString("X2") : "-"
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "NGX API Version",
						getter = () => (m_DebugView != null) ? m_DebugView.ngxVersion.ToString("X2") : "-"
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "Device Status",
						getter = () => m_Data.deviceState.ToString()
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "DLSS Supported",
						getter = () => (!m_Data.dlssSupported) ? "False" : "True"
					},
					(DebugUI.Widget)new DebugUI.Value
					{
						displayName = "DLSS Injection Point",
						getter = () => HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings.dynamicResolutionSettings.DLSSInjectionPoint
					},
					(DebugUI.Widget)m_DlssViewStateTable
				}
			};
			m_Data.dlssFeatureInfos = new Container<DLSSDebugFeatureInfos>[4];
			m_DlssViewStateTableRows = new DebugUI.Table.Row[m_Data.dlssFeatureInfos.Length];
			for (int i = 0; i < m_Data.dlssFeatureInfos.Length; i++)
			{
				Container<DLSSDebugFeatureInfos> c = new Container<DLSSDebugFeatureInfos>
				{
					data = default(DLSSDebugFeatureInfos)
				};
				m_Data.dlssFeatureInfos[i] = c;
				DebugUI.Table.Row row = new DebugUI.Table.Row
				{
					children = 
					{
						(DebugUI.Widget)new DebugUI.Value
						{
							getter = () => (!c.data.validFeature) ? "" : "Valid"
						},
						(DebugUI.Widget)new DebugUI.Value
						{
							getter = () => (!c.data.validFeature) ? "" : resToString(c.data.execData.subrectWidth, c.data.execData.subrectHeight)
						},
						(DebugUI.Widget)new DebugUI.Value
						{
							getter = () => (!c.data.validFeature) ? "" : resToString(c.data.initData.outputRTWidth, c.data.initData.outputRTHeight)
						},
						(DebugUI.Widget)new DebugUI.Value
						{
							getter = () => (!c.data.validFeature) ? "" : c.data.initData.quality.ToString()
						}
					}
				};
				row.isHiddenCallback = () => !c.data.validFeature;
				m_DlssViewStateTableRows[i] = row;
			}
			ObservableList<DebugUI.Widget> children = m_DlssViewStateTable.children;
			DebugUI.Widget[] dlssViewStateTableRows = m_DlssViewStateTableRows;
			children.Add(dlssViewStateTableRows);
			return m_DebugWidget;
			static string resToString(uint a, uint b)
			{
				return a + "x" + b;
			}
		}

		private void UpdateDebugUITable()
		{
			for (int i = 0; i < m_DlssViewStateTableRows.Length; i++)
			{
				DLSSDebugFeatureInfos data = m_Data.dlssFeatureInfos[i].data;
				m_DlssViewStateTableRows[i].displayName = (data.validFeature ? Convert.ToString(data.featureSlot) : "");
			}
		}
	}
}
