using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	internal class CompositorCameraRegistry
	{
		private static List<Camera> s_CompositorManagedCameras = new List<Camera>();

		private static CompositorCameraRegistry s_CompositorCameraRegistry;

		public static CompositorCameraRegistry GetInstance()
		{
			return s_CompositorCameraRegistry ?? (s_CompositorCameraRegistry = new CompositorCameraRegistry());
		}

		internal void RegisterInternalCamera(Camera camera)
		{
			s_CompositorManagedCameras.Add(camera);
		}

		internal void UnregisterInternalCamera(Camera camera)
		{
			s_CompositorManagedCameras.Remove(camera);
		}

		internal void CleanUpCameraOrphans(List<CompositorLayer> layers = null)
		{
			s_CompositorManagedCameras.RemoveAll((Camera x) => x == null);
			for (int num = s_CompositorManagedCameras.Count - 1; num >= 0; num--)
			{
				bool flag = false;
				if (layers != null)
				{
					foreach (CompositorLayer layer in layers)
					{
						if (s_CompositorManagedCameras[num].Equals(layer.camera))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag && s_CompositorManagedCameras[num] != null)
				{
					HDAdditionalCameraData component = s_CompositorManagedCameras[num].GetComponent<HDAdditionalCameraData>();
					if ((bool)component)
					{
						CoreUtils.Destroy(component);
					}
					s_CompositorManagedCameras[num].targetTexture = null;
					CoreUtils.Destroy(s_CompositorManagedCameras[num]);
					s_CompositorManagedCameras.RemoveAt(num);
				}
			}
			if (layers == null)
			{
				return;
			}
			foreach (CompositorLayer layer2 in layers)
			{
				if (layer2 != null && !s_CompositorManagedCameras.Contains(layer2.camera))
				{
					s_CompositorManagedCameras.Add(layer2.camera);
				}
			}
		}

		internal void PrinCameraIDs()
		{
			for (int num = s_CompositorManagedCameras.Count - 1; num >= 0; num--)
			{
				if ((bool)s_CompositorManagedCameras[num])
				{
					s_CompositorManagedCameras[num].GetInstanceID();
				}
			}
		}
	}
}
