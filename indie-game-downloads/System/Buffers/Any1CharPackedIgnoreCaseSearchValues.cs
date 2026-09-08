using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class Any1CharPackedIgnoreCaseSearchValues : SearchValues<char>
{
	private readonly char _lowerCase;

	private readonly char _upperCase;

	private readonly uint _lowerCaseUint;

	public Any1CharPackedIgnoreCaseSearchValues(char value)
	{
		_lowerCase = value;
		_upperCase = (char)(value & -33);
		_lowerCaseUint = value;
	}

	internal override char[] GetValues()
	{
		return new char[2] { _upperCase, _lowerCase };
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		return (value | 0x20) == (int)_lowerCaseUint;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAnyIgnoreCase(ref MemoryMarshal.GetReference(span), _lowerCase, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAnyExceptIgnoreCase(ref MemoryMarshal.GetReference(span), _lowerCase, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		return span.LastIndexOfAny(_lowerCase, _upperCase);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return span.LastIndexOfAnyExcept(_lowerCase, _upperCase);
	}
}
