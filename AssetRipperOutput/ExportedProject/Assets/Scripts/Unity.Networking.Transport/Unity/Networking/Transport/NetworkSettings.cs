using System;
using System.Reflection;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.TLS;
using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkSettings : IDisposable
	{
		private struct ParameterSlice
		{
			public int Offset;

			public int Size;
		}

		private const int k_MapInitialCapacity = 8;

		private NativeHashMap<long, ParameterSlice> m_ParameterOffsets;

		private NativeList<byte> m_Parameters;

		private byte m_Initialized;

		public bool IsCreated
		{
			get
			{
				if (m_Initialized != 0)
				{
					return m_Parameters.IsCreated;
				}
				return true;
			}
		}

		private bool EnsureInitializedOrError()
		{
			if (m_Initialized == 0)
			{
				m_Initialized = 1;
				m_Parameters = new NativeList<byte>(Allocator.Temp);
				m_ParameterOffsets = new NativeHashMap<long, ParameterSlice>(8, Allocator.Temp);
			}
			if (!m_Parameters.IsCreated)
			{
				Debug.LogError("The NetworkSettings has been deallocated, it is not allowed to access it.");
				return false;
			}
			return true;
		}

		public NetworkSettings(Allocator allocator)
		{
			m_Initialized = 1;
			m_Parameters = new NativeList<byte>(allocator);
			m_ParameterOffsets = new NativeHashMap<long, ParameterSlice>(8, allocator);
		}

		public void Dispose()
		{
			m_Initialized = 1;
			if (m_Parameters.IsCreated)
			{
				m_Parameters.Dispose();
				m_ParameterOffsets.Dispose();
			}
		}

		public unsafe void AddRawParameterStruct<T>(ref T parameter) where T : unmanaged, INetworkParameter
		{
			if (EnsureInitializedOrError())
			{
				ValidateParameterOrError(ref parameter);
				long hashCode = Unity.Burst.BurstRuntime.GetHashCode64<T>();
				ParameterSlice parameterSlice = default(ParameterSlice);
				parameterSlice.Offset = m_Parameters.Length;
				parameterSlice.Size = UnsafeUtility.SizeOf<T>();
				ParameterSlice item = parameterSlice;
				if (m_ParameterOffsets.TryAdd(hashCode, item))
				{
					m_Parameters.Resize(m_Parameters.Length + item.Size, NativeArrayOptions.UninitializedMemory);
				}
				else
				{
					item = m_ParameterOffsets[hashCode];
				}
				T* ptr = (T*)((byte*)m_Parameters.GetUnsafePtr() + item.Offset);
				*ptr = parameter;
			}
		}

		public unsafe bool TryGet<T>(out T parameter) where T : unmanaged, INetworkParameter
		{
			parameter = default(T);
			if (!EnsureInitializedOrError())
			{
				return false;
			}
			long hashCode = Unity.Burst.BurstRuntime.GetHashCode64<T>();
			if (m_ParameterOffsets.TryGetValue(hashCode, out var item))
			{
				parameter = *(T*)((byte*)m_Parameters.GetUnsafeReadOnlyPtr() + item.Offset);
				return true;
			}
			return false;
		}

		internal static void ValidateParameterOrError<T>(ref T parameter) where T : INetworkParameter
		{
			if (!parameter.Validate())
			{
				Debug.LogError("The provided network parameter (" + parameter.GetType().Name + ") is not valid");
			}
		}

		internal static NetworkSettings FromArray(params INetworkParameter[] parameters)
		{
			NetworkSettings networkSettings = new NetworkSettings(Allocator.Temp);
			for (int i = 0; i < parameters.Length; i++)
			{
				INetworkParameter networkParameter = parameters[i];
				Type type = networkParameter.GetType();
				if (type == typeof(BaselibNetworkParameter))
				{
					BaselibNetworkParameter baselibNetworkParameter = (BaselibNetworkParameter)(object)networkParameter;
					if (baselibNetworkParameter.receiveQueueCapacity == 0)
					{
						baselibNetworkParameter.receiveQueueCapacity = 64;
					}
					if (baselibNetworkParameter.sendQueueCapacity == 0)
					{
						baselibNetworkParameter.sendQueueCapacity = 64;
					}
					if (baselibNetworkParameter.maximumPayloadSize == 0)
					{
						baselibNetworkParameter.maximumPayloadSize = 2000u;
					}
					networkParameter = baselibNetworkParameter;
				}
				if (type == typeof(RelayNetworkParameter))
				{
					RelayNetworkParameter relayNetworkParameter = (RelayNetworkParameter)(object)networkParameter;
					if (relayNetworkParameter.RelayConnectionTimeMS == 0)
					{
						relayNetworkParameter.RelayConnectionTimeMS = 3000;
					}
					networkParameter = relayNetworkParameter;
				}
				else if (type == typeof(SecureNetworkProtocolParameter))
				{
					SecureNetworkProtocolParameter secureNetworkProtocolParameter = (SecureNetworkProtocolParameter)(object)networkParameter;
					if (secureNetworkProtocolParameter.SSLHandshakeTimeoutMin == 0)
					{
						secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = SecureNetworkProtocol.DefaultParameters.SSLHandshakeTimeoutMin;
					}
					if (secureNetworkProtocolParameter.SSLHandshakeTimeoutMax == 0)
					{
						secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = SecureNetworkProtocol.DefaultParameters.SSLHandshakeTimeoutMax;
					}
					networkParameter = secureNetworkProtocolParameter;
				}
				MethodInfo methodInfo = typeof(NetworkSettings).GetMethod("AddRawParameterStruct").MakeGenericMethod(type);
				try
				{
					methodInfo.Invoke(networkSettings, new object[1] { networkParameter });
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			return networkSettings;
		}

		internal bool TryGet(Type parameterType, out INetworkParameter parameter)
		{
			parameter = null;
			if (!m_Parameters.IsCreated)
			{
				return false;
			}
			MethodInfo methodInfo = typeof(NetworkSettings).GetMethod("TryGet").MakeGenericMethod(parameterType);
			object[] array = new object[1];
			object obj = methodInfo.Invoke(this, array);
			parameter = (INetworkParameter)array[0];
			return (bool)obj;
		}
	}
}
