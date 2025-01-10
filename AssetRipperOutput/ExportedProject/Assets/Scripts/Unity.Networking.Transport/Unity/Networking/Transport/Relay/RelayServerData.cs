using System;
using System.Diagnostics;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public struct RelayServerData
	{
		public NetworkEndPoint Endpoint;

		public ushort Nonce;

		public RelayConnectionData ConnectionData;

		public RelayConnectionData HostConnectionData;

		public RelayAllocationId AllocationId;

		public RelayHMACKey HMACKey;

		public unsafe fixed byte HMAC[32];

		public readonly byte IsSecure;

		private unsafe RelayServerData(byte[] allocationId, byte[] connectionData, byte[] hostConnectionData, byte[] key)
		{
			Nonce = 0;
			AllocationId = RelayAllocationId.FromByteArray(allocationId);
			ConnectionData = RelayConnectionData.FromByteArray(connectionData);
			HostConnectionData = RelayConnectionData.FromByteArray(hostConnectionData);
			HMACKey = RelayHMACKey.FromByteArray(key);
			Endpoint = default(NetworkEndPoint);
			IsSecure = 0;
			fixed (byte* result = HMAC)
			{
				ComputeBindHMAC(result, Nonce, ref ConnectionData, ref HMACKey);
			}
		}

		public RelayServerData(Allocation allocation, string connectionType)
			: this(allocation.AllocationIdBytes, allocation.ConnectionData, allocation.ConnectionData, allocation.Key)
		{
			if (!new string[2] { "udp", "dtls" }.Contains(connectionType))
			{
				throw new ArgumentException("Invalid connection type: " + connectionType + ". Must be udp or dtls.");
			}
			RelayServerEndpoint relayServerEndpoint = allocation.ServerEndpoints.First((RelayServerEndpoint ep) => ep.ConnectionType == connectionType);
			Endpoint = HostToEndpoint(relayServerEndpoint.Host, (ushort)relayServerEndpoint.Port);
			IsSecure = (byte)(relayServerEndpoint.Secure ? 1 : 0);
		}

		public RelayServerData(JoinAllocation allocation, string connectionType)
			: this(allocation.AllocationIdBytes, allocation.ConnectionData, allocation.HostConnectionData, allocation.Key)
		{
			if (!new string[2] { "udp", "dtls" }.Contains(connectionType))
			{
				throw new ArgumentException("Invalid connection type: " + connectionType + ". Must be udp, or dtls.");
			}
			RelayServerEndpoint relayServerEndpoint = allocation.ServerEndpoints.First((RelayServerEndpoint ep) => ep.ConnectionType == connectionType);
			Endpoint = HostToEndpoint(relayServerEndpoint.Host, (ushort)relayServerEndpoint.Port);
			IsSecure = (byte)(relayServerEndpoint.Secure ? 1 : 0);
		}

		public RelayServerData(string host, ushort port, byte[] allocationId, byte[] connectionData, byte[] hostConnectionData, byte[] key, bool isSecure)
			: this(allocationId, connectionData, hostConnectionData, key)
		{
			Endpoint = HostToEndpoint(host, port);
			IsSecure = (byte)(isSecure ? 1 : 0);
		}

		[Obsolete("Will be removed in Unity Transport 2.0. Use the new constructor introduced in 1.3 instead.", false)]
		public unsafe RelayServerData(ref NetworkEndPoint endpoint, ushort nonce, RelayAllocationId allocationId, string connectionData, string hostConnectionData, string key, bool isSecure)
		{
			Endpoint = endpoint;
			AllocationId = allocationId;
			Nonce = nonce;
			IsSecure = (byte)(isSecure ? 1 : 0);
			fixed (byte* dest = ConnectionData.Value)
			{
				fixed (byte* dest2 = HostConnectionData.Value)
				{
					fixed (byte* dest3 = HMACKey.Value)
					{
						Base64.FromBase64String(connectionData, dest, 255);
						Base64.FromBase64String(hostConnectionData, dest2, 255);
						Base64.FromBase64String(key, dest3, 64);
					}
				}
			}
			fixed (byte* result = HMAC)
			{
				ComputeBindHMAC(result, Nonce, ref ConnectionData, ref HMACKey);
			}
		}

		public unsafe RelayServerData(ref NetworkEndPoint endpoint, ushort nonce, ref RelayAllocationId allocationId, ref RelayConnectionData connectionData, ref RelayConnectionData hostConnectionData, ref RelayHMACKey key, bool isSecure)
		{
			Endpoint = endpoint;
			Nonce = nonce;
			AllocationId = allocationId;
			ConnectionData = connectionData;
			HostConnectionData = hostConnectionData;
			HMACKey = key;
			IsSecure = (byte)(isSecure ? 1 : 0);
			fixed (byte* result = HMAC)
			{
				ComputeBindHMAC(result, Nonce, ref connectionData, ref key);
			}
		}

		[Obsolete("Will be removed in Unity Transport 2.0. There shouldn't be any need to call this method.")]
		public unsafe void ComputeNewNonce()
		{
			Nonce = (ushort)new Unity.Mathematics.Random((uint)Stopwatch.GetTimestamp()).NextUInt(1u, 61439u);
			fixed (byte* result = HMAC)
			{
				ComputeBindHMAC(result, Nonce, ref ConnectionData, ref HMACKey);
			}
		}

		private unsafe static void ComputeBindHMAC(byte* result, ushort nonce, ref RelayConnectionData connectionData, ref RelayHMACKey key)
		{
			byte[] array = new byte[64];
			fixed (byte* ptr = key.Value)
			{
				fixed (byte* destination = &array[0])
				{
					UnsafeUtility.MemCpy(destination, ptr, array.Length);
				}
				byte* ptr2 = stackalloc byte[263];
				*ptr2 = 218;
				ptr2[1] = 114;
				ptr2[5] = (byte)nonce;
				ptr2[6] = (byte)(nonce >> 8);
				ptr2[7] = byte.MaxValue;
				fixed (byte* source = connectionData.Value)
				{
					UnsafeUtility.MemCpy(ptr2 + 8, source, 255L);
				}
				HMACSHA256.ComputeHash(ptr, array.Length, ptr2, 263, result);
			}
		}

		private static NetworkEndPoint HostToEndpoint(string host, ushort port)
		{
			if (NetworkEndPoint.TryParse(host, port, out var endpoint))
			{
				return endpoint;
			}
			if (NetworkEndPoint.TryParse(host, port, out endpoint, NetworkFamily.Ipv6))
			{
				return endpoint;
			}
			UnityEngine.Debug.LogError("Host " + host + " is not a valid IPv4 or IPv6 address.");
			return endpoint;
		}
	}
}
