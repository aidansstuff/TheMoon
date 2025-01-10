using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Qos.Internal;
using Unity.Services.Qos.Models;

namespace Unity.Services.Qos.Runner
{
	internal interface IQosRunner
	{
		Task<List<Unity.Services.Qos.Internal.QosResult>> MeasureQosAsync(IList<QosServer> servers);

		Task<List<QosAnnotatedResult>> MeasureQosAsync(IList<QosServiceServer> servers);
	}
}
