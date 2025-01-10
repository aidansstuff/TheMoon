using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/HDRP Decal Projector")]
	public class DecalProjector : MonoBehaviour, IVersionable<DecalProjector.Version>
	{
		internal struct CachedDecalData
		{
			public float drawDistance;

			public float fadeScale;

			public float startAngleFade;

			public float endAngleFade;

			public Vector4 uvScaleBias;

			public bool affectsTransparency;

			public int layerMask;

			public ulong sceneLayerMask;

			public float fadeFactor;

			public DecalLayerEnum decalLayerMask;
		}

		private enum Version
		{
			Initial = 0,
			UseZProjectionAxisAndScaleIndependance = 1,
			FixPivotPosition = 2
		}

		[SerializeField]
		private Material m_Material;

		[SerializeField]
		private float m_DrawDistance = 1000f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FadeScale = 0.9f;

		[SerializeField]
		[Range(0f, 180f)]
		private float m_StartAngleFade = 180f;

		[SerializeField]
		[Range(0f, 180f)]
		private float m_EndAngleFade = 180f;

		[SerializeField]
		private Vector2 m_UVScale = new Vector2(1f, 1f);

		[SerializeField]
		private Vector2 m_UVBias = new Vector2(0f, 0f);

		[SerializeField]
		private bool m_AffectsTransparency;

		[SerializeField]
		private DecalLayerEnum m_DecalLayerMask = DecalLayerEnum.DecalLayerDefault;

		[SerializeField]
		private DecalScaleMode m_ScaleMode;

		[SerializeField]
		internal Vector3 m_Offset = new Vector3(0f, 0f, 0f);

		[SerializeField]
		internal Vector3 m_Size = new Vector3(1f, 1f, 1f);

		[SerializeField]
		[Range(0f, 1f)]
		private float m_FadeFactor = 1f;

		private Material m_OldMaterial;

		private DecalSystem.DecalHandle m_Handle;

		private static readonly MigrationDescription<Version, DecalProjector> k_Migration = MigrationDescription.New<Version, DecalProjector>(MigrationStep.New(Version.UseZProjectionAxisAndScaleIndependance, delegate(DecalProjector decal)
		{
			decal.m_Size.Scale(decal.transform.lossyScale);
			decal.transform.RotateAround(decal.transform.position, decal.transform.right, 90f);
			foreach (Transform item in decal.transform)
			{
				item.RotateAround(decal.transform.position, decal.transform.right, -90f);
			}
			float y = decal.m_Size.y;
			decal.m_Size.y = decal.m_Size.z;
			decal.m_Size.z = y;
			y = (0f - decal.m_Offset.y) * decal.transform.lossyScale.y;
			decal.m_Offset.y = decal.m_Offset.z * decal.transform.lossyScale.z;
			decal.m_Offset.z = y;
			decal.m_Offset.x *= decal.transform.lossyScale.x;
			if (decal.m_Handle != null)
			{
				DecalSystem.instance.RemoveDecal(decal.m_Handle);
			}
			decal.m_Handle = DecalSystem.instance.AddDecal(decal);
		}), MigrationStep.New(Version.FixPivotPosition, delegate(DecalProjector decal)
		{
			Vector3 vector = decal.m_Offset - new Vector3(0f, 0f, decal.m_Size.z * 0.5f);
			decal.transform.Translate(vector);
			decal.m_Offset.x = 0f;
			decal.m_Offset.y = 0f;
			decal.m_Offset.z = decal.m_Size.z * 0.5f;
			Transform parent = decal.transform.parent;
			if (parent != null)
			{
				vector.x *= parent.transform.lossyScale.x;
				vector.y *= parent.transform.lossyScale.y;
				vector.z *= parent.transform.lossyScale.z;
				vector = decal.transform.rotation * -vector;
			}
			foreach (Transform item2 in decal.transform)
			{
				item2.Translate(vector, Space.World);
			}
			if (decal.m_Handle != null)
			{
				DecalSystem.instance.RemoveDecal(decal.m_Handle);
			}
			decal.m_Handle = DecalSystem.instance.AddDecal(decal);
		}));

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		public Material material
		{
			get
			{
				return m_Material;
			}
			set
			{
				m_Material = value;
				OnValidate();
			}
		}

		public float drawDistance
		{
			get
			{
				return m_DrawDistance;
			}
			set
			{
				m_DrawDistance = Mathf.Max(0f, value);
				OnValidate();
			}
		}

		public float fadeScale
		{
			get
			{
				return m_FadeScale;
			}
			set
			{
				m_FadeScale = Mathf.Clamp01(value);
				OnValidate();
			}
		}

		public float startAngleFade
		{
			get
			{
				return m_StartAngleFade;
			}
			set
			{
				m_StartAngleFade = Mathf.Clamp(value, 0f, 180f);
				OnValidate();
			}
		}

		public float endAngleFade
		{
			get
			{
				return m_EndAngleFade;
			}
			set
			{
				m_EndAngleFade = Mathf.Clamp(value, m_StartAngleFade, 180f);
				OnValidate();
			}
		}

		public Vector2 uvScale
		{
			get
			{
				return m_UVScale;
			}
			set
			{
				m_UVScale = value;
				OnValidate();
			}
		}

		public Vector2 uvBias
		{
			get
			{
				return m_UVBias;
			}
			set
			{
				m_UVBias = value;
				OnValidate();
			}
		}

		public bool affectsTransparency
		{
			get
			{
				return m_AffectsTransparency;
			}
			set
			{
				m_AffectsTransparency = value;
				OnValidate();
			}
		}

		public DecalLayerEnum decalLayerMask
		{
			get
			{
				return m_DecalLayerMask;
			}
			set
			{
				m_DecalLayerMask = value;
			}
		}

		public DecalScaleMode scaleMode
		{
			get
			{
				return m_ScaleMode;
			}
			set
			{
				m_ScaleMode = value;
				OnValidate();
			}
		}

		public Vector3 pivot
		{
			get
			{
				return m_Offset;
			}
			set
			{
				m_Offset = value;
				OnValidate();
			}
		}

		public Vector3 size
		{
			get
			{
				return m_Size;
			}
			set
			{
				m_Size = value;
				OnValidate();
			}
		}

		public float fadeFactor
		{
			get
			{
				return m_FadeFactor;
			}
			set
			{
				m_FadeFactor = Mathf.Clamp01(value);
				OnValidate();
			}
		}

		internal Vector3 effectiveScale
		{
			get
			{
				if (m_ScaleMode != DecalScaleMode.InheritFromHierarchy)
				{
					return Vector3.one;
				}
				return base.transform.lossyScale;
			}
		}

		internal Vector3 position => base.transform.position;

		internal Vector4 uvScaleBias => new Vector4(m_UVScale.x, m_UVScale.y, m_UVBias.x, m_UVBias.y);

		internal DecalSystem.DecalHandle Handle
		{
			get
			{
				return m_Handle;
			}
			set
			{
				m_Handle = value;
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

		public event Action OnMaterialChange;

		public void ResizeAroundPivot(Vector3 newSize)
		{
			for (int i = 0; i < 3; i++)
			{
				if (m_Size[i] > Mathf.Epsilon)
				{
					m_Offset[i] *= newSize[i] / m_Size[i];
				}
			}
			size = newSize;
		}

		internal CachedDecalData GetCachedDecalData()
		{
			CachedDecalData result = default(CachedDecalData);
			result.drawDistance = m_DrawDistance;
			result.fadeScale = m_FadeScale;
			result.startAngleFade = m_StartAngleFade;
			result.endAngleFade = m_EndAngleFade;
			result.uvScaleBias = uvScaleBias;
			result.affectsTransparency = m_AffectsTransparency;
			result.layerMask = base.gameObject.layer;
			result.sceneLayerMask = base.gameObject.sceneCullingMask;
			result.fadeFactor = m_FadeFactor;
			result.decalLayerMask = decalLayerMask;
			return result;
		}

		private void InitMaterial()
		{
			if (m_Material == null)
			{
				m_Material = null;
			}
		}

		private void Reset()
		{
			InitMaterial();
		}

		private void OnEnable()
		{
			InitMaterial();
			if (m_Handle != null)
			{
				DecalSystem.instance.RemoveDecal(m_Handle);
				m_Handle = null;
			}
			m_Handle = DecalSystem.instance.AddDecal(this);
			m_OldMaterial = m_Material;
		}

		private void OnDisable()
		{
			if (m_Handle != null)
			{
				DecalSystem.instance.RemoveDecal(m_Handle);
				m_Handle = null;
			}
		}

		internal void OnValidate()
		{
			if (m_Handle == null)
			{
				return;
			}
			if (m_Material == null)
			{
				DecalSystem.instance.RemoveDecal(m_Handle);
			}
			if (m_OldMaterial != m_Material)
			{
				DecalSystem.instance.RemoveDecal(m_Handle);
				if (m_Material != null)
				{
					m_Handle = DecalSystem.instance.AddDecal(this);
					if (!DecalSystem.IsHDRenderPipelineDecal(m_Material.shader))
					{
						m_AffectsTransparency = false;
					}
				}
				if (this.OnMaterialChange != null)
				{
					this.OnMaterialChange();
				}
				m_OldMaterial = m_Material;
			}
			else
			{
				DecalSystem.instance.UpdateCachedData(m_Handle, this);
			}
		}

		public bool IsValid()
		{
			if (m_Material == null)
			{
				return false;
			}
			return true;
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
