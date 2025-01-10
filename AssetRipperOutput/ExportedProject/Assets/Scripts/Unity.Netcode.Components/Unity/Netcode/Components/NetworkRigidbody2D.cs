using UnityEngine;

namespace Unity.Netcode.Components
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(NetworkTransform))]
	[AddComponentMenu("Netcode/Network Rigidbody 2D")]
	public class NetworkRigidbody2D : NetworkBehaviour
	{
		private Rigidbody2D m_Rigidbody;

		private NetworkTransform m_NetworkTransform;

		private bool m_OriginalKinematic;

		private RigidbodyInterpolation2D m_OriginalInterpolation;

		private bool m_IsAuthority;

		private bool HasAuthority => m_NetworkTransform.CanCommitToTransform;

		private void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody2D>();
			m_NetworkTransform = GetComponent<NetworkTransform>();
		}

		private void FixedUpdate()
		{
			if (base.IsSpawned && HasAuthority != m_IsAuthority)
			{
				m_IsAuthority = HasAuthority;
				UpdateRigidbodyKinematicMode();
			}
		}

		private void UpdateRigidbodyKinematicMode()
		{
			if (!m_IsAuthority)
			{
				m_OriginalKinematic = m_Rigidbody.isKinematic;
				m_Rigidbody.isKinematic = true;
				m_OriginalInterpolation = m_Rigidbody.interpolation;
				m_Rigidbody.interpolation = RigidbodyInterpolation2D.None;
			}
			else
			{
				m_Rigidbody.isKinematic = m_OriginalKinematic;
				m_Rigidbody.interpolation = m_OriginalInterpolation;
			}
		}

		public override void OnNetworkSpawn()
		{
			m_IsAuthority = HasAuthority;
			m_OriginalKinematic = m_Rigidbody.isKinematic;
			m_OriginalInterpolation = m_Rigidbody.interpolation;
			UpdateRigidbodyKinematicMode();
		}

		public override void OnNetworkDespawn()
		{
			UpdateRigidbodyKinematicMode();
		}

		protected override void __initializeVariables()
		{
			base.__initializeVariables();
		}

		protected internal override string __getTypeName()
		{
			return "NetworkRigidbody2D";
		}
	}
}
