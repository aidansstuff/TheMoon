using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Conditional("UNITY_EDITOR")]
	internal class HDRPHelpURLAttribute : CoreRPHelpURLAttribute
	{
		public HDRPHelpURLAttribute(string pageName)
			: base(pageName, "com.unity.render-pipelines.high-definition")
		{
		}
	}
}
