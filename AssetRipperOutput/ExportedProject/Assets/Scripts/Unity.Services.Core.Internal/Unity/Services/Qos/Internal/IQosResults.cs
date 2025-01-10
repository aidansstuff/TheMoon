using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Internal;
using UnityEngine.Scripting;

namespace Unity.Services.Qos.Internal
{
	[RequireImplementors]
	public interface IQosResults : IServiceComponent
	{
		Task<IList<QosResult>> GetSortedQosResultsAsync(string service, IList<string> regions);
	}
}
