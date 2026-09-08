using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct TotalOrderIeee754Comparer<T> : IComparer<T>, IEqualityComparer<T>, IEquatable<TotalOrderIeee754Comparer<T>> where T : IFloatingPointIeee754<T>?
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Compare(T? x, T? y)
	{
		if (typeof(T) == typeof(float))
		{
			return CompareIntegerSemantic<int>(BitConverter.SingleToInt32Bits((float)(object)x), BitConverter.SingleToInt32Bits((float)(object)y));
		}
		if (typeof(T) == typeof(double))
		{
			return CompareIntegerSemantic<long>(BitConverter.DoubleToInt64Bits((double)(object)x), BitConverter.DoubleToInt64Bits((double)(object)y));
		}
		if (typeof(T) == typeof(Half))
		{
			return CompareIntegerSemantic<short>(BitConverter.HalfToInt16Bits((Half)(object)x), BitConverter.HalfToInt16Bits((Half)(object)y));
		}
		return CompareGeneric(x, y);
		static int CompareGeneric(T val, T val2)
		{
			if (val == null)
			{
				if (val2 != null)
				{
					return -1;
				}
				return 0;
			}
			if (val2 == null)
			{
				return 1;
			}
			if (val < val2)
			{
				return -1;
			}
			if (val > val2)
			{
				return 1;
			}
			if (val == val2)
			{
				if (T.IsZero(val))
				{
					if (T.IsNegative(val))
					{
						if (!T.IsNegative(val2))
						{
							return -1;
						}
						return 0;
					}
					return (!T.IsPositive(val2)) ? 1 : 0;
				}
				return 0;
			}
			if (T.IsNaN(val))
			{
				if (T.IsNaN(val2))
				{
					if (T.IsNegative(val))
					{
						if (!T.IsPositive(val2))
						{
							return CompareSignificand(val2, val);
						}
						return -1;
					}
					if (!T.IsNegative(val2))
					{
						return CompareSignificand(val, val2);
					}
					return 1;
				}
				if (!T.IsPositive(val))
				{
					return -1;
				}
				return 1;
			}
			if (T.IsNaN(val2))
			{
				if (!T.IsPositive(val2))
				{
					return 1;
				}
				return -1;
			}
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
			return 0;
		}
		static int CompareIntegerSemantic<TInteger>(TInteger val, TInteger val2) where TInteger : struct, IBinaryInteger<TInteger?>?, ISignedNumber<TInteger?>?
		{
			if (!((INumberBase<TInteger>)TInteger/*cast due to constrained. prefix*/).IsNegative(val) || !((INumberBase<TInteger>)TInteger/*cast due to constrained. prefix*/).IsNegative(val2))
			{
				return ((IComparable<TInteger>)val/*cast due to constrained. prefix*/).CompareTo(val2);
			}
			return ((IComparable<TInteger>)val2/*cast due to constrained. prefix*/).CompareTo(val);
		}
		static int CompareSignificand(T val, T val2)
		{
			int significandBitLength = val.GetSignificandBitLength();
			int significandBitLength2 = val2.GetSignificandBitLength();
			if (significandBitLength == significandBitLength2)
			{
				int significandByteCount = val.GetSignificandByteCount();
				int significandByteCount2 = val2.GetSignificandByteCount();
				Span<byte> span = (((uint)significandByteCount > 256u) ? ((Span<byte>)new byte[significandByteCount]) : stackalloc byte[significandByteCount]);
				Span<byte> span2 = span;
				span = (((uint)significandByteCount2 > 256u) ? ((Span<byte>)new byte[significandByteCount2]) : stackalloc byte[significandByteCount2]);
				Span<byte> span3 = span;
				val.WriteSignificandBigEndian(span2);
				val2.WriteSignificandBigEndian(span3);
				return ((ReadOnlySpan<byte>)span2).SequenceCompareTo((ReadOnlySpan<byte>)span3);
			}
			return significandBitLength.CompareTo(significandBitLength2);
		}
	}

	public bool Equals(T? x, T? y)
	{
		return Compare(x, y) == 0;
	}

	public int GetHashCode([DisallowNull] T obj)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		return obj.GetHashCode();
	}

	public bool Equals(TotalOrderIeee754Comparer<T> other)
	{
		return true;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		return obj is TotalOrderIeee754Comparer<T>;
	}

	public override int GetHashCode()
	{
		return EqualityComparer<T>.Default.GetHashCode();
	}
}
