using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Core.Configuration
{
	internal class CloudProjectId : ICloudProjectId, IServiceComponent
	{
		public string GetCloudProjectId()
		{
			return Application.cloudProjectId;
		}
	}
}
