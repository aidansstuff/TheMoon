using System.Linq;

namespace Unity.Multiplayer.Tools.Common
{
	internal static class StringUtil
	{
		internal static string AddSpacesToCamelCase(string s)
		{
			return string.Concat(s.Select((char x) => (!char.IsUpper(x)) ? x.ToString() : (" " + x))).TrimStart(' ');
		}
	}
}
