using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	internal sealed class WordStorageDebugView
	{
		private WordStorage m_wordStorage;

		public FixedString128Bytes[] Table
		{
			get
			{
				FixedString128Bytes[] array = new FixedString128Bytes[m_wordStorage.Entries];
				for (int i = 0; i < m_wordStorage.Entries; i++)
				{
					m_wordStorage.GetFixedString(i, ref array[i]);
				}
				return array;
			}
		}

		public WordStorageDebugView(WordStorage wordStorage)
		{
			m_wordStorage = wordStorage;
		}
	}
}
