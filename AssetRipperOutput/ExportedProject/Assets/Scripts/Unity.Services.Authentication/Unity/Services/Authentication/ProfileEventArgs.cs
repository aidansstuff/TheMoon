using System;

namespace Unity.Services.Authentication
{
	internal class ProfileEventArgs : EventArgs
	{
		public string Profile { get; }

		public ProfileEventArgs(string profile)
		{
			Profile = profile;
		}
	}
}
