using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Intrinsics;

internal interface ISimdVector<TSelf, T> : IAdditionOperators<TSelf, TSelf, TSelf>, IBitwiseOperators<TSelf, TSelf, TSelf>, IDivisionOperators<TSelf, TSelf, TSelf>, IEqualityOperators<TSelf, TSelf, bool>, IEquatable<TSelf>, IMultiplyOperators<TSelf, TSelf, TSelf>, IShiftOperators<TSelf, int, TSelf>, ISubtractionOperators<TSelf, TSelf, TSelf>, IUnaryNegationOperators<TSelf, TSelf>, IUnaryPlusOperators<TSelf, TSelf> where TSelf : ISimdVector<TSelf, T>
{
	static abstract int Alignment { get; }

	static abstract TSelf AllBitsSet { get; }

	static abstract int ElementCount { get; }

	static abstract bool IsHardwareAccelerated { get; }

	static abstract bool IsSupported { get; }

	static abstract TSelf One { get; }

	static abstract TSelf Zero { get; }

	T this[int index] { get; }

	static abstract TSelf operator /(TSelf left, T right);

	static abstract TSelf operator *(TSelf left, T right);

	static abstract TSelf Abs(TSelf vector);

	static virtual TSelf Add(TSelf left, TSelf right)
	{
		return left + right;
	}

	static abstract bool All(TSelf vector, T value);

	static abstract bool AllWhereAllBitsSet(TSelf vector);

	static virtual TSelf AndNot(TSelf left, TSelf right)
	{
		return left & ~right;
	}

	static abstract bool Any(TSelf vector, T value);

	static abstract bool AnyWhereAllBitsSet(TSelf vector);

	static virtual TSelf BitwiseAnd(TSelf left, TSelf right)
	{
		return left & right;
	}

	static virtual TSelf BitwiseOr(TSelf left, TSelf right)
	{
		return left | right;
	}

	static abstract TSelf Ceiling(TSelf vector);

	static abstract TSelf Clamp(TSelf value, TSelf min, TSelf max);

	static abstract TSelf ClampNative(TSelf value, TSelf min, TSelf max);

	static virtual TSelf ConditionalSelect(TSelf condition, TSelf left, TSelf right)
	{
		return (left & condition) | (right & ~condition);
	}

	static abstract TSelf CopySign(TSelf value, TSelf sign);

	static virtual void CopyTo(TSelf vector, T[] destination)
	{
		CopyTo(vector, destination.AsSpan());
	}

	static virtual void CopyTo(TSelf vector, T[] destination, int startIndex)
	{
		CopyTo(vector, destination.AsSpan(startIndex));
	}

	static virtual void CopyTo(TSelf vector, Span<T> destination)
	{
		if (destination.Length < ElementCount)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		StoreUnsafe(vector, ref MemoryMarshal.GetReference(destination));
	}

	static abstract int Count(TSelf vector, T value);

	static abstract int CountWhereAllBitsSet(TSelf vector);

	static abstract TSelf Create(T value);

	static virtual TSelf Create(T[] values)
	{
		return Create(values.AsSpan());
	}

	static virtual TSelf Create(T[] values, int index)
	{
		return Create(values.AsSpan(index));
	}

