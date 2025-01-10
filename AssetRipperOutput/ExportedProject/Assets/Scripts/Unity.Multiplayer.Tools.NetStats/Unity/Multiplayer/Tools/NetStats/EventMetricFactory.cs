using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal class EventMetricFactory : IMetricFactory
	{
		private interface IEventMetricFactory
		{
			IMetric Construct(MetricId id);
		}

		private class EventMetricFactoryImpl<T> : IEventMetricFactory where T : unmanaged
		{
			public IMetric Construct(MetricId id)
			{
				return new EventMetric<T>(id);
			}
		}

		private static readonly Dictionary<FixedString128Bytes, IEventMetricFactory> k_FactoriesByName;

		private static readonly Dictionary<Type, FixedString128Bytes> k_TypeNames;

		public static bool TryGetFactoryTypeName(Type type, out FixedString128Bytes typeName)
		{
			return k_TypeNames.TryGetValue(type, out typeName);
		}

		static EventMetricFactory()
		{
			k_FactoriesByName = new Dictionary<FixedString128Bytes, IEventMetricFactory>();
			k_TypeNames = new Dictionary<Type, FixedString128Bytes>();
			TypeRegistration.RunIfNeeded();
		}

		internal static void RegisterType<T>() where T : unmanaged
		{
			if (!k_TypeNames.ContainsKey(typeof(T)))
			{
				FixedString128Bytes fixedString128Bytes = typeof(T).FullName;
				k_FactoriesByName.Add(fixedString128Bytes, new EventMetricFactoryImpl<T>());
				k_TypeNames.Add(typeof(T), fixedString128Bytes);
			}
		}

		public bool TryConstruct(MetricHeader header, out IMetric metric)
		{
			if (!k_FactoriesByName.TryGetValue(header.EventFactoryTypeName, out var value))
			{
				Debug.LogError("Failed to find factory for event type " + header.EventFactoryTypeName.ToString());
				metric = null;
				return false;
			}
			metric = value.Construct(header.MetricId);
			return true;
		}
	}
}
