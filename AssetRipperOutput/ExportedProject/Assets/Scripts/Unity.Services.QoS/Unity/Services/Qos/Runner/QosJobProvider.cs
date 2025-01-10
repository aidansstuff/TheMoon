using System.Collections.Generic;
using Unity.Networking.QoS;

namespace Unity.Services.Qos.Runner
{
	internal delegate IQosJob QosJobProvider(IList<UcgQosServer> servers, string title);
}
