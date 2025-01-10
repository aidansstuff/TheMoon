using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal static class DictionaryExtensions
	{
		public static TDictionary MergeNoOverride<TDictionary, TKey, TValue>(this TDictionary self, [NotNull] IDictionary<TKey, TValue> dictionary) where TDictionary : IDictionary<TKey, TValue>
		{
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				if (!self.ContainsKey(item.Key))
				{
					self[item.Key] = item.Value;
				}
			}
			return self;
		}

		public static TDictionary MergeAllowOverride<TDictionary, TKey, TValue>(this TDictionary self, [NotNull] IDictionary<TKey, TValue> dictionary) where TDictionary : IDictionary<TKey, TValue>
		{
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				self[item.Key] = item.Value;
			}
			return self;
		}

		public static bool ValueEquals<TKey, TValue>(this IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y)
		{
			return x.ValueEquals(y, EqualityComparer<TValue>.Default);
		}

		public static bool ValueEquals<TKey, TValue, TComparer>(this IDictionary<TKey, TValue> x, IDictionary<TKey, TValue> y, TComparer valueComparer) where TComparer : IEqualityComparer<TValue>
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null || x.Count != y.Count)
			{
				return false;
			}
			foreach (KeyValuePair<TKey, TValue> item in x)
			{
				if (!y.TryGetValue(item.Key, out var value) || !valueComparer.Equals(item.Value, value))
				{
					return false;
				}
			}
			return true;
		}
	}
}
