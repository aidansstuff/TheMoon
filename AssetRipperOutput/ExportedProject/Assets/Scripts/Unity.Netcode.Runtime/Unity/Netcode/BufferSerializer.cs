using System;
using Unity.Collections;
using UnityEngine;

namespace Unity.Netcode
{
	public ref struct BufferSerializer<TReaderWriter> where TReaderWriter : IReaderWriter
	{
		private TReaderWriter m_Implementation;

		public bool IsReader => m_Implementation.IsReader;

		public bool IsWriter => m_Implementation.IsWriter;

		internal BufferSerializer(TReaderWriter implementation)
		{
			m_Implementation = implementation;
		}

		public FastBufferReader GetFastBufferReader()
		{
			return m_Implementation.GetFastBufferReader();
		}

		public FastBufferWriter GetFastBufferWriter()
		{
			return m_Implementation.GetFastBufferWriter();
		}

		public void SerializeValue(ref string s, bool oneByteChars = false)
		{
			m_Implementation.SerializeValue(ref s, oneByteChars);
		}

		public void SerializeValue(ref byte value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue<T>(ref T value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForPrimitives));
		}

		public void SerializeValue<T>(ref T[] value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForPrimitives));
		}

		public void SerializeValue<T>(ref T value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForEnums));
		}

		public void SerializeValue<T>(ref T[] value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForEnums));
		}

		public void SerializeValue<T>(ref T value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForStructs));
		}

		public void SerializeValue<T>(ref T[] value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForStructs));
		}

		public void SerializeValue<T>(ref NativeArray<T> value, Allocator allocator, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			m_Implementation.SerializeValue(ref value, allocator, default(FastBufferWriter.ForGeneric));
		}

		public void SerializeValue<T>(ref T value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForNetworkSerializable));
		}

		public void SerializeValue<T>(ref T[] value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForNetworkSerializable));
		}

		public void SerializeValue(ref Vector2 value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector2[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector3 value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector3[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector2Int value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector2Int[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector3Int value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector3Int[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector4 value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Vector4[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Quaternion value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Quaternion[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Color value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Color[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Color32 value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Color32[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Ray value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Ray[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Ray2D value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue(ref Ray2D[] value)
		{
			m_Implementation.SerializeValue(ref value);
		}

		public void SerializeValue<T>(ref T value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			m_Implementation.SerializeValue(ref value, default(FastBufferWriter.ForFixedStrings));
		}

		public void SerializeValue<T>(ref NativeArray<T> value, Allocator allocator) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			m_Implementation.SerializeValue(ref value, allocator);
		}

		public void SerializeNetworkSerializable<T>(ref T value) where T : INetworkSerializable, new()
		{
			m_Implementation.SerializeNetworkSerializable(ref value);
		}

		public bool PreCheck(int amount)
		{
			return m_Implementation.PreCheck(amount);
		}

		public void SerializeValuePreChecked(ref string s, bool oneByteChars = false)
		{
			m_Implementation.SerializeValuePreChecked(ref s, oneByteChars);
		}

		public void SerializeValuePreChecked(ref byte value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked<T>(ref T value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForPrimitives));
		}

		public void SerializeValuePreChecked<T>(ref T[] value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForPrimitives));
		}

		public void SerializeValuePreChecked<T>(ref T value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForEnums));
		}

		public void SerializeValuePreChecked<T>(ref T[] value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForEnums));
		}

		public void SerializeValuePreChecked<T>(ref T value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForStructs));
		}

		public void SerializeValuePreChecked<T>(ref T[] value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForStructs));
		}

		public void SerializeValuePreChecked<T>(ref NativeArray<T> value, Allocator allocator, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			m_Implementation.SerializeValuePreChecked(ref value, allocator);
		}

		public void SerializeValuePreChecked(ref Vector2 value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector2[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector3 value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector3[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector2Int value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector2Int[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector3Int value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector3Int[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector4 value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Vector4[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Quaternion value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Quaternion[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Color value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Color[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Color32 value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Color32[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Ray value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Ray[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Ray2D value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked(ref Ray2D[] value)
		{
			m_Implementation.SerializeValuePreChecked(ref value);
		}

		public void SerializeValuePreChecked<T>(ref T value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			m_Implementation.SerializeValuePreChecked(ref value, default(FastBufferWriter.ForFixedStrings));
		}
	}
}
