using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public class HDVolumeDebugSettings : VolumeDebugSettings<HDAdditionalCameraData>
	{
		public override Type targetRenderPipeline => typeof(HDRenderPipeline);

		public override VolumeStack selectedCameraVolumeStack
		{
			get
			{
				Camera camera = base.selectedCamera;
				if (camera == null)
				{
					return null;
				}
				VolumeStack volumeStack = HDCamera.GetOrCreate(camera).volumeStack;
				if (volumeStack != null)
				{
					return volumeStack;
				}
				return VolumeManager.instance.stack;
			}
		}

		public override LayerMask selectedCameraLayerMask
		{
			get
			{
				if (base.selectedCamera == null)
				{
					return 0;
				}
				return base.selectedCamera.GetComponent<HDAdditionalCameraData>().volumeLayerMask;
			}
		}

		public override Vector3 selectedCameraPosition
		{
			get
			{
				Camera camera = base.selectedCamera;
				if (camera == null)
				{
					return Vector3.zero;
				}
				Transform transform = HDCamera.GetOrCreate(camera).volumeAnchor;
				if (transform == null)
				{
					if (camera.TryGetComponent<HDAdditionalCameraData>(out var component))
					{
						transform = component.volumeAnchorOverride;
					}
					if (transform == null)
					{
						transform = camera.transform;
					}
					VolumeStack volumeStack = selectedCameraVolumeStack;
					if (volumeStack != null)
					{
						VolumeManager.instance.Update(volumeStack, transform, selectedCameraLayerMask);
					}
				}
				return transform.position;
			}
		}
	}
}
