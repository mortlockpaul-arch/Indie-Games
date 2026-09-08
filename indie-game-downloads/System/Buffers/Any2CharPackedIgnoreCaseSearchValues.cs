using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class Any2CharPackedIgnoreCaseSearchValues : SearchValues<char>
{
	private readonly char _e0;

	private readonly char _e1;

	private readonly uint _uint0;

	private readonly uint _uint1;

	private IndexOfAnyAsciiSearcher.AsciiState _state;

	public Any2CharPackedIgnoreCaseSearchValues(char value0, char value1)
	{
		char e = value0;
		char e2 = value1;
		_e0 = e;
		_e1 = e2;
		_uint0 = value0;
		_uint1 = value1;
		_003C_003Ey__InlineArray4<char> buffer = default(_003C_003Ey__InlineArray4<char>);
		buffer[0] = (char)(_e0 & -33);
		buffer[1] = _e0;
		buffer[2] = (char)(_e1 & -33);
		buffer[3] = _e1;
		IndexOfAnyAsciiSearcher.ComputeAsciiState<char>(buffer, out _state);
	}

	internal override char[] GetValues()
	{
		return new char[4]
		{
			(char)(_e0 & -33),
			_e0,
			(char)(_e1 & -33),
			_e1
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		uint num = (uint)(value | 0x20);
		if (num != _uint0)
		{
			return num == _uint1;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAnyIgnoreCase(ref MemoryMarshal.GetReference(span), _e0, _e1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Sse2))]
	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return PackedSpanHelpers.IndexOfAnyExceptIgnoreCase(ref MemoryMarshal.GetReference(span), _e0, _e1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.DontNegate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}
}
