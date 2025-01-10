using System;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[RequireComponent(typeof(Light))]
	[Obsolete("This component will be removed in the future, it's content have been moved to HDAdditionalLightData.")]
	[ExecuteAlways]
	internal class AdditionalShadowData : MonoBehaviour
	{
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.customResolution instead.")]
		[FormerlySerializedAs("shadowResolution")]
		internal int customResolution = 512;

		[SerializeField]
		[Range(0f, 1f)]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowDimmer instead.")]
		internal float shadowDimmer = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Obsolete("Obsolete, use HDAdditionalLightData.volumetricShadowDimmer instead.")]
		internal float volumetricShadowDimmer = 1f;

		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowFadeDistance instead.")]
		internal float shadowFadeDistance = 10000f;

		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.contactShadows instead.")]
		internal bool contactShadows;

		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowTint instead.")]
		internal Color shadowTint = Color.black;

		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.normalBias instead.")]
		internal float normalBias = 0.75f;

		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowUpdateMode instead.")]
		internal ShadowUpdateMode shadowUpdateMode;

		[HideInInspector]
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowCascadeRatios instead.")]
		internal float[] shadowCascadeRatios = new float[3] { 0.05f, 0.2f, 0.3f };

		[HideInInspector]
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowCascadeBorders instead.")]
		internal float[] shadowCascadeBorders = new float[4] { 0.2f, 0.2f, 0.2f, 0.2f };

		[HideInInspector]
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowAlgorithm instead.")]
		internal int shadowAlgorithm;

		[HideInInspector]
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowVariant instead.")]
		internal int shadowVariant;

		[HideInInspector]
		[SerializeField]
		[Obsolete("Obsolete, use HDAdditionalLightData.shadowPrecision instead.")]
		internal int shadowPrecision;
	}
}
