using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	internal sealed class WordStorageStatic
	{
		public struct Thing
		{
			public WordStorage Data;
		}

		public static Thing Ref;

		private WordStorageStatic()
		{
		}
	}
}
