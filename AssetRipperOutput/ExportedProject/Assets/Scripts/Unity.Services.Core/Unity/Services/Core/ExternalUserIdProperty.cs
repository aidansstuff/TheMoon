using System;

namespace Unity.Services.Core
{
	internal class ExternalUserIdProperty
	{
		private string m_UserId;

		public string UserId
		{
			get
			{
				return m_UserId;
			}
			set
			{
				m_UserId = value;
				this.UserIdChanged?.Invoke(m_UserId);
			}
		}

		public event Action<string> UserIdChanged;
	}
}
