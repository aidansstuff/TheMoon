using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[CompilerGenerated]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("Unity.MonoScriptGenerator.MonoScriptInfoGenerator", null)]
internal class UnitySourceGeneratedAssemblyMonoScriptTypes_v1
{
	private struct MonoScriptData
	{
		public byte[] FilePathsData;

		public byte[] TypesData;

		public int TotalTypes;

		public int TotalFiles;

		public bool IsEditorOnly;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static MonoScriptData Get()
	{
		MonoScriptData result = default(MonoScriptData);
		result.FilePathsData = new byte[116]
		{
			0, 0, 0, 1, 0, 0, 0, 108, 92, 65,
			115, 115, 101, 116, 115, 92, 83, 97, 109, 112,
			108, 101, 115, 92, 78, 101, 116, 99, 111, 100,
			101, 32, 102, 111, 114, 32, 71, 97, 109, 101,
			79, 98, 106, 101, 99, 116, 115, 92, 49, 46,
			48, 46, 48, 45, 112, 114, 101, 46, 54, 92,
			67, 108, 105, 101, 110, 116, 78, 101, 116, 119,
			111, 114, 107, 84, 114, 97, 110, 115, 102, 111,
			114, 109, 92, 83, 99, 114, 105, 112, 116, 115,
			92, 67, 108, 105, 101, 110, 116, 78, 101, 116,
			119, 111, 114, 107, 84, 114, 97, 110, 115, 102,
			111, 114, 109, 46, 99, 115
		};
		result.TypesData = new byte[49]
		{
			0, 0, 0, 0, 44, 85, 110, 105, 116, 121,
			46, 78, 101, 116, 99, 111, 100, 101, 46, 83,
			97, 109, 112, 108, 101, 115, 124, 67, 108, 105,
			101, 110, 116, 78, 101, 116, 119, 111, 114, 107,
			84, 114, 97, 110, 115, 102, 111, 114, 109
		};
		result.TotalFiles = 1;
		result.TotalTypes = 1;
		result.IsEditorOnly = false;
		return result;
	}
}
