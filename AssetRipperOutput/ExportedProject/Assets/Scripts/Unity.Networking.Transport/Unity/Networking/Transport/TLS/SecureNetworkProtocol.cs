using System;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Protocols;
using Unity.TLS.LowLevel;
using UnityEngine;

namespace Unity.Networking.Transport.TLS
{
	[BurstCompile]
	internal struct SecureNetworkProtocol : INetworkProtocol, IDisposable
	{
		public IntPtr UserData;

		public static readonly SecureNetworkProtocolParameter DefaultParameters = new SecureNetworkProtocolParameter
		{
			Protocol = SecureTransportProtocol.DTLS,
			SSLReadTimeoutMs = 0u,
			SSLHandshakeTimeoutMin = 1000u,
			SSLHandshakeTimeoutMax = 60000u,
			ClientAuthenticationPolicy = SecureClientAuthPolicy.Optional
		};

		private unsafe static void CreateSecureClient(uint role, SecureClientState* state)
		{
			Binding.unitytls_client* clientPtr = Binding.unitytls_client_create(role, state->ClientConfig);
			state->ClientPtr = clientPtr;
		}

		private unsafe static Binding.unitytls_client_config* GetSecureClientConfig(SecureNetworkProtocolData* protocolData)
		{
			Binding.unitytls_client_config* ptr = (Binding.unitytls_client_config*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Binding.unitytls_client_config>(), UnsafeUtility.AlignOf<Binding.unitytls_client_config>(), Allocator.Persistent);
			*ptr = default(Binding.unitytls_client_config);
			Binding.unitytls_client_init_config(ptr);
			ptr->dataSendCB = ManagedSecureFunctions.s_SendCallback.Data.Value;
			ptr->dataReceiveCB = ManagedSecureFunctions.s_RecvMethod.Data.Value;
			ptr->logCallback = IntPtr.Zero;
			ptr->clientAuth = 0u;
			ptr->transportProtocol = protocolData->Protocol;
			ptr->clientAuth = protocolData->ClientAuth;
			ptr->ssl_read_timeout_ms = protocolData->SSLReadTimeoutMs;
			ptr->ssl_handshake_timeout_min = protocolData->SSLHandshakeTimeoutMin;
			ptr->ssl_handshake_timeout_max = protocolData->SSLHandshakeTimeoutMax;
			return ptr;
		}

		public unsafe void Initialize(NetworkSettings settings)
		{
			ManagedSecureFunctions.Initialize();
			SecureNetworkProtocolParameter secureParameters = settings.GetSecureParameters();
			UserData = (IntPtr)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<SecureNetworkProtocolData>(), UnsafeUtility.AlignOf<SecureNetworkProtocolData>(), Allocator.Persistent);
			*(SecureNetworkProtocolData*)(void*)UserData = new SecureNetworkProtocolData
			{
				SecureClients = new UnsafeHashMap<NetworkInterfaceEndPoint, SecureClientState>(1, Allocator.Persistent),
				Rsa = secureParameters.Rsa,
				RsaKey = secureParameters.RsaKey,
				Pem = secureParameters.Pem,
				Hostname = secureParameters.Hostname,
				Protocol = (uint)secureParameters.Protocol,
				SSLReadTimeoutMs = secureParameters.SSLReadTimeoutMs,
				SSLHandshakeTimeoutMin = secureParameters.SSLHandshakeTimeoutMin,
				SSLHandshakeTimeoutMax = secureParameters.SSLHandshakeTimeoutMax,
				ClientAuth = (uint)secureParameters.ClientAuthenticationPolicy
			};
		}

		public unsafe static void DisposeSecureClient(ref SecureClientState state)
		{
			if (state.ClientConfig->transportUserData.ToPointer() != null)
			{
				UnsafeUtility.Free(state.ClientConfig->transportUserData.ToPointer(), Allocator.Persistent);
			}
			if (state.ClientConfig != null)
			{
				UnsafeUtility.Free(state.ClientConfig, Allocator.Persistent);
			}
			state.ClientConfig = null;
			if (state.ClientPtr != null)
			{
				Binding.unitytls_client_destroy(state.ClientPtr);
			}
		}

