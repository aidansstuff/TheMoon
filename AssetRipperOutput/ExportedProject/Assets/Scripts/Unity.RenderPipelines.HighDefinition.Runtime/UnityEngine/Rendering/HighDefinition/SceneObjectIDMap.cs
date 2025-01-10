using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SceneObjectIDMap
	{
		public static bool TryGetSceneObjectID<TCategory>(GameObject gameObject, out int index, out TCategory category) where TCategory : struct, IConvertible
		{
			if (!typeof(TCategory).IsEnum)
			{
				throw new ArgumentException("'TCategory' must be an Enum type.");
			}
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			index = 0;
			category = default(TCategory);
			if (TryGetOrCreateSceneIDMapFor(gameObject.scene, out var map))
			{
				return map.TryGetSceneIDFor<TCategory>(gameObject, out index, out category);
			}
			return false;
		}

		public static int GetOrCreateSceneObjectID<TCategory>(GameObject gameObject, TCategory category) where TCategory : struct, IConvertible
		{
			if (!typeof(TCategory).IsEnum)
			{
				throw new ArgumentException("'TCategory' must be an Enum type.");
			}
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			if (!TryGetOrCreateSceneIDMapFor(gameObject.scene, out var map))
			{
				throw new ArgumentException($"Provided GameObject {gameObject} does not belong to a loaded scene.");
			}
			if (!map.TryGetSceneIDFor<TCategory>(gameObject, out var index, out var _))
			{
				map.TryInsert(gameObject, category, out index);
			}
			return index;
		}

		public static void GetAllIDsForAllScenes<TCategory>(TCategory category, List<GameObject> outGameObjects, List<int> outIndices, List<Scene> outScenes) where TCategory : struct, IConvertible
		{
			if (outGameObjects == null)
			{
				throw new ArgumentNullException("outGameObjects");
			}
			if (outIndices == null)
			{
				throw new ArgumentNullException("outIndices");
			}
			if (outIndices == null)
			{
				throw new ArgumentNullException("outScenes");
			}
			int count = outGameObjects.Count;
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				GetAllIDsFor(category, sceneAt, outGameObjects, outIndices);
				int j = 0;
				for (int num = outGameObjects.Count - count; j < num; j++)
				{
					outScenes.Add(sceneAt);
				}
			}
		}

		public static void GetAllIDsFor<TCategory>(TCategory category, Scene scene, List<GameObject> outGameObjects, List<int> outIndices) where TCategory : struct, IConvertible
		{
			if (outGameObjects == null)
			{
				throw new ArgumentNullException("outGameObjects");
			}
			if (outIndices == null)
			{
				throw new ArgumentNullException("outIndices");
			}
			if (TryGetSceneIDMapFor(scene, out var map))
			{
				map.GetALLIDsFor(category, outGameObjects, outIndices);
			}
		}

		private static bool TryGetSceneIDMapFor(Scene scene, out SceneObjectIDMapSceneAsset map)
		{
			if (!scene.isLoaded)
			{
				map = null;
				return false;
			}
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (rootGameObjects[i].name == "SceneIDMap" && (map = rootGameObjects[i].GetComponent<SceneObjectIDMapSceneAsset>()) != null && !map.Equals(null))
				{
					return true;
				}
			}
			map = null;
			return false;
		}

		private static SceneObjectIDMapSceneAsset CreateSceneIDMapFor(Scene scene)
		{
			GameObject obj = new GameObject("SceneIDMap")
			{
				hideFlags = (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInBuild)
			};
			SceneObjectIDMapSceneAsset result = obj.AddComponent<SceneObjectIDMapSceneAsset>();
			SceneManager.MoveGameObjectToScene(obj, scene);
			return result;
		}

		private static bool TryGetOrCreateSceneIDMapFor(Scene scene, out SceneObjectIDMapSceneAsset map)
		{
			if (!scene.isLoaded)
			{
				map = null;
				return false;
			}
			if (!TryGetSceneIDMapFor(scene, out map))
			{
				map = CreateSceneIDMapFor(scene);
			}
			return true;
		}
	}
}
