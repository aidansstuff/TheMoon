using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SceneObjectIDMapSceneAsset : MonoBehaviour, ISerializationCallbackReceiver
	{
		[Serializable]
		private struct Entry
		{
			public int id;

			public int category;

			public GameObject gameObject;
		}

		internal const string k_GameObjectName = "SceneIDMap";

		[SerializeField]
		private List<Entry> m_Entries = new List<Entry>();

		private Dictionary<GameObject, int> m_IndexByGameObject = new Dictionary<GameObject, int>();

		[NonSerialized]
		private bool m_RebuildRequested;

		[NonSerialized]
		private bool m_CleanDestroyedGameObjectsRequested;

		public void GetALLIDsFor<TCategory>(TCategory category, List<GameObject> outGameObjects, List<int> outIndices) where TCategory : struct, IConvertible
		{
			if (outGameObjects == null)
			{
				throw new ArgumentNullException("outGameObjects");
			}
			if (outIndices == null)
			{
				throw new ArgumentNullException("outIndices");
			}
			CleanDestroyedGameObjects();
			int num = Convert.ToInt32(category);
			for (int num2 = m_Entries.Count - 1; num2 >= 0; num2--)
			{
				if (m_Entries[num2].category == num)
				{
					outIndices.Add(m_Entries[num2].id);
					outGameObjects.Add(m_Entries[num2].gameObject);
				}
			}
		}

		internal bool TryGetSceneIDFor<TCategory>(GameObject gameObject, out int index, out TCategory category) where TCategory : struct, IConvertible
		{
			Verify();
			if (!typeof(TCategory).IsEnum)
			{
				throw new ArgumentException("'TCategory' must be an Enum type.");
			}
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			if (m_IndexByGameObject.TryGetValue(gameObject, out var value))
			{
				if (value < m_Entries.Count)
				{
					category = (TCategory)(object)m_Entries[value].category;
					index = m_Entries[value].id;
					return true;
				}
				m_IndexByGameObject.Remove(gameObject);
			}
			category = default(TCategory);
			index = -1;
			return false;
		}

		internal bool TryInsert<TCategory>(GameObject gameObject, TCategory category, out int index) where TCategory : struct, IConvertible
		{
			Verify();
			if (!typeof(TCategory).IsEnum)
			{
				throw new ArgumentException("'TCategory' must be an Enum type.");
			}
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			if (gameObject.scene != base.gameObject.scene)
			{
				index = -1;
				return false;
			}
			if (TryGetSceneIDFor<TCategory>(gameObject, out index, out var _))
			{
				return false;
			}
			index = Insert(gameObject, category);
			return true;
		}

		private int Insert<TCategory>(GameObject gameObject, TCategory category) where TCategory : struct, IConvertible
		{
			Verify();
			Entry entry = default(Entry);
			entry.gameObject = gameObject;
			entry.category = Convert.ToInt32(category);
			Entry item = entry;
			int num = -1;
			if (m_Entries.Count > 0 && m_Entries[0].id != 0)
			{
				num = 0;
				item.id = 0;
			}
			else
			{
				for (int i = 0; i < m_Entries.Count - 1; i++)
				{
					if (m_Entries[i].id + 1 != m_Entries[i + 1].id)
					{
						num = i + 1;
						item.id = m_Entries[i].id + 1;
						break;
					}
				}
			}
			if (num == -1)
			{
				num = m_Entries.Count;
				item.id = m_Entries.Count;
			}
			m_IndexByGameObject.Add(gameObject, num);
			m_Entries.Insert(num, item);
			for (int j = num + 1; j < m_Entries.Count; j++)
			{
				m_IndexByGameObject[m_Entries[j].gameObject] = j;
			}
			return m_Entries[num].id;
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_RebuildRequested = true;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_CleanDestroyedGameObjectsRequested = true;
		}

		private void CleanDestroyedGameObjects()
		{
			m_CleanDestroyedGameObjectsRequested = false;
			bool flag = false;
			for (int num = m_Entries.Count - 1; num >= 0; num--)
			{
				if (m_Entries[num].gameObject == null)
				{
					m_Entries.RemoveAt(num);
					flag = true;
				}
			}
			if (flag)
			{
				BuildIndex();
			}
		}

		private void BuildIndex()
		{
			m_RebuildRequested = false;
			m_IndexByGameObject.Clear();
			for (int i = 0; i < m_Entries.Count; i++)
			{
				m_IndexByGameObject[m_Entries[i].gameObject] = i;
			}
		}

		private void Verify()
		{
			if (m_CleanDestroyedGameObjectsRequested)
			{
				CleanDestroyedGameObjects();
			}
			if (m_RebuildRequested)
			{
				BuildIndex();
			}
		}
	}
}
