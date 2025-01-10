using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class HDRenderQueue
	{
		public enum Priority
		{
			Background = 1000,
			Opaque = 2000,
			OpaqueDecal = 2225,
			OpaqueAlphaTest = 2450,
			OpaqueDecalAlphaTest = 2475,
			OpaqueLast = 2500,
			AfterPostprocessOpaque = 2501,
			AfterPostprocessOpaqueAlphaTest = 2510,
			PreRefractionFirst = 2650,
			PreRefraction = 2750,
			PreRefractionLast = 2850,
			TransparentFirst = 2900,
			Transparent = 3000,
			TransparentLast = 3100,
			LowTransparentFirst = 3300,
			LowTransparent = 3400,
			LowTransparentLast = 3500,
			AfterPostprocessTransparentFirst = 3600,
			AfterPostprocessTransparent = 3700,
			AfterPostprocessTransparentLast = 3800,
			Overlay = 4000
		}

		public enum RenderQueueType
		{
			Background = 0,
			Opaque = 1,
			AfterPostProcessOpaque = 2,
			PreRefraction = 3,
			Transparent = 4,
			LowTransparent = 5,
			AfterPostprocessTransparent = 6,
			Overlay = 7,
			Unknown = 8
		}

		public enum OpaqueRenderQueue
		{
			Default = 0,
			AfterPostProcessing = 1
		}

		public enum TransparentRenderQueue
		{
			BeforeRefraction = 0,
			Default = 1,
			LowResolution = 2,
			AfterPostProcessing = 3
		}

		private const int k_TransparentPriorityQueueRangeStep = 100;

		public static readonly RenderQueueRange k_RenderQueue_OpaqueNoAlphaTest = new RenderQueueRange
		{
			lowerBound = 1000,
			upperBound = 2449
		};

		public static readonly RenderQueueRange k_RenderQueue_OpaqueAlphaTest = new RenderQueueRange
		{
			lowerBound = 2450,
			upperBound = 2500
		};

		public static readonly RenderQueueRange k_RenderQueue_OpaqueDecalAndAlphaTest = new RenderQueueRange
		{
			lowerBound = 2225,
			upperBound = 2500
		};

		public static readonly RenderQueueRange k_RenderQueue_AllOpaque = new RenderQueueRange
		{
			lowerBound = 1000,
			upperBound = 2500
		};

		public static readonly RenderQueueRange k_RenderQueue_AfterPostProcessOpaque = new RenderQueueRange
		{
			lowerBound = 2501,
			upperBound = 2510
		};

		public static readonly RenderQueueRange k_RenderQueue_PreRefraction = new RenderQueueRange
		{
			lowerBound = 2650,
			upperBound = 2850
		};

		public static readonly RenderQueueRange k_RenderQueue_Transparent = new RenderQueueRange
		{
			lowerBound = 2900,
			upperBound = 3100
		};

		public static readonly RenderQueueRange k_RenderQueue_TransparentWithLowRes = new RenderQueueRange
		{
			lowerBound = 2900,
			upperBound = 3500
		};

		public static readonly RenderQueueRange k_RenderQueue_LowTransparent = new RenderQueueRange
		{
			lowerBound = 3300,
			upperBound = 3500
		};

		public static readonly RenderQueueRange k_RenderQueue_AllTransparent = new RenderQueueRange
		{
			lowerBound = 2650,
			upperBound = 3100
		};

		public static readonly RenderQueueRange k_RenderQueue_AllTransparentWithLowRes = new RenderQueueRange
		{
			lowerBound = 2650,
			upperBound = 3500
		};

		public static readonly RenderQueueRange k_RenderQueue_AfterPostProcessTransparent = new RenderQueueRange
		{
			lowerBound = 3600,
			upperBound = 3800
		};

		public static readonly RenderQueueRange k_RenderQueue_Overlay = new RenderQueueRange
		{
			lowerBound = 4000,
			upperBound = 5000
		};

		public static readonly RenderQueueRange k_RenderQueue_All = new RenderQueueRange
		{
			lowerBound = 0,
			upperBound = 5000
		};

		public const int sortingPriorityRange = 50;

		public const int meshDecalPriorityRange = 50;

		internal static RenderQueueType MigrateRenderQueueToHDRP10(RenderQueueType renderQueue)
		{
			return (int)renderQueue switch
			{
				0 => RenderQueueType.Background, 
				1 => RenderQueueType.Opaque, 
				2 => RenderQueueType.AfterPostProcessOpaque, 
				3 => RenderQueueType.Opaque, 
				4 => RenderQueueType.PreRefraction, 
				5 => RenderQueueType.Transparent, 
				6 => RenderQueueType.LowTransparent, 
				7 => RenderQueueType.AfterPostprocessTransparent, 
				8 => RenderQueueType.Transparent, 
				9 => RenderQueueType.Overlay, 
				_ => RenderQueueType.Unknown, 
			};
		}

		public static bool Contains(this RenderQueueRange range, int value)
		{
			if (range.lowerBound <= value)
			{
				return value <= range.upperBound;
			}
			return false;
		}

		public static int Clamps(this RenderQueueRange range, int value)
		{
			return Math.Max(range.lowerBound, Math.Min(value, range.upperBound));
		}

		public static int ClampsTransparentRangePriority(int value)
		{
			return Math.Max(-50, Math.Min(value, 50));
		}

		public static RenderQueueType GetTypeByRenderQueueValue(int renderQueue)
		{
			if (renderQueue == 1000)
			{
				return RenderQueueType.Background;
			}
			if (k_RenderQueue_AllOpaque.Contains(renderQueue))
			{
				return RenderQueueType.Opaque;
			}
			if (k_RenderQueue_AfterPostProcessOpaque.Contains(renderQueue))
			{
				return RenderQueueType.AfterPostProcessOpaque;
			}
			if (k_RenderQueue_PreRefraction.Contains(renderQueue))
			{
				return RenderQueueType.PreRefraction;
			}
			if (k_RenderQueue_Transparent.Contains(renderQueue))
			{
				return RenderQueueType.Transparent;
			}
			if (k_RenderQueue_LowTransparent.Contains(renderQueue))
			{
				return RenderQueueType.LowTransparent;
			}
			if (k_RenderQueue_AfterPostProcessTransparent.Contains(renderQueue))
			{
				return RenderQueueType.AfterPostprocessTransparent;
			}
			if (renderQueue == 4000)
			{
				return RenderQueueType.Overlay;
			}
			return RenderQueueType.Unknown;
		}

		public static int ChangeType(RenderQueueType targetType, int offset = 0, bool alphaTest = false, bool receiveDecal = false)
		{
			switch (targetType)
			{
			case RenderQueueType.Background:
				return 1000;
			case RenderQueueType.Opaque:
				if (!alphaTest)
				{
					if (!receiveDecal)
					{
						return 2000;
					}
					return 2225;
				}
				if (!receiveDecal)
				{
					return 2450;
				}
				return 2475;
			case RenderQueueType.AfterPostProcessOpaque:
				if (!alphaTest)
				{
					return 2501;
				}
				return 2510;
			case RenderQueueType.PreRefraction:
				return 2750 + offset;
			case RenderQueueType.Transparent:
				return 3000 + offset;
			case RenderQueueType.LowTransparent:
				return 3400 + offset;
			case RenderQueueType.AfterPostprocessTransparent:
				return 3700 + offset;
			case RenderQueueType.Overlay:
				return 4000;
			default:
				throw new ArgumentException("Unknown RenderQueueType, was " + targetType);
			}
		}

		public static RenderQueueType GetTransparentEquivalent(RenderQueueType type)
		{
			switch (type)
			{
			case RenderQueueType.Opaque:
				return RenderQueueType.Transparent;
			case RenderQueueType.AfterPostProcessOpaque:
				return RenderQueueType.AfterPostprocessTransparent;
			default:
				return type;
			case RenderQueueType.Background:
			case RenderQueueType.Overlay:
				throw new ArgumentException("Unknown RenderQueueType conversion to transparent equivalent, was " + type);
			}
		}

		public static RenderQueueType GetOpaqueEquivalent(RenderQueueType type)
		{
			switch (type)
			{
			case RenderQueueType.PreRefraction:
			case RenderQueueType.Transparent:
			case RenderQueueType.LowTransparent:
				return RenderQueueType.Opaque;
			case RenderQueueType.AfterPostprocessTransparent:
				return RenderQueueType.AfterPostProcessOpaque;
			default:
				return type;
			case RenderQueueType.Background:
			case RenderQueueType.Overlay:
				throw new ArgumentException("Unknown RenderQueueType conversion to opaque equivalent, was " + type);
			}
		}

		public static OpaqueRenderQueue ConvertToOpaqueRenderQueue(RenderQueueType renderQueue)
		{
			return renderQueue switch
			{
				RenderQueueType.Opaque => OpaqueRenderQueue.Default, 
				RenderQueueType.AfterPostProcessOpaque => OpaqueRenderQueue.AfterPostProcessing, 
				_ => throw new ArgumentException("Cannot map to OpaqueRenderQueue, was " + renderQueue), 
			};
		}

		public static RenderQueueType ConvertFromOpaqueRenderQueue(OpaqueRenderQueue opaqueRenderQueue)
		{
			return opaqueRenderQueue switch
			{
				OpaqueRenderQueue.Default => RenderQueueType.Opaque, 
				OpaqueRenderQueue.AfterPostProcessing => RenderQueueType.AfterPostProcessOpaque, 
				_ => throw new ArgumentException("Unknown OpaqueRenderQueue, was " + opaqueRenderQueue), 
			};
		}

		public static TransparentRenderQueue ConvertToTransparentRenderQueue(RenderQueueType renderQueue)
		{
			return renderQueue switch
			{
				RenderQueueType.PreRefraction => TransparentRenderQueue.BeforeRefraction, 
				RenderQueueType.Transparent => TransparentRenderQueue.Default, 
				RenderQueueType.LowTransparent => TransparentRenderQueue.LowResolution, 
				RenderQueueType.AfterPostprocessTransparent => TransparentRenderQueue.AfterPostProcessing, 
				_ => throw new ArgumentException("Cannot map to TransparentRenderQueue, was " + renderQueue), 
			};
		}

		public static RenderQueueType ConvertFromTransparentRenderQueue(TransparentRenderQueue transparentRenderqueue)
		{
			return transparentRenderqueue switch
			{
				TransparentRenderQueue.BeforeRefraction => RenderQueueType.PreRefraction, 
				TransparentRenderQueue.Default => RenderQueueType.Transparent, 
				TransparentRenderQueue.LowResolution => RenderQueueType.LowTransparent, 
				TransparentRenderQueue.AfterPostProcessing => RenderQueueType.AfterPostprocessTransparent, 
				_ => throw new ArgumentException("Unknown TransparentRenderQueue, was " + transparentRenderqueue), 
			};
		}

		public static string GetShaderTagValue(int index)
		{
			if (k_RenderQueue_AllTransparent.Contains(index) || k_RenderQueue_AfterPostProcessTransparent.Contains(index) || k_RenderQueue_LowTransparent.Contains(index))
			{
				int num = index - 3000;
				return "Transparent" + ((num < 0) ? "" : "+") + num;
			}
			if (index >= 4000)
			{
				return "Overlay+" + (index - 4000);
			}
			if (index >= 2450)
			{
				return "AlphaTest+" + (index - 2450);
			}
			if (index >= 2000)
			{
				return "Geometry+" + (index - 2000);
			}
			int num2 = index - 1000;
			return "Background" + ((num2 < 0) ? "" : "+") + num2;
		}
	}
}
