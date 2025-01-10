using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition.Attributes;
using UnityEngine.Video;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[Serializable]
	internal class CompositorLayer
	{
		public enum LayerType
		{
			Camera = 0,
			Video = 1,
			Image = 2
		}

		public enum UIColorBufferFormat
		{
			R11G11B10 = 74,
			R16G16B16A16 = 48,
			R32G32B32A32 = 52
		}

		public enum OutputTarget
		{
			CompositorLayer = 0,
			CameraStack = 1
		}

		public enum ResolutionScale
		{
			Full = 1,
			Half = 2,
			Quarter = 4
		}

		[SerializeField]
		private string m_LayerName;

		[SerializeField]
		private OutputTarget m_OutputTarget;

		[SerializeField]
		private bool m_ClearDepth;

		[SerializeField]
		private bool m_ClearAlpha = true;

		[SerializeField]
		private Renderer m_OutputRenderer;

		[SerializeField]
		private LayerType m_Type;

		[SerializeField]
		private Camera m_Camera;

		[SerializeField]
		private VideoPlayer m_InputVideo;

		[SerializeField]
		private Texture m_InputTexture;

		[SerializeField]
		private BackgroundFitMode m_BackgroundFit;

		[SerializeField]
		private ResolutionScale m_ResolutionScale = ResolutionScale.Full;

		[SerializeField]
		private UIColorBufferFormat m_ColorBufferFormat = UIColorBufferFormat.R16G16B16A16;

		[SerializeField]
		private bool m_OverrideAntialiasing;

		[SerializeField]
		private HDAdditionalCameraData.AntialiasingMode m_Antialiasing;

		[SerializeField]
		private bool m_OverrideClearMode;

		[SerializeField]
		private HDAdditionalCameraData.ClearColorMode m_ClearMode = HDAdditionalCameraData.ClearColorMode.Color;

		[SerializeField]
		private bool m_OverrideCullingMask;

		[SerializeField]
		private LayerMask m_CullingMask;

		[SerializeField]
		private bool m_OverrideVolumeMask;

		[SerializeField]
		private LayerMask m_VolumeMask;

		[SerializeField]
		private int m_LayerPositionInStack;

		[SerializeField]
		private List<CompositionFilter> m_InputFilters = new List<CompositionFilter>();

		[SerializeField]
		private MaterialSharedProperty m_AOVBitmask;

		[SerializeField]
		private Dictionary<string, int> m_AOVMap;

		private List<RTHandle> m_AOVHandles;

		[SerializeField]
		private List<RenderTexture> m_AOVRenderTargets;

		private RTHandle m_RTHandle;

		[SerializeField]
		private RenderTexture m_RenderTarget;

		[SerializeField]
		private RTHandle m_AOVTmpRTHandle;

		[SerializeField]
		private bool m_ClearsBackGround;

		private static readonly string[] k_AOVNames = Enum.GetNames(typeof(MaterialSharedProperty));

		[SerializeField]
		private bool m_Show = true;

		[SerializeField]
		private Camera m_LayerCamera;

		[SerializeField]
		private float m_AlphaMin;

		[SerializeField]
		private float m_AlphaMax = 1f;

		public string name => m_LayerName;

		public OutputTarget outputTarget => m_OutputTarget;

		public Camera sourceCamera => m_Camera;

		public bool hasLayerOverrides
		{
			get
			{
				if (!m_OverrideAntialiasing && !m_OverrideCullingMask && !m_OverrideVolumeMask)
				{
					return m_OverrideClearMode;
				}
				return true;
			}
		}

		public bool clearsBackGround
		{
			get
			{
				return m_ClearsBackGround;
			}
			set
			{
				m_ClearsBackGround = value;
			}
		}

		public bool enabled
		{
			get
			{
				return m_Show;
			}
			set
			{
				m_Show = value;
			}
		}

		public float aspectRatio
		{
			get
			{
				CompositionManager instance = CompositionManager.GetInstance();
				if (instance != null && instance.outputCamera != null)
				{
					return (float)instance.outputCamera.pixelWidth / (float)instance.outputCamera.pixelHeight;
				}
				return 1f;
			}
		}

		public Camera camera => m_LayerCamera;

		internal bool isUsingACameraClone => !m_LayerCamera.Equals(m_Camera);

		public int pixelWidth
		{
			get
			{
				CompositionManager instance = CompositionManager.GetInstance();
				if ((bool)instance && (bool)instance.outputCamera)
				{
					return (int)(EnumToScale(m_ResolutionScale) * (float)instance.outputCamera.pixelWidth);
				}
				return 0;
			}
		}

		public int pixelHeight
		{
			get
			{
				CompositionManager instance = CompositionManager.GetInstance();
				if ((bool)instance && (bool)instance.outputCamera)
				{
					return (int)(EnumToScale(m_ResolutionScale) * (float)instance.outputCamera.pixelHeight);
				}
				return 0;
			}
		}

		private CompositorLayer()
		{
		}

		public static CompositorLayer CreateStackLayer(LayerType type = LayerType.Camera, string layerName = "New Layer")
		{
			CompositorLayer compositorLayer = new CompositorLayer();
			compositorLayer.m_LayerName = layerName;
			compositorLayer.m_Type = type;
			compositorLayer.m_Camera = CompositionManager.GetSceneCamera();
			compositorLayer.m_CullingMask = (compositorLayer.m_Camera ? compositorLayer.m_Camera.cullingMask : 0);
			compositorLayer.m_OutputTarget = OutputTarget.CameraStack;
			compositorLayer.m_ClearDepth = true;
			if (compositorLayer.m_Type == LayerType.Image || compositorLayer.m_Type == LayerType.Video)
			{
				if (compositorLayer.m_Camera == null)
				{
					compositorLayer.m_Camera = CompositionManager.CreateCamera(layerName);
				}
				compositorLayer.m_OverrideCullingMask = true;
				compositorLayer.m_CullingMask = 0;
				compositorLayer.m_OverrideVolumeMask = true;
				compositorLayer.m_VolumeMask = 0;
				compositorLayer.m_ClearAlpha = false;
				compositorLayer.m_OverrideAntialiasing = true;
				compositorLayer.m_Antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
			}
			return compositorLayer;
		}

		public static CompositorLayer CreateOutputLayer(string layerName)
		{
			return new CompositorLayer
			{
				m_LayerName = layerName,
				m_OutputTarget = OutputTarget.CompositorLayer
			};
		}

		private static float EnumToScale(ResolutionScale scale)
		{
			return 1f / (float)scale;
		}

		private static T AddComponent<T>(GameObject go) where T : Component
		{
			return go.AddComponent<T>();
		}

		public void Init(string layerID = "")
		{
			if (m_LayerName == "")
			{
				m_LayerName = layerID;
			}
			CompositionManager instance = CompositionManager.GetInstance();
			if (m_LayerCamera == null && m_OutputTarget == OutputTarget.CameraStack)
			{
				bool flag = !enabled && m_LayerPositionInStack == 0 && (bool)m_Camera;
				if (m_Type != LayerType.Image && m_Type != LayerType.Video && !hasLayerOverrides && !flag && !instance.IsThisCameraShared(m_Camera))
				{
					m_LayerCamera = m_Camera;
				}
				else
				{
					GameObject gameObject = new GameObject("Layer " + layerID)
					{
						hideFlags = (HideFlags.HideAndDontSave | HideFlags.HideInInspector)
					};
					m_LayerCamera = gameObject.AddComponent<Camera>();
					gameObject.AddComponent<HDAdditionalCameraData>();
					CopyInternalCameraData();
					CompositorCameraRegistry.GetInstance().RegisterInternalCamera(m_LayerCamera);
					m_LayerCamera.name = "Compositor" + layerID;
					m_LayerCamera.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
					if (m_LayerCamera.tag == "MainCamera")
					{
						m_LayerCamera.tag = "Untagged";
					}
				}
			}
			m_ClearsBackGround = false;
			m_LayerPositionInStack = 0;
			if (m_ColorBufferFormat != UIColorBufferFormat.R11G11B10 && m_ColorBufferFormat != UIColorBufferFormat.R16G16B16A16 && m_ColorBufferFormat != UIColorBufferFormat.R32G32B32A32)
			{
				m_ColorBufferFormat = UIColorBufferFormat.R16G16B16A16;
			}
			if (m_OutputTarget != OutputTarget.CameraStack && m_RenderTarget == null && instance.outputCamera.pixelWidth > 0 && instance.outputCamera.pixelHeight > 0)
			{
				float num = EnumToScale(m_ResolutionScale);
				int width = (int)(num * (float)instance.outputCamera.pixelWidth);
				int height = (int)(num * (float)instance.outputCamera.pixelHeight);
				m_RenderTarget = new RenderTexture(width, height, 24, (GraphicsFormat)m_ColorBufferFormat);
			}
			if (m_OutputTarget != OutputTarget.CameraStack && m_RTHandle == null && m_RenderTarget != null)
			{
				m_RTHandle = RTHandles.Alloc(m_RenderTarget);
			}
			if (m_OutputTarget != OutputTarget.CameraStack && m_AOVBitmask != 0)
			{
				int num2 = 1 << (int)m_AOVBitmask;
				if (num2 > 1)
				{
					m_AOVMap = new Dictionary<string, int>();
					m_AOVRenderTargets = new List<RenderTexture>();
					m_AOVHandles = new List<RTHandle>();
					string[] names = Enum.GetNames(typeof(MaterialSharedProperty));
					int num3 = names.Length;
					int num4 = 0;
					for (int i = 0; i < num3; i++)
					{
						if ((num2 & (1 << i)) != 0)
						{
							m_AOVMap[names[i]] = num4;
							m_AOVRenderTargets.Add(new RenderTexture(pixelWidth, pixelHeight, 24, (GraphicsFormat)m_ColorBufferFormat));
							m_AOVHandles.Add(RTHandles.Alloc(m_AOVRenderTargets[num4]));
							num4++;
						}
					}
				}
			}
			else
			{
				if (m_AOVRenderTargets != null)
				{
					foreach (RenderTexture aOVRenderTarget in m_AOVRenderTargets)
					{
						CoreUtils.Destroy(aOVRenderTarget);
					}
					m_AOVRenderTargets.Clear();
				}
				if (m_AOVMap != null)
				{
					m_AOVMap.Clear();
					m_AOVMap = null;
				}
			}
			if (m_OutputRenderer != null && Application.IsPlaying(instance.gameObject))
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetTexture("_BaseColorMap", m_RenderTarget);
				m_OutputRenderer.SetPropertyBlock(materialPropertyBlock);
			}
			if ((bool)m_LayerCamera)
			{
				m_LayerCamera.enabled = m_Show;
				HDAdditionalCameraData hDAdditionalCameraData = m_LayerCamera.GetComponent<HDAdditionalCameraData>() ?? AddComponent<HDAdditionalCameraData>(m_LayerCamera.gameObject);
				AdditionalCompositorData additionalCompositorData = m_LayerCamera.GetComponent<AdditionalCompositorData>();
				if (additionalCompositorData == null)
				{
					additionalCompositorData = AddComponent<AdditionalCompositorData>(m_LayerCamera.gameObject);
					additionalCompositorData.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
				}
				if (additionalCompositorData != null)
				{
					additionalCompositorData.ResetData();
				}
				SetLayerMaskOverrides();
				if (m_Type == LayerType.Video && m_InputVideo != null)
				{
					m_InputVideo.targetCamera = m_LayerCamera;
					m_InputVideo.renderMode = VideoRenderMode.CameraNearPlane;
				}
				else if (m_Type == LayerType.Image && m_InputTexture != null)
				{
					hDAdditionalCameraData.clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
					additionalCompositorData.clearColorTexture = m_InputTexture;
					additionalCompositorData.imageFitMode = m_BackgroundFit;
				}
				SetAdditionalLayerData();
				if (m_InputFilters == null)
				{
					m_InputFilters = new List<CompositionFilter>();
				}
			}
		}

		public bool Validate()
		{
			if ((m_OutputTarget != OutputTarget.CameraStack && m_RenderTarget == null) || (m_OutputTarget != OutputTarget.CameraStack && m_RTHandle == null))
			{
				Init();
			}
			if (m_OutputTarget == OutputTarget.CameraStack && m_LayerCamera == null)
			{
				Init();
			}
			return true;
		}

		public void DestroyCameras()
		{
			if (!(m_LayerCamera != null))
			{
				return;
			}
			if (isUsingACameraClone)
			{
				HDAdditionalCameraData component = m_LayerCamera.GetComponent<HDAdditionalCameraData>();
				if ((bool)component)
				{
					CoreUtils.Destroy(component);
				}
				m_LayerCamera.targetTexture = null;
				CompositorCameraRegistry.GetInstance().UnregisterInternalCamera(m_LayerCamera);
				CoreUtils.Destroy(m_LayerCamera);
				m_LayerCamera = null;
			}
			else
			{
				m_LayerCamera.targetTexture = null;
				m_LayerCamera = null;
			}
		}

		public void DestroyRT()
		{
			if (m_RTHandle != null)
			{
				RTHandles.Release(m_RTHandle);
				m_RTHandle = null;
			}
			if (m_RenderTarget != null)
			{
				CoreUtils.Destroy(m_RenderTarget);
				m_RenderTarget = null;
			}
			if (m_AOVHandles != null)
			{
				foreach (RTHandle aOVHandle in m_AOVHandles)
				{
					aOVHandle.Release();
				}
			}
			if (m_AOVRenderTargets != null)
			{
				foreach (RenderTexture aOVRenderTarget in m_AOVRenderTargets)
				{
					CoreUtils.Destroy(aOVRenderTarget);
				}
			}
			m_AOVMap?.Clear();
			m_AOVMap = null;
		}

		public void Destroy()
		{
			DestroyCameras();
			DestroyRT();
		}

		public void SetLayerMaskOverrides()
		{
			if (m_OverrideCullingMask && (bool)m_LayerCamera)
			{
				m_LayerCamera.cullingMask = (m_ClearsBackGround ? ((LayerMask)0) : m_CullingMask);
			}
			if (!m_LayerCamera)
			{
				return;
			}
			HDAdditionalCameraData component = m_LayerCamera.GetComponent<HDAdditionalCameraData>();
			if ((bool)component)
			{
				if (m_OverrideVolumeMask && (bool)m_LayerCamera)
				{
					component.volumeLayerMask = m_VolumeMask;
				}
				component.volumeLayerMask = (int)component.volumeLayerMask | int.MinValue;
				if (m_OverrideAntialiasing)
				{
					component.antialiasing = m_Antialiasing;
				}
				if (m_OverrideClearMode)
				{
					component.clearColorMode = m_ClearMode;
				}
			}
		}

		public void SetAdditionalLayerData()
		{
			if ((bool)m_LayerCamera)
			{
				AdditionalCompositorData component = m_LayerCamera.GetComponent<AdditionalCompositorData>();
				if (component != null)
				{
					component.Init(m_InputFilters, m_ClearAlpha);
					component.alphaMin = m_AlphaMin;
					component.alphaMax = m_AlphaMax;
				}
			}
		}

		internal void CopyInternalCameraData()
		{
			if (!isUsingACameraClone)
			{
				return;
			}
			float depth = m_LayerCamera.depth;
			if ((bool)m_Camera)
			{
				m_LayerCamera.CopyFrom(m_Camera);
				m_LayerCamera.depth = depth;
				HDAdditionalCameraData component = m_Camera.GetComponent<HDAdditionalCameraData>();
				HDAdditionalCameraData component2 = m_LayerCamera.GetComponent<HDAdditionalCameraData>();
				if ((bool)component)
				{
					component.CopyTo(component2);
				}
			}
		}

		public void UpdateOutputCamera()
		{
			if (m_LayerCamera == null)
			{
				return;
			}
			CompositionManager instance = CompositionManager.GetInstance();
			m_LayerCamera.enabled = (m_Show || m_ClearsBackGround) && instance.enableOutput;
			if (m_Type == LayerType.Image)
			{
				AdditionalCompositorData component = m_LayerCamera.GetComponent<AdditionalCompositorData>();
				if ((bool)component)
				{
					component.clearColorTexture = ((m_Show && m_InputTexture != null) ? m_InputTexture : ((m_LayerPositionInStack == 0) ? Texture2D.blackTexture : null));
				}
			}
			if (m_LayerCamera.enabled)
			{
				CopyInternalCameraData();
			}
		}

		public void Update()
		{
			UpdateOutputCamera();
			SetLayerMaskOverrides();
			SetAdditionalLayerData();
		}

		public void SetPriotiry(float priority)
		{
			if ((bool)m_LayerCamera)
			{
				m_LayerCamera.depth = priority;
			}
		}

		public RenderTexture GetRenderTarget(bool allowAOV = true, bool alwaysShow = false)
		{
			if (m_Show || alwaysShow)
			{
				if (m_AOVMap != null && allowAOV)
				{
					using Dictionary<string, int>.Enumerator enumerator = m_AOVMap.GetEnumerator();
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, int> current = enumerator.Current;
						return m_AOVRenderTargets[current.Value];
					}
				}
				return m_RenderTarget;
			}
			return null;
		}

		public bool ValidateRTSize(int referenceWidth, int referenceHeight)
		{
			if (m_RenderTarget == null)
			{
				return true;
			}
			float num = EnumToScale(m_ResolutionScale);
			if (m_RenderTarget.width == Mathf.FloorToInt((float)referenceWidth * num))
			{
				return m_RenderTarget.height == Mathf.FloorToInt((float)referenceHeight * num);
			}
			return false;
		}

		public void SetupClearColor()
		{
			if ((bool)m_LayerCamera && (bool)m_Camera)
			{
				m_LayerCamera.enabled = true;
				m_LayerCamera.cullingMask = 0;
				HDAdditionalCameraData component = m_LayerCamera.GetComponent<HDAdditionalCameraData>();
				HDAdditionalCameraData component2 = m_Camera.GetComponent<HDAdditionalCameraData>();
				component.clearColorMode = component2.clearColorMode;
				component.clearDepth = true;
				m_ClearsBackGround = true;
			}
		}

		public void AddInputFilter(CompositionFilter filter)
		{
			foreach (CompositionFilter inputFilter in m_InputFilters)
			{
				if (inputFilter.filterType == filter.filterType)
				{
					return;
				}
			}
			m_InputFilters.Add(filter);
		}

		public void SetupLayerCamera(CompositorLayer targetLayer, int layerPositionInStack)
		{
			if (!m_LayerCamera || targetLayer == null)
			{
				return;
			}
			if (targetLayer.GetRenderTarget() == null)
			{
				m_LayerCamera.enabled = false;
				return;
			}
			m_LayerPositionInStack = layerPositionInStack;
			HDAdditionalCameraData component = m_LayerCamera.GetComponent<HDAdditionalCameraData>();
			m_LayerCamera.targetTexture = targetLayer.GetRenderTarget(allowAOV: false);
			if (layerPositionInStack != 0)
			{
				component.clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
				AdditionalCompositorData additionalCompositorData = m_LayerCamera.GetComponent<AdditionalCompositorData>();
				if (!additionalCompositorData)
				{
					additionalCompositorData = m_LayerCamera.gameObject.AddComponent<AdditionalCompositorData>();
				}
				if (m_Type != LayerType.Image || (m_Type == LayerType.Image && m_InputTexture == null))
				{
					additionalCompositorData.clearColorTexture = targetLayer.GetRenderTarget();
					additionalCompositorData.clearDepthTexture = targetLayer.m_RTHandle;
				}
				component.volumeLayerMask = (int)component.volumeLayerMask | int.MinValue;
			}
			else
			{
				m_ClearDepth = true;
			}
			component.clearDepth = m_ClearDepth;
			int num = 1 << (int)targetLayer.m_AOVBitmask;
			if (m_Show && num > 1)
			{
				AOVRequestBuilder aOVRequestBuilder = new AOVRequestBuilder();
				int num2 = 0;
				for (int i = 0; i < k_AOVNames.Length; i++)
				{
					if ((num & (1 << i)) != 0)
					{
						int fullscreenOutput = i;
						AOVRequest settings = new AOVRequest(AOVRequest.NewDefault());
						_ = ref settings.SetFullscreenOutput((MaterialSharedProperty)fullscreenOutput);
						int indexLocalCopy = num2;
						aOVRequestBuilder.Add(settings, (AOVBuffers bufferId) => targetLayer.m_AOVTmpRTHandle ?? (targetLayer.m_AOVTmpRTHandle = RTHandles.Alloc(targetLayer.pixelWidth, targetLayer.pixelHeight)), null, new AOVBuffers[1] { AOVBuffers.Color }, delegate(CommandBuffer cmd, List<RTHandle> textures, RenderOutputProperties properties)
						{
							cmd.Blit(textures[0], targetLayer.m_AOVRenderTargets[indexLocalCopy]);
						});
						num2++;
					}
				}
				component.SetAOVRequests(aOVRequestBuilder.Build());
				m_LayerCamera.enabled = true;
			}
			else
			{
				component.SetAOVRequests(null);
			}
		}
	}
}
