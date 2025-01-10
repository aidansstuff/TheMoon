using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Unity.Services.Core.Internal
{
	internal static class DependencyTreeExtensions
	{
		internal static string ToJson(this DependencyTree tree, ICollection<int> order = null)
		{
			JArray jArray = new JArray();
			JProperty jProperty = new JProperty("ordered", jArray);
			if (order != null)
			{
				foreach (int item in order)
				{
					JObject packageJObject = GetPackageJObject(tree, item);
					jArray.Add(new JObject(packageJObject));
				}
			}
			JArray jArray2 = new JArray();
			JProperty jProperty2 = new JProperty("packages", jArray2);
			foreach (int key in tree.PackageTypeHashToInstance.Keys)
			{
				JObject packageJObject2 = GetPackageJObject(tree, key);
				jArray2.Add(packageJObject2);
			}
			JArray jArray3 = new JArray();
			JProperty jProperty3 = new JProperty("components", jArray3);
			foreach (int key2 in tree.ComponentTypeHashToInstance.Keys)
			{
				JObject componentJObject = GetComponentJObject(tree, key2);
				jArray3.Add(componentJObject);
			}
			return new JObject(jProperty, jProperty2, jProperty3).ToString();
		}

		internal static bool IsOptional(this DependencyTree tree, int componentTypeHash)
		{
			if (tree.ComponentTypeHashToInstance.TryGetValue(componentTypeHash, out var value))
			{
				return value == null;
			}
			return false;
		}

		internal static bool IsProvided(this DependencyTree tree, int componentTypeHash)
		{
			return tree.ComponentTypeHashToPackageTypeHash.ContainsKey(componentTypeHash);
		}

		private static JObject GetPackageJObject(DependencyTree tree, int packageHash)
		{
			JProperty jProperty = new JProperty("packageHash", packageHash);
			tree.PackageTypeHashToInstance.TryGetValue(packageHash, out var value);
			JProperty jProperty2 = new JProperty("packageProvider", (value != null) ? value.GetType().Name : "null");
			JArray jArray = new JArray();
			JProperty jProperty3 = new JProperty("packageDependencies", jArray);
			if (tree.PackageTypeHashToComponentTypeHashDependencies.TryGetValue(packageHash, out var value2))
			{
				foreach (int item2 in value2)
				{
					JProperty jProperty4 = new JProperty("dependencyHash", item2);
					tree.ComponentTypeHashToInstance.TryGetValue(item2, out var value3);
					JProperty jProperty5 = new JProperty("dependencyComponent", GetComponentIdentifier(value3));
					JProperty jProperty6 = new JProperty("dependencyProvided", tree.IsProvided(item2) ? "true" : "false");
					JProperty jProperty7 = new JProperty("dependencyOptional", tree.IsOptional(item2) ? "true" : "false");
					JObject item = new JObject(jProperty4, jProperty5, jProperty6, jProperty7);
					jArray.Add(item);
				}
			}
			return new JObject(jProperty, jProperty2, jProperty3);
		}

		private static JObject GetComponentJObject(DependencyTree tree, int componentHash)
		{
			JProperty jProperty = new JProperty("componentHash", componentHash);
			tree.ComponentTypeHashToInstance.TryGetValue(componentHash, out var value);
			JProperty jProperty2 = new JProperty("component", GetComponentIdentifier(value));
			tree.ComponentTypeHashToPackageTypeHash.TryGetValue(componentHash, out var value2);
			JProperty jProperty3 = new JProperty("componentPackageHash", value2);
			IInitializablePackage value3;
			bool flag = tree.PackageTypeHashToInstance.TryGetValue(value2, out value3);
			JProperty jProperty4 = new JProperty("componentPackage", flag ? value3.GetType().Name : "null");
			return new JObject(jProperty, jProperty2, jProperty3, jProperty4);
		}

		private static string GetComponentIdentifier(IServiceComponent component)
		{
			if (component == null)
			{
				return "null";
			}
			if (component is MissingComponent missingComponent)
			{
				return missingComponent.IntendedType.Name;
			}
			return component.GetType().Name;
		}
	}
}
