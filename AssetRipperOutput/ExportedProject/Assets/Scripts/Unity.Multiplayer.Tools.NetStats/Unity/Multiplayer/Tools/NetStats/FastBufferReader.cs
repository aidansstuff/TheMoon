using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Multiplayer.Tools.NetStats
{
	internal struct FastBufferReader : IDisposable
	{
		internal struct ReaderHandle
		{
			internal unsafe byte* BufferPointer;

			internal int Position;

			internal int Length;

			internal Allocator Allocator;
		}

		internal unsafe readonly ReaderHandle* Handle;

		public unsafe int Position
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Handle->Position;
			}
		}

		public unsafe int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Handle->Length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void CommitBitwiseReads(int amount)
		{
			Handle->Position += amount;
		}

		private unsafe static ReaderHandle* CreateHandle(byte* buffer, int length, int offset, Allocator allocator)
		{
			ReaderHandle* ptr = null;
			if (allocator == Allocator.None)
			{
				ptr = (ReaderHandle*)UnsafeUtility.Malloc(sizeof(ReaderHandle) + length, UnsafeUtility.AlignOf<byte>(), Allocator.Temp);
				ptr->BufferPointer = buffer;
				ptr->Position = offset;
			}
			else
			{
				ptr = (ReaderHandle*)UnsafeUtility.Malloc(sizeof(ReaderHandle) + length, UnsafeUtility.AlignOf<byte>(), allocator);
				UnsafeUtility.MemCpy(ptr + 1, buffer + offset, length);
				ptr->BufferPointer = (byte*)(ptr + 1);
				ptr->Position = 0;
			}
			ptr->Length = length;
			ptr->Allocator = allocator;
			return ptr;
		}

		public unsafe FastBufferReader(NativeArray<byte> buffer, Allocator allocator, int length = -1, int offset = 0)
		{
			Handle = CreateHandle((byte*)buffer.GetUnsafePtr(), Math.Max(1, (length == -1) ? buffer.Length : length), offset, allocator);
		}

		public unsafe FastBufferReader(ArraySegment<byte> buffer, Allocator allocator, int length = -1, int offset = 0)
		{
			if (allocator == Allocator.None)
			{
				throw new NotSupportedException("Allocator.None cannot be used with managed source buffers.");
			}
			fixed (byte* buffer2 = buffer.Array)
			{
				Handle = CreateHandle(buffer2, Math.Max(1, (length == -1) ? buffer.Count : length), offset, allocator);
			}
		}

		public unsafe FastBufferReader(byte[] buffer, Allocator allocator, int length = -1, int offset = 0)
		{
			if (allocator == Allocator.None)
			{
				throw new NotSupportedException("Allocator.None cannot be used with managed source buffers.");
			}
			fixed (byte* buffer2 = buffer)
			{
				Handle = CreateHandle(buffer2, Math.Max(1, (length == -1) ? buffer.Length : length), offset, allocator);
			}
		}

		public unsafe FastBufferReader(byte* buffer, Allocator allocator, int length, int offset = 0)
		{
			Handle = CreateHandle(buffer, Math.Max(1, length), offset, allocator);
		}

		public unsafe FastBufferReader(FastBufferWriter writer, Allocator allocator, int length = -1, int offset = 0)
		{
			Handle = CreateHandle(writer.GetUnsafePtr(), Math.Max(1, (length == -1) ? writer.Length : length), offset, allocator);
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.Free(Handle, Handle->Allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Seek(int where)
		{
			Handle->Position = Math.Min(Length, where);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void MarkBytesRead(int amount)
		{
			Handle->Position += amount;
		}

		public BitReader EnterBitwiseContext()
		{
			return new BitReader(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryBeginRead(int bytes)
		{
			if (Handle->Position + bytes > Handle->Length)
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryBeginReadValue<T>(in T value) where T : unmanaged
		{
			int num = sizeof(T);
			if (Handle->Position + num > Handle->Length)
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool TryBeginReadInternal(int bytes)
		{
			if (Handle->Position + bytes > Handle->Length)
			{
				return false;
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

		public void ReadNetworkSerializable<T>(out T value) where T : INetworkSerializable, new()
		{
			value = new T();
			BufferSerializer<BufferSerializerReader> serializer = new BufferSerializer<BufferSerializerReader>(new BufferSerializerReader(this));
			value.NetworkSerialize(serializer);
		}

		public void ReadNetworkSerializable<T>(out T[] value) where T : INetworkSerializable, new()
		{
			ReadValueSafe(out int value2);
			value = new T[value2];
			for (int i = 0; i < value2; i++)
			{
				ReadNetworkSerializable(out value[i]);
			}
		}

		public unsafe void ReadValue(out string s, bool oneByteChars = false)
		{
			ReadValue(out uint value);
			s = "".PadRight((int)value);
			int length = s.Length;
			fixed (char* ptr = s)
			{
				if (oneByteChars)
				{
					for (int i = 0; i < length; i++)
					{
						ReadByte(out var value2);
						ptr[i] = (char)value2;
					}
				}
				else
				{
					ReadBytes((byte*)ptr, length * 2);
				}
			}
		}

		public unsafe void ReadValueSafe(out string s, bool oneByteChars = false)
		{
			if (!TryBeginReadInternal(4))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			ReadValue(out uint value);
			if (!TryBeginReadInternal((int)value * (oneByteChars ? 1 : 2)))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			s = "".PadRight((int)value);
			int length = s.Length;
			fixed (char* ptr = s)
			{
				if (oneByteChars)
				{
					for (int i = 0; i < length; i++)
					{
						ReadByte(out var value2);
						ptr[i] = (char)value2;
					}
				}
				else
				{
					ReadBytes((byte*)ptr, length * 2);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValue<T>(out T[] array) where T : unmanaged
		{
			ReadValue(out int value);
			int size = value * sizeof(T);
			array = new T[value];
			fixed (T* ptr = array)
			{
				byte* value2 = (byte*)ptr;
				ReadBytes(value2, size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafe<T>(out T[] array) where T : unmanaged
		{
			if (!TryBeginReadInternal(4))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			ReadValue(out int value);
			int num = value * sizeof(T);
			if (!TryBeginReadInternal(num))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			array = new T[value];
			fixed (T* ptr = array)
			{
				byte* value2 = (byte*)ptr;
				ReadBytes(value2, num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadPartialValue<T>(out T value, int bytesToRead, int offsetBytes = 0) where T : unmanaged
		{
			T val = new T();
			byte* destination = (byte*)(&val) + offsetBytes;
			byte* source = Handle->BufferPointer + Handle->Position;
			UnsafeUtility.MemCpy(destination, source, bytesToRead);
			Handle->Position += bytesToRead;
			value = val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadByte(out byte value)
		{
			value = Handle->BufferPointer[Handle->Position++];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadByteSafe(out byte value)
		{
			if (!TryBeginReadInternal(1))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			value = Handle->BufferPointer[Handle->Position++];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadBytes(byte* value, int size, int offset = 0)
		{
			UnsafeUtility.MemCpy(value + offset, Handle->BufferPointer + Handle->Position, size);
			Handle->Position += size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadBytesSafe(byte* value, int size, int offset = 0)
		{
			if (!TryBeginReadInternal(size))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			UnsafeUtility.MemCpy(value + offset, Handle->BufferPointer + Handle->Position, size);
			Handle->Position += size;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadBytes(ref byte[] value, int size, int offset = 0)
		{
			fixed (byte* value2 = value)
			{
				ReadBytes(value2, size, offset);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadBytesSafe(ref byte[] value, int size, int offset = 0)
		{
			fixed (byte* value2 = value)
			{
				ReadBytesSafe(value2, size, offset);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValue<T>(out T value) where T : unmanaged
		{
			int num = sizeof(T);
			fixed (T* destination = &value)
			{
				UnsafeUtility.MemCpy(destination, Handle->BufferPointer + Handle->Position, num);
			}
			Handle->Position += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafe<T>(out T value) where T : unmanaged
		{
			int num = sizeof(T);
			if (!TryBeginReadInternal(num))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			fixed (T* destination = &value)
			{
				UnsafeUtility.MemCpy(destination, Handle->BufferPointer + Handle->Position, num);
			}
			Handle->Position += num;
		}
	}
}
