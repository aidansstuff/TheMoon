using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiAimConstraintJob : IWeightedAnimationJob, IAnimationJob
	{
		public enum WorldUpType
		{
			None = 0,
			SceneUp = 1,
			ObjectUp = 2,
			ObjectRotationUp = 3,
			Vector = 4
		}

		private const float k_Epsilon = 1E-05f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle drivenParent;

		public Vector3Property drivenOffset;

		public NativeArray<ReadOnlyTransformHandle> sourceTransforms;

		public NativeArray<PropertyStreamHandle> sourceWeights;

		public NativeArray<Quaternion> sourceOffsets;

		public NativeArray<float> weightBuffer;

		public Vector3 aimAxis;

		public Vector3 upAxis;

		public WorldUpType worldUpType;

		public Vector3 worldUpAxis;

		public ReadOnlyTransformHandle worldUpObject;

		public Vector3 axesMask;

		public FloatProperty minLimit;

		public FloatProperty maxLimit;

		public FloatProperty jobWeight { get; set; }

		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			float num = jobWeight.Get(stream);
			if (num > 0f)
			{
				AnimationStreamHandleUtility.ReadFloats(stream, sourceWeights, weightBuffer);
				float num2 = AnimationRuntimeUtils.Sum(weightBuffer);
				if (num2 < 1E-05f)
				{
					AnimationRuntimeUtils.PassThrough(stream, driven);
					return;
				}
				Vector3 vector = ComputeWorldUpVector(stream);
				Vector3 vector2 = AnimationRuntimeUtils.Select(Vector3.zero, upAxis, axesMask);
				float num3 = ((num2 > 1f) ? (1f / num2) : 1f);
				Vector2 vector3 = new Vector2(minLimit.Get(stream), maxLimit.Get(stream));
				Vector3 position = driven.GetPosition(stream);
				Quaternion localRotation = driven.GetLocalRotation(stream);
				Quaternion quaternion = Quaternion.Inverse(drivenParent.GetRotation(stream));
				Quaternion quaternion2 = QuaternionExt.zero;
				float num4 = 0f;
				for (int i = 0; i < sourceTransforms.Length; i++)
				{
					float num5 = weightBuffer[i] * num3;
					if (num5 < 1E-05f)
					{
						continue;
					}
					ReadOnlyTransformHandle value = sourceTransforms[i];
					Vector3 vector4 = localRotation * aimAxis;
					Vector3 vector5 = quaternion * (value.GetPosition(stream) - position);
					if (vector5.sqrMagnitude < 1E-05f)
					{
						continue;
					}
					Vector3 normalized = Vector3.Cross(vector4, vector5).normalized;
					if (Vector3.Dot(axesMask, axesMask) < 3f)
					{
						normalized = AnimationRuntimeUtils.Select(Vector3.zero, normalized, axesMask).normalized;
						if (Vector3.Dot(normalized, normalized) > 1E-05f)
						{
							vector4 = AnimationRuntimeUtils.ProjectOnPlane(vector4, normalized);
							vector5 = AnimationRuntimeUtils.ProjectOnPlane(vector5, normalized);
						}
						else
						{
							vector5 = vector4;
						}
					}
					Quaternion quaternion3 = Quaternion.AngleAxis(Mathf.Clamp(Vector3.Angle(vector4, vector5), vector3.x, vector3.y), normalized);
					if (worldUpType != 0 && Vector3.Dot(vector2, vector2) > 1E-05f)
					{
						Vector3 normalized2 = Vector3.Cross(Vector3.Cross(quaternion * vector, vector5).normalized, vector5).normalized;
						quaternion3 = QuaternionExt.FromToRotation(Vector3.Cross(Vector3.Cross(localRotation * quaternion3 * vector2, vector5).normalized, vector5).normalized, normalized2) * quaternion3;
					}
					quaternion2 = QuaternionExt.Add(quaternion2, QuaternionExt.Scale(sourceOffsets[i] * quaternion3, num5));
					sourceTransforms[i] = value;
					num4 += num5;
				}
				quaternion2 = QuaternionExt.NormalizeSafe(quaternion2);
				if (num4 < 1f)
				{
					quaternion2 = Quaternion.Lerp(Quaternion.identity, quaternion2, num4);
				}
				Quaternion b = quaternion2 * localRotation;
				if (Vector3.Dot(axesMask, axesMask) < 3f)
				{
					b = Quaternion.Euler(AnimationRuntimeUtils.Select(localRotation.eulerAngles, b.eulerAngles, axesMask));
				}
				Vector3 vector6 = drivenOffset.Get(stream);
				if (Vector3.Dot(vector6, vector6) > 0f)
				{
					b *= Quaternion.Euler(vector6);
				}
				driven.SetLocalRotation(stream, Quaternion.Lerp(localRotation, b, num));
			}
			else
			{
				AnimationRuntimeUtils.PassThrough(stream, driven);
			}
		}

		private Vector3 ComputeWorldUpVector(AnimationStream stream)
		{
			Vector3 result = Vector3.up;
			switch (worldUpType)
			{
			case WorldUpType.None:
				result = Vector3.zero;
				break;
			case WorldUpType.ObjectUp:
			{
				Vector3 vector = Vector3.zero;
				if (worldUpObject.IsValid(stream))
				{
					vector = worldUpObject.GetPosition(stream);
				}
				Vector3 position = driven.GetPosition(stream);
				result = (vector - position).normalized;
				break;
			}
			case WorldUpType.ObjectRotationUp:
			{
				Quaternion quaternion = Quaternion.identity;
				if (worldUpObject.IsValid(stream))
				{
					quaternion = worldUpObject.GetRotation(stream);
				}
				result = quaternion * worldUpAxis;
				break;
			}
			case WorldUpType.Vector:
				result = worldUpAxis;
				break;
			}
			return result;
		}
	}
}
