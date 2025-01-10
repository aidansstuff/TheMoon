using Unity.Netcode;
using UnityEngine;

public class MineshaftElevatorController : NetworkBehaviour
{
	public Animator elevatorAnimator;

	public Transform elevatorPoint;

	public bool elevatorFinishedMoving;

	public float elevatorFinishTimer;

	public bool elevatorIsAtBottom;

	public bool elevatorCalled;

	public bool elevatorMovingDown;

	private bool movingDownLastFrame = true;

	public bool calledDown;

	public float callCooldown;

	public AudioSource elevatorAudio;

	public AudioClip elevatorStartUpSFX;

	public AudioClip elevatorStartDownSFX;

	public AudioClip elevatorTravelSFX;

	public AudioClip elevatorFinishUpSFX;

	public AudioClip elevatorFinishDownSFX;

	public GameObject elevatorCalledBottomButton;

	public GameObject elevatorCalledTopButton;

	public Transform elevatorTopPoint;

	public Transform elevatorBottomPoint;

	public Transform elevatorInsidePoint;

	public Vector3 previousElevatorPosition;

	public bool elevatorDoorOpen;

	public AudioSource elevatorJingleMusic;

	private bool playMusic;

	private bool startedMusic;

	private float stopPlayingMusicTimer;

	public AudioClip[] elevatorHalloweenClips;

	public AudioClip[] elevatorHalloweenClipsLoop;

	[ServerRpc]
	public void SetElevatorMusicServerRpc(bool setOn)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
		{
			if (base.OwnerClientId != networkManager.LocalClientId)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			ServerRpcParams serverRpcParams = default(ServerRpcParams);
			FastBufferWriter bufferWriter = __beginSendServerRpc(248132445u, serverRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in setOn, default(FastBufferWriter.ForPrimitives));
			__endSendServerRpc(ref bufferWriter, 248132445u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			SetElevatorMusicClientRpc(setOn);
		}
	}

