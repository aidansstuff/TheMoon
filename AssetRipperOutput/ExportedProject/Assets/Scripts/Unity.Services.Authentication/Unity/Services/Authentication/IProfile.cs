using System;

namespace Unity.Services.Authentication
{
	internal interface IProfile
	{
		string Current { get; set; }

		event Action<ProfileEventArgs> ProfileChange;
	}
}
