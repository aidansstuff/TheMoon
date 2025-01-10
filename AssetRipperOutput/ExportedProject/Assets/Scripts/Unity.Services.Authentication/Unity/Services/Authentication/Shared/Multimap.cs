using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Services.Authentication.Shared
{
	internal class Multimap<TKey, TValue> : IDictionary<TKey, IList<TValue>>, ICollection<KeyValuePair<TKey, IList<TValue>>>, IEnumerable<KeyValuePair<TKey, IList<TValue>>>, IEnumerable
	{
		private readonly Dictionary<TKey, IList<TValue>> _dictionary;

		public IList<TValue> this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				_dictionary[key] = value;
			}
		}

		public ICollection<TKey> Keys => _dictionary.Keys;

		public ICollection<IList<TValue>> Values => _dictionary.Values;

		public int Count => _dictionary.Count;

		public bool IsReadOnly => false;

		public Multimap()
		{
			_dictionary = new Dictionary<TKey, IList<TValue>>();
		}

		public Multimap(IEqualityComparer<TKey> comparer)
		{
			_dictionary = new Dictionary<TKey, IList<TValue>>(comparer);
		}

		public IEnumerator<KeyValuePair<TKey, IList<TValue>>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, IList<TValue>> item)
		{
			if (!TryAdd(item.Key, item.Value))
			{
				throw new InvalidOperationException("Could not add values to Multimap.");
			}
		}

		public void Add(Multimap<TKey, TValue> multimap)
		{
			foreach (KeyValuePair<TKey, IList<TValue>> item in multimap)
			{
				if (!TryAdd(item.Key, item.Value))
				{
					throw new InvalidOperationException("Could not add values to Multimap.");
				}
			}
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, IList<TValue>> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, IList<TValue>> item)
		{
			throw new NotImplementedException();
		}

		public void Add(TKey key, IList<TValue> value)
		{
			if (value == null || value.Count <= 0)
			{
				return;
			}
			if (_dictionary.TryGetValue(key, out var value2))
			{
				foreach (TValue item in value)
				{
					value2.Add(item);
				}
				return;
			}
			value2 = new List<TValue>(value);
			if (!TryAdd(key, value2))
			{
				throw new InvalidOperationException("Could not add values to Multimap.");
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			IList<TValue> value;
			return TryRemove(key, out value);
		}

		public bool TryGetValue(TKey key, out IList<TValue> value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_dictionary).CopyTo(array, index);
		}

		public void Add(TKey key, TValue value)
		{
			if (value == null)
			{
				return;
			}
			if (_dictionary.TryGetValue(key, out var value2))
			{
				value2.Add(value);
				return;
			}
			value2 = new List<TValue> { value };
			if (TryAdd(key, value2))
			{
				return;
			}
			throw new InvalidOperationException("Could not add value to Multimap.");
		}

		private bool TryRemove(TKey key, out IList<TValue> value)
		{
			_dictionary.TryGetValue(key, out value);
			return _dictionary.Remove(key);
		}

		private bool TryAdd(TKey key, IList<TValue> value)
		{
			try
			{
				_dictionary.Add(key, value);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return true;
		}
	}
}
