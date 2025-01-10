using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct AOVRequestData
	{
		private class PushCameraTexturePassData
		{
			public TextureHandle source;

			public RTHandle target;
		}

		private class PushCustomPassTexturePassData
		{
			public TextureHandle source;

			public RTHandle customPassSource;

			public RTHandle target;
		}

		[Obsolete("Since 2019.3, use AOVRequestData.NewDefault() instead.")]
		public static readonly AOVRequestData @default = default(AOVRequestData);

		public static readonly AOVRequestData defaultAOVRequestDataNonAlloc = NewDefault();

		private AOVRequest m_Settings;

		private AOVBuffers[] m_RequestedAOVBuffers;

		private CustomPassAOVBuffers[] m_CustomPassAOVBuffers;

		private FramePassCallback m_Callback;

		private FramePassCallbackEx m_CallbackEx;

		private readonly AOVRequestBufferAllocator m_BufferAllocator;

		private readonly AOVRequestCustomPassBufferAllocator m_CustomPassBufferAllocator;

		private List<GameObject> m_LightFilter;

		public bool isValid
		{
			get
			{
				if (m_RequestedAOVBuffers != null || m_CustomPassAOVBuffers != null)
				{
					if (m_Callback == null)
					{
						return m_CallbackEx != null;
					}
					return true;
				}
				return false;
			}
		}

		public bool overrideRenderFormat => m_Settings.overrideRenderFormat;

		internal bool hasLightFilter => m_LightFilter != null;

		public static AOVRequestData NewDefault()
		{
			AOVRequestData result = default(AOVRequestData);
			result.m_Settings = AOVRequest.NewDefault();
			result.m_RequestedAOVBuffers = new AOVBuffers[0];
			result.m_Callback = null;
			return result;
		}

		public AOVRequestData(AOVRequest settings, AOVRequestBufferAllocator bufferAllocator, List<GameObject> lightFilter, AOVBuffers[] requestedAOVBuffers, FramePassCallback callback)
		{
			m_Settings = settings;
			m_BufferAllocator = bufferAllocator;
			m_RequestedAOVBuffers = requestedAOVBuffers;
			m_LightFilter = lightFilter;
			m_Callback = callback;
			m_CallbackEx = null;
			m_CustomPassAOVBuffers = null;
			m_CustomPassBufferAllocator = null;
		}

		public AOVRequestData(AOVRequest settings, AOVRequestBufferAllocator bufferAllocator, List<GameObject> lightFilter, AOVBuffers[] requestedAOVBuffers, CustomPassAOVBuffers[] customPassAOVBuffers, AOVRequestCustomPassBufferAllocator customPassBufferAllocator, FramePassCallbackEx callback)
		{
			m_Settings = settings;
			m_BufferAllocator = bufferAllocator;
			m_RequestedAOVBuffers = requestedAOVBuffers;
			m_CustomPassAOVBuffers = customPassAOVBuffers;
			m_CustomPassBufferAllocator = customPassBufferAllocator;
			m_LightFilter = lightFilter;
			m_Callback = null;
			m_CallbackEx = callback;
		}

		public void AllocateTargetTexturesIfRequired(ref List<RTHandle> textures)
		{
			if (!isValid || textures == null)
			{
				return;
			}
			textures.Clear();
			if (m_RequestedAOVBuffers != null)
			{
				AOVBuffers[] requestedAOVBuffers = m_RequestedAOVBuffers;
				foreach (AOVBuffers aovBufferId in requestedAOVBuffers)
				{
					textures.Add(m_BufferAllocator(aovBufferId));
				}
			}
		}

		public void AllocateTargetTexturesIfRequired(ref List<RTHandle> textures, ref List<RTHandle> customPassTextures)
		{
			if (!isValid || textures == null)
			{
				return;
			}
			textures.Clear();
			customPassTextures.Clear();
			if (m_RequestedAOVBuffers != null)
			{
				AOVBuffers[] requestedAOVBuffers = m_RequestedAOVBuffers;
				for (int i = 0; i < requestedAOVBuffers.Length; i++)
				{
					AOVBuffers aovBufferId = requestedAOVBuffers[i];
					RTHandle rTHandle = m_BufferAllocator(aovBufferId);
					textures.Add(rTHandle);
					if (rTHandle == null)
					{
						Debug.LogError("Allocation for requested AOVBuffers ID: " + aovBufferId.ToString() + " have fail. Please ensure the callback allocator do the correct allocation.");
					}
				}
			}
			if (m_CustomPassAOVBuffers == null)
			{
				return;
			}
			CustomPassAOVBuffers[] customPassAOVBuffers = m_CustomPassAOVBuffers;
			foreach (CustomPassAOVBuffers customPassAOVBuffers2 in customPassAOVBuffers)
			{
				RTHandle rTHandle2 = m_CustomPassBufferAllocator(customPassAOVBuffers2);
				customPassTextures.Add(rTHandle2);
				if (rTHandle2 == null)
				{
					Debug.LogError("Allocation for requested AOVBuffers ID: " + customPassAOVBuffers2.ToString() + " have fail. Please ensure the callback for custom pass allocator do the correct allocation.");
				}
			}
		}

		internal void OverrideBufferFormatForAOVs(ref GraphicsFormat format, List<RTHandle> aovBuffers)
		{
			if (m_RequestedAOVBuffers != null && aovBuffers.Count != 0)
			{
				int num = Array.IndexOf(m_RequestedAOVBuffers, AOVBuffers.Color);
				if (num < 0)
				{
					num = Array.IndexOf(m_RequestedAOVBuffers, AOVBuffers.Output);
				}
				if (num >= 0)
				{
					format = aovBuffers[num].rt.graphicsFormat;
				}
			}
		}

		internal void PushCameraTexture(RenderGraph renderGraph, AOVBuffers aovBufferId, HDCamera camera, TextureHandle source, List<RTHandle> targets)
		{
			if (!isValid || m_RequestedAOVBuffers == null)
			{
				return;
			}
			int num = Array.IndexOf(m_RequestedAOVBuffers, aovBufferId);
			if (num == -1)
			{
				return;
			}
			PushCameraTexturePassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<PushCameraTexturePassData>("Push AOV Camera Texture", out passData, ProfilingSampler.Get((HDProfileId)(215 + aovBufferId)));
			passData.source = renderGraphBuilder.ReadTexture(in source);
			passData.target = targets[num];
			renderGraphBuilder.SetRenderFunc(delegate(PushCameraTexturePassData data, RenderGraphContext ctx)
			{
				HDUtils.BlitCameraTexture(ctx.cmd, data.source, data.target);
			});
		}

		internal void PushCustomPassTexture(RenderGraph renderGraph, CustomPassInjectionPoint injectionPoint, TextureHandle cameraSource, Lazy<RTHandle> customPassSource, List<RTHandle> targets)
		{
			if (!isValid || m_CustomPassAOVBuffers == null)
			{
				return;
			}
			int num = -1;
			for (int i = 0; i < m_CustomPassAOVBuffers.Length; i++)
			{
				if (m_CustomPassAOVBuffers[i].injectionPoint == injectionPoint)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			PushCustomPassTexturePassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<PushCustomPassTexturePassData>("Push Custom Pass Texture", out passData);
			if (m_CustomPassAOVBuffers[num].outputType == CustomPassAOVBuffers.OutputType.Camera)
			{
				passData.source = renderGraphBuilder.ReadTexture(in cameraSource);
				passData.customPassSource = null;
			}
			else
			{
				passData.customPassSource = customPassSource.Value;
			}
			passData.target = targets[num];
			renderGraphBuilder.SetRenderFunc(delegate(PushCustomPassTexturePassData data, RenderGraphContext ctx)
			{
				if (data.customPassSource != null)
				{
					HDUtils.BlitCameraTexture(ctx.cmd, data.customPassSource, data.target);
				}
				else
				{
					HDUtils.BlitCameraTexture(ctx.cmd, data.source, data.target);
				}
			});
		}

		public void Execute(CommandBuffer cmd, List<RTHandle> framePassTextures, RenderOutputProperties outputProperties)
		{
			if (isValid)
			{
				m_Callback(cmd, framePassTextures, outputProperties);
			}
		}

		public void Execute(CommandBuffer cmd, List<RTHandle> framePassTextures, List<RTHandle> customPassTextures, RenderOutputProperties outputProperties)
		{
			if (isValid)
			{
				if (m_CallbackEx != null)
				{
					m_CallbackEx(cmd, framePassTextures, customPassTextures, outputProperties);
				}
				else
				{
					m_Callback(cmd, framePassTextures, outputProperties);
				}
			}
		}

		public void SetupDebugData(ref DebugDisplaySettings debugDisplaySettings)
		{
			if (isValid)
			{
				debugDisplaySettings = new DebugDisplaySettings();
				m_Settings.FillDebugData(debugDisplaySettings);
			}
		}

		public bool IsLightEnabled(GameObject gameObject)
		{
			if (m_LightFilter != null)
			{
				return m_LightFilter.Contains(gameObject);
			}
			return true;
		}

		internal int GetHash()
		{
			int num = m_Settings.GetHashCode();
			if (m_LightFilter != null)
			{
				foreach (GameObject item in m_LightFilter)
				{
					num += item.GetHashCode();
				}
			}
			return num;
		}

		internal bool HasSameSettings(AOVRequestData other)
		{
			if (m_Settings != other.m_Settings)
			{
				return false;
			}
			if (m_LightFilter != null)
			{
				return m_LightFilter.Equals(other.m_LightFilter);
			}
			return true;
		}
	}
}
