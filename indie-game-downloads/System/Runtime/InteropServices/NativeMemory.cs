using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices;

public static class NativeMemory
{
	[CLSCompliant(false)]
	public unsafe static void* Alloc(nuint elementCount, nuint elementSize)
	{
		return Alloc(GetByteCount(elementCount, elementSize));
	}

	[CLSCompliant(false)]
	public unsafe static void* AllocZeroed(nuint byteCount)
	{
		return AllocZeroed(byteCount, 1u);
	}

	[CLSCompliant(false)]
	public unsafe static void Clear(void* ptr, nuint byteCount)
	{
		SpanHelpers.ClearWithoutReferences(ref *(byte*)ptr, byteCount);
	}

	[CLSCompliant(false)]
	public unsafe static void Copy(void* source, void* destination, nuint byteCount)
	{
		SpanHelpers.Memmove(ref *(byte*)destination, ref *(byte*)source, byteCount);
	}

	[CLSCompliant(false)]
	public unsafe static void Fill(void* ptr, nuint byteCount, byte value)
	{
		SpanHelpers.Fill(ref *(byte*)ptr, byteCount, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static nuint GetByteCount(nuint elementCount, nuint elementSize)
	{
		nuint num = (nuint)((nint)1 << (0x20 & 0x3F));
		if ((elementSize < num && elementCount < num) || elementSize == 0 || UIntPtr.MaxValue / elementSize >= elementCount)
		{
			return elementCount * elementSize;
		}
		return UIntPtr.MaxValue;
	}

	[CLSCompliant(false)]
	public unsafe static void* AlignedAlloc(nuint byteCount, nuint alignment)
	{
		if (!BitOperations.IsPow2(alignment))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AlignmentMustBePow2);
		}
		void* intPtr = Interop.Ucrtbase._aligned_malloc((byteCount != 0) ? byteCount : 1, alignment);
		if (intPtr == null)
		{
			ThrowHelper.ThrowOutOfMemoryException();
		}
		return intPtr;
	}

	[CLSCompliant(false)]
	public unsafe static void AlignedFree(void* ptr)
	{
		if (ptr != null)
		{
			Interop.Ucrtbase._aligned_free(ptr);
		}
	}

	[CLSCompliant(false)]
	public unsafe static void* AlignedRealloc(void* ptr, nuint byteCount, nuint alignment)
	{
		if (!BitOperations.IsPow2(alignment))
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AlignmentMustBePow2);
		}
		void* intPtr = Interop.Ucrtbase._aligned_realloc(ptr, (byteCount != 0) ? byteCount : 1, alignment);
		if (intPtr == null)
		{
			ThrowHelper.ThrowOutOfMemoryException();
		}
		return intPtr;
	}

	[CLSCompliant(false)]
	public unsafe static void* Alloc(nuint byteCount)
	{
		void* intPtr = Interop.Ucrtbase.malloc(byteCount);
		if (intPtr == null)
		{
			ThrowHelper.ThrowOutOfMemoryException();
		}
		return intPtr;
	}

	[CLSCompliant(false)]
	public unsafe static void* AllocZeroed(nuint elementCount, nuint elementSize)
	{
		void* intPtr = Interop.Ucrtbase.calloc(elementCount, elementSize);
		if (intPtr == null)
		{
			ThrowHelper.ThrowOutOfMemoryException();
		}
		return intPtr;
	}

	[CLSCompliant(false)]
	public unsafe static void Free(void* ptr)
	{
		if (ptr != null)
		{
			Interop.Ucrtbase.free(ptr);
		}
	}

	[CLSCompliant(false)]
	public unsafe static void* Realloc(void* ptr, nuint byteCount)
	{
		void* intPtr = Interop.Ucrtbase.realloc(ptr, (byteCount != 0) ? byteCount : 1);
		if (intPtr == null)
		{
			ThrowHelper.ThrowOutOfMemoryException();
		}
		return intPtr;
	}
}
