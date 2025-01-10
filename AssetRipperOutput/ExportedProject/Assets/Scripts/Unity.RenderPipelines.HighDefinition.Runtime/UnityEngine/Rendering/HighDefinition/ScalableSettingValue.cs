using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class ScalableSettingValue<T>
	{
		[SerializeField]
		private T m_Override;

		[SerializeField]
		private bool m_UseOverride;

		[SerializeField]
		private int m_Level;

		public int level
		{
			get
			{
				return m_Level;
			}
			set
			{
				m_Level = value;
			}
		}

		public bool useOverride
		{
			get
			{
				return m_UseOverride;
			}
			set
			{
				m_UseOverride = value;
			}
		}

		public T @override
		{
			get
			{
				return m_Override;
			}
			set
			{
				m_Override = value;
			}
		}

		public T Value(ScalableSetting<T> source)
		{
			if (!m_UseOverride && source != null)
			{
				return source[m_Level];
			}
			return m_Override;
		}

		public void CopyTo(ScalableSettingValue<T> target)
		{
			target.m_Override = m_Override;
			target.m_UseOverride = m_UseOverride;
			target.m_Level = m_Level;
		}
	}
}
