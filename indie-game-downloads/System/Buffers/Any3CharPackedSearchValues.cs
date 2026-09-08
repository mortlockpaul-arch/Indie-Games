using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class Any3CharPackedSearchValues : SearchValues<char>
{
	private readonly char _e0;

	private readonly char _e1;

	private readonly char _e2;

	public Any3CharPackedSearchValues(char value0, char value1, char value2)
	{
		char e = value0;
		char e2 = value1;
		char e3 = value2;
		_e0 = e;
		_e1 = e2;
		_e2 = e3;
	}

	internal override char[] GetValues()
	{
		return new char[3] { _e0, _e1, _e2 };
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		if (value != _e0 && value != _e1)
		{
			return value == _e2;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAny(ref MemoryMarshal.GetReference(span), _e0, _e1, _e2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAnyExcept(ref MemoryMarshal.GetReference(span), _e0, _e1, _e2, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		return span.LastIndexOfAny(_e0, _e1, _e2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return span.LastIndexOfAnyExcept(_e0, _e1, _e2);
	}
}
