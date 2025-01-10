using System;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class FallbackSerializer<T> : INetworkVariableSerializer<T>
	{
		public void Write(FastBufferWriter writer, ref T value)
		{
			if (UserNetworkVariableSerialization<T>.ReadValue == null || UserNetworkVariableSerialization<T>.WriteValue == null || UserNetworkVariableSerialization<T>.DuplicateValue == null)
			{
				throw new ArgumentException("Type " + typeof(T).FullName + " is not supported by " + typeof(NetworkVariable<>).Name + ". If this is a type you can change, then either implement INetworkSerializable or mark it as serializable by memcpy by adding INetworkSerializeByMemcpy to its interface list. If not, assign serialization code to UserNetworkVariableSerialization.WriteValue, UserNetworkVariableSerialization.ReadValue, and UserNetworkVariableSerialization.DuplicateValue, or if it's serializable by memcpy (contains no pointers), wrap it in " + typeof(ForceNetworkSerializeByMemcpy<>).Name + ".");
			}
			UserNetworkVariableSerialization<T>.WriteValue(writer, in value);
		}

		public void Read(FastBufferReader reader, ref T value)
		{
			if (UserNetworkVariableSerialization<T>.ReadValue == null || UserNetworkVariableSerialization<T>.WriteValue == null || UserNetworkVariableSerialization<T>.DuplicateValue == null)
			{
				throw new ArgumentException("Type " + typeof(T).FullName + " is not supported by " + typeof(NetworkVariable<>).Name + ". If this is a type you can change, then either implement INetworkSerializable or mark it as serializable by memcpy by adding INetworkSerializeByMemcpy to its interface list. If not, assign serialization code to UserNetworkVariableSerialization.WriteValue, UserNetworkVariableSerialization.ReadValue, and UserNetworkVariableSerialization.DuplicateValue, or if it's serializable by memcpy (contains no pointers), wrap it in " + typeof(ForceNetworkSerializeByMemcpy<>).Name + ".");
			}
			UserNetworkVariableSerialization<T>.ReadValue(reader, out value);
		}

		void INetworkVariableSerializer<T>.ReadWithAllocator(FastBufferReader reader, out T value, Allocator allocator)
		{
			throw new NotImplementedException();
		}

		public void Duplicate(in T value, ref T duplicatedValue)
		{
			if (UserNetworkVariableSerialization<T>.ReadValue == null || UserNetworkVariableSerialization<T>.WriteValue == null || UserNetworkVariableSerialization<T>.DuplicateValue == null)
			{
				throw new ArgumentException("Type " + typeof(T).FullName + " is not supported by " + typeof(NetworkVariable<>).Name + ". If this is a type you can change, then either implement INetworkSerializable or mark it as serializable by memcpy by adding INetworkSerializeByMemcpy to its interface list. If not, assign serialization code to UserNetworkVariableSerialization.WriteValue, UserNetworkVariableSerialization.ReadValue, and UserNetworkVariableSerialization.DuplicateValue, or if it's serializable by memcpy (contains no pointers), wrap it in " + typeof(ForceNetworkSerializeByMemcpy<>).Name + ".");
			}
			UserNetworkVariableSerialization<T>.DuplicateValue(in value, ref duplicatedValue);
		}

		void INetworkVariableSerializer<T>.Duplicate(in T value, ref T duplicatedValue)
		{
			Duplicate(in value, ref duplicatedValue);
		}
	}
}
