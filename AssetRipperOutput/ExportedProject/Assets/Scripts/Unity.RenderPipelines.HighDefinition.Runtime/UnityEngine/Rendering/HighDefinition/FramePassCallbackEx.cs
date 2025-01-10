using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public delegate void FramePassCallbackEx(CommandBuffer cmd, List<RTHandle> buffers, List<RTHandle> customPassbuffers, RenderOutputProperties outputProperties);
}
