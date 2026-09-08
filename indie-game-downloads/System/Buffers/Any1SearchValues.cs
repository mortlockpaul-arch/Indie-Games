using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers;

internal sealed class Any1SearchValues<T, TImpl> : SearchValues<T> where T : struct, IEquatable<T> where TImpl : struct, INumber<TImpl>
{
	private readonly TImpl _e0;

	public Any1SearchValues(ReadOnlySpan<TImpl> values)
	{
		_e0 = values[0];
	}

	internal unsafe override T[] GetValues()
	{
		TImpl e = _e0;
		return new T[1] { Unsafe.Read<T>(&e) };
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe override bool ContainsCore(T value)
	{
		return Unsafe.Read<TImpl?>(&value) == _e0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAny(ReadOnlySpan<T> span)
	{
		return SpanHelpers.NonPackedIndexOfValueType<TImpl, SpanHelpers.DontNegate<TImpl>>(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyExcept(ReadOnlySpan<T> span)
	{
		return SpanHelpers.NonPackedIndexOfValueType<TImpl, SpanHelpers.Negate<TImpl>>(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAny(ReadOnlySpan<T> span)
	{
		return SpanHelpers.LastIndexOfValueType(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<T> span)
	{
		return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, span.Length);
	}
}
