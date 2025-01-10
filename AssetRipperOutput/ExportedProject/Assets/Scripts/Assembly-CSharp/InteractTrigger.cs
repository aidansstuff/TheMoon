using System.Collections;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

public class InteractTrigger : NetworkBehaviour
{
	[Header("Aesthetics")]
	public Sprite hoverIcon;

	public string hoverTip;

	[Space(5f)]
	public Sprite disabledHoverIcon;

	public string disabledHoverTip;

	[Header("Interaction")]
	public bool interactable = true;

	public bool oneHandedItemAllowed = true;

	public bool twoHandedItemAllowed;

	[Space(5f)]
	public bool holdInteraction;

	public float timeToHold = 0.5f;

	public float timeToHoldSpeedMultiplier = 1f;

	public string holdTip;

	public bool isBeingHeldByPlayer;

	public InteractEventFloat holdingInteractEvent;

	private float timeHeld;

	private bool isHoldingThisFrame;

	[Space(5f)]
	public bool touchTrigger;

	public bool triggerOnce;

	private bool hasTriggered;

	[Header("Misc")]
	public bool interactCooldown = true;

	public float cooldownTime = 1f;

	[HideInInspector]
	public float currentCooldownValue;

	public bool disableTriggerMesh = true;

	[Space(5f)]
	public bool RandomChanceTrigger;

	public int randomChancePercentage;

	[Header("Events")]
	public InteractEvent onInteractEarlyOtherClients;

	public InteractEvent onInteract;

	public InteractEvent onInteractEarly;

	public InteractEvent onStopInteract;

	public InteractEvent onCancelAnimation;

	[Header("Special Animation")]
	public bool specialCharacterAnimation;

	public bool stopAnimationManually;

	public string stopAnimationString = "SA_stopAnimation";

	public bool hidePlayerItem;

	public bool isPlayingSpecialAnimation;

	public float animationWaitTime = 2f;

	public string animationString;

	[Space(5f)]
	public bool lockPlayerPosition;

	public Transform playerPositionNode;

	private Transform lockedPlayer;

	public bool clampLooking;

	public bool setVehicleAnimation;

	public float minVerticalClamp;

	public float maxVerticalClamp;

	public float horizontalClamp;

	public bool allowUseWhileInAnimation;

	public Transform overridePlayerParent;

	private bool usedByOtherClient;

	private StartOfRound playersManager;

	private float updateInterval = 1f;

	[Header("Ladders")]
	public bool isLadder;

	public Transform topOfLadderPosition;

	public bool useRaycastToGetTopPosition;

	public Transform bottomOfLadderPosition;

	public Transform ladderHorizontalPosition;

	[Space(5f)]
	public Transform ladderPlayerPositionNode;

	public bool usingLadder;

	private bool atBottomOfLadder;

	private Vector3 moveVelocity;

	private PlayerControllerB playerScriptInSpecialAnimation;

	private Coroutine useLadderCoroutine;

	private int playerUsingId;

	public void StopInteraction()
	{
		if (isBeingHeldByPlayer && currentCooldownValue <= 0f)
		{
			isBeingHeldByPlayer = false;
			onStopInteract.Invoke(null);
		}
	}

	public void HoldInteractNotFilled()
	{
		holdingInteractEvent.Invoke(HUDManager.Instance.holdFillAmount / timeToHold);
		if (!specialCharacterAnimation && !isLadder)
		{
			if (!isBeingHeldByPlayer)
			{
				onInteractEarly.Invoke(null);
			}
			isBeingHeldByPlayer = true;
		}
	}

