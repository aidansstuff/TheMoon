using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct ProbeCapturePositionSettings
	{
		[Obsolete("Since 2019.3, use ProbeCapturePositionSettings.NewDefault() instead.")]
		public static readonly ProbeCapturePositionSettings @default;

		public Vector3 proxyPosition;

		public Quaternion proxyRotation;

		public Vector3 referencePosition;

		public Quaternion referenceRotation;

		public Matrix4x4 influenceToWorld;

		public static ProbeCapturePositionSettings NewDefault()
		{
			return new ProbeCapturePositionSettings(Vector3.zero, Quaternion.identity, Vector3.zero, Quaternion.identity, Matrix4x4.identity);
		}

		public ProbeCapturePositionSettings(Vector3 proxyPosition, Quaternion proxyRotation, Matrix4x4 influenceToWorld)
		{
			this.proxyPosition = proxyPosition;
			this.proxyRotation = proxyRotation;
			referencePosition = Vector3.zero;
			referenceRotation = Quaternion.identity;
			this.influenceToWorld = influenceToWorld;
		}

		public ProbeCapturePositionSettings(Vector3 proxyPosition, Quaternion proxyRotation, Vector3 referencePosition, Quaternion referenceRotation, Matrix4x4 influenceToWorld)
		{
			this.proxyPosition = proxyPosition;
			this.proxyRotation = proxyRotation;
			this.referencePosition = referencePosition;
			this.referenceRotation = referenceRotation;
			this.influenceToWorld = influenceToWorld;
		}

		public static ProbeCapturePositionSettings ComputeFrom(HDProbe probe, Transform reference)
		{
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (reference != null)
			{
				vector = reference.position;
				quaternion = reference.rotation;
			}
			else if (probe.type == ProbeSettings.ProbeType.PlanarProbe)
			{
				PlanarReflectionProbe planarReflectionProbe = (PlanarReflectionProbe)probe;
				return ComputeFromMirroredReference(planarReflectionProbe, planarReflectionProbe.referencePosition);
			}
			return ComputeFrom(probe, vector, quaternion);
		}

		public static ProbeCapturePositionSettings ComputeFromMirroredReference(HDProbe probe, Vector3 referencePosition)
		{
			ProbeCapturePositionSettings result = ComputeFrom(probe, referencePosition, Quaternion.identity);
			Vector3 vector = Matrix4x4.TRS(result.proxyPosition, result.proxyRotation, Vector3.one).MultiplyPoint(probe.settings.proxySettings.mirrorPositionProxySpace);
			result.referenceRotation = Quaternion.LookRotation(vector - result.referencePosition);
			return result;
		}

		public Hash128 ComputeHash()
		{
			Hash128 hash = default(Hash128);
			Hash128 hash2 = default(Hash128);
			HashUtilities.QuantisedVectorHash(ref proxyPosition, ref hash);
			HashUtilities.QuantisedVectorHash(ref referencePosition, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			Vector3 value = proxyRotation.eulerAngles;
			HashUtilities.QuantisedVectorHash(ref value, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			value = referenceRotation.eulerAngles;
			HashUtilities.QuantisedVectorHash(ref value, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			return hash;
		}

		private static ProbeCapturePositionSettings ComputeFrom(HDProbe probe, Vector3 referencePosition, Quaternion referenceRotation)
		{
			ProbeCapturePositionSettings result = default(ProbeCapturePositionSettings);
			Matrix4x4 proxyToWorld = probe.proxyToWorld;
			result.proxyPosition = proxyToWorld.GetColumn(3);
			if (Vector3.Distance(result.proxyPosition, referencePosition) < 0.0001f)
			{
				referencePosition += new Vector3(0.0001f, 0.0001f, 0.0001f);
			}
			result.proxyRotation = proxyToWorld.rotation;
			result.referencePosition = referencePosition;
			result.referenceRotation = referenceRotation;
			result.influenceToWorld = probe.influenceToWorld;
			return result;
		}
	}
}
