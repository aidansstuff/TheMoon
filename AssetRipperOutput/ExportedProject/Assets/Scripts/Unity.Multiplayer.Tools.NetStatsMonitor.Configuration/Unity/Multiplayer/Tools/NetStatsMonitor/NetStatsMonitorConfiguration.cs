using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStatsMonitor
{
	[CreateAssetMenu(fileName = "NetStatsMonitorConfiguration", menuName = "Multiplayer/NetStatsMonitorConfiguration", order = 900)]
	public class NetStatsMonitorConfiguration : ScriptableObject
	{
		[field: SerializeField]
		public List<DisplayElementConfiguration> DisplayElements { get; set; } = new List<DisplayElementConfiguration>();


		[field: SerializeField]
		internal int? ConfigurationHash { get; private set; }

		public void OnConfigurationModified()
		{
			RecomputeConfigurationHash();
		}

		internal void OnValidate()
		{
			for (int i = 0; i < DisplayElements.Count; i++)
			{
				if (!DisplayElements[i].FieldsInitialized)
				{
					DisplayElements[i] = new DisplayElementConfiguration();
				}
				else
				{
					DisplayElements[i].OnValidate();
				}
			}
			RecomputeConfigurationHash();
		}

		internal void RecomputeConfigurationHash()
		{
			int num = 0;
			foreach (DisplayElementConfiguration displayElement in DisplayElements)
			{
				num = HashCode.Combine(num, displayElement.ComputeHashCode());
			}
			ConfigurationHash = num;
		}
	}
}