	public void Interact(Transform playerTransform)
	{
		if ((triggerOnce && hasTriggered) || StartOfRound.Instance.firingPlayersCutsceneRunning)
		{
			return;
		}
		hasTriggered = true;
		if (RandomChanceTrigger && Random.Range(0, 101) > randomChancePercentage)
		{
			return;
		}
		if (!interactable || isPlayingSpecialAnimation || usingLadder)
		{
			if (usingLadder)
			{
				CancelLadderAnimation();
			}
			return;
		}
		PlayerControllerB component = playerTransform.GetComponent<PlayerControllerB>();
		if (component.inSpecialInteractAnimation && !component.isClimbingLadder && !allowUseWhileInAnimation)
		{
			return;
		}
		if (interactCooldown)
		{
			if (currentCooldownValue >= 0f)
			{
				return;
			}
			currentCooldownValue = cooldownTime;
		}
		if (!specialCharacterAnimation && !isLadder)
		{
			onInteract.Invoke(component);
			return;
		}
		component.ResetFallGravity();
		if (isLadder)
		{
			if (component.isInHangarShipRoom)
			{
				return;
			}
			ladderPlayerPositionNode.position = new Vector3(ladderHorizontalPosition.position.x, Mathf.Clamp(component.thisPlayerBody.position.y, bottomOfLadderPosition.position.y + 0.3f, topOfLadderPosition.position.y - 2.2f), ladderHorizontalPosition.position.z);
			if (!LadderPositionObstructed(component))
			{
				if (useLadderCoroutine != null)
				{
					StopCoroutine(useLadderCoroutine);
				}
				useLadderCoroutine = StartCoroutine(ladderClimbAnimation(component));
			}
		}
		else
		{
			StartCoroutine(specialInteractAnimation(component));
		}
	}

