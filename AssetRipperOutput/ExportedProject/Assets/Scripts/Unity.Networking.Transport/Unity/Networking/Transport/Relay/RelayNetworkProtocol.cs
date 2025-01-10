using System;
using System.Threading;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Protocols;
using Unity.Networking.Transport.TLS;
using Unity.TLS.LowLevel;
using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	[BurstCompile]
	internal struct RelayNetworkProtocol : INetworkProtocol, IDisposable
	{
		private enum RelayConnectionState : byte
		{
			Unbound = 0,
			Handshake = 1,
			Binding = 2,
			Bound = 3,
			Connected = 4
		}

		private enum SecuredRelayConnectionState : byte
		{
			Unsecure = 0,
			Secured = 1
		}

		private struct RelayProtocolData
		{
			public RelayConnectionState ConnectionState;

			public SecuredRelayConnectionState SecureState;

			public SessionIdToken ConnectionReceiveToken;

			public long LastConnectAttempt;

			public long LastUpdateTime;

			public long LastReceiveTime;

			public long LastSentTime;

			public int ConnectTimeoutMS;

			public int RelayConnectionTimeMS;

			public RelayAllocationId HostAllocationId;

			public NetworkInterfaceEndPoint ServerEndpoint;

			public RelayServerData ServerData;

			public SecureClientState SecureClientState;

			public bool ConnectOnBind;
		}

		public IntPtr UserData;

		public static ushort SwitchEndianness(ushort value)
		{
			if (DataStreamWriter.IsLittleEndian)
			{
				return (ushort)((value << 8) | (value >> 8));
			}
			return value;
		}

		public unsafe void Initialize(NetworkSettings settings)
		{
			RelayNetworkParameter relayParameters = settings.GetRelayParameters();
			NetworkConfigParameter networkConfigParameters = settings.GetNetworkConfigParameters();
			if (relayParameters.ServerData.IsSecure == 1)
			{
				ManagedSecureFunctions.Initialize();
			}
			UserData = (IntPtr)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<RelayProtocolData>(), UnsafeUtility.AlignOf<RelayProtocolData>(), Allocator.Persistent);
			*(RelayProtocolData*)(void*)UserData = new RelayProtocolData
			{
				ServerData = relayParameters.ServerData,
				ConnectionState = RelayConnectionState.Unbound,
				ConnectTimeoutMS = networkConfigParameters.connectTimeoutMS,
				RelayConnectionTimeMS = relayParameters.RelayConnectionTimeMS,
				SecureState = SecuredRelayConnectionState.Unsecure
			};
		}

		public unsafe void Dispose()
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)UserData;
			if (ptr->SecureClientState.ClientPtr != null)
			{
				SecureNetworkProtocol.DisposeSecureClient(ref ptr->SecureClientState);
			}
			if (UserData != (IntPtr)0)
			{
				UnsafeUtility.Free(UserData.ToPointer(), Allocator.Persistent);
			}
			UserData = default(IntPtr);
		}

		private bool TryExtractParameters<T>(out T config, params INetworkParameter[] param)
		{
			for (int i = 0; i < param.Length; i++)
			{
				if (param[i] is T)
				{
					config = (T)param[i];
					return true;
				}
			}
			config = default(T);
			return false;
		}

		public unsafe int Bind(INetworkInterface networkInterface, ref NetworkInterfaceEndPoint localEndPoint)
		{
			if (networkInterface.Bind(localEndPoint) != 0)
			{
				return -1;
			}
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)UserData;
			networkInterface.CreateInterfaceEndPoint(ptr->ServerData.Endpoint, out ptr->ServerEndpoint);
			if (ptr->ServerData.IsSecure == 1)
			{
				ptr->ConnectionState = RelayConnectionState.Handshake;
			}
			else
			{
				ptr->ConnectionState = RelayConnectionState.Binding;
			}
			return 0;
		}

		public unsafe int CreateConnectionAddress(INetworkInterface networkInterface, NetworkEndPoint endPoint, out NetworkInterfaceEndPoint address)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)UserData;
			address = default(NetworkInterfaceEndPoint);
			fixed (byte* ptr2 = address.data)
			{
				*(RelayAllocationId*)ptr2 = ptr->HostAllocationId;
			}
			return 0;
		}

		public unsafe NetworkEndPoint GetRemoteEndPoint(INetworkInterface networkInterface, NetworkInterfaceEndPoint address)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)UserData;
			return networkInterface.GetGenericEndPoint(ptr->ServerEndpoint);
		}

		public NetworkProtocol CreateProtocolInterface()
		{
			return new NetworkProtocol(new TransportFunctionPointer<NetworkProtocol.ComputePacketOverheadDelegate>(ComputePacketOverhead), new TransportFunctionPointer<NetworkProtocol.ProcessReceiveDelegate>(ProcessReceive), new TransportFunctionPointer<NetworkProtocol.ProcessSendDelegate>(ProcessSend), new TransportFunctionPointer<NetworkProtocol.ProcessSendConnectionAcceptDelegate>(ProcessSendConnectionAccept), new TransportFunctionPointer<NetworkProtocol.ConnectDelegate>(Connect), new TransportFunctionPointer<NetworkProtocol.DisconnectDelegate>(Disconnect), new TransportFunctionPointer<NetworkProtocol.ProcessSendPingDelegate>(ProcessSendPing), new TransportFunctionPointer<NetworkProtocol.ProcessSendPongDelegate>(ProcessSendPong), new TransportFunctionPointer<NetworkProtocol.UpdateDelegate>(Update), needsUpdate: true, UserData, 48, 8);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ComputePacketOverheadDelegate))]
		public static int ComputePacketOverhead(ref NetworkDriver.Connection connection, out int dataOffset)
		{
			int num = UnityTransportProtocol.ComputePacketOverhead(ref connection, out dataOffset);
			dataOffset += 38;
			return num + 38;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessReceiveDelegate))]
		public unsafe static void ProcessReceive(IntPtr stream, ref NetworkInterfaceEndPoint endpoint, int size, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData, ref ProcessPacketCommand command)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			if (endpoint != ptr->ServerEndpoint)
			{
				command.Type = ProcessPacketCommandType.Drop;
			}
			else if (ptr->ConnectionState == RelayConnectionState.Handshake)
			{
				SecureUserData* ptr2 = (SecureUserData*)(void*)ptr->SecureClientState.ClientConfig->transportUserData;
				SecureNetworkProtocol.SetSecureUserData(stream, size, ref endpoint, ref sendInterface, ref queueHandle, ptr2);
				uint num = Binding.unitytls_client_get_state(ptr->SecureClientState.ClientPtr);
				if (num == 2 || num == 1)
				{
					bool flag = false;
					do
					{
						SecureNetworkProtocol.SecureHandshakeStep(ref ptr->SecureClientState);
						num = Binding.unitytls_client_get_state(ptr->SecureClientState.ClientPtr);
					}
					while (size != 0 && ptr2->BytesProcessed == 0 && num == 2);
				}
				if (num == 3)
				{
					ptr->ConnectionState = RelayConnectionState.Binding;
					ptr->SecureState = SecuredRelayConnectionState.Secured;
				}
				command.Type = ProcessPacketCommandType.Drop;
			}
			else if (ptr->ServerData.IsSecure == 1 && ptr->SecureState != SecuredRelayConnectionState.Secured)
			{
				command.Type = ProcessPacketCommandType.Drop;
			}
			else if (ptr->ServerData.IsSecure == 1 && ptr->SecureState == SecuredRelayConnectionState.Secured)
			{
				SecureUserData* secureUserData = (SecureUserData*)(void*)ptr->SecureClientState.ClientConfig->transportUserData;
				SecureNetworkProtocol.SetSecureUserData(stream, size, ref endpoint, ref sendInterface, ref queueHandle, secureUserData);
				NativeArray<byte> nativeArray = new NativeArray<byte>(1472, Allocator.Temp);
				UIntPtr uIntPtr = default(UIntPtr);
				if (Binding.unitytls_client_read_data(ptr->SecureClientState.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr(1472u), &uIntPtr) == 0)
				{
					UnsafeUtility.MemCpy((void*)stream, nativeArray.GetUnsafePtr(), uIntPtr.ToUInt32());
					if (ProcessRelayData(stream, ref endpoint, (int)uIntPtr.ToUInt32(), ref sendInterface, ref queueHandle, ref command, ptr))
					{
						return;
					}
				}
				command.Type = ProcessPacketCommandType.Drop;
			}
			else if (!ProcessRelayData(stream, ref endpoint, size, ref sendInterface, ref queueHandle, ref command, ptr))
			{
				command.Type = ProcessPacketCommandType.Drop;
			}
		}

		private unsafe static bool ProcessRelayData(IntPtr stream, ref NetworkInterfaceEndPoint endpoint, int size, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, ref ProcessPacketCommand command, RelayProtocolData* protocolData)
		{
			byte* ptr = (byte*)(void*)stream;
			RelayMessageHeader relayMessageHeader = *(RelayMessageHeader*)ptr;
			if (size < 4 || !relayMessageHeader.IsValid())
			{
				command.Type = ProcessPacketCommandType.Drop;
				return true;
			}
			if (protocolData->ServerData.IsSecure == 1 && protocolData->SecureState == SecuredRelayConnectionState.Secured)
			{
				SecureUserData* secureUserData = (SecureUserData*)(void*)protocolData->SecureClientState.ClientConfig->transportUserData;
				SecureNetworkProtocol.SetSecureUserData(stream, size, ref endpoint, ref sendInterface, ref queueHandle, secureUserData);
			}
			protocolData->LastReceiveTime = protocolData->LastUpdateTime;
			switch (relayMessageHeader.Type)
			{
			case RelayMessageType.BindReceived:
				command.Type = ProcessPacketCommandType.Drop;
				if (size != 4)
				{
					Debug.LogError("Received an invalid Relay Bind Received message: Wrong length");
					return true;
				}
				protocolData->ConnectionState = RelayConnectionState.Bound;
				if (protocolData->ConnectOnBind)
				{
					SendConnectionRequestToRelay(protocolData, ref sendInterface, ref queueHandle);
				}
				command.Type = ProcessPacketCommandType.ProtocolStatusUpdate;
				command.As.ProtocolStatusUpdate.Status = 1;
				return true;
			case RelayMessageType.Accepted:
			{
				command.Type = ProcessPacketCommandType.Drop;
				if (size != 36)
				{
					Debug.LogError("Received an invalid Relay Accepted message: Wrong length");
					return true;
				}
				if (protocolData->HostAllocationId != default(RelayAllocationId))
				{
					return true;
				}
				RelayMessageAccepted relayMessageAccepted = *(RelayMessageAccepted*)ptr;
				protocolData->HostAllocationId = relayMessageAccepted.FromAllocationId;
				command.Type = ProcessPacketCommandType.AddressUpdate;
				command.Address = default(NetworkInterfaceEndPoint);
				command.SessionId = protocolData->ConnectionReceiveToken;
				command.As.AddressUpdate.NewAddress = default(NetworkInterfaceEndPoint);
				fixed (byte* ptr2 = command.As.AddressUpdate.NewAddress.data)
				{
					*(RelayAllocationId*)ptr2 = relayMessageAccepted.FromAllocationId;
				}
				SessionIdToken connectionReceiveToken = protocolData->ConnectionReceiveToken;
				if (SendHeaderOnlyHostMessage(UdpCProtocol.ConnectionRequest, connectionReceiveToken, protocolData, ref relayMessageAccepted.FromAllocationId, ref sendInterface, ref queueHandle) < 0)
				{
					Debug.LogError("Failed to send Connection Request message to host.");
					return false;
				}
				return true;
			}
			case RelayMessageType.Relay:
			{
				RelayMessageRelay relayMessageRelay = *(RelayMessageRelay*)ptr;
				relayMessageRelay.DataLength = SwitchEndianness(relayMessageRelay.DataLength);
				if (size < 38 || size != 38 + relayMessageRelay.DataLength)
				{
					Debug.LogError("Received an invalid Relay Received message: Wrong length");
					command.Type = ProcessPacketCommandType.Drop;
					return true;
				}
				UnityTransportProtocol.ProcessReceive(stream + 38, ref endpoint, size - 38, ref sendInterface, ref queueHandle, IntPtr.Zero, ref command);
				switch (command.Type)
				{
				case ProcessPacketCommandType.ConnectionAccept:
					protocolData->ConnectionState = RelayConnectionState.Connected;
					break;
				case ProcessPacketCommandType.Data:
					command.As.Data.Offset += 38;
					break;
				case ProcessPacketCommandType.DataWithImplicitConnectionAccept:
					command.As.DataWithImplicitConnectionAccept.Offset += 38;
					break;
				case ProcessPacketCommandType.Disconnect:
					SendRelayDisconnect(protocolData, ref relayMessageRelay.FromAllocationId, ref sendInterface, ref queueHandle);
					break;
				}
				command.Address = default(NetworkInterfaceEndPoint);
				fixed (byte* ptr2 = command.Address.data)
				{
					*(RelayAllocationId*)ptr2 = relayMessageRelay.FromAllocationId;
				}
				return true;
			}
			case RelayMessageType.Error:
				command.Type = ProcessPacketCommandType.Drop;
				ProcessRelayError(ptr, size, ref command);
				return true;
			default:
				command.Type = ProcessPacketCommandType.Drop;
				return true;
			}
		}

		private unsafe static void ProcessRelayError(byte* data, int size, ref ProcessPacketCommand command)
		{
			if (size != 21)
			{
				Debug.LogError("Received an invalid Relay Error message (wrong length).");
				return;
			}
			RelayMessageError relayMessageError = *(RelayMessageError*)data;
			switch (relayMessageError.ErrorCode)
			{
			case 0:
				Debug.LogError("Received error message from Relay: invalid protocol version. Make sure your Unity Transport package is up to date.");
				break;
			case 1:
				Debug.LogError("Received error message from Relay: player timed out due to inactivity.");
				break;
			case 2:
				Debug.LogError("Received error message from Relay: unauthorized.");
				break;
			case 3:
				Debug.LogError("Received error message from Relay: allocation ID client mismatch.");
				break;
			case 4:
				Debug.LogError("Received error message from Relay: allocation ID not found.");
				break;
			case 5:
				Debug.LogError("Received error message from Relay: not connected.");
				break;
			case 6:
				Debug.LogError("Received error message from Relay: self-connect not allowed.");
				break;
			default:
				Debug.LogError($"Received error message from Relay with unknown error code {relayMessageError.ErrorCode}");
				break;
			}
			if (relayMessageError.ErrorCode == 1 || relayMessageError.ErrorCode == 4)
			{
				Debug.LogError("Relay allocation is invalid. See NetworkDriver.GetRelayConnectionStatus and RelayConnectionStatus.AllocationInvalid for details on how to handle this situation.");
				command.Type = ProcessPacketCommandType.ProtocolStatusUpdate;
				command.As.ProtocolStatusUpdate.Status = 2;
			}
		}

		private unsafe static int SendMessage(RelayProtocolData* protocolData, ref NetworkSendInterface sendInterface, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle)
		{
			if (protocolData->ServerData.IsSecure == 1 && protocolData->SecureState == SecuredRelayConnectionState.Secured)
			{
				SecureUserData* secureUserData = (SecureUserData*)(void*)protocolData->SecureClientState.ClientConfig->transportUserData;
				SecureNetworkProtocol.SetSecureUserData(IntPtr.Zero, 0, ref protocolData->ServerEndpoint, ref sendInterface, ref queueHandle, secureUserData);
				NativeArray<byte> nativeArray = new NativeArray<byte>(sendHandle.size, Allocator.Temp);
				UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), (void*)sendHandle.data, sendHandle.size);
				sendInterface.AbortSendMessage.Ptr.Invoke(ref sendHandle, sendInterface.UserData);
				uint num = Binding.unitytls_client_send_data(protocolData->SecureClientState.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr((uint)nativeArray.Length));
				if (num != 0)
				{
					Debug.LogError($"Secure send failed with result: {num}.");
					return -3;
				}
				return nativeArray.Length;
			}
			return sendInterface.EndSendMessage.Ptr.Invoke(ref sendHandle, ref protocolData->ServerEndpoint, sendInterface.UserData, ref queueHandle);
		}

		private unsafe static void SendRelayDisconnect(RelayProtocolData* protocolData, ref RelayAllocationId hostAllocationId, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 36) != 0)
			{
				Debug.LogError("Failed to send Disconnect message to relay.");
				return;
			}
			byte* ptr = (byte*)(void*)handle.data;
			handle.size = 36;
			if (handle.size > handle.capacity)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				Debug.LogError("Failed to send Disconnect message to relay.");
				return;
			}
			RelayMessageDisconnect* ptr2 = (RelayMessageDisconnect*)ptr;
			*ptr2 = RelayMessageDisconnect.Create(protocolData->ServerData.AllocationId, hostAllocationId);
			if (SendMessage(protocolData, ref sendInterface, ref handle, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Disconnect message to relay.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendDelegate))]
		public unsafe static int ProcessSend(ref NetworkDriver.Connection connection, bool hasPipeline, ref NetworkSendInterface sendInterface, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			ushort dataLength = (ushort)UnityTransportProtocol.WriteSendMessageHeader(ref connection, hasPipeline, ref sendHandle, 38);
			RelayMessageRelay* ptr2 = (RelayMessageRelay*)(void*)sendHandle.data;
			fixed (byte* ptr3 = connection.Address.data)
			{
				*ptr2 = RelayMessageRelay.Create(ptr->ServerData.AllocationId, *(RelayAllocationId*)ptr3, dataLength);
			}
			Interlocked.Exchange(ref ptr->LastSentTime, ptr->LastUpdateTime);
			return SendMessage(ptr, ref sendInterface, ref sendHandle, ref queueHandle);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendConnectionAcceptDelegate))]
		public unsafe static void ProcessSendConnectionAccept(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			RelayAllocationId relayAllocationId = default(RelayAllocationId);
			fixed (byte* ptr2 = connection.Address.data)
			{
				relayAllocationId = *(RelayAllocationId*)ptr2;
			}
			int num = 38 + UnityTransportProtocol.GetConnectionAcceptMessageMaxLength();
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, num) != 0)
			{
				Debug.LogError("Failed to send a ConnectionRequest packet");
				return;
			}
			if (handle.capacity < num)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				Debug.LogError("Failed to send a ConnectionAccept packet: size exceeds capacity");
				return;
			}
			byte* ptr3 = (byte*)(void*)handle.data;
			int num2 = UnityTransportProtocol.WriteConnectionAcceptMessage(ref connection, ptr3 + 38, handle.capacity - 38);
			if (num2 < 0)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				Debug.LogError("Failed to send a ConnectionAccept packet");
				return;
			}
			handle.size = 38 + num2;
			RelayMessageRelay* ptr4 = (RelayMessageRelay*)ptr3;
			*ptr4 = RelayMessageRelay.Create(ptr->ServerData.AllocationId, relayAllocationId, (ushort)num2);
			if (SendMessage(ptr, ref sendInterface, ref handle, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Connection Accept message to host.");
			}
		}

		private unsafe static int SendHeaderOnlyHostMessage(UdpCProtocol type, SessionIdToken token, RelayProtocolData* relayProtocolData, ref RelayAllocationId hostAllocationId, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 48) != 0)
			{
				return -1;
			}
			byte* ptr = (byte*)(void*)handle.data;
			handle.size = 48;
			if (handle.size > handle.capacity)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				return -1;
			}
			RelayMessageRelay* ptr2 = (RelayMessageRelay*)ptr;
			*ptr2 = RelayMessageRelay.Create(relayProtocolData->ServerData.AllocationId, hostAllocationId, 10);
			UdpCHeader* ptr3 = (UdpCHeader*)((byte*)ptr2 + 38);
			*ptr3 = new UdpCHeader
			{
				Type = (byte)type,
				SessionToken = token,
				Flags = (UdpCHeader.HeaderFlags)0
			};
			return SendMessage(relayProtocolData, ref sendInterface, ref handle, ref queueHandle);
		}

		private unsafe static void SendConnectionRequestToRelay(RelayProtocolData* relayProtocolData, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 276) != 0)
			{
				Debug.LogError("Failed to send ConnectRequest to relay.");
				return;
			}
			byte* ptr = (byte*)(void*)handle.data;
			handle.size = 276;
			if (handle.size > handle.capacity)
			{
				sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
				Debug.LogError("Failed to send ConnectRequest to relay.");
				return;
			}
			RelayMessageConnectRequest* ptr2 = (RelayMessageConnectRequest*)ptr;
			*ptr2 = RelayMessageConnectRequest.Create(relayProtocolData->ServerData.AllocationId, relayProtocolData->ServerData.HostConnectionData);
			if (SendMessage(relayProtocolData, ref sendInterface, ref handle, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send ConnectRequest to relay.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ConnectDelegate))]
		public unsafe static void Connect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			ptr->ConnectionReceiveToken = connection.ReceiveToken;
			if (ptr->ConnectionState != RelayConnectionState.Bound)
			{
				ptr->ConnectOnBind = true;
				return;
			}
			if (ptr->HostAllocationId == default(RelayAllocationId))
			{
				SendConnectionRequestToRelay(ptr, ref sendInterface, ref queueHandle);
				return;
			}
			SessionIdToken connectionReceiveToken = ptr->ConnectionReceiveToken;
			if (SendHeaderOnlyHostMessage(UdpCProtocol.ConnectionRequest, connectionReceiveToken, ptr, ref ptr->HostAllocationId, ref sendInterface, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Connection Request message to host.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.DisconnectDelegate))]
		public unsafe static void Disconnect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* relayProtocolData = (RelayProtocolData*)(void*)userData;
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyHostMessage(UdpCProtocol.Disconnect, sendToken, relayProtocolData, ref connection.Address.AsRelayAllocationId(), ref sendInterface, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Disconnect message to host.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPingDelegate))]
		public unsafe static void ProcessSendPing(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* relayProtocolData = (RelayProtocolData*)(void*)userData;
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyHostMessage(UdpCProtocol.Ping, sendToken, relayProtocolData, ref connection.Address.AsRelayAllocationId(), ref sendInterface, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Ping message to host.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPingDelegate))]
		public unsafe static void ProcessSendPong(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* relayProtocolData = (RelayProtocolData*)(void*)userData;
			SessionIdToken sendToken = connection.SendToken;
			if (SendHeaderOnlyHostMessage(UdpCProtocol.Pong, sendToken, relayProtocolData, ref connection.Address.AsRelayAllocationId(), ref sendInterface, ref queueHandle) < 0)
			{
				Debug.LogError("Failed to send Pong message to host.");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.UpdateDelegate))]
		public unsafe static void Update(long updateTime, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			switch (ptr->ConnectionState)
			{
			case RelayConnectionState.Handshake:
			{
				if (ptr->SecureClientState.ClientPtr == null)
				{
					Binding.unitytls_client_config* ptr2 = (Binding.unitytls_client_config*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Binding.unitytls_client_config>(), UnsafeUtility.AlignOf<Binding.unitytls_client_config>(), Allocator.Persistent);
					*ptr2 = default(Binding.unitytls_client_config);
					Binding.unitytls_client_init_config(ptr2);
					ptr2->dataSendCB = ManagedSecureFunctions.s_SendCallback.Data.Value;
					ptr2->dataReceiveCB = ManagedSecureFunctions.s_RecvMethod.Data.Value;
					ptr2->clientAuth = 2u;
					ptr2->transportProtocol = 1u;
					ptr2->clientAuth = 1u;
					ptr2->ssl_read_timeout_ms = SecureNetworkProtocol.DefaultParameters.SSLReadTimeoutMs;
					ptr2->ssl_handshake_timeout_min = SecureNetworkProtocol.DefaultParameters.SSLHandshakeTimeoutMin;
					ptr2->ssl_handshake_timeout_max = SecureNetworkProtocol.DefaultParameters.SSLHandshakeTimeoutMax;
					ptr2->hostname = ((FixedString32Bytes)"relay").GetUnsafePtr();
					ptr2->psk = new Binding.unitytls_dataRef
					{
						dataPtr = ptr->ServerData.HMACKey.Value,
						dataLen = new UIntPtr(64u)
					};
					ptr2->pskIdentity = new Binding.unitytls_dataRef
					{
						dataPtr = ptr->ServerData.AllocationId.Value,
						dataLen = new UIntPtr(16u)
					};
					ptr->SecureClientState.ClientConfig = ptr2;
					ptr->SecureClientState.ClientPtr = Binding.unitytls_client_create(2u, ptr->SecureClientState.ClientConfig);
					IntPtr intPtr = (IntPtr)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<SecureUserData>(), UnsafeUtility.AlignOf<SecureUserData>(), Allocator.Persistent);
					*(SecureUserData*)(void*)intPtr = new SecureUserData
					{
						Interface = default(NetworkSendInterface),
						Remote = default(NetworkInterfaceEndPoint),
						QueueHandle = default(NetworkSendQueueHandle),
						StreamData = IntPtr.Zero,
						Size = 0,
						BytesProcessed = 0
					};
					ptr->SecureClientState.ClientConfig->transportUserData = intPtr;
					Binding.unitytls_client_init(ptr->SecureClientState.ClientPtr);
				}
				if (Binding.unitytls_client_get_state(ptr->SecureClientState.ClientPtr) == 2)
				{
					return;
				}
				SecureUserData* secureUserData = (SecureUserData*)(void*)ptr->SecureClientState.ClientConfig->transportUserData;
				SecureNetworkProtocol.SetSecureUserData(IntPtr.Zero, 0, ref ptr->ServerEndpoint, ref sendInterface, ref queueHandle, secureUserData);
				SecureNetworkProtocol.SecureHandshakeStep(ref ptr->SecureClientState);
				break;
			}
			case RelayConnectionState.Binding:
				if (updateTime - ptr->LastConnectAttempt > ptr->ConnectTimeoutMS || ptr->LastUpdateTime == 0L)
				{
					ptr->LastConnectAttempt = updateTime;
					ptr->LastSentTime = updateTime;
					if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle3, sendInterface.UserData, 295) != 0)
					{
						Debug.LogError("Failed to send Bind message to relay.");
						return;
					}
					if (!WriteBindMessage(ref ptr->ServerEndpoint, ref handle3, ref queueHandle, userData))
					{
						sendInterface.AbortSendMessage.Ptr.Invoke(ref handle3, sendInterface.UserData);
						return;
					}
					if (SendMessage(ptr, ref sendInterface, ref handle3, ref queueHandle) < 0)
					{
						Debug.LogError("Failed to send Bind message to relay.");
						return;
					}
				}
				break;
			case RelayConnectionState.Bound:
			case RelayConnectionState.Connected:
			{
				if (updateTime - ptr->LastSentTime >= ptr->RelayConnectionTimeMS)
				{
					if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle, sendInterface.UserData, 22) != 0)
					{
						Debug.LogError("Failed to send a RelayPingMessage packet");
						return;
					}
					if (!WriteRelayPingMessage(ref ptr->ServerEndpoint, ref handle, ref queueHandle, userData))
					{
						sendInterface.AbortSendMessage.Ptr.Invoke(ref handle, sendInterface.UserData);
						return;
					}
					if (SendMessage(ptr, ref sendInterface, ref handle, ref queueHandle) < 0)
					{
						Debug.LogError("Failed to send Ping message to relay.");
						return;
					}
					ptr->LastSentTime = updateTime;
				}
				int num = ptr->RelayConnectionTimeMS * 3;
				if (ptr->LastReceiveTime > 0 && updateTime - ptr->LastReceiveTime >= num)
				{
					if (sendInterface.BeginSendMessage.Ptr.Invoke(out var handle2, sendInterface.UserData, 22) != 0)
					{
						Debug.LogError("Failed to send Bind message to relay.");
						return;
					}
					if (!WriteBindMessage(ref ptr->ServerEndpoint, ref handle2, ref queueHandle, userData))
					{
						sendInterface.AbortSendMessage.Ptr.Invoke(ref handle2, sendInterface.UserData);
						return;
					}
					if (SendMessage(ptr, ref sendInterface, ref handle2, ref queueHandle) < 0)
					{
						Debug.LogError("Failed to send Bind message to relay.");
						return;
					}
					ptr->LastReceiveTime = updateTime;
				}
				break;
			}
			}
			ptr->LastUpdateTime = updateTime;
		}

		[BurstCompatible]
		private unsafe static bool WriteRelayPingMessage(ref NetworkInterfaceEndPoint serverEndpoint, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			byte* ptr2 = (byte*)(void*)sendHandle.data;
			sendHandle.size = 22;
			if (sendHandle.size > sendHandle.capacity)
			{
				Debug.LogError("Failed to send a RelayPingMessage packet");
				return false;
			}
			RelayMessagePing* ptr3 = (RelayMessagePing*)ptr2;
			*ptr3 = RelayMessagePing.Create(ptr->ServerData.AllocationId, 0);
			return true;
		}

		[BurstCompatible]
		private unsafe static bool WriteBindMessage(ref NetworkInterfaceEndPoint serverEndpoint, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			DataStreamWriter writer = WriterForSendBuffer(295, ref sendHandle);
			if (!writer.IsCreated)
			{
				Debug.LogError("Failed to send a RelayBindMessage packet");
				return false;
			}
			RelayProtocolData* ptr = (RelayProtocolData*)(void*)userData;
			RelayMessageBind.Write(writer, 0, ptr->ServerData.Nonce, ptr->ServerData.ConnectionData.Value, ptr->ServerData.HMAC);
			return true;
		}

		private unsafe static DataStreamWriter WriterForSendBuffer(int requestSize, ref NetworkInterfaceSendHandle sendHandle)
		{
			if (requestSize <= sendHandle.capacity)
			{
				sendHandle.size = requestSize;
				return new DataStreamWriter((byte*)(void*)sendHandle.data, sendHandle.size);
			}
			return default(DataStreamWriter);
		}
	}
}
