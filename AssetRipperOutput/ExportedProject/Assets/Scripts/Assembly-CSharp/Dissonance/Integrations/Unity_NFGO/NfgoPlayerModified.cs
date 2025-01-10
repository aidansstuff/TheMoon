using System;
using GameNetcodeStuff;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Dissonance.Integrations.Unity_NFGO
{
	[RequireComponent(typeof(NetworkObject))]
	public class NfgoPlayerModified : NetworkBehaviour, IDissonancePlayer
	{
		private static readonly Log Log = Logs.Create(LogCategory.Network, "NfgoPlayer");

		private DissonanceComms _comms;

		private Transform _transform;

		private string _playerIdString;

		private readonly NetworkVariable<FixedString128Bytes> _playerId = new NetworkVariable<FixedString128Bytes>(new FixedString128Bytes(""));

		private bool hasStartedTracking;

		[NotNull]
		private Transform Transform
		{
			get
			{
				if (_transform == null)
				{
					_transform = base.transform;
				}
				return _transform;
			}
		}

		public Vector3 Position => Transform.position;

		public Quaternion Rotation => Transform.rotation;

		public bool IsTracking { get; private set; }

		public string PlayerId
		{
			get
			{
				if (_playerIdString == null || !_playerId.Value.Equals(_playerIdString))
				{
					_playerIdString = _playerId.Value.ToString();
				}
				return _playerIdString;
			}
		}

		public NetworkPlayerType Type
		{
			get
			{
				if (_comms == null || _playerId.Value.IsEmpty)
				{
					return NetworkPlayerType.Unknown;
				}
				if (!_playerId.Value.Equals(_comms.LocalPlayerName))
				{
					return NetworkPlayerType.Remote;
				}
				return NetworkPlayerType.Local;
			}
		}

		public override void OnDestroy()
		{
			if (_comms != null)
			{
				_comms.LocalPlayerNameChanged -= OnLocalPlayerIdChanged;
			}
			NetworkVariable<FixedString128Bytes> playerId = _playerId;
			playerId.OnValueChanged = (NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate)Delegate.Remove(playerId.OnValueChanged, new NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate(OnNetworkVariablePlayerIdChanged));
		}

		public void VoiceChatTrackingStart()
		{
			_comms = UnityEngine.Object.FindObjectOfType<DissonanceComms>();
			if (_comms == null)
			{
				throw Log.CreateUserErrorException("cannot find DissonanceComms component in scene", "not placing a DissonanceComms component on a game object in the scene", "https://placeholder-software.co.uk/dissonance/docs/Basics/Quick-Start-UNet-HLAPI.html", "A6A291D8-5B53-417E-95CD-EC670637C532");
			}
			if (!hasStartedTracking)
			{
				NetworkVariable<FixedString128Bytes> playerId = _playerId;
				playerId.OnValueChanged = (NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate)Delegate.Combine(playerId.OnValueChanged, new NetworkVariable<FixedString128Bytes>.OnValueChangedDelegate(OnNetworkVariablePlayerIdChanged));
			}
			if (base.gameObject.GetComponent<PlayerControllerB>().isPlayerControlled && base.IsOwner)
			{
				if (_comms.LocalPlayerName != null)
				{
					SetNameServerRpc(_comms.LocalPlayerName);
				}
				if (!hasStartedTracking)
				{
					_comms.LocalPlayerNameChanged += OnLocalPlayerIdChanged;
				}
			}
			else if (!_playerId.Value.IsEmpty)
			{
				StartTracking();
			}
			hasStartedTracking = true;
		}

		[ServerRpc]
		public void SetNameServerRpc(string playerName)
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
					if (networkManager.LogLevel <= Unity.Netcode.LogLevel.Normal)
					{
						Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
					}
					return;
				}
				ServerRpcParams serverRpcParams = default(ServerRpcParams);
				FastBufferWriter bufferWriter = __beginSendServerRpc(2623869394u, serverRpcParams, RpcDelivery.Reliable);
				bool value = playerName != null;
				bufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				if (value)
				{
					bufferWriter.WriteValueSafe(playerName);
				}
				__endSendServerRpc(ref bufferWriter, 2623869394u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				_playerId.Value = playerName;
			}
		}

		private void OnLocalPlayerIdChanged(string _)
		{
			if (IsTracking)
			{
				StopTracking();
			}
			if (base.gameObject.GetComponent<PlayerControllerB>().isPlayerControlled && base.IsOwner)
			{
				SetNameServerRpc(_comms.LocalPlayerName);
			}
			StartTracking();
		}

		private void OnNetworkVariablePlayerIdChanged<T>(T previousvalue, T newvalue)
		{
			if (IsTracking)
			{
				StopTracking();
			}
			StartTracking();
		}

		private void StartTracking()
		{
			if (IsTracking)
			{
				throw Log.CreatePossibleBugException("Attempting to start player tracking, but tracking is already started", "4C2E74AA-CA09-4F98-B820-F2518A4E87D2");
			}
			if (_comms != null)
			{
				_comms.TrackPlayerPosition(this);
				IsTracking = true;
			}
		}

		private void StopTracking()
		{
			if (!IsTracking)
			{
				throw Log.CreatePossibleBugException("Attempting to stop player tracking, but tracking is not started", "BF8542EB-C13E-46FA-A8A0-B162F188BBA3");
			}
			if (_comms != null)
			{
				_comms.StopTracking(this);
				IsTracking = false;
			}
		}

		protected override void __initializeVariables()
		{
			if (_playerId == null)
			{
				throw new Exception("NfgoPlayerModified._playerId cannot be null. All NetworkVariableBase instances must be initialized.");
			}
			_playerId.Initialize(this);
			__nameNetworkVariable(_playerId, "_playerId");
			NetworkVariableFields.Add(_playerId);
			base.__initializeVariables();
		}

		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeRPCS_NfgoPlayerModified()
		{
			NetworkManager.__rpc_func_table.Add(2623869394u, __rpc_handler_2623869394);
		}

		private static void __rpc_handler_2623869394(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if ((object)networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (rpcParams.Server.Receive.SenderClientId != target.OwnerClientId)
			{
				if (networkManager.LogLevel <= Unity.Netcode.LogLevel.Normal)
				{
					Debug.LogError("Only the owner can invoke a ServerRpc that requires ownership!");
				}
				return;
			}
			reader.ReadValueSafe(out bool value, default(FastBufferWriter.ForPrimitives));
			string s = null;
			if (value)
			{
				reader.ReadValueSafe(out s, oneByteChars: false);
			}
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((NfgoPlayerModified)target).SetNameServerRpc(s);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}

		protected internal override string __getTypeName()
		{
			return "NfgoPlayerModified";
		}
	}
}
