using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEngine.VFX.Utility
{
	[VFXBinder("HDRP/HDRP Camera")]
	public class HDRPCameraBinder : VFXBinderBase
	{
		public HDAdditionalCameraData AdditionalData;

		private Camera m_Camera;

		[VFXPropertyBinding(new string[] { "UnityEditor.VFX.CameraType" })]
		[SerializeField]
		private ExposedProperty CameraProperty = "Camera";

		private RTHandle m_Texture;

		private ExposedProperty m_Position;

		private ExposedProperty m_Angles;

		private ExposedProperty m_Scale;

		private ExposedProperty m_FieldOfView;

		private ExposedProperty m_NearPlane;

		private ExposedProperty m_FarPlane;

		private ExposedProperty m_AspectRatio;

		private ExposedProperty m_Dimensions;

		private ExposedProperty m_ScaledDimensions;

		private ExposedProperty m_DepthBuffer;

		private ExposedProperty m_ColorBuffer;

		private ExposedProperty m_Orthographic;

		private ExposedProperty m_OrthographicSize;

		private ExposedProperty m_LensShift;

		public void SetCameraProperty(string name)
		{
			CameraProperty = name;
			UpdateSubProperties();
		}

		private void UpdateSubProperties()
		{
			if (AdditionalData != null)
			{
				m_Camera = AdditionalData.GetComponent<Camera>();
			}
			m_Position = CameraProperty + "_transform_position";
			m_Angles = CameraProperty + "_transform_angles";
			m_Scale = CameraProperty + "_transform_scale";
			m_Orthographic = CameraProperty + "_orthographic";
			m_FieldOfView = CameraProperty + "_fieldOfView";
			m_NearPlane = CameraProperty + "_nearPlane";
			m_FarPlane = CameraProperty + "_farPlane";
			m_OrthographicSize = CameraProperty + "_orthographicSize";
			m_AspectRatio = CameraProperty + "_aspectRatio";
			m_Dimensions = CameraProperty + "_pixelDimensions";
			m_LensShift = CameraProperty + "_lensShift";
			m_DepthBuffer = CameraProperty + "_depthBuffer";
			m_ColorBuffer = CameraProperty + "_colorBuffer";
			m_ScaledDimensions = CameraProperty + "_scaledPixelDimensions";
		}

		private void RequestHDRPBuffersAccess(ref HDAdditionalCameraData.BufferAccess access)
		{
			access.RequestAccess(HDAdditionalCameraData.BufferAccessType.Color);
			access.RequestAccess(HDAdditionalCameraData.BufferAccessType.Depth);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (AdditionalData != null)
			{
				AdditionalData.requestGraphicsBuffer += RequestHDRPBuffersAccess;
			}
			UpdateSubProperties();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (AdditionalData != null)
			{
				AdditionalData.requestGraphicsBuffer -= RequestHDRPBuffersAccess;
			}
		}

		private void OnValidate()
		{
			UpdateSubProperties();
			if (AdditionalData != null)
			{
				AdditionalData.requestGraphicsBuffer += RequestHDRPBuffersAccess;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			if (AdditionalData != null && m_Camera != null && component.HasVector3(m_Position) && component.HasVector3(m_Angles) && component.HasVector3(m_Scale) && component.HasBool(m_Orthographic) && component.HasFloat(m_FieldOfView) && component.HasFloat(m_NearPlane) && component.HasFloat(m_FarPlane) && component.HasFloat(m_OrthographicSize) && component.HasFloat(m_AspectRatio) && component.HasVector2(m_Dimensions) && component.HasVector2(m_LensShift) && component.HasTexture(m_DepthBuffer) && component.HasTexture(m_ColorBuffer))
			{
				return component.HasVector2(m_ScaledDimensions);
			}
			return false;
		}

		public override void UpdateBinding(VisualEffect component)
		{
			RTHandle graphicsBuffer = AdditionalData.GetGraphicsBuffer(HDAdditionalCameraData.BufferAccessType.Depth);
			RTHandle graphicsBuffer2 = AdditionalData.GetGraphicsBuffer(HDAdditionalCameraData.BufferAccessType.Color);
			if (graphicsBuffer != null || graphicsBuffer2 != null)
			{
				component.SetVector3(m_Position, AdditionalData.transform.position);
				component.SetVector3(m_Angles, AdditionalData.transform.eulerAngles);
				component.SetVector3(m_Scale, AdditionalData.transform.lossyScale);
				component.SetBool(m_Orthographic, m_Camera.orthographic);
				component.SetFloat(m_OrthographicSize, m_Camera.orthographicSize);
				component.SetFloat(m_FieldOfView, MathF.PI / 180f * m_Camera.fieldOfView);
				component.SetFloat(m_NearPlane, m_Camera.nearClipPlane);
				component.SetFloat(m_FarPlane, m_Camera.farClipPlane);
				component.SetVector2(m_LensShift, m_Camera.lensShift);
				component.SetFloat(m_AspectRatio, m_Camera.aspect);
				component.SetVector2(m_Dimensions, new Vector2(m_Camera.pixelWidth, m_Camera.pixelHeight));
				DynamicResolutionHandler.UpdateAndUseCamera(m_Camera);
				Vector2 v = DynamicResolutionHandler.instance.GetScaledSize(new Vector2Int(m_Camera.pixelWidth, m_Camera.pixelHeight));
				DynamicResolutionHandler.ClearSelectedCamera();
				component.SetVector2(m_ScaledDimensions, v);
				if (graphicsBuffer != null)
				{
					component.SetTexture(m_DepthBuffer, graphicsBuffer.rt);
				}
				if (graphicsBuffer2 != null)
				{
					component.SetTexture(m_ColorBuffer, graphicsBuffer2.rt);
				}
			}
		}

		public override string ToString()
		{
			return string.Format(string.Format("HDRP Camera : '{0}' -> {1}", (AdditionalData == null) ? "null" : AdditionalData.gameObject.name, CameraProperty));
		}
	}
}
