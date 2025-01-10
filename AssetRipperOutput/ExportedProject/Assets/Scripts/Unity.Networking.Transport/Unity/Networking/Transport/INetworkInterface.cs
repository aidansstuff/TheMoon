using System;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Networking.Transport
{
	public interface INetworkInterface : IDisposable
	{
		NetworkInterfaceEndPoint LocalEndPoint { get; }

		int Initialize(NetworkSettings settings);

		JobHandle ScheduleReceive(NetworkPacketReceiver receiver, JobHandle dep);

		JobHandle ScheduleSend(NativeQueue<QueuedSendMessage> sendQueue, JobHandle dep);

		int Bind(NetworkInterfaceEndPoint endpoint);

		int Listen();

		NetworkSendInterface CreateSendInterface();

		int CreateInterfaceEndPoint(NetworkEndPoint address, out NetworkInterfaceEndPoint endpoint);

		NetworkEndPoint GetGenericEndPoint(NetworkInterfaceEndPoint endpoint);
	}
}
