namespace Unity.Networking.Transport
{
	public interface INetworkPipelineStage
	{
		int StaticSize { get; }

		unsafe NetworkPipelineStage StaticInitialize(byte* staticInstanceBuffer, int staticInstanceBufferLength, NetworkSettings settings);
	}
}
