using System;
using JetBrains.Annotations;
using Unity.Multiplayer.Tools.NetStats;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[AddComponentMenu("Netcode/RuntimeNetStatsMonitor", 1000)]
	public class RuntimeNetStatsMonitor : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Visibility toggle to hide or show the on-screen display.")]
		private bool m_Visible = true;

		[SerializeField]
		[Min(1f)]
		[Tooltip("The maximum rate at which the Runtime Net Stats Monitor's on-screen display is updated (per second). The on-screen display will never be updated faster than the overall refresh rate.")]
		private double m_MaxRefreshRate = 30.0;

		public bool Visible
		{
			get
			{
				return m_Visible;
			}
			set
			{
				m_Visible = value;
			}
		}

		public double MaxRefreshRate
		{
			get
			{
				return m_MaxRefreshRate;
			}
			set
			{
				m_MaxRefreshRate = Math.Max(value, 1.0);
			}
		}

		[field: SerializeField]
		public StyleSheet CustomStyleSheet { get; set; }

		[field: Tooltip("Optional panel settings that can be used to override the default. These panel settings can be used to control a number of things, including how the on-screen display of the Runtime Net Stats Monitor scales on different devices and displays. ")]
		[field: SerializeField]
		public PanelSettings PanelSettingsOverride { get; set; }

		[field: SerializeField]
		public PositionConfiguration Position { get; set; } = new PositionConfiguration();


		[CanBeNull]
		[field: SerializeField]
		[field: Tooltip("The configuration asset used to configure the information displayed in this Runtime Net Stats Monitor. The NetStatsMonitorConfiguration can created from the Create menu, or from C# using ScriptableObject.CreateInstance.")]
		public NetStatsMonitorConfiguration Configuration { get; set; }

		private void Start()
		{
			Setup();
		}

		private void OnEnable()
		{
			Setup();
		}

		private void OnDisable()
		{
			Teardown();
		}

		private void OnDestroy()
		{
			Teardown();
		}

		private void OnValidate()
		{
			if (base.enabled)
			{
				ApplyConfiguration();
			}
			else
			{
				Teardown();
			}
		}

		internal void Setup()
		{
		}

		internal void Teardown()
		{
		}

		public void ApplyConfiguration()
		{
			if (Configuration != null)
			{
				Configuration.RecomputeConfigurationHash();
			}
		}

		public void AddCustomValue(MetricId metricId, float value)
		{
		}
	}
}
