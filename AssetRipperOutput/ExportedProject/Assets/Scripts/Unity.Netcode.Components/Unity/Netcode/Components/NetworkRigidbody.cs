using UnityEngine;

namespace Unity.Netcode.Components
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(NetworkTransform))]
	[AddComponentMenu("Netcode/Network Rigidbody")]
	public class NetworkRigidbody : NetworkBehaviour
	{
		private bool m_IsServerAuthoritative;

		private Rigidbody m_Rigidbody;

		private NetworkTransform m_NetworkTransform;

		private RigidbodyInterpolation m_OriginalInterpolation;

		private bool m_IsAuthority;

		private void Awake()
		{
			m_NetworkTransform = GetComponent<NetworkTransform>();
			m_IsServerAuthoritative = m_NetworkTransform.IsServerAuthoritative();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_OriginalInterpolation = m_Rigidbody.interpolation;
			m_Rigidbody.interpolation = ((!m_NetworkTransform.Interpolate) ? m_OriginalInterpolation : RigidbodyInterpolation.None);
			m_Rigidbody.isKinematic = true;
		}

		public override void OnGainedOwnership()
		{
			UpdateOwnershipAuthority();
		}

		public override void OnLostOwnership()
		{
			UpdateOwnershipAuthority();
		}

		private void UpdateOwnershipAuthority()
		{
			if (m_IsServerAuthoritative)
			{
				m_IsAuthority = base.NetworkManager.IsServer;
			}
			else
			{
				m_IsAuthority = base.IsOwner;
			}
			m_Rigidbody.isKinematic = !m_IsAuthority;
			m_Rigidbody.interpolation = (m_IsAuthority ? m_OriginalInterpolation : RigidbodyInterpolation.None);
		}

		public override void OnNetworkSpawn()
		{
			UpdateOwnershipAuthority();
		}

		public override void OnNetworkDespawn()
		{
			m_Rigidbody.interpolation = m_OriginalInterpolation;
			m_Rigidbody.isKinematic = true;
		}

		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		protected internal override string __getTypeName()
		{
			return "NetworkRigidbody";
		}
	}
}
