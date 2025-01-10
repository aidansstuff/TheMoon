using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class CustomPostProcessVolumeComponent : VolumeComponent
	{
		private bool m_IsInitialized;

		internal string typeName;

		internal static HashSet<CustomPostProcessVolumeComponent> instances = new HashSet<CustomPostProcessVolumeComponent>();

		public virtual CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

		public virtual bool visibleInSceneView => true;

		public virtual void Setup()
		{
		}

		public abstract void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination);

		public virtual void Cleanup()
		{
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			CleanupInternal();
		}

		internal void CleanupInternal()
		{
			if (m_IsInitialized)
			{
				Cleanup();
			}
			m_IsInitialized = false;
			instances.Remove(this);
		}

		internal void SetupIfNeeded()
		{
			if (!m_IsInitialized)
			{
				Setup();
				m_IsInitialized = true;
				typeName = GetType().Name;
				instances.Add(this);
			}
		}

		internal static void CleanupAllCustomPostProcesses()
		{
			foreach (CustomPostProcessVolumeComponent item in instances.ToList())
			{
				item.CleanupInternal();
			}
		}
	}
}
