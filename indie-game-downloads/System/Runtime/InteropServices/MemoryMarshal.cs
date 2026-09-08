using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices;

public static class MemoryMarshal
{
	[Intrinsic]
	[NonVersionable]
	public static ref T GetArrayDataReference<T>(T[] array)
	{
		return ref GetArrayDataReference(array);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	public unsafe static ref byte GetArrayDataReference(Array array)
	{
		return ref Unsafe.AddByteOffset(ref Unsafe.As<RawData>(array).Data, (nuint)RuntimeHelpers.GetMethodTable(array)->BaseSize - (nuint)(2 * sizeof(nint)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<byte> AsBytes<T>(Span<T> span) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		return new Span<byte>(ref Unsafe.As<T, byte>(ref GetReference(span)), checked(span.Length * Unsafe.SizeOf<T>()));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<byte> AsBytes<T>(ReadOnlySpan<T> span) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		return new ReadOnlySpan<byte>(ref Unsafe.As<T, byte>(ref GetReference(span)), checked(span.Length * Unsafe.SizeOf<T>()));
	}

	public static Memory<T> AsMemory<T>(ReadOnlyMemory<T> memory)
	{
		return new Memory<T>(memory._object, memory._index, memory._length);
	}

	public static ref T GetReference<T>(Span<T> span)
	{
		return ref span._reference;
	}

	public static ref T GetReference<T>(ReadOnlySpan<T> span)
	{
		return ref span._reference;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static ref T GetNonNullPinnableReference<T>(Span<T> span)
	{
		if (span.Length == 0)
		{
			return ref Unsafe.AsRef<T>((void*)1);
		}
		return ref Unsafe.AsRef(in span._reference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static ref T GetNonNullPinnableReference<T>(ReadOnlySpan<T> span)
	{
		if (span.Length == 0)
		{
			return ref Unsafe.AsRef<T>((void*)1);
		}
		return ref Unsafe.AsRef(in span._reference);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<TTo> Cast<TFrom, TTo>(Span<TFrom> span) where TFrom : struct where TTo : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<TFrom>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(TFrom));
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<TTo>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(TTo));
		}
		uint num = (uint)Unsafe.SizeOf<TFrom>();
		uint num2 = (uint)Unsafe.SizeOf<TTo>();
		uint length = (uint)span.Length;
		return new Span<TTo>(length: (num == num2) ? ((int)length) : ((num != 1) ? checked((int)unchecked((ulong)((long)length * (long)num) / (ulong)num2)) : ((int)(length / num2))), reference: ref Unsafe.As<TFrom, TTo>(ref span._reference));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<TTo> Cast<TFrom, TTo>(ReadOnlySpan<TFrom> span) where TFrom : struct where TTo : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<TFrom>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(TFrom));
		}
		if (RuntimeHelpers.IsReferenceOrContainsReferences<TTo>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(TTo));
		}
		uint num = (uint)Unsafe.SizeOf<TFrom>();
		uint num2 = (uint)Unsafe.SizeOf<TTo>();
		uint length = (uint)span.Length;
		return new ReadOnlySpan<TTo>(length: (num == num2) ? ((int)length) : ((num != 1) ? checked((int)unchecked((ulong)((long)length * (long)num) / (ulong)num2)) : ((int)(length / num2))), reference: ref Unsafe.As<TFrom, TTo>(ref GetReference(span)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Span<T> CreateSpan<T>(scoped ref T reference, int length)
	{
		return new Span<T>(ref Unsafe.AsRef(in reference), length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped ref readonly T reference, int length)
	{
		return new ReadOnlySpan<T>(ref Unsafe.AsRef(in reference), length);
	}

	[CLSCompliant(false)]
	public unsafe static ReadOnlySpan<char> CreateReadOnlySpanFromNullTerminated(char* value)
	{
		if (value == null)
		{
			return default(ReadOnlySpan<char>);
		}
		return new ReadOnlySpan<char>(value, string.wcslen(value));
	}

	[CLSCompliant(false)]
	public unsafe static ReadOnlySpan<byte> CreateReadOnlySpanFromNullTerminated(byte* value)
	{
		if (value == null)
		{
			return default(ReadOnlySpan<byte>);
		}
		return new ReadOnlySpan<byte>(value, string.strlen(value));
	}

	public static bool TryGetArray<T>(ReadOnlyMemory<T> memory, out ArraySegment<T> segment)
	{
		object objectStartLength = memory.GetObjectStartLength(out var start, out var length);
		if (objectStartLength != null && (!(typeof(T) == typeof(char)) || !(objectStartLength.GetType() == typeof(string))))
		{
			if (RuntimeHelpers.ObjectHasComponentSize(objectStartLength))
			{
				segment = new ArraySegment<T>(Unsafe.As<T[]>(objectStartLength), start & 0x7FFFFFFF, length);
				return true;
			}
			if (Unsafe.As<MemoryManager<T>>(objectStartLength).TryGetArray(out var segment2))
			{
				segment = new ArraySegment<T>(segment2.Array, segment2.Offset + start, length);
				return true;
			}
		}
		if (length == 0)
		{
			segment = ArraySegment<T>.Empty;
			return true;
		}
		segment = default(ArraySegment<T>);
		return false;
	}

	public static bool TryGetMemoryManager<T, TManager>(ReadOnlyMemory<T> memory, [NotNullWhen(true)] out TManager? manager) where TManager : MemoryManager<T>
	{
		int start;
		int length;
		return (manager = memory.GetObjectStartLength(out start, out length) as TManager) != null;
	}

	public static bool TryGetMemoryManager<T, TManager>(ReadOnlyMemory<T> memory, [NotNullWhen(true)] out TManager? manager, out int start, out int length) where TManager : MemoryManager<T>
	{
		if ((manager = memory.GetObjectStartLength(out start, out length) as TManager) == null)
		{
			start = 0;
			length = 0;
			return false;
		}
		return true;
	}

	public static IEnumerable<T> ToEnumerable<T>(ReadOnlyMemory<T> memory)
	{
		object objectStartLength = memory.GetObjectStartLength(out var start, out var length);
		if (length == 0 || objectStartLength == null)
		{
			return Array.Empty<T>();
		}
		if (typeof(T) == typeof(char) && objectStartLength is string text)
		{
			IEnumerable<char> enumerable;
			if (start != 0 || length != text.Length)
			{
				enumerable = FromString(text, start, length);
			}
			else
			{
				IEnumerable<char> enumerable2 = text;
				enumerable = enumerable2;
			}
			return (IEnumerable<T>)enumerable;
		}
		if (RuntimeHelpers.ObjectHasComponentSize(objectStartLength))
		{
			T[] array = Unsafe.As<T[]>(objectStartLength);
			start &= 0x7FFFFFFF;
			if (start != 0 || length != array.Length)
			{
				return FromArray(array, start, length);
			}
			return array;
		}
		return FromMemoryManager(memory);
		static IEnumerable<T> FromArray(T[] array2, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return array2[offset + i];
			}
		}
		static IEnumerable<T> FromMemoryManager(ReadOnlyMemory<T> readOnlyMemory)
		{
			for (int i = 0; i < readOnlyMemory.Length; i++)
			{
				yield return readOnlyMemory.Span[i];
			}
		}
		static IEnumerable<char> FromString(string s, int offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				yield return s[offset + i];
			}
		}
	}

	public static bool TryGetString(ReadOnlyMemory<char> memory, [NotNullWhen(true)] out string? text, out int start, out int length)
	{
		if (memory.GetObjectStartLength(out var start2, out var length2) is string text2)
		{
			text = text2;
			start = start2;
			length = length2;
			return true;
		}
		text = null;
		start = 0;
		length = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Read<T>(ReadOnlySpan<byte> source) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if (Unsafe.SizeOf<T>() > source.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		return Unsafe.ReadUnaligned<T>(in GetReference(source));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryRead<T>(ReadOnlySpan<byte> source, out T value) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if (Unsafe.SizeOf<T>() > (uint)source.Length)
		{
			value = default(T);
			return false;
		}
		value = Unsafe.ReadUnaligned<T>(in GetReference(source));
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<T>(Span<byte> destination, in T value) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if ((uint)Unsafe.SizeOf<T>() > (uint)destination.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		Unsafe.WriteUnaligned(ref GetReference(destination), value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryWrite<T>(Span<byte> destination, in T value) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if (Unsafe.SizeOf<T>() > (uint)destination.Length)
		{
			return false;
		}
		Unsafe.WriteUnaligned(ref GetReference(destination), value);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref T AsRef<T>(Span<byte> span) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if (Unsafe.SizeOf<T>() > (uint)span.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		return ref Unsafe.As<byte, T>(ref GetReference(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T AsRef<T>(ReadOnlySpan<byte> span) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
		}
		if (Unsafe.SizeOf<T>() > (uint)span.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
		}
		return ref Unsafe.As<byte, T>(ref GetReference(span));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Memory<T> CreateFromPinnedArray<T>(T[]? array, int start, int length)
	{
		if (array == null)
		{
			if (start != 0 || length != 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return default(Memory<T>);
		}
		if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
		{
			ThrowHelper.ThrowArrayTypeMismatchException();
		}
		if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException();
		}
		return new Memory<T>((object)array, start | int.MinValue, length);
	}
}
