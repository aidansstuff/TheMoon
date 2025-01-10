using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.NVIDIA;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class DLSSPass
	{
		public struct ViewResourceHandles
		{
			public TextureHandle source;

			public TextureHandle output;

			public TextureHandle depth;

			public TextureHandle motionVectors;

			public TextureHandle biasColorMask;

			public void WriteResources(RenderGraphBuilder builder)
			{
				source = builder.WriteTexture(in source);
				output = builder.WriteTexture(in output);
				depth = builder.WriteTexture(in depth);
				motionVectors = builder.WriteTexture(in motionVectors);
				if (biasColorMask.IsValid())
				{
					biasColorMask = builder.WriteTexture(in biasColorMask);
				}
			}
		}

		public struct CameraResourcesHandles
		{
			internal ViewResourceHandles resources;

			internal bool copyToViews;

			internal ViewResourceHandles tmpView0;

			internal ViewResourceHandles tmpView1;
		}

		public struct Parameters
		{
			public bool resetHistory;

			public float preExposure;

			public HDCamera hdCamera;

			public GlobalDynamicResolutionSettings drsSettings;
		}

		public struct ViewResources
		{
			public Texture source;

			public Texture output;

			public Texture depth;

			public Texture motionVectors;

			public Texture biasColorMask;
		}

		public struct CameraResources
		{
			internal ViewResources resources;

			internal bool copyToViews;

			internal ViewResources tmpView0;

			internal ViewResources tmpView1;
		}

		private struct Resolution
		{
			public uint width;

			public uint height;

			public static bool operator ==(Resolution a, Resolution b)
			{
				if (a.width == b.width)
				{
					return a.height == b.height;
				}
				return false;
			}

			public static bool operator !=(Resolution a, Resolution b)
			{
				return !(a == b);
			}

			public override bool Equals(object obj)
			{
				if (obj is Resolution)
				{
					return (Resolution)obj == this;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (int)(width ^ height);
			}
		}

		private struct DlssViewData
		{
			public DLSSQuality perfQuality;

			public Resolution inputRes;

			public Resolution outputRes;

			public float sharpness;

			public float jitterX;

			public float jitterY;

			public bool reset;

			public bool CanFitInput(in Resolution inputRect)
			{
				if (inputRes.width >= inputRect.width)
				{
					return inputRes.height > inputRect.height;
				}
				return false;
			}
		}

		private struct OptimalSettingsRequest
		{
			public DLSSQuality quality;

			public Rect viewport;

			public OptimalDLSSSettingsData optimalSettings;

			public bool CanFit(Resolution rect)
			{
				if (rect.width >= optimalSettings.minWidth && rect.height >= optimalSettings.minHeight && rect.width <= optimalSettings.maxWidth)
				{
					return rect.height <= optimalSettings.maxHeight;
				}
				return false;
			}
		}

		private class ViewState
		{
			private DLSSContext m_DlssContext;

			private GraphicsDevice m_Device;

			private DlssViewData m_Data;

			private bool m_UsingOptimalSettings;

			private bool m_UseAutomaticSettings;

			private Resolution m_BackbufferRes;

			private OptimalSettingsRequest m_OptimalSettingsRequest;

			public DLSSContext DLSSContext => m_DlssContext;

			public bool useAutomaticSettings => m_UseAutomaticSettings;

			public OptimalSettingsRequest OptimalSettingsRequestData => m_OptimalSettingsRequest;

			public void Init(GraphicsDevice device)
			{
				m_Device = device;
				m_DlssContext = null;
			}

			public void RequestUseAutomaticSettings(bool useAutomaticSettings, DLSSQuality quality, Rect viewport, in OptimalDLSSSettingsData optimalSettings)
			{
				m_UseAutomaticSettings = useAutomaticSettings;
				m_OptimalSettingsRequest.quality = quality;
				m_OptimalSettingsRequest.viewport = viewport;
				m_OptimalSettingsRequest.optimalSettings = optimalSettings;
			}

			public void ClearAutomaticSettings()
			{
				m_UseAutomaticSettings = false;
			}

			private bool ShouldUseAutomaticSettings()
			{
				if (!m_UseAutomaticSettings || m_DlssContext == null)
				{
					return false;
				}
				if (m_DlssContext.initData.quality == m_OptimalSettingsRequest.quality && m_DlssContext.initData.outputRTHeight == (uint)m_OptimalSettingsRequest.viewport.height && m_DlssContext.initData.outputRTWidth == (uint)m_OptimalSettingsRequest.viewport.width)
				{
					return IsOptimalSettingsValid(in m_OptimalSettingsRequest.optimalSettings);
				}
				return false;
			}

			public void UpdateViewState(in DlssViewData viewData, CommandBuffer cmdBuffer)
			{
				bool flag = ShouldUseAutomaticSettings();
				bool flag2 = false;
				if (viewData.outputRes != m_Data.outputRes || viewData.inputRes.width > m_BackbufferRes.width || viewData.inputRes.height > m_BackbufferRes.height || (viewData.inputRes != m_BackbufferRes && !m_OptimalSettingsRequest.CanFit(viewData.inputRes)) || viewData.perfQuality != m_Data.perfQuality || m_DlssContext == null || flag != m_UsingOptimalSettings)
				{
					flag2 = true;
					m_BackbufferRes = viewData.inputRes;
					if (m_DlssContext != null)
					{
						m_Device.DestroyFeature(cmdBuffer, m_DlssContext);
						m_DlssContext = null;
					}
					DLSSCommandInitializationData initSettings = default(DLSSCommandInitializationData);
					initSettings.SetFlag(DLSSFeatureFlags.IsHDR, value: true);
					initSettings.SetFlag(DLSSFeatureFlags.MVLowRes, value: true);
					initSettings.SetFlag(DLSSFeatureFlags.DepthInverted, value: true);
					initSettings.SetFlag(DLSSFeatureFlags.DoSharpening, value: true);
					initSettings.inputRTWidth = m_BackbufferRes.width;
					initSettings.inputRTHeight = m_BackbufferRes.height;
					initSettings.outputRTWidth = viewData.outputRes.width;
					initSettings.outputRTHeight = viewData.outputRes.height;
					initSettings.quality = viewData.perfQuality;
					m_UsingOptimalSettings = flag;
					m_DlssContext = m_Device.CreateFeature(cmdBuffer, in initSettings);
				}
				m_Data = viewData;
				m_Data.reset = flag2 || viewData.reset;
			}

			public void SubmitDlssCommands(Texture source, Texture depth, Texture motionVectors, Texture biasColorMask, Texture output, float preExposure, CommandBuffer cmdBuffer)
			{
				if (m_DlssContext != null)
				{
					m_DlssContext.executeData.sharpness = (m_UsingOptimalSettings ? m_OptimalSettingsRequest.optimalSettings.sharpness : m_Data.sharpness);
					m_DlssContext.executeData.mvScaleX = 0f - (float)m_Data.inputRes.width;
					m_DlssContext.executeData.mvScaleY = 0f - (float)m_Data.inputRes.height;
					m_DlssContext.executeData.subrectOffsetX = 0u;
					m_DlssContext.executeData.subrectOffsetY = 0u;
					m_DlssContext.executeData.subrectWidth = m_Data.inputRes.width;
					m_DlssContext.executeData.subrectHeight = m_Data.inputRes.height;
					m_DlssContext.executeData.jitterOffsetX = m_Data.jitterX;
					m_DlssContext.executeData.jitterOffsetY = m_Data.jitterY;
					m_DlssContext.executeData.preExposure = preExposure;
					m_DlssContext.executeData.invertYAxis = 1u;
					m_DlssContext.executeData.invertXAxis = 0u;
					m_DlssContext.executeData.reset = (m_Data.reset ? 1 : 0);
					DLSSTextureTable dLSSTextureTable = default(DLSSTextureTable);
					dLSSTextureTable.colorInput = source;
					dLSSTextureTable.colorOutput = output;
					dLSSTextureTable.depth = depth;
					dLSSTextureTable.motionVectors = motionVectors;
					dLSSTextureTable.biasColorMask = biasColorMask;
					DLSSTextureTable textures = dLSSTextureTable;
					m_Device.ExecuteDLSS(cmdBuffer, m_DlssContext, in textures);
				}
			}

			public void Cleanup(CommandBuffer cmdBuffer)
			{
				if (m_DlssContext != null)
				{
					m_Device.DestroyFeature(cmdBuffer, m_DlssContext);
					m_DlssContext = null;
				}
				m_Device = null;
				m_Data = default(DlssViewData);
				m_UsingOptimalSettings = false;
				m_UseAutomaticSettings = false;
				m_BackbufferRes = default(Resolution);
				m_OptimalSettingsRequest = default(OptimalSettingsRequest);
			}
		}

		private class CameraState
		{
			private WeakReference<Camera> m_CamReference = new WeakReference<Camera>(null);

			private List<ViewState> m_Views;

			private GraphicsDevice m_Device;

			private PerformDynamicRes m_ScaleDelegate;

			public PerformDynamicRes ScaleDelegate => m_ScaleDelegate;

			public List<ViewState> ViewStates => m_Views;

			public ulong LastFrameId { get; set; }

			public CameraState()
			{
				m_ScaleDelegate = ScaleFn;
			}

			public void Init(GraphicsDevice device, Camera camera)
			{
				m_CamReference.SetTarget(camera);
				m_Device = device;
			}

			public bool IsAlive()
			{
				Camera target;
				return m_CamReference.TryGetTarget(out target);
			}

			public void ClearAutomaticSettings()
			{
				if (m_Views == null)
				{
					return;
				}
				foreach (ViewState view in m_Views)
				{
					view.ClearAutomaticSettings();
				}
			}

			private float ScaleFn()
			{
				if (m_Views == null || m_Views.Count == 0)
				{
					return 100f;
				}
				ViewState viewState = m_Views[0];
				if (!viewState.useAutomaticSettings)
				{
					return 100f;
				}
				OptimalDLSSSettingsData optimalSettings = viewState.OptimalSettingsRequestData.optimalSettings;
				Rect viewport = viewState.OptimalSettingsRequestData.viewport;
				float a = (float)optimalSettings.outRenderWidth / viewport.width;
				float b = (float)optimalSettings.outRenderHeight / viewport.height;
				return Mathf.Min(a, b) * 100f;
			}

			public void SubmitCommands(HDCamera camera, float preExposure, in DlssViewData viewData, in CameraResources camResources, CommandBuffer cmdBuffer)
			{
				int num = 1;
				int index = 0;
				if (camera.xr.enabled)
				{
					num = (camera.xr.singlePassEnabled ? camera.xr.viewCount : 2);
					index = camera.xr.multipassId;
				}
				if (m_Views == null || m_Views.Count != num)
				{
					if (m_Views != null)
					{
						Cleanup(cmdBuffer);
					}
					m_Views = ListPool<ViewState>.Get();
					for (int i = 0; i < num; i++)
					{
						ViewState viewState2 = GenericPool<ViewState>.Get();
						viewState2.Init(m_Device);
						m_Views.Add(viewState2);
					}
				}
				if (camResources.copyToViews)
				{
					for (int j = 0; j < m_Views.Count; j++)
					{
						_ = m_Views[j];
						ViewResources viewResources2 = ((j == 0) ? camResources.tmpView0 : camResources.tmpView1);
						cmdBuffer.CopyTexture(camResources.resources.source, j, viewResources2.source, 0);
						cmdBuffer.CopyTexture(camResources.resources.depth, j, viewResources2.depth, 0);
						cmdBuffer.CopyTexture(camResources.resources.motionVectors, j, viewResources2.motionVectors, 0);
						if (camResources.resources.biasColorMask != null)
						{
							cmdBuffer.CopyTexture(camResources.resources.biasColorMask, j, viewResources2.biasColorMask, 0);
						}
					}
					for (int k = 0; k < m_Views.Count; k++)
					{
						ViewState viewState3 = m_Views[k];
						ViewResources viewResources3 = ((k == 0) ? camResources.tmpView0 : camResources.tmpView1);
						RunPass(viewState3, cmdBuffer, in viewData, in viewResources3);
						cmdBuffer.CopyTexture(viewResources3.output, 0, camResources.resources.output, k);
					}
				}
				else
				{
					RunPass(m_Views[index], cmdBuffer, in viewData, in camResources.resources);
				}
				void RunPass(ViewState viewState, CommandBuffer cmdBuffer, in DlssViewData viewData, in ViewResources viewResources)
				{
					viewState.UpdateViewState(in viewData, cmdBuffer);
					viewState.SubmitDlssCommands(viewResources.source, viewResources.depth, viewResources.motionVectors, viewResources.biasColorMask, viewResources.output, preExposure, cmdBuffer);
				}
			}

			public void Cleanup(CommandBuffer cmdBuffer)
			{
				if (m_Views == null)
				{
					return;
				}
				foreach (ViewState view in m_Views)
				{
					view.Cleanup(cmdBuffer);
					GenericPool<ViewState>.Release(view);
				}
				ListPool<ViewState>.Release(m_Views);
				m_Views = null;
				m_CamReference.SetTarget(null);
				m_Device = null;
			}
		}

		private static uint s_ExpectedDeviceVersion = 4u;

		private Dictionary<int, CameraState> m_CameraStates = new Dictionary<int, CameraState>();

		private List<int> m_InvalidCameraKeys = new List<int>();

		private CommandBuffer m_CommandBuffer = new CommandBuffer();

		private ulong m_FrameId;

		private GraphicsDevice m_Device;

		private static ulong sMaximumFrameExpiration = 400uL;

		public static ViewResources GetViewResources(in ViewResourceHandles handles)
		{
			ViewResources viewResources = default(ViewResources);
			viewResources.source = handles.source;
			viewResources.output = handles.output;
			viewResources.depth = handles.depth;
			viewResources.motionVectors = handles.motionVectors;
			ViewResources result = viewResources;
			result.biasColorMask = (handles.biasColorMask.IsValid() ? ((Texture)handles.biasColorMask) : null);
			return result;
		}

		public static CameraResourcesHandles CreateCameraResources(HDCamera camera, RenderGraph renderGraph, RenderGraphBuilder builder, in ViewResourceHandles resources)
		{
			CameraResourcesHandles result = default(CameraResourcesHandles);
			result.resources = resources;
			result.copyToViews = camera.xr.enabled && camera.xr.singlePassEnabled && camera.xr.viewCount > 1;
			if (result.copyToViews)
			{
				CreateCopyNoXR(in resources, out result.tmpView0);
				CreateCopyNoXR(in resources, out result.tmpView1);
			}
			return result;
			void CreateCopyNoXR(in ViewResourceHandles input, out ViewResourceHandles newResources)
			{
				newResources.source = GetTmpViewXrTex(in input.source);
				newResources.output = GetTmpViewXrTex(in input.output);
				newResources.depth = GetTmpViewXrTex(in input.depth);
				newResources.motionVectors = GetTmpViewXrTex(in input.motionVectors);
				newResources.biasColorMask = GetTmpViewXrTex(in input.biasColorMask);
				newResources.WriteResources(builder);
			}
			TextureHandle GetTmpViewXrTex(in TextureHandle handle)
			{
				if (!handle.IsValid())
				{
					return TextureHandle.nullHandle;
				}
				TextureDesc desc = renderGraph.GetTextureDesc(handle);
				desc.slices = 1;
				desc.dimension = TextureDimension.Tex2D;
				return renderGraph.CreateTexture(in desc);
			}
		}

		public static CameraResources GetCameraResources(in CameraResourcesHandles handles)
		{
			CameraResources cameraResources = default(CameraResources);
			cameraResources.resources = GetViewResources(in handles.resources);
			cameraResources.copyToViews = handles.copyToViews;
			CameraResources result = cameraResources;
			if (result.copyToViews)
			{
				result.tmpView0 = GetViewResources(in handles.tmpView0);
				result.tmpView1 = GetViewResources(in handles.tmpView1);
			}
			return result;
		}

		public static bool SetupFeature(HDRenderPipelineGlobalSettings pipelineSettings = null)
		{
			if (!NVUnityPlugin.IsLoaded())
			{
				return false;
			}
			if (s_ExpectedDeviceVersion != GraphicsDevice.version)
			{
				Debug.LogWarning("Cannot instantiate NVIDIA device because the version HDRP expects does not match the backend version.");
				return false;
			}
			if (!SystemInfo.graphicsDeviceVendor.ToLower().Contains("nvidia"))
			{
				return false;
			}
			GraphicsDevice graphicsDevice = null;
			return ((!(pipelineSettings != null) || !pipelineSettings.useDLSSCustomProjectId) ? GraphicsDevice.CreateGraphicsDevice() : GraphicsDevice.CreateGraphicsDevice(pipelineSettings.DLSSProjectId))?.IsFeatureAvailable(GraphicsDeviceFeature.DLSS) ?? false;
		}

		public static DLSSPass Create(HDRenderPipelineGlobalSettings pipelineSettings = null)
		{
			if (!SetupFeature(pipelineSettings))
			{
				return null;
			}
			return new DLSSPass(GraphicsDevice.device);
		}

		public void BeginFrame(HDCamera hdCamera)
		{
			InternalNVIDIABeginFrame(hdCamera);
		}

		public void SetupDRSScaling(bool enableAutomaticSettings, Camera camera, XRPass xrPass, ref GlobalDynamicResolutionSettings dynamicResolutionSettings)
		{
			InternalNVIDIASetupDRSScaling(enableAutomaticSettings, camera, xrPass, ref dynamicResolutionSettings);
		}

		public void Render(Parameters parameters, CameraResources resources, CommandBuffer cmdBuffer)
		{
			InternalNVIDIARender(in parameters, resources, cmdBuffer);
		}

		private DLSSPass(GraphicsDevice device)
		{
			m_Device = device;
		}

		private static bool IsOptimalSettingsValid(in OptimalDLSSSettingsData optimalSettings)
		{
			if (optimalSettings.maxHeight >= optimalSettings.minHeight && optimalSettings.maxWidth >= optimalSettings.minWidth && optimalSettings.maxWidth != 0 && optimalSettings.maxHeight != 0 && optimalSettings.minWidth != 0)
			{
				return optimalSettings.minHeight != 0;
			}
			return false;
		}

		private bool HasCameraStateExpired(CameraState cameraState)
		{
			return m_FrameId - cameraState.LastFrameId >= sMaximumFrameExpiration;
		}

		private void ProcessInvalidCameras()
		{
			foreach (KeyValuePair<int, CameraState> cameraState in m_CameraStates)
			{
				if (!cameraState.Value.IsAlive() || HasCameraStateExpired(cameraState.Value))
				{
					m_InvalidCameraKeys.Add(cameraState.Key);
				}
			}
		}

		private void CleanupCameraStates()
		{
			if (m_InvalidCameraKeys.Count == 0)
			{
				return;
			}
			m_CommandBuffer.Clear();
			foreach (int invalidCameraKey in m_InvalidCameraKeys)
			{
				if (m_CameraStates.TryGetValue(invalidCameraKey, out var value))
				{
					value.Cleanup(m_CommandBuffer);
					m_CameraStates.Remove(invalidCameraKey);
					GenericPool<CameraState>.Release(value);
				}
			}
			Graphics.ExecuteCommandBuffer(m_CommandBuffer);
			m_InvalidCameraKeys.Clear();
		}

		private void InternalNVIDIASetupDRSScaling(bool enableAutomaticSettings, Camera camera, XRPass xrPass, ref GlobalDynamicResolutionSettings dynamicResolutionSettings)
		{
			if (m_Device == null)
			{
				return;
			}
			int instanceID = camera.GetInstanceID();
			CameraState value = null;
			if (!m_CameraStates.TryGetValue(instanceID, out value) || value.ViewStates == null || value.ViewStates.Count == 0 || value.ViewStates[0].DLSSContext == null)
			{
				return;
			}
			DLSSQuality quality = value.ViewStates[0].DLSSContext.initData.quality;
			Rect viewport = ((xrPass != null && xrPass.enabled) ? xrPass.GetViewport() : new Rect(camera.pixelRect.x, camera.pixelRect.y, camera.pixelWidth, camera.pixelHeight));
			OptimalDLSSSettingsData optimalSettings = default(OptimalDLSSSettingsData);
			m_Device.GetOptimalSettings((uint)viewport.width, (uint)viewport.height, quality, out optimalSettings);
			foreach (ViewState viewState in value.ViewStates)
			{
				viewState?.RequestUseAutomaticSettings(enableAutomaticSettings, quality, viewport, in optimalSettings);
			}
			if (enableAutomaticSettings)
			{
				if (IsOptimalSettingsValid(in optimalSettings) && enableAutomaticSettings)
				{
					dynamicResolutionSettings.maxPercentage = Mathf.Min((float)optimalSettings.maxWidth / viewport.width, (float)optimalSettings.maxHeight / viewport.height) * 100f;
					dynamicResolutionSettings.minPercentage = Mathf.Max((float)optimalSettings.minWidth / viewport.width, (float)optimalSettings.minHeight / viewport.height) * 100f;
					DynamicResolutionHandler.SetSystemDynamicResScaler(value.ScaleDelegate, DynamicResScalePolicyType.ReturnsPercentage);
					DynamicResolutionHandler.SetActiveDynamicScalerSlot(DynamicResScalerSlot.System);
				}
			}
			else
			{
				value.ClearAutomaticSettings();
			}
		}

		private void InternalNVIDIABeginFrame(HDCamera hdCamera)
		{
			if (m_Device != null)
			{
				ProcessInvalidCameras();
				int instanceID = hdCamera.camera.GetInstanceID();
				CameraState value = null;
				m_CameraStates.TryGetValue(instanceID, out value);
				bool flag = hdCamera.IsDLSSEnabled();
				if (value == null && flag)
				{
					value = GenericPool<CameraState>.Get();
					value.Init(m_Device, hdCamera.camera);
					m_CameraStates.Add(instanceID, value);
				}
				else if (value != null && !flag)
				{
					m_InvalidCameraKeys.Add(instanceID);
				}
				if (value != null)
				{
					value.LastFrameId = m_FrameId;
				}
				CleanupCameraStates();
				m_FrameId++;
			}
		}

		private void InternalNVIDIARender(in Parameters parameters, CameraResources resources, CommandBuffer cmdBuffer)
		{
			if (m_Device != null && m_CameraStates.Count != 0 && m_CameraStates.TryGetValue(parameters.hdCamera.camera.GetInstanceID(), out var value))
			{
				DlssViewData viewData = default(DlssViewData);
				viewData.perfQuality = (DLSSQuality)(parameters.hdCamera.deepLearningSuperSamplingUseCustomQualitySettings ? parameters.hdCamera.deepLearningSuperSamplingQuality : parameters.drsSettings.DLSSPerfQualitySetting);
				viewData.sharpness = (parameters.hdCamera.deepLearningSuperSamplingUseCustomAttributes ? parameters.hdCamera.deepLearningSuperSamplingSharpening : parameters.drsSettings.DLSSSharpness);
				viewData.inputRes = new Resolution
				{
					width = (uint)parameters.hdCamera.actualWidth,
					height = (uint)parameters.hdCamera.actualHeight
				};
				viewData.outputRes = new Resolution
				{
					width = (uint)DynamicResolutionHandler.instance.finalViewport.x,
					height = (uint)DynamicResolutionHandler.instance.finalViewport.y
				};
				viewData.jitterX = 0f - parameters.hdCamera.taaJitter.x;
				viewData.jitterY = 0f - parameters.hdCamera.taaJitter.y;
				viewData.reset = parameters.resetHistory;
				value.SubmitCommands(parameters.hdCamera, parameters.preExposure, in viewData, in resources, cmdBuffer);
			}
		}
	}
}
