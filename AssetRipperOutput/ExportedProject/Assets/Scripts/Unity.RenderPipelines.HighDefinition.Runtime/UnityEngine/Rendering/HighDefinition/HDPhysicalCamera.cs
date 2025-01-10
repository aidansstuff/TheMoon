using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[Obsolete("Properties have been migrated to Camera class", false)]
	public struct HDPhysicalCamera
	{
		public const float kMinAperture = 0.7f;

		public const float kMaxAperture = 32f;

		public const int kMinBladeCount = 3;

		public const int kMaxBladeCount = 11;

		[SerializeField]
		[Min(1f)]
		private int m_Iso;

		[SerializeField]
		[Min(0f)]
		private float m_ShutterSpeed;

		[SerializeField]
		[Range(0.7f, 32f)]
		private float m_Aperture;

		[SerializeField]
		[Min(0.1f)]
		private float m_FocusDistance;

		[SerializeField]
		[Range(3f, 11f)]
		private int m_BladeCount;

		[SerializeField]
		private Vector2 m_Curvature;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_BarrelClipping;

		[SerializeField]
		[Range(-1f, 1f)]
		private float m_Anamorphism;

		public float focusDistance
		{
			get
			{
				return m_FocusDistance;
			}
			set
			{
				m_FocusDistance = Mathf.Max(value, 0.1f);
			}
		}

		public int iso
		{
			get
			{
				return m_Iso;
			}
			set
			{
				m_Iso = Mathf.Max(value, 1);
			}
		}

		public float shutterSpeed
		{
			get
			{
				return m_ShutterSpeed;
			}
			set
			{
				m_ShutterSpeed = Mathf.Max(value, 0f);
			}
		}

		public float aperture
		{
			get
			{
				return m_Aperture;
			}
			set
			{
				m_Aperture = Mathf.Clamp(value, 0.7f, 32f);
			}
		}

		public int bladeCount
		{
			get
			{
				return m_BladeCount;
			}
			set
			{
				m_BladeCount = Mathf.Clamp(value, 3, 11);
			}
		}

		public Vector2 curvature
		{
			get
			{
				return m_Curvature;
			}
			set
			{
				m_Curvature.x = Mathf.Max(value.x, 0.7f);
				m_Curvature.y = Mathf.Min(value.y, 32f);
			}
		}

		public float barrelClipping
		{
			get
			{
				return m_BarrelClipping;
			}
			set
			{
				m_BarrelClipping = Mathf.Clamp01(value);
			}
		}

		public float anamorphism
		{
			get
			{
				return m_Anamorphism;
			}
			set
			{
				m_Anamorphism = Mathf.Clamp(value, -1f, 1f);
			}
		}

		[Obsolete("The CopyTo method is obsolete and does not work anymore. Use the assignement operator instead to get a copy of the HDPhysicalCamera parameters.", true)]
		public void CopyTo(HDPhysicalCamera c)
		{
		}

		public static HDPhysicalCamera GetDefaults()
		{
			HDPhysicalCamera result = default(HDPhysicalCamera);
			result.iso = 200;
			result.shutterSpeed = 0.005f;
			result.aperture = 16f;
			result.focusDistance = 10f;
			result.bladeCount = 5;
			result.curvature = new Vector2(2f, 11f);
			result.barrelClipping = 0.25f;
			result.anamorphism = 0f;
			return result;
		}
	}
}
