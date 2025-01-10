using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Unity.Netcode
{
	internal static class RpcMessageHelpers
	{
		public unsafe static void Serialize(ref FastBufferWriter writer, ref RpcMetadata metadata, ref FastBufferWriter payload)
		{
			BytePacker.WriteValueBitPacked(writer, metadata.NetworkObjectId);
			BytePacker.WriteValueBitPacked(writer, metadata.NetworkBehaviourId);
			BytePacker.WriteValueBitPacked(writer, metadata.NetworkRpcMethodId);
			writer.WriteBytesSafe(payload.GetUnsafePtr(), payload.Length);
		}

		public unsafe static bool Deserialize(ref FastBufferReader reader, ref NetworkContext context, ref RpcMetadata metadata, ref FastBufferReader payload)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkObjectId);
			ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkBehaviourId);
			ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkRpcMethodId);
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.SpawnManager.SpawnedObjects.ContainsKey(metadata.NetworkObjectId))
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnSpawn, metadata.NetworkObjectId, reader, ref context);
				return false;
			}
			_ = networkManager.SpawnManager.SpawnedObjects[metadata.NetworkObjectId];
			if (networkManager.SpawnManager.SpawnedObjects[metadata.NetworkObjectId].GetNetworkBehaviourAtOrderIndex(metadata.NetworkBehaviourId) == null)
			{
				return false;
			}
			if (!NetworkManager.__rpc_func_table.ContainsKey(metadata.NetworkRpcMethodId))
			{
				return false;
			}
			payload = new FastBufferReader(reader.GetUnsafePtrAtCurrentPosition(), Allocator.None, reader.Length - reader.Position);
			return true;
		}

		public static void Handle(ref NetworkContext context, ref RpcMetadata metadata, ref FastBufferReader payload, ref __RpcParams rpcParams)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.SpawnManager.SpawnedObjects.TryGetValue(metadata.NetworkObjectId, out var value))
			{
				throw new InvalidOperationException("An RPC called on a NetworkObject that is not in the spawned objects list. Please make sure the NetworkObject is spawned before calling RPCs.");
			}
			NetworkBehaviour networkBehaviourAtOrderIndex = value.GetNetworkBehaviourAtOrderIndex(metadata.NetworkBehaviourId);
			try
			{
				NetworkManager.__rpc_func_table[metadata.NetworkRpcMethodId](networkBehaviourAtOrderIndex, payload, rpcParams);
			}
			catch (Exception innerException)
			{
				Debug.LogException(new Exception("Unhandled RPC exception!", innerException));
				if (networkManager.LogLevel != 0)
				{
					return;
				}
				Debug.Log("RPC Table Contents");
				foreach (KeyValuePair<uint, NetworkManager.RpcReceiveHandler> item in NetworkManager.__rpc_func_table)
				{
					Debug.Log($"{item.Key} | {item.Value.Method.Name}");
				}
			}
		}
	}
}
