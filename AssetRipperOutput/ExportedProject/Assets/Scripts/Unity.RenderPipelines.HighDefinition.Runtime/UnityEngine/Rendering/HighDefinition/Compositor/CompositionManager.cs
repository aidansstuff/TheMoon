using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[AddComponentMenu("")]
	[ExecuteAlways]
	internal class CompositionManager : MonoBehaviour
	{
		public enum OutputDisplay
		{
			Display1 = 0,
			Display2 = 1,
			Display3 = 2,
			Display4 = 3,
			Display5 = 4,
			Display6 = 5,
			Display7 = 6,
			Display8 = 7
		}

		public enum AlphaChannelSupport
		{
			None = 0,
			Rendering = 1,
			RenderingAndPostProcessing = 2
		}

		[SerializeField]
		private bool m_Enable = true;

		[SerializeField]
		private Material m_Material;

		[SerializeField]
		private OutputDisplay m_OutputDisplay;

		[SerializeField]
		private List<CompositorLayer> m_InputLayers = new List<CompositorLayer>();

		internal AlphaChannelSupport m_AlphaSupport = AlphaChannelSupport.RenderingAndPostProcessing;

		internal float timeSinceLastRepaint;

		[SerializeField]
		private Shader m_Shader;

		[HideInInspector]
		[SerializeField]
		private CompositionProfile m_CompositionProfile;

		[SerializeField]
		private Camera m_OutputCamera;

		internal bool m_ShaderPropertiesAreDirty;

		internal Matrix4x4 m_ViewProjMatrix;

		internal Matrix4x4 m_ViewProjMatrixFlipped;

		internal GameObject m_CompositorGameObject;

		internal MaterialPropertyBlock fullscreenProperties;

		private ShaderVariablesGlobal m_ShaderVariablesGlobalCB;

		private int m_RecorderTempRT = Shader.PropertyToID("TempRecorder");

		private static CompositionManager s_CompositorInstance;

		private static Color s_TransparentBlack = new Color(0f, 0f, 0f, 0f);

		private static string s_CompositorGlobalVolumeName = "__Internal_Global_Composition_Volume";

		private static HDRenderPipelineGlobalSettings m_globalSettings;

		public bool enableInternal
		{
			get
			{
				return m_Enable;
			}
			set
			{
				m_Enable = value;
			}
		}

		public List<CompositorLayer> layers => m_InputLayers;

		public AlphaChannelSupport alphaSupport => m_AlphaSupport;

		public bool enableOutput
		{
			get
			{
				if ((bool)m_OutputCamera)
				{
					return m_OutputCamera.enabled;
				}
				return false;
			}
			set
			{
				if (!m_OutputCamera || m_OutputCamera.enabled == value)
				{
					return;
				}
				m_OutputCamera.enabled = value;
				foreach (CompositorLayer inputLayer in m_InputLayers)
				{
					if ((bool)inputLayer.camera && inputLayer.isUsingACameraClone)
					{
						inputLayer.camera.enabled = value;
					}
					else if ((bool)inputLayer.camera && !value)
					{
						inputLayer.camera.targetTexture = null;
					}
				}
				if (value)
				{
					RegisterCustomPasses();
				}
				else
				{
					UnRegisterCustomPasses();
				}
			}
		}

		public int numLayers => m_InputLayers.Count;

		public Shader shader
		{
			get
			{
				return m_Shader;
			}
			set
			{
				m_Shader = value;
			}
		}

		public CompositionProfile profile
		{
			get
			{
				return m_CompositionProfile;
			}
			set
			{
				m_CompositionProfile = value;
			}
		}

		public Camera outputCamera
		{
			get
			{
				return m_OutputCamera;
			}
			set
			{
				m_OutputCamera = value;
			}
		}

		public float aspectRatio
		{
			get
			{
				if ((bool)m_OutputCamera)
				{
					return (float)m_OutputCamera.pixelWidth / (float)m_OutputCamera.pixelHeight;
				}
				return 1f;
			}
		}

		public bool shaderPropertiesAreDirty
		{
			set
			{
				m_ShaderPropertiesAreDirty = true;
			}
		}

		public bool ValidateLayerListOrder(int oldIndex, int newIndex)
		{
			if (m_InputLayers.Count > 1 && m_InputLayers[0].outputTarget == CompositorLayer.OutputTarget.CameraStack)
			{
				CompositorLayer item = m_InputLayers[newIndex];
				m_InputLayers.RemoveAt(newIndex);
				m_InputLayers.Insert(oldIndex, item);
				return false;
			}
			return true;
		}

		public bool RuntimeCheck()
		{
			for (int i = 0; i < m_InputLayers.Count; i++)
			{
				if (!m_InputLayers[i].Validate())
				{
					return false;
				}
			}
			return true;
		}

		private bool ValidatePipeline()
		{
			if (RenderPipelineManager.currentPipeline is HDRenderPipeline hDRenderPipeline)
			{
				m_AlphaSupport = AlphaChannelSupport.RenderingAndPostProcessing;
				if (hDRenderPipeline.GetColorBufferFormat() == GraphicsFormat.B10G11R11_UFloatPack32)
				{
					m_AlphaSupport = AlphaChannelSupport.None;
				}
				else if (hDRenderPipeline.GetColorBufferFormat() == GraphicsFormat.B10G11R11_UFloatPack32)
				{
					m_AlphaSupport = AlphaChannelSupport.Rendering;
				}
				RegisterCustomPasses();
				return true;
			}
			return false;
		}

		private bool ValidateCompositionShader()
		{
			if (m_Shader == null)
			{
				return false;
			}
			if (m_CompositionProfile == null)
			{
				Debug.Log("A composition profile was not found. Set the composition graph from the Compositor window to create one.");
				return false;
			}
			return true;
		}

		private bool ValidateProfile()
		{
			if ((bool)m_CompositionProfile)
			{
				return true;
			}
			Debug.LogError("No composition profile was found! Use the compositor tool to create one.");
			return false;
		}

		private bool ValidateMainCompositorCamera()
		{
			if (m_OutputCamera == null)
			{
				return false;
			}
			HDAdditionalCameraData component = m_OutputCamera.GetComponent<HDAdditionalCameraData>();
			if (component == null)
			{
				m_OutputCamera.gameObject.AddComponent(typeof(HDAdditionalCameraData));
				component = m_OutputCamera.GetComponent<HDAdditionalCameraData>();
			}
			if ((bool)component)
			{
				component.customRender += CustomRender;
			}
			else
			{
				Debug.Log("Null additional data in compositor output");
			}
			return true;
		}

		private bool ValidateAndFixRuntime()
		{
			if (m_OutputCamera == null)
			{
				return false;
			}
			if (m_Shader == null)
			{
				m_InputLayers.Clear();
				m_CompositionProfile = null;
				return false;
			}
			if (m_CompositionProfile == null)
			{
				return false;
			}
			if (m_Material == null)
			{
				SetupCompositionMaterial();
			}
			HDAdditionalCameraData component = m_OutputCamera.GetComponent<HDAdditionalCameraData>();
			if ((bool)component && !component.hasCustomRender)
			{
				component.customRender += CustomRender;
			}
			return true;
		}

		public void DropCompositorCamera()
		{
			if ((bool)m_OutputCamera)
			{
				HDAdditionalCameraData component = m_OutputCamera.GetComponent<HDAdditionalCameraData>();
				if ((bool)component && component.hasCustomRender)
				{
					component.customRender -= CustomRender;
				}
			}
		}

		public void Init()
		{
			if (ValidateCompositionShader() && ValidateProfile() && ValidateMainCompositorCamera())
			{
				UpdateDisplayNumber();
				SetupCompositionMaterial();
				SetupCompositorLayers();
				SetupGlobalCompositorVolume();
				SetupCompositorConstants();
				SetupLayerPriorities();
			}
			else
			{
				Debug.LogError("The compositor was disabled due to a validation error in the configuration.");
				enableInternal = false;
			}
		}

		private void Start()
		{
			Init();
		}

		private void OnValidate()
		{
			if (shader == null)
			{
				m_InputLayers.Clear();
				m_CompositionProfile = null;
			}
		}

		public void OnEnable()
		{
			enableOutput = true;
			s_CompositorInstance = null;
			RenderPipelineManager.beginContextRendering += ResizeCallback;
		}

		public void DeleteLayerRTs()
		{
			for (int num = m_InputLayers.Count - 1; num >= 0; num--)
			{
				m_InputLayers[num].DestroyCameras();
			}
			for (int num2 = m_InputLayers.Count - 1; num2 >= 0; num2--)
			{
				m_InputLayers[num2].DestroyRT();
			}
		}

		public bool IsOutputLayer(int layerID)
		{
			if (layerID >= 0 && layerID < m_InputLayers.Count && m_InputLayers[layerID].outputTarget == CompositorLayer.OutputTarget.CameraStack)
			{
				return false;
			}
			return true;
		}

		public void UpdateDisplayNumber()
		{
			if ((bool)m_OutputCamera)
			{
				m_OutputCamera.targetDisplay = (int)m_OutputDisplay;
			}
		}

		private void SetupCompositorLayers()
		{
			for (int i = 0; i < m_InputLayers.Count; i++)
			{
				m_InputLayers[i].Init($"Layer{i}");
			}
			SetLayerRenderTargets();
		}

		public void SetNewCompositionShader()
		{
			m_Material = null;
			SetupCompositionMaterial();
		}

		public void SetupCompositionMaterial()
		{
			if ((bool)m_Shader)
			{
				m_Material = new Material(m_Shader);
				m_CompositionProfile.AddPropertiesFromShaderAndMaterial(this, m_Shader, m_Material);
				m_CompositionProfile.hideFlags = HideFlags.NotEditable;
			}
			else
			{
				m_CompositionProfile = null;
				m_Material = null;
			}
		}

		public void SetupLayerPriorities()
		{
			int num = 0;
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				inputLayer.SetPriotiry((float)num * 1f);
				num++;
			}
		}

		public void OnAfterAssemblyReload()
		{
			if ((bool)m_OutputCamera)
			{
				HDAdditionalCameraData component = m_OutputCamera.GetComponent<HDAdditionalCameraData>();
				if ((bool)component && !component.hasCustomRender)
				{
					component.customRender += CustomRender;
				}
			}
		}

		public void OnDisable()
		{
			enableOutput = false;
		}

		private void SetupGlobalCompositorVolume()
		{
			Resources.FindObjectsOfTypeAll(typeof(CustomPassVolume));
			m_CompositorGameObject = new GameObject(s_CompositorGlobalVolumeName)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			Volume volume = m_CompositorGameObject.AddComponent<Volume>();
			volume.gameObject.layer = 31;
			volume.profile.Add<AlphaInjection>();
			volume.profile.Add<ChromaKeying>().activate.Override(x: true);
			CustomPassVolume customPassVolume = m_CompositorGameObject.AddComponent<CustomPassVolume>();
			customPassVolume.injectionPoint = CustomPassInjectionPoint.BeforeRendering;
			customPassVolume.AddPassOfType(typeof(CustomClear));
		}

		private void SetupCompositorConstants()
		{
			m_ViewProjMatrix = Matrix4x4.Scale(new Vector3(2f, 2f, 0f)) * Matrix4x4.Translate(new Vector3(-0.5f, -0.5f, 0f));
			m_ViewProjMatrixFlipped = Matrix4x4.Scale(new Vector3(2f, -2f, 0f)) * Matrix4x4.Translate(new Vector3(-0.5f, -0.5f, 0f));
		}

		public void UpdateLayerSetup()
		{
			SetupCompositorLayers();
			SetupLayerPriorities();
		}

		private void LateUpdate()
		{
			if (!enableOutput || !ValidatePipeline() || !ValidateAndFixRuntime() || !RuntimeCheck())
			{
				return;
			}
			UpdateDisplayNumber();
			if (!m_CompositionProfile)
			{
				return;
			}
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				inputLayer.Update();
			}
			SetLayerRenderTargets();
		}

		private void OnDestroy()
		{
			DeleteLayerRTs();
			if (m_CompositorGameObject != null)
			{
				CoreUtils.Destroy(m_CompositorGameObject);
				m_CompositorGameObject = null;
			}
			Object[] array = Resources.FindObjectsOfTypeAll(typeof(CustomPassVolume));
			for (int i = 0; i < array.Length; i++)
			{
				CustomPassVolume customPassVolume = (CustomPassVolume)array[i];
				if (customPassVolume.name == "Global Composition Volume" && customPassVolume.injectionPoint == CustomPassInjectionPoint.BeforeRendering)
				{
					CoreUtils.Destroy(customPassVolume);
				}
			}
			UnRegisterCustomPasses();
			CompositorCameraRegistry.GetInstance().CleanUpCameraOrphans();
			RenderPipelineManager.beginContextRendering -= ResizeCallback;
		}

		public void AddInputFilterAtLayer(CompositionFilter filter, int index)
		{
			m_InputLayers[index].AddInputFilter(filter);
		}

		private int GetBaseLayerForSubLayerAtIndex(int index)
		{
			int result = 0;
			index = ((index > m_InputLayers.Count - 1) ? (m_InputLayers.Count - 1) : index);
			for (int num = index; num >= 0; num--)
			{
				if (m_InputLayers[num].outputTarget == CompositorLayer.OutputTarget.CompositorLayer)
				{
					result = num;
					break;
				}
			}
			return result;
		}

		private static string GetSubLayerName(int count)
		{
			if (count == 0)
			{
				return "New SubLayer";
			}
			return $"New SubLayer ({count + 1})";
		}

		public string GetNewSubLayerName(int index, CompositorLayer.LayerType type = CompositorLayer.LayerType.Camera)
		{
			int baseLayerForSubLayerAtIndex = GetBaseLayerForSubLayerAtIndex(index - 1);
			int num = 0;
			string subLayerName = GetSubLayerName(num);
			int num2 = baseLayerForSubLayerAtIndex + 1;
			while (num2 < m_InputLayers.Count && m_InputLayers[num2].outputTarget != 0)
			{
				if (m_InputLayers[num2].name == subLayerName)
				{
					subLayerName = GetSubLayerName(++num);
					num2 = baseLayerForSubLayerAtIndex + 1;
				}
				else
				{
					num2++;
				}
			}
			return subLayerName;
		}

		public void AddNewLayer(int index, CompositorLayer.LayerType type = CompositorLayer.LayerType.Camera)
		{
			CompositorLayer item = CompositorLayer.CreateStackLayer(type, GetNewSubLayerName(index, type));
			if (index >= 0 && index < m_InputLayers.Count)
			{
				m_InputLayers.Insert(index, item);
			}
			else
			{
				m_InputLayers.Add(item);
			}
		}

		private int GetNumChildrenForLayerAtIndex(int indx)
		{
			if (m_InputLayers[indx].outputTarget == CompositorLayer.OutputTarget.CameraStack)
			{
				return 0;
			}
			int num = 0;
			for (int i = indx + 1; i < m_InputLayers.Count && m_InputLayers[i].outputTarget == CompositorLayer.OutputTarget.CameraStack; i++)
			{
				num++;
			}
			return num;
		}

		public void RemoveLayerAtIndex(int indx)
		{
			for (int num = GetNumChildrenForLayerAtIndex(indx); num >= 0; num--)
			{
				m_InputLayers[indx + num].Destroy();
				m_InputLayers.RemoveAt(indx + num);
			}
		}

		public void SetLayerRenderTargets()
		{
			int num = 0;
			CompositorLayer targetLayer = null;
			for (int i = 0; i < m_InputLayers.Count; i++)
			{
				if (m_InputLayers[i].outputTarget != CompositorLayer.OutputTarget.CameraStack)
				{
					targetLayer = m_InputLayers[i];
					m_InputLayers[i].clearsBackGround = i + 1 >= m_InputLayers.Count || m_InputLayers[i + 1].outputTarget == CompositorLayer.OutputTarget.CompositorLayer;
				}
				if (m_InputLayers[i].outputTarget == CompositorLayer.OutputTarget.CameraStack && i > 0)
				{
					m_InputLayers[i].SetupLayerCamera(targetLayer, num);
					if (!m_InputLayers[i].enabled && num == 0)
					{
						m_InputLayers[i].SetupClearColor();
					}
					num++;
				}
				else
				{
					num = 0;
				}
			}
		}

		public void ReorderChildren(int oldIndex, int newIndex)
		{
			if (m_InputLayers[newIndex].outputTarget != 0)
			{
				return;
			}
			if (oldIndex > newIndex)
			{
				for (int i = 1; oldIndex + i < m_InputLayers.Count && m_InputLayers[oldIndex + i].outputTarget == CompositorLayer.OutputTarget.CameraStack; i++)
				{
					CompositorLayer item = m_InputLayers[oldIndex + i];
					m_InputLayers.RemoveAt(oldIndex + i);
					m_InputLayers.Insert(newIndex + i, item);
				}
			}
			else
			{
				while (m_InputLayers[oldIndex].outputTarget == CompositorLayer.OutputTarget.CameraStack)
				{
					CompositorLayer item2 = m_InputLayers[oldIndex];
					m_InputLayers.RemoveAt(oldIndex);
					m_InputLayers.Insert(newIndex, item2);
				}
			}
		}

		public RenderTexture GetRenderTarget(int indx)
		{
			if (indx >= 0 && indx < m_InputLayers.Count)
			{
				return m_InputLayers[indx].GetRenderTarget(allowAOV: true, alwaysShow: true);
			}
			return null;
		}

		public void Repaint()
		{
			for (int i = 0; i < m_InputLayers.Count; i++)
			{
				if ((bool)m_InputLayers[i].camera)
				{
					m_InputLayers[i].camera.Render();
				}
			}
		}

		private void ResizeCallback(ScriptableRenderContext cntx, List<Camera> cameras)
		{
			if (!m_OutputCamera || !enableOutput)
			{
				return;
			}
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				if (inputLayer.ValidateRTSize(m_OutputCamera.pixelWidth, m_OutputCamera.pixelHeight))
				{
					continue;
				}
				for (int num = m_InputLayers.Count - 1; num >= 0; num--)
				{
					if ((bool)m_InputLayers[num].camera)
					{
						m_InputLayers[num].camera.targetTexture = null;
					}
					m_InputLayers[num].DestroyRT();
				}
				SetupCompositorLayers();
				InternalRender(cntx);
				break;
			}
		}

		private void InternalRender(ScriptableRenderContext cntx)
		{
			HDRenderPipeline hDRenderPipeline = RenderPipelineManager.currentPipeline as HDRenderPipeline;
			if (!enableOutput || hDRenderPipeline == null)
			{
				return;
			}
			List<Camera> list = new List<Camera>(1);
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				if ((bool)inputLayer.camera && inputLayer.camera.enabled)
				{
					list.Clear();
					ScriptableRenderContext.EmitGeometryForCamera(inputLayer.camera);
					list.Add(inputLayer.camera);
					hDRenderPipeline.InternalRender(cntx, list);
				}
			}
		}

		private void CustomRender(ScriptableRenderContext context, HDCamera camera)
		{
			if (camera == null || camera.camera == null || m_Material == null || m_Shader == null)
			{
				CommandBufferPool.Get("Compositor Blit").ClearRenderTarget(clearDepth: false, clearColor: true, Color.black);
				return;
			}
			timeSinceLastRepaint = 0f;
			m_CompositionProfile.CopyPropertiesToMaterial(m_Material);
			int num = 0;
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				if (inputLayer.outputTarget != CompositorLayer.OutputTarget.CameraStack)
				{
					m_Material.SetTexture(inputLayer.name, inputLayer.GetRenderTarget(), RenderTextureSubElement.Color);
				}
				num++;
			}
			CommandBuffer commandBuffer = CommandBufferPool.Get("Compositor Blit");
			camera.UpdateShaderVariablesGlobalCB(ref m_ShaderVariablesGlobalCB, 0);
			m_ShaderVariablesGlobalCB._WorldSpaceCameraPos_Internal = new Vector3(0f, 0f, 0f);
			commandBuffer.SetViewport(new Rect(0f, 0f, camera.camera.pixelWidth, camera.camera.pixelHeight));
			commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: false, Color.red);
			foreach (CompositorLayer inputLayer2 in m_InputLayers)
			{
				if (inputLayer2.clearsBackGround)
				{
					commandBuffer.SetRenderTarget(inputLayer2.GetRenderTarget());
					commandBuffer.ClearRenderTarget(clearDepth: false, clearColor: true, s_TransparentBlack);
				}
			}
			int num2 = m_Material.FindPass("DrawProcedural");
			bool flag = num2 != -1;
			if (!flag)
			{
				num2 = m_Material.FindPass("ForwardOnly");
			}
			if ((bool)camera.camera.targetTexture)
			{
				if (flag)
				{
					CoreUtils.DrawFullScreen(commandBuffer, m_Material, camera.camera.targetTexture, null, num2);
				}
				else
				{
					m_ShaderVariablesGlobalCB._ViewProjMatrix = m_ViewProjMatrixFlipped;
					ConstantBuffer.PushGlobal(commandBuffer, in m_ShaderVariablesGlobalCB, HDShaderIDs._ShaderVariablesGlobal);
					commandBuffer.Blit(null, camera.camera.targetTexture, m_Material, num2);
				}
				IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions = CameraCaptureBridge.GetCaptureActions(camera.camera);
				if (captureActions != null)
				{
					captureActions.Reset();
					while (captureActions.MoveNext())
					{
						captureActions.Current(camera.camera.targetTexture, commandBuffer);
					}
				}
			}
			else
			{
				IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> captureActions2 = CameraCaptureBridge.GetCaptureActions(camera.camera);
				if (captureActions2 != null)
				{
					RenderTextureFormat format = m_InputLayers[0].GetRenderTarget().format;
					commandBuffer.GetTemporaryRT(m_RecorderTempRT, camera.camera.pixelWidth, camera.camera.pixelHeight, 0, FilterMode.Point, format);
					if (flag)
					{
						CoreUtils.DrawFullScreen(commandBuffer, m_Material, m_RecorderTempRT, null, num2);
					}
					else
					{
						m_ShaderVariablesGlobalCB._ViewProjMatrix = m_ViewProjMatrixFlipped;
						ConstantBuffer.PushGlobal(commandBuffer, in m_ShaderVariablesGlobalCB, HDShaderIDs._ShaderVariablesGlobal);
						commandBuffer.Blit(null, m_RecorderTempRT, m_Material, num2);
						captureActions2.Reset();
						while (captureActions2.MoveNext())
						{
							captureActions2.Current(m_RecorderTempRT, commandBuffer);
						}
					}
				}
				if (flag)
				{
					if (fullscreenProperties == null)
					{
						fullscreenProperties = new MaterialPropertyBlock();
					}
					fullscreenProperties.SetFloat("_FlipY", 1f);
					CoreUtils.DrawFullScreen(commandBuffer, m_Material, BuiltinRenderTextureType.CameraTarget, fullscreenProperties, num2);
				}
				else
				{
					m_ShaderVariablesGlobalCB._ViewProjMatrix = m_ViewProjMatrix;
					ConstantBuffer.PushGlobal(commandBuffer, in m_ShaderVariablesGlobalCB, HDShaderIDs._ShaderVariablesGlobal);
					commandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, m_Material, num2);
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			CommandBufferPool.Release(commandBuffer);
		}

		internal bool IsThisCameraShared(Camera camera)
		{
			if (camera == null)
			{
				return false;
			}
			int num = 0;
			foreach (CompositorLayer inputLayer in m_InputLayers)
			{
				if (inputLayer.outputTarget == CompositorLayer.OutputTarget.CameraStack && camera.Equals(inputLayer.sourceCamera))
				{
					num++;
				}
			}
			return num > 1;
		}

		public static Camera GetSceneCamera()
		{
			if (Camera.main != null)
			{
				return Camera.main;
			}
			Camera[] allCameras = Camera.allCameras;
			foreach (Camera camera in allCameras)
			{
				if (camera != GetInstance().outputCamera)
				{
					return camera;
				}
			}
			return null;
		}

		public static Camera CreateCamera(string cameraName)
		{
			GameObject obj = new GameObject(cameraName)
			{
				hideFlags = (HideFlags.HideAndDontSave | HideFlags.HideInInspector)
			};
			Camera result = obj.AddComponent<Camera>();
			obj.AddComponent<HDAdditionalCameraData>();
			return result;
		}

		public static CompositionManager GetInstance()
		{
			return s_CompositorInstance ?? (s_CompositorInstance = Object.FindObjectOfType<CompositionManager>(includeInactive: true));
		}

		public static Vector4 GetAlphaScaleAndBiasForCamera(HDCamera hdCamera)
		{
			AdditionalCompositorData component = null;
			hdCamera.camera.TryGetComponent<AdditionalCompositorData>(out component);
			if ((bool)component)
			{
				float alphaMin = component.alphaMin;
				float num = component.alphaMax;
				if (num == alphaMin)
				{
					num += 0.0001f;
				}
				float num2 = 1f / (num - alphaMin);
				float y = (0f - alphaMin) * num2;
				return new Vector4(num2, y, 0f, 0f);
			}
			return new Vector4(1f, 0f, 0f, 0f);
		}

		internal static Texture GetClearTextureForStackedCamera(HDCamera hdCamera)
		{
			AdditionalCompositorData component = null;
			hdCamera.camera.TryGetComponent<AdditionalCompositorData>(out component);
			if ((bool)component)
			{
				return component.clearColorTexture;
			}
			return null;
		}

		internal static RenderTexture GetClearDepthForStackedCamera(HDCamera hdCamera)
		{
			AdditionalCompositorData component = null;
			hdCamera.camera.TryGetComponent<AdditionalCompositorData>(out component);
			if ((bool)component)
			{
				return component.clearDepthTexture;
			}
			return null;
		}

		internal static void RegisterCustomPasses()
		{
			if (m_globalSettings != HDRenderPipelineGlobalSettings.instance)
			{
				UnRegisterCustomPasses();
				m_globalSettings = null;
			}
			if (m_globalSettings == null)
			{
				m_globalSettings = HDRenderPipelineGlobalSettings.instance;
			}
			if (!(m_globalSettings == null) && m_globalSettings.beforePostProcessCustomPostProcesses != null)
			{
				if (!m_globalSettings.beforePostProcessCustomPostProcesses.Contains(typeof(ChromaKeying).AssemblyQualifiedName))
				{
					m_globalSettings.beforePostProcessCustomPostProcesses.Add(typeof(ChromaKeying).AssemblyQualifiedName);
				}
				if (!m_globalSettings.beforePostProcessCustomPostProcesses.Contains(typeof(AlphaInjection).AssemblyQualifiedName))
				{
					m_globalSettings.beforePostProcessCustomPostProcesses.Add(typeof(AlphaInjection).AssemblyQualifiedName);
				}
			}
		}

		internal static void UnRegisterCustomPasses()
		{
			if (!(m_globalSettings == null) && m_globalSettings.beforePostProcessCustomPostProcesses != null)
			{
				if (m_globalSettings.beforePostProcessCustomPostProcesses.Contains(typeof(ChromaKeying).AssemblyQualifiedName))
				{
					m_globalSettings.beforePostProcessCustomPostProcesses.Remove(typeof(ChromaKeying).AssemblyQualifiedName);
				}
				if (m_globalSettings.beforePostProcessCustomPostProcesses.Contains(typeof(AlphaInjection).AssemblyQualifiedName))
				{
					m_globalSettings.beforePostProcessCustomPostProcesses.Remove(typeof(AlphaInjection).AssemblyQualifiedName);
				}
			}
		}
	}
}
