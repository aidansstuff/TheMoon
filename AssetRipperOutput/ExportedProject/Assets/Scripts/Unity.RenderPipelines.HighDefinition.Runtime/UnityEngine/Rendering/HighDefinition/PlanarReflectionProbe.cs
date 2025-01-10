using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Planar Reflection Probe")]
	public sealed class PlanarReflectionProbe : HDProbe, IVersionable<PlanarReflectionProbe.PlanarProbeVersion>
	{
		private enum PlanarProbeVersion
		{
			Initial = 0,
			First = 2,
			CaptureSettings = 3,
			ProbeSettings = 4,
			SeparatePassThrough = 5,
			UpgradeFrameSettingsToStruct = 6,
			PlanarResolutionScalability = 7
		}

		[SerializeField]
		private Vector3 m_LocalReferencePosition = -Vector3.forward;

		[SerializeField]
		[FormerlySerializedAs("version")]
		[FormerlySerializedAs("m_Version")]
		private int m_PlanarProbeVersion;

		private static readonly MigrationDescription<PlanarProbeVersion, PlanarReflectionProbe> k_PlanarProbeMigration = MigrationDescription.New<PlanarProbeVersion, PlanarReflectionProbe>(MigrationStep.New(PlanarProbeVersion.CaptureSettings, delegate(PlanarReflectionProbe p)
		{
			if (p.m_ObsoleteCaptureSettings == null)
			{
				p.m_ObsoleteCaptureSettings = new ObsoleteCaptureSettings();
			}
			if (p.m_ObsoleteOverrideFieldOfView)
			{
				p.m_ObsoleteCaptureSettings.overrides |= ObsoleteCaptureSettingsOverrides.FieldOfview;
			}
			p.m_ObsoleteCaptureSettings.fieldOfView = p.m_ObsoleteFieldOfViewOverride;
			p.m_ObsoleteCaptureSettings.nearClipPlane = p.m_ObsoleteCaptureNearPlane;
			p.m_ObsoleteCaptureSettings.farClipPlane = p.m_ObsoleteCaptureFarPlane;
		}), MigrationStep.New(PlanarProbeVersion.ProbeSettings, delegate(PlanarReflectionProbe p)
		{
			HDProbe.k_Migration.ExecuteStep(p, Version.ProbeSettings);
			Vector3 position = p.transform.position;
			Matrix4x4 matrix4x = Matrix4x4.TRS(p.transform.position, p.transform.rotation, Vector3.one);
			p.transform.position = matrix4x.MultiplyPoint(p.influenceVolume.obsoleteOffset);
			Quaternion quaternion = p.transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
			Matrix4x4 inverse = p.proxyToWorld.inverse;
			Vector3 mirrorPositionProxySpace = inverse.MultiplyPoint(position);
			Quaternion mirrorRotationProxySpace = inverse.rotation * quaternion;
			p.m_ProbeSettings.proxySettings.mirrorPositionProxySpace = mirrorPositionProxySpace;
			p.m_ProbeSettings.proxySettings.mirrorRotationProxySpace = mirrorRotationProxySpace;
			p.m_LocalReferencePosition = Quaternion.Euler(-90f, 0f, 0f) * -p.m_LocalReferencePosition;
		}), MigrationStep.New(PlanarProbeVersion.SeparatePassThrough, delegate(PlanarReflectionProbe t)
		{
			HDProbe.k_Migration.ExecuteStep(t, Version.SeparatePassThrough);
		}), MigrationStep.New(PlanarProbeVersion.UpgradeFrameSettingsToStruct, delegate(PlanarReflectionProbe t)
		{
			HDProbe.k_Migration.ExecuteStep(t, Version.UpgradeFrameSettingsToStruct);
		}), MigrationStep.New(PlanarProbeVersion.PlanarResolutionScalability, delegate(PlanarReflectionProbe p)
		{
			HDProbe.k_Migration.ExecuteStep(p, Version.PlanarResolutionScalability);
			p.m_ProbeSettings.resolutionScalable.useOverride = true;
			if (p.m_ProbeSettings.resolution != 0)
			{
				p.m_ProbeSettings.resolutionScalable.@override = p.m_ProbeSettings.resolution;
			}
			else
			{
				p.m_ProbeSettings.resolutionScalable.@override = PlanarReflectionAtlasResolution.Resolution512;
			}
		}));

		[SerializeField]
		[FormerlySerializedAs("m_CaptureNearPlane")]
		[Obsolete("For data migration")]
		private float m_ObsoleteCaptureNearPlane = ObsoleteCaptureSettings.@default.nearClipPlane;

		[SerializeField]
		[FormerlySerializedAs("m_CaptureFarPlane")]
		[Obsolete("For data migration")]
		private float m_ObsoleteCaptureFarPlane = ObsoleteCaptureSettings.@default.farClipPlane;

		[SerializeField]
		[FormerlySerializedAs("m_OverrideFieldOfView")]
		[Obsolete("For data migration")]
		private bool m_ObsoleteOverrideFieldOfView;

		[SerializeField]
		[FormerlySerializedAs("m_FieldOfViewOverride")]
		[Obsolete("For data migration")]
		private float m_ObsoleteFieldOfViewOverride = ObsoleteCaptureSettings.@default.fieldOfView;

		public Vector3 localReferencePosition
		{
			get
			{
				return m_LocalReferencePosition;
			}
			set
			{
				m_LocalReferencePosition = value;
			}
		}

		public Vector3 referencePosition => base.transform.TransformPoint(m_LocalReferencePosition);

		PlanarProbeVersion IVersionable<PlanarProbeVersion>.version
		{
			get
			{
				return (PlanarProbeVersion)m_PlanarProbeVersion;
			}
			set
			{
				m_PlanarProbeVersion = (int)value;
			}
		}

		private void Awake()
		{
			base.type = ProbeSettings.ProbeType.PlanarProbe;
			k_PlanarProbeMigration.Migrate(this);
		}
	}
}
