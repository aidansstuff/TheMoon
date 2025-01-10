using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Internal;
using Unity.Services.Qos.Internal;

namespace Unity.Services.Qos
{
	internal class QosResults : IQosResults, IServiceComponent
	{
		private WrappedQosService _qosService;

		internal QosResults(WrappedQosService qosService)
		{
			_qosService = qosService;
		}

		public Task<IList<Unity.Services.Qos.Internal.QosResult>> GetSortedQosResultsAsync(string service, IList<string> regions)
		{
			return _qosService.GetSortedInternalQosResultsAsync(service, regions);
		}
	}
}
