using System;
using Unity.Netcode;
using UnityEngine;

public class SoccerBallProp : GrabbableObject
{
	[Space(5f)]
	public float ballHitUpwardAmount = 0.5f;

	public AnimationCurve grenadeFallCurve;

	public AnimationCurve grenadeVerticalFallCurve;

	public AnimationCurve soccerBallVerticalOffset;

	public AnimationCurve grenadeVerticalFallCurveNoBounce;

	private Ray soccerRay;

	private RaycastHit soccerHit;

	private int soccerBallMask = 369101057;

	private int previousPlayerHit;

	private float hitTimer;

	public AudioClip[] hitBallSFX;

	public AudioClip[] ballHitFloorSFX;

	public AudioSource soccerBallAudio;

	public Transform ballCollider;

	public override void Start()
	{
		base.Start();
		if (new System.Random(StartOfRound.Instance.randomMapSeed + 51).Next(0, 100) < 15)
		{
			ballCollider.localScale *= 1.56f;
		}
	}

	public override void ActivatePhysicsTrigger(Collider other)
	{
		if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy")) && !Physics.Linecast(other.gameObject.transform.position + Vector3.up, base.transform.position + Vector3.up * 0.5f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
		{
			BeginKickBall(other.gameObject.transform.position + Vector3.up, other.gameObject.CompareTag("Enemy"));
		}
	}

	public Vector3 GetSoccerKickDestination(Vector3 hitFromPosition)
	{
		Vector3 vector = base.transform.position;
		Vector3 value = (base.transform.position - hitFromPosition) * 1000f;
		value = Vector3.Normalize(value);
		value.y = 0.15f;
		soccerRay = new Ray(base.transform.position + Vector3.up * 0.22f, value);
		Debug.DrawRay(base.transform.position, value, Color.yellow, 15f);
		bool flag = false;
		if (Physics.Raycast(soccerRay, out soccerHit, 12f, soccerBallMask, QueryTriggerInteraction.Ignore) && soccerHit.collider.gameObject.layer == 28)
		{
			flag = true;
			vector = ((!(soccerHit.distance < 2f)) ? soccerRay.GetPoint(soccerHit.distance - 0.05f) : (soccerRay.GetPoint(soccerHit.distance - 0.05f) + soccerHit.normal * (soccerHit.distance * 2f)));
		}
		Debug.DrawRay(base.transform.position, value, Color.blue, 15f);
		if (!flag)
		{
			value.y = ballHitUpwardAmount;
			vector = ((!Physics.Raycast(soccerRay, out soccerHit, 12f, soccerBallMask, QueryTriggerInteraction.Ignore)) ? soccerRay.GetPoint(10f) : ((!(soccerHit.distance < 2f)) ? soccerRay.GetPoint(soccerHit.distance - 0.05f) : (soccerRay.GetPoint(soccerHit.distance - 0.05f) + soccerHit.normal * (soccerHit.distance * 2f))));
		}
		Debug.DrawRay(vector, Vector3.down, Color.blue, 15f);
		soccerRay = new Ray(vector, Vector3.down);
		if (Physics.Raycast(soccerRay, out soccerHit, 65f, soccerBallMask, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector2 = soccerHit.point + Vector3.up * itemProperties.verticalOffset;
			Debug.DrawRay(vector2, Vector3.up * 0.5f, Color.green);
			return vector2;
		}
		return Vector3.zero;
	}

	public void BeginKickBall(Vector3 hitFromPosition, bool hitByEnemy)
	{
		if ((hitByEnemy && !base.IsServer) || isHeld || parentObject != null || (base.transform.parent != StartOfRound.Instance.elevatorTransform && base.transform.parent != RoundManager.Instance.spawnedScrapContainer && base.transform.parent != StartOfRound.Instance.propsContainer) || (previousPlayerHit == (int)GameNetworkManager.Instance.localPlayerController.playerClientId && Time.realtimeSinceStartup - hitTimer < 0.35f))
		{
			return;
		}
		hitTimer = Time.realtimeSinceStartup;
		previousPlayerHit = (int)GameNetworkManager.Instance.localPlayerController.playerClientId;
		Vector3 soccerKickDestination = GetSoccerKickDestination(hitFromPosition);
		if (!(soccerKickDestination == Vector3.zero))
		{
			bool setInShipRoom = false;
			bool setInElevator;
			if (!StartOfRound.Instance.shipBounds.bounds.Contains(soccerKickDestination))
			{
				setInElevator = false;
				soccerKickDestination = StartOfRound.Instance.propsContainer.InverseTransformPoint(soccerKickDestination);
			}
			else
			{
				setInElevator = true;
				setInShipRoom = StartOfRound.Instance.shipInnerRoomBounds.bounds.Contains(soccerKickDestination);
				soccerKickDestination = StartOfRound.Instance.elevatorTransform.InverseTransformPoint(soccerKickDestination);
			}
			KickBallLocalClient(soccerKickDestination, setInElevator, setInShipRoom);
			KickBallServerRpc(soccerKickDestination, (int)GameNetworkManager.Instance.localPlayerController.playerClientId, setInElevator, setInShipRoom);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void KickBallServerRpc(Vector3 dest, int playerWhoKicked, bool setInElevator, bool setInShipRoom)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Server && (networkManager.IsClient || networkManager.IsHost))
		{
			ServerRpcParams serverRpcParams = default(ServerRpcParams);
			FastBufferWriter bufferWriter = __beginSendServerRpc(1444407036u, serverRpcParams, RpcDelivery.Reliable);
			bufferWriter.WriteValueSafe(in dest);
			BytePacker.WriteValueBitPacked(bufferWriter, playerWhoKicked);
			bufferWriter.WriteValueSafe(in setInElevator, default(FastBufferWriter.ForPrimitives));
			bufferWriter.WriteValueSafe(in setInShipRoom, default(FastBufferWriter.ForPrimitives));
			__endSendServerRpc(ref bufferWriter, 1444407036u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			if (playerWhoKicked != (int)GameNetworkManager.Instance.localPlayerController.playerClientId)
			{
				KickBallLocalClient(dest, setInElevator, setInShipRoom);
				previousPlayerHit = playerWhoKicked;
			}
			KickBallClientRpc(dest, playerWhoKicked, setInElevator, setInShipRoom);
		}
	}

	[ClientRpc]
	public void KickBallClientRpc(Vector3 dest, int playerWhoKicked, bool setInElevator, bool setInShipRoom)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(1833449416u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in dest);
				BytePacker.WriteValueBitPacked(bufferWriter, playerWhoKicked);
				bufferWriter.WriteValueSafe(in setInElevator, default(FastBufferWriter.ForPrimitives));
				bufferWriter.WriteValueSafe(in setInShipRoom, default(FastBufferWriter.ForPrimitives));
				__endSendClientRpc(ref bufferWriter, 1833449416u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsServer && playerWhoKicked != (int)GameNetworkManager.Instance.localPlayerController.playerClientId)
			{
				previousPlayerHit = playerWhoKicked;
				KickBallLocalClient(dest, setInElevator, setInShipRoom);
			}
		}
	}

