using System;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Internal
{
	[RequireImplementors]
	internal interface IPlayerName
	{
		string PlayerName { get; }

		event Action<string> PlayerNameChanged;
	}
}
