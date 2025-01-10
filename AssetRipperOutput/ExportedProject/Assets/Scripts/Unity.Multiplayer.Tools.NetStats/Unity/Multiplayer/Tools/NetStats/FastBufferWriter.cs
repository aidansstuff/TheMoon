using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal struct FastBufferWriter : IDisposable
	{
		internal struct WriterHandle
		{
			internal unsafe byte* BufferPointer;

			internal int Position;

			internal int Length;

			internal int Capacity;

			internal int MaxCapacity;

			internal Allocator Allocator;

			internal bool BufferGrew;
		}

		internal unsafe readonly WriterHandle* Handle;

		public unsafe int Position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Handle->Position;
			}
		}

		public unsafe int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Handle->Capacity;
			}
		}

		public unsafe int MaxCapacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Handle->MaxCapacity;
			}
		}

		public unsafe int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (Handle->Position <= Handle->Length)
				{
					return Handle->Length;
				}
				return Handle->Position;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void CommitBitwiseWrites(int amount)
		{
			Handle->Position += amount;
		}

		public unsafe FastBufferWriter(int size, Allocator allocator, int maxSize = -1)
		{
			Handle = (WriterHandle*)UnsafeUtility.Malloc(sizeof(WriterHandle) + size, UnsafeUtility.AlignOf<WriterHandle>(), allocator);
			Handle->BufferPointer = (byte*)(Handle + 1);
			Handle->Position = 0;
			Handle->Length = 0;
			Handle->Capacity = size;
			Handle->Allocator = allocator;
			Handle->MaxCapacity = ((maxSize < size) ? size : maxSize);
			Handle->BufferGrew = false;
		}

		public unsafe void Dispose()
		{
			if (Handle->BufferGrew)
			{
				UnsafeUtility.Free(Handle->BufferPointer, Handle->Allocator);
			}
			UnsafeUtility.Free(Handle, Handle->Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Seek(int where)
		{
			where = Math.Min(where, Handle->Capacity);
			if (Handle->Position > Handle->Length && where < Handle->Position)
			{
				Handle->Length = Handle->Position;
			}
			Handle->Position = where;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Truncate(int where = -1)
		{
			if (where == -1)
			{
				where = Position;
			}
			if (Handle->Position > where)
			{
				Handle->Position = where;
			}
			if (Handle->Length > where)
			{
				Handle->Length = where;
			}
		}

		public BitWriter EnterBitwiseContext()
		{
			return new BitWriter(this);
		}

		internal unsafe void Grow(int additionalSizeRequired)
		{
			int num;
			for (num = Handle->Capacity * 2; num < Position + additionalSizeRequired; num *= 2)
			{
			}
			int num2 = Math.Min(num, Handle->MaxCapacity);
			byte* ptr = (byte*)UnsafeUtility.Malloc(num2, UnsafeUtility.AlignOf<byte>(), Handle->Allocator);
			UnsafeUtility.MemCpy(ptr, Handle->BufferPointer, Length);
			if (Handle->BufferGrew)
			{
				UnsafeUtility.Free(Handle->BufferPointer, Handle->Allocator);
			}
			Handle->BufferGrew = true;
			Handle->BufferPointer = ptr;
			Handle->Capacity = num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryBeginWrite(int bytes)
		{
			if (Handle->Position + bytes > Handle->Capacity)
			{
				if (Handle->Position + bytes > Handle->MaxCapacity)
				{
					return false;
				}
				if (Handle->Capacity >= Handle->MaxCapacity)
				{
					return false;
				}
				Grow(bytes);
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryBeginWriteValue<T>(in T value) where T : unmanaged
		{
			int num = sizeof(T);
			if (Handle->Position + num > Handle->Capacity)
			{
				if (Handle->Position + num > Handle->MaxCapacity)
				{
					return false;
				}
				if (Handle->Capacity >= Handle->MaxCapacity)
				{
					return false;
				}
				Grow(num);
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryBeginWriteInternal(int bytes)
		{
			if (Handle->Position + bytes > Handle->Capacity)
			{
				if (Handle->Position + bytes > Handle->MaxCapacity)
				{
					return false;
				}
				if (Handle->Capacity >= Handle->MaxCapacity)
				{
					return false;
				}
				Grow(bytes);
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe byte[] ToArray()
		{
			byte[] array = new byte[Length];
			fixed (byte* destination = array)
			{
				UnsafeUtility.MemCpy(destination, Handle->BufferPointer, Length);
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe byte* GetUnsafePtr()
		{
			return Handle->BufferPointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe byte* GetUnsafePtrAtCurrentPosition()
		{
			return Handle->BufferPointer + Handle->Position;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetWriteSize(string s, bool oneByteChars = false)
		{
			return 4 + s.Length * (oneByteChars ? 1 : 2);
		}

		public void WriteNetworkSerializable<T>(in T value) where T : INetworkSerializable
		{
			BufferSerializer<BufferSerializerWriter> serializer = new BufferSerializer<BufferSerializerWriter>(new BufferSerializerWriter(this));
			value.NetworkSerialize(serializer);
		}

		public void WriteNetworkSerializable<T>(INetworkSerializable[] array, int count = -1, int offset = 0) where T : INetworkSerializable
		{
			int value = ((count != -1) ? count : (array.Length - offset));
			WriteValueSafe(in value);
			for (int i = 0; i < array.Length; i++)
			{
				INetworkSerializable value2 = array[i];
				WriteNetworkSerializable(in value2);
			}
		}

		public unsafe void WriteValue(string s, bool oneByteChars = false)
		{
			uint value = (uint)s.Length;
			WriteValue(in value);
			int length = s.Length;
			if (oneByteChars)
			{
				for (int i = 0; i < length; i++)
				{
					WriteByte((byte)s[i]);
				}
			}
			else
			{
				fixed (char* value2 = s)
				{
					WriteBytes((byte*)value2, length * 2);
				}
			}
		}

		public unsafe void WriteValueSafe(string s, bool oneByteChars = false)
		{
			int writeSize = GetWriteSize(s, oneByteChars);
			if (!TryBeginWriteInternal(writeSize))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			uint value = (uint)s.Length;
			WriteValue(in value);
			int length = s.Length;
			if (oneByteChars)
			{
				for (int i = 0; i < length; i++)
				{
					WriteByte((byte)s[i]);
				}
			}
			else
			{
				fixed (char* value2 = s)
				{
					WriteBytes((byte*)value2, length * 2);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetWriteSize<T>(T[] array, int count = -1, int offset = 0) where T : unmanaged
		{
			int num = ((count != -1) ? count : (array.Length - offset)) * sizeof(T);
			return 4 + num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteValue<T>(T[] array, int count = -1, int offset = 0) where T : unmanaged
		{
			int value = ((count != -1) ? count : (array.Length - offset));
			int size = value * sizeof(T);
			WriteValue(in value);
			fixed (T* ptr = array)
			{
				byte* value2 = (byte*)(ptr + offset);
				WriteBytes(value2, size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteValueSafe<T>(T[] array, int count = -1, int offset = 0) where T : unmanaged
		{
			int value = ((count != -1) ? count : (array.Length - offset));
			int num = value * sizeof(T);
			if (!TryBeginWriteInternal(num + 4))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			WriteValue(in value);
			fixed (T* ptr = array)
			{
				byte* value2 = (byte*)(ptr + offset);
				WriteBytes(value2, num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WritePartialValue<T>(T value, int bytesToWrite, int offsetBytes = 0) where T : unmanaged
		{
			byte* source = (byte*)(&value) + offsetBytes;
			UnsafeUtility.MemCpy(Handle->BufferPointer + Handle->Position, source, bytesToWrite);
			Handle->Position += bytesToWrite;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteByte(byte value)
		{
			Handle->BufferPointer[Handle->Position++] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteByteSafe(byte value)
		{
			if (!TryBeginWriteInternal(1))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			Handle->BufferPointer[Handle->Position++] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytes(byte* value, int size, int offset = 0)
		{
			UnsafeUtility.MemCpy(Handle->BufferPointer + Handle->Position, value + offset, size);
			Handle->Position += size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytesSafe(byte* value, int size, int offset = 0)
		{
			if (!TryBeginWriteInternal(size))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			UnsafeUtility.MemCpy(Handle->BufferPointer + Handle->Position, value + offset, size);
			Handle->Position += size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytes(byte[] value, int size = -1, int offset = 0)
		{
			fixed (byte* value2 = value)
			{
				WriteBytes(value2, (size == -1) ? value.Length : size, offset);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytesSafe(byte[] value, int size = -1, int offset = 0)
		{
			fixed (byte* value2 = value)
			{
				WriteBytesSafe(value2, (size == -1) ? value.Length : size, offset);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void CopyTo(FastBufferWriter other)
		{
			other.WriteBytes(Handle->BufferPointer, Handle->Position);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void CopyFrom(FastBufferWriter other)
		{
			WriteBytes(other.Handle->BufferPointer, other.Handle->Position);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int GetWriteSize<T>(in T value) where T : unmanaged
		{
			return sizeof(T);
		}

		public unsafe static int GetWriteSize<T>() where T : unmanaged
		{
			return sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteValue<T>(in T value) where T : unmanaged
		{
			int num = sizeof(T);
			fixed (T* source = &value)
			{
				UnsafeUtility.MemCpy(Handle->BufferPointer + Handle->Position, source, num);
			}
			Handle->Position += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteValueSafe<T>(in T value) where T : unmanaged
		{
			int num = sizeof(T);
			if (!TryBeginWriteInternal(num))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			fixed (T* source = &value)
			{
				UnsafeUtility.MemCpy(Handle->BufferPointer + Handle->Position, source, num);
			}
			Handle->Position += num;
		}

		public unsafe NativeArray<byte> ToNativeArray(Allocator allocator)
		{
			NativeArray<byte> nativeArray = new NativeArray<byte>(Length, allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), GetUnsafePtr(), nativeArray.Length);
			return nativeArray;
		}
	}
}