		public unsafe void Dispose()
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)UserData;
			NativeArray<NetworkInterfaceEndPoint> keyArray = ptr->SecureClients.GetKeyArray(Allocator.Temp);
			for (int i = 0; i < keyArray.Length; i++)
			{
				SecureClientState state = ptr->SecureClients[keyArray[i]];
				DisposeSecureClient(ref state);
				ptr->SecureClients.Remove(keyArray[i]);
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

		public int Listen(INetworkInterface networkInterface)
		{
			return networkInterface.Listen();
		}

		public NetworkProtocol CreateProtocolInterface()
		{
			return new NetworkProtocol(new TransportFunctionPointer<NetworkProtocol.ComputePacketOverheadDelegate>(ComputePacketOverhead), new TransportFunctionPointer<NetworkProtocol.ProcessReceiveDelegate>(ProcessReceive), new TransportFunctionPointer<NetworkProtocol.ProcessSendDelegate>(ProcessSend), new TransportFunctionPointer<NetworkProtocol.ProcessSendConnectionAcceptDelegate>(ProcessSendConnectionAccept), new TransportFunctionPointer<NetworkProtocol.ConnectDelegate>(Connect), new TransportFunctionPointer<NetworkProtocol.DisconnectDelegate>(Disconnect), new TransportFunctionPointer<NetworkProtocol.ProcessSendPingDelegate>(ProcessSendPing), new TransportFunctionPointer<NetworkProtocol.ProcessSendPongDelegate>(ProcessSendPong), new TransportFunctionPointer<NetworkProtocol.UpdateDelegate>(Update), needsUpdate: true, UserData, 10, 8);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ComputePacketOverheadDelegate))]
		public static int ComputePacketOverhead(ref NetworkDriver.Connection connection, out int dataOffset)
		{
			return UnityTransportProtocol.ComputePacketOverhead(ref connection, out dataOffset);
		}

		public static bool ServerShouldStep(uint currentState)
		{
			if (currentState <= 6 || currentState - 12 <= 4)
			{
				return true;
			}
			return false;
		}

		private static bool ClientShouldStep(uint currentState)
		{
			switch (currentState)
			{
			case 0u:
			case 1u:
				return true;
			case 2u:
			case 3u:
			case 4u:
			case 5u:
				return false;
			case 6u:
			case 7u:
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 14u:
			case 15u:
			case 16u:
				return true;
			default:
				return false;
			}
		}

		internal unsafe static void SetSecureUserData(IntPtr inStream, int size, ref NetworkInterfaceEndPoint remote, ref NetworkSendInterface networkSendInterface, ref NetworkSendQueueHandle queueHandle, SecureUserData* secureUserData)
		{
			secureUserData->Interface = networkSendInterface;
			secureUserData->Remote = remote;
			secureUserData->QueueHandle = queueHandle;
			secureUserData->Size = size;
			secureUserData->StreamData = inStream;
			secureUserData->BytesProcessed = 0;
		}

		private unsafe static bool CreateNewSecureClientState(ref NetworkInterfaceEndPoint endpoint, uint tlsRole, SecureNetworkProtocolData* protocolData, SessionIdToken receiveToken = default(SessionIdToken))
		{
			SecureClientState value;
			if (protocolData->SecureClients.TryAdd(endpoint, default(SecureClientState)))
			{
				value = protocolData->SecureClients[endpoint];
				value.ClientConfig = GetSecureClientConfig(protocolData);
				value.ReceiveToken = receiveToken;
				CreateSecureClient(tlsRole, &value);
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
				value.ClientConfig->transportUserData = intPtr;
				FixedString32Bytes* hostname = &protocolData->Hostname;
				FixedString32Bytes b = default(FixedString32Bytes);
				if (*hostname != b)
				{
					value.ClientConfig->hostname = protocolData->Hostname.GetUnsafePtr();
				}
				else
				{
					value.ClientConfig->hostname = null;
				}
				FixedString4096Bytes* pem = &protocolData->Pem;
				b = default(FixedString32Bytes);
				if (*pem != b)
				{
					value.ClientConfig->caPEM = new Binding.unitytls_dataRef
					{
						dataPtr = protocolData->Pem.GetUnsafePtr(),
						dataLen = new UIntPtr((uint)protocolData->Pem.Length)
					};
				}
				else
				{
					value.ClientConfig->caPEM = new Binding.unitytls_dataRef
					{
						dataPtr = null,
						dataLen = new UIntPtr(0u)
					};
				}
				FixedString4096Bytes* rsa = &protocolData->Rsa;
				b = default(FixedString32Bytes);
				if (*rsa != b)
				{
					FixedString4096Bytes* rsaKey = &protocolData->RsaKey;
					FixedString32Bytes b2 = default(FixedString32Bytes);
					if (*rsaKey != b2)
					{
						value.ClientConfig->serverPEM = new Binding.unitytls_dataRef
						{
							dataPtr = protocolData->Rsa.GetUnsafePtr(),
							dataLen = new UIntPtr((uint)protocolData->Rsa.Length)
						};
						value.ClientConfig->privateKeyPEM = new Binding.unitytls_dataRef
						{
							dataPtr = protocolData->RsaKey.GetUnsafePtr(),
							dataLen = new UIntPtr((uint)protocolData->RsaKey.Length)
						};
						goto IL_0288;
					}
				}
				value.ClientConfig->serverPEM = new Binding.unitytls_dataRef
				{
					dataPtr = null,
					dataLen = new UIntPtr(0u)
				};
				value.ClientConfig->privateKeyPEM = new Binding.unitytls_dataRef
				{
					dataPtr = null,
					dataLen = new UIntPtr(0u)
				};
				goto IL_0288;
			}
			goto IL_02a6;
			IL_02a6:
			return false;
			IL_0288:
			Binding.unitytls_client_init(value.ClientPtr);
			protocolData->SecureClients[endpoint] = value;
			goto IL_02a6;
		}

		internal unsafe static uint SecureHandshakeStep(ref SecureClientState clientAgent)
		{
			bool flag = Binding.unitytls_client_get_role(clientAgent.ClientPtr) == 1;
			bool flag2 = true;
			uint num = 1048584u;
			do
			{
				flag2 = false;
				num = Binding.unitytls_client_handshake(clientAgent.ClientPtr);
				if (num == 1048584)
				{
					uint currentState = Binding.unitytls_client_get_handshake_state(clientAgent.ClientPtr);
					flag2 = (flag ? ServerShouldStep(currentState) : ClientShouldStep(currentState));
				}
			}
			while (flag2);
			return num;
		}

		private unsafe static uint UpdateSecureHandshakeState(SecureNetworkProtocolData* protocolData, ref NetworkInterfaceEndPoint endpoint)
		{
			SecureClientState clientAgent = protocolData->SecureClients[endpoint];
			clientAgent.LastHandshakeUpdate = protocolData->LastUpdate;
			protocolData->SecureClients[endpoint] = clientAgent;
			return SecureHandshakeStep(ref clientAgent);
		}

		private unsafe static void PruneHalfOpenConnections(SecureNetworkProtocolData* protocolData)
		{
			NativeArray<NetworkInterfaceEndPoint> keyArray = protocolData->SecureClients.GetKeyArray(Allocator.Temp);
			bool flag = false;
			for (int i = 0; i < keyArray.Length; i++)
			{
				SecureClientState state = protocolData->SecureClients[keyArray[i]];
				if (Binding.unitytls_client_get_state(state.ClientPtr) == 2 && state.LastHandshakeUpdate > 0 && protocolData->LastUpdate - state.LastHandshakeUpdate > protocolData->SSLHandshakeTimeoutMax)
				{
					DisposeSecureClient(ref state);
					protocolData->SecureClients.Remove(keyArray[i]);
					flag = true;
				}
			}
			if (flag)
			{
				Debug.LogError("Had to prune half-open connections (clients with unfinished TLS handshakes).");
			}
			keyArray.Dispose();
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessReceiveDelegate))]
		public unsafe static void ProcessReceive(IntPtr stream, ref NetworkInterfaceEndPoint endpoint, int size, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData, ref ProcessPacketCommand command)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			CreateNewSecureClientState(ref endpoint, 1u, ptr);
			SecureClientState state = ptr->SecureClients[endpoint];
			SecureUserData* ptr2 = (SecureUserData*)(void*)state.ClientConfig->transportUserData;
			SetSecureUserData(stream, size, ref endpoint, ref sendInterface, ref queueHandle, ptr2);
			uint num = Binding.unitytls_client_get_state(state.ClientPtr);
			uint num2 = 0u;
			switch (num)
			{
			case 1u:
			case 2u:
			{
				bool flag = false;
				do
				{
					num2 = UpdateSecureHandshakeState(ptr, ref endpoint);
					num = Binding.unitytls_client_get_state(state.ClientPtr);
				}
				while (size != 0 && ptr2->BytesProcessed == 0 && num == 2);
				if (Binding.unitytls_client_get_role(state.ClientPtr) == 2 && num == 3)
				{
					SendConnectionRequest(state.ReceiveToken, state, ref endpoint, ref sendInterface, ref queueHandle);
				}
				command.Type = ProcessPacketCommandType.Drop;
				break;
			}
			case 3u:
			{
				NativeArray<byte> nativeArray = new NativeArray<byte>(1472, Allocator.Temp);
				UIntPtr uIntPtr = default(UIntPtr);
				if (Binding.unitytls_client_read_data(state.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr(1472u), &uIntPtr) != 0)
				{
					command.Type = ProcessPacketCommandType.Drop;
					return;
				}
				UnsafeUtility.MemCpy((void*)stream, nativeArray.GetUnsafePtr(), uIntPtr.ToUInt32());
				UnityTransportProtocol.ProcessReceive(stream, ref endpoint, (int)uIntPtr.ToUInt32(), ref sendInterface, ref queueHandle, IntPtr.Zero, ref command);
				if (command.Type == ProcessPacketCommandType.Disconnect)
				{
					DisposeSecureClient(ref state);
					ptr->SecureClients.Remove(endpoint);
					return;
				}
				break;
			}
			}
			num = Binding.unitytls_client_get_state(state.ClientPtr);
			if (num == 64)
			{
				if (num2 == 13)
				{
					Debug.LogError("Secure handshake failure (likely caused by certificate validation failure).");
				}
				command.Type = ProcessPacketCommandType.Drop;
				DisposeSecureClient(ref state);
				ptr->SecureClients.Remove(endpoint);
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendDelegate))]
		public unsafe static int ProcessSend(ref NetworkDriver.Connection connection, bool hasPipeline, ref NetworkSendInterface sendInterface, ref NetworkInterfaceSendHandle sendHandle, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			CreateNewSecureClientState(ref connection.Address, 1u, ptr);
			SecureClientState secureClientState = ptr->SecureClients[connection.Address];
			SetSecureUserData(secureUserData: (SecureUserData*)(void*)secureClientState.ClientConfig->transportUserData, inStream: IntPtr.Zero, size: 0, remote: ref connection.Address, networkSendInterface: ref sendInterface, queueHandle: ref queueHandle);
			UnityTransportProtocol.WriteSendMessageHeader(ref connection, hasPipeline, ref sendHandle, 0);
			NativeArray<byte> nativeArray = new NativeArray<byte>(sendHandle.size, Allocator.Temp);
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), (void*)sendHandle.data, sendHandle.size);
			sendInterface.AbortSendMessage.Ptr.Invoke(ref sendHandle, sendInterface.UserData);
			uint num = Binding.unitytls_client_send_data(secureClientState.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr((uint)nativeArray.Length));
			if (num != 0)
			{
				Debug.LogError($"Secure Send failed with result {num}");
				return -3;
			}
			return nativeArray.Length;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendConnectionAcceptDelegate))]
		public unsafe static void ProcessSendConnectionAccept(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			SecureClientState secureClientState = ptr->SecureClients[connection.Address];
			NativeArray<byte> nativeArray = new NativeArray<byte>(18, Allocator.Temp);
			if (WriteConnectionAcceptMessage(ref connection, (byte*)nativeArray.GetUnsafePtr(), nativeArray.Length) < 0)
			{
				Debug.LogError("Failed to send a ConnectionAccept packet");
				return;
			}
			SecureUserData* secureUserData = (SecureUserData*)(void*)secureClientState.ClientConfig->transportUserData;
			SetSecureUserData(IntPtr.Zero, 0, ref connection.Address, ref sendInterface, ref queueHandle, secureUserData);
			uint num = Binding.unitytls_client_send_data(secureClientState.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr((uint)nativeArray.Length));
			if (num != 0)
			{
				Debug.LogError($"Secure Send failed with result {num}");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
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
			((UdpCHeader*)packet)->Type = 2;
			((UdpCHeader*)packet)->SessionToken = connection.SendToken;
			((UdpCHeader*)packet)->Flags = (UdpCHeader.HeaderFlags)0;
			if (connection.DidReceiveData == 0)
			{
				((UdpCHeader*)packet)->Flags |= UdpCHeader.HeaderFlags.HasConnectToken;
				*(SessionIdToken*)(packet + 10) = connection.ReceiveToken;
			}
			return num;
		}

		private unsafe static void SendConnectionRequest(SessionIdToken token, SecureClientState secureClient, ref NetworkInterfaceEndPoint address, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			NativeArray<byte> nativeArray = new NativeArray<byte>(10, Allocator.Temp);
			UdpCHeader* unsafePtr = (UdpCHeader*)nativeArray.GetUnsafePtr();
			unsafePtr->Type = 0;
			unsafePtr->SessionToken = token;
			unsafePtr->Flags = (UdpCHeader.HeaderFlags)0;
			SecureUserData* secureUserData = (SecureUserData*)(void*)secureClient.ClientConfig->transportUserData;
			SetSecureUserData(IntPtr.Zero, 0, ref address, ref sendInterface, ref queueHandle, secureUserData);
			if (Binding.unitytls_client_send_data(secureClient.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr((uint)nativeArray.Length)) != 0)
			{
				Debug.LogError("We have failed to Send Encrypted SendConnectionRequest");
			}
		}

		private unsafe static uint SendHeaderOnlyMessage(UdpCProtocol type, SessionIdToken token, SecureClientState secureClient, ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle)
		{
			NativeArray<byte> nativeArray = new NativeArray<byte>(10, Allocator.Temp);
			UdpCHeader* unsafePtr = (UdpCHeader*)nativeArray.GetUnsafePtr();
			unsafePtr->Type = (byte)type;
			unsafePtr->SessionToken = token;
			unsafePtr->Flags = (UdpCHeader.HeaderFlags)0;
			SecureUserData* secureUserData = (SecureUserData*)(void*)secureClient.ClientConfig->transportUserData;
			SetSecureUserData(IntPtr.Zero, 0, ref connection.Address, ref sendInterface, ref queueHandle, secureUserData);
			return Binding.unitytls_client_send_data(secureClient.ClientPtr, (byte*)nativeArray.GetUnsafePtr(), new UIntPtr((uint)nativeArray.Length));
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ConnectDelegate))]
		public unsafe static void Connect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			CreateNewSecureClientState(ref connection.Address, 2u, ptr, connection.ReceiveToken);
			SecureClientState state = ptr->SecureClients[connection.Address];
			SecureUserData* secureUserData = (SecureUserData*)(void*)state.ClientConfig->transportUserData;
			SetSecureUserData(IntPtr.Zero, 0, ref connection.Address, ref sendInterface, ref queueHandle, secureUserData);
			if (Binding.unitytls_client_get_state(state.ClientPtr) == 3)
			{
				SendConnectionRequest(connection.ReceiveToken, state, ref connection.Address, ref sendInterface, ref queueHandle);
				return;
			}
			uint num = UpdateSecureHandshakeState(ptr, ref connection.Address);
			if (Binding.unitytls_client_get_state(state.ClientPtr) == 64)
			{
				Debug.LogError($"Handshake failed with result {num}");
				DisposeSecureClient(ref state);
				ptr->SecureClients.Remove(connection.Address);
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.DisconnectDelegate))]
		public unsafe static void Disconnect(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			SecureClientState state = ptr->SecureClients[connection.Address];
			if (connection.State == NetworkConnection.State.Connected)
			{
				SessionIdToken sendToken = connection.SendToken;
				uint num = SendHeaderOnlyMessage(UdpCProtocol.Disconnect, sendToken, state, ref connection, ref sendInterface, ref queueHandle);
				if (num != 0)
				{
					Debug.LogError($"Failed to send secure Disconnect message (result: {num})");
				}
			}
			DisposeSecureClient(ref state);
			ptr->SecureClients.Remove(connection.Address);
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPingDelegate))]
		public unsafe static void ProcessSendPing(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			SecureClientState secureClient = ptr->SecureClients[connection.Address];
			SessionIdToken sendToken = connection.SendToken;
			uint num = SendHeaderOnlyMessage(UdpCProtocol.Ping, sendToken, secureClient, ref connection, ref sendInterface, ref queueHandle);
			if (num != 0)
			{
				Debug.LogError($"Failed to send secure Ping message (result: {num})");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.ProcessSendPongDelegate))]
		public unsafe static void ProcessSendPong(ref NetworkDriver.Connection connection, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			SecureClientState secureClient = ptr->SecureClients[connection.Address];
			SessionIdToken sendToken = connection.SendToken;
			uint num = SendHeaderOnlyMessage(UdpCProtocol.Pong, sendToken, secureClient, ref connection, ref sendInterface, ref queueHandle);
			if (num != 0)
			{
				Debug.LogError($"Failed to send secure Pong message (result: {num})");
			}
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkProtocol.UpdateDelegate))]
		public unsafe static void Update(long updateTime, ref NetworkSendInterface sendInterface, ref NetworkSendQueueHandle queueHandle, IntPtr userData)
		{
			SecureNetworkProtocolData* ptr = (SecureNetworkProtocolData*)(void*)userData;
			ptr->LastUpdate = updateTime;
			if (updateTime - ptr->LastHalfOpenPrune > ptr->SSLHandshakeTimeoutMin)
			{
				PruneHalfOpenConnections(ptr);
				ptr->LastHalfOpenPrune = updateTime;
			}
		}
	}
}
