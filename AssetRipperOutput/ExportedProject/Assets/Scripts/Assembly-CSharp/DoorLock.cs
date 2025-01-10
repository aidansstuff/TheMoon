using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(InteractTrigger))]
public class DoorLock : NetworkBehaviour
{
	private InteractTrigger doorTrigger;

	public float maxTimeLeft = 60f;

	public float lockPickTimeLeft = 60f;

	public bool isLocked;

	public bool isPickingLock;

	[Space(5f)]
	public DoorLock twinDoor;

	public Transform lockPickerPosition;

	public Transform lockPickerPosition2;

	private float enemyDoorMeter;

	private bool isDoorOpened;

	private NavMeshObstacle navMeshObstacle;

	public AudioClip pickingLockSFX;

	public AudioClip unlockSFX;

	public AudioSource doorLockSFX;

	private bool displayedLockTip;

	private bool localPlayerPickingLock;

	private int playersPickingDoor;

	private float playerPickingLockProgress;

	[Space(3f)]
	public float defaultTimeToHold = 0.3f;

	private bool hauntedDoor;

	private float doorHauntInterval;

	public void Awake()
	{
		doorTrigger = base.gameObject.GetComponent<InteractTrigger>();
		lockPickTimeLeft = maxTimeLeft;
		navMeshObstacle = GetComponent<NavMeshObstacle>();
		if (RoundManager.Instance.currentDungeonType == 1 && Random.Range(0, 100) < 7)
		{
			hauntedDoor = true;
		}
	}

	public void OnHoldInteract()
	{
		if (isLocked && !displayedLockTip && HUDManager.Instance.holdFillAmount / doorTrigger.timeToHold > 0.3f)
		{
			displayedLockTip = true;
			HUDManager.Instance.DisplayTip("TIP:", "To get through locked doors efficiently, order a <u>lock-picker</u> from the ship terminal.", isWarning: false, useSave: true, "LCTip_Autopicker");
		}
	}

	public void LockDoor(float timeToLockPick = 30f)
	{
		doorTrigger.interactable = false;
		doorTrigger.timeToHold = timeToLockPick;
		doorTrigger.hoverTip = "Locked (pickable)";
		doorTrigger.holdTip = "Picking lock";
		isLocked = true;
		navMeshObstacle.carving = true;
		navMeshObstacle.carveOnlyStationary = true;
		if (twinDoor != null)
		{
			twinDoor.doorTrigger.interactable = false;
			twinDoor.doorTrigger.timeToHold = 35f;
			twinDoor.doorTrigger.hoverTip = "Locked (pickable)";
			twinDoor.doorTrigger.holdTip = "Picking lock";
			twinDoor.isLocked = true;
		}
	}

	public void UnlockDoor()
	{
		doorLockSFX.Stop();
		doorLockSFX.PlayOneShot(unlockSFX);
		navMeshObstacle.carving = false;
		if (isLocked)
		{
			doorTrigger.interactable = true;
			doorTrigger.hoverTip = "Use door : [LMB]";
			doorTrigger.holdTip = "";
			isPickingLock = false;
			isLocked = false;
			doorTrigger.timeToHoldSpeedMultiplier = 1f;
			navMeshObstacle.carving = false;
			Debug.Log("Unlocking door");
			doorTrigger.timeToHold = defaultTimeToHold;
		}
	}

	public void UnlockDoorSyncWithServer()
	{
		if (isLocked)
		{
			UnlockDoor();
			UnlockDoorServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void UnlockDoorServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(184554516u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 184554516u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				UnlockDoorClientRpc();
			}
		}
	}

