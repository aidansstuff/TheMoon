namespace UnityEngine.Rendering.HighDefinition
{
	internal class WaterSurfacePresets
	{
		internal static void ApplyCommonPreset(WaterSurface waterSurface)
		{
			waterSurface.timeMultiplier = 1f;
			waterSurface.cpuSimulation = false;
			waterSurface.cpuFullResolution = false;
			waterSurface.cpuEvaluateRipples = false;
			waterSurface.waterMask = null;
			waterSurface.waterMaskExtent.Set(100f, 100f);
			waterSurface.waterMaskOffset = Vector2.zero;
			waterSurface.largeBand0Multiplier = 1f;
			waterSurface.largeBand1Multiplier = 1f;
			waterSurface.largeBand0FadeToggle = true;
			waterSurface.largeBand1FadeToggle = true;
			waterSurface.ripplesFadeToggle = true;
			waterSurface.ripplesFadeStart = 50f;
			waterSurface.ripplesFadeDistance = 200f;
			waterSurface.ripples = true;
			waterSurface.ripplesChaos = 0.8f;
			waterSurface.ripplesWindSpeed = 8f;
			waterSurface.refractionColor = new Color(0.1f, 0.5f, 0.5f);
			waterSurface.maxRefractionDistance = 0.5f;
			waterSurface.absorptionDistance = 1.5f;
			waterSurface.caustics = true;
			waterSurface.causticsBand = 2;
			waterSurface.causticsIntensity = 0.5f;
			waterSurface.causticsResolution = WaterSurface.WaterCausticsResolution.Caustics256;
			waterSurface.virtualPlaneDistance = 4f;
			waterSurface.foam = false;
		}

		internal static void ApplyWaterOceanPreset(WaterSurface waterSurface)
		{
			ApplyCommonPreset(waterSurface);
			waterSurface.surfaceType = WaterSurfaceType.OceanSeaLake;
			waterSurface.geometryType = WaterGeometryType.Infinite;
			waterSurface.geometryType = WaterGeometryType.Infinite;
			waterSurface.cpuSimulation = true;
			waterSurface.repetitionSize = 500f;
			waterSurface.largeWindSpeed = 30f;
			waterSurface.largeChaos = 0.85f;
			waterSurface.largeCurrentSpeedValue = 0f;
			waterSurface.largeBand0FadeStart = 1500f;
			waterSurface.largeBand0FadeDistance = 3000f;
			waterSurface.largeBand1FadeStart = 300f;
			waterSurface.largeBand1FadeDistance = 800f;
			waterSurface.scatteringColor = new Color(0f, 0.4f, 0.4f);
			waterSurface.ambientScattering = 0.2f;
			waterSurface.heightScattering = 0.2f;
			waterSurface.displacementScattering = 0.1f;
			waterSurface.directLightTipScattering = 0.6f;
			waterSurface.directLightBodyScattering = 0.5f;
			waterSurface.foam = true;
			waterSurface.foamTextureTiling = 0.15f;
			waterSurface.simulationFoamAmount = 0.2f;
			waterSurface.simulationFoamDrag = 0f;
			waterSurface.simulationFoamSmoothness = 1f;
			waterSurface.foamTexture = null;
			waterSurface.foamMask = null;
			waterSurface.caustics = false;
		}

		internal static void ApplyWaterRiverPreset(WaterSurface waterSurface)
		{
			ApplyCommonPreset(waterSurface);
			waterSurface.surfaceType = WaterSurfaceType.River;
			waterSurface.geometryType = WaterGeometryType.Quad;
			waterSurface.repetitionSize = 75f;
			waterSurface.largeWindSpeed = 17.5f;
			waterSurface.largeChaos = 0.9f;
			waterSurface.largeCurrentSpeedValue = 1.5f;
			waterSurface.largeBand0FadeStart = 150f;
			waterSurface.largeBand0FadeDistance = 300f;
			waterSurface.ripplesChaos = 0.2f;
			waterSurface.scatteringColor = new Color(0f, 0.4f, 0.45f);
			waterSurface.ambientScattering = 0.35f;
			waterSurface.heightScattering = 0.2f;
			waterSurface.displacementScattering = 0.1f;
			waterSurface.directLightTipScattering = 0.6f;
			waterSurface.directLightBodyScattering = 0.5f;
			waterSurface.causticsPlaneBlendDistance = 1f;
		}

		internal static void ApplyWaterPoolPreset(WaterSurface waterSurface)
		{
			ApplyCommonPreset(waterSurface);
			waterSurface.surfaceType = WaterSurfaceType.Pool;
			waterSurface.geometryType = WaterGeometryType.Quad;
			waterSurface.timeMultiplier = 0.8f;
			waterSurface.ripplesWindSpeed = 5f;
			waterSurface.ripplesChaos = 1f;
			waterSurface.refractionColor = new Color(0.2f, 0.55f, 0.55f);
			waterSurface.maxRefractionDistance = 0.35f;
			waterSurface.absorptionDistance = 5f;
			waterSurface.scatteringColor = new Color(0f, 0.5f, 0.6f);
			waterSurface.ambientScattering = 0.6f;
			waterSurface.heightScattering = 0f;
			waterSurface.displacementScattering = 0f;
			waterSurface.directLightBodyScattering = 0.2f;
			waterSurface.directLightTipScattering = 0.2f;
			waterSurface.causticsPlaneBlendDistance = 2f;
			waterSurface.underWater = true;
			waterSurface.volumeBounds = waterSurface.GetComponent<BoxCollider>();
			waterSurface.volumePrority = 0;
			waterSurface.transitionSize = 0.2f;
			waterSurface.absorbtionDistanceMultiplier = 1f;
		}
	}
}
