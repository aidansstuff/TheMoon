using System;
using Unity.Collections;
using UnityEngine.Experimental.GlobalIllumination;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class GlobalIlluminationUtils
	{
		public static Lightmapping.RequestLightsDelegate hdLightsDelegate = delegate(Light[] requests, NativeArray<LightDataGI> lightsOutput)
		{
			LightDataGI lightDataGI = default(LightDataGI);
			for (int i = 0; i < requests.Length; i++)
			{
				Light light = requests[i];
				if (LightmapperUtils.Extract(light.bakingOutput.lightmapBakeType) == LightMode.Realtime)
				{
					LightDataGIExtract(light, ref lightDataGI);
				}
				else
				{
					lightDataGI.InitNoBake(light.GetInstanceID());
				}
				lightsOutput[i] = lightDataGI;
			}
		};

		public static bool LightDataGIExtract(Light light, ref LightDataGI lightDataGI)
		{
			HDAdditionalLightData hDAdditionalLightData = light.GetComponent<HDAdditionalLightData>();
			if (hDAdditionalLightData == null)
			{
				hDAdditionalLightData = HDUtils.s_DefaultHDAdditionalLightData;
			}
			LightmapperUtils.Extract(light, out var cookie);
			lightDataGI.cookieID = cookie.instanceID;
			lightDataGI.cookieScale = cookie.scale;
			Color color = new Color(1f, 1f, 1f);
			if (hDAdditionalLightData.useColorTemperature)
			{
				color = Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature);
			}
			LightMode lightMode = LightmapperUtils.Extract(light.bakingOutput.lightmapBakeType);
			float num = 1f;
			if (lightMode == LightMode.Realtime || lightMode == LightMode.Mixed)
			{
				num = hDAdditionalLightData.lightDimmer;
			}
			lightDataGI.instanceID = light.GetInstanceID();
			LinearColor color2 = (hDAdditionalLightData.affectDiffuse ? LinearColor.Convert(light.color, light.intensity) : LinearColor.Black());
			color2.red *= color.r;
			color2.green *= color.g;
			color2.blue *= color.b;
			color2.intensity *= num;
			LinearColor indirectColor = (hDAdditionalLightData.affectDiffuse ? LightmapperUtils.ExtractIndirect(light) : LinearColor.Black());
			indirectColor.red *= color.r;
			indirectColor.green *= color.g;
			indirectColor.blue *= color.b;
			indirectColor.intensity *= num;
			lightDataGI.color = color2;
			lightDataGI.indirectColor = indirectColor;
			if (hDAdditionalLightData.interactsWithSky)
			{
				SkySettings skySettings = SkyManager.GetStaticLightingSky()?.skySettings;
				if (skySettings != null)
				{
					Vector3 vector = skySettings.EvaluateAtmosphericAttenuation(-light.transform.forward, Vector3.zero);
					lightDataGI.color.red = lightDataGI.color.red * vector.x;
					lightDataGI.color.green = lightDataGI.color.green * vector.y;
					lightDataGI.color.blue = lightDataGI.color.blue * vector.z;
					lightDataGI.indirectColor.red = lightDataGI.indirectColor.red * vector.x;
					lightDataGI.indirectColor.green = lightDataGI.indirectColor.green * vector.y;
					lightDataGI.indirectColor.blue = lightDataGI.indirectColor.blue * vector.z;
				}
			}
			lightDataGI.mode = LightmapperUtils.Extract(light.bakingOutput.lightmapBakeType);
			lightDataGI.shadow = (byte)((light.shadows != 0) ? 1u : 0u);
			HDLightType hDLightType = hDAdditionalLightData.ComputeLightType(light);
			if (hDLightType != HDLightType.Area)
			{
				lightDataGI.color.intensity /= MathF.PI;
				lightDataGI.indirectColor.intensity /= MathF.PI;
				color2.intensity /= MathF.PI;
				indirectColor.intensity /= MathF.PI;
			}
			switch (hDLightType)
			{
			case HDLightType.Directional:
				lightDataGI.orientation = light.transform.rotation;
				lightDataGI.position = light.transform.position;
				lightDataGI.range = 0f;
				lightDataGI.coneAngle = hDAdditionalLightData.shapeWidth;
				lightDataGI.innerConeAngle = hDAdditionalLightData.shapeHeight;
				lightDataGI.shape0 = 0f;
				lightDataGI.shape1 = 0f;
				lightDataGI.type = UnityEngine.Experimental.GlobalIllumination.LightType.Directional;
				lightDataGI.falloff = FalloffType.Undefined;
				lightDataGI.coneAngle = hDAdditionalLightData.shapeWidth;
				lightDataGI.innerConeAngle = hDAdditionalLightData.shapeHeight;
				break;
			case HDLightType.Spot:
				switch (hDAdditionalLightData.spotLightShape)
				{
				case SpotLightShape.Cone:
				{
					SpotLight light4 = default(SpotLight);
					light4.instanceID = light.GetInstanceID();
					light4.shadow = light.shadows != LightShadows.None;
					light4.mode = lightMode;
					light4.sphereRadius = 0f;
					light4.position = light.transform.position;
					light4.orientation = light.transform.rotation;
					light4.color = color2;
					light4.indirectColor = indirectColor;
					light4.range = light.range;
					light4.coneAngle = light.spotAngle * (MathF.PI / 180f);
					light4.innerConeAngle = light.spotAngle * (MathF.PI / 180f) * hDAdditionalLightData.innerSpotPercent01;
					light4.falloff = ((!hDAdditionalLightData.applyRangeAttenuation) ? FalloffType.InverseSquaredNoRangeAttenuation : FalloffType.InverseSquared);
					light4.angularFalloff = AngularFalloffType.AnalyticAndInnerAngle;
					lightDataGI.Init(ref light4, ref cookie);
					lightDataGI.shape1 = 1f;
					if (light.cookie != null)
					{
						lightDataGI.cookieID = light.cookie.GetInstanceID();
					}
					else if (hDAdditionalLightData.IESSpot != null)
					{
						lightDataGI.cookieID = hDAdditionalLightData.IESSpot.GetInstanceID();
					}
					else
					{
						lightDataGI.cookieID = 0;
					}
					break;
				}
				case SpotLightShape.Pyramid:
				{
					SpotLightPyramidShape light3 = default(SpotLightPyramidShape);
					light3.instanceID = light.GetInstanceID();
					light3.shadow = light.shadows != LightShadows.None;
					light3.mode = lightMode;
					light3.position = light.transform.position;
					light3.orientation = light.transform.rotation;
					light3.color = color2;
					light3.indirectColor = indirectColor;
					light3.range = light.range;
					light3.angle = light.spotAngle * (MathF.PI / 180f);
					light3.aspectRatio = hDAdditionalLightData.aspectRatio;
					light3.falloff = ((!hDAdditionalLightData.applyRangeAttenuation) ? FalloffType.InverseSquaredNoRangeAttenuation : FalloffType.InverseSquared);
					lightDataGI.Init(ref light3, ref cookie);
					if (light.cookie != null)
					{
						lightDataGI.cookieID = light.cookie.GetInstanceID();
					}
					else if (hDAdditionalLightData.IESSpot != null)
					{
						lightDataGI.cookieID = hDAdditionalLightData.IESSpot.GetInstanceID();
					}
					else
					{
						lightDataGI.cookieID = 0;
					}
					break;
				}
				case SpotLightShape.Box:
				{
					SpotLightBoxShape light2 = default(SpotLightBoxShape);
					light2.instanceID = light.GetInstanceID();
					light2.shadow = light.shadows != LightShadows.None;
					light2.mode = lightMode;
					light2.position = light.transform.position;
					light2.orientation = light.transform.rotation;
					light2.color = color2;
					light2.indirectColor = indirectColor;
					light2.range = light.range;
					light2.width = hDAdditionalLightData.shapeWidth;
					light2.height = hDAdditionalLightData.shapeHeight;
					lightDataGI.Init(ref light2, ref cookie);
					if (light.cookie != null)
					{
						lightDataGI.cookieID = light.cookie.GetInstanceID();
					}
					else if (hDAdditionalLightData.IESSpot != null)
					{
						lightDataGI.cookieID = hDAdditionalLightData.IESSpot.GetInstanceID();
					}
					else
					{
						lightDataGI.cookieID = 0;
					}
					break;
				}
				}
				break;
			case HDLightType.Point:
				lightDataGI.orientation = light.transform.rotation;
				lightDataGI.position = light.transform.position;
				lightDataGI.range = light.range;
				lightDataGI.coneAngle = 0f;
				lightDataGI.innerConeAngle = 0f;
				lightDataGI.shape0 = 0f;
				lightDataGI.shape1 = 0f;
				lightDataGI.type = UnityEngine.Experimental.GlobalIllumination.LightType.Point;
				lightDataGI.falloff = ((!hDAdditionalLightData.applyRangeAttenuation) ? FalloffType.InverseSquaredNoRangeAttenuation : FalloffType.InverseSquared);
				break;
			case HDLightType.Area:
				switch (hDAdditionalLightData.areaLightShape)
				{
				case AreaLightShape.Rectangle:
					lightDataGI.orientation = light.transform.rotation;
					lightDataGI.position = light.transform.position;
					lightDataGI.range = light.range;
					lightDataGI.coneAngle = 0f;
					lightDataGI.innerConeAngle = 0f;
					lightDataGI.shape0 = hDAdditionalLightData.shapeWidth;
					lightDataGI.shape1 = hDAdditionalLightData.shapeHeight;
					lightDataGI.type = UnityEngine.Experimental.GlobalIllumination.LightType.Rectangle;
					lightDataGI.falloff = ((!hDAdditionalLightData.applyRangeAttenuation) ? FalloffType.InverseSquaredNoRangeAttenuation : FalloffType.InverseSquared);
					if (hDAdditionalLightData.areaLightCookie != null)
					{
						lightDataGI.cookieID = hDAdditionalLightData.areaLightCookie.GetInstanceID();
					}
					else if (hDAdditionalLightData.IESSpot != null)
					{
						lightDataGI.cookieID = hDAdditionalLightData.IESSpot.GetInstanceID();
					}
					else
					{
						lightDataGI.cookieID = 0;
					}
					break;
				case AreaLightShape.Tube:
					lightDataGI.InitNoBake(lightDataGI.instanceID);
					break;
				case AreaLightShape.Disc:
					lightDataGI.orientation = light.transform.rotation;
					lightDataGI.position = light.transform.position;
					lightDataGI.range = light.range;
					lightDataGI.coneAngle = 0f;
					lightDataGI.innerConeAngle = 0f;
					lightDataGI.shape0 = 0f;
					lightDataGI.shape1 = 0f;
					lightDataGI.type = UnityEngine.Experimental.GlobalIllumination.LightType.Disc;
					lightDataGI.falloff = ((!hDAdditionalLightData.applyRangeAttenuation) ? FalloffType.InverseSquaredNoRangeAttenuation : FalloffType.InverseSquared);
					lightDataGI.cookieID = (hDAdditionalLightData.areaLightCookie ? hDAdditionalLightData.areaLightCookie.GetInstanceID() : 0);
					break;
				}
				break;
			}
			return true;
		}
	}
}
