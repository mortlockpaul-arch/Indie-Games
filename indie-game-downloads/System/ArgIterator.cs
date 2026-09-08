using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

public ref struct ArgIterator
{
	private struct SigPointer
	{
		internal nint _ptr;

		internal uint _len;
	}

	private nint _argCookie;

	private SigPointer _sigPtr;

	private nint _argPtr;

	private int _remainingArgs;

	private unsafe ArgIterator* ThisPtr => (ArgIterator*)Unsafe.AsPointer(in _argCookie);

	public unsafe ArgIterator(RuntimeArgumentHandle arglist)
	{
		nint value = arglist.Value;
		if (value == 0)
		{
			throw new ArgumentException(SR.InvalidOperation_HandleIsNotInitialized);
		}
		Init(ThisPtr, value);
	}

	[DllImport("QCall", EntryPoint = "ArgIterator_Init", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ArgIterator_Init")]
	private unsafe static extern void Init(ArgIterator* thisPtr, nint cookie);

	[CLSCompliant(false)]
	public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr)
	{
		nint value = arglist.Value;
		if (value == 0)
		{
			throw new ArgumentException(SR.InvalidOperation_HandleIsNotInitialized);
		}
		Init(ThisPtr, value, ptr);
	}

	[DllImport("QCall", EntryPoint = "ArgIterator_Init2", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ArgIterator_Init2")]
	private unsafe static extern void Init(ArgIterator* thisPtr, nint cookie, void* ptr);

	[CLSCompliant(false)]
	public unsafe TypedReference GetNextArg()
	{
		if (_argCookie == 0)
		{
			ThrowHelper.ThrowNotSupportedException();
		}
		if (_remainingArgs == 0)
		{
			throw new InvalidOperationException(SR.InvalidOperation_EnumEnded);
		}
		TypedReference result = default(TypedReference);
		GetNextArg(ThisPtr, &result);
		return result;
	}

	[DllImport("QCall", EntryPoint = "ArgIterator_GetNextArg", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ArgIterator_GetNextArg")]
	private unsafe static extern void GetNextArg(ArgIterator* thisPtr, TypedReference* pResult);

	[CLSCompliant(false)]
	public unsafe TypedReference GetNextArg(RuntimeTypeHandle rth)
	{
		if (_sigPtr._ptr != IntPtr.Zero)
		{
			return GetNextArg();
		}
		if (_argPtr == IntPtr.Zero)
		{
			throw new ArgumentNullException(null);
		}
		if (rth.IsNullHandle())
		{
			throw new ArgumentNullException("rth", SR.Arg_InvalidHandle);
		}
		TypedReference result = default(TypedReference);
		GetNextArg(ThisPtr, new QCallTypeHandle(ref rth), &result);
		return result;
	}

	[DllImport("QCall", EntryPoint = "ArgIterator_GetNextArg2", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ArgIterator_GetNextArg2")]
	private unsafe static extern void GetNextArg(ArgIterator* thisPtr, QCallTypeHandle rth, TypedReference* pResult);

	public void End()
	{
	}

	public int GetRemainingCount()
	{
		if (_argCookie == 0)
		{
			ThrowHelper.ThrowNotSupportedException();
		}
		return _remainingArgs;
	}

	public unsafe RuntimeTypeHandle GetNextArgType()
	{
		return RuntimeTypeHandle.FromIntPtr(GetNextArgType(ThisPtr));
	}

	[DllImport("QCall", EntryPoint = "ArgIterator_GetNextArgType", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ArgIterator_GetNextArgType")]
	private unsafe static extern nint GetNextArgType(ArgIterator* thisPtr);

	public override int GetHashCode()
	{
		return HashCode.Combine(_argCookie);
	}

	public override bool Equals(object? o)
	{
		throw new NotSupportedException(SR.NotSupported_NYI);
	}
}
