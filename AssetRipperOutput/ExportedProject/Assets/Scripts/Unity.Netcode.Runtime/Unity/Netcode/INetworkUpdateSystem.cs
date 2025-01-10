namespace Unity.Netcode
{
	public interface INetworkUpdateSystem
	{
		void NetworkUpdate(NetworkUpdateStage updateStage);
	}
}
