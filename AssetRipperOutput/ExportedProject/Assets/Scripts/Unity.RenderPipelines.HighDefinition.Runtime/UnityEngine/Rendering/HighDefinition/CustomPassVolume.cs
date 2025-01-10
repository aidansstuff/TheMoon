using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	public class CustomPassVolume : MonoBehaviour, IVolume
	{
		[SerializeField]
		[FormerlySerializedAs("isGlobal")]
		private bool m_IsGlobal = true;

		[Min(0f)]
		public float fadeRadius;

		[Tooltip("Sets the Volume priority in the stack. A higher value means higher priority. You can use negative values.")]
		public float priority;

		[SerializeReference]
		public List<CustomPass> customPasses = new List<CustomPass>();

		public CustomPassInjectionPoint injectionPoint = CustomPassInjectionPoint.BeforeTransparent;

		[SerializeField]
		internal Camera m_TargetCamera;

		[SerializeField]
		internal bool useTargetCamera;

		private static HashSet<CustomPassVolume> m_ActivePassVolumes = new HashSet<CustomPassVolume>();

		private static List<CustomPassVolume> m_OverlappingPassVolumes = new List<CustomPassVolume>();

		internal List<Collider> m_Colliders = new List<Collider>();

		private List<Collider> m_OverlappingColliders = new List<Collider>();

		private static List<CustomPassInjectionPoint> m_InjectionPoints;

		public bool isGlobal
		{
			get
			{
				return m_IsGlobal;
			}
			set
			{
				m_IsGlobal = value;
			}
		}

		public Camera targetCamera
		{
			get
			{
				if (!useTargetCamera)
				{
					return null;
				}
				return m_TargetCamera;
			}
			set
			{
				m_TargetCamera = value;
				useTargetCamera = value != null;
			}
		}

		public float fadeValue { get; private set; }

		public List<Collider> colliders => m_Colliders;

		private static List<CustomPassInjectionPoint> injectionPoints
		{
			get
			{
				if (m_InjectionPoints == null)
				{
					m_InjectionPoints = Enum.GetValues(typeof(CustomPassInjectionPoint)).Cast<CustomPassInjectionPoint>().ToList();
				}
				return m_InjectionPoints;
			}
		}

		private void OnEnable()
		{
			customPasses.RemoveAll((CustomPass c) => c == null);
			GetComponents(m_Colliders);
			Register(this);
		}

		private void OnDisable()
		{
			UnRegister(this);
			CleanupPasses();
		}

		private bool IsVisible(HDCamera hdCamera)
		{
			if (useTargetCamera)
			{
				return targetCamera == hdCamera.camera;
			}
			if (hdCamera.camera.cameraType != CameraType.SceneView && (hdCamera.volumeLayerMask & (1 << base.gameObject.layer)) == 0)
			{
				return false;
			}
			return true;
		}

		internal bool Execute(RenderGraph renderGraph, HDCamera hdCamera, CullingResults cullingResult, CullingResults cameraCullingResult, in CustomPass.RenderTargets targets)
		{
			bool result = false;
			if (!IsVisible(hdCamera))
			{
				return false;
			}
			foreach (CustomPass customPass in customPasses)
			{
				if (customPass != null && customPass.WillBeExecuted(hdCamera))
				{
					customPass.ExecuteInternal(renderGraph, hdCamera, cullingResult, cameraCullingResult, in targets, this);
					result = true;
				}
			}
			return result;
		}

		internal bool WillExecuteInjectionPoint(HDCamera hdCamera)
		{
			bool result = false;
			if (!IsVisible(hdCamera))
			{
				return false;
			}
			foreach (CustomPass customPass in customPasses)
			{
				if (customPass != null && customPass.WillBeExecuted(hdCamera))
				{
					result = true;
				}
			}
			return result;
		}

		internal void CleanupPasses()
		{
			foreach (CustomPass customPass in customPasses)
			{
				customPass.CleanupPassInternal();
			}
		}

		private static void Register(CustomPassVolume volume)
		{
			m_ActivePassVolumes.Add(volume);
		}

		private static void UnRegister(CustomPassVolume volume)
		{
			m_ActivePassVolumes.Remove(volume);
		}

		internal static void Update(HDCamera camera)
		{
			Vector3 position = camera.volumeAnchor.position;
			m_OverlappingPassVolumes.Clear();
			foreach (CustomPassVolume activePassVolume in m_ActivePassVolumes)
			{
				if (!activePassVolume.IsVisible(camera))
				{
					continue;
				}
				if (activePassVolume.useTargetCamera)
				{
					if (activePassVolume.targetCamera == camera.camera)
					{
						m_OverlappingPassVolumes.Add(activePassVolume);
					}
				}
				else if (activePassVolume.isGlobal)
				{
					activePassVolume.fadeValue = 1f;
					m_OverlappingPassVolumes.Add(activePassVolume);
				}
				else
				{
					if (activePassVolume.m_Colliders.Count == 0)
					{
						continue;
					}
					activePassVolume.m_OverlappingColliders.Clear();
					float num = Mathf.Max(float.Epsilon, activePassVolume.fadeRadius * activePassVolume.fadeRadius);
					float num2 = 1E+20f;
					foreach (Collider collider in activePassVolume.m_Colliders)
					{
						if ((bool)collider && collider.enabled && !(collider is MeshCollider { convex: false }))
						{
							float sqrMagnitude = (collider.ClosestPoint(position) - position).sqrMagnitude;
							num2 = Mathf.Min(num2, sqrMagnitude);
							if (sqrMagnitude <= num)
							{
								activePassVolume.m_OverlappingColliders.Add(collider);
							}
						}
					}
					activePassVolume.fadeValue = 1f - Mathf.Clamp01(Mathf.Sqrt(num2 / num));
					if (activePassVolume.m_OverlappingColliders.Count > 0)
					{
						m_OverlappingPassVolumes.Add(activePassVolume);
					}
				}
			}
			m_OverlappingPassVolumes.Sort(delegate(CustomPassVolume v1, CustomPassVolume v2)
			{
				if (v1.priority == v2.priority)
				{
					if (v1.isGlobal && v2.isGlobal)
					{
						return 0;
					}
					if (v1.isGlobal)
					{
						return 1;
					}
					if (v2.isGlobal)
					{
						return -1;
					}
					return GetVolumeExtent(v1).CompareTo(GetVolumeExtent(v2));
				}
				return v2.priority.CompareTo(v1.priority);
			});
			static float GetVolumeExtent(CustomPassVolume volume)
			{
				float num3 = 0f;
				foreach (Collider overlappingCollider in volume.m_OverlappingColliders)
				{
					num3 += overlappingCollider.bounds.extents.magnitude;
				}
				return num3;
			}
		}

		internal void AggregateCullingParameters(ref ScriptableCullingParameters cullingParameters, HDCamera hdCamera)
		{
			foreach (CustomPass customPass in customPasses)
			{
				if (customPass != null && customPass.enabled)
				{
					customPass.InternalAggregateCullingParameters(ref cullingParameters, hdCamera);
				}
			}
		}

		internal static CullingResults? Cull(ScriptableRenderContext renderContext, HDCamera hdCamera)
		{
			CullingResults? result = null;
			Update(hdCamera);
			hdCamera.camera.TryGetCullingParameters(out var cullingParameters);
			cullingParameters.cullingMask = 0u;
			cullingParameters.cullingOptions = CullingOptions.None;
			foreach (CustomPassVolume overlappingPassVolume in m_OverlappingPassVolumes)
			{
				overlappingPassVolume?.AggregateCullingParameters(ref cullingParameters, hdCamera);
			}
			if (cullingParameters.cullingMask != 0 && (cullingParameters.cullingMask & hdCamera.camera.cullingMask) != cullingParameters.cullingMask)
			{
				result = renderContext.Cull(ref cullingParameters);
			}
			return result;
		}

		internal static void Cleanup()
		{
			foreach (CustomPassVolume activePassVolume in m_ActivePassVolumes)
			{
				activePassVolume.CleanupPasses();
			}
		}

		[Obsolete("In order to support multiple custom pass volume per injection points, please use GetActivePassVolumes.")]
		public static CustomPassVolume GetActivePassVolume(CustomPassInjectionPoint injectionPoint)
		{
			List<CustomPassVolume> list = new List<CustomPassVolume>();
			GetActivePassVolumes(injectionPoint, list);
			return list.FirstOrDefault();
		}

		public static void GetActivePassVolumes(CustomPassInjectionPoint injectionPoint, List<CustomPassVolume> volumes)
		{
			volumes.Clear();
			foreach (CustomPassVolume overlappingPassVolume in m_OverlappingPassVolumes)
			{
				if (overlappingPassVolume.injectionPoint == injectionPoint)
				{
					volumes.Add(overlappingPassVolume);
				}
			}
		}

		public CustomPass AddPassOfType<T>() where T : CustomPass
		{
			return AddPassOfType(typeof(T));
		}

		public CustomPass AddPassOfType(Type passType)
		{
			if (!typeof(CustomPass).IsAssignableFrom(passType))
			{
				Debug.LogError($"Can't add pass type {passType} to the list because it does not inherit from CustomPass.");
				return null;
			}
			CustomPass customPass = Activator.CreateInstance(passType) as CustomPass;
			customPasses.Add(customPass);
			return customPass;
		}
	}
}