	[ClientRpc]
	public void SetElevatorMusicClientRpc(bool setOn)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(2139513831u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in setOn, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 2139513831u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer)
			{
				playMusic = setOn;
			}
		}
	}

	private void OnEnable()
	{
		RoundManager.Instance.currentMineshaftElevator = this;
	}

	private void OnDisable()
	{
		RoundManager.Instance.currentMineshaftElevator = null;
	}

	public void LateUpdate()
	{
		previousElevatorPosition = elevatorInsidePoint.position;
	}

	public void Update()
	{
		if (!playMusic)
		{
			if (stopPlayingMusicTimer <= 0f)
			{
				if (elevatorJingleMusic.isPlaying)
				{
					if (elevatorJingleMusic.pitch < 0.5f)
					{
						elevatorJingleMusic.volume -= Time.deltaTime * 3f;
						if (elevatorJingleMusic.volume <= 0.01f)
						{
							elevatorJingleMusic.Stop();
						}
					}
					else
					{
						elevatorJingleMusic.pitch -= Time.deltaTime;
						elevatorJingleMusic.volume = Mathf.Max(elevatorJingleMusic.volume - Time.deltaTime * 2f, 0.4f);
					}
				}
			}
			else
			{
				stopPlayingMusicTimer -= Time.deltaTime;
			}
		}
		else
		{
			stopPlayingMusicTimer = 1.5f;
			if (!elevatorJingleMusic.isPlaying)
			{
				if (elevatorMovingDown)
				{
					elevatorJingleMusic.Play();
					elevatorJingleMusic.volume = 1f;
				}
				else
				{
					elevatorJingleMusic.Play();
					elevatorJingleMusic.volume = 1f;
				}
			}
			elevatorJingleMusic.pitch = Mathf.Clamp(elevatorJingleMusic.pitch += Time.deltaTime * 2f, 0.3f, 1f);
		}
		elevatorAnimator.SetBool("ElevatorGoingUp", !elevatorMovingDown);
		elevatorCalledTopButton.SetActive(!elevatorMovingDown || elevatorCalled);
		elevatorCalledBottomButton.SetActive(elevatorMovingDown || elevatorCalled);
		if (elevatorMovingDown != movingDownLastFrame)
		{
			movingDownLastFrame = elevatorMovingDown;
			if (elevatorMovingDown)
			{
				elevatorAudio.PlayOneShot(elevatorStartDownSFX);
			}
			else
			{
				elevatorAudio.PlayOneShot(elevatorStartUpSFX);
			}
			if (base.IsServer)
			{
				SetElevatorMovingServerRpc(elevatorMovingDown);
			}
		}
		if (!base.IsServer)
		{
			return;
		}
		if (elevatorFinishedMoving)
		{
			if (base.IsServer && startedMusic)
			{
				playMusic = false;
				startedMusic = false;
				SetElevatorMusicServerRpc(setOn: false);
			}
		}
		else if (base.IsServer && !startedMusic)
		{
			startedMusic = true;
			playMusic = true;
			SetElevatorMusicServerRpc(setOn: true);
		}
		if (elevatorFinishedMoving)
		{
			if (elevatorCalled)
			{
				if (callCooldown <= 0f)
				{
					SwitchElevatorDirection();
					SetElevatorCalledClientRpc(setCalled: false, elevatorMovingDown);
				}
				else
				{
					callCooldown -= Time.deltaTime;
				}
			}
		}
		else if (elevatorFinishTimer <= 0f)
		{
			elevatorFinishedMoving = true;
			Debug.Log("Elevator finished moving!");
			PlayFinishAudio(!elevatorMovingDown);
			ElevatorFinishServerRpc(!elevatorMovingDown);
		}
		else
		{
			elevatorFinishTimer -= Time.deltaTime;
		}
	}

	private void SwitchElevatorDirection()
	{
		elevatorMovingDown = !elevatorMovingDown;
		elevatorFinishedMoving = false;
		elevatorFinishTimer = 14f;
		elevatorCalled = false;
		SetElevatorFinishedMovingClientRpc(finished: false);
	}

	[ClientRpc]
	public void SetElevatorFinishedMovingClientRpc(bool finished)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(3289543899u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in finished, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 3289543899u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer)
			{
				elevatorFinishedMoving = finished;
			}
		}
	}

	public void AnimationEvent_ElevatorFinishTop()
	{
		if (!elevatorMovingDown && !elevatorFinishedMoving)
		{
			elevatorFinishedMoving = true;
			if (base.IsServer)
			{
				PlayFinishAudio(!elevatorMovingDown);
				ElevatorFinishServerRpc(!elevatorMovingDown);
			}
		}
	}

	public void AnimationEvent_ElevatorStartFromBottom()
	{
		ShakePlayerCamera(shakeHard: false);
	}

	public void AnimationEvent_ElevatorHitBottom()
	{
		ShakePlayerCamera(shakeHard: true);
	}

	public void AnimationEvent_ElevatorTravel()
	{
		elevatorAudio.PlayOneShot(elevatorTravelSFX);
	}

	public void AnimationEvent_ElevatorFinishBottom()
	{
		if (elevatorMovingDown && !elevatorFinishedMoving)
		{
			elevatorFinishedMoving = true;
			if (base.IsServer)
			{
				Debug.Log("Elevator finished moving B!");
				PlayFinishAudio(!elevatorMovingDown);
				ElevatorFinishServerRpc(!elevatorMovingDown);
			}
		}
	}

	private void ShakePlayerCamera(bool shakeHard)
	{
		if (Vector3.Distance(StartOfRound.Instance.audioListener.transform.position, elevatorPoint.position) < 4f)
		{
			if (shakeHard)
			{
				HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
			}
			else
			{
				HUDManager.Instance.ShakeCamera(ScreenShakeType.Long);
			}
		}
	}

	[ServerRpc]
	public void ElevatorFinishServerRpc(bool atTop)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
		{
			if (base.OwnerClientId != networkManager.LocalClientId)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			ServerRpcParams serverRpcParams = default(ServerRpcParams);
			FastBufferWriter bufferWriter = __beginSendServerRpc(1003104612u, serverRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in atTop, default(FastBufferWriter.ForPrimitives));
			__endSendServerRpc(ref bufferWriter, 1003104612u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			ElevatorFinishClientRpc(atTop);
		}
	}

	[ClientRpc]
	public void ElevatorFinishClientRpc(bool atTop)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(3032205731u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in atTop, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 3032205731u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer)
			{
				PlayFinishAudio(atTop);
				elevatorFinishedMoving = true;
			}
		}
	}

	private void PlayFinishAudio(bool atTop)
	{
		if (atTop)
		{
			elevatorAudio.PlayOneShot(elevatorFinishUpSFX);
		}
		else
		{
			elevatorAudio.PlayOneShot(elevatorFinishDownSFX);
		}
	}

	[ServerRpc]
	public void SetElevatorMovingServerRpc(bool movingDown)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
		{
			if (base.OwnerClientId != networkManager.LocalClientId)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			ServerRpcParams serverRpcParams = default(ServerRpcParams);
			FastBufferWriter bufferWriter = __beginSendServerRpc(2622819109u, serverRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in movingDown, default(FastBufferWriter.ForPrimitives));
			__endSendServerRpc(ref bufferWriter, 2622819109u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			SetElevatorMovingClientRpc(movingDown);
		}
	}

	[ClientRpc]
	public void SetElevatorMovingClientRpc(bool movingDown)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(316684258u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in movingDown, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 316684258u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer)
			{
				elevatorMovingDown = movingDown;
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void CallElevatorServerRpc(bool callDown)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(816247597u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in callDown, default(FastBufferWriter.ForPrimitives));
				__endSendServerRpc(ref bufferWriter, 816247597u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				CallElevatorOnServer(callDown);
			}
		}
	}

	public void CallElevatorOnServer(bool callDown)
	{
		if (elevatorMovingDown != callDown)
		{
			elevatorCalled = true;
			callCooldown = 4f;
			SetElevatorCalledClientRpc(elevatorCalled, elevatorMovingDown);
		}
	}

	public void SetElevatorDoorOpen()
	{
		elevatorDoorOpen = true;
	}

	public void SetElevatorDoorClosed()
	{
		elevatorDoorOpen = false;
	}

	[ClientRpc]
	public void SetElevatorCalledClientRpc(bool setCalled, bool elevatorDown)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(2906284547u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in setCalled, default(FastBufferWriter.ForPrimitives));
				bufferWriter.WriteValueSafe(in elevatorDown, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 2906284547u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer)
			{
				elevatorCalled = setCalled;
				elevatorMovingDown = elevatorDown;
			}
		}
	}

	public void CallElevator(bool callDown)
	{
		Debug.Log($"Call elevator 0; call down: {callDown}; elevator moving down: {elevatorMovingDown}");
		CallElevatorServerRpc(callDown);
	}

	[ServerRpc(RequireOwnership = false)]
	public void PressElevatorButtonServerRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
			{
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(3485555877u, serverRpcParams, RpcDelivery.Reliable);
				__endSendServerRpc(ref bufferWriter, 3485555877u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				PressElevatorButtonOnServer();
			}
		}
	}

	public void PressElevatorButtonOnServer(bool requireFinishedMoving = false)
	{
		if (elevatorFinishedMoving || (elevatorFinishTimer < 0.16f && !requireFinishedMoving))
		{
			SwitchElevatorDirection();
		}
	}

	public void PressElevatorButton()
	{
		PressElevatorButtonServerRpc();
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_MineshaftElevatorController()
	{
		NetworkManager.__rpc_func_table.Add(248132445u, __rpc_handler_248132445);
		NetworkManager.__rpc_func_table.Add(2139513831u, __rpc_handler_2139513831);
		NetworkManager.__rpc_func_table.Add(3289543899u, __rpc_handler_3289543899);
		NetworkManager.__rpc_func_table.Add(1003104612u, __rpc_handler_1003104612);
		NetworkManager.__rpc_func_table.Add(3032205731u, __rpc_handler_3032205731);
		NetworkManager.__rpc_func_table.Add(2622819109u, __rpc_handler_2622819109);
		NetworkManager.__rpc_func_table.Add(316684258u, __rpc_handler_316684258);
		NetworkManager.__rpc_func_table.Add(816247597u, __rpc_handler_816247597);
		NetworkManager.__rpc_func_table.Add(2906284547u, __rpc_handler_2906284547);
		NetworkManager.__rpc_func_table.Add(3485555877u, __rpc_handler_3485555877);
	}

	private static void __rpc_handler_248132445(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
		{
			if (networkManager.LogLevel <= LogLevel.Normal)
			{
				Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
			}
		}
		else
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((MineshaftElevatorController)target).SetElevatorMusicServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2139513831(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((MineshaftElevatorController)target).SetElevatorMusicClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3289543899(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((MineshaftElevatorController)target).SetElevatorFinishedMovingClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_1003104612(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
		{
			if (networkManager.LogLevel <= LogLevel.Normal)
			{
				Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
			}
		}
		else
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((MineshaftElevatorController)target).ElevatorFinishServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3032205731(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((MineshaftElevatorController)target).ElevatorFinishClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2622819109(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
		{
			if (networkManager.LogLevel <= LogLevel.Normal)
			{
				Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
			}
		}
		else
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((MineshaftElevatorController)target).SetElevatorMovingServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_316684258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((MineshaftElevatorController)target).SetElevatorMovingClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_816247597(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((MineshaftElevatorController)target).CallElevatorServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2906284547(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			reader.ReadValueSafe(out bool value2, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((MineshaftElevatorController)target).SetElevatorCalledClientRpc(value, value2);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_3485555877(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((MineshaftElevatorController)target).PressElevatorButtonServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "MineshaftElevatorController";
	}
}
