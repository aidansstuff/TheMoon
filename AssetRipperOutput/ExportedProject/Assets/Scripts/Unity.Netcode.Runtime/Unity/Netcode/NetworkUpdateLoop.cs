using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Unity.Netcode
{
	public static class NetworkUpdateLoop
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkInitialization
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkInitialization);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.Initialization);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkEarlyUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkEarlyUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.EarlyUpdate);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkFixedUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkFixedUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.FixedUpdate);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkPreUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkPreUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.PreUpdate);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.Update);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkPreLateUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkPreLateUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.PreLateUpdate);
				};
				return result;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct NetworkPostLateUpdate
		{
			public static PlayerLoopSystem CreateLoopSystem()
			{
				PlayerLoopSystem result = default(PlayerLoopSystem);
				result.type = typeof(NetworkPostLateUpdate);
				result.updateDelegate = delegate
				{
					RunNetworkUpdateStage(NetworkUpdateStage.PostLateUpdate);
				};
				return result;
			}
		}

		private enum LoopSystemPosition
		{
			After = 0,
			Before = 1
		}

		private static Dictionary<NetworkUpdateStage, HashSet<INetworkUpdateSystem>> s_UpdateSystem_Sets;

		private static Dictionary<NetworkUpdateStage, INetworkUpdateSystem[]> s_UpdateSystem_Arrays;

		private const int k_UpdateSystem_InitialArrayCapacity = 1024;

		public static NetworkUpdateStage UpdateStage;

		static NetworkUpdateLoop()
		{
			s_UpdateSystem_Sets = new Dictionary<NetworkUpdateStage, HashSet<INetworkUpdateSystem>>();
			s_UpdateSystem_Arrays = new Dictionary<NetworkUpdateStage, INetworkUpdateSystem[]>();
			foreach (NetworkUpdateStage value in Enum.GetValues(typeof(NetworkUpdateStage)))
			{
				s_UpdateSystem_Sets.Add(value, new HashSet<INetworkUpdateSystem>());
				s_UpdateSystem_Arrays.Add(value, new INetworkUpdateSystem[1024]);
			}
		}

		public static void RegisterAllNetworkUpdates(this INetworkUpdateSystem updateSystem)
		{
			foreach (NetworkUpdateStage value in Enum.GetValues(typeof(NetworkUpdateStage)))
			{
				updateSystem.RegisterNetworkUpdate(value);
			}
		}

		public static void RegisterNetworkUpdate(this INetworkUpdateSystem updateSystem, NetworkUpdateStage updateStage = NetworkUpdateStage.Update)
		{
			HashSet<INetworkUpdateSystem> hashSet = s_UpdateSystem_Sets[updateStage];
			if (!hashSet.Contains(updateSystem))
			{
				hashSet.Add(updateSystem);
				int count = hashSet.Count;
				INetworkUpdateSystem[] array = s_UpdateSystem_Arrays[updateStage];
				int num = array.Length;
				if (count > num)
				{
					INetworkUpdateSystem[] array3 = (s_UpdateSystem_Arrays[updateStage] = new INetworkUpdateSystem[num *= 2]);
					array = array3;
				}
				hashSet.CopyTo(array);
				if (count < num)
				{
					array[count] = null;
				}
			}
		}

		public static void UnregisterAllNetworkUpdates(this INetworkUpdateSystem updateSystem)
		{
			foreach (NetworkUpdateStage value in Enum.GetValues(typeof(NetworkUpdateStage)))
			{
				updateSystem.UnregisterNetworkUpdate(value);
			}
		}

		public static void UnregisterNetworkUpdate(this INetworkUpdateSystem updateSystem, NetworkUpdateStage updateStage = NetworkUpdateStage.Update)
		{
			HashSet<INetworkUpdateSystem> hashSet = s_UpdateSystem_Sets[updateStage];
			if (hashSet.Contains(updateSystem))
			{
				hashSet.Remove(updateSystem);
				int count = hashSet.Count;
				INetworkUpdateSystem[] array = s_UpdateSystem_Arrays[updateStage];
				int num = array.Length;
				hashSet.CopyTo(array);
				if (count < num)
				{
					array[count] = null;
				}
			}
		}

		internal static void RunNetworkUpdateStage(NetworkUpdateStage updateStage)
		{
			UpdateStage = updateStage;
			INetworkUpdateSystem[] array = s_UpdateSystem_Arrays[updateStage];
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				INetworkUpdateSystem networkUpdateSystem = array[i];
				if (networkUpdateSystem != null)
				{
					networkUpdateSystem.NetworkUpdate(updateStage);
					continue;
				}
				break;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Initialize()
		{
			UnregisterLoopSystems();
			RegisterLoopSystems();
		}

		private static bool TryAddLoopSystem(ref PlayerLoopSystem parentLoopSystem, PlayerLoopSystem childLoopSystem, Type anchorSystemType, LoopSystemPosition loopSystemPosition)
		{
			int num = -1;
			if (anchorSystemType != null)
			{
				for (int i = 0; i < parentLoopSystem.subSystemList.Length; i++)
				{
					if (parentLoopSystem.subSystemList[i].type == anchorSystemType)
					{
						num = ((loopSystemPosition == LoopSystemPosition.After) ? (i + 1) : i);
						break;
					}
				}
			}
			else
			{
				num = ((loopSystemPosition == LoopSystemPosition.After) ? parentLoopSystem.subSystemList.Length : 0);
			}
			if (num == -1)
			{
				return false;
			}
			PlayerLoopSystem[] array = new PlayerLoopSystem[parentLoopSystem.subSystemList.Length + 1];
			if (num > 0)
			{
				Array.Copy(parentLoopSystem.subSystemList, array, num);
			}
			array[num] = childLoopSystem;
			if (num < parentLoopSystem.subSystemList.Length)
			{
				Array.Copy(parentLoopSystem.subSystemList, num, array, num + 1, parentLoopSystem.subSystemList.Length - num);
			}
			parentLoopSystem.subSystemList = array;
			return true;
		}

		private static bool TryRemoveLoopSystem(ref PlayerLoopSystem parentLoopSystem, Type childSystemType)
		{
			int num = -1;
			for (int i = 0; i < parentLoopSystem.subSystemList.Length; i++)
			{
				if (parentLoopSystem.subSystemList[i].type == childSystemType)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return false;
			}
			PlayerLoopSystem[] array = new PlayerLoopSystem[parentLoopSystem.subSystemList.Length - 1];
			if (num > 0)
			{
				Array.Copy(parentLoopSystem.subSystemList, array, num);
			}
			if (num < parentLoopSystem.subSystemList.Length - 1)
			{
				Array.Copy(parentLoopSystem.subSystemList, num + 1, array, num, parentLoopSystem.subSystemList.Length - num - 1);
			}
			parentLoopSystem.subSystemList = array;
			return true;
		}

		internal static void RegisterLoopSystems()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				ref PlayerLoopSystem reference = ref currentPlayerLoop.subSystemList[i];
				if (reference.type == typeof(Initialization))
				{
					TryAddLoopSystem(ref reference, NetworkInitialization.CreateLoopSystem(), null, LoopSystemPosition.After);
				}
				else if (reference.type == typeof(EarlyUpdate))
				{
					TryAddLoopSystem(ref reference, NetworkEarlyUpdate.CreateLoopSystem(), typeof(EarlyUpdate.ScriptRunDelayedStartupFrame), LoopSystemPosition.Before);
				}
				else if (reference.type == typeof(FixedUpdate))
				{
					TryAddLoopSystem(ref reference, NetworkFixedUpdate.CreateLoopSystem(), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), LoopSystemPosition.Before);
				}
				else if (reference.type == typeof(PreUpdate))
				{
					TryAddLoopSystem(ref reference, NetworkPreUpdate.CreateLoopSystem(), typeof(PreUpdate.PhysicsUpdate), LoopSystemPosition.Before);
				}
				else if (reference.type == typeof(Update))
				{
					TryAddLoopSystem(ref reference, NetworkUpdate.CreateLoopSystem(), typeof(Update.ScriptRunBehaviourUpdate), LoopSystemPosition.Before);
				}
				else if (reference.type == typeof(PreLateUpdate))
				{
					TryAddLoopSystem(ref reference, NetworkPreLateUpdate.CreateLoopSystem(), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), LoopSystemPosition.Before);
				}
				else if (reference.type == typeof(PostLateUpdate))
				{
					TryAddLoopSystem(ref reference, NetworkPostLateUpdate.CreateLoopSystem(), typeof(PostLateUpdate.PlayerSendFrameComplete), LoopSystemPosition.After);
				}
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		internal static void UnregisterLoopSystems()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				ref PlayerLoopSystem reference = ref currentPlayerLoop.subSystemList[i];
				if (reference.type == typeof(Initialization))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkInitialization));
				}
				else if (reference.type == typeof(EarlyUpdate))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkEarlyUpdate));
				}
				else if (reference.type == typeof(FixedUpdate))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkFixedUpdate));
				}
				else if (reference.type == typeof(PreUpdate))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkPreUpdate));
				}
				else if (reference.type == typeof(Update))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkUpdate));
				}
				else if (reference.type == typeof(PreLateUpdate))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkPreLateUpdate));
				}
				else if (reference.type == typeof(PostLateUpdate))
				{
					TryRemoveLoopSystem(ref reference, typeof(NetworkPostLateUpdate));
				}
			}
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}
	}
}
