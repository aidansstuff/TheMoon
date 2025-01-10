namespace UnityEngine.Rendering.HighDefinition
{
	[AddComponentMenu("Rendering/Reflection Proxy Volume")]
	public class ReflectionProxyVolumeComponent : MonoBehaviour
	{
		[SerializeField]
		private ProxyVolume m_ProxyVolume = new ProxyVolume();

		public ProxyVolume proxyVolume => m_ProxyVolume;
	}
}
