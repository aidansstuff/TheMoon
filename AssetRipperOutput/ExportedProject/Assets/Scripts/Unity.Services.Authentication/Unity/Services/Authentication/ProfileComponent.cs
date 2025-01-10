using System;

namespace Unity.Services.Authentication
{
	internal class ProfileComponent : IProfile
	{
		private string _current;

		public string Current
		{
			get
			{
				return _current;
			}
			set
			{
				SetProfile(value);
			}
		}

		public event Action<ProfileEventArgs> ProfileChange;

		internal ProfileComponent(string profile)
		{
			SetProfile(profile);
		}

		public void SetProfile(string profile)
		{
			_current = profile;
			this.ProfileChange?.Invoke(new ProfileEventArgs(_current));
		}
	}
}
