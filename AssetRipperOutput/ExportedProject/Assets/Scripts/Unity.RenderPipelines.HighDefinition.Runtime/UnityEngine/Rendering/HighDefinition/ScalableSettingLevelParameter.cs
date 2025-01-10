using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class ScalableSettingLevelParameter : NoInterpIntParameter
	{
		public enum Level
		{
			Low = 0,
			Medium = 1,
			High = 2
		}

		public const int LevelCount = 3;

		public (int level, bool useOverride) levelAndOverride
		{
			get
			{
				if (value != 3)
				{
					return (level: value, useOverride: false);
				}
				return (level: 0, useOverride: true);
			}
			set
			{
				(int level, bool useOverride) tuple = value;
				int item = tuple.level;
				bool item2 = tuple.useOverride;
				this.value = GetScalableSettingLevelParameterValue(item, item2);
			}
		}

		public ScalableSettingLevelParameter(int level, bool useOverride, bool overrideState = false)
			: base(useOverride ? 3 : level, overrideState)
		{
		}

		internal static int GetScalableSettingLevelParameterValue(int level, bool useOverride)
		{
			if (!useOverride)
			{
				return level;
			}
			return 3;
		}
	}
}
