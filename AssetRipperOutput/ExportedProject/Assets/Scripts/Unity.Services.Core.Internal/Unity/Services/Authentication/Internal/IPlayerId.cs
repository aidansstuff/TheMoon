using System;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Authentication.Internal
{
	[RequireImplementors]
	public interface IPlayerId : IServiceComponent
	{
		string PlayerId { get; }

		event Action<string> PlayerIdChanged;
	}
}
