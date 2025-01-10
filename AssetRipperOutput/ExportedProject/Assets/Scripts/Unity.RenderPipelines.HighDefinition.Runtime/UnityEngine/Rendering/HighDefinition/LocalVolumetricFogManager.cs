using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class LocalVolumetricFogManager
	{
		private static LocalVolumetricFogManager m_Manager;

		private List<LocalVolumetricFog> m_Volumes;

		public static LocalVolumetricFogManager manager
		{
			get
			{
				if (m_Manager == null)
				{
					m_Manager = new LocalVolumetricFogManager();
				}
				return m_Manager;
			}
		}

		private LocalVolumetricFogManager()
		{
			m_Volumes = new List<LocalVolumetricFog>();
		}

		public void RegisterVolume(LocalVolumetricFog volume)
		{
			m_Volumes.Add(volume);
		}

		public void DeRegisterVolume(LocalVolumetricFog volume)
		{
			if (m_Volumes.Contains(volume))
			{
				m_Volumes.Remove(volume);
			}
		}

		public bool ContainsVolume(LocalVolumetricFog volume)
		{
			return m_Volumes.Contains(volume);
		}

		public List<LocalVolumetricFog> PrepareLocalVolumetricFogData(CommandBuffer cmd, HDCamera currentCam)
		{
			float time = currentCam.time;
			foreach (LocalVolumetricFog volume in m_Volumes)
			{
				volume.PrepareParameters(time);
			}
			return m_Volumes;
		}
	}
}