	private bool LadderPositionObstructed(PlayerControllerB playerController)
	{
		RaycastHit hitInfo2;
		if (playerController.transform.position.y >= topOfLadderPosition.position.y - 0.5f)
		{
			if (Physics.Linecast(playerController.gameplayCamera.transform.position, ladderPlayerPositionNode.position + Vector3.up * 2.8f, out var _, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
			{
				return true;
			}
		}
		else if (Physics.Linecast(playerController.gameplayCamera.transform.position, ladderPlayerPositionNode.position, out hitInfo2, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
		{
			return true;
		}
		return false;
	}

	private IEnumerator ladderClimbAnimation(PlayerControllerB playerController)
	{
		onInteractEarly.Invoke(null);
		lockedPlayer = playerController.thisPlayerBody;
		playerScriptInSpecialAnimation = playerController;
		if (hidePlayerItem && playerScriptInSpecialAnimation.currentlyHeldObjectServer != null)
		{
			playerScriptInSpecialAnimation.currentlyHeldObjectServer.EnableItemMeshes(enable: false);
		}
		SetUsingLadderOnLocalClient(isUsing: true);
		hoverTip = "Let go : [LMB]";
		if (!playerController.isTestingPlayer)
		{
			playerController.UpdateSpecialAnimationValue(specialAnimation: true, (short)ladderPlayerPositionNode.eulerAngles.y, 0f, climbingLadder: true);
		}
		playerController.enteringSpecialAnimation = true;
		playerController.inSpecialInteractAnimation = true;
		playerController.currentTriggerInAnimationWith = this;
		playerController.isCrouching = false;
		playerController.playerBodyAnimator.SetBool("crouching", value: false);
		playerController.playerBodyAnimator.SetTrigger("EnterLadder");
		playerController.thisController.enabled = false;
		float timer2 = 0f;
		while (timer2 <= animationWaitTime)
		{
			yield return null;
			timer2 += Time.deltaTime;
			playerController.thisPlayerBody.position = Vector3.Lerp(playerController.thisPlayerBody.position, ladderPlayerPositionNode.position, Mathf.SmoothStep(0f, 1f, timer2 / animationWaitTime));
			lockedPlayer.rotation = Quaternion.Lerp(lockedPlayer.rotation, ladderPlayerPositionNode.rotation, Mathf.SmoothStep(0f, 1f, timer2 / animationWaitTime));
		}
		playerController.TeleportPlayer(ladderPlayerPositionNode.position, withRotation: false, 0f, allowInteractTrigger: true);
		Debug.Log("Finished snapping to ladder");
		playerController.playerBodyAnimator.SetBool("ClimbingLadder", value: true);
		playerController.isClimbingLadder = true;
		playerController.enteringSpecialAnimation = false;
		playerController.ladderCameraHorizontal = 0f;
		playerController.clampCameraRotation = bottomOfLadderPosition.eulerAngles;
		int finishClimbingLadder = 0;
		while (finishClimbingLadder == 0)
		{
			yield return null;
			if (playerController.thisPlayerBody.position.y < bottomOfLadderPosition.position.y)
			{
				finishClimbingLadder = 1;
			}
			else if (playerController.thisPlayerBody.position.y + 2f > topOfLadderPosition.position.y)
			{
				finishClimbingLadder = 2;
			}
		}
		playerController.isClimbingLadder = false;
		playerController.playerBodyAnimator.SetBool("ClimbingLadder", value: false);
		if (finishClimbingLadder == 1)
		{
			ladderPlayerPositionNode.position = bottomOfLadderPosition.position;
		}
		else if (!useRaycastToGetTopPosition)
		{
			ladderPlayerPositionNode.position = topOfLadderPosition.position;
		}
		else
		{
			Ray ray = new Ray(playerController.transform.position + Vector3.up, topOfLadderPosition.position + Vector3.up - playerController.transform.position + Vector3.up);
			if (Physics.Linecast(playerController.transform.position + Vector3.up, topOfLadderPosition.position + Vector3.up, out var hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(playerController.transform.position + Vector3.up, topOfLadderPosition.position + Vector3.up, Color.red, 10f);
				ladderPlayerPositionNode.position = ray.GetPoint(Mathf.Max(hitInfo.distance - 1.2f, 0f));
				Debug.DrawRay(ladderPlayerPositionNode.position, Vector3.up * 0.5f, Color.yellow, 10f);
			}
			else
			{
				Debug.DrawLine(playerController.transform.position + Vector3.up, topOfLadderPosition.position + Vector3.up, Color.green, 10f);
				ladderPlayerPositionNode.position = topOfLadderPosition.position;
			}
		}
		timer2 = 0f;
		float shorterWaitTime = animationWaitTime / 2f;
		while (timer2 <= shorterWaitTime)
		{
			yield return null;
			timer2 += Time.deltaTime;
			playerController.thisPlayerBody.position = Vector3.Lerp(playerController.thisPlayerBody.position, ladderPlayerPositionNode.position, Mathf.SmoothStep(0f, 1f, timer2 / shorterWaitTime));
			playerController.thisPlayerBody.rotation = Quaternion.Lerp(playerController.thisPlayerBody.rotation, ladderPlayerPositionNode.rotation, Mathf.SmoothStep(0f, 1f, timer2 / shorterWaitTime));
			playerController.gameplayCamera.transform.rotation = Quaternion.Slerp(playerController.gameplayCamera.transform.rotation, playerController.gameplayCamera.transform.parent.rotation, Mathf.SmoothStep(0f, 1f, timer2 / shorterWaitTime));
		}
		playerController.gameplayCamera.transform.localEulerAngles = Vector3.zero;
		Debug.Log("Finished ladder sequence");
		playerController.UpdateSpecialAnimationValue(specialAnimation: false, 0);
		playerController.inSpecialInteractAnimation = false;
		playerController.thisController.enabled = true;
		SetUsingLadderOnLocalClient(isUsing: false);
		hoverTip = "Use ladder : [LMB]";
		lockedPlayer = null;
		currentCooldownValue = cooldownTime;
		onInteract.Invoke(null);
	}

	public void CancelAnimationExternally()
	{
		if (isLadder)
		{
			CancelLadderAnimation();
		}
		else
		{
			StopSpecialAnimation();
		}
	}

	public void CancelLadderAnimation()
	{
		if (useLadderCoroutine != null)
		{
			StopCoroutine(useLadderCoroutine);
		}
		onCancelAnimation.Invoke(playerScriptInSpecialAnimation);
		playerScriptInSpecialAnimation.currentTriggerInAnimationWith = null;
		playerScriptInSpecialAnimation.isClimbingLadder = false;
		playerScriptInSpecialAnimation.thisController.enabled = true;
		playerScriptInSpecialAnimation.playerBodyAnimator.SetBool("ClimbingLadder", value: false);
		playerScriptInSpecialAnimation.gameplayCamera.transform.localEulerAngles = Vector3.zero;
		playerScriptInSpecialAnimation.UpdateSpecialAnimationValue(specialAnimation: false, 0);
		playerScriptInSpecialAnimation.inSpecialInteractAnimation = false;
		SetUsingLadderOnLocalClient(isUsing: false);
		lockedPlayer = null;
		currentCooldownValue = cooldownTime;
		if (hidePlayerItem && playerScriptInSpecialAnimation.currentlyHeldObjectServer != null)
		{
			playerScriptInSpecialAnimation.currentlyHeldObjectServer.EnableItemMeshes(enable: true);
		}
		onInteract.Invoke(null);
	}

	private void SetUsingLadderOnLocalClient(bool isUsing)
	{
		usingLadder = isUsing;
		if (isUsing)
		{
			hoverTip = "Let go : [LMB]";
		}
		else
		{
			hoverTip = "Climb : [LMB]";
		}
	}

	private IEnumerator specialInteractAnimation(PlayerControllerB playerController)
	{
		UpdateUsedByPlayerServerRpc((int)playerController.playerClientId);
		onInteractEarly.Invoke(null);
		isPlayingSpecialAnimation = true;
		lockedPlayer = playerController.thisPlayerBody;
		playerScriptInSpecialAnimation = playerController;
		if (clampLooking)
		{
			playerScriptInSpecialAnimation.minVerticalClamp = minVerticalClamp;
			playerScriptInSpecialAnimation.maxVerticalClamp = maxVerticalClamp;
			playerScriptInSpecialAnimation.horizontalClamp = horizontalClamp;
			playerScriptInSpecialAnimation.clampLooking = true;
		}
		if ((bool)overridePlayerParent)
		{
			playerScriptInSpecialAnimation.overridePhysicsParent = overridePlayerParent;
		}
		if (setVehicleAnimation)
		{
			playerScriptInSpecialAnimation.inVehicleAnimation = true;
		}
		if (hidePlayerItem && playerScriptInSpecialAnimation.currentlyHeldObjectServer != null)
		{
			playerScriptInSpecialAnimation.currentlyHeldObjectServer.EnableItemMeshes(enable: false);
		}
		playerController.Crouch(crouch: false);
		playerController.UpdateSpecialAnimationValue(specialAnimation: true, (short)playerPositionNode.eulerAngles.y);
		playerController.inSpecialInteractAnimation = true;
		playerController.currentTriggerInAnimationWith = this;
		playerController.playerBodyAnimator.ResetTrigger(animationString);
		playerController.playerBodyAnimator.SetTrigger(animationString);
		HUDManager.Instance.ClearControlTips();
		if (!stopAnimationManually)
		{
			yield return new WaitForSeconds(animationWaitTime);
			StopSpecialAnimation();
		}
	}

	public void StopSpecialAnimation()
	{
		Debug.Log($"Stop special animation on {base.gameObject.name}, by {lockedPlayer}; {GameNetworkManager.Instance.localPlayerController}");
		if (lockedPlayer != GameNetworkManager.Instance.localPlayerController.transform)
		{
			return;
		}
		if (isPlayingSpecialAnimation && stopAnimationManually && lockedPlayer != null)
		{
			Debug.Log($"Calling stop animation function StopUsing server rpc for player: {GameNetworkManager.Instance.localPlayerController.playerClientId}");
			StopUsingServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId);
		}
		if (lockedPlayer != null)
		{
			PlayerControllerB component = lockedPlayer.GetComponent<PlayerControllerB>();
			onCancelAnimation.Invoke(component);
			if (hidePlayerItem && component.currentlyHeldObjectServer != null)
			{
				component.currentlyHeldObjectServer.EnableItemMeshes(enable: true);
			}
			isPlayingSpecialAnimation = false;
			component.inSpecialInteractAnimation = false;
			if (component.clampLooking)
			{
				component.gameplayCamera.transform.localEulerAngles = Vector3.zero;
			}
			component.clampLooking = false;
			component.inVehicleAnimation = false;
			if ((bool)overridePlayerParent && component.overridePhysicsParent == overridePlayerParent)
			{
				component.overridePhysicsParent = null;
			}
			component.currentTriggerInAnimationWith = null;
			if (component.isClimbingLadder)
			{
				CancelLadderAnimation();
				component.isClimbingLadder = false;
			}
			Debug.Log("Stop special animation F");
			if (stopAnimationManually)
			{
				component.playerBodyAnimator.SetTrigger(stopAnimationString);
			}
			component.UpdateSpecialAnimationValue(specialAnimation: false, 0);
			lockedPlayer = null;
			currentCooldownValue = cooldownTime;
			onInteract.Invoke(null);
			Debug.Log("Stop special animation G");
			if (component.isHoldingObject && component.currentlyHeldObjectServer != null)
			{
				component.currentlyHeldObjectServer.SetControlTipsForItem();
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateUsedByPlayerServerRpc(int playerNum)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(1430497838u, serverRpcParams, RpcDelivery.Reliable);
				BytePacker.WriteValueBitPacked(bufferWriter, playerNum);
				__endSendServerRpc(ref bufferWriter, 1430497838u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				UpdateUsedByPlayerClientRpc(playerNum);
			}
		}
	}

	[ClientRpc]
	private void UpdateUsedByPlayerClientRpc(int playerNum)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default(ClientRpcParams);
			FastBufferWriter bufferWriter = __beginSendClientRpc(3458599252u, clientRpcParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(bufferWriter, playerNum);
			__endSendClientRpc(ref bufferWriter, 3458599252u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
		{
			onInteractEarlyOtherClients.Invoke(StartOfRound.Instance.allPlayerScripts[playerNum]);
			if (hidePlayerItem && StartOfRound.Instance.allPlayerScripts[playerNum].currentlyHeldObjectServer != null)
			{
				StartOfRound.Instance.allPlayerScripts[playerNum].currentlyHeldObjectServer.EnableItemMeshes(enable: false);
				playerUsingId = playerNum;
			}
			if (specialCharacterAnimation && setVehicleAnimation)
			{
				StartOfRound.Instance.allPlayerScripts[playerNum].inVehicleAnimation = true;
				StartOfRound.Instance.allPlayerScripts[playerNum].syncFullCameraRotation = StartOfRound.Instance.allPlayerScripts[playerNum].gameplayCamera.transform.localEulerAngles;
				lockedPlayer = StartOfRound.Instance.allPlayerScripts[playerNum].transform;
				playerScriptInSpecialAnimation = StartOfRound.Instance.allPlayerScripts[playerNum];
			}
			if (stopAnimationManually)
			{
				isPlayingSpecialAnimation = true;
				StartOfRound.Instance.allPlayerScripts[playerNum].currentTriggerInAnimationWith = this;
			}
			else
			{
				StartCoroutine(isSpecialAnimationPlayingTimer(playerNum));
			}
		}
	}

	private IEnumerator isSpecialAnimationPlayingTimer(int playerNum)
	{
		StartOfRound.Instance.allPlayerScripts[playerNum].currentTriggerInAnimationWith = this;
		isPlayingSpecialAnimation = true;
		yield return new WaitForSeconds(animationWaitTime);
		StartOfRound.Instance.allPlayerScripts[playerNum].currentTriggerInAnimationWith = null;
		isPlayingSpecialAnimation = false;
	}

	[ServerRpc(RequireOwnership = false)]
	private void StopUsingServerRpc(int playerUsing)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(880620475u, serverRpcParams, RpcDelivery.Reliable);
				BytePacker.WriteValueBitPacked(bufferWriter, playerUsing);
				__endSendServerRpc(ref bufferWriter, 880620475u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				StopUsingClientRpc(playerUsing);
			}
		}
	}

	[ClientRpc]
	private void StopUsingClientRpc(int playerUsing)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(953330655u, clientRpcParams, RpcDelivery.Reliable);
				BytePacker.WriteValueBitPacked(bufferWriter, playerUsing);
				__endSendClientRpc(ref bufferWriter, 953330655u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !(GameNetworkManager.Instance.localPlayerController == StartOfRound.Instance.allPlayerScripts[playerUsing]))
			{
				StartOfRound.Instance.allPlayerScripts[playerUsing].inVehicleAnimation = false;
				lockedPlayer = null;
				playerScriptInSpecialAnimation = null;
				SetInteractTriggerNotInAnimation(playerUsing);
			}
		}
	}

	public void SetInteractTriggerNotInAnimation(int playerUsing = -1)
	{
		if (playerUsing == -1)
		{
			playerUsing = playerUsingId;
		}
		isPlayingSpecialAnimation = false;
		if (playerUsing != -1)
		{
			StartOfRound.Instance.allPlayerScripts[playerUsing].inVehicleAnimation = false;
			if (StartOfRound.Instance.allPlayerScripts[playerUsing].currentlyHeldObjectServer != null)
			{
				StartOfRound.Instance.allPlayerScripts[playerUsing].currentlyHeldObjectServer.EnableItemMeshes(enable: true);
			}
			StartOfRound.Instance.allPlayerScripts[playerUsing].currentTriggerInAnimationWith = null;
			playerUsingId = -1;
		}
	}

	private void LateUpdate()
	{
		if (isPlayingSpecialAnimation && lockedPlayer != null && !playerScriptInSpecialAnimation.isPlayerDead && lockPlayerPosition)
		{
			lockedPlayer.localPosition = Vector3.Lerp(lockedPlayer.localPosition, lockedPlayer.parent.InverseTransformPoint(playerPositionNode.position), Time.deltaTime * 20f);
			lockedPlayer.rotation = Quaternion.Lerp(lockedPlayer.rotation, playerPositionNode.rotation, Time.deltaTime * 20f);
		}
	}

	private void OnEnable()
	{
		if (interactCooldown)
		{
			currentCooldownValue = cooldownTime;
		}
	}

	private void Update()
	{
		if (currentCooldownValue >= 0f)
		{
			currentCooldownValue -= Time.deltaTime;
		}
		if (isPlayingSpecialAnimation)
		{
			if (lockedPlayer != null && playerScriptInSpecialAnimation.isPlayerDead)
			{
				StopSpecialAnimation();
			}
		}
		else if (usingLadder && playerScriptInSpecialAnimation != null && playerScriptInSpecialAnimation.isPlayerDead)
		{
			CancelLadderAnimation();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (touchTrigger && other.gameObject.CompareTag("Player") && (bool)other.gameObject.GetComponent<PlayerControllerB>() && other.gameObject.GetComponent<PlayerControllerB>().IsOwner)
		{
			Interact(other.gameObject.GetComponent<PlayerControllerB>().thisPlayerBody);
		}
	}

	private void Start()
	{
		if (disableTriggerMesh && (bool)base.gameObject.GetComponent<MeshRenderer>())
		{
			base.gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		playersManager = Object.FindObjectOfType<StartOfRound>();
	}

	public void SetInteractionToHold(bool mustHold)
	{
		holdInteraction = mustHold;
	}

	public void SetInteractionToHoldOpposite(bool mustHold)
	{
		holdInteraction = !mustHold;
	}

	public void SetRandomTimeToHold(float min, float max)
	{
		timeToHold = Random.Range(min, max);
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_InteractTrigger()
	{
		NetworkManager.__rpc_func_table.Add(1430497838u, __rpc_handler_1430497838);
		NetworkManager.__rpc_func_table.Add(3458599252u, __rpc_handler_3458599252);
		NetworkManager.__rpc_func_table.Add(880620475u, __rpc_handler_880620475);
		NetworkManager.__rpc_func_table.Add(953330655u, __rpc_handler_953330655);
	}

	private static void __rpc_handler_1430497838(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((InteractTrigger)target).UpdateUsedByPlayerServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3458599252(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((InteractTrigger)target).UpdateUsedByPlayerClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_880620475(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((InteractTrigger)target).StopUsingServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_953330655(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((InteractTrigger)target).StopUsingClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "InteractTrigger";
	}
}
