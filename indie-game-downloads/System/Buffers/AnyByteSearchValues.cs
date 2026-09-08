using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.Wasm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class AnyByteSearchValues : SearchValues<byte>
{
	private IndexOfAnyAsciiSearcher.AnyByteState _state;

	public AnyByteSearchValues(ReadOnlySpan<byte> values)
	{
		IndexOfAnyAsciiSearcher.ComputeAnyByteState(values, out _state);
	}

	internal override byte[] GetValues()
	{
		return _state.Lookup.GetByteValues();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(byte value)
	{
		return _state.Lookup.Contains(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int IndexOfAny(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int IndexOfAnyExcept(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAny(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.DontNegate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override bool ContainsAny(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.ContainsAny<IndexOfAnyAsciiSearcher.DontNegate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd))]
	[CompExactlyDependsOn(typeof(PackedSimd))]
	internal override bool ContainsAnyExcept(ReadOnlySpan<byte> span)
	{
		return IndexOfAnyAsciiSearcher.ContainsAny<IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length, ref _state);
	}
}
