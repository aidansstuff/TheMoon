using System.Net;
using System.Threading.Tasks;

namespace Unity.Services.Qos.Runner
{
	internal delegate Task<IPAddress[]> DnsResolver(string host);
}
