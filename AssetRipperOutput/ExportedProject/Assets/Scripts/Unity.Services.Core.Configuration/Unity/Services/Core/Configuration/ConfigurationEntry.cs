using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Services.Core.Configuration
{
	[Serializable]
	internal class ConfigurationEntry
	{
		[JsonRequired]
		[SerializeField]
		private string m_Value;

		[JsonRequired]
		[SerializeField]
		private bool m_IsReadOnly;

		[JsonIgnore]
		public string Value => m_Value;

		[JsonIgnore]
		public bool IsReadOnly
		{
			get
			{
				return m_IsReadOnly;
			}
			internal set
			{
				m_IsReadOnly = value;
			}
		}

		public ConfigurationEntry()
		{
		}

		public ConfigurationEntry(string value, bool isReadOnly = false)
		{
			m_Value = value;
			m_IsReadOnly = isReadOnly;
		}

		public bool TrySetValue(string value)
		{
			if (IsReadOnly)
			{
				return false;
			}
			m_Value = value;
			return true;
		}

		public static implicit operator string(ConfigurationEntry entry)
		{
			return entry.Value;
		}

		public static implicit operator ConfigurationEntry(string value)
		{
			return new ConfigurationEntry(value);
		}
	}
}