	static virtual TSelf Create(ReadOnlySpan<T> values)
	{
		if (values.Length < ElementCount)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.values);
		}
		return LoadUnsafe(in MemoryMarshal.GetReference(values));
	}

	static virtual TSelf CreateScalar(T value)
	{
		return WithElement(Zero, 0, value);
	}

	static virtual TSelf CreateScalarUnsafe(T value)
	{
		Unsafe.SkipInit<TSelf>(out var value2);
		return WithElement(value2, 0, value);
	}

	static virtual TSelf Divide(TSelf left, TSelf right)
	{
		return left / right;
	}

	static virtual TSelf Divide(TSelf left, T right)
	{
		return left / right;
	}

	static abstract T Dot(TSelf left, TSelf right);

	static abstract TSelf Equals(TSelf left, TSelf right);

	static abstract bool EqualsAll(TSelf left, TSelf right);

	static abstract bool EqualsAny(TSelf left, TSelf right);

	static abstract TSelf Floor(TSelf vector);

	static abstract T GetElement(TSelf vector, int index);

	static abstract TSelf GreaterThan(TSelf left, TSelf right);

	static abstract bool GreaterThanAll(TSelf left, TSelf right);

	static abstract bool GreaterThanAny(TSelf left, TSelf right);

	static abstract TSelf GreaterThanOrEqual(TSelf left, TSelf right);

	static abstract bool GreaterThanOrEqualAll(TSelf left, TSelf right);

	static abstract bool GreaterThanOrEqualAny(TSelf left, TSelf right);

	static abstract int IndexOf(TSelf vector, T value);

	static abstract int IndexOfWhereAllBitsSet(TSelf vector);

	static abstract TSelf IsEvenInteger(TSelf vector);

	static abstract TSelf IsFinite(TSelf vector);

	static abstract TSelf IsInfinity(TSelf vector);

	static abstract TSelf IsInteger(TSelf vector);

	static abstract TSelf IsNaN(TSelf vector);

	static abstract TSelf IsNegative(TSelf vector);

	static abstract TSelf IsNegativeInfinity(TSelf vector);

	static abstract TSelf IsNormal(TSelf vector);

	static abstract TSelf IsOddInteger(TSelf vector);

	static abstract TSelf IsPositive(TSelf vector);

	static abstract TSelf IsPositiveInfinity(TSelf vector);

	static abstract TSelf IsSubnormal(TSelf vector);

	static abstract TSelf IsZero(TSelf vector);

	static abstract int LastIndexOf(TSelf vector, T value);

	static abstract int LastIndexOfWhereAllBitsSet(TSelf vector);

	static abstract TSelf LessThan(TSelf left, TSelf right);

	static abstract bool LessThanAll(TSelf left, TSelf right);

	static abstract bool LessThanAny(TSelf left, TSelf right);

	static abstract TSelf LessThanOrEqual(TSelf left, TSelf right);

	static abstract bool LessThanOrEqualAll(TSelf left, TSelf right);

	static abstract bool LessThanOrEqualAny(TSelf left, TSelf right);

	unsafe static virtual TSelf Load(T* source)
	{
		return LoadUnsafe(in *source);
	}

	unsafe static virtual TSelf LoadAligned(T* source)
	{
		if ((nuint)source % (nuint)(uint)Alignment != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		return LoadUnsafe(in *source);
	}

	unsafe static virtual TSelf LoadAlignedNonTemporal(T* source)
	{
		return LoadAligned(source);
	}

	static virtual TSelf LoadUnsafe(ref readonly T source)
	{
		return LoadUnsafe(in source, 0u);
	}

	static abstract TSelf LoadUnsafe(ref readonly T source, nuint elementOffset);

	static abstract TSelf Max(TSelf left, TSelf right);

	static abstract TSelf MaxMagnitude(TSelf left, TSelf right);

	static abstract TSelf MaxMagnitudeNumber(TSelf left, TSelf right);

	static abstract TSelf MaxNative(TSelf left, TSelf right);

	static abstract TSelf MaxNumber(TSelf left, TSelf right);

	static abstract TSelf Min(TSelf left, TSelf right);

	static abstract TSelf MinMagnitude(TSelf left, TSelf right);

	static abstract TSelf MinMagnitudeNumber(TSelf left, TSelf right);

	static abstract TSelf MinNative(TSelf left, TSelf right);

	static abstract TSelf MinNumber(TSelf left, TSelf right);

	static virtual TSelf Multiply(TSelf left, TSelf right)
	{
		return left * right;
	}

	static virtual TSelf Multiply(TSelf left, T right)
	{
		return left * right;
	}

	static abstract TSelf MultiplyAddEstimate(TSelf left, TSelf right, TSelf addend);

	static virtual TSelf Negate(TSelf vector)
	{
		return -vector;
	}

	static abstract bool None(TSelf vector, T value);

	static abstract bool NoneWhereAllBitsSet(TSelf vector);

	static virtual TSelf OnesComplement(TSelf vector)
	{
		return ~vector;
	}

	static abstract TSelf Round(TSelf vector);

	static virtual TSelf ShiftLeft(TSelf vector, int shiftCount)
	{
		return vector << shiftCount;
	}

	static virtual TSelf ShiftRightArithmetic(TSelf vector, int shiftCount)
	{
		return vector >> shiftCount;
	}

	static virtual TSelf ShiftRightLogical(TSelf vector, int shiftCount)
	{
		return vector >>> shiftCount;
	}

	static abstract TSelf Sqrt(TSelf vector);

	unsafe static virtual void Store(TSelf source, T* destination)
	{
		StoreUnsafe(source, ref *destination);
	}

	unsafe static virtual void StoreAligned(TSelf source, T* destination)
	{
		if ((nuint)destination % (nuint)(uint)Alignment != 0)
		{
			ThrowHelper.ThrowAccessViolationException();
		}
		StoreUnsafe(source, ref *destination);
	}

	unsafe static virtual void StoreAlignedNonTemporal(TSelf source, T* destination)
	{
		StoreAligned(source, destination);
	}

	static virtual void StoreUnsafe(TSelf vector, ref T destination)
	{
		StoreUnsafe(vector, ref destination, 0u);
	}

	static abstract void StoreUnsafe(TSelf vector, ref T destination, nuint elementOffset);

	static virtual TSelf Subtract(TSelf left, TSelf right)
	{
		return left - right;
	}

	static abstract T Sum(TSelf vector);

	static virtual T ToScalar(TSelf vector)
	{
		return GetElement(vector, 0);
	}

	static abstract TSelf Truncate(TSelf vector);

	static virtual bool TryCopyTo(TSelf vector, Span<T> destination)
	{
		if (destination.Length < ElementCount)
		{
			return false;
		}
		StoreUnsafe(vector, ref MemoryMarshal.GetReference(destination));
		return true;
	}

	static abstract TSelf WithElement(TSelf vector, int index, T value);

	static virtual TSelf Xor(TSelf left, TSelf right)
	{
		return left ^ right;
	}
}
