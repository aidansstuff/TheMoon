using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Netcode
{
	[Serializable]
	public static class NetworkVariableSerialization<T>
	{
		internal delegate bool EqualsDelegate(ref T a, ref T b);

		internal static INetworkVariableSerializer<T> Serializer = new FallbackSerializer<T>();

		internal static EqualsDelegate AreEqual;

		internal unsafe static bool ValueEquals<TValueType>(ref TValueType a, ref TValueType b) where TValueType : unmanaged
		{
			void* ptr = UnsafeUtility.AddressOf(ref a);
			void* ptr2 = UnsafeUtility.AddressOf(ref b);
			return UnsafeUtility.MemCmp(ptr, ptr2, sizeof(TValueType)) == 0;
		}

		internal unsafe static bool ValueEqualsArray<TValueType>(ref NativeArray<TValueType> a, ref NativeArray<TValueType> b) where TValueType : unmanaged
		{
			if (a.IsCreated != b.IsCreated)
			{
				return false;
			}
			if (!a.IsCreated)
			{
				return true;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			TValueType* unsafePtr = (TValueType*)a.GetUnsafePtr();
			TValueType* unsafePtr2 = (TValueType*)b.GetUnsafePtr();
			return UnsafeUtility.MemCmp(unsafePtr, unsafePtr2, sizeof(TValueType) * a.Length) == 0;
		}

		internal static bool EqualityEqualsObject<TValueType>(ref TValueType a, ref TValueType b) where TValueType : class, IEquatable<TValueType>
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		internal static bool EqualityEquals<TValueType>(ref TValueType a, ref TValueType b) where TValueType : unmanaged, IEquatable<TValueType>
		{
			return a.Equals(b);
		}

		internal unsafe static bool EqualityEqualsArray<TValueType>(ref NativeArray<TValueType> a, ref NativeArray<TValueType> b) where TValueType : unmanaged, IEquatable<TValueType>
		{
			if (a.IsCreated != b.IsCreated)
			{
				return false;
			}
			if (!a.IsCreated)
			{
				return true;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			TValueType* unsafePtr = (TValueType*)a.GetUnsafePtr();
			TValueType* unsafePtr2 = (TValueType*)b.GetUnsafePtr();
			for (int i = 0; i < a.Length; i++)
			{
				if (!EqualityEquals(ref unsafePtr[i], ref unsafePtr2[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool ClassEquals<TValueType>(ref TValueType a, ref TValueType b) where TValueType : class
		{
			return a == b;
		}

		internal static void Write(FastBufferWriter writer, ref T value)
		{
			Serializer.Write(writer, ref value);
		}

		internal static void Read(FastBufferReader reader, ref T value)
		{
			Serializer.Read(reader, ref value);
		}
	}
}
