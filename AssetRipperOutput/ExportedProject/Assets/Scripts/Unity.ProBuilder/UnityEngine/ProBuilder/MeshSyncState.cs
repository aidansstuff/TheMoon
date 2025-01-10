using System;

namespace UnityEngine.ProBuilder
{
	public enum MeshSyncState
	{
		Null = 0,
		[Obsolete("InstanceIDMismatch is no longer used. Mesh references are not tracked by Instance ID.")]
		InstanceIDMismatch = 1,
		Lightmap = 2,
		InSync = 3,
		NeedsRebuild = 4
	}
}
