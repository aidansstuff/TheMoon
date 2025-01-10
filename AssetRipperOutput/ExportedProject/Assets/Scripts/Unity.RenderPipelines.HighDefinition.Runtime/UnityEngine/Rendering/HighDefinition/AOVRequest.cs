using System;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct AOVRequest
	{
		[Obsolete("Since 2019.3, use AOVRequest.NewDefault() instead.")]
		public static readonly AOVRequest @default;

		private MaterialSharedProperty m_MaterialProperty;

		private LightingProperty m_LightingProperty;

		private DebugLightFilterMode m_LightFilterProperty;

		private DebugFullScreen m_DebugFullScreen;

		internal bool m_OverrideRenderFormat;

		internal bool overrideRenderFormat => m_OverrideRenderFormat;

		private unsafe AOVRequest* thisPtr
		{
			get
			{
				fixed (AOVRequest* result = &this)
				{
					return result;
				}
			}
		}

		public static AOVRequest NewDefault()
		{
			AOVRequest result = default(AOVRequest);
			result.m_MaterialProperty = MaterialSharedProperty.None;
			result.m_LightingProperty = LightingProperty.None;
			result.m_DebugFullScreen = DebugFullScreen.None;
			result.m_LightFilterProperty = DebugLightFilterMode.None;
			result.m_OverrideRenderFormat = false;
			return result;
		}

		public AOVRequest(AOVRequest other)
		{
			m_MaterialProperty = other.m_MaterialProperty;
			m_LightingProperty = other.m_LightingProperty;
			m_DebugFullScreen = other.m_DebugFullScreen;
			m_LightFilterProperty = other.m_LightFilterProperty;
			m_OverrideRenderFormat = other.m_OverrideRenderFormat;
		}

		public unsafe ref AOVRequest SetFullscreenOutput(MaterialSharedProperty materialProperty)
		{
			m_MaterialProperty = materialProperty;
			return ref *thisPtr;
		}

		public unsafe ref AOVRequest SetFullscreenOutput(LightingProperty lightingProperty)
		{
			m_LightingProperty = lightingProperty;
			return ref *thisPtr;
		}

		public unsafe ref AOVRequest SetFullscreenOutput(DebugFullScreen debugFullScreen)
		{
			m_DebugFullScreen = debugFullScreen;
			return ref *thisPtr;
		}

		public unsafe ref AOVRequest SetLightFilter(DebugLightFilterMode filter)
		{
			m_LightFilterProperty = filter;
			return ref *thisPtr;
		}

		public unsafe ref AOVRequest SetOverrideRenderFormat(bool flag)
		{
			m_OverrideRenderFormat = flag;
			return ref *thisPtr;
		}

		public void FillDebugData(DebugDisplaySettings debug)
		{
			debug.SetDebugViewCommonMaterialProperty(m_MaterialProperty);
			switch (m_LightingProperty)
			{
			case LightingProperty.DiffuseOnly:
				debug.SetDebugLightingMode(DebugLightingMode.DiffuseLighting);
				break;
			case LightingProperty.SpecularOnly:
				debug.SetDebugLightingMode(DebugLightingMode.SpecularLighting);
				break;
			case LightingProperty.DirectDiffuseOnly:
				debug.SetDebugLightingMode(DebugLightingMode.DirectDiffuseLighting);
				break;
			case LightingProperty.DirectSpecularOnly:
				debug.SetDebugLightingMode(DebugLightingMode.DirectSpecularLighting);
				break;
			case LightingProperty.IndirectDiffuseOnly:
				debug.SetDebugLightingMode(DebugLightingMode.IndirectDiffuseLighting);
				break;
			case LightingProperty.ReflectionOnly:
				debug.SetDebugLightingMode(DebugLightingMode.ReflectionLighting);
				break;
			case LightingProperty.RefractionOnly:
				debug.SetDebugLightingMode(DebugLightingMode.RefractionLighting);
				break;
			case LightingProperty.EmissiveOnly:
				debug.SetDebugLightingMode(DebugLightingMode.EmissiveLighting);
				break;
			default:
				debug.SetDebugLightingMode(DebugLightingMode.None);
				break;
			}
			debug.SetDebugLightFilterMode(m_LightFilterProperty);
			switch (m_DebugFullScreen)
			{
			case DebugFullScreen.None:
				debug.SetFullScreenDebugMode(FullScreenDebugMode.None);
				break;
			case DebugFullScreen.Depth:
				debug.SetFullScreenDebugMode(FullScreenDebugMode.DepthPyramid);
				break;
			case DebugFullScreen.ScreenSpaceAmbientOcclusion:
				debug.SetFullScreenDebugMode(FullScreenDebugMode.ScreenSpaceAmbientOcclusion);
				break;
			case DebugFullScreen.MotionVectors:
				debug.SetFullScreenDebugMode(FullScreenDebugMode.MotionVectors);
				break;
			case DebugFullScreen.WorldSpacePosition:
				debug.SetFullScreenDebugMode(FullScreenDebugMode.WorldSpacePosition);
				break;
			default:
				throw new ArgumentException("Unknown DebugFullScreen");
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is AOVRequest)
			{
				return (AOVRequest)obj == this;
			}
			return false;
		}

		public static bool operator ==(AOVRequest a, AOVRequest b)
		{
			if (a.m_DebugFullScreen == b.m_DebugFullScreen && a.m_LightFilterProperty == b.m_LightFilterProperty && a.m_LightingProperty == b.m_LightingProperty && a.m_MaterialProperty == b.m_MaterialProperty)
			{
				return a.m_OverrideRenderFormat == b.m_OverrideRenderFormat;
			}
			return false;
		}

		public static bool operator !=(AOVRequest a, AOVRequest b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = (int)(num * 23 + m_DebugFullScreen);
			num = (int)(num * 23 + m_LightFilterProperty);
			num = (int)(num * 23 + m_LightingProperty);
			num = (int)(num * 23 + m_MaterialProperty);
			return m_OverrideRenderFormat ? (num * 23 + 1) : num;
		}
	}
}
