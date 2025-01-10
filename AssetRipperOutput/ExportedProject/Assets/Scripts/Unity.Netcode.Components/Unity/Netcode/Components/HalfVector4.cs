using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Netcode.Components
{
	public struct HalfVector4 : INetworkSerializable
	{
		internal const int Length = 4;

		public half4 Axis;

		public half X => Axis.x;

		public half Y => Axis.y;

		public half Z => Axis.z;

		public half W => Axis.w;

		private void SerializeWrite(FastBufferWriter writer)
		{
			for (int i = 0; i < 4; i++)
			{
				half value = Axis[i];
				writer.WriteUnmanagedSafe(in value);
			}
		}

		private void SerializeRead(FastBufferReader reader)
		{
			for (int i = 0; i < 4; i++)
			{
				half value = Axis[i];
				reader.ReadUnmanagedSafe(out value);
				Axis[i] = value;
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
		public Vector4 ToVector4()
		{
			return math.float4(Axis);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion ToQuaternion()
		{
			return math.quaternion(Axis);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateFrom(ref Vector4 vector4)
		{
			Axis = math.half4(vector4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UpdateFrom(ref Quaternion quaternion)
		{
			Axis = math.half4(math.half(quaternion.x), math.half(quaternion.y), math.half(quaternion.z), math.half(quaternion.w));
		}

		public HalfVector4(Vector4 vector4)
		{
			Axis = default(half4);
			UpdateFrom(ref vector4);
		}

		public HalfVector4(float x, float y, float z, float w)
			: this(new Vector4(x, y, z, w))
		{
		}
	}
}
