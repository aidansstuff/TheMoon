namespace UnityEngine.Rendering.HighDefinition
{
	public class CustomPassAOVBuffers
	{
		public enum OutputType
		{
			CustomPassBuffer = 0,
			Camera = 1
		}

		public CustomPassInjectionPoint injectionPoint;

		public OutputType outputType;

		public CustomPassAOVBuffers(CustomPassInjectionPoint injectionPoint, OutputType outputType)
		{
			this.injectionPoint = injectionPoint;
			this.outputType = outputType;
		}
	}
}
