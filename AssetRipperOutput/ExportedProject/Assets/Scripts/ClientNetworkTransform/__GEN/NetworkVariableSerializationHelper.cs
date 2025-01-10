using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace __GEN
{
	internal class NetworkVariableSerializationHelper
	{
		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeSerialization()
		{
			NetworkVariableSerializationTypes.InitializeSerializer_UnmanagedINetworkSerializable<NetworkTransform.NetworkTransformState>();
			NetworkVariableSerializationTypes.InitializeEqualityChecker_UnmanagedValueEquals<NetworkTransform.NetworkTransformState>();
		}
	}
}
