using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Netcode.Components
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Netcode/Network Transform")]
	[DefaultExecutionOrder(100000)]
	public class NetworkTransform : NetworkBehaviour
	{
		public delegate(Vector3 pos, Quaternion rotOut, Vector3 scale) OnClientRequestChangeDelegate(Vector3 pos, Quaternion rot, Vector3 scale);

		public struct NetworkTransformState : INetworkSerializable
		{
			private const int k_InLocalSpaceBit = 1;

			private const int k_PositionXBit = 2;

			private const int k_PositionYBit = 4;

			private const int k_PositionZBit = 8;

			private const int k_RotAngleXBit = 16;

			private const int k_RotAngleYBit = 32;

			private const int k_RotAngleZBit = 64;

			private const int k_ScaleXBit = 128;

			private const int k_ScaleYBit = 256;

			private const int k_ScaleZBit = 512;

			private const int k_TeleportingBit = 1024;

			private const int k_Interpolate = 2048;

			private const int k_QuaternionSync = 4096;

			private const int k_QuaternionCompress = 8192;

			private const int k_UseHalfFloats = 16384;

			private const int k_Synchronization = 32768;

			private const int k_PositionSlerp = 65536;

			private const int k_IsParented = 131072;

			private const int k_TrackStateId = 268435456;

			private uint m_Bitset;

			internal double SentTime;

			internal float PositionX;

			internal float PositionY;

			internal float PositionZ;

			internal float RotAngleX;

			internal float RotAngleY;

			internal float RotAngleZ;

			internal Quaternion Rotation;

			internal float ScaleX;

			internal float ScaleY;

			internal float ScaleZ;

			internal Vector3 CurrentPosition;

			internal Vector3 DeltaPosition;

			internal NetworkDeltaPosition NetworkDeltaPosition;

			internal HalfVector3 HalfVectorScale;

			internal Vector3 Scale;

			internal Vector3 LossyScale;

			internal HalfVector4 HalfVectorRotation;

			internal uint QuaternionCompressed;

			internal int NetworkTick;

			internal int StateId;

			private FastBufferReader m_Reader;

			private FastBufferWriter m_Writer;

			internal HalfVector3 HalfEulerRotation;

			internal uint BitSet
			{
				get
				{
					return m_Bitset;
				}
				set
				{
					m_Bitset = value;
				}
			}

			internal bool IsDirty { get; set; }

			public int LastSerializedSize { get; internal set; }

			public bool InLocalSpace
			{
				get
				{
					return GetFlag(1);
				}
				internal set
				{
					SetFlag(value, 1);
				}
			}

			public bool HasPositionX
			{
				get
				{
					return GetFlag(2);
				}
				internal set
				{
					SetFlag(value, 2);
				}
			}

			public bool HasPositionY
			{
				get
				{
					return GetFlag(4);
				}
				internal set
				{
					SetFlag(value, 4);
				}
			}

			public bool HasPositionZ
			{
				get
				{
					return GetFlag(8);
				}
				internal set
				{
					SetFlag(value, 8);
				}
			}

			public bool HasPositionChange => HasPositionX | HasPositionY | HasPositionZ;

			public bool HasRotAngleX
			{
				get
				{
					return GetFlag(16);
				}
				internal set
				{
					SetFlag(value, 16);
				}
			}

			public bool HasRotAngleY
			{
				get
				{
					return GetFlag(32);
				}
				internal set
				{
					SetFlag(value, 32);
				}
			}

			public bool HasRotAngleZ
			{
				get
				{
					return GetFlag(64);
				}
				internal set
				{
					SetFlag(value, 64);
				}
			}

			public bool HasRotAngleChange => HasRotAngleX | HasRotAngleY | HasRotAngleZ;

			public bool HasScaleX
			{
				get
				{
					return GetFlag(128);
				}
				internal set
				{
					SetFlag(value, 128);
				}
			}

			public bool HasScaleY
			{
				get
				{
					return GetFlag(256);
				}
				internal set
				{
					SetFlag(value, 256);
				}
			}

			public bool HasScaleZ
			{
				get
				{
					return GetFlag(512);
				}
				internal set
				{
					SetFlag(value, 512);
				}
			}

			public bool HasScaleChange => HasScaleX | HasScaleY | HasScaleZ;

			public bool IsTeleportingNextFrame
			{
				get
				{
					return GetFlag(1024);
				}
				internal set
				{
					SetFlag(value, 1024);
				}
			}

			public bool UseInterpolation
			{
				get
				{
					return GetFlag(2048);
				}
				internal set
				{
					SetFlag(value, 2048);
				}
			}

			public bool QuaternionSync
			{
				get
				{
					return GetFlag(4096);
				}
				internal set
				{
					SetFlag(value, 4096);
				}
			}

			public bool QuaternionCompression
			{
				get
				{
					return GetFlag(8192);
				}
				internal set
				{
					SetFlag(value, 8192);
				}
			}

			public bool UseHalfFloatPrecision
			{
				get
				{
					return GetFlag(16384);
				}
				internal set
				{
					SetFlag(value, 16384);
				}
			}

			public bool IsSynchronizing
			{
				get
				{
					return GetFlag(32768);
				}
				internal set
				{
					SetFlag(value, 32768);
				}
			}

			public bool UsePositionSlerp
			{
				get
				{
					return GetFlag(65536);
				}
				internal set
				{
					SetFlag(value, 65536);
				}
			}

			internal bool IsParented
			{
				get
				{
					return GetFlag(131072);
				}
				set
				{
					SetFlag(value, 131072);
				}
			}

			internal bool TrackByStateId
			{
				get
				{
					return GetFlag(268435456);
				}
				set
				{
					SetFlag(value, 268435456);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal bool HasScale(int axisIndex)
			{
				return GetFlag(128 << axisIndex);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal void SetHasScale(int axisIndex, bool isSet)
			{
				SetFlag(isSet, 128 << axisIndex);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool GetFlag(int flag)
			{
				return (m_Bitset & flag) != 0;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void SetFlag(bool set, int flag)
			{
				if (set)
				{
					m_Bitset |= (uint)flag;
				}
				else
				{
					m_Bitset &= (uint)(~flag);
				}
			}

			internal void ClearBitSetForNextTick()
			{
				m_Bitset &= 96257u;
				IsDirty = false;
			}

			public Quaternion GetRotation()
			{
				if (HasRotAngleChange)
				{
					if (QuaternionSync)
					{
						return Rotation;
					}
					return Quaternion.Euler(RotAngleX, RotAngleY, RotAngleZ);
				}
				return Quaternion.identity;
			}

			public Vector3 GetPosition()
			{
				if (HasPositionChange)
				{
					if (UseHalfFloatPrecision)
					{
						if (IsTeleportingNextFrame)
						{
							return CurrentPosition;
						}
						return NetworkDeltaPosition.GetFullPosition();
					}
					return new Vector3(PositionX, PositionY, PositionZ);
				}
				return Vector3.zero;
			}

			public Vector3 GetScale()
			{
				if (HasScaleChange)
				{
					if (UseHalfFloatPrecision)
					{
						if (IsTeleportingNextFrame)
						{
							return Scale;
						}
						return HalfVectorScale.ToVector3();
					}
					return new Vector3(ScaleX, ScaleY, ScaleZ);
				}
				return Vector3.zero;
			}

			public int GetNetworkTick()
			{
				return NetworkTick;
			}

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				int num = 0;
				bool isWriter = serializer.IsWriter;
				if (isWriter)
				{
					m_Writer = serializer.GetFastBufferWriter();
					num = m_Writer.Position;
				}
				else
				{
					m_Reader = serializer.GetFastBufferReader();
					num = m_Reader.Position;
				}
				if (isWriter)
				{
					BytePacker.WriteValueBitPacked(m_Writer, m_Bitset);
					BytePacker.WriteValueBitPacked(m_Writer, NetworkTick);
				}
				else
				{
					ByteUnpacker.ReadValueBitPacked(m_Reader, out m_Bitset);
					ByteUnpacker.ReadValueBitPacked(m_Reader, out NetworkTick);
				}
				if (TrackByStateId)
				{
					serializer.SerializeValue(ref StateId, default(FastBufferWriter.ForPrimitives));
				}
				if (HasPositionChange)
				{
					if (UseHalfFloatPrecision)
					{
						NetworkDeltaPosition.HalfVector3.AxisToSynchronize[0] = HasPositionX;
						NetworkDeltaPosition.HalfVector3.AxisToSynchronize[1] = HasPositionY;
						NetworkDeltaPosition.HalfVector3.AxisToSynchronize[2] = HasPositionZ;
						if (IsTeleportingNextFrame)
						{
							serializer.SerializeValue(ref CurrentPosition);
							if (IsSynchronizing)
							{
								serializer.SerializeValue(ref DeltaPosition);
								if (!isWriter)
								{
									NetworkDeltaPosition.NetworkTick = NetworkTick;
									NetworkDeltaPosition.NetworkSerialize(serializer);
								}
								else
								{
									serializer.SerializeNetworkSerializable(ref NetworkDeltaPosition);
								}
							}
						}
						else if (!isWriter)
						{
							NetworkDeltaPosition.NetworkTick = NetworkTick;
							NetworkDeltaPosition.NetworkSerialize(serializer);
						}
						else
						{
							serializer.SerializeNetworkSerializable(ref NetworkDeltaPosition);
						}
					}
					else
					{
						if (HasPositionX)
						{
							serializer.SerializeValue(ref PositionX, default(FastBufferWriter.ForPrimitives));
						}
						if (HasPositionY)
						{
							serializer.SerializeValue(ref PositionY, default(FastBufferWriter.ForPrimitives));
						}
						if (HasPositionZ)
						{
							serializer.SerializeValue(ref PositionZ, default(FastBufferWriter.ForPrimitives));
						}
					}
				}
				if (HasRotAngleChange)
				{
					if (QuaternionSync)
					{
						if (IsTeleportingNextFrame)
						{
							serializer.SerializeValue(ref Rotation);
						}
						else if (QuaternionCompression)
						{
							if (isWriter)
							{
								QuaternionCompressed = QuaternionCompressor.CompressQuaternion(ref Rotation);
							}
							serializer.SerializeValue(ref QuaternionCompressed, default(FastBufferWriter.ForPrimitives));
							if (!isWriter)
							{
								QuaternionCompressor.DecompressQuaternion(ref Rotation, QuaternionCompressed);
							}
						}
						else if (UseHalfFloatPrecision)
						{
							if (isWriter)
							{
								HalfVectorRotation.UpdateFrom(ref Rotation);
							}
							serializer.SerializeNetworkSerializable(ref HalfVectorRotation);
							if (!isWriter)
							{
								Rotation = HalfVectorRotation.ToQuaternion();
							}
						}
						else
						{
							serializer.SerializeValue(ref Rotation);
						}
					}
					else if (UseHalfFloatPrecision && !IsTeleportingNextFrame)
					{
						if (HasRotAngleChange)
						{
							HalfEulerRotation.AxisToSynchronize[0] = HasRotAngleX;
							HalfEulerRotation.AxisToSynchronize[1] = HasRotAngleY;
							HalfEulerRotation.AxisToSynchronize[2] = HasRotAngleZ;
							if (isWriter)
							{
								HalfEulerRotation.Set(RotAngleX, RotAngleY, RotAngleZ);
							}
							serializer.SerializeValue(ref HalfEulerRotation, default(FastBufferWriter.ForNetworkSerializable));
							if (!isWriter)
							{
								Vector3 vector = HalfEulerRotation.ToVector3();
								if (HasRotAngleX)
								{
									RotAngleX = vector.x;
								}
								if (HasRotAngleY)
								{
									RotAngleY = vector.y;
								}
								if (HasRotAngleZ)
								{
									RotAngleZ = vector.z;
								}
							}
						}
					}
					else
					{
						if (HasRotAngleX)
						{
							serializer.SerializeValue(ref RotAngleX, default(FastBufferWriter.ForPrimitives));
						}
						if (HasRotAngleY)
						{
							serializer.SerializeValue(ref RotAngleY, default(FastBufferWriter.ForPrimitives));
						}
						if (HasRotAngleZ)
						{
							serializer.SerializeValue(ref RotAngleZ, default(FastBufferWriter.ForPrimitives));
						}
					}
				}
				if (HasScaleChange)
				{
					if (IsTeleportingNextFrame && IsParented)
					{
						serializer.SerializeValue(ref LossyScale);
					}
					if (UseHalfFloatPrecision)
					{
						if (IsTeleportingNextFrame)
						{
							serializer.SerializeValue(ref Scale);
						}
						else
						{
							HalfVectorScale.AxisToSynchronize[0] = HasScaleX;
							HalfVectorScale.AxisToSynchronize[1] = HasScaleY;
							HalfVectorScale.AxisToSynchronize[2] = HasScaleZ;
							if (isWriter)
							{
								HalfVectorScale.Set(Scale[0], Scale[1], Scale[2]);
							}
							serializer.SerializeValue(ref HalfVectorScale, default(FastBufferWriter.ForNetworkSerializable));
							if (!isWriter)
							{
								Scale = HalfVectorScale.ToVector3();
								if (HasScaleX)
								{
									ScaleX = Scale.x;
								}
								if (HasScaleY)
								{
									ScaleY = Scale.y;
								}
								if (HasScaleZ)
								{
									ScaleZ = Scale.x;
								}
							}
						}
					}
					else
					{
						if (HasScaleX)
						{
							serializer.SerializeValue(ref ScaleX, default(FastBufferWriter.ForPrimitives));
						}
						if (HasScaleY)
						{
							serializer.SerializeValue(ref ScaleY, default(FastBufferWriter.ForPrimitives));
						}
						if (HasScaleZ)
						{
							serializer.SerializeValue(ref ScaleZ, default(FastBufferWriter.ForPrimitives));
						}
					}
				}
				if (!isWriter)
				{
					IsDirty = HasPositionChange || HasRotAngleChange || HasScaleChange;
					LastSerializedSize = m_Reader.Position - num;
				}
				else
				{
					LastSerializedSize = m_Writer.Position - num;
				}
			}
		}

		public const float PositionThresholdDefault = 0.001f;

		public const float RotAngleThresholdDefault = 0.01f;

		public const float ScaleThresholdDefault = 0.01f;

		public OnClientRequestChangeDelegate OnClientRequestChange;

		internal static bool TrackByStateId;

		public bool SyncPositionX = true;

		public bool SyncPositionY = true;

		public bool SyncPositionZ = true;

		public bool SyncRotAngleX = true;

		public bool SyncRotAngleY = true;

		public bool SyncRotAngleZ = true;

		public bool SyncScaleX = true;

		public bool SyncScaleY = true;

		public bool SyncScaleZ = true;

		public float PositionThreshold = 0.001f;

		[Range(1E-05f, 360f)]
		public float RotAngleThreshold = 0.01f;

		public float ScaleThreshold = 0.01f;

		[Tooltip("When enabled, this will synchronize the full Quaternion (i.e. all Euler rotation axis are updated if one axis has a delta)")]
		public bool UseQuaternionSynchronization;

		[Tooltip("When enabled, this uses a smallest three implementation that reduces full Quaternion updates down to the size of an unsigned integer (ignores half float precision settings).")]
		public bool UseQuaternionCompression;

		[Tooltip("When enabled, this will use half float precision values for position (uses delta position updating), rotation (except when Quaternion compression is enabled), and scale.")]
		public bool UseHalfFloatPrecision;

		[Tooltip("Sets whether this transform should sync in local space or in world space")]
		public bool InLocalSpace;

		public bool Interpolate = true;

		[Tooltip("When enabled the position interpolator will Slerp towards its current target position.")]
		public bool SlerpPosition;

		protected bool m_CachedIsServer;

		protected NetworkManager m_CachedNetworkManager;

		private NetworkTransformState m_LocalAuthoritativeNetworkState;

		private ClientRpcParams m_ClientRpcParams = new ClientRpcParams
		{
			Send = default(ClientRpcSendParams)
		};

		private List<ulong> m_ClientIds = new List<ulong> { 0uL };

		private BufferedLinearInterpolatorVector3 m_PositionInterpolator;

		private BufferedLinearInterpolatorVector3 m_ScaleInterpolator;

		private BufferedLinearInterpolatorQuaternion m_RotationInterpolator;

		private Vector3 m_CurrentPosition;

		private Vector3 m_TargetPosition;

		private Vector3 m_CurrentScale;

		private Vector3 m_TargetScale;

		private Quaternion m_CurrentRotation;

		private Vector3 m_TargetRotation;

		private string m_MessageName;

		private NetworkDeltaPosition m_HalfPositionState = new NetworkDeltaPosition(Vector3.zero, 0);

		private NetworkTransformState m_OldState;

		private NetworkVariable<NetworkTransformState> m_InternalStatNetVar = new NetworkVariable<NetworkTransformState>(default(NetworkTransformState), NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);

		private bool SynchronizePosition
		{
			get
			{
				if (!SyncPositionX && !SyncPositionY)
				{
					return SyncPositionZ;
				}
				return true;
			}
		}

		private bool SynchronizeRotation
		{
			get
			{
				if (!SyncRotAngleX && !SyncRotAngleY)
				{
					return SyncRotAngleZ;
				}
				return true;
			}
		}

		private bool SynchronizeScale
		{
			get
			{
				if (!SyncScaleX && !SyncScaleY)
				{
					return SyncScaleZ;
				}
				return true;
			}
		}

		public bool CanCommitToTransform { get; protected set; }

		internal NetworkTransformState LocalAuthoritativeNetworkState => m_LocalAuthoritativeNetworkState;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetSpaceRelativePosition(bool getCurrentState = false)
		{
			if (!getCurrentState || CanCommitToTransform)
			{
				if (!InLocalSpace)
				{
					return base.transform.position;
				}
				return base.transform.localPosition;
			}
			return m_CurrentPosition;
		}

		public Quaternion GetSpaceRelativeRotation(bool getCurrentState = false)
		{
			if (!getCurrentState || CanCommitToTransform)
			{
				if (!InLocalSpace)
				{
					return base.transform.rotation;
				}
				return base.transform.localRotation;
			}
			return m_CurrentRotation;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetScale(bool getCurrentState = false)
		{
			if (!getCurrentState || CanCommitToTransform)
			{
				return base.transform.localScale;
			}
			return m_CurrentScale;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void UpdatePositionInterpolator(Vector3 position, double time, bool resetInterpolator = false)
		{
			if (!CanCommitToTransform)
			{
				if (resetInterpolator)
				{
					m_PositionInterpolator.ResetTo(position, time);
				}
				else
				{
					m_PositionInterpolator.AddMeasurement(position, time);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddLogEntry(ref NetworkTransformState networkTransformState, ulong targetClient, bool preUpdate = false)
		{
		}

		internal void UpdatePositionSlerp()
		{
			if (m_PositionInterpolator != null)
			{
				m_PositionInterpolator.IsSlerp = SlerpPosition;
			}
		}

		private bool ShouldSynchronizeHalfFloat(ulong targetClientId)
		{
			if (!IsServerAuthoritative() && base.NetworkObject.OwnerClientId == targetClientId)
			{
				return base.NetworkObject.IsOwnedByServer;
			}
			return true;
		}

		protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
		{
			ulong targetIdBeingSynchronized = base.m_TargetIdBeingSynchronized;
			NetworkTransformState networkTransformState = default(NetworkTransformState);
			networkTransformState.HalfEulerRotation = default(HalfVector3);
			networkTransformState.HalfVectorRotation = default(HalfVector4);
			networkTransformState.HalfVectorScale = default(HalfVector3);
			networkTransformState.NetworkDeltaPosition = default(NetworkDeltaPosition);
			NetworkTransformState networkState = networkTransformState;
			if (serializer.IsWriter)
			{
				networkState.IsTeleportingNextFrame = true;
				Transform transformToUse = base.transform;
				ApplyTransformToNetworkStateWithInfo(ref networkState, ref transformToUse, isSynchronization: true, targetIdBeingSynchronized);
				networkState.NetworkSerialize(serializer);
				return;
			}
			networkState.NetworkSerialize(serializer);
			InLocalSpace = networkState.InLocalSpace;
			Interpolate = networkState.UseInterpolation;
			UseQuaternionSynchronization = networkState.QuaternionSync;
			UseHalfFloatPrecision = networkState.UseHalfFloatPrecision;
			UseQuaternionCompression = networkState.QuaternionCompression;
			SlerpPosition = networkState.UsePositionSlerp;
			UpdatePositionSlerp();
			ApplyTeleportingState(networkState);
			m_LocalAuthoritativeNetworkState = networkState;
			m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = false;
			m_LocalAuthoritativeNetworkState.IsSynchronizing = false;
		}

		protected void TryCommitTransformToServer(Transform transformToCommit, double dirtyTime)
		{
			if (!base.IsSpawned)
			{
				NetworkLog.LogError("Cannot commit transform when not spawned!");
			}
			else if (!base.IsServer && !base.IsOwner)
			{
				NetworkLog.LogError((base.gameObject != base.NetworkObject.gameObject) ? ("Non-authority instance of " + base.NetworkObject.gameObject.name + " is trying to commit a transform on " + base.gameObject.name + "!") : ("Non-authority instance of " + base.NetworkObject.gameObject.name + " is trying to commit a transform!"));
			}
			else if (CanCommitToTransform)
			{
				OnUpdateAuthoritativeState(ref transformToCommit);
			}
			else
			{
				Vector3 pos = (InLocalSpace ? transformToCommit.localPosition : transformToCommit.position);
				Quaternion rot = (InLocalSpace ? transformToCommit.localRotation : transformToCommit.rotation);
				if (!base.IsServer)
				{
					SetStateServerRpc(pos, rot, transformToCommit.localScale, shouldTeleport: false);
				}
				else
				{
					SetStateClientRpc(pos, rot, transformToCommit.localScale, shouldTeleport: false);
				}
			}
		}

		protected virtual void OnAuthorityPushTransformState(ref NetworkTransformState networkTransformState)
		{
		}

		private void TryCommitTransform(ref Transform transformToCommit, bool synchronize = false)
		{
			if (!base.IsServer && !base.IsOwner)
			{
				NetworkLog.LogError("[" + base.name + "] is trying to commit the transform without authority!");
			}
			else if (ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, ref transformToCommit, synchronize, 0uL))
			{
				m_LocalAuthoritativeNetworkState.LastSerializedSize = m_OldState.LastSerializedSize;
				OnAuthorityPushTransformState(ref m_LocalAuthoritativeNetworkState);
				UpdateTransformState();
				m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = false;
			}
		}

		private void ResetInterpolatedStateToCurrentAuthoritativeState()
		{
			double time = base.NetworkManager.ServerTime.Time;
			UpdatePositionInterpolator(GetSpaceRelativePosition(), time, resetInterpolator: true);
			UpdatePositionSlerp();
			m_ScaleInterpolator.ResetTo(base.transform.localScale, time);
			m_RotationInterpolator.ResetTo(GetSpaceRelativeRotation(), time);
		}

		internal NetworkTransformState ApplyLocalNetworkState(Transform transform)
		{
			m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();
			ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, ref transform, isSynchronization: false, 0uL);
			return m_LocalAuthoritativeNetworkState;
		}

		internal bool ApplyTransformToNetworkState(ref NetworkTransformState networkState, double dirtyTime, Transform transformToUse)
		{
			networkState.UseInterpolation = Interpolate;
			networkState.QuaternionSync = UseQuaternionSynchronization;
			networkState.UseHalfFloatPrecision = UseHalfFloatPrecision;
			networkState.QuaternionCompression = UseQuaternionCompression;
			m_HalfPositionState = new NetworkDeltaPosition(Vector3.zero, 0, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
			return ApplyTransformToNetworkStateWithInfo(ref networkState, ref transformToUse, isSynchronization: false, 0uL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool ApplyTransformToNetworkStateWithInfo(ref NetworkTransformState networkState, ref Transform transformToUse, bool isSynchronization = false, ulong targetClientId = 0uL)
		{
			bool num = networkState.IsTeleportingNextFrame && !isSynchronization;
			bool flag = false;
			bool flag2 = num && networkState.HasPositionChange;
			bool flag3 = num && networkState.HasRotAngleChange;
			bool flag4 = num && networkState.HasScaleChange;
			Vector3 vector = (InLocalSpace ? transformToUse.localPosition : transformToUse.position);
			Vector3 vector2 = (InLocalSpace ? transformToUse.localEulerAngles : transformToUse.eulerAngles);
			Vector3 localScale = transformToUse.localScale;
			networkState.IsSynchronizing = isSynchronization;
			if (InLocalSpace != networkState.InLocalSpace)
			{
				networkState.InLocalSpace = InLocalSpace;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (Interpolate != networkState.UseInterpolation)
			{
				networkState.UseInterpolation = Interpolate;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (UseQuaternionSynchronization != networkState.QuaternionSync)
			{
				networkState.QuaternionSync = UseQuaternionSynchronization;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (UseQuaternionCompression != networkState.QuaternionCompression)
			{
				networkState.QuaternionCompression = UseQuaternionCompression;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (UseHalfFloatPrecision != networkState.UseHalfFloatPrecision)
			{
				networkState.UseHalfFloatPrecision = UseHalfFloatPrecision;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (SlerpPosition != networkState.UsePositionSlerp)
			{
				networkState.UsePositionSlerp = SlerpPosition;
				flag = true;
				networkState.IsTeleportingNextFrame = true;
			}
			if (!UseHalfFloatPrecision)
			{
				if (SyncPositionX && (Mathf.Abs(networkState.PositionX - vector.x) >= PositionThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.PositionX = vector.x;
					networkState.HasPositionX = true;
					flag2 = true;
				}
				if (SyncPositionY && (Mathf.Abs(networkState.PositionY - vector.y) >= PositionThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.PositionY = vector.y;
					networkState.HasPositionY = true;
					flag2 = true;
				}
				if (SyncPositionZ && (Mathf.Abs(networkState.PositionZ - vector.z) >= PositionThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.PositionZ = vector.z;
					networkState.HasPositionZ = true;
					flag2 = true;
				}
			}
			else if (SynchronizePosition)
			{
				flag2 = networkState.IsTeleportingNextFrame;
				if (!flag2)
				{
					for (int i = 0; i < 3; i++)
					{
						if (Math.Abs(vector[i] - m_HalfPositionState.PreviousPosition[i]) >= PositionThreshold)
						{
							flag2 = i switch
							{
								1 => SyncPositionY, 
								0 => SyncPositionX, 
								_ => SyncPositionZ, 
							};
							if (flag2)
							{
								break;
							}
						}
					}
				}
				if (flag2)
				{
					if (!isSynchronization)
					{
						if (networkState.IsTeleportingNextFrame)
						{
							m_HalfPositionState = new NetworkDeltaPosition(vector, networkState.NetworkTick, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
							networkState.CurrentPosition = vector;
						}
						else
						{
							m_HalfPositionState.HalfVector3.AxisToSynchronize = math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ);
							m_HalfPositionState.UpdateFrom(ref vector, networkState.NetworkTick);
						}
						networkState.NetworkDeltaPosition = m_HalfPositionState;
					}
					else
					{
						if (ShouldSynchronizeHalfFloat(targetClientId))
						{
							if (m_HalfPositionState.NetworkTick > 0)
							{
								networkState.CurrentPosition = m_HalfPositionState.CurrentBasePosition;
								networkState.NetworkDeltaPosition = m_HalfPositionState;
								if (base.NetworkObject.IsOwnedByServer || IsServerAuthoritative())
								{
									networkState.DeltaPosition = m_HalfPositionState.HalfDeltaConvertedBack;
								}
								else
								{
									networkState.DeltaPosition = m_HalfPositionState.DeltaPosition;
								}
							}
							else
							{
								networkState.NetworkDeltaPosition = new NetworkDeltaPosition(Vector3.zero, 0, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
								networkState.DeltaPosition = Vector3.zero;
								networkState.CurrentPosition = vector;
							}
						}
						else
						{
							networkState.NetworkDeltaPosition = new NetworkDeltaPosition(Vector3.zero, 0, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
							networkState.CurrentPosition = vector;
						}
						AddLogEntry(ref networkState, targetClientId, preUpdate: true);
					}
					networkState.HasPositionX = SyncPositionX;
					networkState.HasPositionY = SyncPositionY;
					networkState.HasPositionZ = SyncPositionZ;
				}
			}
			if (!UseQuaternionSynchronization)
			{
				if (SyncRotAngleX && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleX, vector2.x)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.RotAngleX = vector2.x;
					networkState.HasRotAngleX = true;
					flag3 = true;
				}
				if (SyncRotAngleY && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleY, vector2.y)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.RotAngleY = vector2.y;
					networkState.HasRotAngleY = true;
					flag3 = true;
				}
				if (SyncRotAngleZ && (Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleZ, vector2.z)) >= RotAngleThreshold || networkState.IsTeleportingNextFrame))
				{
					networkState.RotAngleZ = vector2.z;
					networkState.HasRotAngleZ = true;
					flag3 = true;
				}
			}
			else if (SynchronizeRotation)
			{
				flag3 = networkState.IsTeleportingNextFrame;
				if (!flag3)
				{
					Vector3 eulerAngles = networkState.Rotation.eulerAngles;
					for (int j = 0; j < 3; j++)
					{
						if (Mathf.Abs(Mathf.DeltaAngle(eulerAngles[j], vector2[j])) >= RotAngleThreshold)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (flag3)
				{
					networkState.Rotation = (InLocalSpace ? transformToUse.localRotation : transformToUse.rotation);
					networkState.HasRotAngleX = true;
					networkState.HasRotAngleY = true;
					networkState.HasRotAngleZ = true;
				}
			}
			if (isSynchronization || networkState.IsTeleportingNextFrame)
			{
				bool flag5 = false;
				if (base.NetworkObject.transform.parent != null)
				{
					NetworkObject component = base.NetworkObject.transform.parent.GetComponent<NetworkObject>();
					flag5 = (component == null && base.NetworkObject.IsSceneObject != false) || component != null;
				}
				networkState.IsParented = flag5;
				if (flag5)
				{
					networkState.LossyScale = base.transform.lossyScale;
				}
			}
			if (!isSynchronization)
			{
				if (!UseHalfFloatPrecision)
				{
					if (SyncScaleX && (Mathf.Abs(networkState.ScaleX - localScale.x) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
					{
						networkState.ScaleX = localScale.x;
						networkState.HasScaleX = true;
						flag4 = true;
					}
					if (SyncScaleY && (Mathf.Abs(networkState.ScaleY - localScale.y) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
					{
						networkState.ScaleY = localScale.y;
						networkState.HasScaleY = true;
						flag4 = true;
					}
					if (SyncScaleZ && (Mathf.Abs(networkState.ScaleZ - localScale.z) >= ScaleThreshold || networkState.IsTeleportingNextFrame))
					{
						networkState.ScaleZ = localScale.z;
						networkState.HasScaleZ = true;
						flag4 = true;
					}
				}
				else if (SynchronizeScale)
				{
					Vector3 scale = networkState.Scale;
					for (int k = 0; k < 3; k++)
					{
						if (Mathf.Abs(localScale[k] - scale[k]) >= ScaleThreshold || networkState.IsTeleportingNextFrame)
						{
							flag4 = true;
							networkState.Scale[k] = localScale[k];
							networkState.SetHasScale(k, k switch
							{
								1 => SyncScaleY, 
								0 => SyncScaleX, 
								_ => SyncScaleZ, 
							});
						}
					}
				}
			}
			else if (SynchronizeScale)
			{
				if (!UseHalfFloatPrecision)
				{
					networkState.ScaleX = base.transform.localScale.x;
					networkState.ScaleY = base.transform.localScale.y;
					networkState.ScaleZ = base.transform.localScale.z;
				}
				else
				{
					networkState.Scale = base.transform.localScale;
				}
				networkState.HasScaleX = true;
				networkState.HasScaleY = true;
				networkState.HasScaleZ = true;
				flag4 = true;
			}
			flag = flag || flag2 || flag3 || flag4;
			if (flag && base.enabled)
			{
				networkState.NetworkTick = base.NetworkManager.ServerTime.Tick;
			}
			networkState.IsDirty |= flag;
			return flag;
		}

		private void ApplyAuthoritativeState()
		{
			NetworkTransformState localAuthoritativeNetworkState = m_LocalAuthoritativeNetworkState;
			Vector3 currentPosition = m_CurrentPosition;
			Quaternion currentRotation = m_CurrentRotation;
			Vector3 eulerAngles = currentRotation.eulerAngles;
			Vector3 currentScale = m_CurrentScale;
			InLocalSpace = localAuthoritativeNetworkState.InLocalSpace;
			Interpolate = localAuthoritativeNetworkState.UseInterpolation;
			UseHalfFloatPrecision = localAuthoritativeNetworkState.UseHalfFloatPrecision;
			UseQuaternionSynchronization = localAuthoritativeNetworkState.QuaternionSync;
			UseQuaternionCompression = localAuthoritativeNetworkState.QuaternionCompression;
			if (SlerpPosition != localAuthoritativeNetworkState.UsePositionSlerp)
			{
				SlerpPosition = localAuthoritativeNetworkState.UsePositionSlerp;
				UpdatePositionSlerp();
			}
			if (Interpolate)
			{
				if (SynchronizePosition)
				{
					Vector3 interpolatedValue = m_PositionInterpolator.GetInterpolatedValue();
					if (UseHalfFloatPrecision)
					{
						currentPosition = interpolatedValue;
					}
					else
					{
						if (SyncPositionX)
						{
							currentPosition.x = interpolatedValue.x;
						}
						if (SyncPositionY)
						{
							currentPosition.y = interpolatedValue.y;
						}
						if (SyncPositionZ)
						{
							currentPosition.z = interpolatedValue.z;
						}
					}
				}
				if (SynchronizeScale)
				{
					if (UseHalfFloatPrecision)
					{
						currentScale = m_ScaleInterpolator.GetInterpolatedValue();
					}
					else
					{
						Vector3 interpolatedValue2 = m_ScaleInterpolator.GetInterpolatedValue();
						if (SyncScaleX)
						{
							currentScale.x = interpolatedValue2.x;
						}
						if (SyncScaleY)
						{
							currentScale.y = interpolatedValue2.y;
						}
						if (SyncScaleZ)
						{
							currentScale.z = interpolatedValue2.z;
						}
					}
				}
				if (SynchronizeRotation)
				{
					Quaternion interpolatedValue3 = m_RotationInterpolator.GetInterpolatedValue();
					if (UseQuaternionSynchronization)
					{
						currentRotation = interpolatedValue3;
					}
					else
					{
						Vector3 eulerAngles2 = interpolatedValue3.eulerAngles;
						if (SyncRotAngleX)
						{
							eulerAngles.x = eulerAngles2.x;
						}
						if (SyncRotAngleY)
						{
							eulerAngles.y = eulerAngles2.y;
						}
						if (SyncRotAngleZ)
						{
							eulerAngles.z = eulerAngles2.z;
						}
						currentRotation.eulerAngles = eulerAngles;
					}
				}
			}
			else
			{
				if (UseHalfFloatPrecision)
				{
					if (localAuthoritativeNetworkState.HasPositionChange && SynchronizePosition)
					{
						currentPosition = localAuthoritativeNetworkState.CurrentPosition;
					}
					if (localAuthoritativeNetworkState.HasScaleChange && SynchronizeScale)
					{
						for (int i = 0; i < 3; i++)
						{
							if (m_LocalAuthoritativeNetworkState.HasScale(i))
							{
								currentScale[i] = m_LocalAuthoritativeNetworkState.Scale[i];
							}
						}
					}
				}
				else
				{
					if (localAuthoritativeNetworkState.HasPositionX)
					{
						currentPosition.x = localAuthoritativeNetworkState.PositionX;
					}
					if (localAuthoritativeNetworkState.HasPositionY)
					{
						currentPosition.y = localAuthoritativeNetworkState.PositionY;
					}
					if (localAuthoritativeNetworkState.HasPositionZ)
					{
						currentPosition.z = localAuthoritativeNetworkState.PositionZ;
					}
					if (localAuthoritativeNetworkState.HasScaleX)
					{
						currentScale.x = localAuthoritativeNetworkState.ScaleX;
					}
					if (localAuthoritativeNetworkState.HasScaleY)
					{
						currentScale.y = localAuthoritativeNetworkState.ScaleY;
					}
					if (localAuthoritativeNetworkState.HasScaleZ)
					{
						currentScale.z = localAuthoritativeNetworkState.ScaleZ;
					}
				}
				if (SynchronizeRotation)
				{
					if (localAuthoritativeNetworkState.QuaternionSync && localAuthoritativeNetworkState.HasRotAngleChange)
					{
						currentRotation = localAuthoritativeNetworkState.Rotation;
					}
					else
					{
						if (localAuthoritativeNetworkState.HasRotAngleX)
						{
							eulerAngles.x = localAuthoritativeNetworkState.RotAngleX;
						}
						if (localAuthoritativeNetworkState.HasRotAngleY)
						{
							eulerAngles.y = localAuthoritativeNetworkState.RotAngleY;
						}
						if (localAuthoritativeNetworkState.HasRotAngleZ)
						{
							eulerAngles.z = localAuthoritativeNetworkState.RotAngleZ;
						}
						currentRotation.eulerAngles = eulerAngles;
					}
				}
			}
			if (SynchronizePosition)
			{
				if (localAuthoritativeNetworkState.HasPositionChange || Interpolate)
				{
					m_CurrentPosition = currentPosition;
				}
				if (InLocalSpace)
				{
					base.transform.localPosition = m_CurrentPosition;
				}
				else
				{
					base.transform.position = m_CurrentPosition;
				}
			}
			if (SynchronizeRotation)
			{
				if (localAuthoritativeNetworkState.HasRotAngleChange || Interpolate)
				{
					m_CurrentRotation = currentRotation;
				}
				if (InLocalSpace)
				{
					base.transform.localRotation = m_CurrentRotation;
				}
				else
				{
					base.transform.rotation = m_CurrentRotation;
				}
			}
			if (SynchronizeScale)
			{
				if (localAuthoritativeNetworkState.HasScaleChange || Interpolate)
				{
					m_CurrentScale = currentScale;
				}
				base.transform.localScale = m_CurrentScale;
			}
		}

		private void ApplyTeleportingState(NetworkTransformState newState)
		{
			if (!newState.IsTeleportingNextFrame)
			{
				return;
			}
			double sentTime = newState.SentTime;
			Vector3 vector = GetSpaceRelativePosition();
			Quaternion quaternion = GetSpaceRelativeRotation();
			Vector3 eulerAngles = quaternion.eulerAngles;
			Vector3 vector2 = base.transform.localScale;
			bool isSynchronizing = newState.IsSynchronizing;
			m_ScaleInterpolator.Clear();
			m_PositionInterpolator.Clear();
			m_RotationInterpolator.Clear();
			if (newState.HasPositionChange)
			{
				if (!UseHalfFloatPrecision)
				{
					if (newState.HasPositionX)
					{
						vector.x = newState.PositionX;
					}
					if (newState.HasPositionY)
					{
						vector.y = newState.PositionY;
					}
					if (newState.HasPositionZ)
					{
						vector.z = newState.PositionZ;
					}
					UpdatePositionInterpolator(vector, sentTime, resetInterpolator: true);
				}
				else
				{
					m_HalfPositionState = new NetworkDeltaPosition(newState.CurrentPosition, newState.NetworkTick, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
					if (isSynchronizing)
					{
						if (ShouldSynchronizeHalfFloat(base.NetworkManager.LocalClientId))
						{
							m_HalfPositionState.HalfVector3.Axis = newState.NetworkDeltaPosition.HalfVector3.Axis;
							m_HalfPositionState.DeltaPosition = newState.DeltaPosition;
							vector = m_HalfPositionState.ToVector3(newState.NetworkTick);
						}
						else
						{
							vector = newState.CurrentPosition;
						}
						AddLogEntry(ref newState, base.NetworkObject.OwnerClientId, preUpdate: true);
					}
					else
					{
						vector = newState.CurrentPosition;
					}
					if (Interpolate)
					{
						UpdatePositionInterpolator(vector, sentTime, resetInterpolator: true);
					}
				}
				m_CurrentPosition = vector;
				m_TargetPosition = vector;
				if (newState.InLocalSpace)
				{
					base.transform.localPosition = vector;
				}
				else
				{
					base.transform.position = vector;
				}
			}
			if (newState.HasScaleChange)
			{
				bool flag = false;
				if (newState.IsParented)
				{
					flag = ((!(base.transform.parent == null)) ? (!base.NetworkObject.WorldPositionStays()) : base.NetworkObject.WorldPositionStays());
				}
				if (UseHalfFloatPrecision)
				{
					vector2 = (flag ? newState.LossyScale : newState.Scale);
				}
				else
				{
					if (newState.HasScaleX)
					{
						vector2.x = (flag ? newState.LossyScale.x : newState.ScaleX);
					}
					if (newState.HasScaleY)
					{
						vector2.y = (flag ? newState.LossyScale.y : newState.ScaleY);
					}
					if (newState.HasScaleZ)
					{
						vector2.z = (flag ? newState.LossyScale.z : newState.ScaleZ);
					}
				}
				m_CurrentScale = vector2;
				m_TargetScale = vector2;
				m_ScaleInterpolator.ResetTo(vector2, sentTime);
				base.transform.localScale = vector2;
			}
			if (newState.HasRotAngleChange)
			{
				if (newState.QuaternionSync)
				{
					quaternion = newState.Rotation;
				}
				else
				{
					if (newState.HasRotAngleX)
					{
						eulerAngles.x = newState.RotAngleX;
					}
					if (newState.HasRotAngleY)
					{
						eulerAngles.y = newState.RotAngleY;
					}
					if (newState.HasRotAngleZ)
					{
						eulerAngles.z = newState.RotAngleZ;
					}
					quaternion.eulerAngles = eulerAngles;
				}
				m_CurrentRotation = quaternion;
				m_TargetRotation = quaternion.eulerAngles;
				m_RotationInterpolator.ResetTo(quaternion, sentTime);
				if (InLocalSpace)
				{
					base.transform.localRotation = quaternion;
				}
				else
				{
					base.transform.rotation = quaternion;
				}
			}
			if (isSynchronizing)
			{
				AddLogEntry(ref newState, base.NetworkObject.OwnerClientId);
			}
		}

		private void ApplyUpdatedState(NetworkTransformState newState)
		{
			InLocalSpace = newState.InLocalSpace;
			Interpolate = newState.UseInterpolation;
			UseQuaternionSynchronization = newState.QuaternionSync;
			UseQuaternionCompression = newState.QuaternionCompression;
			UseHalfFloatPrecision = newState.UseHalfFloatPrecision;
			if (SlerpPosition != newState.UsePositionSlerp)
			{
				SlerpPosition = newState.UsePositionSlerp;
				UpdatePositionSlerp();
			}
			m_LocalAuthoritativeNetworkState = newState;
			if (m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame)
			{
				ApplyTeleportingState(m_LocalAuthoritativeNetworkState);
				return;
			}
			double sentTime = newState.SentTime;
			Quaternion newMeasurement = GetSpaceRelativeRotation();
			Vector3 eulerAngles = newMeasurement.eulerAngles;
			if (UseHalfFloatPrecision && m_LocalAuthoritativeNetworkState.HasPositionChange)
			{
				m_HalfPositionState.HalfVector3.Axis = m_LocalAuthoritativeNetworkState.NetworkDeltaPosition.HalfVector3.Axis;
				m_TargetPosition = m_HalfPositionState.ToVector3(newState.NetworkTick);
				m_LocalAuthoritativeNetworkState.CurrentPosition = m_TargetPosition;
			}
			if (!Interpolate)
			{
				return;
			}
			if (m_LocalAuthoritativeNetworkState.HasPositionChange)
			{
				if (!m_LocalAuthoritativeNetworkState.UseHalfFloatPrecision)
				{
					Vector3 targetPosition = m_TargetPosition;
					if (m_LocalAuthoritativeNetworkState.HasPositionX)
					{
						targetPosition.x = m_LocalAuthoritativeNetworkState.PositionX;
					}
					if (m_LocalAuthoritativeNetworkState.HasPositionY)
					{
						targetPosition.y = m_LocalAuthoritativeNetworkState.PositionY;
					}
					if (m_LocalAuthoritativeNetworkState.HasPositionZ)
					{
						targetPosition.z = m_LocalAuthoritativeNetworkState.PositionZ;
					}
					m_TargetPosition = targetPosition;
				}
				UpdatePositionInterpolator(m_TargetPosition, sentTime);
			}
			if (m_LocalAuthoritativeNetworkState.HasScaleChange)
			{
				Vector3 targetScale = m_TargetScale;
				if (UseHalfFloatPrecision)
				{
					for (int i = 0; i < 3; i++)
					{
						if (m_LocalAuthoritativeNetworkState.HasScale(i))
						{
							targetScale[i] = m_LocalAuthoritativeNetworkState.Scale[i];
						}
					}
				}
				else
				{
					if (m_LocalAuthoritativeNetworkState.HasScaleX)
					{
						targetScale.x = m_LocalAuthoritativeNetworkState.ScaleX;
					}
					if (m_LocalAuthoritativeNetworkState.HasScaleY)
					{
						targetScale.y = m_LocalAuthoritativeNetworkState.ScaleY;
					}
					if (m_LocalAuthoritativeNetworkState.HasScaleZ)
					{
						targetScale.z = m_LocalAuthoritativeNetworkState.ScaleZ;
					}
				}
				m_TargetScale = targetScale;
				m_ScaleInterpolator.AddMeasurement(targetScale, sentTime);
			}
			if (!m_LocalAuthoritativeNetworkState.HasRotAngleChange)
			{
				return;
			}
			if (m_LocalAuthoritativeNetworkState.QuaternionSync)
			{
				newMeasurement = m_LocalAuthoritativeNetworkState.Rotation;
			}
			else
			{
				eulerAngles = m_TargetRotation;
				if (m_LocalAuthoritativeNetworkState.HasRotAngleX)
				{
					eulerAngles.x = m_LocalAuthoritativeNetworkState.RotAngleX;
				}
				if (m_LocalAuthoritativeNetworkState.HasRotAngleY)
				{
					eulerAngles.y = m_LocalAuthoritativeNetworkState.RotAngleY;
				}
				if (m_LocalAuthoritativeNetworkState.HasRotAngleZ)
				{
					eulerAngles.z = m_LocalAuthoritativeNetworkState.RotAngleZ;
				}
				m_TargetRotation = eulerAngles;
				newMeasurement.eulerAngles = eulerAngles;
			}
			m_RotationInterpolator.AddMeasurement(newMeasurement, sentTime);
		}

		protected virtual void OnNetworkTransformStateUpdated(ref NetworkTransformState oldState, ref NetworkTransformState newState)
		{
		}

		private void OnNetworkStateChanged(NetworkTransformState oldState, NetworkTransformState newState)
		{
			if (base.NetworkObject.IsSpawned && !CanCommitToTransform)
			{
				newState.SentTime = new NetworkTime(base.NetworkManager.NetworkConfig.TickRate, newState.NetworkTick).Time;
				ApplyUpdatedState(newState);
				OnNetworkTransformStateUpdated(ref oldState, ref newState);
			}
		}

		public void SetMaxInterpolationBound(float maxInterpolationBound)
		{
			m_RotationInterpolator.MaxInterpolationBound = maxInterpolationBound;
			m_PositionInterpolator.MaxInterpolationBound = maxInterpolationBound;
			m_ScaleInterpolator.MaxInterpolationBound = maxInterpolationBound;
		}

		protected virtual void Awake()
		{
			m_RotationInterpolator = new BufferedLinearInterpolatorQuaternion();
			m_PositionInterpolator = new BufferedLinearInterpolatorVector3();
			m_ScaleInterpolator = new BufferedLinearInterpolatorVector3();
		}

		private void AxisChangedDeltaPositionCheck()
		{
			if (!UseHalfFloatPrecision || !SynchronizePosition)
			{
				return;
			}
			bool3 axisToSynchronize = m_HalfPositionState.HalfVector3.AxisToSynchronize;
			if (SyncPositionX != axisToSynchronize.x || SyncPositionY != axisToSynchronize.y || SyncPositionZ != axisToSynchronize.z)
			{
				Vector3 fullPosition = m_HalfPositionState.GetFullPosition();
				Vector3 spaceRelativePosition = GetSpaceRelativePosition();
				bool isTeleportingNextFrame = false;
				if (SyncPositionX && SyncPositionX != axisToSynchronize.x)
				{
					isTeleportingNextFrame = Mathf.Abs(spaceRelativePosition.x - fullPosition.x) >= 64f;
				}
				if (SyncPositionY && SyncPositionY != axisToSynchronize.y)
				{
					isTeleportingNextFrame = Mathf.Abs(spaceRelativePosition.y - fullPosition.y) >= 64f;
				}
				if (SyncPositionZ && SyncPositionZ != axisToSynchronize.z)
				{
					isTeleportingNextFrame = Mathf.Abs(spaceRelativePosition.z - fullPosition.z) >= 64f;
				}
				m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = isTeleportingNextFrame;
			}
		}

		internal void OnUpdateAuthoritativeState(ref Transform transformSource)
		{
			if (m_LocalAuthoritativeNetworkState.IsDirty && !m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame)
			{
				m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();
				if (TrackByStateId)
				{
					m_LocalAuthoritativeNetworkState.TrackByStateId = true;
					m_LocalAuthoritativeNetworkState.StateId++;
				}
				else
				{
					m_LocalAuthoritativeNetworkState.TrackByStateId = false;
				}
			}
			AxisChangedDeltaPositionCheck();
			TryCommitTransform(ref transformSource);
		}

		private void NetworkTickSystem_Tick()
		{
			if (CanCommitToTransform)
			{
				Transform transformSource = base.transform;
				OnUpdateAuthoritativeState(ref transformSource);
			}
			else if (base.NetworkManager != null && base.NetworkManager.NetworkTickSystem != null)
			{
				base.NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
			}
		}

		public override void OnNetworkSpawn()
		{
			m_CachedIsServer = base.IsServer;
			m_CachedNetworkManager = base.NetworkManager;
			m_MessageName = $"NTU_{base.NetworkObjectId}_{base.NetworkBehaviourId}";
			base.NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler(m_MessageName, TransformStateUpdate);
			Initialize();
		}

		public override void OnNetworkDespawn()
		{
			if (!base.NetworkManager.ShutdownInProgress && base.NetworkManager.CustomMessagingManager != null)
			{
				base.NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler(m_MessageName);
			}
			CanCommitToTransform = false;
			if (base.NetworkManager != null && base.NetworkManager.NetworkTickSystem != null)
			{
				base.NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
			}
		}

		public override void OnDestroy()
		{
			if (base.NetworkManager != null && base.NetworkManager.NetworkTickSystem != null)
			{
				base.NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
			}
			CanCommitToTransform = false;
			base.OnDestroy();
		}

		public override void OnGainedOwnership()
		{
			if (base.OwnerClientId == base.NetworkManager.LocalClientId)
			{
				Initialize();
			}
		}

		public override void OnLostOwnership()
		{
			if (base.OwnerClientId != base.NetworkManager.LocalClientId)
			{
				Initialize();
			}
		}

		protected virtual void OnInitialize(ref NetworkTransformState replicatedState)
		{
		}

		protected virtual void OnInitialize(ref NetworkVariable<NetworkTransformState> replicatedState)
		{
		}

		protected void Initialize()
		{
			if (!base.IsSpawned)
			{
				return;
			}
			CanCommitToTransform = (IsServerAuthoritative() ? base.IsServer : base.IsOwner);
			Vector3 spaceRelativePosition = GetSpaceRelativePosition();
			Quaternion spaceRelativeRotation = GetSpaceRelativeRotation();
			if (CanCommitToTransform)
			{
				if (UseHalfFloatPrecision)
				{
					m_HalfPositionState = new NetworkDeltaPosition(spaceRelativePosition, base.NetworkManager.NetworkTickSystem.ServerTime.Tick, math.bool3(SyncPositionX, SyncPositionY, SyncPositionZ));
				}
				base.NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
				base.NetworkManager.NetworkTickSystem.Tick += NetworkTickSystem_Tick;
				SetStateInternal(spaceRelativePosition, spaceRelativeRotation, base.transform.localScale, shouldTeleport: true);
			}
			else
			{
				base.NetworkManager.NetworkTickSystem.Tick -= NetworkTickSystem_Tick;
				ResetInterpolatedStateToCurrentAuthoritativeState();
				m_CurrentPosition = spaceRelativePosition;
				m_TargetPosition = spaceRelativePosition;
				m_CurrentScale = base.transform.localScale;
				m_TargetScale = base.transform.localScale;
				m_CurrentRotation = spaceRelativeRotation;
				m_TargetRotation = spaceRelativeRotation.eulerAngles;
			}
			OnInitialize(ref m_LocalAuthoritativeNetworkState);
			if (base.IsOwner)
			{
				m_InternalStatNetVar.Value = m_LocalAuthoritativeNetworkState;
				OnInitialize(ref m_InternalStatNetVar);
			}
		}

		public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
		{
			if (!CanCommitToTransform)
			{
				m_CurrentPosition = GetSpaceRelativePosition();
				m_CurrentRotation = GetSpaceRelativeRotation();
				m_CurrentScale = GetScale();
				m_ScaleInterpolator.Clear();
				m_PositionInterpolator.Clear();
				m_RotationInterpolator.Clear();
				double time = new NetworkTime(base.NetworkManager.NetworkConfig.TickRate, base.NetworkManager.ServerTime.Tick).Time;
				UpdatePositionInterpolator(m_CurrentPosition, time, resetInterpolator: true);
				m_ScaleInterpolator.ResetTo(m_CurrentScale, time);
				m_RotationInterpolator.ResetTo(m_CurrentRotation, time);
			}
			base.OnNetworkObjectParentChanged(parentNetworkObject);
		}

		public void SetState(Vector3? posIn = null, Quaternion? rotIn = null, Vector3? scaleIn = null, bool teleportDisabled = true)
		{
			if (!base.IsSpawned)
			{
				NetworkLog.LogError("Cannot commit transform when not spawned!");
				return;
			}
			if (!base.IsServer && !base.IsOwner)
			{
				NetworkLog.LogError((base.gameObject != base.NetworkObject.gameObject) ? ("Non-authority instance of " + base.NetworkObject.gameObject.name + " is trying to commit a transform on " + base.gameObject.name + "!") : ("Non-authority instance of " + base.NetworkObject.gameObject.name + " is trying to commit a transform!"));
				return;
			}
			Vector3 pos = ((!posIn.HasValue) ? GetSpaceRelativePosition() : posIn.Value);
			Quaternion rot = ((!rotIn.HasValue) ? GetSpaceRelativeRotation() : rotIn.Value);
			Vector3 scale = ((!scaleIn.HasValue) ? base.transform.localScale : scaleIn.Value);
			if (!CanCommitToTransform)
			{
				if (base.IsServer)
				{
					m_ClientIds[0] = base.OwnerClientId;
					m_ClientRpcParams.Send.TargetClientIds = m_ClientIds;
					SetStateClientRpc(pos, rot, scale, !teleportDisabled, m_ClientRpcParams);
				}
				else
				{
					SetStateServerRpc(pos, rot, scale, !teleportDisabled);
				}
			}
			else
			{
				SetStateInternal(pos, rot, scale, !teleportDisabled);
			}
		}

		private void SetStateInternal(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport)
		{
			if (InLocalSpace)
			{
				base.transform.localPosition = pos;
				base.transform.localRotation = rot;
			}
			else
			{
				base.transform.SetPositionAndRotation(pos, rot);
			}
			base.transform.localScale = scale;
			m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;
			Transform transformToCommit = base.transform;
			TryCommitTransform(ref transformToCommit);
		}

		[ClientRpc]
		private void SetStateClientRpc(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport, ClientRpcParams clientRpcParams = default(ClientRpcParams))
		{
			NetworkManager networkManager = base.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				if (__rpc_exec_stage != __RpcExecStage.Client && (networkManager.IsServer || networkManager.IsHost))
				{
					FastBufferWriter bufferWriter = __beginSendClientRpc(1724438000u, clientRpcParams, RpcDelivery.Reliable);
					bufferWriter.WriteValueSafe(in pos);
					bufferWriter.WriteValueSafe(in rot);
					bufferWriter.WriteValueSafe(in scale);
					bufferWriter.WriteValueSafe(in shouldTeleport, default(FastBufferWriter.ForPrimitives));
					__endSendClientRpc(ref bufferWriter, 1724438000u, clientRpcParams, RpcDelivery.Reliable);
				}
				if (__rpc_exec_stage == __RpcExecStage.Client && (networkManager.IsClient || networkManager.IsHost))
				{
					SetStateInternal(pos, rot, scale, shouldTeleport);
				}
			}
		}

		[ServerRpc]
		private void SetStateServerRpc(Vector3 pos, Quaternion rot, Vector3 scale, bool shouldTeleport)
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
				FastBufferWriter bufferWriter = __beginSendServerRpc(640767722u, serverRpcParams, RpcDelivery.Reliable);
				bufferWriter.WriteValueSafe(in pos);
				bufferWriter.WriteValueSafe(in rot);
				bufferWriter.WriteValueSafe(in scale);
				bufferWriter.WriteValueSafe(in shouldTeleport, default(FastBufferWriter.ForPrimitives));
				__endSendServerRpc(ref bufferWriter, 640767722u, serverRpcParams, RpcDelivery.Reliable);
			}
			if (__rpc_exec_stage == __RpcExecStage.Server && (networkManager.IsServer || networkManager.IsHost))
			{
				if (OnClientRequestChange != null)
				{
					(pos, rot, scale) = OnClientRequestChange(pos, rot, scale);
				}
				SetStateInternal(pos, rot, scale, shouldTeleport);
			}
		}

		protected virtual void Update()
		{
			if (!base.IsSpawned || CanCommitToTransform)
			{
				return;
			}
			if (Interpolate)
			{
				NetworkTime serverTime = base.NetworkManager.ServerTime;
				float deltaTime = base.NetworkManager.RealTimeProvider.DeltaTime;
				double time = serverTime.Time;
				int ticks = ((IsServerAuthoritative() || base.IsServer) ? 1 : 2);
				double time2 = serverTime.TimeTicksAgo(ticks).Time;
				if (SynchronizePosition)
				{
					m_PositionInterpolator.Update(deltaTime, time2, time);
				}
				if (SynchronizeRotation)
				{
					m_RotationInterpolator.IsSlerp = !UseHalfFloatPrecision;
					m_RotationInterpolator.Update(deltaTime, time2, time);
				}
				if (SynchronizeScale)
				{
					m_ScaleInterpolator.Update(deltaTime, time2, time);
				}
			}
			ApplyAuthoritativeState();
		}

		public void Teleport(Vector3 newPosition, Quaternion newRotation, Vector3 newScale)
		{
			if (!CanCommitToTransform)
			{
				throw new Exception("Teleporting on non-authoritative side is not allowed!");
			}
			SetStateInternal(newPosition, newRotation, newScale, shouldTeleport: true);
		}

		protected virtual bool OnIsServerAuthoritative()
		{
			return true;
		}

		public bool IsServerAuthoritative()
		{
			return OnIsServerAuthoritative();
		}

		private void TransformStateUpdate(ulong senderId, FastBufferReader messagePayload)
		{
			if (base.IsServer && !OnIsServerAuthoritative())
			{
				ForwardStateUpdateMessage(messagePayload);
			}
			m_OldState = m_LocalAuthoritativeNetworkState;
			messagePayload.ReadNetworkSerializableInPlace(ref m_LocalAuthoritativeNetworkState);
			OnNetworkStateChanged(m_OldState, m_LocalAuthoritativeNetworkState);
		}

		private unsafe void ForwardStateUpdateMessage(FastBufferReader messagePayload)
		{
			int position = messagePayload.Position;
			int size = messagePayload.Length - position;
			FastBufferWriter fastBufferWriter = new FastBufferWriter(size, Allocator.Temp);
			using (fastBufferWriter)
			{
				fastBufferWriter.WriteBytesSafe(messagePayload.GetUnsafePtr(), size, position);
				int count = base.NetworkManager.ConnectionManager.ConnectedClientsList.Count;
				for (int i = 0; i < count; i++)
				{
					ulong clientId = base.NetworkManager.ConnectionManager.ConnectedClientsList[i].ClientId;
					if (OnIsServerAuthoritative() || (clientId != 0L && clientId != base.OwnerClientId))
					{
						base.NetworkManager.CustomMessagingManager.SendNamedMessage(m_MessageName, clientId, fastBufferWriter);
					}
				}
			}
			messagePayload.Seek(position);
		}

		private void UpdateTransformState()
		{
			if (base.NetworkManager.ShutdownInProgress)
			{
				return;
			}
			bool flag = OnIsServerAuthoritative();
			if (flag && !base.IsServer)
			{
				Debug.LogError("Server authoritative NetworkTransform can only be updated by the server!");
			}
			else if (!flag && !base.IsServer && !base.IsOwner)
			{
				Debug.LogError("Owner authoritative NetworkTransform can only be updated by the owner!");
			}
			CustomMessagingManager customMessagingManager = base.NetworkManager.CustomMessagingManager;
			FastBufferWriter fastBufferWriter = new FastBufferWriter(128, Allocator.Temp);
			using (fastBufferWriter)
			{
				fastBufferWriter.WriteNetworkSerializable(in m_LocalAuthoritativeNetworkState);
				if (base.IsServer)
				{
					int count = base.NetworkManager.ConnectionManager.ConnectedClientsList.Count;
					for (int i = 0; i < count; i++)
					{
						ulong clientId = base.NetworkManager.ConnectionManager.ConnectedClientsList[i].ClientId;
						if (clientId != 0L)
						{
							customMessagingManager.SendNamedMessage(m_MessageName, clientId, fastBufferWriter);
						}
					}
				}
				else
				{
					customMessagingManager.SendNamedMessage(m_MessageName, 0uL, fastBufferWriter);
				}
			}
		}

		protected override void __initializeVariables()
		{
			if (m_InternalStatNetVar == null)
			{
				throw new Exception("NetworkTransform.m_InternalStatNetVar cannot be null. All NetworkVariableBase instances must be initialized.");
			}
			m_InternalStatNetVar.Initialize(this);
			__nameNetworkVariable(m_InternalStatNetVar, "m_InternalStatNetVar");
			NetworkVariableFields.Add(m_InternalStatNetVar);
			base.__initializeVariables();
		}

		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeRPCS_NetworkTransform()
		{
			NetworkManager.__rpc_func_table.Add(1724438000u, __rpc_handler_1724438000);
			NetworkManager.__rpc_func_table.Add(640767722u, __rpc_handler_640767722);
		}

		private static void __rpc_handler_1724438000(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
		{
			NetworkManager networkManager = target.NetworkManager;
			if ((object)networkManager != null && networkManager.IsListening)
			{
				reader.ReadValueSafe(out Vector3 value);
				reader.ReadValueSafe(out Quaternion value2);
				reader.ReadValueSafe(out Vector3 value3);
				reader.ReadValueSafe(out bool value4, default(FastBufferWriter.ForPrimitives));
				ClientRpcParams client = rpcParams.Client;
				target.__rpc_exec_stage = __RpcExecStage.Client;
				((NetworkTransform)target).SetStateClientRpc(value, value2, value3, value4, client);
				target.__rpc_exec_stage = __RpcExecStage.None;
			}
		}

		private static void __rpc_handler_640767722(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
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
				return;
			}
			reader.ReadValueSafe(out Vector3 value);
			reader.ReadValueSafe(out Quaternion value2);
			reader.ReadValueSafe(out Vector3 value3);
			reader.ReadValueSafe(out bool value4, default(FastBufferWriter.ForPrimitives));
			target.__rpc_exec_stage = __RpcExecStage.Server;
			((NetworkTransform)target).SetStateServerRpc(value, value2, value3, value4);
			target.__rpc_exec_stage = __RpcExecStage.None;
		}

		protected internal override string __getTypeName()
		{
			return "NetworkTransform";
		}
	}
}
