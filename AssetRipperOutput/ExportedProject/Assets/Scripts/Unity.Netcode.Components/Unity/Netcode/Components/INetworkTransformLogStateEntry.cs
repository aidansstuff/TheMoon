namespace Unity.Netcode.Components
{
	internal interface INetworkTransformLogStateEntry
	{
		void AddLogEntry(NetworkTransform.NetworkTransformState networkTransformState, ulong targetClient, bool preUpdate = false);
	}
}
