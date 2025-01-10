using System;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Core.Configuration.Internal
{
	[RequireImplementors]
	public interface IExternalUserId : IServiceComponent
	{
		string UserId { get; }

		event Action<string> UserIdChanged;
	}
}
