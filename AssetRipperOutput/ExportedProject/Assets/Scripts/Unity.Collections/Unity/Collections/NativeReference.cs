using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeReference<T> : INativeDisposable, IDisposable, IEquatable<NativeReference<T>> where T : unmanaged
	{
		[NativeContainer]
		[NativeContainerIsReadOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ReadOnly
		{
			[NativeDisableUnsafePtrRestriction]
			private unsafe readonly void* m_Data;

			public unsafe T Value => *(T*)m_Data;

			internal unsafe ReadOnly(void* data)
			{
				m_Data = data;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe void* m_Data;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		public unsafe T Value
		{
			get
			{
				return *(T*)m_Data;
			}
			set
			{
				*(T*)m_Data = value;
			}
		}

		public unsafe bool IsCreated => m_Data != null;

		public unsafe NativeReference(AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			Allocate(allocator, out this);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(m_Data, UnsafeUtility.SizeOf<T>());
			}
		}

		public unsafe NativeReference(T value, AllocatorManager.AllocatorHandle allocator)
		{
			Allocate(allocator, out this);
			*(T*)m_Data = value;
		}

		private unsafe static void Allocate(AllocatorManager.AllocatorHandle allocator, out NativeReference<T> reference)
		{
			reference = default(NativeReference<T>);
			reference.m_Data = Memory.Unmanaged.Allocate(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
			reference.m_AllocatorLabel = allocator;
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(m_AllocatorLabel))
			{
				Memory.Unmanaged.Free(m_Data, m_AllocatorLabel);
				m_AllocatorLabel = Allocator.Invalid;
			}
			m_Data = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(m_AllocatorLabel))
			{
				NativeReferenceDisposeJob jobData = default(NativeReferenceDisposeJob);
				jobData.Data = new NativeReferenceDispose
				{
					m_Data = m_Data,
					m_AllocatorLabel = m_AllocatorLabel
				};
				JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
				m_Data = null;
				m_AllocatorLabel = Allocator.Invalid;
				return result;
			}
			m_Data = null;
			return inputDeps;
		}

		public void CopyFrom(NativeReference<T> reference)
		{
			Copy(this, reference);
		}

		public void CopyTo(NativeReference<T> reference)
		{
			Copy(reference, this);
		}

		[NotBurstCompatible]
		public bool Equals(NativeReference<T> other)
		{
			return Value.Equals(other.Value);
		}

		[NotBurstCompatible]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is NativeReference<T>)
			{
				return Equals((NativeReference<T>)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public static bool operator ==(NativeReference<T> left, NativeReference<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(NativeReference<T> left, NativeReference<T> right)
		{
			return !left.Equals(right);
		}

		public unsafe static void Copy(NativeReference<T> dst, NativeReference<T> src)
		{
			UnsafeUtility.MemCpy(dst.m_Data, src.m_Data, UnsafeUtility.SizeOf<T>());
		}

		public unsafe ReadOnly AsReadOnly()
		{
			return new ReadOnly(m_Data);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe void CheckNotDisposed()
		{
			if (m_Data == null)
			{
				throw new ObjectDisposedException("The NativeReference is already disposed.");
			}
		}
	}
}
