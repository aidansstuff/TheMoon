using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Netcode
{
	public struct FastBufferReader : IDisposable
	{
		internal struct ReaderHandle
		{
			internal unsafe byte* BufferPointer;

			internal int Position;

			internal int Length;

			internal Allocator Allocator;
		}

		internal unsafe ReaderHandle* Handle;

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

		public unsafe bool IsInitialized => Handle != null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void CommitBitwiseReads(int amount)
		{
			Handle->Position += amount;
		}

		private unsafe static ReaderHandle* CreateHandle(byte* buffer, int length, int offset, Allocator copyAllocator, Allocator internalAllocator)
		{
			ReaderHandle* ptr;
			if (copyAllocator == Allocator.None)
			{
				ptr = (ReaderHandle*)UnsafeUtility.Malloc(sizeof(ReaderHandle), UnsafeUtility.AlignOf<byte>(), internalAllocator);
				ptr->BufferPointer = buffer;
				ptr->Position = offset;
			}
			else
			{
				ptr = (ReaderHandle*)UnsafeUtility.Malloc(sizeof(ReaderHandle) + length, UnsafeUtility.AlignOf<byte>(), copyAllocator);
				UnsafeUtility.MemCpy(ptr + 1, buffer + offset, length);
				ptr->BufferPointer = (byte*)(ptr + 1);
				ptr->Position = 0;
			}
			ptr->Length = length;
			ptr->Allocator = ((copyAllocator == Allocator.None) ? internalAllocator : copyAllocator);
			return ptr;
		}

		public unsafe FastBufferReader(NativeArray<byte> buffer, Allocator copyAllocator, int length = -1, int offset = 0, Allocator internalAllocator = Allocator.Temp)
		{
			Handle = CreateHandle((byte*)buffer.GetUnsafePtr(), (length == -1) ? buffer.Length : length, offset, copyAllocator, internalAllocator);
		}

		public unsafe FastBufferReader(ArraySegment<byte> buffer, Allocator copyAllocator, int length = -1, int offset = 0)
		{
			if (copyAllocator == Allocator.None)
			{
				throw new NotSupportedException("Allocator.None cannot be used with managed source buffers.");
			}
			fixed (byte* buffer2 = buffer.Array)
			{
				Handle = CreateHandle(buffer2, (length == -1) ? buffer.Count : length, offset, copyAllocator, Allocator.Temp);
			}
		}

		public unsafe FastBufferReader(byte[] buffer, Allocator copyAllocator, int length = -1, int offset = 0)
		{
			if (copyAllocator == Allocator.None)
			{
				throw new NotSupportedException("Allocator.None cannot be used with managed source buffers.");
			}
			fixed (byte* buffer2 = buffer)
			{
				Handle = CreateHandle(buffer2, (length == -1) ? buffer.Length : length, offset, copyAllocator, Allocator.Temp);
			}
		}

		public unsafe FastBufferReader(byte* buffer, Allocator copyAllocator, int length, int offset = 0, Allocator internalAllocator = Allocator.Temp)
		{
			Handle = CreateHandle(buffer, length, offset, copyAllocator, internalAllocator);
		}

		public unsafe FastBufferReader(FastBufferWriter writer, Allocator copyAllocator, int length = -1, int offset = 0, Allocator internalAllocator = Allocator.Temp)
		{
			Handle = CreateHandle(writer.GetUnsafePtr(), (length == -1) ? writer.Length : length, offset, copyAllocator, internalAllocator);
		}

		public unsafe FastBufferReader(FastBufferReader reader, Allocator copyAllocator, int length = -1, int offset = 0, Allocator internalAllocator = Allocator.Temp)
		{
			Handle = CreateHandle(reader.GetUnsafePtr(), (length == -1) ? reader.Length : length, offset, copyAllocator, internalAllocator);
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.Free(Handle, Handle->Allocator);
			Handle = null;
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
			ReadValueSafe(out int value2, default(FastBufferWriter.ForPrimitives));
			value = new T[value2];
			for (int i = 0; i < value2; i++)
			{
				ReadNetworkSerializable(out value[i]);
			}
		}

		public void ReadNetworkSerializable<T>(out NativeArray<T> value, Allocator allocator) where T : unmanaged, INetworkSerializable
		{
			ReadValueSafe(out int value2, default(FastBufferWriter.ForPrimitives));
			value = new NativeArray<T>(value2, allocator);
			for (int i = 0; i < value2; i++)
			{
				ReadNetworkSerializable(out T value3);
				value[i] = value3;
			}
		}

		public void ReadNetworkSerializableInPlace<T>(ref T value) where T : INetworkSerializable
		{
			BufferSerializer<BufferSerializerReader> serializer = new BufferSerializer<BufferSerializerReader>(new BufferSerializerReader(this));
			value.NetworkSerialize(serializer);
		}

		public unsafe void ReadValue(out string s, bool oneByteChars = false)
		{
			ReadValue(out uint value, default(FastBufferWriter.ForPrimitives));
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
			ReadValue(out uint value, default(FastBufferWriter.ForPrimitives));
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
		internal unsafe void ReadUnmanaged<T>(out T value) where T : unmanaged
		{
			fixed (T* ptr = &value)
			{
				byte* value2 = (byte*)ptr;
				ReadBytes(value2, sizeof(T));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void ReadUnmanagedSafe<T>(out T value) where T : unmanaged
		{
			fixed (T* ptr = &value)
			{
				byte* value2 = (byte*)ptr;
				ReadBytesSafe(value2, sizeof(T));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void ReadUnmanaged<T>(out T[] value) where T : unmanaged
		{
			ReadUnmanaged(out int value2);
			int size = value2 * sizeof(T);
			value = new T[value2];
			fixed (T* ptr = value)
			{
				byte* value3 = (byte*)ptr;
				ReadBytes(value3, size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void ReadUnmanagedSafe<T>(out T[] value) where T : unmanaged
		{
			ReadUnmanagedSafe(out int value2);
			int size = value2 * sizeof(T);
			value = new T[value2];
			fixed (T* ptr = value)
			{
				byte* value3 = (byte*)ptr;
				ReadBytesSafe(value3, size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void ReadUnmanaged<T>(out NativeArray<T> value, Allocator allocator) where T : unmanaged
		{
			ReadUnmanaged(out int value2);
			int size = value2 * sizeof(T);
			value = new NativeArray<T>(value2, allocator);
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			ReadBytes(unsafePtr, size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void ReadUnmanagedSafe<T>(out NativeArray<T> value, Allocator allocator) where T : unmanaged
		{
			ReadUnmanagedSafe(out int value2);
			int size = value2 * sizeof(T);
			value = new NativeArray<T>(value2, allocator);
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			ReadBytesSafe(unsafePtr, size);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			ReadNetworkSerializable(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T[] value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			ReadNetworkSerializable(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			ReadNetworkSerializable(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T[] value, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : INetworkSerializable, new()
		{
			ReadNetworkSerializable(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out NativeArray<T> value, Allocator allocator, FastBufferWriter.ForNetworkSerializable unused = default(FastBufferWriter.ForNetworkSerializable)) where T : unmanaged, INetworkSerializable
		{
			ReadNetworkSerializable(out value, allocator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T[] value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out NativeArray<T> value, Allocator allocator, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.ReadWithAllocator(this, out value, allocator);
			}
			else
			{
				ReadUnmanaged(out value, allocator);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueTemp<T>(out NativeArray<T> value, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.ReadWithAllocator(this, out value, Allocator.Temp);
			}
			else
			{
				ReadUnmanaged(out value, Allocator.Temp);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T[] value, FastBufferWriter.ForStructs unused = default(FastBufferWriter.ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out NativeArray<T> value, Allocator allocator, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.ReadWithAllocator(this, out value, allocator);
			}
			else
			{
				ReadUnmanagedSafe(out value, allocator);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafeTemp<T>(out NativeArray<T> value, FastBufferWriter.ForGeneric unused = default(FastBufferWriter.ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.ReadWithAllocator(this, out value, Allocator.Temp);
			}
			else
			{
				ReadUnmanagedSafe(out value, Allocator.Temp);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T[] value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T[] value, FastBufferWriter.ForPrimitives unused = default(FastBufferWriter.ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue<T>(out T[] value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T[] value, FastBufferWriter.ForEnums unused = default(FastBufferWriter.ForEnums)) where T : unmanaged, Enum
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector2 value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector2[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector3 value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector3[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector2Int value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector2Int[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector3Int value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector3Int[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector4 value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Vector4[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Quaternion value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Quaternion[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Color value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Color[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Color32 value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Color32[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Ray value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Ray[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Ray2D value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValue(out Ray2D[] value)
		{
			ReadUnmanaged(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector2 value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector2[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector3 value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector3[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector2Int value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector2Int[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector3Int value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector3Int[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector4 value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Vector4[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Quaternion value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Quaternion[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Color value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Color[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Color32 value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Color32[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Ray value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Ray[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Ray2D value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe(out Ray2D[] value)
		{
			ReadUnmanagedSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValue<T>(out T value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanaged(out int value2);
			value = new T
			{
				Length = value2
			};
			ReadBytes(value.GetUnsafePtr(), value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafe<T>(out T value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanagedSafe(out int value2);
			value = new T
			{
				Length = value2
			};
			ReadBytesSafe(value.GetUnsafePtr(), value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafeInPlace<T>(ref T value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanagedSafe(out int value2);
			value.Length = value2;
			ReadBytesSafe(value.GetUnsafePtr(), value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafe<T>(out NativeArray<T> value, Allocator allocator) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanagedSafe(out int value2);
			value = new NativeArray<T>(value2, allocator);
			T* unsafePtr = (T*)value.GetUnsafePtr();
			for (int i = 0; i < value2; i++)
			{
				ReadValueSafeInPlace(ref unsafePtr[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void ReadValueSafeTemp<T>(out NativeArray<T> value) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanagedSafe(out int value2);
			value = new NativeArray<T>(value2, Allocator.Temp);
			T* unsafePtr = (T*)value.GetUnsafePtr();
			for (int i = 0; i < value2; i++)
			{
				ReadValueSafeInPlace(ref unsafePtr[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ReadValueSafe<T>(out T[] value, FastBufferWriter.ForFixedStrings unused = default(FastBufferWriter.ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			ReadUnmanagedSafe(out int value2);
			value = new T[value2];
			for (int i = 0; i < value2; i++)
			{
				ReadValueSafeInPlace(ref value[i]);
			}
		}
	}
}
