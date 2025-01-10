using System;

namespace Unity.Netcode
{
	public struct ForceNetworkSerializeByMemcpy<T> : INetworkSerializeByMemcpy, IEquatable<ForceNetworkSerializeByMemcpy<T>> where T : unmanaged, IEquatable<T>
	{
		public T Value;

		public ForceNetworkSerializeByMemcpy(T value)
		{
			Value = value;
		}

		public static implicit operator T(ForceNetworkSerializeByMemcpy<T> container)
		{
			return container.Value;
		}

		public static implicit operator ForceNetworkSerializeByMemcpy<T>(T underlyingValue)
		{
			ForceNetworkSerializeByMemcpy<T> result = default(ForceNetworkSerializeByMemcpy<T>);
			result.Value = underlyingValue;
			return result;
		}

		public bool Equals(ForceNetworkSerializeByMemcpy<T> other)
		{
			return Value.Equals(other.Value);
		}

		public override bool Equals(object obj)
		{
			if (obj is ForceNetworkSerializeByMemcpy<T> other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
