using System;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	public struct UnsafeText : INativeDisposable, IDisposable, IUTF8Bytes, INativeList<byte>, IIndexable<byte>
	{
		internal UntypedUnsafeList m_UntypedListData;

		public bool IsCreated => this.AsUnsafeListOfBytes().IsCreated;

		public bool IsEmpty
		{
			get
			{
				if (IsCreated)
				{
					return Length == 0;
				}
				return true;
			}
		}

		public unsafe byte this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElement<byte>(m_UntypedListData.Ptr, index);
			}
			set
			{
				UnsafeUtility.WriteArrayElement(m_UntypedListData.Ptr, index, value);
			}
		}

		public int Capacity
		{
			get
			{
				return this.AsUnsafeListOfBytes().Capacity - 1;
			}
			set
			{
				this.AsUnsafeListOfBytes().SetCapacity(value + 1);
			}
		}

		public int Length
		{
			get
			{
				return this.AsUnsafeListOfBytes().Length - 1;
			}
			set
			{
				this.AsUnsafeListOfBytes().Resize(value + 1);
				this.AsUnsafeListOfBytes()[value] = 0;
			}
		}

		public UnsafeText(int capacity, AllocatorManager.AllocatorHandle allocator)
		{
			m_UntypedListData = default(UntypedUnsafeList);
			this.AsUnsafeListOfBytes() = new UnsafeList<byte>(capacity + 1, allocator);
			Length = 0;
		}

		public void Dispose()
		{
			this.AsUnsafeListOfBytes().Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.AsUnsafeListOfBytes().Dispose(inputDeps);
		}

		public unsafe ref byte ElementAt(int index)
		{
			return ref UnsafeUtility.ArrayElementAsRef<byte>(m_UntypedListData.Ptr, index);
		}

		public void Clear()
		{
			Length = 0;
		}

		public unsafe byte* GetUnsafePtr()
		{
			return (byte*)m_UntypedListData.Ptr;
		}

		public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			this.AsUnsafeListOfBytes().Resize(newLength + 1, clearOptions);
			this.AsUnsafeListOfBytes()[newLength] = 0;
			return true;
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			if (!IsCreated)
			{
				return "";
			}
			return FixedStringMethods.ConvertToString(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexInRange(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException($"Index {index} must be positive.");
			}
			if (index >= Length)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range in UnsafeText of {Length} length.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException($"UnsafeText: {error} while copying \"{source}\"");
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCapacityInRange(int value, int length)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException($"Value {value} must be positive.");
			}
			if ((uint)value < (uint)length)
			{
				throw new ArgumentOutOfRangeException($"Value {value} is out of range in NativeList of '{length}' Length.");
			}
		}
	}
}
