using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ReflectionProbe))]
	public sealed class HDAdditionalReflectionData : HDProbe, IAdditionalData, IVersionable<HDAdditionalReflectionData.ReflectionProbeVersion>
	{
		private enum ReflectionProbeVersion
		{
			First = 0,
			RemoveUsageOfLegacyProbeParamsForStocking = 1,
			HDProbeChild = 2,
			UseInfluenceVolume = 3,
			MergeEditors = 4,
			AddCaptureSettingsAndFrameSettings = 5,
			ModeAndTextures = 6,
			ProbeSettings = 7,
			SeparatePassThrough = 8,
			UpgradeFrameSettingsToStruct = 9
		}

		private ReflectionProbe m_LegacyProbe;

		private static readonly MigrationDescription<ReflectionProbeVersion, HDAdditionalReflectionData> k_ReflectionProbeMigration = MigrationDescription.New<ReflectionProbeVersion, HDAdditionalReflectionData>(MigrationStep.New(ReflectionProbeVersion.RemoveUsageOfLegacyProbeParamsForStocking, delegate(HDAdditionalReflectionData t)
		{
			t.m_ObsoleteBlendDistancePositive = (t.m_ObsoleteBlendDistanceNegative = Vector3.one * t.reflectionProbe.blendDistance);
			t.m_ObsoleteWeight = t.reflectionProbe.importance;
			t.m_ObsoleteMultiplier = t.reflectionProbe.intensity;
			switch (t.reflectionProbe.refreshMode)
			{
			case ReflectionProbeRefreshMode.EveryFrame:
				t.realtimeMode = ProbeSettings.RealtimeMode.EveryFrame;
				break;
			case ReflectionProbeRefreshMode.OnAwake:
				t.realtimeMode = ProbeSettings.RealtimeMode.OnEnable;
				break;
			}
		}), MigrationStep.New(ReflectionProbeVersion.UseInfluenceVolume, delegate(HDAdditionalReflectionData t)
		{
			t.m_ObsoleteInfluenceVolume = t.m_ObsoleteInfluenceVolume ?? new InfluenceVolume();
			t.m_ObsoleteInfluenceVolume.boxSize = t.reflectionProbe.size;
			t.m_ObsoleteInfluenceVolume.obsoleteOffset = t.reflectionProbe.center;
			t.m_ObsoleteInfluenceVolume.sphereRadius = t.m_ObsoleteInfluenceSphereRadius;
			t.m_ObsoleteInfluenceVolume.shape = t.m_ObsoleteInfluenceShape;
			t.m_ObsoleteInfluenceVolume.boxBlendDistancePositive = t.m_ObsoleteBlendDistancePositive;
			t.m_ObsoleteInfluenceVolume.boxBlendDistanceNegative = t.m_ObsoleteBlendDistanceNegative;
			t.m_ObsoleteInfluenceVolume.boxBlendNormalDistancePositive = t.m_ObsoleteBlendNormalDistancePositive;
			t.m_ObsoleteInfluenceVolume.boxBlendNormalDistanceNegative = t.m_ObsoleteBlendNormalDistanceNegative;
			t.m_ObsoleteInfluenceVolume.boxSideFadePositive = t.m_ObsoleteBoxSideFadePositive;
			t.m_ObsoleteInfluenceVolume.boxSideFadeNegative = t.m_ObsoleteBoxSideFadeNegative;
		}), MigrationStep.New(ReflectionProbeVersion.MergeEditors, delegate(HDAdditionalReflectionData t)
		{
			t.m_ObsoleteInfiniteProjection = !t.reflectionProbe.boxProjection;
			t.reflectionProbe.boxProjection = false;
		}), MigrationStep.New(ReflectionProbeVersion.AddCaptureSettingsAndFrameSettings, delegate(HDAdditionalReflectionData t)
		{
			t.m_ObsoleteCaptureSettings = t.m_ObsoleteCaptureSettings ?? new ObsoleteCaptureSettings();
			t.m_ObsoleteCaptureSettings.cullingMask = t.reflectionProbe.cullingMask;
			t.m_ObsoleteCaptureSettings.nearClipPlane = t.reflectionProbe.nearClipPlane;
			t.m_ObsoleteCaptureSettings.farClipPlane = t.reflectionProbe.farClipPlane;
		}), MigrationStep.New(ReflectionProbeVersion.ModeAndTextures, delegate(HDAdditionalReflectionData t)
		{
			t.m_ObsoleteMode = (ProbeSettings.Mode)t.reflectionProbe.mode;
			t.SetTexture(ProbeSettings.Mode.Baked, t.reflectionProbe.bakedTexture);
			t.SetTexture(ProbeSettings.Mode.Custom, t.reflectionProbe.customBakedTexture);
		}), MigrationStep.New(ReflectionProbeVersion.ProbeSettings, delegate(HDAdditionalReflectionData t)
		{
			HDProbe.k_Migration.ExecuteStep(t, Version.ProbeSettings);
			Vector3 position = t.transform.position;
			Matrix4x4 matrix4x = Matrix4x4.TRS(t.transform.position, t.transform.rotation, Vector3.one);
			t.transform.position = matrix4x.MultiplyPoint(t.influenceVolume.obsoleteOffset);
			Vector3 capturePositionProxySpace = t.proxyToWorld.inverse.MultiplyPoint(position);
			t.m_ProbeSettings.proxySettings.capturePositionProxySpace = capturePositionProxySpace;
		}), MigrationStep.New(ReflectionProbeVersion.SeparatePassThrough, delegate(HDAdditionalReflectionData t)
		{
			HDProbe.k_Migration.ExecuteStep(t, Version.SeparatePassThrough);
		}), MigrationStep.New(ReflectionProbeVersion.UpgradeFrameSettingsToStruct, delegate(HDAdditionalReflectionData t)
		{
			HDProbe.k_Migration.ExecuteStep(t, Version.UpgradeFrameSettingsToStruct);
		}));

		[SerializeField]
		[FormerlySerializedAs("version")]
		[FormerlySerializedAs("m_Version")]
		private int m_ReflectionProbeVersion;

		[SerializeField]
		[FormerlySerializedAs("influenceShape")]
		[Obsolete("influenceShape is deprecated, use influenceVolume parameters instead")]
		private InfluenceShape m_ObsoleteInfluenceShape;

		[SerializeField]
		[FormerlySerializedAs("influenceSphereRadius")]
		[Obsolete("influenceSphereRadius is deprecated, use influenceVolume parameters instead")]
		private float m_ObsoleteInfluenceSphereRadius = 3f;

		[SerializeField]
		[FormerlySerializedAs("blendDistancePositive")]
		[Obsolete("blendDistancePositive is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBlendDistancePositive = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("blendDistanceNegative")]
		[Obsolete("blendDistanceNegative is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBlendDistanceNegative = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("blendNormalDistancePositive")]
		[Obsolete("blendNormalDistancePositive is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBlendNormalDistancePositive = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("blendNormalDistanceNegative")]
		[Obsolete("blendNormalDistanceNegative is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBlendNormalDistanceNegative = Vector3.zero;

		[SerializeField]
		[FormerlySerializedAs("boxSideFadePositive")]
		[Obsolete("boxSideFadePositive is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBoxSideFadePositive = Vector3.one;

		[SerializeField]
		[FormerlySerializedAs("boxSideFadeNegative")]
		[Obsolete("boxSideFadeNegative is deprecated, use influenceVolume parameters instead")]
		private Vector3 m_ObsoleteBoxSideFadeNegative = Vector3.one;

		private ReflectionProbe reflectionProbe
		{
			get
			{
				if (m_LegacyProbe == null || m_LegacyProbe.Equals(null))
				{
					m_LegacyProbe = GetComponent<ReflectionProbe>();
				}
				return m_LegacyProbe;
			}
		}

		ReflectionProbeVersion IVersionable<ReflectionProbeVersion>.version
		{
			get
			{
				return (ReflectionProbeVersion)m_ReflectionProbeVersion;
			}
			set
			{
				m_ReflectionProbeVersion = (int)value;
			}
		}

		private void Awake()
		{
			base.type = ProbeSettings.ProbeType.ReflectionProbe;
			k_ReflectionProbeMigration.Migrate(this);
		}

		public override void PrepareCulling()
		{
			base.PrepareCulling();
			InfluenceVolume influence = base.settings.influence;
			Transform transform = base.transform;
			Vector3 position = transform.position;
			ReflectionProbe reflectionProbe = this.reflectionProbe;
			if (!(reflectionProbe == null) && !reflectionProbe.Equals(null))
			{
				switch (influence.shape)
				{
				case InfluenceShape.Box:
					reflectionProbe.size = influence.boxSize;
					reflectionProbe.center = Vector3.zero;
					break;
				case InfluenceShape.Sphere:
					reflectionProbe.size = Vector3.one * (2f * influence.sphereRadius);
					reflectionProbe.center = Vector3.zero;
					break;
				}
				transform.position = position;
				reflectionProbe.mode = ReflectionProbeMode.Custom;
				reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
				if (m_ProbeSettings.mode == ProbeSettings.Mode.Realtime)
				{
					reflectionProbe.renderDynamicObjects = true;
				}
			}
		}

		internal bool ReflectionProbeIsEnabled()
		{
			return reflectionProbe.enabled;
		}
	}
}
