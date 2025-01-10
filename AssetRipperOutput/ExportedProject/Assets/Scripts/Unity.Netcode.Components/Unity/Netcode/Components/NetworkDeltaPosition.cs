using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Netcode.Components
{
	public struct NetworkDeltaPosition : INetworkSerializable
	{
		internal const float MaxDeltaBeforeAdjustment = 64f;

		public HalfVector3 HalfVector3;

		internal Vector3 CurrentBasePosition;

		internal Vector3 PrecisionLossDelta;

		internal Vector3 HalfDeltaConvertedBack;

		internal Vector3 PreviousPosition;

		internal Vector3 DeltaPosition;

		internal int NetworkTick;

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			HalfVector3.NetworkSerialize(serializer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 ToVector3(int networkTick)
		{
			if (networkTick == NetworkTick)
			{
				return CurrentBasePosition + DeltaPosition;
			}
			for (int i = 0; i < 3; i++)
			{
				if (HalfVector3.AxisToSynchronize[i])
				{
					DeltaPosition[i] = Mathf.HalfToFloat(HalfVector3.Axis[i].value);
					if (Mathf.Abs(DeltaPosition[i]) >= 64f)
					{
						CurrentBasePosition[i] += DeltaPosition[i];
						DeltaPosition[i] = 0f;
						HalfVector3.Axis[i] = half.zero;
					}
				}
			}
			return CurrentBasePosition + DeltaPosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetCurrentBasePosition()
		{
			return CurrentBasePosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetFullPosition()
		{
			return CurrentBasePosition + DeltaPosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetConvertedDelta()
		{
			return HalfDeltaConvertedBack;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetDeltaPosition()
		{
			return DeltaPosition;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateFrom(ref Vector3 vector3, int networkTick)
		{
			NetworkTick = networkTick;
			DeltaPosition = vector3 + PrecisionLossDelta - CurrentBasePosition;
			for (int i = 0; i < 3; i++)
			{
				if (HalfVector3.AxisToSynchronize[i])
				{
					HalfVector3.Axis[i] = math.half(DeltaPosition[i]);
					HalfDeltaConvertedBack[i] = Mathf.HalfToFloat(HalfVector3.Axis[i].value);
					PrecisionLossDelta[i] = DeltaPosition[i] - HalfDeltaConvertedBack[i];
					if (Mathf.Abs(HalfDeltaConvertedBack[i]) >= 64f)
					{
						CurrentBasePosition[i] += HalfDeltaConvertedBack[i];
						HalfDeltaConvertedBack[i] = 0f;
						DeltaPosition[i] = 0f;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				if (HalfVector3.AxisToSynchronize[j])
				{
					PreviousPosition[j] = vector3[j];
				}
			}
		}

		public NetworkDeltaPosition(Vector3 vector3, int networkTick, bool3 axisToSynchronize)
		{
			NetworkTick = networkTick;
			CurrentBasePosition = vector3;
			PreviousPosition = vector3;
			PrecisionLossDelta = Vector3.zero;
			DeltaPosition = Vector3.zero;
			HalfDeltaConvertedBack = Vector3.zero;
			HalfVector3 = new HalfVector3(vector3, axisToSynchronize);
			UpdateFrom(ref vector3, networkTick);
		}

		public NetworkDeltaPosition(Vector3 vector3, int networkTick)
			: this(vector3, networkTick, math.bool3(v: true))
		{
		}

		public NetworkDeltaPosition(float x, float y, float z, int networkTick, bool3 axisToSynchronize)
			: this(new Vector3(x, y, z), networkTick, axisToSynchronize)
		{
		}

		public NetworkDeltaPosition(float x, float y, float z, int networkTick)
			: this(new Vector3(x, y, z), networkTick, math.bool3(v: true))
		{
		}
	}
}
