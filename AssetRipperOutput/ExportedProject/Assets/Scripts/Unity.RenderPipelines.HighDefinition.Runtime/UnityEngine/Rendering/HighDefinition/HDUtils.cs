using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	public class HDUtils
	{
		internal struct PackedMipChainInfo
		{
			public Vector2Int textureSize;

			public int mipLevelCount;

			public Vector2Int[] mipLevelSizes;

			public Vector2Int[] mipLevelOffsets;

			private Vector2 cachedTextureScale;

			private Vector2Int cachedHardwareTextureSize;

			private bool m_OffsetBufferWillNeedUpdate;

			public void Allocate()
			{
				mipLevelOffsets = new Vector2Int[15];
				mipLevelSizes = new Vector2Int[15];
				m_OffsetBufferWillNeedUpdate = true;
			}

			public void ComputePackedMipChainInfo(Vector2Int viewportSize)
			{
				bool num = DynamicResolutionHandler.instance.HardwareDynamicResIsEnabled();
				Vector2Int vector2Int = (num ? DynamicResolutionHandler.instance.ApplyScalesOnSize(viewportSize) : viewportSize);
				Vector2 vector = (num ? new Vector2((float)viewportSize.x / (float)vector2Int.x, (float)viewportSize.y / (float)vector2Int.y) : new Vector2(1f, 1f));
				if (cachedHardwareTextureSize == vector2Int && cachedTextureScale == vector)
				{
					return;
				}
				cachedHardwareTextureSize = vector2Int;
				cachedTextureScale = vector;
				mipLevelSizes[0] = vector2Int;
				mipLevelOffsets[0] = Vector2Int.zero;
				int num2 = 0;
				Vector2Int vector2Int2 = vector2Int;
				do
				{
					num2++;
					vector2Int2.x = Math.Max(1, vector2Int2.x + 1 >> 1);
					vector2Int2.y = Math.Max(1, vector2Int2.y + 1 >> 1);
					mipLevelSizes[num2] = vector2Int2;
					Vector2Int vector2Int3 = mipLevelOffsets[num2 - 1];
					Vector2Int vector2Int4 = vector2Int3 + mipLevelSizes[num2 - 1];
					Vector2Int vector2Int5 = default(Vector2Int);
					if (((uint)num2 & (true ? 1u : 0u)) != 0)
					{
						vector2Int5.x = vector2Int3.x;
						vector2Int5.y = vector2Int4.y;
					}
					else
					{
						vector2Int5.x = vector2Int4.x;
						vector2Int5.y = vector2Int3.y;
					}
					mipLevelOffsets[num2] = vector2Int5;
					vector2Int.x = Math.Max(vector2Int.x, vector2Int5.x + vector2Int2.x);
					vector2Int.y = Math.Max(vector2Int.y, vector2Int5.y + vector2Int2.y);
				}
				while (vector2Int2.x > 1 || vector2Int2.y > 1);
				textureSize = new Vector2Int((int)Mathf.Ceil((float)vector2Int.x * vector.x), (int)Mathf.Ceil((float)vector2Int.y * vector.y));
				mipLevelCount = num2 + 1;
				m_OffsetBufferWillNeedUpdate = true;
			}

			public ComputeBuffer GetOffsetBufferData(ComputeBuffer mipLevelOffsetsBuffer)
			{
				if (m_OffsetBufferWillNeedUpdate)
				{
					mipLevelOffsetsBuffer.SetData(mipLevelOffsets);
					m_OffsetBufferWillNeedUpdate = false;
				}
				return mipLevelOffsetsBuffer;
			}
		}

		internal const SortingCriteria k_OpaqueSortingCriteria = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder;

		private static List<CustomPassVolume> m_TempCustomPassVolumeList = new List<CustomPassVolume>();

		private static Texture3D m_ClearTexture3D;

		private static RTHandle m_ClearTexture3DRTH;

		private static Dictionary<GraphicsFormat, int> graphicsFormatSizeCache = new Dictionary<GraphicsFormat, int>
		{
			{
				GraphicsFormat.R8G8B8A8_UNorm,
				4
			},
			{
				GraphicsFormat.R16G16B16A16_SFloat,
				8
			},
			{
				GraphicsFormat.RGB_BC6H_SFloat,
				1
			}
		};

		internal static HDAdditionalReflectionData s_DefaultHDAdditionalReflectionData => ComponentSingleton<HDAdditionalReflectionData>.instance;

		internal static HDAdditionalLightData s_DefaultHDAdditionalLightData => ComponentSingleton<HDAdditionalLightData>.instance;

		internal static HDAdditionalCameraData s_DefaultHDAdditionalCameraData => ComponentSingleton<HDAdditionalCameraData>.instance;

		public static Texture3D clearTexture3D
		{
			get
			{
				if (m_ClearTexture3D == null)
				{
					m_ClearTexture3D = new Texture3D(1, 1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None)
					{
						name = "Transparent Texture 3D"
					};
					m_ClearTexture3D.SetPixel(0, 0, 0, Color.clear);
					m_ClearTexture3D.Apply();
					RTHandles.Release(m_ClearTexture3DRTH);
					m_ClearTexture3DRTH = null;
				}
				return m_ClearTexture3D;
			}
		}

		public static RTHandle clearTexture3DRTH
		{
			get
			{
				if (m_ClearTexture3DRTH == null || m_ClearTexture3D == null)
				{
					RTHandles.Release(m_ClearTexture3DRTH);
					m_ClearTexture3DRTH = RTHandles.Alloc(clearTexture3D);
				}
				return m_ClearTexture3DRTH;
			}
		}

		public static RenderPipelineSettings hdrpSettings => HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings;

		[Obsolete("Use GetRendererConfiguration() instead. #from(23.2).")]
		public static PerObjectData GetBakedLightingRenderConfig()
		{
			return PerObjectData.LightProbe | PerObjectData.LightProbeProxyVolume | PerObjectData.Lightmaps;
		}

		[Obsolete("Use GetRendererConfiguration() instead. #from(23.2).")]
		public static PerObjectData GetBakedLightingWithShadowMaskRenderConfig()
		{
			return GetBakedLightingRenderConfig() | PerObjectData.OcclusionProbe | PerObjectData.OcclusionProbeProxyVolume | PerObjectData.ShadowMask;
		}

		public static PerObjectData GetRendererConfiguration(bool apv, bool shadowMask)
		{
			PerObjectData perObjectData = PerObjectData.Lightmaps;
			if (!apv)
			{
				perObjectData |= PerObjectData.LightProbe | PerObjectData.LightProbeProxyVolume;
			}
			if (shadowMask)
			{
				perObjectData |= PerObjectData.OcclusionProbe | PerObjectData.OcclusionProbeProxyVolume | PerObjectData.ShadowMask;
			}
			return perObjectData;
		}

		public static Material GetBlitMaterial(TextureDimension dimension, bool singleSlice = false)
		{
			return Blitter.GetBlitMaterial(dimension, singleSlice);
		}

		internal static List<RenderPipelineMaterial> GetRenderPipelineMaterialList()
		{
			Type baseType = typeof(RenderPipelineMaterial);
			Assembly assembly = baseType.Assembly;
			try
			{
				return (from t in assembly.GetTypes()
					where t.IsSubclassOf(baseType)
					select t).Select(Activator.CreateInstance).Cast<RenderPipelineMaterial>().ToList();
			}
			catch (ReflectionTypeLoadException ex)
			{
				Exception[] loaderExceptions = ex.LoaderExceptions;
				for (int i = 0; i < loaderExceptions.Length; i++)
				{
					TypeLoadException ex2 = (TypeLoadException)loaderExceptions[i];
					Debug.LogError("Encountered an exception while attempting to reflect the HDRP assembly to extract all RenderPipelineMaterial types.\nThis exception must be fixed in order to fully initialize HDRP correctly.\n" + ex2.Message + "\n" + ex2.TypeName);
				}
				return null;
			}
		}

		internal static int GetRuntimeDebugPanelWidth(HDCamera hdCamera)
		{
			int val = (DebugManager.instance.displayRuntimeUI ? 610 : 0);
			return Math.Min(hdCamera.actualWidth, val);
		}

		internal static float ProjectionMatrixAspect(in Matrix4x4 matrix)
		{
			return (0f - matrix.m11) / matrix.m00;
		}

		internal static bool IsProjectionMatrixAsymmetric(in Matrix4x4 matrix)
		{
			if (matrix.m02 == 0f)
			{
				return matrix.m12 != 0f;
			}
			return true;
		}

		internal static Matrix4x4 ComputePixelCoordToWorldSpaceViewDirectionMatrix(float verticalFoV, Vector2 lensShift, Vector4 screenSize, Matrix4x4 worldToViewMatrix, bool renderToCubemap, float aspectRatio = -1f, bool isOrthographic = false)
		{
			Matrix4x4 matrix4x;
			if (isOrthographic)
			{
				matrix4x = new Matrix4x4(new Vector4(-2f * screenSize.z, 0f, 0f, 0f), new Vector4(0f, -2f * screenSize.w, 0f, 0f), new Vector4(1f, 1f, -1f, 0f), new Vector4(0f, 0f, 0f, 0f));
			}
			else
			{
				aspectRatio = ((aspectRatio < 0f) ? (screenSize.x * screenSize.w) : aspectRatio);
				float num = Mathf.Tan(0.5f * verticalFoV);
				float num2 = (1f - 2f * lensShift.y) * num;
				float num3 = -2f * screenSize.w * num;
				float x = (1f - 2f * lensShift.x) * num * aspectRatio;
				float x2 = -2f * screenSize.z * num * aspectRatio;
				if (renderToCubemap)
				{
					num3 = 0f - num3;
					num2 = 0f - num2;
				}
				matrix4x = new Matrix4x4(new Vector4(x2, 0f, 0f, 0f), new Vector4(0f, num3, 0f, 0f), new Vector4(x, num2, -1f, 0f), new Vector4(0f, 0f, 0f, 1f));
			}
			Vector4 column = new Vector4(0f, 0f, 0f, 1f);
			worldToViewMatrix.SetColumn(3, column);
			worldToViewMatrix.SetRow(2, -worldToViewMatrix.GetRow(2));
			return Matrix4x4.Transpose(worldToViewMatrix.transpose * matrix4x);
		}

		internal static float ComputZPlaneTexelSpacing(float planeDepth, float verticalFoV, float resolutionY)
		{
			return Mathf.Tan(0.5f * verticalFoV) * (2f / resolutionY) * planeDepth;
		}

		public static void BlitQuad(CommandBuffer cmd, Texture source, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear)
		{
			Blitter.BlitQuad(cmd, source, scaleBiasTex, scaleBiasRT, mipLevelTex, bilinear);
		}

		public static void BlitQuadWithPadding(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.BlitQuadWithPadding(cmd, source, textureSize, scaleBiasTex, scaleBiasRT, mipLevelTex, bilinear, paddingInPixels);
		}

		public static void BlitQuadWithPaddingMultiply(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.BlitQuadWithPaddingMultiply(cmd, source, textureSize, scaleBiasTex, scaleBiasRT, mipLevelTex, bilinear, paddingInPixels);
		}

		public static void BlitOctahedralWithPadding(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.BlitOctahedralWithPadding(cmd, source, textureSize, scaleBiasTex, scaleBiasRT, mipLevelTex, bilinear, paddingInPixels);
		}

		public static void BlitOctahedralWithPaddingMultiply(CommandBuffer cmd, Texture source, Vector2 textureSize, Vector4 scaleBiasTex, Vector4 scaleBiasRT, int mipLevelTex, bool bilinear, int paddingInPixels)
		{
			Blitter.BlitOctahedralWithPaddingMultiply(cmd, source, textureSize, scaleBiasTex, scaleBiasRT, mipLevelTex, bilinear, paddingInPixels);
		}

		public static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.BlitTexture(cmd, source, scaleBias, mipLevel, bilinear);
		}

		public static void BlitTexture2D(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, float mipLevel, bool bilinear)
		{
			Blitter.BlitTexture2D(cmd, source, scaleBias, mipLevel, bilinear);
		}

		internal static void BlitColorAndDepth(CommandBuffer cmd, Texture sourceColor, RenderTexture sourceDepth, Vector4 scaleBias, float mipLevel, bool blitDepth)
		{
			Blitter.BlitColorAndDepth(cmd, sourceColor, sourceDepth, scaleBias, mipLevel, blitDepth);
		}

		private static void BlitTexture(CommandBuffer cmd, RTHandle source, Vector4 scaleBias, Material material, int pass)
		{
			Blitter.BlitTexture(cmd, source, scaleBias, material, pass);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, float mipLevel = 0f, bool bilinear = false)
		{
			Blitter.BlitCameraTexture(cmd, source, destination, mipLevel, bilinear);
		}

		public static void BlitCameraTexture2D(CommandBuffer cmd, RTHandle source, RTHandle destination, float mipLevel = 0f, bool bilinear = false)
		{
			Blitter.BlitCameraTexture2D(cmd, source, destination, mipLevel, bilinear);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Material material, int pass)
		{
			Blitter.BlitCameraTexture(cmd, source, destination, material, pass);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Vector4 scaleBias, float mipLevel = 0f, bool bilinear = false)
		{
			Blitter.BlitCameraTexture(cmd, source, destination, scaleBias, mipLevel, bilinear);
		}

		public static void BlitCameraTexture(CommandBuffer cmd, RTHandle source, RTHandle destination, Rect destViewport, float mipLevel = 0f, bool bilinear = false)
		{
			Blitter.BlitCameraTexture(cmd, source, destination, destViewport, mipLevel, bilinear);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RTHandle colorBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.SetRenderTarget(commandBuffer, colorBuffer);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RTHandle colorBuffer, RTHandle depthStencilBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.SetRenderTarget(commandBuffer, colorBuffer, depthStencilBuffer);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Material material, RenderTargetIdentifier[] colorBuffers, RTHandle depthStencilBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.SetRenderTarget(commandBuffer, colorBuffers, depthStencilBuffer);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Rect viewport, Material material, RenderTargetIdentifier destination, CubemapFace cubemapFace, MaterialPropertyBlock properties = null, int shaderPassId = 0, int depthSlice = -1)
		{
			CoreUtils.SetRenderTarget(commandBuffer, destination, ClearFlag.None, 0, cubemapFace, depthSlice);
			commandBuffer.SetViewport(viewport);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Rect viewport, Material material, RenderTargetIdentifier destination, MaterialPropertyBlock properties = null, int shaderPassId = 0, int depthSlice = -1)
		{
			DrawFullScreen(commandBuffer, viewport, material, destination, CubemapFace.Unknown, properties, shaderPassId, depthSlice);
		}

		public static void DrawFullScreen(CommandBuffer commandBuffer, Rect viewport, Material material, RenderTargetIdentifier destination, RTHandle depthStencilBuffer, MaterialPropertyBlock properties = null, int shaderPassId = 0)
		{
			CoreUtils.SetRenderTarget(commandBuffer, destination, depthStencilBuffer, ClearFlag.None);
			commandBuffer.SetViewport(viewport);
			commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
		}

		internal static Vector4 GetMouseCoordinates(HDCamera camera)
		{
			Vector2 mousePosition = MousePositionDebug.instance.GetMousePosition(camera.screenSize.y, camera.camera.cameraType == CameraType.SceneView);
			return new Vector4(mousePosition.x, mousePosition.y, RTHandles.rtHandleProperties.rtHandleScale.x * mousePosition.x / camera.screenSize.x, RTHandles.rtHandleProperties.rtHandleScale.y * mousePosition.y / camera.screenSize.y);
		}

		internal static Vector4 GetMouseClickCoordinates(HDCamera camera)
		{
			Vector2 mouseClickPosition = MousePositionDebug.instance.GetMouseClickPosition(camera.screenSize.y);
			return new Vector4(mouseClickPosition.x, mouseClickPosition.y, RTHandles.rtHandleProperties.rtHandleScale.x * mouseClickPosition.x / camera.screenSize.x, RTHandles.rtHandleProperties.rtHandleScale.y * mouseClickPosition.y / camera.screenSize.y);
		}

		internal static bool IsRegularPreviewCamera(Camera camera)
		{
			if (camera.cameraType == CameraType.Preview)
			{
				camera.TryGetComponent<HDAdditionalCameraData>(out var component);
				if (!(component == null))
				{
					return !component.isEditorCameraPreview;
				}
				return true;
			}
			return false;
		}

		internal static string GetHDRenderPipelinePath()
		{
			return "Packages/com.unity.render-pipelines.high-definition/";
		}

		internal static string GetCorePath()
		{
			return "Packages/com.unity.render-pipelines.core/";
		}

		internal static string GetVFXPath()
		{
			return "Packages/com.unity.visualeffectgraph/";
		}

		internal static RenderPipelineAsset SwitchToBuiltinRenderPipeline(out bool assetWasFromQuality)
		{
			RenderPipelineAsset renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
			assetWasFromQuality = false;
			if (renderPipelineAsset != null && GraphicsSettings.currentRenderPipeline == renderPipelineAsset)
			{
				GraphicsSettings.renderPipelineAsset = null;
				return renderPipelineAsset;
			}
			RenderPipelineAsset renderPipeline = QualitySettings.renderPipeline;
			QualitySettings.renderPipeline = null;
			assetWasFromQuality = true;
			return renderPipeline;
		}

		internal static void RestoreRenderPipelineAsset(bool wasUnsetFromQuality, RenderPipelineAsset renderPipelineAsset)
		{
			if (wasUnsetFromQuality)
			{
				QualitySettings.renderPipeline = renderPipelineAsset;
			}
			else
			{
				GraphicsSettings.renderPipelineAsset = renderPipelineAsset;
			}
		}

		internal static int DivRoundUp(int x, int y)
		{
			return (x + y - 1) / y;
		}

		internal static bool IsQuaternionValid(Quaternion q)
		{
			return q[0] * q[0] + q[1] * q[1] + q[2] * q[2] + q[3] * q[3] > float.Epsilon;
		}

		internal static void CheckRTCreated(RenderTexture rt)
		{
			if (!rt.IsCreated())
			{
				rt.Create();
			}
		}

		internal static float ComputeViewportScale(int viewportSize, int bufferSize)
		{
			float num = 1f / (float)bufferSize;
			return (float)viewportSize * num;
		}

		internal static float ComputeViewportLimit(int viewportSize, int bufferSize)
		{
			float num = 1f / (float)bufferSize;
			return ((float)viewportSize - 0.5f) * num;
		}

		internal static Vector4 ComputeViewportScaleAndLimit(Vector2Int viewportSize, Vector2Int bufferSize)
		{
			return new Vector4(ComputeViewportScale(viewportSize.x, bufferSize.x), ComputeViewportScale(viewportSize.y, bufferSize.y), ComputeViewportLimit(viewportSize.x, bufferSize.x), ComputeViewportLimit(viewportSize.y, bufferSize.y));
		}

		internal static bool IsSupportedGraphicDevice(GraphicsDeviceType graphicDevice)
		{
			if (graphicDevice != GraphicsDeviceType.Direct3D11 && graphicDevice != GraphicsDeviceType.Direct3D12 && graphicDevice != GraphicsDeviceType.PlayStation4 && graphicDevice != GraphicsDeviceType.PlayStation5 && graphicDevice != GraphicsDeviceType.PlayStation5NGGC && graphicDevice != GraphicsDeviceType.XboxOne && graphicDevice != GraphicsDeviceType.XboxOneD3D12 && graphicDevice != GraphicsDeviceType.GameCoreXboxOne && graphicDevice != GraphicsDeviceType.GameCoreXboxSeries && graphicDevice != GraphicsDeviceType.Metal)
			{
				return graphicDevice == GraphicsDeviceType.Vulkan;
			}
			return true;
		}

		internal static bool IsMacOSVersionAtLeast(string os, int majorVersion, int minorVersion, int patchVersion)
		{
			int num = os.LastIndexOf(" ");
			string[] array = os.Substring(num + 1).Split('.');
			int num2 = Convert.ToInt32(array[0]);
			int num3 = Convert.ToInt32(array[1]);
			int num4 = Convert.ToInt32(array[2]);
			if (num2 < majorVersion)
			{
				return false;
			}
			if (num2 > majorVersion)
			{
				return true;
			}
			if (num3 < minorVersion)
			{
				return false;
			}
			if (num3 > minorVersion)
			{
				return true;
			}
			if (num4 < patchVersion)
			{
				return false;
			}
			return true;
		}

		internal static bool IsOperatingSystemSupported(string os)
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && os.StartsWith("Mac") && !IsMacOSVersionAtLeast(os, 10, 13, 0))
			{
				return false;
			}
			return true;
		}

		internal static void GetScaleAndBiasForLinearDistanceFade(float fadeDistance, out float scale, out float bias)
		{
			float num = 0.9f * fadeDistance;
			scale = 1f / (fadeDistance - num);
			bias = (0f - num) / (fadeDistance - num);
		}

		internal static float ComputeLinearDistanceFade(float distanceToCamera, float fadeDistance)
		{
			GetScaleAndBiasForLinearDistanceFade(fadeDistance, out var scale, out var bias);
			return 1f - Mathf.Clamp01(distanceToCamera * scale + bias);
		}

		internal static float ComputeWeightedLinearFadeDistance(Vector3 position1, Vector3 position2, float weight, float fadeDistance)
		{
			return ComputeLinearDistanceFade(Vector3.Magnitude(position1 - position2), fadeDistance) * weight;
		}

		internal static bool WillCustomPassBeExecuted(HDCamera hdCamera, CustomPassInjectionPoint injectionPoint)
		{
			if (!hdCamera.frameSettings.IsEnabled(FrameSettingsField.CustomPass))
			{
				return false;
			}
			bool flag = false;
			CustomPassVolume.GetActivePassVolumes(injectionPoint, m_TempCustomPassVolumeList);
			foreach (CustomPassVolume tempCustomPassVolume in m_TempCustomPassVolumeList)
			{
				if (tempCustomPassVolume == null)
				{
					return false;
				}
				flag |= tempCustomPassVolume.WillExecuteInjectionPoint(hdCamera);
			}
			return flag;
		}

		internal static bool PostProcessIsFinalPass(HDCamera hdCamera, AOVRequestData aovRequest)
		{
			if (!aovRequest.isValid && !Debug.isDebugBuild && !WillCustomPassBeExecuted(hdCamera, CustomPassInjectionPoint.AfterPostProcess))
			{
				return !hdCamera.hasCaptureActions;
			}
			return false;
		}

		internal unsafe static Vector4 ConvertGUIDToVector4(string guid)
		{
			byte[] array = new byte[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = byte.Parse(guid.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			Vector4 result;
			fixed (byte* ptr = array)
			{
				result = *(Vector4*)ptr;
			}
			return result;
		}

		internal unsafe static string ConvertVector4ToGUID(Vector4 vector)
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte* ptr = (byte*)(&vector);
			for (int i = 0; i < 16; i++)
			{
				stringBuilder.Append(ptr[i].ToString("x2"));
			}
			byte[] destination = new byte[16];
			Marshal.Copy((IntPtr)ptr, destination, 0, 16);
			return stringBuilder.ToString();
		}

		public static Color NormalizeColor(Color color)
		{
			Vector4 vector = Vector4.Max(color, Vector4.one * 0.0001f);
			Color color2 = vector;
			color = vector / ColorUtils.Luminance(in color2);
			color.a = 1f;
			return color;
		}

		[Obsolete("Please use CoreUtils.DrawRendererList instead.")]
		public static void DrawRendererList(ScriptableRenderContext renderContext, CommandBuffer cmd, RendererList rendererList)
		{
			CoreUtils.DrawRendererList(renderContext, cmd, rendererList);
		}

		internal unsafe static string ComputeProbeCameraName(string probeName, int face, string viewerName)
		{
			probeName = probeName ?? string.Empty;
			viewerName = viewerName ?? "null";
			int num = Mathf.Min(probeName.Length, 40);
			int num2 = Mathf.Min(viewerName.Length, 40);
			int num3 = "HDProbe RenderCamera (".Length + num + ": ".Length + 2 + " for viewer '".Length + num2 + "')".Length;
			char* ptr = stackalloc char[num3];
			char* ptr2 = ptr;
			int num4 = 0;
			int num5 = 0;
			while (num5 < "HDProbe RenderCamera (".Length)
			{
				*ptr2 = "HDProbe RenderCamera ("[num5];
				num5++;
				ptr2++;
			}
			num5 = 0;
			int num6 = Mathf.Min(probeName.Length, 40);
			while (num5 < num6)
			{
				*ptr2 = probeName[num5];
				num5++;
				ptr2++;
			}
			num4 += num6;
			num5 = 0;
			while (num5 < ": ".Length)
			{
				*ptr2 = ": "[num5];
				num5++;
				ptr2++;
			}
			int num7 = face * 205 >> 11;
			*(ptr2++) = (char)(num7 + 48);
			*(ptr2++) = (char)(face - num7 * 10 + 48);
			num4 += 2;
			num5 = 0;
			while (num5 < " for viewer '".Length)
			{
				*ptr2 = " for viewer '"[num5];
				num5++;
				ptr2++;
			}
			num5 = 0;
			num6 = Mathf.Min(viewerName.Length, 40);
			while (num5 < num6)
			{
				*ptr2 = viewerName[num5];
				num5++;
				ptr2++;
			}
			num4 += num6;
			num5 = 0;
			while (num5 < "')".Length)
			{
				*ptr2 = "')"[num5];
				num5++;
				ptr2++;
			}
			num4 += "HDProbe RenderCamera (".Length + ": ".Length + " for viewer '".Length + "')".Length;
			return new string(ptr, 0, num4);
		}

		internal unsafe static string ComputeCameraName(string cameraName)
		{
			int num = Mathf.Min(cameraName.Length, 40);
			int num2 = "HDRenderPipeline::Render ".Length + num;
			char* ptr = stackalloc char[num2];
			char* ptr2 = ptr;
			int num3 = 0;
			int num4 = 0;
			while (num4 < "HDRenderPipeline::Render ".Length)
			{
				*ptr2 = "HDRenderPipeline::Render "[num4];
				num4++;
				ptr2++;
			}
			num4 = 0;
			int num5 = num;
			while (num4 < num5)
			{
				*ptr2 = cameraName[num4];
				num4++;
				ptr2++;
			}
			num3 += num5;
			num3 += "HDRenderPipeline::Render ".Length;
			return new string(ptr, 0, num3);
		}

		internal static float ClampFOV(float fov)
		{
			return Mathf.Clamp(fov, 1E-05f, 179f);
		}

		internal static ulong GetSceneCullingMaskFromCamera(Camera camera)
		{
			return 0uL;
		}

		internal static HDAdditionalCameraData TryGetAdditionalCameraDataOrDefault(Camera camera)
		{
			if (camera == null || camera.Equals(null))
			{
				return s_DefaultHDAdditionalCameraData;
			}
			if (camera.TryGetComponent<HDAdditionalCameraData>(out var component))
			{
				return component;
			}
			return s_DefaultHDAdditionalCameraData;
		}

		internal static int GetFormatSizeInBytes(GraphicsFormat format)
		{
			if (graphicsFormatSizeCache.TryGetValue(format, out var value))
			{
				return value;
			}
			string text = format.ToString();
			int num = text.IndexOf('_');
			text = text.Substring(0, (num == -1) ? text.Length : num);
			int num2 = 0;
			foreach (Match item in Regex.Matches(text, "\\d+"))
			{
				num2 += int.Parse(item.Value);
			}
			value = num2 / 8;
			graphicsFormatSizeCache[format] = value;
			return value;
		}

		internal static void DisplayMessageNotification(string msg)
		{
			Debug.LogError(msg);
		}

		internal static string GetUnsupportedAPIMessage(string graphicAPI)
		{
			string operatingSystem = SystemInfo.operatingSystem;
			OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
			bool flag = true;
			string text = null;
			switch (operatingSystemFamily)
			{
			case OperatingSystemFamily.MacOSX:
				text = "Mac";
				break;
			case OperatingSystemFamily.Windows:
				text = "Windows";
				break;
			case OperatingSystemFamily.Linux:
				text = "Linux";
				break;
			}
			string text2 = ((!flag) ? ("Platform " + operatingSystem + " is not supported with HDRP") : ("Platform " + operatingSystem + " with graphics API " + graphicAPI + " is not supported with HDRP"));
			if (graphicAPI.StartsWith("OpenGL"))
			{
				if (SystemInfo.operatingSystem.StartsWith("Mac"))
				{
					text2 += ", use the Metal graphics API instead";
				}
				else if (SystemInfo.operatingSystem.StartsWith("Windows"))
				{
					text2 += ", use the Vulkan graphics API instead";
				}
			}
			text2 += ".\nChange the platform/device to a compatible one or remove incompatible graphics APIs.\n";
			if (text != null)
			{
				text2 = text2 + "To do this, go to Project Settings > Player > Other Settings and modify the Graphics APIs for " + text + " list.";
			}
			return text2;
		}

		internal static int GetTextureHash(Texture texture)
		{
			return CoreUtils.GetTextureHash(texture);
		}

		internal static void ReleaseComponentSingletons()
		{
			ComponentSingleton<HDAdditionalReflectionData>.Release();
			ComponentSingleton<HDAdditionalLightData>.Release();
			ComponentSingleton<HDAdditionalCameraData>.Release();
		}

		internal static float InterpolateOrientation(float fromValue, float toValue, float t)
		{
			float num = Mathf.Abs(toValue - fromValue);
			float num2 = 0f;
			if (fromValue < toValue)
			{
				if (360f - toValue + fromValue < num)
				{
					float num3 = toValue - 360f;
					num2 = fromValue + (num3 - fromValue) * t;
					if (num2 < 0f)
					{
						num2 += 360f;
					}
				}
				else
				{
					num2 = fromValue + (toValue - fromValue) * t;
				}
			}
			else if (360f - fromValue + toValue < num)
			{
				float num4 = toValue + 360f;
				num2 = fromValue + (num4 - fromValue) * t;
				if (num2 > 360f)
				{
					num2 -= 360f;
				}
			}
			else
			{
				num2 = fromValue + (toValue - fromValue) * t;
			}
			return num2;
		}

		internal static void ConvertHDRColorToLDR(Color hdr, out Color ldr, out float intensity)
		{
			hdr.a = 1f;
			ldr = hdr;
			intensity = 1f;
			float maxColorComponent = hdr.maxColorComponent;
			if (maxColorComponent != 0f)
			{
				float num = 191f / maxColorComponent;
				ldr.r = Mathf.Min(191f, num * hdr.r) / 255f;
				ldr.g = Mathf.Min(191f, num * hdr.g) / 255f;
				ldr.b = Mathf.Min(191f, num * hdr.b) / 255f;
				intensity = 255f / num;
			}
		}
	}
}
