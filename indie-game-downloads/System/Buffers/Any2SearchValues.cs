using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers;

internal sealed class Any2SearchValues<T, TImpl> : SearchValues<T> where T : struct, IEquatable<T> where TImpl : struct, INumber<TImpl>
{
	private readonly TImpl _e0;

	private readonly TImpl _e1;

	public Any2SearchValues(ReadOnlySpan<TImpl> values)
	{
		TImpl e = values[0];
		TImpl e2 = values[1];
		_e0 = e;
		_e1 = e2;
	}

	internal unsafe override T[] GetValues()
	{
		TImpl e = _e0;
		TImpl e2 = _e1;
		return new T[2]
		{
			Unsafe.Read<T>(&e),
			Unsafe.Read<T>(&e2)
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe override bool ContainsCore(T value)
	{
		if (!(Unsafe.Read<TImpl?>(&value) == _e0))
		{
			return Unsafe.Read<TImpl?>(&value) == _e1;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAny(ReadOnlySpan<T> span)
	{
		return SpanHelpers.NonPackedIndexOfAnyValueType<TImpl, SpanHelpers.DontNegate<TImpl>>(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, _e1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int IndexOfAnyExcept(ReadOnlySpan<T> span)
	{
		return SpanHelpers.NonPackedIndexOfAnyValueType<TImpl, SpanHelpers.Negate<TImpl>>(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, _e1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAny(ReadOnlySpan<T> span)
	{
		return SpanHelpers.LastIndexOfAnyValueType(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, _e1, span.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override int LastIndexOfAnyExcept(ReadOnlySpan<T> span)
	{
		return SpanHelpers.LastIndexOfAnyExceptValueType(ref Unsafe.As<T, TImpl>(ref MemoryMarshal.GetReference(span)), _e0, _e1, span.Length);
	}
}
