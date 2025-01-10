using System;
using System.Threading.Tasks;

namespace Unity.Services.Vivox.Internal
{
	public interface IVivoxTokenProviderInternal
	{
		Task<string> GetTokenAsync(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string conferenceUri = null, string fromUserUri = null, string realm = null);
	}
}
