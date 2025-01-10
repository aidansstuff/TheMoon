using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public struct UnsafeStream : INativeDisposable, IDisposable
	{
		[BurstCompile]
		private struct DisposeJob : IJob
		{
			public UnsafeStream Container;

			public void Execute()
			{
				Container.Deallocate();
			}
		}

		[BurstCompile]
		private struct ConstructJobList : IJob
		{
			public UnsafeStream Container;

			[ReadOnly]
			[NativeDisableUnsafePtrRestriction]
			public unsafe UntypedUnsafeList* List;

			public unsafe void Execute()
			{
				Container.AllocateForEach(List->m_length);
			}
		}

		[BurstCompile]
		private struct ConstructJob : IJob
		{
			public UnsafeStream Container;

			[ReadOnly]
			public NativeArray<int> Length;

			public void Execute()
			{
				Container.AllocateForEach(Length[0]);
			}
		}

		[BurstCompatible]
		public struct Writer
		{
			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeStreamBlockData* m_BlockStream;

			[NativeDisableUnsafePtrRestriction]
			private unsafe UnsafeStreamBlock* m_CurrentBlock;

			[NativeDisableUnsafePtrRestriction]
			private unsafe byte* m_CurrentPtr;

			[NativeDisableUnsafePtrRestriction]
			private unsafe byte* m_CurrentBlockEnd;

			internal int m_ForeachIndex;

			private int m_ElementCount;

			[NativeDisableUnsafePtrRestriction]
			private unsafe UnsafeStreamBlock* m_FirstBlock;

			private int m_FirstOffset;

			private int m_NumberOfBlocks;

			[NativeSetThreadIndex]
			private int m_ThreadIndex;

			public unsafe int ForEachCount => m_BlockStream->RangeCount;

			internal unsafe Writer(ref UnsafeStream stream)
			{
				m_BlockStream = stream.m_Block;
				m_ForeachIndex = int.MinValue;
				m_ElementCount = -1;
				m_CurrentBlock = null;
				m_CurrentBlockEnd = null;
				m_CurrentPtr = null;
				m_FirstBlock = null;
				m_NumberOfBlocks = 0;
				m_FirstOffset = 0;
				m_ThreadIndex = 0;
			}

			public unsafe void BeginForEachIndex(int foreachIndex)
			{
				m_ForeachIndex = foreachIndex;
				m_ElementCount = 0;
				m_NumberOfBlocks = 0;
				m_FirstBlock = m_CurrentBlock;
				m_FirstOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
			}

			public unsafe void EndForEachIndex()
			{
				m_BlockStream->Ranges[m_ForeachIndex].ElementCount = m_ElementCount;
				m_BlockStream->Ranges[m_ForeachIndex].OffsetInFirstBlock = m_FirstOffset;
				m_BlockStream->Ranges[m_ForeachIndex].Block = m_FirstBlock;
				m_BlockStream->Ranges[m_ForeachIndex].LastOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
				m_BlockStream->Ranges[m_ForeachIndex].NumberOfBlocks = m_NumberOfBlocks;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public void Write<T>(T value) where T : struct
			{
				Allocate<T>() = value;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Allocate<T>() where T : struct
			{
				int size = UnsafeUtility.SizeOf<T>();
				return ref UnsafeUtility.AsRef<T>(Allocate(size));
			}

			public unsafe byte* Allocate(int size)
			{
				byte* currentPtr = m_CurrentPtr;
				m_CurrentPtr += size;
				if (m_CurrentPtr > m_CurrentBlockEnd)
				{
					UnsafeStreamBlock* currentBlock = m_CurrentBlock;
					m_CurrentBlock = m_BlockStream->Allocate(currentBlock, m_ThreadIndex);
					m_CurrentPtr = m_CurrentBlock->Data;
					if (m_FirstBlock == null)
					{
						m_FirstOffset = (int)(m_CurrentPtr - (byte*)m_CurrentBlock);
						m_FirstBlock = m_CurrentBlock;
					}
					else
					{
						m_NumberOfBlocks++;
					}
					m_CurrentBlockEnd = (byte*)m_CurrentBlock + 4096;
					currentPtr = m_CurrentPtr;
					m_CurrentPtr += size;
				}
				m_ElementCount++;
				return currentPtr;
			}
		}

		[BurstCompatible]
		public struct Reader
		{
			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeStreamBlockData* m_BlockStream;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe UnsafeStreamBlock* m_CurrentBlock;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe byte* m_CurrentPtr;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe byte* m_CurrentBlockEnd;

			internal int m_RemainingItemCount;

			internal int m_LastBlockSize;

			public unsafe int ForEachCount => m_BlockStream->RangeCount;

			public int RemainingItemCount => m_RemainingItemCount;

			internal unsafe Reader(ref UnsafeStream stream)
			{
				m_BlockStream = stream.m_Block;
				m_CurrentBlock = null;
				m_CurrentPtr = null;
				m_CurrentBlockEnd = null;
				m_RemainingItemCount = 0;
				m_LastBlockSize = 0;
			}

			public unsafe int BeginForEachIndex(int foreachIndex)
			{
				m_RemainingItemCount = m_BlockStream->Ranges[foreachIndex].ElementCount;
				m_LastBlockSize = m_BlockStream->Ranges[foreachIndex].LastOffset;
				m_CurrentBlock = m_BlockStream->Ranges[foreachIndex].Block;
				m_CurrentPtr = (byte*)m_CurrentBlock + m_BlockStream->Ranges[foreachIndex].OffsetInFirstBlock;
				m_CurrentBlockEnd = (byte*)m_CurrentBlock + 4096;
				return m_RemainingItemCount;
			}

			public void EndForEachIndex()
			{
			}

			public unsafe byte* ReadUnsafePtr(int size)
			{
				m_RemainingItemCount--;
				byte* currentPtr = m_CurrentPtr;
				m_CurrentPtr += size;
				if (m_CurrentPtr > m_CurrentBlockEnd)
				{
					m_CurrentBlock = m_CurrentBlock->Next;
					m_CurrentPtr = m_CurrentBlock->Data;
					m_CurrentBlockEnd = (byte*)m_CurrentBlock + 4096;
					currentPtr = m_CurrentPtr;
					m_CurrentPtr += size;
				}
				return currentPtr;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Read<T>() where T : struct
			{
				int size = UnsafeUtility.SizeOf<T>();
				return ref UnsafeUtility.AsRef<T>(ReadUnsafePtr(size));
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe ref T Peek<T>() where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				byte* ptr = m_CurrentPtr;
				if (ptr + num > m_CurrentBlockEnd)
				{
					ptr = m_CurrentBlock->Next->Data;
				}
				return ref UnsafeUtility.AsRef<T>(ptr);
			}

			public unsafe int Count()
			{
				int num = 0;
				for (int i = 0; i != m_BlockStream->RangeCount; i++)
				{
					num += m_BlockStream->Ranges[i].ElementCount;
				}
				return num;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeStreamBlockData* m_Block;

		internal AllocatorManager.AllocatorHandle m_Allocator;

		public unsafe bool IsCreated => m_Block != null;

		public unsafe int ForEachCount => m_Block->RangeCount;

		public UnsafeStream(int bufferCount, AllocatorManager.AllocatorHandle allocator)
		{
			AllocateBlock(out this, allocator);
			AllocateForEach(bufferCount);
		}

		[NotBurstCompatible]
		public unsafe static JobHandle ScheduleConstruct<T>(out UnsafeStream stream, NativeList<T> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator) where T : unmanaged
		{
			AllocateBlock(out stream, allocator);
			ConstructJobList jobData = default(ConstructJobList);
			jobData.List = (UntypedUnsafeList*)bufferCount.GetUnsafeList();
			jobData.Container = stream;
			return IJobExtensions.Schedule(jobData, dependency);
		}

		[NotBurstCompatible]
		public static JobHandle ScheduleConstruct(out UnsafeStream stream, NativeArray<int> bufferCount, JobHandle dependency, AllocatorManager.AllocatorHandle allocator)
		{
			AllocateBlock(out stream, allocator);
			ConstructJob jobData = default(ConstructJob);
			jobData.Length = bufferCount;
			jobData.Container = stream;
			return IJobExtensions.Schedule(jobData, dependency);
		}

		internal unsafe static void AllocateBlock(out UnsafeStream stream, AllocatorManager.AllocatorHandle allocator)
		{
			int num = 128;
			int num2 = sizeof(UnsafeStreamBlockData) + sizeof(UnsafeStreamBlock*) * num;
			byte* ptr = (byte*)Memory.Unmanaged.Allocate(num2, 16, allocator);
			UnsafeUtility.MemClear(ptr, num2);
			UnsafeStreamBlockData* ptr2 = (stream.m_Block = (UnsafeStreamBlockData*)ptr);
			stream.m_Allocator = allocator;
			ptr2->Allocator = allocator;
			ptr2->BlockCount = num;
			ptr2->Blocks = (UnsafeStreamBlock**)(ptr + sizeof(UnsafeStreamBlockData));
			ptr2->Ranges = null;
			ptr2->RangeCount = 0;
		}

		internal unsafe void AllocateForEach(int forEachCount)
		{
			long size = sizeof(UnsafeStreamRange) * forEachCount;
			m_Block->Ranges = (UnsafeStreamRange*)Memory.Unmanaged.Allocate(size, 16, m_Allocator);
			m_Block->RangeCount = forEachCount;
			UnsafeUtility.MemClear(m_Block->Ranges, size);
		}

		public unsafe bool IsEmpty()
		{
			if (!IsCreated)
			{
				return true;
			}
			for (int i = 0; i != m_Block->RangeCount; i++)
			{
				if (m_Block->Ranges[i].ElementCount > 0)
				{
					return false;
				}
			}
			return true;
		}

		public Reader AsReader()
		{
			return new Reader(ref this);
		}

		public Writer AsWriter()
		{
			return new Writer(ref this);
		}

		public unsafe int Count()
		{
			int num = 0;
			for (int i = 0; i != m_Block->RangeCount; i++)
			{
				num += m_Block->Ranges[i].ElementCount;
			}
			return num;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public NativeArray<T> ToNativeArray<T>(AllocatorManager.AllocatorHandle allocator) where T : struct
		{
			NativeArray<T> result = CollectionHelper.CreateNativeArray<T>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
			Reader reader = AsReader();
			int num = 0;
			for (int i = 0; i != reader.ForEachCount; i++)
			{
				reader.BeginForEachIndex(i);
				int remainingItemCount = reader.RemainingItemCount;
				for (int j = 0; j < remainingItemCount; j++)
				{
					result[num] = reader.Read<T>();
					num++;
				}
				reader.EndForEachIndex();
			}
			return result;
		}

		private unsafe void Deallocate()
		{
			if (m_Block == null)
			{
				return;
			}
			for (int i = 0; i != m_Block->BlockCount; i++)
			{
				UnsafeStreamBlock* ptr = m_Block->Blocks[i];
				while (ptr != null)
				{
					UnsafeStreamBlock* next = ptr->Next;
					Memory.Unmanaged.Free(ptr, m_Allocator);
					ptr = next;
				}
			}
			Memory.Unmanaged.Free(m_Block->Ranges, m_Allocator);
			Memory.Unmanaged.Free(m_Block, m_Allocator);
			m_Block = null;
			m_Allocator = Allocator.None;
		}

		public void Dispose()
		{
			Deallocate();
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			DisposeJob jobData = default(DisposeJob);
			jobData.Container = this;
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_Block = null;
			return result;
		}
	}
}
