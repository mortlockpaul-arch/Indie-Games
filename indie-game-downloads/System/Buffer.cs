using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System;

public static class Buffer
{
	[DllImport("QCall", EntryPoint = "Buffer_Clear", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Buffer_Clear")]
	private unsafe static extern void ZeroMemoryInternal(void* b, nuint byteLength);

	[DllImport("QCall", EntryPoint = "Buffer_MemMove", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Buffer_MemMove")]
	private unsafe static extern void MemmoveInternal(byte* dest, byte* src, nuint len);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BulkMoveWithWriteBarrierInternal(ref byte destination, ref byte source, nuint byteCount);

	internal unsafe static void Memcpy(byte* dest, byte* src, int len)
	{
		Memmove(ref *dest, ref *src, (uint)len);
	}

	internal unsafe static void Memcpy(byte* pDest, int destIndex, byte[] src, int srcIndex, int len)
	{
		Memmove(ref pDest[(uint)destIndex], ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(src), (nint)(uint)srcIndex), (uint)len);
	}

	public static void BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count)
	{
		ArgumentNullException.ThrowIfNull(src, "src");
		ArgumentNullException.ThrowIfNull(dst, "dst");
		nuint num = src.NativeLength;
		if (src.GetType() != typeof(byte[]))
		{
			if (!src.GetCorElementTypeOfElementType().IsPrimitiveType())
			{
				throw new ArgumentException(SR.Arg_MustBePrimArray, "src");
			}
			num *= src.GetElementSize();
		}
		nuint num2 = num;
		if (src != dst)
		{
			num2 = dst.NativeLength;
			if (dst.GetType() != typeof(byte[]))
			{
				if (!dst.GetCorElementTypeOfElementType().IsPrimitiveType())
				{
					throw new ArgumentException(SR.Arg_MustBePrimArray, "dst");
				}
				num2 *= dst.GetElementSize();
			}
		}
		ArgumentOutOfRangeException.ThrowIfNegative(srcOffset, "srcOffset");
		ArgumentOutOfRangeException.ThrowIfNegative(dstOffset, "dstOffset");
		ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
		nuint num3 = (nuint)count;
		nuint num4 = (nuint)srcOffset;
		nuint num5 = (nuint)dstOffset;
		if (num < num4 + num3 || num2 < num5 + num3)
		{
			throw new ArgumentException(SR.Argument_InvalidOffLen);
		}
		Memmove(ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(dst), num5), ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(src), num4), num3);
	}

	public static int ByteLength(Array array)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		if (!array.GetCorElementTypeOfElementType().IsPrimitiveType())
		{
			throw new ArgumentException(SR.Arg_MustBePrimArray, "array");
		}
		checked
		{
			return (int)unchecked(array.NativeLength * array.GetElementSize());
		}
	}

	public static byte GetByte(Array array, int index)
	{
		if ((uint)index >= (uint)ByteLength(array))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);
	}

	public static void SetByte(Array array, int index, byte value)
	{
		if ((uint)index >= (uint)ByteLength(array))
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index);
		}
		Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static void MemoryCopy(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy)
	{
		if (sourceBytesToCopy > destinationSizeInBytes)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
		}
		Memmove(ref *(byte*)destination, ref *(byte*)source, checked((nuint)sourceBytesToCopy));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CLSCompliant(false)]
	public unsafe static void MemoryCopy(void* source, void* destination, ulong destinationSizeInBytes, ulong sourceBytesToCopy)
	{
		if (sourceBytesToCopy > destinationSizeInBytes)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
		}
		Memmove(ref *(byte*)destination, ref *(byte*)source, checked((nuint)sourceBytesToCopy));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal unsafe static void MemmoveInternal(ref byte dest, ref byte src, nuint len)
	{
		fixed (byte* dest2 = &dest)
		{
			fixed (byte* src2 = &src)
			{
				MemmoveInternal(dest2, src2, len);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal unsafe static void ZeroMemoryInternal(ref byte b, nuint byteLength)
	{
		fixed (byte* b2 = &b)
		{
			ZeroMemoryInternal(b2, byteLength);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Intrinsic]
	internal static void Memmove<T>(ref T destination, ref T source, nuint elementCount)
	{
		if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			SpanHelpers.Memmove(ref Unsafe.As<T, byte>(ref destination), ref Unsafe.As<T, byte>(ref source), elementCount * (nuint)Unsafe.SizeOf<T>());
		}
		else
		{
			BulkMoveWithWriteBarrier(ref Unsafe.As<T, byte>(ref destination), ref Unsafe.As<T, byte>(ref source), elementCount * (nuint)Unsafe.SizeOf<T>());
		}
	}

	internal static void BulkMoveWithWriteBarrier(ref byte destination, ref byte source, nuint byteCount)
	{
		if (byteCount <= 16384)
		{
			BulkMoveWithWriteBarrierInternal(ref destination, ref source, byteCount);
			Thread.FastPollGC();
		}
		else
		{
			BulkMoveWithWriteBarrierBatch(ref destination, ref source, byteCount);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void BulkMoveWithWriteBarrierBatch(ref byte destination, ref byte source, nuint byteCount)
	{
		if (Unsafe.AreSame(in source, in destination))
		{
			return;
		}
		if ((nuint)Unsafe.ByteOffset(in source, in destination) >= byteCount)
		{
			do
			{
				byteCount -= 16384;
				BulkMoveWithWriteBarrierInternal(ref destination, ref source, 16384u);
				Thread.FastPollGC();
				destination = ref Unsafe.AddByteOffset(ref destination, 16384u);
				source = ref Unsafe.AddByteOffset(ref source, 16384u);
			}
			while (byteCount > 16384);
		}
		else
		{
			do
			{
				byteCount -= 16384;
				BulkMoveWithWriteBarrierInternal(ref Unsafe.AddByteOffset(ref destination, byteCount), ref Unsafe.AddByteOffset(ref source, byteCount), 16384u);
				Thread.FastPollGC();
			}
			while (byteCount > 16384);
		}
		BulkMoveWithWriteBarrierInternal(ref destination, ref source, byteCount);
		Thread.FastPollGC();
	}
}
