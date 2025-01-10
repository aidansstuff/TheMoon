using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Networking.Transport.Protocols;
using UnityEngine;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompile]
	internal struct UnityTransportProtocol : INetworkProtocol, IDisposable
	{
		public void Initialize(NetworkSettings settings)
		{
		}

		public void Dispose()
		{
		}

		public int Bind(INetworkInterface networkInterface, ref NetworkInterfaceEndPoint localEndPoint)
		{
			if (networkInterface.Bind(localEndPoint) != 0)
			{
				return -1;
			}
			return 0;
		}

		public int CreateConnectionAddress(INetworkInterface networkInterface, NetworkEndPoint remoteEndpoint, out NetworkInterfaceEndPoint remoteAddress)
		{
			remoteAddress = default(NetworkInterfaceEndPoint);
			return networkInterface.CreateInterfaceEndPoint(remoteEndpoint, out remoteAddress);
		}

		public NetworkEndPoint GetRemoteEndPoint(INetworkInterface networkInterface, NetworkInterfaceEndPoint address)
		{
			return networkInterface.GetGenericEndPoint(address);
		}

		public NetworkProtocol CreateProtocolInterface()
		{
			return new NetworkProtocol(new TransportFunctionPointer<NetworkProtocol.ComputePacketOverheadDelegate>(ComputePacketOverhead), new TransportFunctionPointer<NetworkProtocol.ProcessReceiveDelegate>(ProcessReceive), new TransportFunctionPointer<NetworkProtocol.ProcessSendDelegate>(ProcessSend), new TransportFunctionPointer<NetworkProtocol.ProcessSendConnectionAcceptDelegate>(ProcessSendConnectionAccept), new TransportFunctionPointer<NetworkProtocol.ConnectDelegate>(Connect), new TransportFunctionPointer<NetworkProtocol.DisconnectDelegate>(Disconnect), new TransportFunctionPointer<NetworkProtocol.ProcessSendPingDelegate>(ProcessSendPing), new TransportFunctionPointer<NetworkProtocol.ProcessSendPongDelegate>(ProcessSendPong), new TransportFunctionPointer<NetworkProtocol.UpdateDelegate>(Update), needsUpdate: false, IntPtr.Zero, 10, 8);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ComputePacketOverheadDelegate))]
		public static int ComputePacketOverhead(ref NetworkDriver.Connection connection, out int dataOffset)
		{
			dataOffset = 10;
			int num = ((connection.DidReceiveData == 0) ? 8 : 0);
			return dataOffset + num;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessReceiveDelegate))]
		public unsafe static void ProcessReceive(IntPtr stream, ref NetworkInterfaceEndPoint endpoint, int size, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData, ref ProcessPacketCommand command)
		{
			byte* ptr = (byte*)(void*)stream;
			UdpCHeader udpCHeader = *(UdpCHeader*)ptr;
			if (size < 10)
			{
				Debug.LogError("Received an invalid message header");
				command.Type = ProcessPacketCommandType.Drop;
				return;
			}
			UdpCProtocol type = (UdpCProtocol)udpCHeader.Type;
			command.Address = endpoint;
			command.SessionId = udpCHeader.SessionToken;
			if (type != UdpCProtocol.Data && (udpCHeader.Flags & UdpCHeader.HeaderFlags.HasPipeline) != 0)
			{
				Debug.LogError("Received an invalid non-data message with a pipeline");
				command.Type = ProcessPacketCommandType.Drop;
				return;
			}
			switch (type)
			{
			case UdpCProtocol.ConnectionAccept:
				if ((udpCHeader.Flags & UdpCHeader.HeaderFlags.HasConnectToken) == 0)
				{
					Debug.LogError("Received an invalid ConnectionAccept without a token");
					command.Type = ProcessPacketCommandType.Drop;
				}
				else if (size != 18)
				{
					Debug.LogError("Received an invalid ConnectionAccept with wrong length");
					command.Type = ProcessPacketCommandType.Drop;
				}
				else
				{
					command.Type = ProcessPacketCommandType.ConnectionAccept;
					command.As.ConnectionAccept.ConnectionToken = *(SessionIdToken*)(void*)(stream + 10);
				}
				break;
			case UdpCProtocol.ConnectionReject:
				command.Type = ProcessPacketCommandType.ConnectionReject;
				break;
			case UdpCProtocol.ConnectionRequest:
				command.Type = ProcessPacketCommandType.ConnectionRequest;
				break;
			case UdpCProtocol.Disconnect:
				command.Type = ProcessPacketCommandType.Disconnect;
				break;
			case UdpCProtocol.Ping:
				command.Type = ProcessPacketCommandType.Ping;
				break;
			case UdpCProtocol.Pong:
				command.Type = ProcessPacketCommandType.Pong;
				break;
			case UdpCProtocol.Data:
			{
				int num = size - 10;
				byte hasPipelineByte = (byte)(((udpCHeader.Flags & UdpCHeader.HeaderFlags.HasPipeline) != 0) ? 1 : 0);
				if ((udpCHeader.Flags & UdpCHeader.HeaderFlags.HasConnectToken) != 0)
				{
					num -= 8;
					command.Type = ProcessPacketCommandType.DataWithImplicitConnectionAccept;
					command.As.DataWithImplicitConnectionAccept.Offset = 10;
					command.As.DataWithImplicitConnectionAccept.Length = num;
					command.As.DataWithImplicitConnectionAccept.HasPipelineByte = hasPipelineByte;
					command.As.DataWithImplicitConnectionAccept.ConnectionToken = *(SessionIdToken*)(void*)(stream + 10 + num);
				}
				else
				{
					command.Type = ProcessPacketCommandType.Data;
					command.As.Data.Offset = 10;
					command.As.Data.Length = num;
					command.As.Data.HasPipelineByte = hasPipelineByte;
				}
				break;
			}
			default:
				command.Type = ProcessPacketCommandType.Drop;
				break;
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendDelegate))]
		public static int ProcessSend(ref NetworkDriver.Connection connection, bool hasPipeline, ref NetworkSendInterface sendInterface, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			WriteSendMessageHeader(ref connection, hasPipeline, ref sendHandle, 0);
			return sendInterface.EndSendMessage.Ptr.Invoke(ref sendHandle, ref connection.Address, sendInterface.UserData, ref queueHandle);
		}

		internal unsafe static int WriteSendMessageHeader(ref NetworkDriver.Connection connection, bool hasPipeline, ref NetworkInterfaceSendHandle sendHandle, int offset)
		{
			UdpCHeader.HeaderFlags headerFlags = (UdpCHeader.HeaderFlags)0;
			if (connection.DidReceiveData == 0)
			{
				headerFlags |= UdpCHeader.HeaderFlags.HasConnectToken;
				SessionIdToken* ptr = (SessionIdToken*)((byte*)(void*)sendHandle.data + sendHandle.size);
				*ptr = connection.ReceiveToken;
				sendHandle.size += 8;
			}
			if (hasPipeline)
			{
				headerFlags |= UdpCHeader.HeaderFlags.HasPipeline;
			}
			UdpCHeader* ptr2 = (UdpCHeader*)(void*)(sendHandle.data + offset);
			*ptr2 = new UdpCHeader
			{
				Type = 4,
				SessionToken = connection.SendToken,
				Flags = headerFlags
			};
			return sendHandle.size - offset;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendConnectionAcceptDelegate))]
		public unsafe static void ProcessSendConnectionAccept(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 18) != 0)
			{
				Debug.LogError("Failed to send a ConnectionAccept packet");
				return;
			}
			byte* packet = (byte*)(void*)handle.data;
			int num = WriteConnectionAcceptMessage(ref connection, packet, handle.capacity);
			if (num < 0)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				Debug.LogError("Failed to send a ConnectionAccept packet");
				return;
			}
			handle.size = num;
			if (sendInterface.EndSendMessage.Ptr.Invoke(ref handle, ref connection.Address, sendInterface.UserData, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send a ConnectionAccept packet");
			}
		}

		internal static int GetConnectionAcceptMessageMaxLength()
		{
			return 18;
		}

		internal unsafe static int WriteConnectionAcceptMessage(ref NetworkDriver.Connection connection, byte* packet, int capacity)
		{
			int num = 10;
			if (connection.DidReceiveData == 0)
			{
				num += 8;
			}
			if (num > capacity)
			{
				Debug.LogError("Failed to create a ConnectionAccept packet: size exceeds capacity");
				return -1;
			}
			*(UdpCHeader*)packet = new UdpCHeader
			{
				Type = 2,
				SessionToken = connection.SendToken,
				Flags = (UdpCHeader.HeaderFlags)0
			};
			if (connection.DidReceiveData == 0)
			{
				((UdpCHeader*)packet)->Flags |= UdpCHeader.HeaderFlags.HasConnectToken;
				*(SessionIdToken*)(packet + 10) = connection.ReceiveToken;
			}
			return num;
		}

		private unsafe static int SendHeaderOnlyMessage(UdpCProtocol type, SessionIdToken token, ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 10) != 0)
			{
				return -1;
			}
			byte* ptr = (byte*)(void*)handle.data;
			handle.size = 10;
			if (handle.size > handle.capacity)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				return -1;
			}
			UdpCHeader* ptr2 = (UdpCHeader*)ptr;
			*ptr2 = new UdpCHeader
			{
				Type = (byte)type,
				SessionToken = token,
				Flags = (UdpCHeader.HeaderFlags)0
			};
			if (sendInterface.EndSendMessage.Ptr.Invoke(ref handle, ref connection.Address, sendInterface.UserData, ref queueHandle) < 0)
			{
				return -1;
			}
			return 10;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ConnectDelegate))]
		public static void Connect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SessionIdToken receiveToken = connection.ReceiveToken;
			if (SendHeaderOnlyMessage(UdpCProtocol.ConnectionRequest, receiveToken, ref connection, ref sendInterface, ref queueHandle) == -1)
			{
				Debug.LogError("Failed to send ConnectionRequest message");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.DisconnectDelegate))]
		public static void Disconnect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyMessage(UdpCProtocol.Disconnect, sendToken, ref connection, ref sendInterface, ref queueHandle) == -1)
			{
				Debug.LogError("Failed to send Disconnect message");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPingDelegate))]
		public static void ProcessSendPing(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyMessage(UdpCProtocol.Ping, sendToken, ref connection, ref sendInterface, ref queueHandle) == -1)
			{
				Debug.LogError("Failed to send Ping message");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPongDelegate))]
		public static void ProcessSendPong(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyMessage(UdpCProtocol.Pong, sendToken, ref connection, ref sendInterface, ref queueHandle) == -1)
			{
				Debug.LogError("Failed to send Pong message");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.UpdateDelegate))]
		public static void Update(long updateTime, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
		}
	}
}
