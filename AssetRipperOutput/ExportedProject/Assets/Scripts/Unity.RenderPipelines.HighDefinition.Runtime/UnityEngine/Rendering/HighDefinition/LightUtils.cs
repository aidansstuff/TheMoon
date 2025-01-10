using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class LightUtils
	{
		private static float s_LuminanceToEvFactor => Mathf.Log(100f / ColorUtils.s_LightMeterCalibrationConstant, 2f);

		private static float s_EvToLuminanceFactor => 0f - Mathf.Log(100f / ColorUtils.s_LightMeterCalibrationConstant, 2f);

		public static float ConvertPointLightLumenToCandela(float intensity)
		{
			return intensity / (MathF.PI * 4f);
		}

		public static float ConvertPointLightCandelaToLumen(float intensity)
		{
			return intensity * (MathF.PI * 4f);
		}

		public static float ConvertSpotLightLumenToCandela(float intensity, float angle, bool exact)
		{
			if (!exact)
			{
				return intensity / MathF.PI;
			}
			return intensity / (2f * (1f - Mathf.Cos(angle / 2f)) * MathF.PI);
		}

		public static float ConvertSpotLightCandelaToLumen(float intensity, float angle, bool exact)
		{
			if (!exact)
			{
				return intensity * MathF.PI;
			}
			return intensity * (2f * (1f - Mathf.Cos(angle / 2f)) * MathF.PI);
		}

		public static float ConvertFrustrumLightLumenToCandela(float intensity, float angleA, float angleB)
		{
			return intensity / (4f * Mathf.Asin(Mathf.Sin(angleA / 2f) * Mathf.Sin(angleB / 2f)));
		}

		public static float ConvertFrustrumLightCandelaToLumen(float intensity, float angleA, float angleB)
		{
			return intensity * (4f * Mathf.Asin(Mathf.Sin(angleA / 2f) * Mathf.Sin(angleB / 2f)));
		}

		public static float ConvertSphereLightLumenToLuminance(float intensity, float sphereRadius)
		{
			return intensity / (MathF.PI * 4f * sphereRadius * sphereRadius * MathF.PI);
		}

		public static float ConvertSphereLightLuminanceToLumen(float intensity, float sphereRadius)
		{
			return intensity * (MathF.PI * 4f * sphereRadius * sphereRadius * MathF.PI);
		}

		public static float ConvertDiscLightLumenToLuminance(float intensity, float discRadius)
		{
			return intensity / (discRadius * discRadius * MathF.PI * MathF.PI);
		}

		public static float ConvertDiscLightLuminanceToLumen(float intensity, float discRadius)
		{
			return intensity * (discRadius * discRadius * MathF.PI * MathF.PI);
		}

		public static float ConvertRectLightLumenToLuminance(float intensity, float width, float height)
		{
			return intensity / (width * height * MathF.PI);
		}

		public static float ConvertRectLightLuminanceToLumen(float intensity, float width, float height)
		{
			return intensity * (width * height * MathF.PI);
		}

		public static float ConvertLuxToCandela(float lux, float distance)
		{
			return lux * distance * distance;
		}

		public static float ConvertCandelaToLux(float candela, float distance)
		{
			return candela / (distance * distance);
		}

		public static float ConvertEvToLuminance(float ev)
		{
			return Mathf.Pow(2f, ev + s_EvToLuminanceFactor);
		}

		public static float ConvertEvToCandela(float ev)
		{
			return ConvertEvToLuminance(ev);
		}

		public static float ConvertEvToLux(float ev, float distance)
		{
			return ConvertCandelaToLux(ConvertEvToLuminance(ev), distance);
		}

		public static float ConvertLuminanceToEv(float luminance)
		{
			return Mathf.Log(luminance, 2f) + s_LuminanceToEvFactor;
		}

		public static float ConvertCandelaToEv(float candela)
		{
			return ConvertLuminanceToEv(candela);
		}

		public static float ConvertLuxToEv(float lux, float distance)
		{
			return ConvertLuminanceToEv(ConvertLuxToCandela(lux, distance));
		}

		public static float ConvertPunctualLightLumenToCandela(HDLightType lightType, float lumen, float initialIntensity, bool enableSpotReflector)
		{
			if (lightType == HDLightType.Spot && enableSpotReflector)
			{
				return initialIntensity;
			}
			return ConvertPointLightLumenToCandela(lumen);
		}

		public static float ConvertPunctualLightLumenToLux(HDLightType lightType, float lumen, float initialIntensity, bool enableSpotReflector, float distance)
		{
			return ConvertCandelaToLux(ConvertPunctualLightLumenToCandela(lightType, lumen, initialIntensity, enableSpotReflector), distance);
		}

		public static float ConvertPunctualLightCandelaToLumen(HDLightType lightType, SpotLightShape spotLightShape, float candela, bool enableSpotReflector, float spotAngle, float aspectRatio)
		{
			if (lightType == HDLightType.Spot && enableSpotReflector)
			{
				switch (spotLightShape)
				{
				case SpotLightShape.Cone:
					return ConvertSpotLightCandelaToLumen(candela, spotAngle * (MathF.PI / 180f), exact: true);
				case SpotLightShape.Pyramid:
				{
					CalculateAnglesForPyramid(aspectRatio, spotAngle * (MathF.PI / 180f), out var angleA, out var angleB);
					return ConvertFrustrumLightCandelaToLumen(candela, angleA, angleB);
				}
				default:
					return ConvertPointLightCandelaToLumen(candela);
				}
			}
			return ConvertPointLightCandelaToLumen(candela);
		}

		public static float ConvertPunctualLightLuxToLumen(HDLightType lightType, SpotLightShape spotLightShape, float lux, bool enableSpotReflector, float spotAngle, float aspectRatio, float distance)
		{
			float candela = ConvertLuxToCandela(lux, distance);
			return ConvertPunctualLightCandelaToLumen(lightType, spotLightShape, candela, enableSpotReflector, spotAngle, aspectRatio);
		}

		public static float ConvertPunctualLightEvToLumen(HDLightType lightType, SpotLightShape spotLightShape, float ev, bool enableSpotReflector, float spotAngle, float aspectRatio)
		{
			float candela = ConvertEvToCandela(ev);
			return ConvertPunctualLightCandelaToLumen(lightType, spotLightShape, candela, enableSpotReflector, spotAngle, aspectRatio);
		}

		public static float ConvertPunctualLightLumenToEv(HDLightType lightType, float lumen, float initialIntensity, bool enableSpotReflector)
		{
			return ConvertCandelaToEv(ConvertPunctualLightLumenToCandela(lightType, lumen, initialIntensity, enableSpotReflector));
		}

		public static float ConvertAreaLightLumenToLuminance(AreaLightShape areaLightShape, float lumen, float width, float height = 0f)
		{
			return areaLightShape switch
			{
				AreaLightShape.Tube => CalculateLineLightLumenToLuminance(lumen, width), 
				AreaLightShape.Rectangle => ConvertRectLightLumenToLuminance(lumen, width, height), 
				AreaLightShape.Disc => ConvertDiscLightLumenToLuminance(lumen, width), 
				_ => lumen, 
			};
		}

		public static float ConvertAreaLightLuminanceToLumen(AreaLightShape areaLightShape, float luminance, float width, float height = 0f)
		{
			return areaLightShape switch
			{
				AreaLightShape.Tube => CalculateLineLightLuminanceToLumen(luminance, width), 
				AreaLightShape.Rectangle => ConvertRectLightLuminanceToLumen(luminance, width, height), 
				AreaLightShape.Disc => ConvertDiscLightLuminanceToLumen(luminance, width), 
				_ => luminance, 
			};
		}

		public static float ConvertAreaLightLumenToEv(AreaLightShape AreaLightShape, float lumen, float width, float height)
		{
			return ConvertLuminanceToEv(ConvertAreaLightLumenToLuminance(AreaLightShape, lumen, width, height));
		}

		public static float ConvertAreaLightEvToLumen(AreaLightShape AreaLightShape, float ev, float width, float height)
		{
			float luminance = ConvertEvToLuminance(ev);
			return ConvertAreaLightLuminanceToLumen(AreaLightShape, luminance, width, height);
		}

		public static float CalculateLineLightLumenToLuminance(float intensity, float lineWidth)
		{
			return intensity / (MathF.PI * 4f * lineWidth);
		}

		public static float CalculateLineLightLuminanceToLumen(float intensity, float lineWidth)
		{
			return intensity * (MathF.PI * 4f * lineWidth);
		}

		public static void CalculateAnglesForPyramid(float aspectRatio, float spotAngle, out float angleA, out float angleB)
		{
			if (aspectRatio < 1f)
			{
				aspectRatio = 1f / aspectRatio;
			}
			angleA = spotAngle;
			float f = angleA * 0.5f;
			f = Mathf.Atan(Mathf.Tan(f) * aspectRatio);
			angleB = f * 2f;
		}

		internal static void ConvertLightIntensity(LightUnit oldLightUnit, LightUnit newLightUnit, HDAdditionalLightData hdLight, Light light)
		{
			float num = hdLight.intensity;
			_ = hdLight.luxAtDistance;
			HDLightType hDLightType = hdLight.ComputeLightType(light);
			if (hDLightType != HDLightType.Area)
			{
				if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Candela)
				{
					num = ConvertPunctualLightLumenToCandela(hDLightType, num, light.intensity, hdLight.enableSpotReflector);
				}
				else if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Lux)
				{
					num = ConvertPunctualLightLumenToLux(hDLightType, num, light.intensity, hdLight.enableSpotReflector, hdLight.luxAtDistance);
				}
				else if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Ev100)
				{
					num = ConvertPunctualLightLumenToEv(hDLightType, num, light.intensity, hdLight.enableSpotReflector);
				}
				else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Lumen)
				{
					num = ConvertPunctualLightCandelaToLumen(hDLightType, hdLight.spotLightShape, num, hdLight.enableSpotReflector, light.spotAngle, hdLight.aspectRatio);
				}
				else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Lux)
				{
					num = ConvertCandelaToLux(num, hdLight.luxAtDistance);
				}
				else if (oldLightUnit == LightUnit.Candela && newLightUnit == LightUnit.Ev100)
				{
					num = ConvertCandelaToEv(num);
				}
				else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Lumen)
				{
					num = ConvertPunctualLightLuxToLumen(hDLightType, hdLight.spotLightShape, num, hdLight.enableSpotReflector, light.spotAngle, hdLight.aspectRatio, hdLight.luxAtDistance);
				}
				else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Candela)
				{
					num = ConvertLuxToCandela(num, hdLight.luxAtDistance);
				}
				else if (oldLightUnit == LightUnit.Lux && newLightUnit == LightUnit.Ev100)
				{
					num = ConvertLuxToEv(num, hdLight.luxAtDistance);
				}
				else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lumen)
				{
					num = ConvertPunctualLightEvToLumen(hDLightType, hdLight.spotLightShape, num, hdLight.enableSpotReflector, light.spotAngle, hdLight.aspectRatio);
				}
				else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Candela)
				{
					num = ConvertEvToCandela(num);
				}
				else if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lux)
				{
					num = ConvertEvToLux(num, hdLight.luxAtDistance);
				}
			}
			else
			{
				if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Nits)
				{
					num = ConvertAreaLightLumenToLuminance(hdLight.areaLightShape, num, hdLight.shapeWidth, hdLight.shapeHeight);
				}
				if (oldLightUnit == LightUnit.Nits && newLightUnit == LightUnit.Lumen)
				{
					num = ConvertAreaLightLuminanceToLumen(hdLight.areaLightShape, num, hdLight.shapeWidth, hdLight.shapeHeight);
				}
				if (oldLightUnit == LightUnit.Nits && newLightUnit == LightUnit.Ev100)
				{
					num = ConvertLuminanceToEv(num);
				}
				if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Nits)
				{
					num = ConvertEvToLuminance(num);
				}
				if (oldLightUnit == LightUnit.Ev100 && newLightUnit == LightUnit.Lumen)
				{
					num = ConvertAreaLightEvToLumen(hdLight.areaLightShape, num, hdLight.shapeWidth, hdLight.shapeHeight);
				}
				if (oldLightUnit == LightUnit.Lumen && newLightUnit == LightUnit.Ev100)
				{
					num = ConvertAreaLightLumenToEv(hdLight.areaLightShape, num, hdLight.shapeWidth, hdLight.shapeHeight);
				}
			}
			hdLight.intensity = num;
		}

		internal static Color EvaluateLightColor(Light light, HDAdditionalLightData hdLight)
		{
			Color result = light.color.linear * light.intensity;
			if (hdLight.useColorTemperature)
			{
				result *= Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature);
			}
			return result;
		}
	}
}
