using System;
using System.Diagnostics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	[DebuggerTypeProxy(typeof(WordStorageDebugView))]
	public struct WordStorage
	{
		private struct Entry
		{
			public int offset;

			public int length;
		}

		private NativeArray<byte> buffer;

		private NativeArray<Entry> entry;

		private NativeParallelMultiHashMap<int, int> hash;

		private int chars;

		private int entries;

		private const int kMaxEntries = 16384;

		private const int kMaxChars = 2097152;

		public const int kMaxCharsPerEntry = 4096;

		[NotBurstCompatible]
		public static ref WordStorage Instance
		{
			get
			{
				Initialize();
				return ref WordStorageStatic.Ref.Data;
			}
		}

		public int Entries => entries;

		[NotBurstCompatible]
		public static void Initialize()
		{
			if (!WordStorageStatic.Ref.Data.buffer.IsCreated)
			{
				WordStorageStatic.Ref.Data.buffer = new NativeArray<byte>(2097152, Allocator.Persistent);
				WordStorageStatic.Ref.Data.entry = new NativeArray<Entry>(16384, Allocator.Persistent);
				WordStorageStatic.Ref.Data.hash = new NativeParallelMultiHashMap<int, int>(16384, Allocator.Persistent);
				Clear();
				AppDomain.CurrentDomain.DomainUnload += delegate
				{
					Shutdown();
				};
				AppDomain.CurrentDomain.ProcessExit += delegate
				{
					Shutdown();
				};
			}
		}

		[NotBurstCompatible]
		public static void Shutdown()
		{
			if (WordStorageStatic.Ref.Data.buffer.IsCreated)
			{
				WordStorageStatic.Ref.Data.buffer.Dispose();
				WordStorageStatic.Ref.Data.entry.Dispose();
				WordStorageStatic.Ref.Data.hash.Dispose();
				WordStorageStatic.Ref.Data = default(WordStorage);
			}
		}

		[NotBurstCompatible]
		public static void Clear()
		{
			Initialize();
			WordStorageStatic.Ref.Data.chars = 0;
			WordStorageStatic.Ref.Data.entries = 0;
			WordStorageStatic.Ref.Data.hash.Clear();
			FixedString32Bytes value = default(FixedString32Bytes);
			WordStorageStatic.Ref.Data.GetOrCreateIndex(ref value);
		}

		[NotBurstCompatible]
		public static void Setup()
		{
			Clear();
		}

		public unsafe void GetFixedString<T>(int index, ref T temp) where T : IUTF8Bytes, INativeList<byte>
		{
			Entry entry = this.entry[index];
			temp.Length = entry.length;
			UnsafeUtility.MemCpy(temp.GetUnsafePtr(), (byte*)buffer.GetUnsafePtr() + entry.offset, temp.Length);
		}

		public int GetIndexFromHashAndFixedString<T>(int h, ref T temp) where T : IUTF8Bytes, INativeList<byte>
		{
			if (hash.TryGetFirstValue(h, out var item, out var it))
			{
				do
				{
					Entry entry = this.entry[item];
					if (entry.length == temp.Length)
					{
						int i;
						for (i = 0; i < entry.length && temp[i] == buffer[entry.offset + i]; i++)
						{
						}
						if (i == temp.Length)
						{
							return item;
						}
					}
				}
				while (hash.TryGetNextValue(out item, ref it));
			}
			return -1;
		}

		public bool Contains<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int hashCode = value.GetHashCode();
			return GetIndexFromHashAndFixedString(hashCode, ref value) != -1;
		}

		[NotBurstCompatible]
		public bool Contains(string value)
		{
			FixedString512Bytes value2 = value;
			return Contains(ref value2);
		}

		public int GetOrCreateIndex<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int hashCode = value.GetHashCode();
			int indexFromHashAndFixedString = GetIndexFromHashAndFixedString(hashCode, ref value);
			if (indexFromHashAndFixedString != -1)
			{
				return indexFromHashAndFixedString;
			}
			int offset = chars;
			ushort num = (ushort)value.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[chars++] = value[i];
			}
			entry[entries] = new Entry
			{
				offset = offset,
				length = num
			};
			hash.Add(hashCode, entries);
			return entries++;
		}
	}
}
