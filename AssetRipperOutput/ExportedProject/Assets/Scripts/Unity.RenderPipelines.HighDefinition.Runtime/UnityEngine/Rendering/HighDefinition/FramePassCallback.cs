using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public delegate void FramePassCallback(CommandBuffer cmd, List<RTHandle> buffers, RenderOutputProperties outputProperties);
}
