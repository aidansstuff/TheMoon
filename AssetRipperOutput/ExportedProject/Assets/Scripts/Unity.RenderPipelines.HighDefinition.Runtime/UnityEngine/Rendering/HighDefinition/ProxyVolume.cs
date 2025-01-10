using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class ProxyVolume : IVersionable<ProxyVolume.Version>, ISerializationCallbackReceiver
	{
		private enum Version
		{
			Initial = 0,
			InfiniteProjectionInShape = 1,
			ForcePositiveSize = 2
		}

		[SerializeField]
		[FormerlySerializedAs("m_ShapeType")]
		private ProxyShape m_Shape;

		[SerializeField]
		[Min(0f)]
		private Vector3 m_BoxSize = Vector3.one;

		[SerializeField]
		[Min(0f)]
		private float m_SphereRadius = 1f;

		private static readonly MigrationDescription<Version, ProxyVolume> k_Migration = MigrationDescription.New<Version, ProxyVolume>(MigrationStep.New(Version.InfiniteProjectionInShape, delegate(ProxyVolume p)
		{
			if ((p.shape == ProxyShape.Sphere && p.m_ObsoleteSphereInfiniteProjection) || (p.shape == ProxyShape.Box && p.m_ObsoleteBoxInfiniteProjection))
			{
				p.shape = ProxyShape.Infinite;
			}
		}), MigrationStep.New(Version.ForcePositiveSize, delegate(ProxyVolume p)
		{
			p.sphereRadius = Mathf.Abs(p.sphereRadius);
			p.boxSize = new Vector3(Mathf.Abs(p.boxSize.x), Mathf.Abs(p.boxSize.y), Mathf.Abs(p.boxSize.z));
		}));

		[SerializeField]
		private Version m_CSVersion = MigrationDescription.LastVersion<Version>();

		[SerializeField]
		[FormerlySerializedAs("m_SphereInfiniteProjection")]
		[Obsolete("For data migration")]
		private bool m_ObsoleteSphereInfiniteProjection;

		[SerializeField]
		[FormerlySerializedAs("m_BoxInfiniteProjection")]
		[Obsolete("Kept only for compatibility. Use m_Shape instead")]
		private bool m_ObsoleteBoxInfiniteProjection;

		public ProxyShape shape
		{
			get
			{
				return m_Shape;
			}
			private set
			{
				m_Shape = value;
			}
		}

		public Vector3 boxSize
		{
			get
			{
				return m_BoxSize;
			}
			set
			{
				m_BoxSize = value;
			}
		}

		public float sphereRadius
		{
			get
			{
				return m_SphereRadius;
			}
			set
			{
				m_SphereRadius = value;
			}
		}

		internal Vector3 extents => GetExtents(shape);

		Version IVersionable<Version>.version
		{
			get
			{
				return m_CSVersion;
			}
			set
			{
				m_CSVersion = value;
			}
		}

		internal Hash128 ComputeHash()
		{
			Hash128 hash = default(Hash128);
			Hash128 hash2 = default(Hash128);
			HashUtilities.ComputeHash128(ref m_Shape, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxSize, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_SphereRadius, ref hash2);
			return hash;
		}

		private Vector3 GetExtents(ProxyShape shape)
		{
			return shape switch
			{
				ProxyShape.Box => m_BoxSize * 0.5f, 
				ProxyShape.Sphere => Vector3.one * m_SphereRadius, 
				_ => Vector3.one, 
			};
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			k_Migration.Migrate(this);
		}
	}
}