	private void KickBallLocalClient(Vector3 destinationPos, bool setInElevator, bool setInShipRoom)
	{
		RoundManager.PlayRandomClip(soccerBallAudio, hitBallSFX, randomize: true, 1f, 10419);
		fallTime = 0f;
		hasHitGround = false;
		if (setInElevator)
		{
			base.transform.SetParent(StartOfRound.Instance.elevatorTransform, worldPositionStays: true);
		}
		else
		{
			base.transform.SetParent(StartOfRound.Instance.propsContainer, worldPositionStays: true);
		}
		GameNetworkManager.Instance.localPlayerController.SetItemInElevator(setInElevator, setInShipRoom, this);
		startFallingPosition = base.transform.parent.InverseTransformPoint(base.transform.position + Vector3.up * 0.07f);
		targetFloorPosition = destinationPos;
	}

	public override void FallWithCurve()
	{
		float magnitude = (startFallingPosition - targetFloorPosition).magnitude;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, base.transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
		base.transform.localPosition = Vector3.Lerp(startFallingPosition, targetFloorPosition, grenadeFallCurve.Evaluate(fallTime));
		if (magnitude < 3f)
		{
			base.transform.localPosition = Vector3.Lerp(new Vector3(base.transform.localPosition.x, startFallingPosition.y, base.transform.localPosition.z), new Vector3(base.transform.localPosition.x, targetFloorPosition.y, base.transform.localPosition.z), grenadeVerticalFallCurveNoBounce.Evaluate(fallTime));
		}
		else
		{
			base.transform.localPosition = Vector3.Lerp(new Vector3(base.transform.localPosition.x, startFallingPosition.y, base.transform.localPosition.z), new Vector3(base.transform.localPosition.x, targetFloorPosition.y, base.transform.localPosition.z), grenadeVerticalFallCurve.Evaluate(fallTime));
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + soccerBallVerticalOffset.Evaluate(fallTime) * ballHitUpwardAmount, base.transform.localPosition.z);
		}
		fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
	}

	public override void PlayDropSFX()
	{
		if (itemProperties.dropSFX != null)
		{
			RoundManager.PlayRandomClip(soccerBallAudio, ballHitFloorSFX, randomize: true, 1f, 10419);
			WalkieTalkie.TransmitOneShotAudio(soccerBallAudio, itemProperties.dropSFX);
			if (base.IsOwner)
			{
				RoundManager.Instance.PlayAudibleNoise(base.transform.position, 15f, 0.85f, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed, 941);
			}
		}
		hasHitGround = true;
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_SoccerBallProp()
	{
		NetworkManager.__rpc_func_table.Add(1444407036u, __rpc_handler_1444407036);
		NetworkManager.__rpc_func_table.Add(1833449416u, __rpc_handler_1833449416);
	}

	private static void __rpc_handler_1444407036(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			ByteUnpacker.ReadValueBitPacked(reader, out int value2);
			reader.ReadValueSafe(out bool value3, default(FastBufferWriter.ForPrimitives));
			reader.ReadValueSafe(out bool value4, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((SoccerBallProp)target).KickBallServerRpc(value, value2, value3, value4);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_1833449416(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			reader.ReadValueSafe(out Vector3 value);
			ByteUnpacker.ReadValueBitPacked(reader, out int value2);
			reader.ReadValueSafe(out bool value3, default(FastBufferWriter.ForPrimitives));
			reader.ReadValueSafe(out bool value4, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((SoccerBallProp)target).KickBallClientRpc(value, value2, value3, value4);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "SoccerBallProp";
	}
}