	[ClientRpc]
	public void UnlockDoorClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(1778576778u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 1778576778u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				UnlockDoor();
			}
		}
	}

	private void TryDoorHaunt()
	{
		if (!(Time.realtimeSinceStartup - doorHauntInterval > 0.5f))
		{
			return;
		}
		doorHauntInterval = Time.realtimeSinceStartup + Mathf.Max(Random.Range(-18f, 30f), 0f);
		if (Random.Range(0, 100) > 50)
		{
			return;
		}
		if (enemyDoorMeter >= 0.1f || StartOfRound.Instance.fearLevel >= 0.4f || GameNetworkManager.Instance.localPlayerController.insanityLevel < GameNetworkManager.Instance.localPlayerController.maxInsanityLevel / 1.2f)
		{
			doorHauntInterval = Mathf.Max(Time.realtimeSinceStartup + 15f, doorHauntInterval);
		}
		else if (Vector3.Distance(base.transform.position, GameNetworkManager.Instance.localPlayerController.transform.position) > 18f || Physics.Linecast(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, base.transform.position, 256, QueryTriggerInteraction.Ignore))
		{
			doorHauntInterval = Mathf.Max(Time.realtimeSinceStartup + 15f, doorHauntInterval);
		}
		else
		{
			if (Physics.CheckSphere(base.transform.position, 4f, 8912896, QueryTriggerInteraction.Ignore))
			{
				return;
			}
			AnimatedObjectTrigger component = base.gameObject.GetComponent<AnimatedObjectTrigger>();
			if (Random.Range(0, 100) < 16)
			{
				component.TriggerAnimationNonPlayer(playSecondaryAudios: true, overrideBool: true);
				OpenDoorAsEnemyServerRpc();
				return;
			}
			component.TriggerAnimationNonPlayer();
			if (component.boolValue)
			{
				OpenDoorAsEnemyServerRpc();
			}
			else
			{
				CloseDoorNonPlayerServerRpc();
			}
		}
	}

	private void Update()
	{
		if (isLocked)
		{
			if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
			{
				return;
			}
			if (GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer != null && GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer.itemProperties.itemId == 14)
			{
				if (StartOfRound.Instance.localPlayerUsingController)
				{
					doorTrigger.disabledHoverTip = "Use key: [R-trigger]";
				}
				else
				{
					doorTrigger.disabledHoverTip = "Use key: [ LMB ]";
				}
			}
			else
			{
				doorTrigger.disabledHoverTip = "Locked";
			}
			if (playersPickingDoor > 0)
			{
				playerPickingLockProgress = Mathf.Clamp(playerPickingLockProgress + (float)playersPickingDoor * 0.85f * Time.deltaTime, 1f, 3.5f);
			}
			doorTrigger.timeToHoldSpeedMultiplier = Mathf.Clamp((float)playersPickingDoor * 0.85f, 1f, 3.5f);
		}
		else
		{
			navMeshObstacle.carving = false;
			if (hauntedDoor)
			{
				TryDoorHaunt();
			}
		}
		if (isLocked && isPickingLock)
		{
			lockPickTimeLeft -= Time.deltaTime;
			doorTrigger.disabledHoverTip = $"Picking lock: {(int)lockPickTimeLeft} sec.";
			if (base.IsServer && lockPickTimeLeft < 0f)
			{
				UnlockDoor();
				UnlockDoorServerRpc();
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (NetworkManager.Singleton == null || !base.IsServer || isLocked || isDoorOpened || !other.CompareTag("Enemy"))
		{
			return;
		}
		EnemyAICollisionDetect component = other.GetComponent<EnemyAICollisionDetect>();
		if (!(component == null) && !component.mainScript.isEnemyDead)
		{
			enemyDoorMeter += Time.deltaTime * component.mainScript.openDoorSpeedMultiplier;
			if (enemyDoorMeter > 1f)
			{
				enemyDoorMeter = 0f;
				base.gameObject.GetComponent<AnimatedObjectTrigger>().TriggerAnimationNonPlayer(component.mainScript.useSecondaryAudiosOnAnimatedObjects, overrideBool: true);
				OpenDoorAsEnemyServerRpc();
			}
		}
	}

	public void OpenOrCloseDoor(PlayerControllerB playerWhoTriggered)
	{
		AnimatedObjectTrigger component = base.gameObject.GetComponent<AnimatedObjectTrigger>();
		component.TriggerAnimation(playerWhoTriggered);
		isDoorOpened = component.boolValue;
		navMeshObstacle.enabled = !component.boolValue;
	}

	public void SetDoorAsOpen(bool isOpen)
	{
		isDoorOpened = isOpen;
		navMeshObstacle.enabled = !isOpen;
	}

	public void OpenDoorAsEnemy(bool setOpen = true)
	{
		if (hauntedDoor)
		{
			doorHauntInterval = Time.realtimeSinceStartup;
		}
		if (!setOpen)
		{
			isDoorOpened = false;
			navMeshObstacle.enabled = true;
		}
		else
		{
			isDoorOpened = true;
			navMeshObstacle.enabled = false;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void OpenDoorAsEnemyServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(2046162111u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 2046162111u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				OpenDoorAsEnemyClientRpc();
			}
		}
	}

	[ClientRpc]
	public void OpenDoorAsEnemyClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(1188121580u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 1188121580u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				OpenDoorAsEnemy();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void CloseDoorNonPlayerServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(2211684126u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 2211684126u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				CloseDoorNonPlayerClientRpc();
			}
		}
	}

	[ClientRpc]
	public void CloseDoorNonPlayerClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(844660933u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 844660933u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				OpenDoorAsEnemy(setOpen: false);
			}
		}
	}

	public void TryPickingLock()
	{
		if (isLocked)
		{
			HUDManager.Instance.holdFillAmount = playerPickingLockProgress;
			if (!localPlayerPickingLock)
			{
				localPlayerPickingLock = true;
				PlayerPickLockServerRpc();
			}
		}
	}

	public void StopPickingLock()
	{
		if (localPlayerPickingLock)
		{
			localPlayerPickingLock = false;
			if (playersPickingDoor == 1)
			{
				playerPickingLockProgress = Mathf.Clamp(playerPickingLockProgress - 1f, 0f, 45f);
			}
			PlayerStopPickingLockServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void PlayerStopPickingLockServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(3458026102u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 3458026102u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				PlayerStopPickingLockClientRpc();
			}
		}
	}

	[ClientRpc]
	public void PlayerStopPickingLockClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(3319502281u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 3319502281u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				doorLockSFX.Stop();
				playersPickingDoor = Mathf.Clamp(playersPickingDoor - 1, 0, 4);
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void PlayerPickLockServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(2269869251u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 2269869251u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				PlayerPickLockClientRpc();
			}
		}
	}

	[ClientRpc]
	public void PlayerPickLockClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(1721192172u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 1721192172u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				doorLockSFX.clip = pickingLockSFX;
				doorLockSFX.Play();
				playersPickingDoor = Mathf.Clamp(playersPickingDoor + 1, 0, 4);
			}
		}
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_DoorLock()
	{
		NetworkManager.__rpc_func_table.Add(184554516u, __rpc_handler_184554516);
		NetworkManager.__rpc_func_table.Add(1778576778u, __rpc_handler_1778576778);
		NetworkManager.__rpc_func_table.Add(2046162111u, __rpc_handler_2046162111);
		NetworkManager.__rpc_func_table.Add(1188121580u, __rpc_handler_1188121580);
		NetworkManager.__rpc_func_table.Add(2211684126u, __rpc_handler_2211684126);
		NetworkManager.__rpc_func_table.Add(844660933u, __rpc_handler_844660933);
		NetworkManager.__rpc_func_table.Add(3458026102u, __rpc_handler_3458026102);
		NetworkManager.__rpc_func_table.Add(3319502281u, __rpc_handler_3319502281);
		NetworkManager.__rpc_func_table.Add(2269869251u, __rpc_handler_2269869251);
		NetworkManager.__rpc_func_table.Add(1721192172u, __rpc_handler_1721192172);
	}

	private static void __rpc_handler_184554516(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((DoorLock)target).UnlockDoorServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_1778576778(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((DoorLock)target).UnlockDoorClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2046162111(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((DoorLock)target).OpenDoorAsEnemyServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_1188121580(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((DoorLock)target).OpenDoorAsEnemyClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2211684126(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((DoorLock)target).CloseDoorNonPlayerServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_844660933(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((DoorLock)target).CloseDoorNonPlayerClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3458026102(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((DoorLock)target).PlayerStopPickingLockServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3319502281(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((DoorLock)target).PlayerStopPickingLockClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2269869251(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((DoorLock)target).PlayerPickLockServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_1721192172(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((DoorLock)target).PlayerPickLockClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "DoorLock";
	}
}
