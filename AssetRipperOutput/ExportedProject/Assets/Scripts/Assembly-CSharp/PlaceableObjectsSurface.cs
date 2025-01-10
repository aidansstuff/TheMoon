using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

public class PlaceableObjectsSurface : NetworkBehaviour
{
	public NetworkObject parentTo;

	public Collider placeableBounds;

	public InteractTrigger triggerScript;

	private float checkHoverTipInterval;

	private void Update()
	{
		if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
		{
			triggerScript.interactable = GameNetworkManager.Instance.localPlayerController.isHoldingObject;
		}
	}

	public void PlaceObject(PlayerControllerB playerWhoTriggered)
	{
		if (!playerWhoTriggered.isHoldingObject || playerWhoTriggered.isGrabbingObjectAnimation || !(playerWhoTriggered.currentlyHeldObjectServer != null))
		{
			return;
		}
		Debug.Log("Placing object in storage");
		Vector3 vector = itemPlacementPosition(playerWhoTriggered.gameplayCamera.transform, playerWhoTriggered.currentlyHeldObjectServer);
		if (!(vector == Vector3.zero))
		{
			if (parentTo != null)
			{
				vector = parentTo.transform.InverseTransformPoint(vector);
			}
			playerWhoTriggered.DiscardHeldObject(placeObject: true, parentTo, vector, matchRotationOfParent: false);
			Debug.Log("discard held object called from placeobject");
		}
	}

	private Vector3 itemPlacementPosition(Transform gameplayCamera, GrabbableObject heldObject)
	{
		if (Physics.Raycast(gameplayCamera.position, gameplayCamera.forward, out var hitInfo, 7f, 1073744640, QueryTriggerInteraction.Ignore))
		{
			if (placeableBounds.ClosestPoint(hitInfo.point) == hitInfo.point)
			{
				return hitInfo.point + placeableBounds.transform.up * heldObject.itemProperties.verticalOffset;
			}
			return placeableBounds.ClosestPoint(hitInfo.point) + placeableBounds.transform.up * heldObject.itemProperties.verticalOffset;
		}
		return Vector3.zero;
	}

	protected override void __initializeVariables()
	{
		base.__initializeVariables();
	}

	protected internal override string __getTypeName()
	{
		return "PlaceableObjectsSurface";
	}
}
