using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

public class BridgeTriggerType2 : NetworkBehaviour
{
	private int timesTriggered;

	public AnimatedObjectTrigger animatedObjectTrigger;

	private bool bridgeFell;

	private void OnTriggerEnter(Collider other)
	{
		if (!bridgeFell)
		{
			PlayerControllerB component = other.gameObject.GetComponent<PlayerControllerB>();
			if (component != null && GameNetworkManager.Instance.localPlayerController == component)
			{
				AddToBridgeInstabilityServerRpc();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void AddToBridgeInstabilityServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
		{
			ServerRpcParams serverRpcParams = default(ServerRpcParams);
			FastBufferWriter bufferWriter = __beginSendServerRpc(1248555425u, serverRpcParams, RpcDelivery.Reliable);
			__endSendServerRpc(ref bufferWriter, 1248555425u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			timesTriggered++;
			if (timesTriggered == 2)
			{
				animatedObjectTrigger.TriggerAnimation(GameNetworkManager.Instance.localPlayerController);
			}
			if (timesTriggered >= 4)
			{
				bridgeFell = true;
				animatedObjectTrigger.TriggerAnimation(GameNetworkManager.Instance.localPlayerController);
			}
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_BridgeTriggerType2()
	{
		NetworkManager.__rpc_func_table.Add(1248555425u, __rpc_handler_1248555425);
	}

	private static void __rpc_handler_1248555425(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((BridgeTriggerType2)target).AddToBridgeInstabilityServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "BridgeTriggerType2";
	}
}
