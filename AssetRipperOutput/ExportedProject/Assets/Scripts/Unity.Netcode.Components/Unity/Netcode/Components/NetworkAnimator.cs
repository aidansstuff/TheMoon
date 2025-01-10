using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Netcode.Components
{
	[AddComponentMenu("Netcode/Network Animator")]
	[RequireComponent(typeof(Animator))]
	public class NetworkAnimator : NetworkBehaviour, ISerializationCallbackReceiver
	{
		[Serializable]
		internal class TransitionStateinfo
		{
			public bool IsCrossFadeExit;

			public int Layer;

			public int OriginatingState;

			public int DestinationState;

			public float TransitionDuration;

			public int TriggerNameHash;

			public int TransitionIndex;
		}

		internal struct AnimationState : INetworkSerializable
		{
			internal bool HasBeenProcessed;

			internal int StateHash;

			internal float NormalizedTime;

			internal int Layer;

			internal float Weight;

			internal float Duration;

			internal bool Transition;

			internal bool CrossFade;

			private const byte k_IsTransition = 1;

			private const byte k_IsCrossFade = 2;

			private byte m_StateFlags;

			internal int DestinationStateHash;

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				if (serializer.IsWriter)
				{
					FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
					m_StateFlags = 0;
					if (Transition)
					{
						m_StateFlags |= 1;
					}
					if (CrossFade)
					{
						m_StateFlags |= 2;
					}
					serializer.SerializeValue(ref m_StateFlags);
					BytePacker.WriteValuePacked(fastBufferWriter, StateHash);
					BytePacker.WriteValuePacked(fastBufferWriter, Layer);
					if (Transition)
					{
						BytePacker.WriteValuePacked(fastBufferWriter, DestinationStateHash);
					}
				}
				else
				{
					FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
					serializer.SerializeValue(ref m_StateFlags);
					Transition = (m_StateFlags & 1) == 1;
					CrossFade = (m_StateFlags & 2) == 2;
					ByteUnpacker.ReadValuePacked(fastBufferReader, out StateHash);
					ByteUnpacker.ReadValuePacked(fastBufferReader, out Layer);
					if (Transition)
					{
						ByteUnpacker.ReadValuePacked(fastBufferReader, out DestinationStateHash);
					}
				}
				serializer.SerializeValue(ref NormalizedTime, default(FastBufferWriter.ForPrimitives));
				serializer.SerializeValue(ref Weight, default(FastBufferWriter.ForPrimitives));
				if (CrossFade)
				{
					serializer.SerializeValue(ref Duration, default(FastBufferWriter.ForPrimitives));
				}
			}
		}

		internal struct AnimationMessage : INetworkSerializable
		{
			internal bool HasBeenProcessed;

			internal List<AnimationState> AnimationStates;

			internal int IsDirtyCount;

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				AnimationState animationState = default(AnimationState);
				if (serializer.IsReader)
				{
					AnimationStates = new List<AnimationState>();
					serializer.SerializeValue(ref IsDirtyCount, default(FastBufferWriter.ForPrimitives));
					for (int i = 0; i < IsDirtyCount; i++)
					{
						animationState = default(AnimationState);
						serializer.SerializeValue(ref animationState, default(FastBufferWriter.ForNetworkSerializable));
						AnimationStates.Add(animationState);
					}
				}
				else
				{
					serializer.SerializeValue(ref IsDirtyCount, default(FastBufferWriter.ForPrimitives));
					for (int j = 0; j < IsDirtyCount; j++)
					{
						animationState = AnimationStates[j];
						serializer.SerializeNetworkSerializable(ref animationState);
					}
				}
			}
		}

		internal struct ParametersUpdateMessage : INetworkSerializable
		{
			internal byte[] Parameters;

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				serializer.SerializeValue(ref Parameters, default(FastBufferWriter.ForPrimitives));
			}
		}

		internal struct AnimationTriggerMessage : INetworkSerializable
		{
			internal int Hash;

			internal bool IsTriggerSet;

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				serializer.SerializeValue(ref Hash, default(FastBufferWriter.ForPrimitives));
				serializer.SerializeValue(ref IsTriggerSet, default(FastBufferWriter.ForPrimitives));
			}
		}

		private struct AnimatorParamCache
		{
			internal int Hash;

			internal int Type;

			internal unsafe fixed byte Value[4];
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		private struct AnimationParamEnumWrapper
		{
			internal static readonly int AnimatorControllerParameterInt;

			internal static readonly int AnimatorControllerParameterFloat;

			internal static readonly int AnimatorControllerParameterBool;

			internal static readonly int AnimatorControllerParameterTriggerBool;

			static AnimationParamEnumWrapper()
			{
				AnimatorControllerParameterInt = UnsafeUtility.EnumToInt(AnimatorControllerParameterType.Int);
				AnimatorControllerParameterFloat = UnsafeUtility.EnumToInt(AnimatorControllerParameterType.Float);
				AnimatorControllerParameterBool = UnsafeUtility.EnumToInt(AnimatorControllerParameterType.Bool);
				AnimatorControllerParameterTriggerBool = UnsafeUtility.EnumToInt(AnimatorControllerParameterType.Trigger);
			}
		}

		[HideInInspector]
		[SerializeField]
		internal List<TransitionStateinfo> TransitionStateInfoList;

		private Dictionary<int, Dictionary<int, TransitionStateinfo>> m_DestinationStateToTransitioninfo = new Dictionary<int, Dictionary<int, TransitionStateinfo>>();

		[SerializeField]
		private Animator m_Animator;

		private const int k_MaxAnimationParams = 32;

		private int[] m_TransitionHash;

		private int[] m_AnimationHash;

		private float[] m_LayerWeights;

		private static byte[] s_EmptyArray = new byte[0];

		private List<int> m_ParametersToUpdate;

		private List<ulong> m_ClientSendList;

		private ClientRpcParams m_ClientRpcParams;

		private AnimationMessage m_AnimationMessage;

		private NetworkAnimatorStateChangeHandler m_NetworkAnimatorStateChangeHandler;

		internal List<AnimatorStateInfo> SynchronizationStateInfo;

		private FastBufferWriter m_ParameterWriter = new FastBufferWriter(128, Allocator.Persistent);

		private NativeArray<AnimatorParamCache> m_CachedAnimatorParameters;

		public Animator Animator
		{
			get
			{
				return m_Animator;
			}
			set
			{
				m_Animator = value;
			}
		}

		private void BuildDestinationToTransitionInfoTable()
		{
			foreach (TransitionStateinfo transitionStateInfo in TransitionStateInfoList)
			{
				if (!m_DestinationStateToTransitioninfo.ContainsKey(transitionStateInfo.Layer))
				{
					m_DestinationStateToTransitioninfo.Add(transitionStateInfo.Layer, new Dictionary<int, TransitionStateinfo>());
				}
				Dictionary<int, TransitionStateinfo> dictionary = m_DestinationStateToTransitioninfo[transitionStateInfo.Layer];
				if (!dictionary.ContainsKey(transitionStateInfo.DestinationState))
				{
					dictionary.Add(transitionStateInfo.DestinationState, transitionStateInfo);
				}
			}
		}

		private void BuildTransitionStateInfoList()
		{
		}

		public void OnAfterDeserialize()
		{
			BuildDestinationToTransitionInfoTable();
		}

		public void OnBeforeSerialize()
		{
			BuildTransitionStateInfoList();
		}

		internal bool IsServerAuthoritative()
		{
			return OnIsServerAuthoritative();
		}

		protected virtual bool OnIsServerAuthoritative()
		{
			return true;
		}

		private void SpawnCleanup()
		{
			if (m_NetworkAnimatorStateChangeHandler != null)
			{
				m_NetworkAnimatorStateChangeHandler.DeregisterUpdate();
				m_NetworkAnimatorStateChangeHandler = null;
			}
		}

		public override void OnDestroy()
		{
			SpawnCleanup();
			_ = m_CachedAnimatorParameters;
			if (m_CachedAnimatorParameters.IsCreated)
			{
				m_CachedAnimatorParameters.Dispose();
			}
			if (m_ParameterWriter.IsInitialized)
			{
				m_ParameterWriter.Dispose();
			}
			base.OnDestroy();
		}

		private unsafe void Awake()
		{
			int layerCount = m_Animator.layerCount;
			m_TransitionHash = new int[layerCount];
			m_AnimationHash = new int[layerCount];
			m_LayerWeights = new float[layerCount];
			m_AnimationMessage = new AnimationMessage
			{
				AnimationStates = new List<AnimationState>()
			};
			for (int i = 0; i < m_Animator.layerCount; i++)
			{
				m_AnimationMessage.AnimationStates.Add(default(AnimationState));
				float layerWeight = m_Animator.GetLayerWeight(i);
				if (layerWeight != m_LayerWeights[i])
				{
					m_LayerWeights[i] = layerWeight;
				}
			}
			AnimatorControllerParameter[] parameters = m_Animator.parameters;
			m_CachedAnimatorParameters = new NativeArray<AnimatorParamCache>(parameters.Length, Allocator.Persistent);
			m_ParametersToUpdate = new List<int>(parameters.Length);
			for (int j = 0; j < parameters.Length; j++)
			{
				AnimatorControllerParameter animatorControllerParameter = parameters[j];
				AnimatorParamCache animatorParamCache = default(AnimatorParamCache);
				animatorParamCache.Type = UnsafeUtility.EnumToInt(animatorControllerParameter.type);
				animatorParamCache.Hash = animatorControllerParameter.nameHash;
				AnimatorParamCache value = animatorParamCache;
				switch (animatorControllerParameter.type)
				{
				case AnimatorControllerParameterType.Float:
				{
					float @float = m_Animator.GetFloat(value.Hash);
					UnsafeUtility.WriteArrayElement(value.Value, 0, @float);
					break;
				}
				case AnimatorControllerParameterType.Int:
				{
					int integer = m_Animator.GetInteger(value.Hash);
					UnsafeUtility.WriteArrayElement(value.Value, 0, integer);
					break;
				}
				case AnimatorControllerParameterType.Bool:
				{
					bool @bool = m_Animator.GetBool(value.Hash);
					UnsafeUtility.WriteArrayElement(value.Value, 0, @bool);
					break;
				}
				}
				m_CachedAnimatorParameters[j] = value;
			}
		}

		internal AnimationMessage GetAnimationMessage()
		{
			return m_AnimationMessage;
		}

		public override void OnNetworkSpawn()
		{
			if (m_Animator == null)
			{
				NetworkLog.LogWarningServer("[" + base.gameObject.name + "][NetworkAnimator] Animator is not assigned! Animation synchronization will not work for this instance!");
			}
			if (base.IsServer)
			{
				m_ClientSendList = new List<ulong>(128);
				m_ClientRpcParams = new ClientRpcParams
				{
					Send = new ClientRpcSendParams
					{
						TargetClientIds = m_ClientSendList
					}
				};
			}
			m_NetworkAnimatorStateChangeHandler = new NetworkAnimatorStateChangeHandler(this);
		}

		public override void OnNetworkDespawn()
		{
			SpawnCleanup();
		}

		private void WriteSynchronizationData<T>(ref BufferSerializer<T> serializer) where T : IReaderWriter
		{
			m_ParametersToUpdate.Clear();
			for (int i = 0; i < m_CachedAnimatorParameters.Length; i++)
			{
				m_ParametersToUpdate.Add(i);
			}
			WriteParameters(ref m_ParameterWriter);
			ParametersUpdateMessage parametersUpdateMessage = default(ParametersUpdateMessage);
			parametersUpdateMessage.Parameters = m_ParameterWriter.ToArray();
			ParametersUpdateMessage value = parametersUpdateMessage;
			serializer.SerializeValue(ref value, default(FastBufferWriter.ForNetworkSerializable));
			m_AnimationMessage.IsDirtyCount = 0;
			for (int j = 0; j < m_Animator.layerCount; j++)
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(j);
				SynchronizationStateInfo?.Add(currentAnimatorStateInfo);
				int stateHash = currentAnimatorStateInfo.fullPathHash;
				float normalizedTime = currentAnimatorStateInfo.normalizedTime;
				bool flag = m_Animator.IsInTransition(j);
				AnimationState value2 = m_AnimationMessage.AnimationStates[j];
				if (flag)
				{
					AnimatorTransitionInfo animatorTransitionInfo = m_Animator.GetAnimatorTransitionInfo(j);
					AnimatorStateInfo nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(j);
					if (nextAnimatorStateInfo.length > 0f)
					{
						float num = nextAnimatorStateInfo.speed * nextAnimatorStateInfo.speedMultiplier;
						float num2 = nextAnimatorStateInfo.length * num;
						float num3 = Mathf.Min(animatorTransitionInfo.duration, animatorTransitionInfo.duration * animatorTransitionInfo.normalizedTime) * 0.5f;
						normalizedTime = Mathf.Min(1f, (num3 > 0f) ? (num3 / num2) : 0f);
					}
					else
					{
						normalizedTime = 0f;
					}
					stateHash = nextAnimatorStateInfo.fullPathHash;
					if (m_DestinationStateToTransitioninfo.ContainsKey(j) && m_DestinationStateToTransitioninfo[j].ContainsKey(nextAnimatorStateInfo.shortNameHash))
					{
						TransitionStateinfo transitionStateinfo = m_DestinationStateToTransitioninfo[j][nextAnimatorStateInfo.shortNameHash];
						stateHash = transitionStateinfo.OriginatingState;
						value2.DestinationStateHash = transitionStateinfo.DestinationState;
					}
				}
				value2.Transition = flag;
				value2.StateHash = stateHash;
				value2.NormalizedTime = normalizedTime;
				value2.Layer = j;
				value2.Weight = m_LayerWeights[j];
				m_AnimationMessage.AnimationStates[j] = value2;
			}
			m_AnimationMessage.IsDirtyCount = m_Animator.layerCount;
			m_AnimationMessage.NetworkSerialize(serializer);
		}

		protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
		{
			if (serializer.IsWriter)
			{
				WriteSynchronizationData(ref serializer);
				return;
			}
			ParametersUpdateMessage value = default(ParametersUpdateMessage);
			AnimationMessage value2 = default(AnimationMessage);
			serializer.SerializeValue(ref value, default(FastBufferWriter.ForNetworkSerializable));
			UpdateParameters(ref value);
			serializer.SerializeValue(ref value2, default(FastBufferWriter.ForNetworkSerializable));
			foreach (AnimationState animationState in value2.AnimationStates)
			{
				UpdateAnimationState(animationState);
			}
		}

		private void CheckForStateChange(int layer)
		{
			bool flag = false;
			AnimationState value = m_AnimationMessage.AnimationStates[m_AnimationMessage.IsDirtyCount];
			float layerWeight = m_Animator.GetLayerWeight(layer);
			value.CrossFade = false;
			value.Transition = false;
			value.NormalizedTime = 0f;
			value.Layer = layer;
			value.Duration = 0f;
			value.Weight = m_LayerWeights[layer];
			value.DestinationStateHash = 0;
			if (layerWeight != m_LayerWeights[layer])
			{
				m_LayerWeights[layer] = layerWeight;
				flag = true;
				value.Weight = layerWeight;
			}
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(layer);
			if (m_Animator.IsInTransition(layer))
			{
				AnimatorTransitionInfo animatorTransitionInfo = m_Animator.GetAnimatorTransitionInfo(layer);
				AnimatorStateInfo nextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(layer);
				if (animatorTransitionInfo.anyState && animatorTransitionInfo.fullPathHash == 0 && m_TransitionHash[layer] != nextAnimatorStateInfo.fullPathHash)
				{
					m_TransitionHash[layer] = nextAnimatorStateInfo.fullPathHash;
					m_AnimationHash[layer] = 0;
					value.DestinationStateHash = nextAnimatorStateInfo.fullPathHash;
					value.CrossFade = true;
					value.Transition = true;
					value.Duration = animatorTransitionInfo.duration;
					value.NormalizedTime = animatorTransitionInfo.normalizedTime;
					flag = true;
				}
				else if (!animatorTransitionInfo.anyState && animatorTransitionInfo.fullPathHash != m_TransitionHash[layer])
				{
					m_TransitionHash[layer] = animatorTransitionInfo.fullPathHash;
					m_AnimationHash[layer] = 0;
					value.StateHash = animatorTransitionInfo.fullPathHash;
					value.CrossFade = false;
					value.Transition = true;
					value.NormalizedTime = animatorTransitionInfo.normalizedTime;
					flag = true;
				}
			}
			else if (currentAnimatorStateInfo.fullPathHash != m_AnimationHash[layer])
			{
				m_TransitionHash[layer] = 0;
				m_AnimationHash[layer] = currentAnimatorStateInfo.fullPathHash;
				if (m_AnimationHash[layer] != 0)
				{
					value.StateHash = currentAnimatorStateInfo.fullPathHash;
					value.NormalizedTime = currentAnimatorStateInfo.normalizedTime;
				}
				flag = true;
			}
			if (flag)
			{
				m_AnimationMessage.AnimationStates[m_AnimationMessage.IsDirtyCount] = value;
				m_AnimationMessage.IsDirtyCount++;
			}
		}

		internal void CheckForAnimatorChanges()
		{
			if (CheckParametersChanged())
			{
				SendParametersUpdate();
			}
			if (m_Animator.runtimeAnimatorController == null)
			{
				if (base.NetworkManager.LogLevel == LogLevel.Developer)
				{
					Debug.LogError("[" + GetType().Name + "] Could not find an assigned RuntimeAnimatorController! Cannot check Animator for changes in state!");
				}
				return;
			}
			m_AnimationMessage.IsDirtyCount = 0;
			for (int i = 0; i < m_Animator.layerCount; i++)
			{
				AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(i);
				float num = currentAnimatorStateInfo.speed * currentAnimatorStateInfo.speedMultiplier;
				if (num > 0f)
				{
					_ = 1f / num;
				}
				CheckForStateChange(i);
			}
			if (m_AnimationMessage.IsDirtyCount > 0)
			{
				if (!base.IsServer && base.IsOwner)
				{
					SendAnimStateServerRpc(m_AnimationMessage);
					return;
				}
				m_ClientSendList.Clear();
				m_ClientSendList.AddRange(base.NetworkManager.ConnectedClientsIds);
				m_ClientSendList.Remove(base.NetworkManager.LocalClientId);
				m_ClientRpcParams.Send.TargetClientIds = m_ClientSendList;
				SendAnimStateClientRpc(m_AnimationMessage, m_ClientRpcParams);
			}
		}

		private void SendParametersUpdate(ClientRpcParams clientRpcParams = default(ClientRpcParams), bool sendDirect = false)
		{
			WriteParameters(ref m_ParameterWriter);
			ParametersUpdateMessage parametersUpdateMessage = default(ParametersUpdateMessage);
			parametersUpdateMessage.Parameters = m_ParameterWriter.ToArray();
			ParametersUpdateMessage parametersUpdateMessage2 = parametersUpdateMessage;
			if (!base.IsServer)
			{
				SendParametersUpdateServerRpc(parametersUpdateMessage2);
			}
			else if (sendDirect)
			{
				SendParametersUpdateClientRpc(parametersUpdateMessage2, clientRpcParams);
			}
			else
			{
				m_NetworkAnimatorStateChangeHandler.SendParameterUpdate(parametersUpdateMessage2, clientRpcParams);
			}
		}

		private unsafe T GetValue<T>(ref AnimatorParamCache animatorParamCache)
		{
			T result;
			fixed (byte* ptr = animatorParamCache.Value)
			{
				void* source = ptr;
				result = UnsafeUtility.ReadArrayElement<T>(source, 0);
			}
			return result;
		}

		private unsafe bool CheckParametersChanged()
		{
			m_ParametersToUpdate.Clear();
			for (int i = 0; i < m_CachedAnimatorParameters.Length; i++)
			{
				ref AnimatorParamCache reference = ref UnsafeUtility.ArrayElementAsRef<AnimatorParamCache>(m_CachedAnimatorParameters.GetUnsafePtr(), i);
				if (m_Animator.IsParameterControlledByCurve(reference.Hash))
				{
					continue;
				}
				int hash = reference.Hash;
				if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterInt)
				{
					int integer = m_Animator.GetInteger(hash);
					if (GetValue<int>(ref reference) != integer)
					{
						m_ParametersToUpdate.Add(i);
					}
				}
				else if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterBool)
				{
					bool @bool = m_Animator.GetBool(hash);
					if (GetValue<bool>(ref reference) != @bool)
					{
						m_ParametersToUpdate.Add(i);
					}
				}
				else if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterFloat)
				{
					float @float = m_Animator.GetFloat(hash);
					if (GetValue<float>(ref reference) != @float)
					{
						m_ParametersToUpdate.Add(i);
					}
				}
			}
			return m_ParametersToUpdate.Count > 0;
		}

		private unsafe void WriteParameters(ref FastBufferWriter writer)
		{
			writer.Seek(0);
			writer.Truncate();
			BytePacker.WriteValuePacked(writer, (uint)m_ParametersToUpdate.Count);
			foreach (int item in m_ParametersToUpdate)
			{
				ref AnimatorParamCache reference = ref UnsafeUtility.ArrayElementAsRef<AnimatorParamCache>(m_CachedAnimatorParameters.GetUnsafePtr(), item);
				int hash = reference.Hash;
				BytePacker.WriteValuePacked(writer, (uint)item);
				if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterInt)
				{
					int integer = m_Animator.GetInteger(hash);
					fixed (byte* ptr = reference.Value)
					{
						void* destination = ptr;
						UnsafeUtility.WriteArrayElement(destination, 0, integer);
						BytePacker.WriteValuePacked(writer, (uint)integer);
					}
				}
				else if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterBool)
				{
					bool @bool = m_Animator.GetBool(hash);
					fixed (byte* ptr = reference.Value)
					{
						void* destination2 = ptr;
						UnsafeUtility.WriteArrayElement(destination2, 0, @bool);
						BytePacker.WriteValuePacked(writer, @bool);
					}
				}
				else
				{
					if (reference.Type != AnimationParamEnumWrapper.AnimatorControllerParameterFloat)
					{
						continue;
					}
					float @float = m_Animator.GetFloat(hash);
					fixed (byte* ptr = reference.Value)
					{
						void* destination3 = ptr;
						UnsafeUtility.WriteArrayElement(destination3, 0, @float);
						BytePacker.WriteValuePacked(writer, @float);
					}
				}
			}
		}

		private unsafe void ReadParameters(FastBufferReader reader)
		{
			ByteUnpacker.ReadValuePacked(reader, out uint value);
			for (int i = 0; i < value; i++)
			{
				ByteUnpacker.ReadValuePacked(reader, out uint value2);
				ref AnimatorParamCache reference = ref UnsafeUtility.ArrayElementAsRef<AnimatorParamCache>(m_CachedAnimatorParameters.GetUnsafePtr(), (int)value2);
				int hash = reference.Hash;
				if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterInt)
				{
					ByteUnpacker.ReadValuePacked(reader, out uint value3);
					m_Animator.SetInteger(hash, (int)value3);
					fixed (byte* ptr = reference.Value)
					{
						void* destination = ptr;
						UnsafeUtility.WriteArrayElement(destination, 0, value3);
					}
				}
				else if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterBool)
				{
					ByteUnpacker.ReadValuePacked(reader, out bool value4);
					m_Animator.SetBool(hash, value4);
					fixed (byte* ptr = reference.Value)
					{
						void* destination2 = ptr;
						UnsafeUtility.WriteArrayElement(destination2, 0, value4);
					}
				}
				else if (reference.Type == AnimationParamEnumWrapper.AnimatorControllerParameterFloat)
				{
					ByteUnpacker.ReadValuePacked(reader, out float value5);
					m_Animator.SetFloat(hash, value5);
					fixed (byte* ptr = reference.Value)
					{
						void* destination3 = ptr;
						UnsafeUtility.WriteArrayElement(destination3, 0, value5);
					}
				}
			}
		}

		internal unsafe void UpdateParameters(ref ParametersUpdateMessage parametersUpdate)
		{
			if (parametersUpdate.Parameters != null && parametersUpdate.Parameters.Length != 0)
			{
				fixed (byte* buffer = parametersUpdate.Parameters)
				{
					FastBufferReader reader = new FastBufferReader(buffer, Allocator.None, parametersUpdate.Parameters.Length);
					ReadParameters(reader);
				}
			}
		}

		internal void UpdateAnimationState(AnimationState animationState)
		{
			if (animationState.Layer < m_LayerWeights.Length && m_LayerWeights[animationState.Layer] != animationState.Weight)
			{
				m_Animator.SetLayerWeight(animationState.Layer, animationState.Weight);
			}
			if (animationState.StateHash == 0 && !animationState.Transition)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(animationState.Layer);
			if (animationState.Transition && !animationState.CrossFade)
			{
				if (m_DestinationStateToTransitioninfo.ContainsKey(animationState.Layer))
				{
					if (m_DestinationStateToTransitioninfo[animationState.Layer].ContainsKey(animationState.DestinationStateHash))
					{
						if (currentAnimatorStateInfo.shortNameHash == animationState.StateHash)
						{
							TransitionStateinfo transitionStateinfo = m_DestinationStateToTransitioninfo[animationState.Layer][animationState.DestinationStateHash];
							m_Animator.CrossFade(transitionStateinfo.DestinationState, transitionStateinfo.TransitionDuration, transitionStateinfo.Layer, 0f, animationState.NormalizedTime);
						}
						else if (base.NetworkManager.LogLevel == LogLevel.Developer)
						{
							NetworkLog.LogWarning($"Current State Hash ({currentAnimatorStateInfo.fullPathHash}) != AnimationState.StateHash ({animationState.StateHash})");
						}
					}
					else if (base.NetworkManager.LogLevel == LogLevel.Developer)
					{
						NetworkLog.LogError($"[DestinationState To Transition Info] Layer ({animationState.Layer}) sub-table does not contain destination state ({animationState.DestinationStateHash})!");
					}
				}
				else if (base.NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogError($"[DestinationState To Transition Info] Layer ({animationState.Layer}) does not exist!");
				}
			}
			else if (animationState.Transition && animationState.CrossFade)
			{
				m_Animator.CrossFade(animationState.DestinationStateHash, animationState.Duration, animationState.Layer, animationState.NormalizedTime);
			}
			else if (currentAnimatorStateInfo.fullPathHash != animationState.StateHash && m_Animator.HasState(animationState.Layer, animationState.StateHash))
			{
				m_Animator.Play(animationState.StateHash, animationState.Layer, animationState.NormalizedTime);
			}
		}

		[ServerRpc]
		private void SendParametersUpdateServerRpc(ParametersUpdateMessage parametersUpdate, ServerRpcParams serverRpcParams = default(ServerRpcParams))
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
				FastBufferWriter bufferWriter = __beginSendServerRpc(1665640498u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in parametersUpdate, default(FastBufferWriter.ForNetworkSerializable));
				__endSendServerRpc(ref bufferWriter, 1665640498u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage != __RpcExecStage.Server || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			if (IsServerAuthoritative())
			{
				m_NetworkAnimatorStateChangeHandler.SendParameterUpdate(parametersUpdate);
			}
			else if (serverRpcParams.Receive.SenderClientId == base.OwnerClientId)
			{
				UpdateParameters(ref parametersUpdate);
				if (base.NetworkManager.ConnectedClientsIds.Count > ((!base.IsHost) ? 1 : 2))
				{
					m_ClientSendList.Clear();
					m_ClientSendList.AddRange(base.NetworkManager.ConnectedClientsIds);
					m_ClientSendList.Remove(serverRpcParams.Receive.SenderClientId);
					m_ClientSendList.Remove(0uL);
					m_ClientRpcParams.Send.TargetClientIds = m_ClientSendList;
					m_NetworkAnimatorStateChangeHandler.SendParameterUpdate(parametersUpdate, m_ClientRpcParams);
				}
			}
		}

		[ClientRpc]
		internal void SendParametersUpdateClientRpc(ParametersUpdateMessage parametersUpdate, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			NetworkManager networkManager = base.NetworkManager;
			if ((object)networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				FastBufferWriter bufferWriter = __beginSendClientRpc(1189168715u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in parametersUpdate, default(FastBufferWriter.ForNetworkSerializable));
				__endSendClientRpc(ref bufferWriter, 1189168715u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
			{
				bool flag = IsServerAuthoritative();
				if ((!flag && !base.IsOwner) || flag)
				{
					m_NetworkAnimatorStateChangeHandler.ProcessParameterUpdate(parametersUpdate);
				}
			}
		}

		[ServerRpc]
		private void SendAnimStateServerRpc(AnimationMessage animationMessage, ServerRpcParams serverRpcParams = default(ServerRpcParams))
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
				FastBufferWriter bufferWriter = __beginSendServerRpc(4140764492u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in animationMessage, default(FastBufferWriter.ForNetworkSerializable));
				__endSendServerRpc(ref bufferWriter, 4140764492u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage != __RpcExecStage.Server || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			if (IsServerAuthoritative())
			{
				m_NetworkAnimatorStateChangeHandler.SendAnimationUpdate(animationMessage);
			}
			else
			{
				if (serverRpcParams.Receive.SenderClientId != base.OwnerClientId)
				{
					return;
				}
				foreach (AnimationState animationState in animationMessage.AnimationStates)
				{
					UpdateAnimationState(animationState);
				}
				if (base.NetworkManager.ConnectedClientsIds.Count > ((!base.IsHost) ? 1 : 2))
				{
					m_ClientSendList.Clear();
					m_ClientSendList.AddRange(base.NetworkManager.ConnectedClientsIds);
					m_ClientSendList.Remove(serverRpcParams.Receive.SenderClientId);
					m_ClientSendList.Remove(0uL);
					m_ClientRpcParams.Send.TargetClientIds = m_ClientSendList;
					m_NetworkAnimatorStateChangeHandler.SendAnimationUpdate(animationMessage, m_ClientRpcParams);
				}
			}
		}

		[ClientRpc]
		internal void SendAnimStateClientRpc(AnimationMessage animationMessage, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			NetworkManager networkManager = base.NetworkManager;
			if ((object)networkManager == null || !networkManager.IsListening)
			{
				return;
			}
			if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
			{
				FastBufferWriter bufferWriter = __beginSendClientRpc(1069363937u, clientRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in animationMessage, default(FastBufferWriter.ForNetworkSerializable));
				__endSendClientRpc(ref bufferWriter, 1069363937u, clientRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage != __RpcExecStage.Client || (!networkManager.IsClient && !networkManager.IsHost))
			{
				return;
			}
			if (base.IsHost)
			{
				if (base.NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogWarning("Detected the Host is sending itself animation updates! Please report this issue.");
				}
				return;
			}
			foreach (AnimationState animationState in animationMessage.AnimationStates)
			{
				UpdateAnimationState(animationState);
			}
		}

		[ServerRpc]
		internal void SendAnimTriggerServerRpc(AnimationTriggerMessage animationTriggerMessage, ServerRpcParams serverRpcParams = default(ServerRpcParams))
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
				FastBufferWriter bufferWriter = __beginSendServerRpc(817791944u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in animationTriggerMessage, default(FastBufferWriter.ForNetworkSerializable));
				__endSendServerRpc(ref bufferWriter, 817791944u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage != __RpcExecStage.Server || (!networkManager.IsServer && !networkManager.IsHost))
			{
				return;
			}
			if (serverRpcParams.Receive.SenderClientId != base.OwnerClientId)
			{
				if (base.NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogWarning("[Owner Authoritative] Detected the a non-authoritative client is sending the server animation trigger updates. If you recently changed ownership of the " + base.name + " object, then this could be the reason.");
				}
				return;
			}
			InternalSetTrigger(animationTriggerMessage.Hash, animationTriggerMessage.IsTriggerSet);
			m_ClientSendList.Clear();
			m_ClientSendList.AddRange(base.NetworkManager.ConnectedClientsIds);
			m_ClientSendList.Remove(0uL);
			if (IsServerAuthoritative())
			{
				m_NetworkAnimatorStateChangeHandler.QueueTriggerUpdateToClient(animationTriggerMessage, m_ClientRpcParams);
			}
			else if (base.NetworkManager.ConnectedClientsIds.Count > ((!base.IsHost) ? 1 : 2))
			{
				m_ClientSendList.Remove(serverRpcParams.Receive.SenderClientId);
				m_NetworkAnimatorStateChangeHandler.QueueTriggerUpdateToClient(animationTriggerMessage, m_ClientRpcParams);
			}
		}

		private void InternalSetTrigger(int hash, bool isSet = true)
		{
			m_Animator.SetBool(hash, isSet);
		}

		[ClientRpc]
		internal void SendAnimTriggerClientRpc(AnimationTriggerMessage animationTriggerMessage, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			NetworkManager networkManager = base.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
				{
					FastBufferWriter bufferWriter = __beginSendClientRpc(2230447564u, clientRpcParams, RpcDelivery.Reliable);
					bufferWriter.WriteValueSafe(in animationTriggerMessage, default(FastBufferWriter.ForNetworkSerializable));
					__endSendClientRpc(ref bufferWriter, 2230447564u, clientRpcParams, RpcDelivery.Reliable);
				}
				if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
				{
					InternalSetTrigger(animationTriggerMessage.Hash, animationTriggerMessage.IsTriggerSet);
				}
			}
		}

		public void SetTrigger(string triggerName)
		{
			SetTrigger(Animator.StringToHash(triggerName));
		}

		public void SetTrigger(int hash, bool setTrigger = true)
		{
			if (!base.IsOwner && !base.IsServer)
			{
				return;
			}
			AnimationTriggerMessage animationTriggerMessage = default(AnimationTriggerMessage);
			animationTriggerMessage.Hash = hash;
			animationTriggerMessage.IsTriggerSet = setTrigger;
			AnimationTriggerMessage animationTriggerMessage2 = animationTriggerMessage;
			if (base.IsServer)
			{
				m_NetworkAnimatorStateChangeHandler.QueueTriggerUpdateToClient(animationTriggerMessage2);
				if (!base.IsHost)
				{
					InternalSetTrigger(hash, setTrigger);
				}
			}
			else
			{
				m_NetworkAnimatorStateChangeHandler.QueueTriggerUpdateToServer(animationTriggerMessage2);
				if (!IsServerAuthoritative())
				{
					InternalSetTrigger(hash, setTrigger);
				}
			}
		}

		public void ResetTrigger(string triggerName)
		{
			ResetTrigger(Animator.StringToHash(triggerName));
		}

		public void ResetTrigger(int hash)
		{
			SetTrigger(hash, setTrigger: false);
		}

		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeRPCS_NetworkAnimator()
		{
			NetworkManager.__rpc_func_table.Add(1665640498u, __rpc_handler_1665640498);
			NetworkManager.__rpc_func_table.Add(1189168715u, __rpc_handler_1189168715);
			NetworkManager.__rpc_func_table.Add(4140764492u, __rpc_handler_4140764492);
			NetworkManager.__rpc_func_table.Add(1069363937u, __rpc_handler_1069363937);
			NetworkManager.__rpc_func_table.Add(817791944u, __rpc_handler_817791944);
			NetworkManager.__rpc_func_table.Add(2230447564u, __rpc_handler_2230447564);
		}

		private static void __rpc_handler_1665640498(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
				reader.ReadValueSafe(out ParametersUpdateMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ServerRpcParams server = rpcParams.Server;
				target.__rpc_exec_stage = __RpcExecStage.Server;
				((NetworkAnimator)target).SendParametersUpdateServerRpc(value, server);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_1189168715(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				reader.ReadValueSafe(out ParametersUpdateMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ClientRpcParams client = rpcParams.Client;
				target.__rpc_exec_stage = __RpcExecStage.Client;
				((NetworkAnimator)target).SendParametersUpdateClientRpc(value, client);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_4140764492(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
				reader.ReadValueSafe(out AnimationMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ServerRpcParams server = rpcParams.Server;
				target.__rpc_exec_stage = __RpcExecStage.Server;
				((NetworkAnimator)target).SendAnimStateServerRpc(value, server);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_1069363937(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				reader.ReadValueSafe(out AnimationMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ClientRpcParams client = rpcParams.Client;
				target.__rpc_exec_stage = __RpcExecStage.Client;
				((NetworkAnimator)target).SendAnimStateClientRpc(value, client);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_817791944(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
				reader.ReadValueSafe(out AnimationTriggerMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ServerRpcParams server = rpcParams.Server;
				target.__rpc_exec_stage = __RpcExecStage.Server;
				((NetworkAnimator)target).SendAnimTriggerServerRpc(value, server);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_2230447564(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				reader.ReadValueSafe(out AnimationTriggerMessage value, default(FastBufferWriter.ForNetworkSerializable));
				ClientRpcParams client = rpcParams.Client;
				target.__rpc_exec_stage = __RpcExecStage.Client;
				((NetworkAnimator)target).SendAnimTriggerClientRpc(value, client);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		protected internal override string __getTypeName()
		{
			return "NetworkAnimator";
		}
	}
}
