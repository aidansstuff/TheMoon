using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Netcode
{
	internal class DeferredMessageManager : IDeferredNetworkMessageManager
	{
		protected struct TriggerData
		{
			public FastBufferReader Reader;

			public NetworkMessageHeader Header;

			public ulong SenderId;

			public float Timestamp;

			public int SerializedHeaderSize;
		}

		protected struct TriggerInfo
		{
			public float Expiry;

			public NativeList<TriggerData> TriggerData;
		}

		protected readonly Dictionary<IDeferredNetworkMessageManager.TriggerType, Dictionary<ulong, TriggerInfo>> m_Triggers = new Dictionary<IDeferredNetworkMessageManager.TriggerType, Dictionary<ulong, TriggerInfo>>();

		private readonly NetworkManager m_NetworkManager;

		internal DeferredMessageManager(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
		}

		public unsafe virtual void DeferMessage(IDeferredNetworkMessageManager.TriggerType trigger, ulong key, FastBufferReader reader, ref NetworkContext context)
		{
			if (!m_Triggers.TryGetValue(trigger, out var value))
			{
				value = new Dictionary<ulong, TriggerInfo>();
				m_Triggers[trigger] = value;
			}
			if (!value.TryGetValue(key, out var value2))
			{
				TriggerInfo triggerInfo = default(TriggerInfo);
				triggerInfo.Expiry = m_NetworkManager.RealTimeProvider.RealTimeSinceStartup + m_NetworkManager.NetworkConfig.SpawnTimeout;
				triggerInfo.TriggerData = new NativeList<TriggerData>(Allocator.Persistent);
				value2 = (value[key] = triggerInfo);
			}
			ref NativeList<TriggerData> triggerData = ref value2.TriggerData;
			TriggerData value3 = new TriggerData
			{
				Reader = new FastBufferReader(reader.GetUnsafePtr(), Allocator.Persistent, reader.Length),
				Header = context.Header,
				Timestamp = context.Timestamp,
				SenderId = context.SenderId,
				SerializedHeaderSize = context.SerializedHeaderSize
			};
			triggerData.Add(in value3);
		}

		public unsafe virtual void CleanupStaleTriggers()
		{
			foreach (KeyValuePair<IDeferredNetworkMessageManager.TriggerType, Dictionary<ulong, TriggerInfo>> trigger in m_Triggers)
			{
				ulong* ptr = stackalloc ulong[trigger.Value.Count];
				int num = 0;
				foreach (KeyValuePair<ulong, TriggerInfo> item in trigger.Value)
				{
					if (item.Value.Expiry < m_NetworkManager.RealTimeProvider.RealTimeSinceStartup)
					{
						ptr[num++] = item.Key;
						PurgeTrigger(trigger.Key, item.Key, item.Value);
					}
				}
				for (int i = 0; i < num; i++)
				{
					trigger.Value.Remove(ptr[i]);
				}
			}
		}

		protected virtual void PurgeTrigger(IDeferredNetworkMessageManager.TriggerType triggerType, ulong key, TriggerInfo triggerInfo)
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
			{
				NetworkLog.LogWarning($"Deferred messages were received for a trigger of type {triggerType} with key {key}, but that trigger was not received within within {m_NetworkManager.NetworkConfig.SpawnTimeout} second(s).");
			}
			foreach (TriggerData triggerDatum in triggerInfo.TriggerData)
			{
				triggerDatum.Reader.Dispose();
			}
			triggerInfo.TriggerData.Dispose();
		}

		public virtual void ProcessTriggers(IDeferredNetworkMessageManager.TriggerType trigger, ulong key)
		{
			if (!m_Triggers.TryGetValue(trigger, out var value) || !value.TryGetValue(key, out var value2))
			{
				return;
			}
			foreach (TriggerData triggerDatum in value2.TriggerData)
			{
				TriggerData current = triggerDatum;
				m_NetworkManager.ConnectionManager.MessageManager.HandleMessage(in current.Header, current.Reader, current.SenderId, current.Timestamp, current.SerializedHeaderSize);
			}
			value2.TriggerData.Dispose();
			value.Remove(key);
		}

		public virtual void CleanupAllTriggers()
		{
			foreach (KeyValuePair<IDeferredNetworkMessageManager.TriggerType, Dictionary<ulong, TriggerInfo>> trigger in m_Triggers)
			{
				foreach (KeyValuePair<ulong, TriggerInfo> item in trigger.Value)
				{
					foreach (TriggerData triggerDatum in item.Value.TriggerData)
					{
						triggerDatum.Reader.Dispose();
					}
					item.Value.TriggerData.Dispose();
				}
			}
			m_Triggers.Clear();
		}
	}
}
