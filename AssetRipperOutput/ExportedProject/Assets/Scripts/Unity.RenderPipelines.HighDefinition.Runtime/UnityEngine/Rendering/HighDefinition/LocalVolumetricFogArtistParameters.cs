using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct LocalVolumetricFogArtistParameters
	{
		[ColorUsage(false)]
		public Color albedo;

		public float meanFreePath;

		public LocalVolumetricFogBlendingMode blendingMode;

		public int priority;

		[FormerlySerializedAs("asymmetry")]
		public float anisotropy;

		public Texture volumeMask;

		public Vector3 textureScrollingSpeed;

		public Vector3 textureTiling;

		[FormerlySerializedAs("m_PositiveFade")]
		public Vector3 positiveFade;

		[FormerlySerializedAs("m_NegativeFade")]
		public Vector3 negativeFade;

		[SerializeField]
		[FormerlySerializedAs("m_UniformFade")]
		internal float m_EditorUniformFade;

		[SerializeField]
		internal Vector3 m_EditorPositiveFade;

		[SerializeField]
		internal Vector3 m_EditorNegativeFade;

		[SerializeField]
		[FormerlySerializedAs("advancedFade")]
		[FormerlySerializedAs("m_AdvancedFade")]
		internal bool m_EditorAdvancedFade;

		public Vector3 size;

		public bool invertFade;

		public float distanceFadeStart;

		public float distanceFadeEnd;

		[SerializeField]
		[FormerlySerializedAs("volumeScrollingAmount")]
		public Vector3 textureOffset;

		public LocalVolumetricFogFalloffMode falloffMode;

		public LocalVolumetricFogMaskMode maskMode;

		public Material materialMask;

		internal const float kMinFogDistance = 0.05f;

		[Obsolete("Never worked correctly due to having engine working in percent. Will be removed soon.")]
		public bool advancedFade => true;

		public LocalVolumetricFogArtistParameters(Color color, float _meanFreePath, float _anisotropy)
		{
			albedo = color;
			meanFreePath = _meanFreePath;
			blendingMode = LocalVolumetricFogBlendingMode.Additive;
			priority = 0;
			anisotropy = _anisotropy;
			volumeMask = null;
			materialMask = null;
			textureScrollingSpeed = Vector3.zero;
			textureTiling = Vector3.one;
			textureOffset = textureScrollingSpeed;
			size = Vector3.one;
			positiveFade = Vector3.one * 0.1f;
			negativeFade = Vector3.one * 0.1f;
			invertFade = false;
			distanceFadeStart = 10000f;
			distanceFadeEnd = 10000f;
			falloffMode = LocalVolumetricFogFalloffMode.Linear;
			maskMode = LocalVolumetricFogMaskMode.Texture;
			m_EditorPositiveFade = positiveFade;
			m_EditorNegativeFade = negativeFade;
			m_EditorUniformFade = 0.1f;
			m_EditorAdvancedFade = false;
		}

		internal void Update(float time)
		{
			if (volumeMask != null)
			{
				textureOffset = -(textureScrollingSpeed * time);
			}
		}

		internal void Constrain()
		{
			albedo.r = Mathf.Clamp01(albedo.r);
			albedo.g = Mathf.Clamp01(albedo.g);
			albedo.b = Mathf.Clamp01(albedo.b);
			albedo.a = 1f;
			meanFreePath = Mathf.Clamp(meanFreePath, 0.05f, float.MaxValue);
			anisotropy = Mathf.Clamp(anisotropy, -1f, 1f);
			textureOffset = Vector3.zero;
			distanceFadeStart = Mathf.Max(0f, distanceFadeStart);
			distanceFadeEnd = Mathf.Max(distanceFadeStart, distanceFadeEnd);
		}

		internal LocalVolumetricFogEngineData ConvertToEngineData()
		{
			LocalVolumetricFogEngineData result = default(LocalVolumetricFogEngineData);
			result.extinction = VolumeRenderingUtils.ExtinctionFromMeanFreePath(meanFreePath);
			result.scattering = VolumeRenderingUtils.ScatteringFromExtinctionAndAlbedo(result.extinction, (Vector4)albedo);
			result.blendingMode = blendingMode;
			result.albedo = (Vector4)albedo;
			result.textureScroll = textureOffset;
			result.textureTiling = textureTiling;
			Vector3 vector = positiveFade;
			Vector3 vector2 = negativeFade;
			result.rcpPosFaceFade.x = Mathf.Min(1f / vector.x, float.MaxValue);
			result.rcpPosFaceFade.y = Mathf.Min(1f / vector.y, float.MaxValue);
			result.rcpPosFaceFade.z = Mathf.Min(1f / vector.z, float.MaxValue);
			result.rcpNegFaceFade.y = Mathf.Min(1f / vector2.y, float.MaxValue);
			result.rcpNegFaceFade.x = Mathf.Min(1f / vector2.x, float.MaxValue);
			result.rcpNegFaceFade.z = Mathf.Min(1f / vector2.z, float.MaxValue);
			result.invertFade = (invertFade ? 1 : 0);
			result.falloffMode = falloffMode;
			float num = Mathf.Max(distanceFadeEnd - distanceFadeStart, 1.526E-05f);
			result.rcpDistFadeLen = 1f / num;
			result.endTimesRcpDistFadeLen = distanceFadeEnd * result.rcpDistFadeLen;
			return result;
		}

		internal void MigrateToFixUniformBlendDistanceToBeMetric()
		{
			if (!m_EditorAdvancedFade)
			{
				m_EditorAdvancedFade = true;
				negativeFade = (positiveFade = m_EditorUniformFade * Vector3.one);
				m_EditorUniformFade = 0f;
			}
			m_EditorPositiveFade = positiveFade;
			m_EditorNegativeFade = negativeFade;
		}
	}
}
