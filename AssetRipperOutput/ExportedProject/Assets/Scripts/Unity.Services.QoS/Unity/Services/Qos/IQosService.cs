using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.Qos
{
	public interface IQosService
	{
		Task<IList<IQosResult>> GetSortedQosResultsAsync(string service, IList<string> regions);

		Task<IList<IQosAnnotatedResult>> GetSortedRelayQosResultsAsync(IList<string> regions);

		Task<IList<IQosAnnotatedResult>> GetSortedMultiplayQosResultsAsync(IList<string> fleet);
	}
}
