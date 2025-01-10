using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public struct GlobalXRSettings
	{
		public bool singlePass;

		public bool occlusionMesh;

		public bool cameraJitter;

		public bool allowMotionBlur;

		internal static GlobalXRSettings NewDefault()
		{
			GlobalXRSettings result = default(GlobalXRSettings);
			result.singlePass = true;
			result.occlusionMesh = true;
			result.cameraJitter = false;
			result.allowMotionBlur = false;
			return result;
		}
	}
}
