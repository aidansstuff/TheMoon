using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Configuration
{
	internal static class LabelGeneration
	{
		public static (string, NetworkDirection) SeparateDirectionFromName(string name)
		{
			StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase;
			NetworkDirection networkDirection = NetworkDirection.None;
			string text = name;
			int num = text.IndexOf("sent", comparisonType);
			if (num > 0)
			{
				networkDirection |= NetworkDirection.Sent;
				text = text.Remove(num, "sent".Length);
			}
			int num2 = text.IndexOf("received", comparisonType);
			if (num2 > 0)
			{
				networkDirection |= NetworkDirection.Received;
				text = text.Remove(num2, "received".Length);
			}
			text = text.Trim();
			return (text, networkDirection);
		}

		public static string GenerateLabel(List<MetricId> stats)
		{
			string text;
			switch (stats.Count)
			{
			case 0:
				return "";
			case 1:
				return stats[0].DisplayName;
			case 2:
			{
				string displayName = stats[0].DisplayName;
				string displayName2 = stats[1].DisplayName;
				if (displayName == displayName2)
				{
					return "2 × " + displayName;
				}
				NetworkDirection networkDirection;
				(text, networkDirection) = SeparateDirectionFromName(displayName);
				var (text2, networkDirection2) = SeparateDirectionFromName(displayName2);
				if (text == text2)
				{
					if (networkDirection != NetworkDirection.Received)
					{
						if (networkDirection == NetworkDirection.Sent && networkDirection2 == NetworkDirection.Received)
						{
							goto IL_00b4;
						}
					}
					else if (networkDirection2 == NetworkDirection.Sent)
					{
						goto IL_00b4;
					}
				}
				switch (networkDirection)
				{
				case NetworkDirection.Sent:
					if (networkDirection2 != NetworkDirection.Sent)
					{
						break;
					}
					return text + " and " + text2 + " Sent";
				case NetworkDirection.Received:
					if (networkDirection2 != NetworkDirection.Received)
					{
						break;
					}
					return text + " and " + text2 + " Received";
				}
				return displayName + " and " + displayName2;
			}
			default:
				{
					return "";
				}
				IL_00b4:
				return text + " Sent and Received";
			}
		}
	}
}
