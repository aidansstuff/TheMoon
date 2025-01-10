namespace UnityEngine.Rendering.HighDefinition
{
	internal struct HDLightRenderEntity
	{
		public int entityIndex;

		public static readonly HDLightRenderEntity Invalid = new HDLightRenderEntity
		{
			entityIndex = HDLightRenderDatabase.InvalidDataIndex
		};

		public bool valid => entityIndex != HDLightRenderDatabase.InvalidDataIndex;
	}
}
