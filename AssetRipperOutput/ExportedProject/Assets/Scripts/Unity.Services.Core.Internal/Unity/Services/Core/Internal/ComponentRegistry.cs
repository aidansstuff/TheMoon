using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity.Services.Core.Internal
{
	internal class ComponentRegistry : IComponentRegistry
	{
		[NotNull]
		internal Dictionary<int, IServiceComponent> ComponentTypeHashToInstance { get; }

		public ComponentRegistry([NotNull] Dictionary<int, IServiceComponent> componentTypeHashToInstance)
		{
			ComponentTypeHashToInstance = componentTypeHashToInstance;
		}

		public void RegisterServiceComponent<TComponent>(TComponent component) where TComponent : IServiceComponent
		{
			Type typeFromHandle = typeof(TComponent);
			if (component.GetType() == typeFromHandle)
			{
				throw new ArgumentException("Interface type of component not specified.");
			}
			int hashCode = typeFromHandle.GetHashCode();
			if (IsComponentTypeRegistered(hashCode))
			{
				throw new InvalidOperationException("A component with the type " + typeFromHandle.FullName + " has already been registered.");
			}
			ComponentTypeHashToInstance[hashCode] = component;
		}

		public TComponent GetServiceComponent<TComponent>() where TComponent : IServiceComponent
		{
			Type typeFromHandle = typeof(TComponent);
			if (!ComponentTypeHashToInstance.TryGetValue(typeFromHandle.GetHashCode(), out var value) || value is MissingComponent)
			{
				throw new KeyNotFoundException("There is no component `" + typeFromHandle.Name + "` registered. Are you missing a package?");
			}
			return (TComponent)value;
		}

		private bool IsComponentTypeRegistered(int componentTypeHash)
		{
			if (ComponentTypeHashToInstance.TryGetValue(componentTypeHash, out var value) && value != null)
			{
				return !(value is MissingComponent);
			}
			return false;
		}

		public void ResetProvidedComponents(IDictionary<int, IServiceComponent> componentTypeHashToInstance)
		{
			ComponentTypeHashToInstance.Clear();
			ComponentTypeHashToInstance.MergeAllowOverride(componentTypeHashToInstance);
		}
	}
}
