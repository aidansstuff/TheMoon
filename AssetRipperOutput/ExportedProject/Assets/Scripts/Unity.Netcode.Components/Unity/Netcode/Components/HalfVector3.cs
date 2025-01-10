using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Netcode.Components
{
	public struct HalfVector3 : INetworkSerializable
	{
		internal const int Length = 3;

		public half3 Axis;

		public bool3 AxisToSynchronize;

		public half X => Axis.x;

		public half Y => Axis.y;

		public half Z => Axis.z;

		internal void Set(float x, float y, float z)
		{
			Axis.x = math.half(x);
			Axis.y = math.half(y);
			Axis.z = math.half(z);
		}

		private void SerializeWrite(FastBufferWriter writer)
		{
			for (int i = 0; i < 3; i++)
			{
				if (AxisToSynchronize[i])
				{
					half value = Axis[i];
					writer.WriteUnmanagedSafe(in value);
				}
			}
		}

		private void SerializeRead(FastBufferReader reader)
		{
			for (int i = 0; i < 3; i++)
			{
				if (AxisToSynchronize[i])
				{
					half value = Axis[i];
					reader.ReadUnmanagedSafe(out value);
					Axis[i] = value;
				}
			}
		}

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			if (serializer.IsReader)
			{
				SerializeRead(serializer.GetFastBufferReader());
			}
			else
			{
				SerializeWrite(serializer.GetFastBufferWriter());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 ToVector3()
		{
			Vector3 zero = Vector3.zero;
			Vector3 vector = math.float3(Axis);
			for (int i = 0; i < 3; i++)
			{
				if (AxisToSynchronize[i])
				{
					zero[i] = vector[i];
				}
			}
			return zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateFrom(ref Vector3 vector3)
		{
			half3 half = math.half3(vector3);
			for (int i = 0; i < 3; i++)
			{
				if (AxisToSynchronize[i])
				{
					Axis[i] = half[i];
				}
			}
		}

		public HalfVector3(Vector3 vector3, bool3 axisToSynchronize)
		{
			Axis = half3.zero;
			AxisToSynchronize = axisToSynchronize;
			UpdateFrom(ref vector3);
		}

		public HalfVector3(Vector3 vector3)
			: this(vector3, math.bool3(v: true))
		{
		}

		public HalfVector3(float x, float y, float z, bool3 axisToSynchronize)
			: this(new Vector3(x, y, z), axisToSynchronize)
		{
		}

		public HalfVector3(float x, float y, float z)
			: this(new Vector3(x, y, z), math.bool3(v: true))
		{
		}
	}
}
