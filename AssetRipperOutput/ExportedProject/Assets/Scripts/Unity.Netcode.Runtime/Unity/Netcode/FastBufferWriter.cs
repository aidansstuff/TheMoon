using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Netcode
{
	public struct FastBufferWriter : IDisposable
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

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForPrimitives
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForEnums
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForStructs
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForNetworkSerializable
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForFixedStrings
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ForGeneric
		{
		}

		internal unsafe WriterHandle* Handle;

		private static byte[] s_ByteArrayCache = new byte[65535];

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

		public unsafe bool IsInitialized => Handle != null;

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
			Handle = null;
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

		internal unsafe ArraySegment<byte> ToTempByteArray()
		{
			int length = Length;
			if (length > s_ByteArrayCache.Length)
			{
				return new ArraySegment<byte>(ToArray(), 0, length);
			}
			fixed (byte* destination = s_ByteArrayCache)
			{
				UnsafeUtility.MemCpy(destination, Handle->BufferPointer, length);
			}
			return new ArraySegment<byte>(s_ByteArrayCache, 0, length);
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

		public void WriteNetworkSerializable<T>(T[] array, int count = -1, int offset = 0) where T : INetworkSerializable
		{
			int value = ((count != -1) ? count : (array.Length - offset));
			WriteValueSafe(in value, default(ForPrimitives));
			for (int i = 0; i < array.Length; i++)
			{
				T value2 = array[i];
				WriteNetworkSerializable(in value2);
			}
		}

		public void WriteNetworkSerializable<T>(NativeArray<T> array, int count = -1, int offset = 0) where T : unmanaged, INetworkSerializable
		{
			int value = ((count != -1) ? count : (array.Length - offset));
			WriteValueSafe(in value, default(ForPrimitives));
			foreach (T item in array)
			{
				T value2 = item;
				WriteNetworkSerializable(in value2);
			}
		}

		public unsafe void WriteValue(string s, bool oneByteChars = false)
		{
			uint value = (uint)s.Length;
			WriteValue(in value, default(ForPrimitives));
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
			WriteValue(in value, default(ForPrimitives));
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
		public unsafe static int GetWriteSize<T>(NativeArray<T> array, int count = -1, int offset = 0) where T : unmanaged
		{
			int num = ((count != -1) ? count : (array.Length - offset)) * sizeof(T);
			return 4 + num;
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
		public unsafe void WriteBytes(NativeArray<byte> value, int size = -1, int offset = 0)
		{
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			WriteBytes(unsafePtr, (size == -1) ? value.Length : size, offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytes(NativeList<byte> value, int size = -1, int offset = 0)
		{
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			WriteBytes(unsafePtr, (size == -1) ? value.Length : size, offset);
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
		public unsafe void WriteBytesSafe(NativeArray<byte> value, int size = -1, int offset = 0)
		{
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			WriteBytesSafe(unsafePtr, (size == -1) ? value.Length : size, offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteBytesSafe(NativeList<byte> value, int size = -1, int offset = 0)
		{
			byte* unsafePtr = (byte*)value.GetUnsafePtr();
			WriteBytesSafe(unsafePtr, (size == -1) ? value.Length : size, offset);
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
		public unsafe static int GetWriteSize<T>(in T value, ForStructs unused = default(ForStructs)) where T : unmanaged
		{
			return sizeof(T);
		}

		public static int GetWriteSize<T>(in T value) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			return value.Length + 4;
		}

		public static int GetWriteSize<T>(in NativeArray<T> value) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			int num = 4;
			foreach (T item in value)
			{
				num += 4 + item.Length;
			}
			return num;
		}

		public unsafe static int GetWriteSize<T>() where T : unmanaged
		{
			return sizeof(T);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanaged<T>(in T value) where T : unmanaged
		{
			fixed (T* ptr = &value)
			{
				byte* value2 = (byte*)ptr;
				WriteBytes(value2, sizeof(T));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanagedSafe<T>(in T value) where T : unmanaged
		{
			fixed (T* ptr = &value)
			{
				byte* value2 = (byte*)ptr;
				WriteBytesSafe(value2, sizeof(T));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanaged<T>(T[] value) where T : unmanaged
		{
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			fixed (T* ptr = value)
			{
				byte* value3 = (byte*)ptr;
				WriteBytes(value3, sizeof(T) * value.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanagedSafe<T>(T[] value) where T : unmanaged
		{
			int value2 = value.Length;
			WriteUnmanagedSafe(in value2);
			fixed (T* ptr = value)
			{
				byte* value3 = (byte*)ptr;
				WriteBytesSafe(value3, sizeof(T) * value.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanaged<T>(NativeArray<T> value) where T : unmanaged
		{
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			T* unsafePtr = (T*)value.GetUnsafePtr();
			byte* value3 = (byte*)unsafePtr;
			WriteBytes(value3, sizeof(T) * value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void WriteUnmanagedSafe<T>(NativeArray<T> value) where T : unmanaged
		{
			int value2 = value.Length;
			WriteUnmanagedSafe(in value2);
			T* unsafePtr = (T*)value.GetUnsafePtr();
			byte* value3 = (byte*)unsafePtr;
			WriteBytesSafe(value3, sizeof(T) * value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, ForNetworkSerializable unused = default(ForNetworkSerializable)) where T : INetworkSerializable
		{
			WriteNetworkSerializable(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(T[] value, ForNetworkSerializable unused = default(ForNetworkSerializable)) where T : INetworkSerializable
		{
			WriteNetworkSerializable(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in T value, ForNetworkSerializable unused = default(ForNetworkSerializable)) where T : INetworkSerializable
		{
			WriteNetworkSerializable(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(T[] value, ForNetworkSerializable unused = default(ForNetworkSerializable)) where T : INetworkSerializable
		{
			WriteNetworkSerializable(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, ForStructs unused = default(ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(T[] value, ForStructs unused = default(ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(NativeArray<T> value, ForGeneric unused = default(ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.Write(this, ref value);
			}
			else
			{
				WriteUnmanaged(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in T value, ForStructs unused = default(ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(T[] value, ForStructs unused = default(ForStructs)) where T : unmanaged, INetworkSerializeByMemcpy
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(NativeArray<T> value, ForGeneric unused = default(ForGeneric)) where T : unmanaged
		{
			if (typeof(INetworkSerializable).IsAssignableFrom(typeof(T)))
			{
				NetworkVariableSerialization<NativeArray<T>>.Serializer.Write(this, ref value);
			}
			else
			{
				WriteUnmanagedSafe(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, ForPrimitives unused = default(ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(T[] value, ForPrimitives unused = default(ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in T value, ForPrimitives unused = default(ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(T[] value, ForPrimitives unused = default(ForPrimitives)) where T : unmanaged, IComparable, IConvertible, IComparable<T>, IEquatable<T>
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in T value, ForEnums unused = default(ForEnums)) where T : unmanaged, Enum
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(T[] value, ForEnums unused = default(ForEnums)) where T : unmanaged, Enum
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in T value, ForEnums unused = default(ForEnums)) where T : unmanaged, Enum
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(T[] value, ForEnums unused = default(ForEnums)) where T : unmanaged, Enum
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Vector2 value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Vector2[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Vector3 value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Vector3[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Vector2Int value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Vector2Int[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Vector3Int value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Vector3Int[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Vector4 value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Vector4[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Quaternion value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Quaternion[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Color value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Color[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Color32 value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Color32[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Ray value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Ray[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(in Ray2D value)
		{
			WriteUnmanaged(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue(Ray2D[] value)
		{
			WriteUnmanaged(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Vector2 value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Vector2[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Vector3 value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Vector3[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Vector2Int value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Vector2Int[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Vector3Int value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Vector3Int[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Vector4 value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Vector4[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Quaternion value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Quaternion[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Color value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Color[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Color32 value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Color32[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Ray value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Ray[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(in Ray2D value)
		{
			WriteUnmanagedSafe(in value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe(Ray2D[] value)
		{
			WriteUnmanagedSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteValue<T>(in T value, ForFixedStrings unused = default(ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			fixed (T* ptr = &value)
			{
				WriteBytes(ptr->GetUnsafePtr(), value.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(T[] value, ForFixedStrings unused = default(ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			for (value2 = 0; value2 < value.Length; value2++)
			{
				T value3 = value[value2];
				WriteValue(in value3, default(ForFixedStrings));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValue<T>(in NativeArray<T> value, ForFixedStrings unused = default(ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			foreach (T item in value)
			{
				T value3 = item;
				WriteValue(in value3, default(ForFixedStrings));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in T value, ForFixedStrings unused = default(ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			if (!TryBeginWriteInternal(4 + value.Length))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			WriteValue(in value, default(ForFixedStrings));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(T[] value, ForFixedStrings unused = default(ForFixedStrings)) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			if (!TryBeginWriteInternal(GetWriteSize(value)))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			for (value2 = 0; value2 < value.Length; value2++)
			{
				T value3 = value[value2];
				WriteValue(in value3, default(ForFixedStrings));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteValueSafe<T>(in NativeArray<T> value) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			if (!TryBeginWriteInternal(GetWriteSize(in value)))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			int value2 = value.Length;
			WriteUnmanaged(in value2);
			foreach (T item in value)
			{
				T value3 = item;
				WriteValue(in value3, default(ForFixedStrings));
			}
		}
	}
}
