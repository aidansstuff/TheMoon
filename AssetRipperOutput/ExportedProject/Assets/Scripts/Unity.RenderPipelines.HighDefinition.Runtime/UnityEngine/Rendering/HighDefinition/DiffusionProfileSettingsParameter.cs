using System;
using System.Buffers;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class DiffusionProfileSettingsParameter : VolumeParameter<DiffusionProfileSettings[]>
	{
		private static ArrayPool<DiffusionProfileSettings> s_ArrayPool = ArrayPool<DiffusionProfileSettings>.Create(16, 5);

		internal DiffusionProfileSettings[] accumulatedArray;

		internal int accumulatedCount;

		public DiffusionProfileSettingsParameter(DiffusionProfileSettings[] value, bool overrideState = true)
			: base(value, overrideState)
		{
		}

		private void AddProfile(DiffusionProfileSettings profile)
		{
			if (profile == null)
			{
				return;
			}
			for (int i = 0; i < accumulatedCount; i++)
			{
				if (profile == m_Value[i])
				{
					return;
				}
			}
			m_Value[accumulatedCount++] = profile;
		}

		public override void Interp(DiffusionProfileSettings[] from, DiffusionProfileSettings[] to, float t)
		{
			m_Value = s_ArrayPool.Rent(16);
			accumulatedCount = 0;
			m_Value[accumulatedCount++] = HDRenderPipeline.currentPipeline?.defaultDiffusionProfile;
			if (to != null)
			{
				DiffusionProfileSettings[] array = to;
				foreach (DiffusionProfileSettings profile in array)
				{
					AddProfile(profile);
					if (accumulatedCount >= 16)
					{
						break;
					}
				}
			}
			if (from != null)
			{
				DiffusionProfileSettings[] array = from;
				foreach (DiffusionProfileSettings profile2 in array)
				{
					AddProfile(profile2);
					if (accumulatedCount >= 16)
					{
						break;
					}
				}
			}
			for (int j = accumulatedCount; j < m_Value.Length; j++)
			{
				m_Value[j] = null;
			}
			if (accumulatedArray != null)
			{
				s_ArrayPool.Return(accumulatedArray);
			}
			accumulatedArray = m_Value;
		}

		public override void Release()
		{
			if (accumulatedArray != null)
			{
				s_ArrayPool.Return(accumulatedArray);
			}
			accumulatedArray = null;
		}
	}
}
