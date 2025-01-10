using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

public class Shovel : GrabbableObject
{
	public int shovelHitForce = 1;

	public bool reelingUp;

	public bool isHoldingButton;

	private RaycastHit rayHit;

	private Coroutine reelingUpCoroutine;

	private RaycastHit[] objectsHitByShovel;

	private List<RaycastHit> objectsHitByShovelList = new List<RaycastHit>();

	public AudioClip reelUp;

	public AudioClip swing;

	public AudioClip[] hitSFX;

	public AudioSource shovelAudio;

	private PlayerControllerB previousPlayerHeldBy;

	private int shovelMask = 1084754248;

	public override void ItemActivate(bool used, bool buttonDown = true)
	{
		if (playerHeldBy == null)
		{
			return;
		}
		isHoldingButton = buttonDown;
		if (!reelingUp && buttonDown)
		{
			reelingUp = true;
			previousPlayerHeldBy = playerHeldBy;
			if (reelingUpCoroutine != null)
			{
				StopCoroutine(reelingUpCoroutine);
			}
			reelingUpCoroutine = StartCoroutine(reelUpShovel());
		}
	}

	private IEnumerator reelUpShovel()
	{
		playerHeldBy.activatingItem = true;
		playerHeldBy.twoHanded = true;
		playerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
		playerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: true);
		shovelAudio.PlayOneShot(reelUp);
		ReelUpSFXServerRpc();
		yield return new WaitForSeconds(0.35f);
		yield return new WaitUntil(() => !isHoldingButton || !isHeld);
		SwingShovel(!isHeld);
		yield return new WaitForSeconds(0.13f);
		yield return new WaitForEndOfFrame();
		HitShovel(!isHeld);
		yield return new WaitForSeconds(0.3f);
		reelingUp = false;
		reelingUpCoroutine = null;
	}

	[ServerRpc]
	public void ReelUpSFXServerRpc()
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
			FastBufferWriter bufferWriter = __beginSendServerRpc(4113335123u, serverRpcParams, RpcDelivery.Reliable);
			__endSendServerRpc(ref bufferWriter, 4113335123u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			ReelUpSFXClientRpc();
		}
	}

	[ClientRpc]
	public void ReelUpSFXClientRpc()
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				ClientRpcParams clientRpcParams = default(ClientRpcParams);
				FastBufferWriter bufferWriter = __beginSendClientRpc(2042054613u, clientRpcParams, RpcDelivery.Reliable);
				__endSendClientRpc(ref bufferWriter, 2042054613u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsOwner)
			{
				shovelAudio.PlayOneShot(reelUp);
			}
		}
	}

	public override void DiscardItem()
	{
		if (playerHeldBy != null)
		{
			playerHeldBy.activatingItem = false;
		}
		base.DiscardItem();
	}

	public void SwingShovel(bool cancel = false)
	{
		previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: false);
		if (!cancel)
		{
			shovelAudio.PlayOneShot(swing);
			previousPlayerHeldBy.UpdateSpecialAnimationValue(specialAnimation: true, (short)previousPlayerHeldBy.transform.localEulerAngles.y, 0.4f);
		}
	}

	public void HitShovel(bool cancel = false)
	{
		if (previousPlayerHeldBy == null)
		{
			Debug.LogError("Previousplayerheldby is null on this client when HitShovel is called.");
			return;
		}
		previousPlayerHeldBy.activatingItem = false;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int num = -1;
		if (!cancel)
		{
			previousPlayerHeldBy.twoHanded = false;
			objectsHitByShovel = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, 0.8f, previousPlayerHeldBy.gameplayCamera.transform.forward, 1.5f, shovelMask, QueryTriggerInteraction.Collide);
			objectsHitByShovelList = objectsHitByShovel.OrderBy((RaycastHit x) => x.distance).ToList();
			List<EnemyAI> list = new List<EnemyAI>();
			for (int i = 0; i < objectsHitByShovelList.Count; i++)
			{
				if (objectsHitByShovelList[i].transform.gameObject.layer == 8 || objectsHitByShovelList[i].transform.gameObject.layer == 11)
				{
					if (objectsHitByShovelList[i].collider.isTrigger)
					{
						continue;
					}
					flag = true;
					string text = objectsHitByShovelList[i].collider.gameObject.tag;
					for (int j = 0; j < StartOfRound.Instance.footstepSurfaces.Length; j++)
					{
						if (StartOfRound.Instance.footstepSurfaces[j].surfaceTag == text)
						{
							num = j;
							break;
						}
					}
				}
				else
				{
					if (!objectsHitByShovelList[i].transform.TryGetComponent<IHittable>(out var component) || objectsHitByShovelList[i].transform == previousPlayerHeldBy.transform || (!(objectsHitByShovelList[i].point == Vector3.zero) && Physics.Linecast(previousPlayerHeldBy.gameplayCamera.transform.position, objectsHitByShovelList[i].point, out var _, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)))
					{
						continue;
					}
					flag = true;
					Vector3 forward = previousPlayerHeldBy.gameplayCamera.transform.forward;
					try
					{
						EnemyAICollisionDetect component2 = objectsHitByShovelList[i].transform.GetComponent<EnemyAICollisionDetect>();
						if (component2 != null)
						{
							if (!(component2.mainScript == null) && !list.Contains(component2.mainScript))
							{
								goto IL_02ff;
							}
							continue;
						}
						if (!(objectsHitByShovelList[i].transform.GetComponent<PlayerControllerB>() != null))
						{
							goto IL_02ff;
						}
						if (!flag3)
						{
							flag3 = true;
							goto IL_02ff;
						}
						goto end_IL_0288;
						IL_02ff:
						bool flag4 = component.Hit(shovelHitForce, forward, previousPlayerHeldBy, playHitSFX: true, 1);
						if (flag4 && component2 != null)
						{
							list.Add(component2.mainScript);
						}
						if (!flag2)
						{
							flag2 = flag4;
						}
						end_IL_0288:;
					}
					catch (Exception arg)
					{
						Debug.Log($"Exception caught when hitting object with shovel from player #{previousPlayerHeldBy.playerClientId}: {arg}");
					}
				}
			}
		}
		if (flag)
		{
			RoundManager.PlayRandomClip(shovelAudio, hitSFX);
			UnityEngine.Object.FindObjectOfType<RoundManager>().PlayAudibleNoise(base.transform.position, 17f, 0.8f);
			if (!flag2 && num != -1)
			{
				shovelAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);
				WalkieTalkie.TransmitOneShotAudio(shovelAudio, StartOfRound.Instance.footstepSurfaces[num].hitSurfaceSFX);
			}
			playerHeldBy.playerBodyAnimator.SetTrigger("shovelHit");
			HitShovelServerRpc(num);
		}
	}

	[ServerRpc]
	public void HitShovelServerRpc(int hitSurfaceID)
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
			FastBufferWriter bufferWriter = __beginSendServerRpc(2096026133u, serverRpcParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(bufferWriter, hitSurfaceID);
			__endSendServerRpc(ref bufferWriter, 2096026133u, serverRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
		{
			HitShovelClientRpc(hitSurfaceID);
		}
	}

	[ClientRpc]
	public void HitShovelClientRpc(int hitSurfaceID)
	{
		NetworkManager networkManager = base.NetworkManager;
		if ((object)networkManager == null || !networkManager.IsListening)
		{
			return;
		}
		if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
		{
			ClientRpcParams clientRpcParams = default(ClientRpcParams);
			FastBufferWriter bufferWriter = __beginSendClientRpc(275435223u, clientRpcParams, RpcDelivery.Reliable);
			BytePacker.WriteValueBitPacked(bufferWriter, hitSurfaceID);
			__endSendClientRpc(ref bufferWriter, 275435223u, clientRpcParams, RpcDelivery.Reliable);
		}
		if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost) && !base.IsOwner)
		{
			RoundManager.PlayRandomClip(shovelAudio, hitSFX);
			if (hitSurfaceID != -1)
			{
				HitSurfaceWithShovel(hitSurfaceID);
			}
		}
	}

	private void HitSurfaceWithShovel(int hitSurfaceID)
	{
		shovelAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
		WalkieTalkie.TransmitOneShotAudio(shovelAudio, StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	[RuntimeInitializeOnLoadMethod]
	internal static void InitializeRPCS_Shovel()
	{
		NetworkManager.__rpc_func_table.Add(4113335123u, __rpc_handler_4113335123);
		NetworkManager.__rpc_func_table.Add(2042054613u, __rpc_handler_2042054613);
		NetworkManager.__rpc_func_table.Add(2096026133u, __rpc_handler_2096026133);
		NetworkManager.__rpc_func_table.Add(275435223u, __rpc_handler_275435223);
	}

	private static void __rpc_handler_4113335123(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((Shovel)target).ReelUpSFXServerRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2042054613(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((Shovel)target).ReelUpSFXClientRpc();
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_2096026133(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((Shovel)target).HitShovelServerRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	private static void __rpc_handler_275435223(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
	{
		NetworkManager networkManager = target.NetworkManager;
		if ((object)networkManager != null && networkManager.IsListening)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			target.__rpc_exec_stage = __RpcExecStage.Client;
			((Shovel)target).HitShovelClientRpc(value);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}
	}

	protected internal override string __getTypeName()
	{
		return "Shovel";
	}
}
