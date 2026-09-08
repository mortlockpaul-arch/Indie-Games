using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class AsciiCharSearchValues<TOptimizations, TUniqueLowNibble> : SearchValues<char> where TOptimizations : struct, IndexOfAnyAsciiSearcher.IOptimizations where TUniqueLowNibble : struct, SearchValues.IRuntimeConst
{
	private IndexOfAnyAsciiSearcher.AsciiState _state;

	public AsciiCharSearchValues(ReadOnlySpan<char> values)
	{
		if (TUniqueLowNibble.Value)
		{
			IndexOfAnyAsciiSearcher.ComputeUniqueLowNibbleState(values, out _state);
		}
		else
		{
			IndexOfAnyAsciiSearcher.ComputeAsciiState(values, out _state);
		}
	}

	internal override char[] GetValues()
	{
		return _state.Lookup.GetCharValues();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		return _state.Lookup.Contains256(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.Negate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.DontNegate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override bool ContainsAny(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.ContainsAny<IndexOfAnyAsciiSearcher.DontNegate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override bool ContainsAnyExcept(ReadOnlySpan<char> span)
	{
		return IndexOfAnyAsciiSearcher.ContainsAny<IndexOfAnyAsciiSearcher.Negate, TOptimizations, TUniqueLowNibble>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _state);
	}
}
