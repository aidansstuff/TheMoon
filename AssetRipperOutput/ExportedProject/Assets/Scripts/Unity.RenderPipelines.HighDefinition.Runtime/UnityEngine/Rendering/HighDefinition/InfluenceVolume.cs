using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class InfluenceVolume : IVersionable<InfluenceVolume.Version>, ISerializationCallbackReceiver
	{
		private enum Version
		{
			Initial = 0,
			SphereOffset = 1
		}

		[SerializeField]
		[FormerlySerializedAs("m_ShapeType")]
		private InfluenceShape m_Shape;

		[SerializeField]
		[FormerlySerializedAs("m_BoxBaseSize")]
		private Vector3 m_BoxSize = Vector3.one * 10f;

		[SerializeField]
		[FormerlySerializedAs("m_BoxInfluencePositiveFade")]
		private Vector3 m_BoxBlendDistancePositive;

		[SerializeField]
		[FormerlySerializedAs("m_BoxInfluenceNegativeFade")]
		private Vector3 m_BoxBlendDistanceNegative;

		[SerializeField]
		[FormerlySerializedAs("m_BoxInfluenceNormalPositiveFade")]
		private Vector3 m_BoxBlendNormalDistancePositive;

		[SerializeField]
		[FormerlySerializedAs("m_BoxInfluenceNormalNegativeFade")]
		private Vector3 m_BoxBlendNormalDistanceNegative;

		[SerializeField]
		[FormerlySerializedAs("m_BoxPositiveFaceFade")]
		private Vector3 m_BoxSideFadePositive = Vector3.one;

		[SerializeField]
		[FormerlySerializedAs("m_BoxNegativeFaceFade")]
		private Vector3 m_BoxSideFadeNegative = Vector3.one;

		[SerializeField]
		[FormerlySerializedAs("m_SphereBaseRadius")]
		[Min(0f)]
		private float m_SphereRadius = 3f;

		[SerializeField]
		[FormerlySerializedAs("m_SphereInfluenceFade")]
		private float m_SphereBlendDistance;

		[SerializeField]
		[FormerlySerializedAs("m_SphereInfluenceNormalFade")]
		private float m_SphereBlendNormalDistance;

		[SerializeField]
		[FormerlySerializedAs("editorAdvancedModeBlendDistancePositive")]
		private Vector3 m_EditorAdvancedModeBlendDistancePositive;

		[SerializeField]
		[FormerlySerializedAs("editorAdvancedModeBlendDistanceNegative")]
		private Vector3 m_EditorAdvancedModeBlendDistanceNegative;

		[SerializeField]
		[FormerlySerializedAs("editorSimplifiedModeBlendDistance")]
		private float m_EditorSimplifiedModeBlendDistance;

		[SerializeField]
		[FormerlySerializedAs("editorAdvancedModeBlendNormalDistancePositive")]
		private Vector3 m_EditorAdvancedModeBlendNormalDistancePositive;

		[SerializeField]
		[FormerlySerializedAs("editorAdvancedModeBlendNormalDistanceNegative")]
		private Vector3 m_EditorAdvancedModeBlendNormalDistanceNegative;

		[SerializeField]
		[FormerlySerializedAs("editorSimplifiedModeBlendNormalDistance")]
		private float m_EditorSimplifiedModeBlendNormalDistance;

		[SerializeField]
		[FormerlySerializedAs("editorAdvancedModeEnabled")]
		private bool m_EditorAdvancedModeEnabled;

		[SerializeField]
		private Vector3 m_EditorAdvancedModeFaceFadePositive = Vector3.one;

		[SerializeField]
		private Vector3 m_EditorAdvancedModeFaceFadeNegative = Vector3.one;

		private static readonly MigrationDescription<Version, InfluenceVolume> k_Migration = MigrationDescription.New<Version, InfluenceVolume>(MigrationStep.New(Version.SphereOffset, delegate(InfluenceVolume i)
		{
			if (i.shape == InfluenceShape.Sphere)
			{
				i.m_ObsoleteOffset = i.m_ObsoleteSphereBaseOffset;
			}
		}));

		[SerializeField]
		[ExcludeCopy]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		[SerializeField]
		[FormerlySerializedAs("m_SphereBaseOffset")]
		[Obsolete("For Data Migration")]
		[ExcludeCopy]
		private Vector3 m_ObsoleteSphereBaseOffset;

		[SerializeField]
		[FormerlySerializedAs("m_BoxBaseOffset")]
		[FormerlySerializedAs("m_Offset")]
		[ExcludeCopy]
		private Vector3 m_ObsoleteOffset;

		public InfluenceShape shape
		{
			get
			{
				return m_Shape;
			}
			set
			{
				m_Shape = value;
			}
		}

		public Vector3 extents => GetExtents(shape);

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

		public Vector3 boxBlendOffset => (boxBlendDistanceNegative - boxBlendDistancePositive) * 0.5f;

		public Vector3 boxBlendSize => -(boxBlendDistancePositive + boxBlendDistanceNegative);

		public Vector3 boxBlendDistancePositive
		{
			get
			{
				return m_BoxBlendDistancePositive;
			}
			set
			{
				m_BoxBlendDistancePositive = value;
			}
		}

		public Vector3 boxBlendDistanceNegative
		{
			get
			{
				return m_BoxBlendDistanceNegative;
			}
			set
			{
				m_BoxBlendDistanceNegative = value;
			}
		}

		public Vector3 boxBlendNormalOffset => (boxBlendNormalDistanceNegative - boxBlendNormalDistancePositive) * 0.5f;

		public Vector3 boxBlendNormalSize => -(boxBlendNormalDistancePositive + boxBlendNormalDistanceNegative);

		public Vector3 boxBlendNormalDistancePositive
		{
			get
			{
				return m_BoxBlendNormalDistancePositive;
			}
			set
			{
				m_BoxBlendNormalDistancePositive = value;
			}
		}

		public Vector3 boxBlendNormalDistanceNegative
		{
			get
			{
				return m_BoxBlendNormalDistanceNegative;
			}
			set
			{
				m_BoxBlendNormalDistanceNegative = value;
			}
		}

		public Vector3 boxSideFadePositive
		{
			get
			{
				return m_BoxSideFadePositive;
			}
			set
			{
				m_BoxSideFadePositive = value;
			}
		}

		public Vector3 boxSideFadeNegative
		{
			get
			{
				return m_BoxSideFadeNegative;
			}
			set
			{
				m_BoxSideFadeNegative = value;
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

		public float sphereBlendDistance
		{
			get
			{
				return m_SphereBlendDistance;
			}
			set
			{
				m_SphereBlendDistance = value;
			}
		}

		public float sphereBlendNormalDistance
		{
			get
			{
				return m_SphereBlendNormalDistance;
			}
			set
			{
				m_SphereBlendNormalDistance = value;
			}
		}

		internal EnvShapeType envShape
		{
			get
			{
				InfluenceShape influenceShape = shape;
				if (influenceShape == InfluenceShape.Box || influenceShape != InfluenceShape.Sphere)
				{
					return EnvShapeType.Box;
				}
				return EnvShapeType.Sphere;
			}
		}

		Version IVersionable<Version>.version
		{
			get
			{
				return m_Version;
			}
			set
			{
				m_Version = value;
			}
		}

		[Obsolete("Only used for data migration purpose. Don't use this field.")]
		internal Vector3 obsoleteOffset
		{
			get
			{
				return m_ObsoleteOffset;
			}
			set
			{
				m_ObsoleteOffset = value;
			}
		}

		public Hash128 ComputeHash()
		{
			Hash128 hash = default(Hash128);
			Hash128 hash2 = default(Hash128);
			HashUtilities.ComputeHash128(ref m_Shape, ref hash);
			HashUtilities.ComputeHash128(ref m_ObsoleteOffset, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxBlendDistanceNegative, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxBlendDistancePositive, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxBlendNormalDistanceNegative, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxBlendNormalDistancePositive, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxSideFadeNegative, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxSideFadePositive, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_BoxSize, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_SphereBlendDistance, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_SphereBlendNormalDistance, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref m_SphereRadius, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			return hash;
		}

		internal BoundingSphere GetBoundingSphereAt(Vector3 position)
		{
			if (shape != 0)
			{
				_ = 1;
				return new BoundingSphere(position, sphereRadius);
			}
			float rad = Mathf.Max(boxSize.x, Mathf.Max(boxSize.y, boxSize.z));
			return new BoundingSphere(position, rad);
		}

		internal Bounds GetBoundsAt(Vector3 position)
		{
			if (shape != 0)
			{
				_ = 1;
				return new Bounds(position, Vector3.one * sphereRadius);
			}
			return new Bounds(position, boxSize);
		}

		internal Matrix4x4 GetInfluenceToWorld(Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		internal void CopyTo(InfluenceVolume data)
		{
			data.m_Shape = m_Shape;
			data.m_ObsoleteOffset = m_ObsoleteOffset;
			data.m_BoxSize = m_BoxSize;
			data.m_BoxBlendDistancePositive = m_BoxBlendDistancePositive;
			data.m_BoxBlendDistanceNegative = m_BoxBlendDistanceNegative;
			data.m_BoxBlendNormalDistancePositive = m_BoxBlendNormalDistancePositive;
			data.m_BoxBlendNormalDistanceNegative = m_BoxBlendNormalDistanceNegative;
			data.m_BoxSideFadePositive = m_BoxSideFadePositive;
			data.m_BoxSideFadeNegative = m_BoxSideFadeNegative;
			data.m_SphereRadius = m_SphereRadius;
			data.m_SphereBlendDistance = m_SphereBlendDistance;
			data.m_SphereBlendNormalDistance = m_SphereBlendNormalDistance;
		}

		private Vector3 GetExtents(InfluenceShape shape)
		{
			if (shape == InfluenceShape.Box || shape != InfluenceShape.Sphere)
			{
				return Vector3.Max(Vector3.one * 0.0001f, boxSize * 0.5f);
			}
			return Mathf.Max(0.0001f, sphereRadius) * Vector3.one;
		}

		public float ComputeFOVAt(Vector3 viewerPositionWS, Vector3 lookAtPositionWS, Matrix4x4 influenceToWorld)
		{
			float fieldOfView2 = 0f;
			switch (envShape)
			{
			case EnvShapeType.Box:
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(boxSize.x, 0f - boxSize.y, 0f - boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(boxSize.x, 0f - boxSize.y, boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(boxSize.x, boxSize.y, 0f - boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(boxSize.x, boxSize.y, boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f - boxSize.x, 0f - boxSize.y, 0f - boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f - boxSize.x, 0f - boxSize.y, boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f - boxSize.x, boxSize.y, 0f - boxSize.z)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f - boxSize.x, boxSize.y, boxSize.z)));
				break;
			case EnvShapeType.Sphere:
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(sphereRadius * 2f, 0f, 0f)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3((0f - sphereRadius) * 2f, 0f, 0f)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f, sphereRadius * 2f, 0f)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f, (0f - sphereRadius) * 2f, 0f)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f, 0f, sphereRadius * 2f)));
				GrowFOVToInclude(ref fieldOfView2, influenceToWorld.MultiplyPoint(new Vector3(0f, 0f, (0f - sphereRadius) * 2f)));
				break;
			default:
				fieldOfView2 = 90f;
				break;
			}
			return fieldOfView2;
			void GrowFOVToInclude(ref float fieldOfView, Vector3 positionWS)
			{
				float num = Vector3.Angle(lookAtPositionWS - viewerPositionWS, positionWS - viewerPositionWS);
				fieldOfView = Mathf.Max(num * 2f, fieldOfView);
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			k_Migration.Migrate(this);
		}
	}
}
