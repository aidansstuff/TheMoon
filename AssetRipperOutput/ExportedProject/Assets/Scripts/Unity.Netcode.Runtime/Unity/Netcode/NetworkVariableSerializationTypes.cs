using System;
using Unity.Collections;
using UnityEngine;

namespace Unity.Netcode
{
	public static class NetworkVariableSerializationTypes
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		internal static void InitializeIntegerSerialization()
		{
			NetworkVariableSerialization<short>.Serializer = new ShortSerializer();
			NetworkVariableSerialization<short>.AreEqual = NetworkVariableSerialization<short>.ValueEquals;
			NetworkVariableSerialization<ushort>.Serializer = new UshortSerializer();
			NetworkVariableSerialization<ushort>.AreEqual = NetworkVariableSerialization<ushort>.ValueEquals;
			NetworkVariableSerialization<int>.Serializer = new IntSerializer();
			NetworkVariableSerialization<int>.AreEqual = NetworkVariableSerialization<int>.ValueEquals;
			NetworkVariableSerialization<uint>.Serializer = new UintSerializer();
			NetworkVariableSerialization<uint>.AreEqual = NetworkVariableSerialization<uint>.ValueEquals;
			NetworkVariableSerialization<long>.Serializer = new LongSerializer();
			NetworkVariableSerialization<long>.AreEqual = NetworkVariableSerialization<long>.ValueEquals;
			NetworkVariableSerialization<ulong>.Serializer = new UlongSerializer();
			NetworkVariableSerialization<ulong>.AreEqual = NetworkVariableSerialization<ulong>.ValueEquals;
		}

		public static void InitializeSerializer_UnmanagedByMemcpy<T>() where T : unmanaged
		{
			NetworkVariableSerialization<T>.Serializer = new UnmanagedTypeSerializer<T>();
		}

		public static void InitializeSerializer_UnmanagedByMemcpyArray<T>() where T : unmanaged
		{
			NetworkVariableSerialization<NativeArray<T>>.Serializer = new UnmanagedArraySerializer<T>();
		}

		public static void InitializeSerializer_UnmanagedINetworkSerializable<T>() where T : unmanaged, INetworkSerializable
		{
			NetworkVariableSerialization<T>.Serializer = new UnmanagedNetworkSerializableSerializer<T>();
		}

		public static void InitializeSerializer_UnmanagedINetworkSerializableArray<T>() where T : unmanaged, INetworkSerializable
		{
			NetworkVariableSerialization<NativeArray<T>>.Serializer = new UnmanagedNetworkSerializableArraySerializer<T>();
		}

		public static void InitializeSerializer_ManagedINetworkSerializable<T>() where T : class, INetworkSerializable, new()
		{
			NetworkVariableSerialization<T>.Serializer = new ManagedNetworkSerializableSerializer<T>();
		}

		public static void InitializeSerializer_FixedString<T>() where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			NetworkVariableSerialization<T>.Serializer = new FixedStringSerializer<T>();
		}

		public static void InitializeSerializer_FixedStringArray<T>() where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			NetworkVariableSerialization<NativeArray<T>>.Serializer = new FixedStringArraySerializer<T>();
		}

		public static void InitializeEqualityChecker_ManagedIEquatable<T>() where T : class, IEquatable<T>
		{
			NetworkVariableSerialization<T>.AreEqual = NetworkVariableSerialization<T>.EqualityEqualsObject;
		}

		public static void InitializeEqualityChecker_UnmanagedIEquatable<T>() where T : unmanaged, IEquatable<T>
		{
			NetworkVariableSerialization<T>.AreEqual = NetworkVariableSerialization<T>.EqualityEquals;
		}

		public static void InitializeEqualityChecker_UnmanagedIEquatableArray<T>() where T : unmanaged, IEquatable<T>
		{
			NetworkVariableSerialization<NativeArray<T>>.AreEqual = NetworkVariableSerialization<T>.EqualityEqualsArray;
		}

		public static void InitializeEqualityChecker_UnmanagedValueEquals<T>() where T : unmanaged
		{
			NetworkVariableSerialization<T>.AreEqual = NetworkVariableSerialization<T>.ValueEquals;
		}

		public static void InitializeEqualityChecker_UnmanagedValueEqualsArray<T>() where T : unmanaged
		{
			NetworkVariableSerialization<NativeArray<T>>.AreEqual = NetworkVariableSerialization<T>.ValueEqualsArray;
		}

		public static void InitializeEqualityChecker_ManagedClassEquals<T>() where T : class
		{
			NetworkVariableSerialization<T>.AreEqual = NetworkVariableSerialization<T>.ClassEquals;
		}
	}
}
