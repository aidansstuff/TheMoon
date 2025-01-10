using System.Threading.Tasks;

namespace Unity.Services.Wire.Internal
{
	public interface IChannelTokenProvider
	{
		Task<ChannelToken> GetTokenAsync();
	}
}
