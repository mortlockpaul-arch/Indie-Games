using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace System.Runtime.Intrinsics;

internal static class VectorMath
{
	public static TVectorDouble CosDouble<TVectorDouble, TVectorInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorDouble val = TVectorDouble.Abs(x);
		TVectorInt64 left = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val);
		TVectorDouble left2;
		if (TVectorInt64.LessThanAll(left, TVectorInt64.Create(4605249457297304857L)))
		{
			TVectorDouble right = x * x;
			left2 = ((!TVectorInt64.GreaterThanAny(left, TVectorInt64.Create(4548635623644200959L))) ? TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.5), right, TVectorDouble.One) : TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(CosDoublePoly(x), right, TVectorDouble.Create(-0.5)), right, TVectorDouble.One));
		}
		else
		{
			if (!TVectorInt64.LessThanAll(left, TVectorInt64.Create(4707126720094797824L)))
			{
				return ScalarFallback(x);
			}
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple = SinCosReduce<TVectorDouble, TVectorInt64>(val);
			TVectorDouble item = tuple.r;
			TVectorDouble item2 = tuple.rr;
			TVectorInt64 item3 = tuple.region;
			left2 = TVectorDouble.ConditionalSelect(right: SinDoubleLarge(item, item2), left: CosDoubleLarge(item, item2), condition: Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item3 & TVectorInt64.One, TVectorInt64.Zero)));
			left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals((item3 + TVectorInt64.One) & TVectorInt64.Create(2L), TVectorInt64.Zero)), +left2, -left2);
		}
		return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.GreaterThan(left, TVectorInt64.Create(4485585228861014015L))), left2, TVectorDouble.One);
		static TVectorDouble ScalarFallback(TVectorDouble val3)
		{
			TVectorDouble val2 = TVectorDouble.Zero;
			for (int i = 0; i < TVectorDouble.ElementCount; i++)
			{
				double value = double.Cos(val3[i]);
				val2 = val2.WithElement(i, value);
			}
			return val2;
		}
	}

	public static TVectorSingle CosSingle<TVectorSingle, TVectorInt32, TVectorDouble, TVectorInt64>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorSingle val = TVectorSingle.Abs(x);
		TVectorInt32 left = Unsafe.BitCast<TVectorSingle, TVectorInt32>(val);
		TVectorSingle left2;
		if (TVectorInt32.LessThanAll(left, TVectorInt32.Create(1061752796)))
		{
			if (TVectorInt32.GreaterThanAny(left, TVectorInt32.Create(1006632959)))
			{
				left2 = ((TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(CosSingleSmall(WidenLower<TVectorSingle, TVectorDouble>(x)), CosSingleSmall(WidenUpper<TVectorSingle, TVectorDouble>(x))) : Narrow<TVectorDouble, TVectorSingle>(CosSingleSmall(Widen<TVectorSingle, TVectorDouble>(x))));
			}
			else
			{
				TVectorSingle right = x * x;
				left2 = TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(-0.5f), right, TVectorSingle.One);
			}
		}
		else
		{
			if (!TVectorInt32.LessThanAll(left, TVectorInt32.Create(1251513984)))
			{
				return ScalarFallback(x);
			}
			left2 = ((TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(CoreImpl(WidenLower<TVectorSingle, TVectorDouble>(val)), CoreImpl(WidenUpper<TVectorSingle, TVectorDouble>(val))) : Narrow<TVectorDouble, TVectorSingle>(CoreImpl(Widen<TVectorSingle, TVectorDouble>(val))));
		}
		return TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorInt32, TVectorSingle>(TVectorInt32.GreaterThan(left, TVectorInt32.Create(956301311))), left2, TVectorSingle.One);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TVectorDouble CoreImpl(TVectorDouble ax)
		{
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple = SinCosReduce<TVectorDouble, TVectorInt64>(ax);
			TVectorDouble item = tuple.r;
			TVectorInt64 item2 = tuple.region;
			TVectorDouble val2 = TVectorDouble.ConditionalSelect(right: SinSinglePoly(item), left: CosSingleLarge(item), condition: Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item2 & TVectorInt64.One, TVectorInt64.Zero)));
			return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals((item2 + TVectorInt64.One) & TVectorInt64.Create(2L), TVectorInt64.Zero)), +val2, -val2);
		}
		static TVectorSingle ScalarFallback(TVectorSingle val3)
		{
			TVectorSingle val2 = TVectorSingle.Zero;
			for (int i = 0; i < TVectorSingle.ElementCount; i++)
			{
				float value = float.Cos(val3[i]);
				val2 = val2.WithElement(i, value);
			}
			return val2;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector CopySign<TVector, T>(TVector value, TVector sign) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float))
		{
			return TVector.ConditionalSelect(Create<TVector, T>(-0f), sign, value);
		}
		if (typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(Create<TVector, T>(-0.0), sign, value);
		}
		return TVector.ConditionalSelect(TVector.IsNegative(value ^ sign), -value, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector DegreesToRadians<TVector, T>(TVector degrees) where TVector : unmanaged, ISimdVector<TVector, T> where T : IFloatingPointIeee754<T>
	{
		return degrees * TVector.Create(T.Pi) / TVector.Create(T.CreateTruncating(180));
	}

	public static TVectorDouble ExpDouble<TVectorDouble, TVectorUInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		if (TVectorUInt64.LessThanOrEqualAll(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(TVectorDouble.Abs(x)), TVectorUInt64.Create(4649438849678704640uL)))
		{
			TVectorDouble val = TVectorDouble.MultiplyAddEstimate(x, TVectorDouble.Create(1.4426950408889634), TVectorDouble.Create(6755399441055744.0));
			TVectorUInt64 val2 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val);
			TVectorDouble left = val - TVectorDouble.Create(6755399441055744.0);
			TVectorDouble val3 = TVectorDouble.MultiplyAddEstimate(addend: TVectorDouble.MultiplyAddEstimate(left, TVectorDouble.Create(-355.0 / 512.0), x), left: left, right: TVectorDouble.Create(0.00021219444005469057));
			TVectorDouble val4 = val3 * val3;
			TVectorDouble val5 = val4 * val4;
			TVectorDouble right = val5 * val5;
			return TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.499430431958571E-08), val3, TVectorDouble.Create(2.7632293298250954E-07)), val4, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.7557622532543023E-06), val3, TVectorDouble.Create(2.4801486521374483E-05))), right, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.00019841269432677495), val3, TVectorDouble.Create(0.001388888895122404)), val4, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.008333333333559272), val3, TVectorDouble.Create(0.04166666666649277))), val5, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.1666666666666617), val3, TVectorDouble.Create(0.5000000000000018)), val4, val3 + TVectorDouble.One))) * Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val2 + TVectorUInt64.Create(1023uL) << 52);
		}
		return ScalarFallback(x);
		static TVectorDouble ScalarFallback(TVectorDouble val7)
		{
			TVectorDouble val6 = TVectorDouble.Zero;
			for (int i = 0; i < TVectorDouble.ElementCount; i++)
			{
				double value = double.Exp(val7[i]);
				val6 = val6.WithElement(i, value);
			}
			return val6;
		}
	}

	public static TVectorSingle ExpSingle<TVectorSingle, TVectorUInt32, TVectorDouble, TVectorUInt64>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorSingle val = ((TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(CoreImpl(WidenLower<TVectorSingle, TVectorDouble>(x)), CoreImpl(WidenUpper<TVectorSingle, TVectorDouble>(x))) : Narrow<TVectorDouble, TVectorSingle>(CoreImpl(Widen<TVectorSingle, TVectorDouble>(x))));
		if (TVectorUInt32.GreaterThanAny(Unsafe.BitCast<TVectorSingle, TVectorUInt32>(TVectorSingle.Abs(x)), TVectorUInt32.Create(1118699520u)))
		{
			val = TVectorSingle.ConditionalSelect(TVectorSingle.GreaterThan(x, TVectorSingle.Create(88.72284f)), TVectorSingle.Create(float.PositiveInfinity), val);
			val = TVectorSingle.AndNot(val, TVectorSingle.LessThan(x, TVectorSingle.Create(-103.97208f)));
		}
		return val;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TVectorDouble CoreImpl(TVectorDouble val3)
		{
			TVectorDouble val2 = val3 * TVectorDouble.Create(1.4426950408889634);
			TVectorDouble val4 = TVectorDouble.Create(6755399441055744.0);
			TVectorDouble val5 = val2 + val4;
			TVectorUInt64 val6 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val5);
			TVectorDouble val7 = val2 - (val5 - val4);
			TVectorDouble val8 = val7 * val7;
			return Unsafe.BitCast<TVectorUInt64, TVectorDouble>(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(TVectorDouble.MultiplyAddEstimate(val8 * val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.001341000536524434), val7, TVectorDouble.Create(0.009676036358193323)), TVectorDouble.MultiplyAddEstimate(val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.05550297297702539), val7, TVectorDouble.Create(0.2402210737432219)), TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.6931472254087585), val7, TVectorDouble.Create(1.0000000754895704))))) + (val6 << 52));
		}
	}

	public static TVectorDouble HypotDouble<TVectorDouble, TVectorUInt64>(TVectorDouble x, TVectorDouble y) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorDouble val = TVectorDouble.Abs(x);
		TVectorDouble val2 = TVectorDouble.Abs(y);
		TVectorDouble condition = TVectorDouble.IsPositiveInfinity(val) | TVectorDouble.IsPositiveInfinity(val2);
		TVectorDouble condition2 = TVectorDouble.IsNaN(val) | TVectorDouble.IsNaN(val2);
		TVectorUInt64 val3 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val);
		TVectorUInt64 val4 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val2);
		TVectorUInt64 val5 = TVectorUInt64.Create(2047uL);
		TVectorUInt64 val6 = (val3 >> 52) & val5;
		TVectorUInt64 val7 = (val4 >> 52) & val5;
		TVectorUInt64 val8 = val6 - val7;
		TVectorDouble condition3 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(TVectorUInt64.GreaterThanOrEqual(val8, TVectorUInt64.Create(54uL)) & TVectorUInt64.LessThanOrEqual(val8, TVectorUInt64.Create(18446744073709551562uL)));
		TVectorDouble left = val + val2;
		TVectorUInt64 right = TVectorUInt64.Create(1523uL);
		TVectorUInt64 val9 = TVectorUInt64.GreaterThan(val6, right) | TVectorUInt64.GreaterThan(val7, right);
		TVectorDouble right2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val9), TVectorDouble.Create(4.149515568880993E+180), TVectorDouble.One);
		TVectorUInt64 right3 = val9 & TVectorUInt64.Create(15744584297287254016uL);
		TVectorUInt64 right4 = TVectorUInt64.Create(523uL);
		TVectorUInt64 val10 = TVectorUInt64.AndNot(TVectorUInt64.LessThan(val6, right4) | TVectorUInt64.LessThan(val7, right4), val9);
		right2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val10), TVectorDouble.Create(2.409919865102884E-181), right2);
		right3 = TVectorUInt64.ConditionalSelect(val10, TVectorUInt64.Create(2702159776422297600uL), right3);
		val3 += right3;
		val4 += right3;
		TVectorDouble val11 = TVectorDouble.Create(9.232978617785736E-128);
		TVectorUInt64 val12 = TVectorUInt64.Create(4503599627370496uL);
		TVectorUInt64 val13 = TVectorUInt64.IsZero(val6) & val10;
		val3 += val12 & val13;
		val = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val3);
		val -= val11 & Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val13);
		TVectorUInt64 val14 = TVectorUInt64.IsZero(val7) & val10;
		val4 += val12 & val14;
		val2 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val4);
		val2 -= val11 & Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val14);
		val3 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val);
		val4 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val2);
		TVectorDouble val15 = TVectorDouble.LessThan(val, val2);
		TVectorDouble left2 = val;
		val = TVectorDouble.ConditionalSelect(val15, val2, val);
		val2 = TVectorDouble.ConditionalSelect(val15, left2, val2);
		TVectorUInt64 left3 = val3;
		val3 = TVectorUInt64.ConditionalSelect(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val15), val4, val3);
		val4 = TVectorUInt64.ConditionalSelect(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val15), left3, val4);
		TVectorUInt64 val16 = TVectorUInt64.Create(18446744073575333888uL);
		TVectorDouble val17 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val3 & val16);
		TVectorDouble val18 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val4 & val16);
		TVectorDouble val19 = val - val17;
		TVectorDouble val20 = val2 - val18;
		TVectorDouble val21 = val * val;
		TVectorDouble val22 = val2 * val2;
		TVectorDouble val23 = val21 + val22;
		TVectorDouble addend = val21 - val23 + val22;
		addend += TVectorDouble.MultiplyAddEstimate(val17, val17, -val21);
		addend = TVectorDouble.MultiplyAddEstimate(val17 * 2.0, val19, addend);
		addend = TVectorDouble.MultiplyAddEstimate(val19, val19, addend);
		TVectorDouble condition4 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>(TVectorUInt64.IsZero(val8));
		TVectorDouble addend2 = addend;
		addend2 += TVectorDouble.MultiplyAddEstimate(val18, val18, -val22);
		addend2 = TVectorDouble.MultiplyAddEstimate(val18 * 2.0, val20, addend2);
		addend2 = TVectorDouble.MultiplyAddEstimate(val20, val20, addend2);
		addend = TVectorDouble.ConditionalSelect(condition4, addend2, addend);
		TVectorDouble right5 = TVectorDouble.Sqrt(val23 + addend) * right2;
		return TVectorDouble.ConditionalSelect(right: TVectorDouble.ConditionalSelect(right: TVectorDouble.ConditionalSelect(condition3, left, right5), condition: condition2, left: TVectorDouble.Create(double.NaN)), condition: condition, left: TVectorDouble.Create(double.PositiveInfinity));
	}

	public static TVectorSingle HypotSingle<TVectorSingle, TVectorDouble>(TVectorSingle x, TVectorSingle y) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorSingle vector = TVectorSingle.Abs(x);
		TVectorSingle vector2 = TVectorSingle.Abs(y);
		return TVectorSingle.ConditionalSelect(TVectorSingle.IsPositiveInfinity(vector) | TVectorSingle.IsPositiveInfinity(vector2), right: TVectorSingle.ConditionalSelect(TVectorSingle.IsNaN(vector) | TVectorSingle.IsNaN(vector2), right: (TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(CoreImpl(WidenLower<TVectorSingle, TVectorDouble>(vector), WidenLower<TVectorSingle, TVectorDouble>(vector2)), CoreImpl(WidenUpper<TVectorSingle, TVectorDouble>(vector), WidenUpper<TVectorSingle, TVectorDouble>(vector2))) : Narrow<TVectorDouble, TVectorSingle>(CoreImpl(Widen<TVectorSingle, TVectorDouble>(vector), Widen<TVectorSingle, TVectorDouble>(vector2))), left: TVectorSingle.Create(float.NaN)), left: TVectorSingle.Create(float.PositiveInfinity));
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TVectorDouble CoreImpl(TVectorDouble val, TVectorDouble val2)
		{
			return TVectorDouble.Sqrt(TVectorDouble.MultiplyAddEstimate(val, val, val2 * val2));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVectorSingle IsEvenIntegerSingle<TVectorSingle, TVectorUInt32>(TVectorSingle vector) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint>
	{
		TVectorUInt32 val = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(TVectorSingle.Abs(vector));
		TVectorUInt32 val2 = ((val >> 23) & TVectorUInt32.Create(255u)) - TVectorUInt32.Create(127u);
		TVectorUInt32 shiftAmount = TVectorUInt32.Create(23u) - val2;
		TVectorUInt32 val3 = ShiftLeftUInt32(TVectorUInt32.One, shiftAmount);
		TVectorUInt32 val4 = val3 - TVectorUInt32.One;
		return Unsafe.BitCast<TVectorUInt32, TVectorSingle>((TVectorUInt32.GreaterThan(val, TVectorUInt32.Create(1073741823u)) & TVectorUInt32.LessThan(val, TVectorUInt32.Create(2139095040u)) & ((TVectorUInt32.IsZero(val & val4) & TVectorUInt32.IsZero(val & val3)) | TVectorUInt32.GreaterThan(val, TVectorUInt32.Create(1266679807u)))) | TVectorUInt32.IsZero(val));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVectorDouble IsEvenIntegerDouble<TVectorDouble, TVectorUInt64>(TVectorDouble vector) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorUInt64 val = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(TVectorDouble.Abs(vector));
		TVectorUInt64 val2 = ((val >> 52) & TVectorUInt64.Create(2047uL)) - TVectorUInt64.Create(1023uL);
		TVectorUInt64 shiftAmount = TVectorUInt64.Create(52uL) - val2;
		TVectorUInt64 val3 = ShiftLeftUInt64(TVectorUInt64.One, shiftAmount);
		TVectorUInt64 val4 = val3 - TVectorUInt64.One;
		return Unsafe.BitCast<TVectorUInt64, TVectorDouble>((TVectorUInt64.GreaterThan(val, TVectorUInt64.Create(4611686018427387903uL)) & TVectorUInt64.LessThan(val, TVectorUInt64.Create(9218868437227405312uL)) & ((TVectorUInt64.IsZero(val & val4) & TVectorUInt64.IsZero(val & val3)) | TVectorUInt64.GreaterThan(val, TVectorUInt64.Create(4845873199050653695uL)))) | TVectorUInt64.IsZero(val));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVectorSingle IsOddIntegerSingle<TVectorSingle, TVectorUInt32>(TVectorSingle vector) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint>
	{
		TVectorUInt32 val = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(TVectorSingle.Abs(vector));
		TVectorUInt32 val2 = ((val >> 23) & TVectorUInt32.Create(255u)) - TVectorUInt32.Create(127u);
		TVectorUInt32 shiftAmount = TVectorUInt32.Create(23u) - val2;
		TVectorUInt32 val3 = ShiftLeftUInt32(TVectorUInt32.One, shiftAmount);
		TVectorUInt32 val4 = val3 - TVectorUInt32.One;
		return Unsafe.BitCast<TVectorUInt32, TVectorSingle>(TVectorUInt32.GreaterThan(val, TVectorUInt32.Create(1065353215u)) & TVectorUInt32.LessThan(val, TVectorUInt32.Create(1266679808u)) & TVectorUInt32.IsZero(val & val4) & ~TVectorUInt32.IsZero(val & val3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVectorDouble IsOddIntegerDouble<TVectorDouble, TVectorUInt64>(TVectorDouble vector) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorUInt64 val = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(TVectorDouble.Abs(vector));
		TVectorUInt64 val2 = ((val >> 52) & TVectorUInt64.Create(2047uL)) - TVectorUInt64.Create(1023uL);
		TVectorUInt64 shiftAmount = TVectorUInt64.Create(52uL) - val2;
		TVectorUInt64 val3 = ShiftLeftUInt64(TVectorUInt64.One, shiftAmount);
		TVectorUInt64 val4 = val3 - TVectorUInt64.One;
		return Unsafe.BitCast<TVectorUInt64, TVectorDouble>(TVectorUInt64.GreaterThan(val, TVectorUInt64.Create(4607182418800017407uL)) & TVectorUInt64.LessThan(val, TVectorUInt64.Create(4845873199050653696uL)) & TVectorUInt64.IsZero(val & val4) & ~TVectorUInt64.IsZero(val & val3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector Lerp<TVector, T>(TVector x, TVector y, TVector amount) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		return TVector.MultiplyAddEstimate(x, TVector.One - amount, y * amount);
	}

	public static TVectorDouble LogDouble<TVectorDouble, TVectorInt64, TVectorUInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorDouble val = x;
		TVectorUInt64 val2 = TVectorUInt64.GreaterThanOrEqual(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x) - TVectorUInt64.Create(4503599627370496uL), TVectorUInt64.Create(9214364837600034816uL));
		if (val2 != TVectorUInt64.Zero)
		{
			TVectorDouble val3 = TVectorDouble.IsNegative(x);
			val = TVectorDouble.ConditionalSelect(val3, TVectorDouble.Create(double.NaN), val);
			TVectorDouble val4 = TVectorDouble.IsZero(x);
			val = TVectorDouble.ConditionalSelect(val4, TVectorDouble.Create(double.NegativeInfinity), val);
			TVectorDouble val5 = val4 | val3 | TVectorDouble.IsNaN(x) | TVectorDouble.IsPositiveInfinity(x);
			x = TVectorDouble.ConditionalSelect(TVectorDouble.AndNot(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val2), val5), Unsafe.BitCast<TVectorUInt64, TVectorDouble>(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x * 4503599627370496.0) - TVectorUInt64.Create(234187180623265792uL)), x);
			val2 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val5);
		}
		TVectorUInt64 val6 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x) - TVectorUInt64.Create(4604180019048437077uL);
		TVectorDouble left = ConvertToDouble<TVectorInt64, TVectorDouble>(Unsafe.BitCast<TVectorUInt64, TVectorInt64>(val6) >> 52);
		TVectorDouble val7 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>((val6 & TVectorUInt64.Create(4503599627370495uL)) + TVectorUInt64.Create(4604180019048437077uL)) - TVectorDouble.One;
		TVectorDouble val8 = val7 * val7;
		TVectorDouble val9 = val8 * val8;
		TVectorDouble val10 = val9 * val9;
		TVectorDouble addend = TVectorDouble.MultiplyAddEstimate(val10 * val10, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.08690617411690876), val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.08897063600357775), val7, TVectorDouble.Create(-0.04537017099489198)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.049370587082412105), val7, TVectorDouble.Create(-0.06399503509896004)))), TVectorDouble.MultiplyAddEstimate(val10, TVectorDouble.MultiplyAddEstimate(val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.06796346521153573), val7, TVectorDouble.Create(-0.07129671894628731)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.0768176033283113), val7, TVectorDouble.Create(-0.08334060052755186))), TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.09091434982346239), val7, TVectorDouble.Create(-0.09999975049550124)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.11111095235715944), val7, TVectorDouble.Create(-0.12500000512783127)))), TVectorDouble.MultiplyAddEstimate(val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.1428571456002771), val7, TVectorDouble.Create(-0.1666666666089195)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.19999999997598522), val7, TVectorDouble.Create(-0.25000000000029743))), TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.33333333333341475), val7, TVectorDouble.Create(-0.49999999999999956)), val8, val7))));
		return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val2), val, TVectorDouble.MultiplyAddEstimate(left, TVectorDouble.Create(355.0 / 512.0), TVectorDouble.MultiplyAddEstimate(left, TVectorDouble.Create(-0.00021219444005469057), addend)));
	}

	public static TVectorSingle LogSingle<TVectorSingle, TVectorInt32, TVectorUInt32>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint>
	{
		TVectorSingle val = x;
		TVectorUInt32 val2 = TVectorUInt32.GreaterThanOrEqual(Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x) - TVectorUInt32.Create(8388608u), TVectorUInt32.Create(2130706432u));
		if (val2 != TVectorUInt32.Zero)
		{
			TVectorSingle val3 = TVectorSingle.IsNegative(x);
			val = TVectorSingle.ConditionalSelect(val3, TVectorSingle.Create(float.NaN), val);
			TVectorSingle val4 = TVectorSingle.IsZero(x);
			val = TVectorSingle.ConditionalSelect(val4, TVectorSingle.Create(float.NegativeInfinity), val);
			TVectorSingle val5 = val4 | val3 | TVectorSingle.IsNaN(x) | TVectorSingle.IsPositiveInfinity(x);
			x = TVectorSingle.ConditionalSelect(TVectorSingle.AndNot(Unsafe.BitCast<TVectorUInt32, TVectorSingle>(val2), val5), Unsafe.BitCast<TVectorUInt32, TVectorSingle>(Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x * 8388608f) - TVectorUInt32.Create(192937984u)), x);
			val2 = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(val5);
		}
		TVectorUInt32 val6 = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x) - TVectorUInt32.Create(1059760811u);
		TVectorSingle left = ConvertToSingle<TVectorInt32, TVectorSingle>(Unsafe.BitCast<TVectorUInt32, TVectorInt32>(val6) >> 23);
		TVectorSingle val7 = Unsafe.BitCast<TVectorUInt32, TVectorSingle>((val6 & TVectorUInt32.Create(8388607u)) + TVectorUInt32.Create(1059760811u)) - TVectorSingle.Create(1f);
		TVectorSingle val8 = val7 * val7;
		TVectorSingle val9 = val8 * val8;
		TVectorSingle right = val9 * val9;
		TVectorSingle addend = TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(val8, TVectorSingle.Create(-0.13657966f), TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.14401625f), val7, TVectorSingle.Create(-0.1197452f))), right, TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.13902695f), val7, TVectorSingle.Create(-0.16700386f)), val8, TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.20018855f), val7, TVectorSingle.Create(-0.24999046f))), val9, TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.33332965f), val7, TVectorSingle.Create(-0.5000001f)), val8, val7)));
		return TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorUInt32, TVectorSingle>(val2), val, TVectorSingle.MultiplyAddEstimate(left, TVectorSingle.Create(0.6931472f), addend));
	}

	public static TVectorDouble Log2Double<TVectorDouble, TVectorInt64, TVectorUInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long> where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		TVectorDouble val = x;
		TVectorUInt64 val2 = TVectorUInt64.GreaterThanOrEqual(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x) - TVectorUInt64.Create(4503599627370496uL), TVectorUInt64.Create(9214364837600034816uL));
		if (val2 != TVectorUInt64.Zero)
		{
			TVectorDouble val3 = TVectorDouble.IsNegative(x);
			val = TVectorDouble.ConditionalSelect(val3, TVectorDouble.Create(double.NaN), val);
			TVectorDouble val4 = TVectorDouble.IsZero(x);
			val = TVectorDouble.ConditionalSelect(val4, TVectorDouble.Create(double.NegativeInfinity), val);
			TVectorDouble val5 = val4 | val3 | TVectorDouble.IsNaN(x) | TVectorDouble.IsPositiveInfinity(x);
			x = TVectorDouble.ConditionalSelect(TVectorDouble.AndNot(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val2), val5), Unsafe.BitCast<TVectorUInt64, TVectorDouble>(Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x * 4503599627370496.0) - TVectorUInt64.Create(234187180623265792uL)), x);
			val2 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(val5);
		}
		TVectorUInt64 val6 = Unsafe.BitCast<TVectorDouble, TVectorUInt64>(x) - TVectorUInt64.Create(4604180019048437077uL);
		TVectorDouble addend = ConvertToDouble<TVectorInt64, TVectorDouble>(Unsafe.BitCast<TVectorUInt64, TVectorInt64>(val6) >> 52);
		TVectorDouble val7 = Unsafe.BitCast<TVectorUInt64, TVectorDouble>((val6 & TVectorUInt64.Create(4503599627370495uL)) + TVectorUInt64.Create(4604180019048437077uL)) - TVectorDouble.One;
		TVectorDouble val8 = val7 * val7;
		TVectorDouble val9 = val8 * val8;
		TVectorDouble val10 = val9 * val9;
		TVectorDouble left = TVectorDouble.MultiplyAddEstimate(val10 * val10, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.08690617411690876), val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.08897063600357775), val7, TVectorDouble.Create(-0.04537017099489198)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.049370587082412105), val7, TVectorDouble.Create(-0.06399503509896004)))), TVectorDouble.MultiplyAddEstimate(val10, TVectorDouble.MultiplyAddEstimate(val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.06796346521153573), val7, TVectorDouble.Create(-0.07129671894628731)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.0768176033283113), val7, TVectorDouble.Create(-0.08334060052755186))), TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.09091434982346239), val7, TVectorDouble.Create(-0.09999975049550124)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.11111095235715944), val7, TVectorDouble.Create(-0.12500000512783127)))), TVectorDouble.MultiplyAddEstimate(val9, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.1428571456002771), val7, TVectorDouble.Create(-0.1666666666089195)), val8, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.19999999997598522), val7, TVectorDouble.Create(-0.25000000000029743))), TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.33333333333341475), val7, TVectorDouble.Create(-0.49999999999999956)), val8, val7))));
		return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorUInt64, TVectorDouble>(val2), val, TVectorDouble.MultiplyAddEstimate(left, TVectorDouble.Create(1.4426918029785156), TVectorDouble.MultiplyAddEstimate(left, TVectorDouble.Create(3.2379104477823597E-06), addend)));
	}

	public static TVectorSingle Log2Single<TVectorSingle, TVectorInt32, TVectorUInt32>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint>
	{
		TVectorSingle val = x;
		TVectorUInt32 val2 = TVectorUInt32.GreaterThanOrEqual(Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x) - TVectorUInt32.Create(8388608u), TVectorUInt32.Create(2130706432u));
		if (val2 != TVectorUInt32.Zero)
		{
			TVectorSingle val3 = TVectorSingle.IsNegative(x);
			val = TVectorSingle.ConditionalSelect(val3, TVectorSingle.Create(float.NaN), val);
			TVectorSingle val4 = TVectorSingle.IsZero(x);
			val = TVectorSingle.ConditionalSelect(val4, TVectorSingle.Create(float.NegativeInfinity), val);
			TVectorSingle val5 = val4 | val3 | TVectorSingle.IsNaN(x) | TVectorSingle.IsPositiveInfinity(x);
			x = TVectorSingle.ConditionalSelect(TVectorSingle.AndNot(Unsafe.BitCast<TVectorUInt32, TVectorSingle>(val2), val5), Unsafe.BitCast<TVectorUInt32, TVectorSingle>(Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x * 8388608f) - TVectorUInt32.Create(192937984u)), x);
			val2 = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(val5);
		}
		TVectorUInt32 val6 = Unsafe.BitCast<TVectorSingle, TVectorUInt32>(x) - TVectorUInt32.Create(1059760811u);
		TVectorSingle val7 = ConvertToSingle<TVectorInt32, TVectorSingle>(Unsafe.BitCast<TVectorUInt32, TVectorInt32>(val6) >> 23);
		TVectorSingle val8 = Unsafe.BitCast<TVectorUInt32, TVectorSingle>((val6 & TVectorUInt32.Create(8388607u)) + TVectorUInt32.Create(1059760811u)) - TVectorSingle.One;
		TVectorSingle val9 = val8 * val8;
		TVectorSingle val10 = val9 * val9;
		TVectorSingle right = val10 * val10;
		TVectorSingle val11 = TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.21228963f), val8, TVectorSingle.Create(-0.22616665f)), right, TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.19948183f), val8, TVectorSingle.Create(-0.23594281f)), val9, TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.2888971f), val8, TVectorSingle.Create(-0.36084408f))), val10, TVectorSingle.MultiplyAddEstimate(TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(0.48089063f), val8, TVectorSingle.Create(-0.72134554f)), val9, TVectorSingle.Create(1.4426951f) * val8)));
		return TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorUInt32, TVectorSingle>(val2), val, val7 + val11);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector Max<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.LessThan(y, x) | TVector.IsNaN(x) | (TVector.Equals(x, y) & TVector.IsNegative(y)), x, y);
		}
		return TVector.ConditionalSelect(TVector.GreaterThan(x, y), x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MaxMagnitude<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			TVector val = TVector.Abs(x);
			TVector right = TVector.Abs(y);
			return TVector.ConditionalSelect(TVector.GreaterThan(val, right) | TVector.IsNaN(val) | (TVector.Equals(val, right) & TVector.IsPositive(x)), x, y);
		}
		return MaxMagnitudeNumber<TVector, T>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MaxMagnitudeNumber<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return TVector.Max(x, y);
		}
		TVector val = TVector.Abs(x);
		TVector val2 = TVector.Abs(y);
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.GreaterThan(val, val2) | TVector.IsNaN(val2) | (TVector.Equals(val, val2) & TVector.IsPositive(x)), x, y);
		}
		return TVector.ConditionalSelect((TVector.GreaterThan(val, val2) & TVector.IsPositive(val2)) | (TVector.Equals(val, val2) & TVector.IsNegative(y)) | TVector.IsNegative(val), x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MaxNumber<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.LessThan(y, x) | TVector.IsNaN(y) | (TVector.Equals(x, y) & TVector.IsNegative(y)), x, y);
		}
		return TVector.Max(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector Min<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.LessThan(x, y) | TVector.IsNaN(x) | (TVector.Equals(x, y) & TVector.IsNegative(x)), x, y);
		}
		return TVector.ConditionalSelect(TVector.LessThan(x, y), x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MinMagnitude<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			TVector val = TVector.Abs(x);
			TVector right = TVector.Abs(y);
			return TVector.ConditionalSelect(TVector.LessThan(val, right) | TVector.IsNaN(val) | (TVector.Equals(val, right) & TVector.IsNegative(x)), x, y);
		}
		return MinMagnitudeNumber<TVector, T>(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MinMagnitudeNumber<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(byte) || typeof(T) == typeof(ushort) || typeof(T) == typeof(uint) || typeof(T) == typeof(ulong) || typeof(T) == typeof(nuint))
		{
			return TVector.Min(x, y);
		}
		TVector val = TVector.Abs(x);
		TVector val2 = TVector.Abs(y);
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.LessThan(val, val2) | TVector.IsNaN(val2) | (TVector.Equals(val, val2) & TVector.IsNegative(x)), x, y);
		}
		return TVector.ConditionalSelect((TVector.LessThan(val, val2) & TVector.IsPositive(val)) | (TVector.Equals(val, val2) & TVector.IsNegative(x)) | TVector.IsNegative(val2), x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector MinNumber<TVector, T>(TVector x, TVector y) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
		{
			return TVector.ConditionalSelect(TVector.LessThan(x, y) | TVector.IsNaN(y) | (TVector.Equals(x, y) & TVector.IsNegative(x)), x, y);
		}
		return TVector.Min(x, y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TVector RadiansToDegrees<TVector, T>(TVector radians) where TVector : unmanaged, ISimdVector<TVector, T> where T : IFloatingPointIeee754<T>
	{
		return radians * TVector.Create(T.CreateTruncating(180)) / TVector.Create(T.Pi);
	}

	public static TVectorDouble RoundDouble<TVectorDouble>(TVectorDouble vector, MidpointRounding mode) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		switch (mode)
		{
		case MidpointRounding.AwayFromZero:
			return TVectorDouble.Truncate(vector + CopySign<TVectorDouble, double>(TVectorDouble.Create(0.49999999999999994), vector));
		case MidpointRounding.ToEven:
			return TVectorDouble.Round(vector);
		case MidpointRounding.ToZero:
			return TVectorDouble.Truncate(vector);
		case MidpointRounding.ToNegativeInfinity:
			return TVectorDouble.Floor(vector);
		case MidpointRounding.ToPositiveInfinity:
			return TVectorDouble.Ceiling(vector);
		default:
			ThrowHelper.ThrowArgumentException_InvalidEnumValue(mode, "mode");
			return default(TVectorDouble);
		}
	}

	public static TVectorSingle RoundSingle<TVectorSingle>(TVectorSingle vector, MidpointRounding mode) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float>
	{
		switch (mode)
		{
		case MidpointRounding.AwayFromZero:
			return TVectorSingle.Truncate(vector + CopySign<TVectorSingle, float>(TVectorSingle.Create(0.49999997f), vector));
		case MidpointRounding.ToEven:
			return TVectorSingle.Round(vector);
		case MidpointRounding.ToZero:
			return TVectorSingle.Truncate(vector);
		case MidpointRounding.ToNegativeInfinity:
			return TVectorSingle.Floor(vector);
		case MidpointRounding.ToPositiveInfinity:
			return TVectorSingle.Ceiling(vector);
		default:
			ThrowHelper.ThrowArgumentException_InvalidEnumValue(mode, "mode");
			return default(TVectorSingle);
		}
	}

	public static (TVectorDouble Sin, TVectorDouble Cos) SinCosDouble<TVectorDouble, TVectorInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorDouble val = TVectorDouble.Abs(x);
		TVectorInt64 left = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val);
		TVectorDouble left2;
		TVectorDouble left3;
		if (TVectorInt64.LessThanAll(left, TVectorInt64.Create(4605249457297304857L)))
		{
			TVectorDouble val2 = x * x;
			if (TVectorInt64.GreaterThanAny(left, TVectorInt64.Create(4548635623644200959L)))
			{
				left2 = SinDoublePoly(x);
				left3 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(CosDoublePoly(x), val2, TVectorDouble.Create(-0.5)), val2, TVectorDouble.One);
			}
			else
			{
				TVectorDouble right = val2 * x;
				left2 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-1.0 / 6.0), right, x);
				left3 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.5), val2, TVectorDouble.One);
			}
		}
		else
		{
			if (!TVectorInt64.LessThanAll(left, TVectorInt64.Create(4707126720094797824L)))
			{
				return ScalarFallback(x);
			}
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple = SinCosReduce<TVectorDouble, TVectorInt64>(val);
			TVectorDouble item = tuple.r;
			TVectorDouble item2 = tuple.rr;
			TVectorInt64 item3 = tuple.region;
			TVectorDouble val3 = SinDoubleLarge(item, item2);
			TVectorDouble val4 = CosDoubleLarge(item, item2);
			left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item3 & TVectorInt64.One, TVectorInt64.Zero)), val3, val4);
			left3 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item3 & TVectorInt64.One, TVectorInt64.Zero)), val4, val3);
			TVectorInt64 val5 = Unsafe.BitCast<TVectorDouble, TVectorInt64>(x) >>> 63;
			left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(((val5 & (item3 >>> 1)) | (~val5 & ~(item3 >>> 1))) & TVectorInt64.One, TVectorInt64.Zero)), -left2, +left2);
			left3 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals((item3 + TVectorInt64.One) & TVectorInt64.Create(2L), TVectorInt64.Zero)), +left3, -left3);
		}
		left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.GreaterThan(left, TVectorInt64.Create(4485585228861014015L))), left2, x);
		left3 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.GreaterThan(left, TVectorInt64.Create(4485585228861014015L))), left3, TVectorDouble.One);
		return (Sin: left2, Cos: left3);
		static (TVectorDouble Sin, TVectorDouble Cos) ScalarFallback(TVectorDouble val8)
		{
			TVectorDouble val6 = TVectorDouble.Zero;
			TVectorDouble val7 = TVectorDouble.Zero;
			for (int i = 0; i < TVectorDouble.ElementCount; i++)
			{
				(double Sin, double Cos) tuple2 = double.SinCos(val8[i]);
				double item4 = tuple2.Sin;
				double item5 = tuple2.Cos;
				val6 = val6.WithElement(i, item4);
				val7 = val7.WithElement(i, item5);
			}
			return (Sin: val6, Cos: val7);
		}
	}

	public static (TVectorSingle Sin, TVectorSingle Cos) SinCosSingle<TVectorSingle, TVectorInt32, TVectorDouble, TVectorInt64>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorInt32 left = Unsafe.BitCast<TVectorSingle, TVectorInt32>(TVectorSingle.Abs(x));
		TVectorSingle left2;
		TVectorSingle left3;
		if (TVectorInt32.LessThanAll(left, TVectorInt32.Create(1061752796)))
		{
			if (TVectorInt32.GreaterThanAny(left, TVectorInt32.Create(1006632959)))
			{
				if (TVectorSingle.ElementCount == TVectorDouble.ElementCount)
				{
					TVectorDouble val = Widen<TVectorSingle, TVectorDouble>(x);
					left2 = Narrow<TVectorDouble, TVectorSingle>(SinSinglePoly(val));
					left3 = Narrow<TVectorDouble, TVectorSingle>(CosSingleSmall(val));
				}
				else
				{
					TVectorDouble val2 = WidenLower<TVectorSingle, TVectorDouble>(x);
					TVectorDouble val3 = WidenUpper<TVectorSingle, TVectorDouble>(x);
					left2 = Narrow<TVectorDouble, TVectorSingle>(SinSinglePoly(val2), SinSinglePoly(val3));
					left3 = Narrow<TVectorDouble, TVectorSingle>(CosSingleSmall(val2), CosSingleSmall(val3));
				}
			}
			else
			{
				TVectorSingle val4 = x * x;
				TVectorSingle right = val4 * x;
				left2 = TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(-1f / 6f), right, x);
				left3 = TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(-0.5f), val4, TVectorSingle.One);
			}
		}
		else
		{
			if (!TVectorInt32.LessThanAll(left, TVectorInt32.Create(1251513984)))
			{
				return ScalarFallback(x);
			}
			if (TVectorSingle.ElementCount == TVectorDouble.ElementCount)
			{
				(TVectorDouble Sin, TVectorDouble Cos) tuple = CoreImpl(Widen<TVectorSingle, TVectorDouble>(x));
				TVectorDouble item = tuple.Sin;
				TVectorDouble item2 = tuple.Cos;
				left2 = Narrow<TVectorDouble, TVectorSingle>(item);
				left3 = Narrow<TVectorDouble, TVectorSingle>(item2);
			}
			else
			{
				(TVectorDouble Sin, TVectorDouble Cos) tuple2 = CoreImpl(WidenLower<TVectorSingle, TVectorDouble>(x));
				TVectorDouble item3 = tuple2.Sin;
				TVectorDouble item4 = tuple2.Cos;
				(TVectorDouble Sin, TVectorDouble Cos) tuple3 = CoreImpl(WidenUpper<TVectorSingle, TVectorDouble>(x));
				TVectorDouble item5 = tuple3.Sin;
				TVectorDouble item6 = tuple3.Cos;
				left2 = Narrow<TVectorDouble, TVectorSingle>(item3, item5);
				left3 = Narrow<TVectorDouble, TVectorSingle>(item4, item6);
			}
		}
		left2 = TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorInt32, TVectorSingle>(TVectorInt32.GreaterThan(left, TVectorInt32.Create(956301311))), left2, x);
		left3 = TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorInt32, TVectorSingle>(TVectorInt32.GreaterThan(left, TVectorInt32.Create(956301311))), left3, TVectorSingle.One);
		return (Sin: left2, Cos: left3);
		static (TVectorDouble Sin, TVectorDouble Cos) CoreImpl(TVectorDouble val5)
		{
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple4 = SinCosReduce<TVectorDouble, TVectorInt64>(TVectorDouble.Abs(val5));
			TVectorDouble item7 = tuple4.r;
			TVectorInt64 item8 = tuple4.region;
			TVectorDouble val6 = SinSinglePoly(item7);
			TVectorDouble val7 = CosSingleLarge(item7);
			TVectorDouble val8 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item8 & TVectorInt64.One, TVectorInt64.Zero)), val6, val7);
			TVectorDouble val9 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item8 & TVectorInt64.One, TVectorInt64.Zero)), val7, val6);
			TVectorInt64 val10 = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val5) >>> 63;
			val8 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(((val10 & (item8 >>> 1)) | (~val10 & ~(item8 >>> 1))) & TVectorInt64.One, TVectorInt64.Zero)), -val8, +val8);
			val9 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals((item8 + TVectorInt64.One) & TVectorInt64.Create(2L), TVectorInt64.Zero)), +val9, -val9);
			return (Sin: val8, Cos: val9);
		}
		static (TVectorSingle Sin, TVectorSingle Cos) ScalarFallback(TVectorSingle val7)
		{
			TVectorSingle val5 = TVectorSingle.Zero;
			TVectorSingle val6 = TVectorSingle.Zero;
			for (int i = 0; i < TVectorSingle.ElementCount; i++)
			{
				(float Sin, float Cos) tuple4 = float.SinCos(val7[i]);
				float item7 = tuple4.Sin;
				float item8 = tuple4.Cos;
				val5 = val5.WithElement(i, item7);
				val6 = val6.WithElement(i, item8);
			}
			return (Sin: val5, Cos: val6);
		}
	}

	public static TVectorDouble SinDouble<TVectorDouble, TVectorInt64>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorDouble val = TVectorDouble.Abs(x);
		TVectorInt64 left = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val);
		TVectorDouble left2;
		if (TVectorInt64.LessThanAll(left, TVectorInt64.Create(4605249457297304857L)))
		{
			TVectorDouble val2 = x * x;
			if (TVectorInt64.GreaterThanAny(left, TVectorInt64.Create(4548635623644200959L)))
			{
				left2 = SinDoublePoly(x);
			}
			else
			{
				TVectorDouble right = val2 * x;
				left2 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-1.0 / 6.0), right, x);
			}
		}
		else
		{
			if (!TVectorInt64.LessThanAll(left, TVectorInt64.Create(4707126720094797824L)))
			{
				return ScalarFallback(x);
			}
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple = SinCosReduce<TVectorDouble, TVectorInt64>(val);
			TVectorDouble item = tuple.r;
			TVectorDouble item2 = tuple.rr;
			TVectorInt64 item3 = tuple.region;
			TVectorDouble left3 = SinDoubleLarge(item, item2);
			TVectorDouble right2 = CosDoubleLarge(item, item2);
			left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item3 & TVectorInt64.One, TVectorInt64.Zero)), left3, right2);
			TVectorInt64 val3 = Unsafe.BitCast<TVectorDouble, TVectorInt64>(x) >>> 63;
			left2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(((val3 & (item3 >>> 1)) | (~val3 & ~(item3 >>> 1))) & TVectorInt64.One, TVectorInt64.Zero)), -left2, +left2);
		}
		return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.GreaterThan(left, TVectorInt64.Create(4485585228861014015L))), left2, x);
		static TVectorDouble ScalarFallback(TVectorDouble val5)
		{
			TVectorDouble val4 = TVectorDouble.Zero;
			for (int i = 0; i < TVectorDouble.ElementCount; i++)
			{
				double value = double.Sin(val5[i]);
				val4 = val4.WithElement(i, value);
			}
			return val4;
		}
	}

	public static TVectorSingle SinSingle<TVectorSingle, TVectorInt32, TVectorDouble, TVectorInt64>(TVectorSingle x) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorInt32 left = Unsafe.BitCast<TVectorSingle, TVectorInt32>(TVectorSingle.Abs(x));
		TVectorSingle left2;
		if (TVectorInt32.LessThanAll(left, TVectorInt32.Create(1061752796)))
		{
			if (TVectorInt32.GreaterThanAny(left, TVectorInt32.Create(1006632959)))
			{
				left2 = ((TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(SinSinglePoly(WidenLower<TVectorSingle, TVectorDouble>(x)), SinSinglePoly(WidenUpper<TVectorSingle, TVectorDouble>(x))) : Narrow<TVectorDouble, TVectorSingle>(SinSinglePoly(Widen<TVectorSingle, TVectorDouble>(x))));
			}
			else
			{
				TVectorSingle right = x * x * x;
				left2 = TVectorSingle.MultiplyAddEstimate(TVectorSingle.Create(-1f / 6f), right, x);
			}
		}
		else
		{
			if (!TVectorInt32.LessThanAll(left, TVectorInt32.Create(1251513984)))
			{
				return ScalarFallback(x);
			}
			left2 = ((TVectorSingle.ElementCount != TVectorDouble.ElementCount) ? Narrow<TVectorDouble, TVectorSingle>(CoreImpl(WidenLower<TVectorSingle, TVectorDouble>(x)), CoreImpl(WidenUpper<TVectorSingle, TVectorDouble>(x))) : Narrow<TVectorDouble, TVectorSingle>(CoreImpl(Widen<TVectorSingle, TVectorDouble>(x))));
		}
		return TVectorSingle.ConditionalSelect(Unsafe.BitCast<TVectorInt32, TVectorSingle>(TVectorInt32.GreaterThan(left, TVectorInt32.Create(956301311))), left2, x);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TVectorDouble CoreImpl(TVectorDouble val)
		{
			(TVectorDouble r, TVectorDouble rr, TVectorInt64 region) tuple = SinCosReduce<TVectorDouble, TVectorInt64>(TVectorDouble.Abs(val));
			TVectorDouble item = tuple.r;
			TVectorInt64 item2 = tuple.region;
			TVectorDouble left3 = SinSinglePoly(item);
			TVectorDouble right2 = CosSingleLarge(item);
			TVectorDouble val2 = TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(item2 & TVectorInt64.One, TVectorInt64.Zero)), left3, right2);
			TVectorInt64 val3 = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val) >>> 63;
			return TVectorDouble.ConditionalSelect(Unsafe.BitCast<TVectorInt64, TVectorDouble>(TVectorInt64.Equals(((val3 & (item2 >>> 1)) | (~val3 & ~(item2 >>> 1))) & TVectorInt64.One, TVectorInt64.Zero)), -val2, +val2);
		}
		static TVectorSingle ScalarFallback(TVectorSingle val2)
		{
			TVectorSingle val = TVectorSingle.Zero;
			for (int i = 0; i < TVectorSingle.ElementCount; i++)
			{
				float value = float.Sin(val2[i]);
				val = val.WithElement(i, value);
			}
			return val;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble ConvertToDouble<TVectorInt64, TVectorDouble>(TVectorInt64 vector) where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		Unsafe.SkipInit<TVectorDouble>(out var value);
		if (typeof(TVectorInt64) == typeof(Vector<long>))
		{
			return (TVectorDouble)(object)Vector.ConvertToDouble((Vector<long>)(object)vector);
		}
		if (typeof(TVectorInt64) == typeof(Vector64<long>))
		{
			return (TVectorDouble)(object)Vector64.ConvertToDouble((Vector64<long>)(object)vector);
		}
		if (typeof(TVectorInt64) == typeof(Vector128<long>))
		{
			return (TVectorDouble)(object)Vector128.ConvertToDouble((Vector128<long>)(object)vector);
		}
		if (typeof(TVectorInt64) == typeof(Vector256<long>))
		{
			return (TVectorDouble)(object)Vector256.ConvertToDouble((Vector256<long>)(object)vector);
		}
		if (typeof(TVectorInt64) == typeof(Vector512<long>))
		{
			return (TVectorDouble)(object)Vector512.ConvertToDouble((Vector512<long>)(object)vector);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorSingle ConvertToSingle<TVectorInt32, TVectorSingle>(TVectorInt32 vector) where TVectorInt32 : unmanaged, ISimdVector<TVectorInt32, int> where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float>
	{
		Unsafe.SkipInit<TVectorSingle>(out var value);
		if (typeof(TVectorInt32) == typeof(Vector<int>))
		{
			return (TVectorSingle)(object)Vector.ConvertToSingle((Vector<int>)(object)vector);
		}
		if (typeof(TVectorInt32) == typeof(Vector64<int>))
		{
			return (TVectorSingle)(object)Vector64.ConvertToSingle((Vector64<int>)(object)vector);
		}
		if (typeof(TVectorInt32) == typeof(Vector128<int>))
		{
			return (TVectorSingle)(object)Vector128.ConvertToSingle((Vector128<int>)(object)vector);
		}
		if (typeof(TVectorInt32) == typeof(Vector256<int>))
		{
			return (TVectorSingle)(object)Vector256.ConvertToSingle((Vector256<int>)(object)vector);
		}
		if (typeof(TVectorInt32) == typeof(Vector512<int>))
		{
			return (TVectorSingle)(object)Vector512.ConvertToSingle((Vector512<int>)(object)vector);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble CosDoubleLarge<TVectorDouble>(TVectorDouble r, TVectorDouble rr) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble right = val * val;
		TVectorDouble val2 = val * 0.5;
		TVectorDouble val3 = val2 - TVectorDouble.One;
		return TVectorDouble.MultiplyAddEstimate(CosDoublePoly(r), right, TVectorDouble.MultiplyAddEstimate(r, rr, TVectorDouble.One + val3 - val2)) - val3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble CosDoublePoly<TVectorDouble>(TVectorDouble r) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble val2 = val * val;
		TVectorDouble right = val2 * val2;
		return TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-1.138263981623609E-11), val, TVectorDouble.Create(2.0876146382372144E-09)), right, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-2.755731727234489E-07), val, TVectorDouble.Create(2.4801587298767044E-05)), val2, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.0013888888888887398), val, TVectorDouble.Create(1.0 / 24.0))));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble CosSingleLarge<TVectorDouble>(TVectorDouble r) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble right = val * val;
		return TVectorDouble.MultiplyAddEstimate(CosSinglePoly(r), right, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.5), val, TVectorDouble.One));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble CosSinglePoly<TVectorDouble>(TVectorDouble r) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble right = val * val;
		return TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-2.755731727234489E-07), val, TVectorDouble.Create(2.4801587298767044E-05)), right, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.0013888888888887398), val, TVectorDouble.Create(1.0 / 24.0)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble CosSingleSmall<TVectorDouble>(TVectorDouble x) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = x * x;
		TVectorDouble right = val * val;
		TVectorDouble val2 = val * 0.5;
		TVectorDouble val3 = TVectorDouble.One - val2;
		TVectorDouble addend = val3 + (TVectorDouble.One - val3 - val2);
		return TVectorDouble.MultiplyAddEstimate(CosSinglePoly(x), right, addend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVector Create<TVector, T>(double value) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		Unsafe.SkipInit<TVector>(out var value2);
		if (typeof(TVector) == typeof(Vector<double>))
		{
			return (TVector)(object)Vector.Create(value);
		}
		if (typeof(TVector) == typeof(Vector64<double>))
		{
			return (TVector)(object)Vector64.Create(value);
		}
		if (typeof(TVector) == typeof(Vector128<double>))
		{
			return (TVector)(object)Vector128.Create(value);
		}
		if (typeof(TVector) == typeof(Vector256<double>))
		{
			return (TVector)(object)Vector256.Create(value);
		}
		if (typeof(TVector) == typeof(Vector512<double>))
		{
			return (TVector)(object)Vector512.Create(value);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVector Create<TVector, T>(float value) where TVector : unmanaged, ISimdVector<TVector, T>
	{
		Unsafe.SkipInit<TVector>(out var value2);
		if (typeof(TVector) == typeof(Vector<float>))
		{
			return (TVector)(object)Vector.Create(value);
		}
		if (typeof(TVector) == typeof(Vector64<float>))
		{
			return (TVector)(object)Vector64.Create(value);
		}
		if (typeof(TVector) == typeof(Vector128<float>))
		{
			return (TVector)(object)Vector128.Create(value);
		}
		if (typeof(TVector) == typeof(Vector256<float>))
		{
			return (TVector)(object)Vector256.Create(value);
		}
		if (typeof(TVector) == typeof(Vector512<float>))
		{
			return (TVector)(object)Vector512.Create(value);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorSingle Narrow<TVectorDouble, TVectorSingle>(TVectorDouble vector) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float>
	{
		Unsafe.SkipInit<TVectorSingle>(out var value);
		if (typeof(TVectorDouble) == typeof(Vector128<double>))
		{
			if (false)
			{
			}
			Vector128<double> vector2 = (Vector128<double>)(object)vector;
			return (TVectorSingle)(object)Vector64.Narrow(vector2.GetLower(), vector2.GetUpper());
		}
		if (typeof(TVectorDouble) == typeof(Vector256<double>))
		{
			if (Avx.IsSupported)
			{
				return (TVectorSingle)(object)Avx.ConvertToVector128Single((Vector256<double>)(object)vector);
			}
			Vector256<double> vector3 = (Vector256<double>)(object)vector;
			return (TVectorSingle)(object)Vector128.Narrow(vector3.GetLower(), vector3.GetUpper());
		}
		if (typeof(TVectorDouble) == typeof(Vector512<double>))
		{
			if (Avx512F.IsSupported)
			{
				return (TVectorSingle)(object)Avx512F.ConvertToVector256Single((Vector512<double>)(object)vector);
			}
			Vector512<double> vector4 = (Vector512<double>)(object)vector;
			return (TVectorSingle)(object)Vector256.Narrow(vector4.GetLower(), vector4.GetUpper());
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorSingle Narrow<TVectorDouble, TVectorSingle>(TVectorDouble lower, TVectorDouble upper) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float>
	{
		Unsafe.SkipInit<TVectorSingle>(out var value);
		if (typeof(TVectorDouble) == typeof(Vector<double>))
		{
			return (TVectorSingle)(object)Vector.Narrow((Vector<double>)(object)lower, (Vector<double>)(object)upper);
		}
		if (typeof(TVectorDouble) == typeof(Vector64<double>))
		{
			return (TVectorSingle)(object)Vector64.Narrow((Vector64<double>)(object)lower, (Vector64<double>)(object)upper);
		}
		if (typeof(TVectorDouble) == typeof(Vector128<double>))
		{
			return (TVectorSingle)(object)Vector128.Narrow((Vector128<double>)(object)lower, (Vector128<double>)(object)upper);
		}
		if (typeof(TVectorDouble) == typeof(Vector256<double>))
		{
			return (TVectorSingle)(object)Vector256.Narrow((Vector256<double>)(object)lower, (Vector256<double>)(object)upper);
		}
		if (typeof(TVectorDouble) == typeof(Vector512<double>))
		{
			return (TVectorSingle)(object)Vector512.Narrow((Vector512<double>)(object)lower, (Vector512<double>)(object)upper);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorUInt32 ShiftLeftUInt32<TVectorUInt32>(TVectorUInt32 vector, TVectorUInt32 shiftAmount) where TVectorUInt32 : unmanaged, ISimdVector<TVectorUInt32, uint>
	{
		Unsafe.SkipInit<TVectorUInt32>(out var value);
		if (typeof(TVectorUInt32) == typeof(Vector<uint>))
		{
			return (TVectorUInt32)(object)Vector.ShiftLeft((Vector<uint>)(object)vector, (Vector<uint>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt32) == typeof(Vector64<uint>))
		{
			return (TVectorUInt32)(object)Vector64.ShiftLeft((Vector64<uint>)(object)vector, (Vector64<uint>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt32) == typeof(Vector128<uint>))
		{
			return (TVectorUInt32)(object)Vector128.ShiftLeft((Vector128<uint>)(object)vector, (Vector128<uint>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt32) == typeof(Vector256<uint>))
		{
			return (TVectorUInt32)(object)Vector256.ShiftLeft((Vector256<uint>)(object)vector, (Vector256<uint>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt32) == typeof(Vector512<uint>))
		{
			return (TVectorUInt32)(object)Vector512.ShiftLeft((Vector512<uint>)(object)vector, (Vector512<uint>)(object)shiftAmount);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorUInt64 ShiftLeftUInt64<TVectorUInt64>(TVectorUInt64 vector, TVectorUInt64 shiftAmount) where TVectorUInt64 : unmanaged, ISimdVector<TVectorUInt64, ulong>
	{
		Unsafe.SkipInit<TVectorUInt64>(out var value);
		if (typeof(TVectorUInt64) == typeof(Vector<ulong>))
		{
			return (TVectorUInt64)(object)Vector.ShiftLeft((Vector<ulong>)(object)vector, (Vector<ulong>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt64) == typeof(Vector64<ulong>))
		{
			return (TVectorUInt64)(object)Vector64.ShiftLeft((Vector64<ulong>)(object)vector, (Vector64<ulong>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt64) == typeof(Vector128<ulong>))
		{
			return (TVectorUInt64)(object)Vector128.ShiftLeft((Vector128<ulong>)(object)vector, (Vector128<ulong>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt64) == typeof(Vector256<ulong>))
		{
			return (TVectorUInt64)(object)Vector256.ShiftLeft((Vector256<ulong>)(object)vector, (Vector256<ulong>)(object)shiftAmount);
		}
		if (typeof(TVectorUInt64) == typeof(Vector512<ulong>))
		{
			return (TVectorUInt64)(object)Vector512.ShiftLeft((Vector512<ulong>)(object)vector, (Vector512<ulong>)(object)shiftAmount);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble SinDoubleLarge<TVectorDouble>(TVectorDouble r, TVectorDouble rr) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble val2 = val * r;
		TVectorDouble val3 = val * val;
		TVectorDouble right = val3 * val3;
		TVectorDouble val4 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(1.5918144304485914E-10), right, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-2.5051132068021698E-08), val, TVectorDouble.Create(2.7557316103728802E-06)), val3, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-0.00019841269836761127), val, TVectorDouble.Create(0.00833333333333095))));
		return r - TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(1.0 / 6.0), val2, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(rr, TVectorDouble.Create(0.5), -(val2 * val4)), val, -rr));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble SinDoublePoly<TVectorDouble>(TVectorDouble r) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble right = val * r;
		TVectorDouble val2 = val * val;
		TVectorDouble right2 = val2 * val2;
		return TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(1.5918144304485914E-10), val, TVectorDouble.Create(-2.5051132068021698E-08)), right2, TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.7557316103728802E-06), val, TVectorDouble.Create(-0.00019841269836761127)), val2, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.00833333333333095), val, TVectorDouble.Create(-1.0 / 6.0)))), right, r);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble SinSinglePoly<TVectorDouble>(TVectorDouble r) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		TVectorDouble val = r * r;
		TVectorDouble right = val * r;
		TVectorDouble right2 = val * val;
		return TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.7557316103728802E-06), val, TVectorDouble.Create(-0.00019841269836761127)), right2, TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(0.00833333333333095), val, TVectorDouble.Create(-1.0 / 6.0))), right, r);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (TVectorDouble r, TVectorDouble rr, TVectorInt64 region) SinCosReduce<TVectorDouble, TVectorInt64>(TVectorDouble ax) where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double> where TVectorInt64 : unmanaged, ISimdVector<TVectorInt64, long>
	{
		TVectorDouble val = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.0 / Math.PI), ax, TVectorDouble.Create(6755399441055744.0));
		TVectorInt64 item = Unsafe.BitCast<TVectorDouble, TVectorInt64>(val);
		val -= TVectorDouble.Create(6755399441055744.0);
		TVectorDouble val2 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(-1.5707963267341256), val, ax);
		TVectorDouble val3 = val * 6.077100506303966E-11;
		TVectorDouble val4 = val2 - val3;
		val3 = TVectorDouble.MultiplyAddEstimate(TVectorDouble.Create(2.0222662487959506E-21), val, -(val2 - val4 - val3));
		val2 = val4;
		val4 -= val3;
		return (r: val4, rr: val2 - val4 - val3, region: item);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble Widen<TVectorSingle, TVectorDouble>(TVectorSingle vector) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		Unsafe.SkipInit<TVectorDouble>(out var value);
		if (typeof(TVectorSingle) == typeof(Vector64<float>))
		{
			if (false)
			{
			}
			Vector64<float> source = (Vector64<float>)(object)vector;
			Vector64<double> lower = Vector64.WidenLower(source);
			Vector64<double> upper = Vector64.WidenUpper(source);
			return (TVectorDouble)(object)Vector128.Create(lower, upper);
		}
		if (typeof(TVectorSingle) == typeof(Vector128<float>))
		{
			if (Avx.IsSupported)
			{
				return (TVectorDouble)(object)Avx.ConvertToVector256Double((Vector128<float>)(object)vector);
			}
			Vector128<float> source2 = (Vector128<float>)(object)vector;
			Vector128<double> lower2 = Vector128.WidenLower(source2);
			Vector128<double> upper2 = Vector128.WidenUpper(source2);
			return (TVectorDouble)(object)Vector256.Create(lower2, upper2);
		}
		if (typeof(TVectorSingle) == typeof(Vector256<float>))
		{
			if (Avx512F.IsSupported)
			{
				return (TVectorDouble)(object)Avx512F.ConvertToVector512Double((Vector256<float>)(object)vector);
			}
			Vector256<float> source3 = (Vector256<float>)(object)vector;
			Vector256<double> lower3 = Vector256.WidenLower(source3);
			Vector256<double> upper3 = Vector256.WidenUpper(source3);
			return (TVectorDouble)(object)Vector512.Create(lower3, upper3);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble WidenLower<TVectorSingle, TVectorDouble>(TVectorSingle vector) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		Unsafe.SkipInit<TVectorDouble>(out var value);
		if (typeof(TVectorSingle) == typeof(Vector<float>))
		{
			return (TVectorDouble)(object)Vector.WidenLower((Vector<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector64<float>))
		{
			return (TVectorDouble)(object)Vector64.WidenLower((Vector64<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector128<float>))
		{
			return (TVectorDouble)(object)Vector128.WidenLower((Vector128<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector256<float>))
		{
			return (TVectorDouble)(object)Vector256.WidenLower((Vector256<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector512<float>))
		{
			return (TVectorDouble)(object)Vector512.WidenLower((Vector512<float>)(object)vector);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static TVectorDouble WidenUpper<TVectorSingle, TVectorDouble>(TVectorSingle vector) where TVectorSingle : unmanaged, ISimdVector<TVectorSingle, float> where TVectorDouble : unmanaged, ISimdVector<TVectorDouble, double>
	{
		Unsafe.SkipInit<TVectorDouble>(out var value);
		if (typeof(TVectorSingle) == typeof(Vector<float>))
		{
			return (TVectorDouble)(object)Vector.WidenUpper((Vector<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector64<float>))
		{
			return (TVectorDouble)(object)Vector64.WidenUpper((Vector64<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector128<float>))
		{
			return (TVectorDouble)(object)Vector128.WidenUpper((Vector128<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector256<float>))
		{
			return (TVectorDouble)(object)Vector256.WidenUpper((Vector256<float>)(object)vector);
		}
		if (typeof(TVectorSingle) == typeof(Vector512<float>))
		{
			return (TVectorDouble)(object)Vector512.WidenUpper((Vector512<float>)(object)vector);
		}
		ThrowHelper.ThrowNotSupportedException();
		return value;
	}
}
