using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System;

[Serializable]
[NonVersionable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public readonly struct Decimal : ISpanFormattable, IFormattable, IComparable, IConvertible, IComparable<decimal>, IEquatable<decimal>, ISerializable, IDeserializationCallback, IFloatingPoint<decimal>, IFloatingPointConstants<decimal>, INumberBase<decimal>, IAdditionOperators<decimal, decimal, decimal>, IAdditiveIdentity<decimal, decimal>, IDecrementOperators<decimal>, IDivisionOperators<decimal, decimal, decimal>, IEqualityOperators<decimal, decimal, bool>, IIncrementOperators<decimal>, IMultiplicativeIdentity<decimal, decimal>, IMultiplyOperators<decimal, decimal, decimal>, ISpanParsable<decimal>, IParsable<decimal>, ISubtractionOperators<decimal, decimal, decimal>, IUnaryPlusOperators<decimal, decimal>, IUnaryNegationOperators<decimal, decimal>, IUtf8SpanFormattable, IUtf8SpanParsable<decimal>, INumber<decimal>, IComparisonOperators<decimal, decimal, bool>, IModulusOperators<decimal, decimal, decimal>, ISignedNumber<decimal>, IMinMaxValue<decimal>
{
	[StructLayout(LayoutKind.Explicit)]
	private struct DecCalc
	{
		private readonly struct PowerOvfl(uint hi, uint mid, uint lo)
		{
			public readonly uint Hi = hi;

			public readonly ulong MidLo = ((ulong)mid << 32) + lo;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct Buf12
		{
			[FieldOffset(0)]
			public uint U0;

			[FieldOffset(4)]
			public uint U1;

			[FieldOffset(8)]
			public uint U2;

			[FieldOffset(0)]
			private ulong ulo64LE;

			[FieldOffset(4)]
			private ulong uhigh64LE;

			public ulong Low64
			{
				get
				{
					return ulo64LE;
				}
				set
				{
					ulo64LE = value;
				}
			}

			public ulong High64
			{
				get
				{
					return uhigh64LE;
				}
				set
				{
					uhigh64LE = value;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct Buf16
		{
			[FieldOffset(0)]
			public uint U0;

			[FieldOffset(4)]
			public uint U1;

			[FieldOffset(8)]
			public uint U2;

			[FieldOffset(12)]
			public uint U3;

			[FieldOffset(0)]
			private ulong ulo64LE;

			[FieldOffset(8)]
			private ulong uhigh64LE;

			public ulong Low64
			{
				get
				{
					return ulo64LE;
				}
				set
				{
					ulo64LE = value;
				}
			}

			public ulong High64
			{
				get
				{
					return uhigh64LE;
				}
				set
				{
					uhigh64LE = value;
				}
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct Buf24
		{
			[FieldOffset(0)]
			public uint U0;

			[FieldOffset(4)]
			public uint U1;

			[FieldOffset(8)]
			public uint U2;

			[FieldOffset(12)]
			public uint U3;

			[FieldOffset(16)]
			public uint U4;

			[FieldOffset(20)]
			public uint U5;

			[FieldOffset(0)]
			private ulong ulo64LE;

			[FieldOffset(8)]
			private ulong umid64LE;

			[FieldOffset(16)]
			private ulong uhigh64LE;

			public ulong Low64
			{
				get
				{
					return ulo64LE;
				}
				set
				{
					ulo64LE = value;
				}
			}

			public ulong Mid64
			{
				set
				{
					umid64LE = value;
				}
			}

			public ulong High64
			{
				set
				{
					uhigh64LE = value;
				}
			}
		}

		private struct Buf28
		{
			public Buf24 Buf24;

			public uint U6;
		}

		[FieldOffset(0)]
		private uint uflags;

		[FieldOffset(4)]
		private uint uhi;

		[FieldOffset(8)]
		private uint ulo;

		[FieldOffset(12)]
		private uint umid;

		[FieldOffset(8)]
		private ulong ulomid;

		private static readonly PowerOvfl[] PowerOvflValues = new PowerOvfl[8]
		{
			new PowerOvfl(429496729u, 2576980377u, 2576980377u),
			new PowerOvfl(42949672u, 4123168604u, 687194767u),
			new PowerOvfl(4294967u, 1271310319u, 2645699854u),
			new PowerOvfl(429496u, 3133608139u, 694066715u),
			new PowerOvfl(42949u, 2890341191u, 2216890319u),
			new PowerOvfl(4294u, 4154504685u, 2369172679u),
			new PowerOvfl(429u, 2133437386u, 4102387834u),
			new PowerOvfl(42u, 4078814305u, 410238783u)
		};

		private uint High
		{
			get
			{
				return uhi;
			}
			set
			{
				uhi = value;
			}
		}

		private uint Low
		{
			get
			{
				return ulo;
			}
			set
			{
				ulo = value;
			}
		}

		private uint Mid
		{
			get
			{
				return umid;
			}
			set
			{
				umid = value;
			}
		}

		private bool IsNegative => (int)uflags < 0;

		private int Scale => (byte)(uflags >> 16);

		private ulong Low64
		{
			get
			{
				return ulomid;
			}
			set
			{
				ulomid = value;
			}
		}

		private static ReadOnlySpan<uint> UInt32Powers10 => new uint[10] { 1u, 10u, 100u, 1000u, 10000u, 100000u, 1000000u, 10000000u, 100000000u, 1000000000u };

		private static ReadOnlySpan<ulong> UInt64Powers10 => new ulong[19]
		{
			10uL, 100uL, 1000uL, 10000uL, 100000uL, 1000000uL, 10000000uL, 100000000uL, 1000000000uL, 10000000000uL,
			100000000000uL, 1000000000000uL, 10000000000000uL, 100000000000000uL, 1000000000000000uL, 10000000000000000uL, 100000000000000000uL, 1000000000000000000uL, 10000000000000000000uL
		};

		private static ReadOnlySpan<double> DoublePowers10 => new double[81]
		{
			1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0,
			10000000000.0, 100000000000.0, 1000000000000.0, 10000000000000.0, 100000000000000.0, 1000000000000000.0, 10000000000000000.0, 1E+17, 1E+18, 1E+19,
			1E+20, 1E+21, 1E+22, 1E+23, 1E+24, 1E+25, 1E+26, 1E+27, 1E+28, 1E+29,
			1E+30, 1E+31, 1E+32, 1E+33, 1E+34, 1E+35, 1E+36, 1E+37, 1E+38, 1E+39,
			1E+40, 1E+41, 1E+42, 1E+43, 1E+44, 1E+45, 1E+46, 1E+47, 1E+48, 1E+49,
			1E+50, 1E+51, 1E+52, 1E+53, 1E+54, 1E+55, 1E+56, 1E+57, 1E+58, 1E+59,
			1E+60, 1E+61, 1E+62, 1E+63, 1E+64, 1E+65, 1E+66, 1E+67, 1E+68, 1E+69,
			1E+70, 1E+71, 1E+72, 1E+73, 1E+74, 1E+75, 1E+76, 1E+77, 1E+78, 1E+79,
			1E+80
		};

		private static uint GetExponent(float f)
		{
			return (byte)(BitConverter.SingleToUInt32Bits(f) >> 23);
		}

		private static uint GetExponent(double d)
		{
			return (uint)((int)(BitConverter.DoubleToUInt64Bits(d) >> 52) & 0x7FF);
		}

		private static void UInt64x64To128(ulong a, ulong b, ref DecCalc result)
		{
			ulong num = Math.BigMul(a, b, out var low);
			if (num > uint.MaxValue)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			result.Low64 = low;
			result.High = (uint)num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static (uint Quotient, uint Remainder) Div64By32(ulong dividend, uint den)
		{
			if (X86Base.IsSupported)
			{
				return X86Base.DivRem((uint)dividend, (uint)(dividend >> 32), den);
			}
			uint num = (uint)(dividend / den);
			return (Quotient: num, Remainder: (uint)(int)dividend - num * den);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Div96By32(ref Buf12 bufNum, uint den)
		{
			uint upper;
			if (X86Base.IsSupported)
			{
				upper = 0u;
				if (bufNum.U2 == 0)
				{
					if (bufNum.U1 < den)
					{
						upper = bufNum.U1;
						bufNum.U1 = 0u;
						goto IL_0071;
					}
				}
				else
				{
					ref uint u = ref bufNum.U2;
					(u, upper) = X86Base.DivRem(bufNum.U2, upper, den);
				}
				ref uint u2 = ref bufNum.U1;
				(u2, upper) = X86Base.DivRem(bufNum.U1, upper, den);
				goto IL_0071;
			}
			ulong high;
			if (bufNum.U2 != 0)
			{
				high = bufNum.High64;
				(ulong Quotient, ulong Remainder) tuple3 = Math.DivRem(high, den);
				ulong item = tuple3.Quotient;
				ulong item2 = tuple3.Remainder;
				bufNum.High64 = item;
				high = (item2 << 32) | bufNum.U0;
				if (high == 0L)
				{
					return 0u;
				}
				(ulong Quotient, ulong Remainder) tuple4 = Math.DivRem(high, den);
				item = tuple4.Quotient;
				ulong item3 = tuple4.Remainder;
				bufNum.U0 = (uint)item;
				return (uint)item3;
			}
			high = bufNum.Low64;
			if (high == 0L)
			{
				return 0u;
			}
			(ulong, ulong) tuple5 = Math.DivRem(high, den);
			(bufNum.Low64, _) = tuple5;
			return (uint)tuple5.Item2;
			IL_0071:
			ref uint u3 = ref bufNum.U0;
			(uint, uint) tuple7 = X86Base.DivRem(bufNum.U0, upper, den);
			(u3, _) = tuple7;
			return tuple7.Item2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Div96ByConst(ref ulong high64, ref uint low, uint pow)
		{
			ulong num = high64 / pow;
			uint num2 = (uint)(((high64 - num * pow << 32) + low) / pow);
			if (low == num2 * pow)
			{
				high64 = num;
				low = num2;
				return true;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Unscale(ref uint low, ref ulong high64, ref int scale)
		{
			while ((byte)low == 0 && scale >= 8 && Div96ByConst(ref high64, ref low, 100000000u))
			{
				scale -= 8;
			}
			if ((low & 0xF) == 0 && scale >= 4 && Div96ByConst(ref high64, ref low, 10000u))
			{
				scale -= 4;
			}
			if ((low & 3) == 0 && scale >= 2 && Div96ByConst(ref high64, ref low, 100u))
			{
				scale -= 2;
			}
			if ((low & 1) == 0 && scale >= 1 && Div96ByConst(ref high64, ref low, 10u))
			{
				scale--;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong Div128By64(Buf16* bufNum, ulong den)
		{
			if (X86Base.X64.IsSupported)
			{
				ulong result;
				(result, bufNum->Low64) = X86Base.X64.DivRem(bufNum->Low64, bufNum->High64, den);
				return result;
			}
			uint num = Div96By64(ref *(Buf12*)(&bufNum->U1), den);
			uint num2 = Div96By64(ref *(Buf12*)bufNum, den);
			return ((ulong)num << 32) | num2;
		}

		private static uint Div96By64(ref Buf12 bufNum, ulong den)
		{
			if (X86Base.X64.IsSupported)
			{
				ulong num;
				(num, bufNum.Low64) = X86Base.X64.DivRem(bufNum.Low64, bufNum.U2, den);
				return (uint)num;
			}
			uint u = bufNum.U2;
			ulong low;
			if (u == 0)
			{
				low = bufNum.Low64;
				if (low < den)
				{
					return 0u;
				}
				ulong num2;
				(num2, bufNum.Low64) = Math.DivRem(low, den);
				return (uint)num2;
			}
			uint num3 = (uint)(den >> 32);
			uint num4;
			if (u >= num3)
			{
				low = bufNum.Low64;
				low -= den << 32;
				num4 = 0u;
				do
				{
					num4--;
					low += den;
				}
				while (low >= den);
				bufNum.Low64 = low;
				return num4;
			}
			ulong high = bufNum.High64;
			if (high < num3)
			{
				return 0u;
			}
			(uint Quotient, uint Remainder) tuple3 = Div64By32(high, num3);
			num4 = tuple3.Quotient;
			uint item = tuple3.Remainder;
			low = bufNum.U0 | ((ulong)item << 32);
			ulong num5 = Math.BigMul(num4, (uint)den);
			low -= num5;
			if (low > ~num5)
			{
				do
				{
					num4--;
					low += den;
				}
				while (low >= den);
			}
			bufNum.Low64 = low;
			return num4;
		}

		private static uint Div128By96(ref Buf16 bufNum, ref Buf12 bufDen)
		{
			ulong high = bufNum.High64;
			uint u = bufDen.U2;
			if (high < u)
			{
				return 0u;
			}
			(uint Quotient, uint Remainder) tuple = Div64By32(high, u);
			uint num = tuple.Quotient;
			uint item = tuple.Remainder;
			uint num2 = (uint)Math.BigMul(bufDen.Low64, num, out var low);
			ulong num3 = bufNum.Low64 - low;
			item -= num2;
			if (num3 > ~low)
			{
				item--;
				if (item >= ~num2)
				{
					goto IL_0063;
				}
			}
			else if (item > ~num2)
			{
				goto IL_0063;
			}
			goto IL_008c;
			IL_0063:
			low = bufDen.Low64;
			do
			{
				num--;
				num3 += low;
				item += u;
			}
			while ((num3 >= low || item++ >= u) && item >= u);
			goto IL_008c;
			IL_008c:
			bufNum.Low64 = num3;
			bufNum.U2 = item;
			return num;
		}

		private static uint IncreaseScale(ref Buf12 bufNum, uint power)
		{
			ulong num = Math.BigMul(bufNum.Low64, power, out var low);
			bufNum.Low64 = low;
			num = Math.BigMul(bufNum.U2, power) + num;
			bufNum.U2 = (uint)num;
			return (uint)(num >> 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void IncreaseScale(ref Buf16 bufNum, uint power)
		{
			ulong num = Math.BigMul(bufNum.Low64, power, out var low);
			bufNum.Low64 = low;
			bufNum.High64 = Math.BigMul(bufNum.U2, power) + (nuint)num;
		}

		private static void IncreaseScale64(ref Buf12 bufNum, uint power)
		{
			bufNum.U2 = (uint)Math.BigMul(bufNum.Low64, power, out var low);
			bufNum.Low64 = low;
		}

		private unsafe static int ScaleResult(Buf24* bufRes, uint hiRes, int scale)
		{
			int num = 0;
			if (hiRes > 2)
			{
				num = (int)(hiRes * 32 - 64 - 1);
				num -= BitOperations.LeadingZeroCount(((uint*)bufRes)[hiRes]);
				num = (num * 77 >> 8) + 1;
				if (num > scale)
				{
					goto IL_01cc;
				}
			}
			if (num < scale - 28)
			{
				num = scale - 28;
			}
			if (num == 0)
			{
				goto IL_01ca;
			}
			scale -= num;
			uint num2 = 0u;
			uint remainder = 0u;
			while (true)
			{
				num2 |= remainder;
				uint num3 = num switch
				{
					1 => DivByConst((uint*)bufRes, hiRes, out var quotient, out remainder, 10u), 
					2 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 100u), 
					3 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 1000u), 
					4 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 10000u), 
					5 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 100000u), 
					6 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 1000000u), 
					7 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 10000000u), 
					8 => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 100000000u), 
					_ => DivByConst((uint*)bufRes, hiRes, out quotient, out remainder, 1000000000u), 
				};
				((int*)bufRes)[hiRes] = (int)quotient;
				if (quotient == 0 && hiRes != 0)
				{
					hiRes--;
				}
				num -= 9;
				if (num > 0)
				{
					continue;
				}
				if (hiRes > 2)
				{
					if (scale == 0)
					{
						break;
					}
					num = 1;
					scale--;
					continue;
				}
				num3 >>= 1;
				if (num3 <= remainder && (num3 < remainder || ((*(uint*)bufRes & 1) | num2) != 0) && ++(*(int*)bufRes) == 0)
				{
					uint num4 = 0u;
					while (++((int*)bufRes)[++num4] == 0)
					{
					}
					if (num4 > 2)
					{
						if (scale == 0)
						{
							break;
						}
						hiRes = num4;
						num2 = 0u;
						remainder = 0u;
						num = 1;
						scale--;
						continue;
					}
				}
				goto IL_01ca;
			}
			goto IL_01cc;
			IL_01cc:
			Number.ThrowOverflowException(SR.Overflow_Decimal);
			return 0;
			IL_01ca:
			return scale;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static uint DivByConst(uint* result, uint hiRes, out uint quotient, out uint remainder, uint power)
		{
			(uint, uint) tuple = Math.DivRem(result[hiRes], power);
			quotient = tuple.Item1;
			remainder = tuple.Item2;
			uint num = hiRes - 1;
			while ((int)num >= 0)
			{
				ulong num2 = result[num] + ((ulong)remainder << 32);
				remainder = (uint)(int)num2 - (result[num] = (uint)(num2 / power)) * power;
				num--;
			}
			return power;
		}

		private static int OverflowUnscale(ref Buf12 bufQuo, int scale, bool sticky)
		{
			if (--scale < 0)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			bufQuo.U2 = 429496729u;
			long num = 25769803776L + (long)bufQuo.U1;
			long num2 = (num - (bufQuo.U1 = (uint)((ulong)num / 10uL)) * 10 << 32) + bufQuo.U0;
			uint num3 = (uint)(num2 - (bufQuo.U0 = (uint)((ulong)num2 / 10uL)) * 10);
			if (num3 > 5 || (num3 == 5 && (sticky || (bufQuo.U0 & 1) != 0)))
			{
				Add32To96(ref bufQuo, 1u);
			}
			return scale;
		}

		private static int SearchScale(ref Buf12 bufQuo, int scale)
		{
			uint u = bufQuo.U2;
			ulong low = bufQuo.Low64;
			int num = 0;
			if (u <= 429496729)
			{
				PowerOvfl[] powerOvflValues = PowerOvflValues;
				if (scale > 19)
				{
					num = 28 - scale;
					if (u < powerOvflValues[num - 1].Hi)
					{
						goto IL_00d1;
					}
				}
				else if (u < 4 || (u == 4 && low <= 5441186219426131129L))
				{
					return 9;
				}
				if (u > 42949)
				{
					if (u > 4294967)
					{
						num = 2;
						if (u > 42949672)
						{
							num--;
						}
					}
					else
					{
						num = 4;
						if (u > 429496)
						{
							num--;
						}
					}
				}
				else if (u > 429)
				{
					num = 6;
					if (u > 4294)
					{
						num--;
					}
				}
				else
				{
					num = 8;
					if (u > 42)
					{
						num--;
					}
				}
				if (u == powerOvflValues[num - 1].Hi && low > powerOvflValues[num - 1].MidLo)
				{
					num--;
				}
			}
			goto IL_00d1;
			IL_00d1:
			if (num + scale < 0)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			return num;
		}

		private static bool Add32To96(ref Buf12 bufNum, uint value)
		{
			if ((bufNum.Low64 += value) < value && ++bufNum.U2 == 0)
			{
				return false;
			}
			return true;
		}

		internal unsafe static void DecAddSub(ref DecCalc d1, ref DecCalc d2, bool sign)
		{
			ulong low = d1.Low64;
			uint num = d1.High;
			uint num2 = d1.uflags;
			uint num3 = d2.uflags;
			uint num4 = num3 ^ num2;
			sign ^= (num4 & 0x80000000u) != 0;
			int num6;
			if ((num4 & 0xFF0000) != 0)
			{
				uint num5 = num2;
				num2 = (num3 & 0xFF0000) | (num2 & 0x80000000u);
				num6 = (int)(num2 - num5) >> 16;
				if (num6 < 0)
				{
					num6 = -num6;
					num2 = num5;
					if (sign)
					{
						num2 ^= 0x80000000u;
					}
					low = d2.Low64;
					num = d2.High;
					d2 = d1;
				}
				if (num != 0)
				{
					goto IL_014c;
				}
				if (low > uint.MaxValue)
				{
					goto IL_010f;
				}
				if ((int)low == 0)
				{
					uint num7 = num2 & 0x80000000u;
					if (sign)
					{
						num7 ^= 0x80000000u;
					}
					d1 = d2;
					d1.uflags = (d2.uflags & 0xFF0000) | num7;
					return;
				}
				while (num6 > 9)
				{
					num6 -= 9;
					low = Math.BigMul((uint)low, 1000000000u);
					if (low <= uint.MaxValue)
					{
						continue;
					}
					goto IL_010f;
				}
				low = Math.BigMul((uint)low, UInt32Powers10[num6]);
			}
			goto IL_0424;
			IL_010f:
			while (true)
			{
				uint b = 1000000000u;
				if (num6 < 9)
				{
					b = UInt32Powers10[num6];
				}
				num = (uint)Math.BigMul(low, b, out low);
				if ((num6 -= 9) <= 0)
				{
					break;
				}
				if (num == 0)
				{
					continue;
				}
				goto IL_014c;
			}
			goto IL_0424;
			IL_0330:
			Buf24 value;
			value.Low64 = low;
			value.U2 = num;
			uint num8;
			num6 = ScaleResult(&value, num8, (byte)(num2 >> 16));
			num2 = (num2 & 0xFF00FFFFu) | (uint)(num6 << 16);
			low = value.Low64;
			num = value.U2;
			goto IL_048d;
			IL_014c:
			ulong num9;
			while (true)
			{
				uint b = 1000000000u;
				if (num6 < 9)
				{
					b = UInt32Powers10[num6];
				}
				num9 = Math.BigMul(low, b, out low);
				num9 += Math.BigMul(num, b);
				num6 -= 9;
				if (num9 > uint.MaxValue)
				{
					break;
				}
				num = (uint)num9;
				if (num6 > 0)
				{
					continue;
				}
				goto IL_0424;
			}
			Unsafe.SkipInit<Buf24>(out value);
			value.Low64 = low;
			value.Mid64 = num9;
			num8 = 3u;
			while (num6 > 0)
			{
				uint b = 1000000000u;
				if (num6 < 9)
				{
					b = UInt32Powers10[num6];
				}
				num9 = 0uL;
				uint* ptr = (uint*)(&value);
				uint num10 = 0u;
				do
				{
					num9 += Math.BigMul(ptr[num10], b);
					ptr[num10] = (uint)num9;
					num10++;
					num9 >>= 32;
				}
				while (num10 <= num8);
				if ((uint)num9 != 0)
				{
					ptr[++num8] = (uint)num9;
				}
				num6 -= 9;
			}
			num9 = value.Low64;
			low = d2.Low64;
			uint u = value.U2;
			num = d2.High;
			if (sign)
			{
				low = num9 - low;
				num = u - num;
				if (low > num9)
				{
					num--;
					if (num >= u)
					{
						goto IL_0293;
					}
				}
				else if (num > u)
				{
					goto IL_0293;
				}
			}
			else
			{
				low += num9;
				num += u;
				if (low < num9)
				{
					num++;
					if (num <= u)
					{
						goto IL_02f2;
					}
				}
				else if (num < u)
				{
					goto IL_02f2;
				}
			}
			goto IL_0330;
			IL_02f2:
			uint* ptr2 = (uint*)(&value);
			uint num11 = 3u;
			while (++ptr2[num11++] == 0)
			{
				if (num8 < num11)
				{
					ptr2[num11] = 1u;
					num8 = num11;
					break;
				}
			}
			goto IL_0330;
			IL_0424:
			ulong num12 = low;
			uint num13 = num;
			if (sign)
			{
				low = num12 - d2.Low64;
				num = num13 - d2.High;
				if (low > num12)
				{
					num--;
					if (num >= num13)
					{
						goto IL_0374;
					}
				}
				else if (num > num13)
				{
					goto IL_0374;
				}
			}
			else
			{
				low = num12 + d2.Low64;
				num = num13 + d2.High;
				if (low < num12)
				{
					num++;
					if (num <= num13)
					{
						goto IL_0391;
					}
				}
				else if (num < num13)
				{
					goto IL_0391;
				}
			}
			goto IL_048d;
			IL_048d:
			d1.uflags = num2;
			d1.High = num;
			d1.Low64 = low;
			return;
			IL_0374:
			num2 ^= 0x80000000u;
			num = ~num;
			low = 0L - low;
			if (low == 0L)
			{
				num++;
			}
			goto IL_048d;
			IL_0293:
			uint* ptr3 = (uint*)(&value);
			uint num14 = 3u;
			while (ptr3[num14++]-- == 0)
			{
			}
			if (ptr3[num8] != 0 || --num8 > 2)
			{
				goto IL_0330;
			}
			goto IL_048d;
			IL_0391:
			if ((num2 & 0xFF0000) == 0)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			num2 -= 65536;
			long num15 = (long)num + 4294967296L;
			num = (uint)((ulong)num15 / 10uL);
			long num16 = (num15 - num * 10 << 32) + (long)(low >> 32);
			uint num17 = (uint)((ulong)num16 / 10uL);
			long num18 = (num16 - num17 * 10 << 32) + (uint)low;
			low = num17;
			low <<= 32;
			num17 = (uint)((ulong)num18 / 10uL);
			low += num17;
			num17 = (uint)(int)num18 - num17 * 10;
			if (num17 >= 5 && (num17 > 5 || (low & 1) != 0L) && ++low == 0L)
			{
				num++;
			}
			goto IL_048d;
		}

		internal static long VarCyFromDec(ref DecCalc pdecIn)
		{
			int num = pdecIn.Scale - 4;
			long num2;
			if (num < 0)
			{
				if (pdecIn.High == 0)
				{
					uint b = UInt32Powers10[-num];
					if (Math.BigMul(pdecIn.Low64, b, out var low) == 0L)
					{
						num2 = (long)low;
						goto IL_0055;
					}
				}
			}
			else
			{
				if (num != 0)
				{
					InternalRound(ref pdecIn, (uint)num, MidpointRounding.ToEven);
				}
				if (pdecIn.High == 0)
				{
					num2 = (long)pdecIn.Low64;
					goto IL_0055;
				}
			}
			goto IL_007b;
			IL_007b:
			throw new OverflowException(SR.Overflow_Currency);
			IL_0055:
			if (num2 >= 0 || (num2 == long.MinValue && pdecIn.IsNegative))
			{
				if (pdecIn.IsNegative)
				{
					num2 = -num2;
				}
				return num2;
			}
			goto IL_007b;
		}

		internal static int VarDecCmp(in decimal d1, in decimal d2)
		{
			if ((d2.Low64 | d2.High) == 0L)
			{
				if ((d1.Low64 | d1.High) == 0L)
				{
					return 0;
				}
				return (d1._flags >> 31) | 1;
			}
			if ((d1.Low64 | d1.High) == 0L)
			{
				return -((d2._flags >> 31) | 1);
			}
			int num = (d1._flags >> 31) - (d2._flags >> 31);
			if (num != 0)
			{
				return num;
			}
			return VarDecCmpSub(in d1, in d2);
		}

		private static int VarDecCmpSub(in decimal d1, in decimal d2)
		{
			int flags = d2._flags;
			int num = (flags >> 31) | 1;
			int num2 = flags - d1._flags;
			ulong low = d1.Low64;
			uint num3 = d1.High;
			ulong num4 = d2.Low64;
			uint num5 = d2.High;
			if (num2 != 0)
			{
				num2 >>= 16;
				if (num2 < 0)
				{
					num2 = -num2;
					num = -num;
					ulong num6 = low;
					low = num4;
					num4 = num6;
					uint num7 = num3;
					num3 = num5;
					num5 = num7;
				}
				do
				{
					uint b = ((num2 >= 9) ? 1000000000u : UInt32Powers10[num2]);
					ulong num8 = Math.BigMul(low, b, out low);
					num8 += Math.BigMul(num3, b);
					if (num8 > uint.MaxValue)
					{
						return num;
					}
					num3 = (uint)num8;
				}
				while ((num2 -= 9) > 0);
			}
			uint num9 = num3 - num5;
			if (num9 != 0)
			{
				if (num9 > num3)
				{
					num = -num;
				}
				return num;
			}
			ulong num10 = low - num4;
			if (num10 == 0L)
			{
				num = 0;
			}
			else if (num10 > low)
			{
				num = -num;
			}
			return num;
		}

		internal unsafe static void VarDecMul(ref DecCalc d1, ref DecCalc d2)
		{
			int num = (byte)(d1.uflags + d2.uflags >> 16);
			Unsafe.SkipInit<Buf24>(out var value);
			uint num5;
			if ((d1.High | d1.Mid) == 0)
			{
				if ((d2.High | d2.Mid) == 0)
				{
					ulong num2 = Math.BigMul(d1.Low, d2.Low);
					if (num > 28)
					{
						if (num > 47)
						{
							goto IL_02ba;
						}
						num -= 29;
						ulong num3 = UInt64Powers10[num];
						(ulong Quotient, ulong Remainder) tuple = Math.DivRem(num2, num3);
						num2 = tuple.Quotient;
						ulong item = tuple.Remainder;
						num3 >>= 1;
						if (item >= num3 && (item > num3 || ((int)num2 & 1) != 0))
						{
							num2++;
						}
						num = 28;
					}
					d1.Low64 = num2;
					d1.uflags = ((d2.uflags ^ d1.uflags) & 0x80000000u) | (uint)(num << 16);
					return;
				}
				ulong num4 = Math.BigMul(d1.Low, d2.Low64, out var low);
				value.Low64 = low;
				if (d2.High != 0)
				{
					num4 += Math.BigMul(d1.Low, d2.High);
					if (num4 > uint.MaxValue)
					{
						value.Mid64 = num4;
						num5 = 3u;
						goto IL_026e;
					}
				}
				value.U2 = (uint)num4;
				num5 = 2u;
			}
			else if ((d2.High | d2.Mid) == 0)
			{
				ulong num4 = Math.BigMul(d1.Low64, d2.Low, out var low2);
				value.Low64 = low2;
				if (d1.High != 0)
				{
					num4 += Math.BigMul(d2.Low, d1.High);
					if (num4 > uint.MaxValue)
					{
						value.Mid64 = num4;
						num5 = 3u;
						goto IL_026e;
					}
				}
				value.U2 = (uint)num4;
				num5 = 2u;
			}
			else
			{
				ulong num6 = Math.BigMul(d1.Low64, d2.Low64, out var num4);
				value.Low64 = num4;
				if ((d1.High | d2.High) != 0)
				{
					ulong num7 = Math.BigMul(d1.High, d2.High);
					num7 += Math.BigMul(d1.Low64, d2.High, out num4);
					num6 += num4;
					if (num6 < num4)
					{
						num7++;
					}
					num7 += Math.BigMul(d2.Low64, d1.High, out num4);
					num6 += num4;
					if (num6 < num4)
					{
						num7++;
					}
					value.Mid64 = num6;
					value.High64 = num7;
					num5 = 5u;
				}
				else
				{
					value.Mid64 = num6;
					num5 = 3u;
				}
			}
			uint* ptr = (uint*)(&value);
			while (ptr[num5] == 0)
			{
				if (num5 != 0)
				{
					num5--;
					continue;
				}
				goto IL_02ba;
			}
			goto IL_026e;
			IL_026e:
			if (num5 > 2 || num > 28)
			{
				num = ScaleResult(&value, num5, num);
			}
			d1.Low64 = value.Low64;
			d1.High = value.U2;
			d1.uflags = ((d2.uflags ^ d1.uflags) & 0x80000000u) | (uint)(num << 16);
			return;
			IL_02ba:
			d1 = default(DecCalc);
		}

		internal static void VarDecFromR4(float input, out DecCalc result)
		{
			result = default(DecCalc);
			int num = (int)(GetExponent(input) - 126);
			if (num < -94)
			{
				return;
			}
			if (num > 96)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			uint num2 = 0u;
			if (input < 0f)
			{
				input = 0f - input;
				num2 = 2147483648u;
			}
			double num3 = input;
			int num4 = 6 - (num * 19728 >> 16);
			if (num4 >= 0)
			{
				if (num4 > 28)
				{
					num4 = 28;
				}
				num3 *= DoublePowers10[num4];
			}
			else if (num4 != -1 || num3 >= 10000000.0)
			{
				num3 /= DoublePowers10[-num4];
			}
			else
			{
				num4 = 0;
			}
			if (num3 < 1000000.0 && num4 < 28)
			{
				num3 *= 10.0;
				num4++;
			}
			uint num5;
			if (Sse41.IsSupported)
			{
				num5 = (uint)(int)Math.Round(num3);
			}
			else
			{
				num5 = (uint)(int)num3;
				num3 -= (double)(int)num5;
				if (num3 > 0.5 || (num3 == 0.5 && (num5 & 1) != 0))
				{
					num5++;
				}
			}
			if (num5 == 0)
			{
				return;
			}
			if (num4 < 0)
			{
				num4 = -num4;
				if (num4 < 10)
				{
					result.Low64 = Math.BigMul(num5, UInt32Powers10[num4]);
				}
				else if (num4 > 18)
				{
					UInt64x64To128(Math.BigMul(num5, UInt32Powers10[num4 - 18]), 1000000000000000000uL, ref result);
				}
				else
				{
					ulong low = Math.BigMul(num5, UInt32Powers10[num4 - 9]);
					ulong num6 = Math.BigMul(1000000000u, low, out low);
					result.Low64 = low;
					result.High = (uint)num6;
				}
			}
			else
			{
				int num7 = num4;
				if (num7 > 6)
				{
					num7 = 6;
				}
				if ((num5 & 0xF) == 0 && num7 >= 4)
				{
					(uint Quotient, uint Remainder) tuple = Math.DivRem(num5, 10000u);
					var (num8, _) = tuple;
					if (tuple.Remainder == 0)
					{
						num5 = num8;
						num4 -= 4;
						num7 -= 4;
					}
				}
				if ((num5 & 3) == 0 && num7 >= 2)
				{
					(uint Quotient, uint Remainder) tuple3 = Math.DivRem(num5, 100u);
					var (num9, _) = tuple3;
					if (tuple3.Remainder == 0)
					{
						num5 = num9;
						num4 -= 2;
						num7 -= 2;
					}
				}
				if ((num5 & 1) == 0 && num7 >= 1)
				{
					(uint Quotient, uint Remainder) tuple5 = Math.DivRem(num5, 10u);
					var (num10, _) = tuple5;
					if (tuple5.Remainder == 0)
					{
						num5 = num10;
						num4--;
					}
				}
				num2 |= (uint)(num4 << 16);
				result.Low = num5;
			}
			result.uflags = num2;
		}

		internal static void VarDecFromR8(double input, out DecCalc result)
		{
			result = default(DecCalc);
			int num = (int)(GetExponent(input) - 1022);
			if (num < -94)
			{
				return;
			}
			if (num > 96)
			{
				Number.ThrowOverflowException(SR.Overflow_Decimal);
			}
			uint num2 = 0u;
			if (input < 0.0)
			{
				input = 0.0 - input;
				num2 = 2147483648u;
			}
			double num3 = input;
			int num4 = 14 - (num * 19728 >> 16);
			if (num4 >= 0)
			{
				if (num4 > 28)
				{
					num4 = 28;
				}
				num3 *= DoublePowers10[num4];
			}
			else if (num4 != -1 || num3 >= 1000000000000000.0)
			{
				num3 /= DoublePowers10[-num4];
			}
			else
			{
				num4 = 0;
			}
			if (num3 < 100000000000000.0 && num4 < 28)
			{
				num3 *= 10.0;
				num4++;
			}
			ulong num5;
			if (Sse41.IsSupported)
			{
				num5 = (ulong)(long)Math.Round(num3);
			}
			else
			{
				num5 = (ulong)(long)num3;
				num3 -= (double)(long)num5;
				if (num3 > 0.5 || (num3 == 0.5 && (num5 & 1) != 0L))
				{
					num5++;
				}
			}
			if (num5 == 0L)
			{
				return;
			}
			if (num4 < 0)
			{
				num4 = -num4;
				if (num4 < 10)
				{
					uint b = UInt32Powers10[num4];
					ulong num6 = Math.BigMul((uint)num5, b);
					ulong num7 = Math.BigMul((uint)(num5 >> 32), b);
					result.Low = (uint)num6;
					num7 += num6 >> 32;
					result.Mid = (uint)num7;
					num7 >>= 32;
					result.High = (uint)num7;
				}
				else
				{
					UInt64x64To128(num5, UInt64Powers10[num4 - 1], ref result);
				}
			}
			else
			{
				int num8 = num4;
				if (num8 > 14)
				{
					num8 = 14;
				}
				if ((byte)num5 == 0 && num8 >= 8)
				{
					ulong num9 = num5 / 100000000;
					if ((uint)num5 == (uint)(num9 * 100000000))
					{
						num5 = num9;
						num4 -= 8;
						num8 -= 8;
					}
				}
				if (((int)num5 & 0xF) == 0 && num8 >= 4)
				{
					ulong num10 = num5 / 10000;
					if ((uint)num5 == (uint)(num10 * 10000))
					{
						num5 = num10;
						num4 -= 4;
						num8 -= 4;
					}
				}
				if (((int)num5 & 3) == 0 && num8 >= 2)
				{
					ulong num11 = num5 / 100;
					if ((uint)num5 == (uint)(num11 * 100))
					{
						num5 = num11;
						num4 -= 2;
						num8 -= 2;
					}
				}
				if (((int)num5 & 1) == 0 && num8 >= 1)
				{
					ulong num12 = num5 / 10;
					if ((uint)num5 == (uint)(num12 * 10))
					{
						num5 = num12;
						num4--;
					}
				}
				num2 |= (uint)(num4 << 16);
				result.Low64 = num5;
			}
			result.uflags = num2;
		}

		internal static float VarR4FromDec(in decimal value)
		{
			return (float)VarR8FromDec(in value);
		}

		internal static double VarR8FromDec(in decimal value)
		{
			double num = ((double)value.Low64 + (double)value.High * 1.8446744073709552E+19) / DoublePowers10[value.Scale];
			if (IsNegative(value))
			{
				num = 0.0 - num;
			}
			return num;
		}

		internal static int GetHashCode(in decimal d)
		{
			if ((d.Low64 | d.High) == 0L)
			{
				return 0;
			}
			uint flags = (uint)d._flags;
			if ((flags & 0xFF0000) == 0 || (d.Low & 1) != 0)
			{
				return (int)(flags ^ d.High ^ d.Mid ^ d.Low);
			}
			int scale = (byte)(flags >> 16);
			uint low = d.Low;
			ulong high = ((ulong)d.High << 32) | d.Mid;
			Unscale(ref low, ref high, ref scale);
			flags = (flags & 0xFF00FFFFu) | (uint)(scale << 16);
			return (int)(flags ^ (uint)(int)(high >> 32) ^ (uint)(int)high ^ low);
		}

		internal unsafe static void VarDecDiv(ref DecCalc d1, ref DecCalc d2)
		{
			Unsafe.SkipInit<Buf12>(out var value);
			int scale = (sbyte)(d1.uflags - d2.uflags >> 16);
			bool flag = false;
			uint low;
			uint num;
			Buf16 value3;
			ulong num5;
			Buf12 value4;
			uint num4;
			if ((d2.High | d2.Mid) == 0)
			{
				low = d2.Low;
				if (low == 0)
				{
					throw new DivideByZeroException();
				}
				value.Low64 = d1.Low64;
				value.U2 = d1.High;
				num = Div96By32(ref value, low);
				while (true)
				{
					int num2;
					if (num == 0)
					{
						if (scale >= 0)
						{
							break;
						}
						num2 = Math.Min(9, -scale);
					}
					else
					{
						flag = true;
						if (scale == 28 || (num2 = SearchScale(ref value, scale)) == 0)
						{
							goto IL_0090;
						}
					}
					uint num3 = UInt32Powers10[num2];
					scale += num2;
					if (IncreaseScale(ref value, num3) == 0)
					{
						uint value2;
						(value2, num) = Div64By32(Math.BigMul(num, num3), low);
						if (!Add32To96(ref value, value2))
						{
							scale = OverflowUnscale(ref value, scale, num != 0);
							break;
						}
						continue;
					}
					goto IL_04a8;
				}
			}
			else
			{
				num4 = d2.High;
				if (num4 == 0)
				{
					num4 = d2.Mid;
				}
				int num2 = BitOperations.LeadingZeroCount(num4);
				Unsafe.SkipInit<Buf16>(out value3);
				value3.Low64 = d1.Low64 << num2;
				value3.High64 = d1.Mid + ((ulong)d1.High << 32) >> 32 - num2;
				num5 = d2.Low64 << num2;
				if (d2.High == 0)
				{
					value.U2 = 0u;
					value.Low64 = Div128By64(&value3, num5);
					while (true)
					{
						if (value3.Low64 == 0L)
						{
							if (scale >= 0)
							{
								break;
							}
							num2 = Math.Min(9, -scale);
						}
						else
						{
							flag = true;
							if (scale == 28 || (num2 = SearchScale(ref value, scale)) == 0)
							{
								goto IL_01db;
							}
						}
						uint num3 = UInt32Powers10[num2];
						scale += num2;
						if (IncreaseScale(ref value, num3) == 0)
						{
							IncreaseScale64(ref *(Buf12*)(&value3), num3);
							num4 = Div96By64(ref *(Buf12*)(&value3), num5);
							if (!Add32To96(ref value, num4))
							{
								scale = OverflowUnscale(ref value, scale, value3.Low64 != 0);
								break;
							}
							continue;
						}
						goto IL_04a8;
					}
				}
				else
				{
					Unsafe.SkipInit<Buf12>(out value4);
					value4.Low64 = num5;
					value4.U2 = (uint)(d2.Mid + ((ulong)d2.High << 32) >> 32 - num2);
					value.Low64 = Div128By96(ref value3, ref value4);
					value.U2 = 0u;
					while (true)
					{
						if ((value3.Low64 | value3.U2) == 0L)
						{
							if (scale >= 0)
							{
								break;
							}
							num2 = Math.Min(9, -scale);
						}
						else
						{
							flag = true;
							if (scale == 28 || (num2 = SearchScale(ref value, scale)) == 0)
							{
								goto IL_02fe;
							}
						}
						uint num3 = UInt32Powers10[num2];
						scale += num2;
						if (IncreaseScale(ref value, num3) == 0)
						{
							IncreaseScale(ref value3, num3);
							num4 = Div128By96(ref value3, ref value4);
							if (!Add32To96(ref value, num4))
							{
								scale = OverflowUnscale(ref value, scale, (value3.Low64 | value3.High64) != 0);
								break;
							}
							continue;
						}
						goto IL_04a8;
					}
				}
			}
			goto IL_03f0;
			IL_02fe:
			if ((int)value3.U2 >= 0)
			{
				num4 = value3.U1 >> 31;
				value3.Low64 <<= 1;
				value3.U2 = (value3.U2 << 1) + num4;
				if (value3.U2 <= value4.U2 && (value3.U2 != value4.U2 || (value3.Low64 <= value4.Low64 && (value3.Low64 != value4.Low64 || (value.U0 & 1) == 0))))
				{
					goto IL_03f0;
				}
			}
			goto IL_0467;
			IL_03f0:
			if (flag)
			{
				uint low2 = value.U0;
				ulong high = value.High64;
				Unscale(ref low2, ref high, ref scale);
				d1.Low = low2;
				d1.Mid = (uint)high;
				d1.High = (uint)(high >> 32);
			}
			else
			{
				d1.Low64 = value.Low64;
				d1.High = value.U2;
			}
			d1.uflags = ((d1.uflags ^ d2.uflags) & 0x80000000u) | (uint)(scale << 16);
			return;
			IL_0467:
			if (++value.Low64 == 0L && ++value.U2 == 0)
			{
				scale = OverflowUnscale(ref value, scale, sticky: true);
			}
			goto IL_03f0;
			IL_04a8:
			Number.ThrowOverflowException(SR.Overflow_Decimal);
			return;
			IL_01db:
			ulong low3 = value3.Low64;
			if ((long)low3 >= 0L && (low3 <<= 1) <= num5 && (low3 != num5 || (value.U0 & 1) == 0))
			{
				goto IL_03f0;
			}
			goto IL_0467;
			IL_0090:
			num4 = num << 1;
			if (num4 >= num && (num4 < low || (num4 <= low && (value.U0 & 1) == 0)))
			{
				goto IL_03f0;
			}
			goto IL_0467;
		}

		internal static void VarDecMod(ref DecCalc d1, ref DecCalc d2)
		{
			if ((d2.ulo | d2.umid | d2.uhi) == 0)
			{
				throw new DivideByZeroException();
			}
			if ((d1.ulo | d1.umid | d1.uhi) == 0)
			{
				return;
			}
			d2.uflags = (d2.uflags & 0x7FFFFFFF) | (d1.uflags & 0x80000000u);
			int num = VarDecCmpSub(in Unsafe.As<DecCalc, decimal>(ref d1), in Unsafe.As<DecCalc, decimal>(ref d2));
			if (num == 0)
			{
				d1.ulo = 0u;
				d1.umid = 0u;
				d1.uhi = 0u;
				if (d2.uflags > d1.uflags)
				{
					d1.uflags = d2.uflags;
				}
			}
			else
			{
				if ((int)((uint)num ^ (d1.uflags & 0x80000000u)) < 0)
				{
					return;
				}
				int num2 = (sbyte)(d1.uflags - d2.uflags >> 16);
				if (num2 > 0)
				{
					do
					{
						uint num3 = ((num2 >= 9) ? 1000000000u : UInt32Powers10[num2]);
						uint num4 = (uint)Math.BigMul(d2.Low64, num3, out var low);
						d2.Low64 = low;
						d2.High = num4 + d2.High * num3;
					}
					while ((num2 -= 9) > 0);
					num2 = 0;
				}
				do
				{
					if (num2 < 0)
					{
						d1.uflags = d2.uflags;
						Unsafe.SkipInit<Buf12>(out var value);
						value.Low64 = d1.Low64;
						value.U2 = d1.High;
						uint num6;
						do
						{
							int num5 = SearchScale(ref value, 28 + num2);
							if (num5 == 0)
							{
								break;
							}
							num6 = ((num5 >= 9) ? 1000000000u : UInt32Powers10[num5]);
							num2 += num5;
							IncreaseScale(ref value, num6);
						}
						while (num6 == 1000000000 && num2 < 0);
						d1.Low64 = value.Low64;
						d1.High = value.U2;
					}
					if (d1.High == 0)
					{
						d1.Low64 %= d2.Low64;
						break;
					}
					if ((d2.High | d2.Mid) == 0)
					{
						uint low2 = d2.Low;
						ulong num7 = ((ulong)d1.High << 32) | d1.Mid;
						num7 = (num7 % low2 << 32) | d1.Low;
						d1.Low64 = num7 % low2;
						d1.High = 0u;
						continue;
					}
					VarDecModFull(ref d1, ref d2, num2);
					break;
				}
				while (num2 < 0);
			}
		}

		private unsafe static void VarDecModFull(ref DecCalc d1, ref DecCalc d2, int scale)
		{
			uint num = d2.High;
			if (num == 0)
			{
				num = d2.Mid;
			}
			int num2 = BitOperations.LeadingZeroCount(num);
			Unsafe.SkipInit<Buf28>(out var value);
			value.Buf24.Low64 = d1.Low64 << num2;
			value.Buf24.Mid64 = d1.Mid + ((ulong)d1.High << 32) >> 32 - num2;
			uint num3 = 3u;
			while (scale < 0)
			{
				uint b = ((scale <= -9) ? 1000000000u : UInt32Powers10[-scale]);
				uint* ptr = (uint*)(&value);
				ulong num4 = Math.BigMul(value.Buf24.U0, b);
				value.Buf24.U0 = (uint)num4;
				for (int i = 1; i <= num3; i++)
				{
					num4 >>= 32;
					num4 += Math.BigMul(ptr[i], b);
					ptr[i] = (uint)num4;
				}
				if (num4 > int.MaxValue)
				{
					ptr[++num3] = (uint)(num4 >> 32);
				}
				scale += 9;
			}
			if (d2.High == 0)
			{
				ulong den = d2.Low64 << num2;
				switch (num3)
				{
				case 6u:
					Div96By64(ref *(Buf12*)(&value.Buf24.U4), den);
					goto case 5u;
				case 5u:
					Div96By64(ref *(Buf12*)(&value.Buf24.U3), den);
					goto case 4u;
				case 4u:
					Div96By64(ref *(Buf12*)(&value.Buf24.U2), den);
					break;
				}
				Div96By64(ref *(Buf12*)(&value.Buf24.U1), den);
				Div96By64(ref *(Buf12*)(&value), den);
				d1.Low64 = value.Buf24.Low64 >> num2;
				d1.High = 0u;
				return;
			}
			Unsafe.SkipInit<Buf12>(out var value2);
			value2.Low64 = d2.Low64 << num2;
			value2.U2 = (uint)(d2.Mid + ((ulong)d2.High << 32) >> 32 - num2);
			switch (num3)
			{
			case 6u:
				Div128By96(ref *(Buf16*)(&value.Buf24.U3), ref value2);
				goto case 5u;
			case 5u:
				Div128By96(ref *(Buf16*)(&value.Buf24.U2), ref value2);
				goto case 4u;
			case 4u:
				Div128By96(ref *(Buf16*)(&value.Buf24.U1), ref value2);
				break;
			}
			Div128By96(ref *(Buf16*)(&value), ref value2);
			d1.Low64 = (value.Buf24.Low64 >> num2) + ((ulong)value.Buf24.U2 << 32 - num2 << 32);
			d1.High = value.Buf24.U2 >> num2;
		}

		internal static void InternalRound(ref DecCalc d, uint scale, MidpointRounding mode)
		{
			d.uflags -= scale << 16;
			uint num = 0u;
			while (true)
			{
				uint num6;
				uint num5;
				if (scale >= 9)
				{
					scale -= 9;
					uint num2 = d.uhi;
					if (num2 == 0)
					{
						ulong low = d.Low64;
						ulong num3 = (d.Low64 = low / 1000000000);
						num5 = (uint)(low - num3 * 1000000000);
					}
					else
					{
						ref uint reference = ref d.uhi;
						(uint, uint) tuple = Math.DivRem(num2, 1000000000u);
						reference = tuple.Item1;
						num5 = tuple.Item2;
						num2 = d.umid;
						if ((num2 | num5) != 0)
						{
							num5 = num2 - (d.umid = (uint)((((ulong)num5 << 32) | num2) / 1000000000)) * 1000000000;
						}
						num2 = d.ulo;
						if ((num2 | num5) != 0)
						{
							num5 = num2 - (d.ulo = (uint)((((ulong)num5 << 32) | num2) / 1000000000)) * 1000000000;
						}
					}
					num6 = 1000000000u;
					if (scale != 0)
					{
						num |= num5;
						continue;
					}
				}
				else
				{
					num6 = UInt32Powers10[(int)scale];
					uint num7 = d.uhi;
					if (num7 == 0)
					{
						ulong low2 = d.Low64;
						if (low2 == 0L)
						{
							if (mode <= MidpointRounding.ToZero)
							{
								break;
							}
							num5 = 0u;
						}
						else
						{
							ulong num8 = (d.Low64 = low2 / num6);
							num5 = (uint)(low2 - num8 * num6);
						}
					}
					else
					{
						ref uint reference2 = ref d.uhi;
						(uint, uint) tuple = Math.DivRem(num7, num6);
						reference2 = tuple.Item1;
						num5 = tuple.Item2;
						num7 = d.umid;
						if ((num7 | num5) != 0)
						{
							num5 = num7 - (d.umid = (uint)((((ulong)num5 << 32) | num7) / num6)) * num6;
						}
						num7 = d.ulo;
						if ((num7 | num5) != 0)
						{
							num5 = num7 - (d.ulo = (uint)((((ulong)num5 << 32) | num7) / num6)) * num6;
						}
					}
				}
				switch (mode)
				{
				case MidpointRounding.ToEven:
					num5 <<= 1;
					if ((num | (d.ulo & 1)) != 0)
					{
						num5++;
					}
					if (num6 >= num5)
					{
						break;
					}
					goto IL_01ff;
				case MidpointRounding.AwayFromZero:
					num5 <<= 1;
					if (num6 > num5)
					{
						break;
					}
					goto IL_01ff;
				case MidpointRounding.ToNegativeInfinity:
					if ((num5 | num) == 0 || !d.IsNegative)
					{
						break;
					}
					goto IL_01ff;
				default:
					if ((num5 | num) == 0 || d.IsNegative)
					{
						break;
					}
					goto IL_01ff;
				case MidpointRounding.ToZero:
					break;
					IL_01ff:
					if (++d.Low64 == 0L)
					{
						d.uhi++;
					}
					break;
				}
				break;
			}
		}

		internal static uint DecDivMod1E9(ref DecCalc value)
		{
			ulong num = ((ulong)value.uhi << 32) + value.umid;
			ulong num2 = num / 1000000000;
			value.uhi = (uint)(num2 >> 32);
			value.umid = (uint)num2;
			ulong num3 = (num - (uint)((int)num2 * 1000000000) << 32) + value.ulo;
			return (uint)(int)num3 - (value.ulo = (uint)(num3 / 1000000000)) * 1000000000;
		}
	}

	public const decimal Zero = 0m;

	public const decimal One = 1m;

	public const decimal MinusOne = -1m;

	public const decimal MaxValue = 79228162514264337593543950335m;

	public const decimal MinValue = -79228162514264337593543950335m;

	private const decimal AdditiveIdentity = 0m;

	private const decimal MultiplicativeIdentity = 1m;

	private const decimal NegativeOne = -1m;

	private readonly int _flags;

	private readonly uint _hi32;

	private readonly ulong _lo64;

	public byte Scale => (byte)(_flags >> 16);

	private sbyte Exponent => (sbyte)(95 - Scale);

	static decimal IAdditiveIdentity<decimal, decimal>.AdditiveIdentity => 0m;

	static decimal IFloatingPointConstants<decimal>.E => 2.7182818284590452353602874714m;

	static decimal IFloatingPointConstants<decimal>.Pi => 3.1415926535897932384626433833m;

	static decimal IFloatingPointConstants<decimal>.Tau => 6.2831853071795864769252867666m;

	static decimal IMinMaxValue<decimal>.MinValue => decimal.MinValue;

	static decimal IMinMaxValue<decimal>.MaxValue => decimal.MaxValue;

	static decimal IMultiplicativeIdentity<decimal, decimal>.MultiplicativeIdentity => 1m;

	static decimal INumberBase<decimal>.One => 1m;

	static int INumberBase<decimal>.Radix => 10;

	static decimal INumberBase<decimal>.Zero => 0m;

	static decimal ISignedNumber<decimal>.NegativeOne => -1m;

	internal uint High => _hi32;

	internal uint Low => (uint)_lo64;

	internal uint Mid => (uint)(_lo64 >> 32);

	internal ulong Low64 => _lo64;

	internal Decimal(Currency value)
	{
		this = FromOACurrency(value.m_value);
	}

	public Decimal(int value)
	{
		if (value >= 0)
		{
			_flags = 0;
		}
		else
		{
			_flags = int.MinValue;
			value = -value;
		}
		_lo64 = (uint)value;
		_hi32 = 0u;
	}

	[CLSCompliant(false)]
	public Decimal(uint value)
	{
		_flags = 0;
		_lo64 = value;
		_hi32 = 0u;
	}

	public Decimal(long value)
	{
		if (value >= 0)
		{
			_flags = 0;
		}
		else
		{
			_flags = int.MinValue;
			value = -value;
		}
		_lo64 = (ulong)value;
		_hi32 = 0u;
	}

	[CLSCompliant(false)]
	public Decimal(ulong value)
	{
		_flags = 0;
		_lo64 = value;
		_hi32 = 0u;
	}

	public Decimal(float value)
	{
		DecCalc.VarDecFromR4(value, out AsMutable(ref this));
	}

	public Decimal(double value)
	{
		DecCalc.VarDecFromR8(value, out AsMutable(ref this));
	}

	private Decimal(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		_flags = info.GetInt32("flags");
		_hi32 = (uint)info.GetInt32("hi");
		_lo64 = (ulong)((uint)info.GetInt32("lo") + ((long)info.GetInt32("mid") << 32));
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		info.AddValue("flags", _flags);
		info.AddValue("hi", (int)High);
		info.AddValue("lo", (int)Low);
		info.AddValue("mid", (int)Mid);
	}

	public static decimal FromOACurrency(long cy)
	{
		bool isNegative = false;
		ulong num;
		if (cy < 0)
		{
			isNegative = true;
			num = (ulong)(-cy);
		}
		else
		{
			num = (ulong)cy;
		}
		int num2 = 4;
		if (num != 0L)
		{
			while (num2 != 0 && num % 10 == 0L)
			{
				num2--;
				num /= 10;
			}
		}
		return new decimal((int)num, (int)(num >> 32), 0, isNegative, (byte)num2);
	}

	public static long ToOACurrency(decimal value)
	{
		return DecCalc.VarCyFromDec(ref AsMutable(ref value));
	}

	private static bool IsValid(int flags)
	{
		if ((flags & 0x7F00FFFF) == 0)
		{
			return (uint)(flags & 0xFF0000) <= 1835008u;
		}
		return false;
	}

	public Decimal(int[] bits)
		: this((ReadOnlySpan<int>)(bits ?? throw new ArgumentNullException("bits")))
	{
	}

	public Decimal(ReadOnlySpan<int> bits)
	{
		if (bits.Length == 4)
		{
			int flags = bits[3];
			if (IsValid(flags))
			{
				_lo64 = (uint)bits[0] + ((ulong)(uint)bits[1] << 32);
				_hi32 = (uint)bits[2];
				_flags = flags;
				return;
			}
		}
		throw new ArgumentException(SR.Arg_DecBitCtor);
	}

	public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(scale, 28, "scale");
		_lo64 = (uint)lo + ((ulong)(uint)mid << 32);
		_hi32 = (uint)hi;
		_flags = scale << 16;
		if (isNegative)
		{
			_flags |= int.MinValue;
		}
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
		if (!IsValid(_flags))
		{
			throw new SerializationException(SR.Overflow_Decimal);
		}
	}

	private Decimal(int lo, int mid, int hi, int flags)
	{
		if (IsValid(flags))
		{
			_lo64 = (uint)lo + ((ulong)(uint)mid << 32);
			_hi32 = (uint)hi;
			_flags = flags;
			return;
		}
		throw new ArgumentException(SR.Arg_DecBitCtor);
	}

	private Decimal(in decimal d, int flags)
	{
		this = d;
		_flags = flags;
	}

	public static decimal Add(decimal d1, decimal d2)
	{
		DecCalc.DecAddSub(ref AsMutable(ref d1), ref AsMutable(ref d2), sign: false);
		return d1;
	}

	public static decimal Ceiling(decimal d)
	{
		int flags = d._flags;
		if ((flags & 0xFF0000) != 0)
		{
			DecCalc.InternalRound(ref AsMutable(ref d), (byte)(flags >> 16), MidpointRounding.ToPositiveInfinity);
		}
		return d;
	}

	public static int Compare(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2);
	}

	public int CompareTo(object? value)
	{
		if (value == null)
		{
			return 1;
		}
		if (!(value is decimal d))
		{
			throw new ArgumentException(SR.Arg_MustBeDecimal);
		}
		return DecCalc.VarDecCmp(this, in d);
	}

	public int CompareTo(decimal value)
	{
		return DecCalc.VarDecCmp(this, in value);
	}

	public static decimal Divide(decimal d1, decimal d2)
	{
		DecCalc.VarDecDiv(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public override bool Equals([NotNullWhen(true)] object? value)
	{
		if (value is decimal d)
		{
			return DecCalc.VarDecCmp(this, in d) == 0;
		}
		return false;
	}

	public bool Equals(decimal value)
	{
		return DecCalc.VarDecCmp(this, in value) == 0;
	}

	public override int GetHashCode()
	{
		return DecCalc.GetHashCode(this);
	}

	public static bool Equals(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) == 0;
	}

	public static decimal Floor(decimal d)
	{
		int flags = d._flags;
		if ((flags & 0xFF0000) != 0)
		{
			DecCalc.InternalRound(ref AsMutable(ref d), (byte)(flags >> 16), MidpointRounding.ToNegativeInfinity);
		}
		return d;
	}

	public override string ToString()
	{
		return Number.FormatDecimal(this, null, NumberFormatInfo.CurrentInfo);
	}

	public string ToString([StringSyntax("NumericFormat")] string? format)
	{
		return Number.FormatDecimal(this, format.AsSpan(), NumberFormatInfo.CurrentInfo);
	}

	public string ToString(IFormatProvider? provider)
	{
		return Number.FormatDecimal(this, null, NumberFormatInfo.GetInstance(provider));
	}

	public string ToString([StringSyntax("NumericFormat")] string? format, IFormatProvider? provider)
	{
		return Number.FormatDecimal(this, format.AsSpan(), NumberFormatInfo.GetInstance(provider));
	}

	public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? provider = null)
	{
		return Number.TryFormatDecimal(this, format, NumberFormatInfo.GetInstance(provider), destination, out charsWritten);
	}

	public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, [StringSyntax("NumericFormat")] ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider? provider = null)
	{
		return Number.TryFormatDecimal(this, format, NumberFormatInfo.GetInstance(provider), utf8Destination, out bytesWritten);
	}

	public static decimal Parse(string s)
	{
		return Parse(s, NumberStyles.Number, null);
	}

	public static decimal Parse(string s, NumberStyles style)
	{
		return Parse(s, style, null);
	}

	public static decimal Parse(string s, IFormatProvider? provider)
	{
		return Parse(s, NumberStyles.Number, provider);
	}

	public static decimal Parse(string s, NumberStyles style, IFormatProvider? provider)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return Parse(s.AsSpan(), style, provider);
	}

	public static decimal Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
	{
		NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
		return Number.ParseDecimal(s, style, NumberFormatInfo.GetInstance(provider));
	}

	public static bool TryParse([NotNullWhen(true)] string? s, out decimal result)
	{
		return TryParse(s, NumberStyles.Number, null, out result);
	}

	public static bool TryParse(ReadOnlySpan<char> s, out decimal result)
	{
		return TryParse(s, NumberStyles.Number, null, out result);
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, out decimal result)
	{
		return TryParse(utf8Text, NumberStyles.Number, null, out result);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out decimal result)
	{
		NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
		if (s == null)
		{
			result = 0m;
			return false;
		}
		return Number.TryParseDecimal(s.AsSpan(), style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
	}

	public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out decimal result)
	{
		NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
		return Number.TryParseDecimal(s, style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
	}

	public static int[] GetBits(decimal d)
	{
		return new int[4]
		{
			(int)d.Low,
			(int)d.Mid,
			(int)d.High,
			d._flags
		};
	}

	public static int GetBits(decimal d, Span<int> destination)
	{
		if (destination.Length <= 3)
		{
			ThrowHelper.ThrowArgumentException_DestinationTooShort();
		}
		destination[0] = (int)d.Low;
		destination[1] = (int)d.Mid;
		destination[2] = (int)d.High;
		destination[3] = d._flags;
		return 4;
	}

	public static bool TryGetBits(decimal d, Span<int> destination, out int valuesWritten)
	{
		if (destination.Length <= 3)
		{
			valuesWritten = 0;
			return false;
		}
		destination[0] = (int)d.Low;
		destination[1] = (int)d.Mid;
		destination[2] = (int)d.High;
		destination[3] = d._flags;
		valuesWritten = 4;
		return true;
	}

	internal static void GetBytes(in decimal d, Span<byte> buffer)
	{
		BinaryPrimitives.WriteInt32LittleEndian(buffer, (int)d.Low);
		BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(4), (int)d.Mid);
		BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(8), (int)d.High);
		BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(12), d._flags);
	}

	internal static decimal ToDecimal(ReadOnlySpan<byte> span)
	{
		int lo = BinaryPrimitives.ReadInt32LittleEndian(span);
		int mid = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(4));
		int hi = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8));
		int flags = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(12));
		return new decimal(lo, mid, hi, flags);
	}

	public static decimal Remainder(decimal d1, decimal d2)
	{
		DecCalc.VarDecMod(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public static decimal Multiply(decimal d1, decimal d2)
	{
		DecCalc.VarDecMul(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public static decimal Negate(decimal d)
	{
		return new decimal(in d, d._flags ^ int.MinValue);
	}

	public static decimal Round(decimal d)
	{
		return Round(ref d, 0, MidpointRounding.ToEven);
	}

	public static decimal Round(decimal d, int decimals)
	{
		return Round(ref d, decimals, MidpointRounding.ToEven);
	}

	public static decimal Round(decimal d, MidpointRounding mode)
	{
		return Round(ref d, 0, mode);
	}

	public static decimal Round(decimal d, int decimals, MidpointRounding mode)
	{
		return Round(ref d, decimals, mode);
	}

	private static decimal Round(ref decimal d, int decimals, MidpointRounding mode)
	{
		if ((uint)decimals > 28u)
		{
			throw new ArgumentOutOfRangeException("decimals", SR.ArgumentOutOfRange_DecimalRound);
		}
		if ((uint)mode > 4u)
		{
			throw new ArgumentException(SR.Format(SR.Argument_InvalidEnumValue, mode, "MidpointRounding"), "mode");
		}
		int num = d.Scale - decimals;
		if (num > 0)
		{
			DecCalc.InternalRound(ref AsMutable(ref d), (uint)num, mode);
		}
		return d;
	}

	public static decimal Subtract(decimal d1, decimal d2)
	{
		DecCalc.DecAddSub(ref AsMutable(ref d1), ref AsMutable(ref d2), sign: true);
		return d1;
	}

	public static byte ToByte(decimal value)
	{
		uint num;
		try
		{
			num = ToUInt32(value);
		}
		catch (OverflowException)
		{
			Number.ThrowOverflowException<byte>();
			throw;
		}
		if (num != (byte)num)
		{
			Number.ThrowOverflowException<byte>();
		}
		return (byte)num;
	}

	[CLSCompliant(false)]
	public static sbyte ToSByte(decimal value)
	{
		int num;
		try
		{
			num = ToInt32(value);
		}
		catch (OverflowException)
		{
			Number.ThrowOverflowException<sbyte>();
			throw;
		}
		if (num != (sbyte)num)
		{
			Number.ThrowOverflowException<sbyte>();
		}
		return (sbyte)num;
	}

	public static short ToInt16(decimal value)
	{
		int num;
		try
		{
			num = ToInt32(value);
		}
		catch (OverflowException)
		{
			Number.ThrowOverflowException<short>();
			throw;
		}
		if (num != (short)num)
		{
			Number.ThrowOverflowException<short>();
		}
		return (short)num;
	}

	public static double ToDouble(decimal d)
	{
		return DecCalc.VarR8FromDec(in d);
	}

	public static int ToInt32(decimal d)
	{
		Truncate(ref d);
		if ((d.High | d.Mid) == 0)
		{
			int low = (int)d.Low;
			if (!IsNegative(d))
			{
				if (low >= 0)
				{
					return low;
				}
			}
			else
			{
				low = -low;
				if (low <= 0)
				{
					return low;
				}
			}
		}
		throw new OverflowException(SR.Overflow_Int32);
	}

	public static long ToInt64(decimal d)
	{
		Truncate(ref d);
		if (d.High == 0)
		{
			long low = (long)d.Low64;
			if (!IsNegative(d))
			{
				if (low >= 0)
				{
					return low;
				}
			}
			else
			{
				low = -low;
				if (low <= 0)
				{
					return low;
				}
			}
		}
		throw new OverflowException(SR.Overflow_Int64);
	}

	[CLSCompliant(false)]
	public static ushort ToUInt16(decimal value)
	{
		uint num;
		try
		{
			num = ToUInt32(value);
		}
		catch (OverflowException)
		{
			Number.ThrowOverflowException<ushort>();
			throw;
		}
		if (num != (ushort)num)
		{
			Number.ThrowOverflowException<ushort>();
		}
		return (ushort)num;
	}

	[CLSCompliant(false)]
	public static uint ToUInt32(decimal d)
	{
		Truncate(ref d);
		if ((d.High | d.Mid) == 0)
		{
			uint low = d.Low;
			if (!IsNegative(d) || low == 0)
			{
				return low;
			}
		}
		throw new OverflowException(SR.Overflow_UInt32);
	}

	[CLSCompliant(false)]
	public static ulong ToUInt64(decimal d)
	{
		Truncate(ref d);
		if (d.High == 0)
		{
			ulong low = d.Low64;
			if (!IsNegative(d) || low == 0L)
			{
				return low;
			}
		}
		throw new OverflowException(SR.Overflow_UInt64);
	}

	public static float ToSingle(decimal d)
	{
		return DecCalc.VarR4FromDec(in d);
	}

	public static decimal Truncate(decimal d)
	{
		Truncate(ref d);
		return d;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Truncate(ref decimal d)
	{
		int flags = d._flags;
		if ((flags & 0xFF0000) != 0)
		{
			DecCalc.InternalRound(ref AsMutable(ref d), (byte)(flags >> 16), MidpointRounding.ToZero);
		}
	}

	public static implicit operator decimal(byte value)
	{
		return new decimal((uint)value);
	}

	[CLSCompliant(false)]
	public static implicit operator decimal(sbyte value)
	{
		return new decimal(value);
	}

	public static implicit operator decimal(short value)
	{
		return new decimal(value);
	}

	[CLSCompliant(false)]
	public static implicit operator decimal(ushort value)
	{
		return new decimal((uint)value);
	}

	public static implicit operator decimal(char value)
	{
		return new decimal((uint)value);
	}

	public static implicit operator decimal(int value)
	{
		return new decimal(value);
	}

	[CLSCompliant(false)]
	public static implicit operator decimal(uint value)
	{
		return new decimal(value);
	}

	public static implicit operator decimal(long value)
	{
		return new decimal(value);
	}

	[CLSCompliant(false)]
	public static implicit operator decimal(ulong value)
	{
		return new decimal(value);
	}

	public static explicit operator decimal(float value)
	{
		return new decimal(value);
	}

	public static explicit operator decimal(double value)
	{
		return new decimal(value);
	}

	public static explicit operator byte(decimal value)
	{
		return ToByte(value);
	}

	[CLSCompliant(false)]
	public static explicit operator sbyte(decimal value)
	{
		return ToSByte(value);
	}

	public static explicit operator char(decimal value)
	{
		try
		{
			return (char)ToUInt16(value);
		}
		catch (OverflowException innerException)
		{
			throw new OverflowException(SR.Overflow_Char, innerException);
		}
	}

	public static explicit operator short(decimal value)
	{
		return ToInt16(value);
	}

	[CLSCompliant(false)]
	public static explicit operator ushort(decimal value)
	{
		return ToUInt16(value);
	}

	public static explicit operator int(decimal value)
	{
		return ToInt32(value);
	}

	[CLSCompliant(false)]
	public static explicit operator uint(decimal value)
	{
		return ToUInt32(value);
	}

	public static explicit operator long(decimal value)
	{
		return ToInt64(value);
	}

	[CLSCompliant(false)]
	public static explicit operator ulong(decimal value)
	{
		return ToUInt64(value);
	}

	public static explicit operator float(decimal value)
	{
		return DecCalc.VarR4FromDec(in value);
	}

	public static explicit operator double(decimal value)
	{
		return DecCalc.VarR8FromDec(in value);
	}

	public static decimal operator +(decimal d)
	{
		return d;
	}

	public static decimal operator -(decimal d)
	{
		return new decimal(in d, d._flags ^ int.MinValue);
	}

	public static decimal operator ++(decimal d)
	{
		return Add(d, 1m);
	}

	public static decimal operator --(decimal d)
	{
		return Subtract(d, 1m);
	}

	public static decimal operator +(decimal d1, decimal d2)
	{
		DecCalc.DecAddSub(ref AsMutable(ref d1), ref AsMutable(ref d2), sign: false);
		return d1;
	}

	public static decimal operator -(decimal d1, decimal d2)
	{
		DecCalc.DecAddSub(ref AsMutable(ref d1), ref AsMutable(ref d2), sign: true);
		return d1;
	}

	public static decimal operator *(decimal d1, decimal d2)
	{
		DecCalc.VarDecMul(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public static decimal operator /(decimal d1, decimal d2)
	{
		DecCalc.VarDecDiv(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public static decimal operator %(decimal d1, decimal d2)
	{
		DecCalc.VarDecMod(ref AsMutable(ref d1), ref AsMutable(ref d2));
		return d1;
	}

	public static bool operator ==(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) == 0;
	}

	public static bool operator !=(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) != 0;
	}

	public static bool operator <(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) < 0;
	}

	public static bool operator <=(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) <= 0;
	}

	public static bool operator >(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) > 0;
	}

	public static bool operator >=(decimal d1, decimal d2)
	{
		return DecCalc.VarDecCmp(in d1, in d2) >= 0;
	}

	public TypeCode GetTypeCode()
	{
		return TypeCode.Decimal;
	}

	bool IConvertible.ToBoolean(IFormatProvider provider)
	{
		return Convert.ToBoolean(this);
	}

	char IConvertible.ToChar(IFormatProvider provider)
	{
		throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, "Decimal", "Char"));
	}

	sbyte IConvertible.ToSByte(IFormatProvider provider)
	{
		return Convert.ToSByte(this);
	}

	byte IConvertible.ToByte(IFormatProvider provider)
	{
		return Convert.ToByte(this);
	}

	short IConvertible.ToInt16(IFormatProvider provider)
	{
		return Convert.ToInt16(this);
	}

	ushort IConvertible.ToUInt16(IFormatProvider provider)
	{
		return Convert.ToUInt16(this);
	}

	int IConvertible.ToInt32(IFormatProvider provider)
	{
		return Convert.ToInt32(this);
	}

	uint IConvertible.ToUInt32(IFormatProvider provider)
	{
		return Convert.ToUInt32(this);
	}

	long IConvertible.ToInt64(IFormatProvider provider)
	{
		return Convert.ToInt64(this);
	}

	ulong IConvertible.ToUInt64(IFormatProvider provider)
	{
		return Convert.ToUInt64(this);
	}

	float IConvertible.ToSingle(IFormatProvider provider)
	{
		return DecCalc.VarR4FromDec(this);
	}

	double IConvertible.ToDouble(IFormatProvider provider)
	{
		return DecCalc.VarR8FromDec(this);
	}

	decimal IConvertible.ToDecimal(IFormatProvider provider)
	{
		return this;
	}

	DateTime IConvertible.ToDateTime(IFormatProvider provider)
	{
		throw new InvalidCastException(SR.Format(SR.InvalidCast_FromTo, "Decimal", "DateTime"));
	}

	object IConvertible.ToType(Type type, IFormatProvider provider)
	{
		return Convert.DefaultToType(this, type, provider);
	}

	public static TInteger ConvertToInteger<TInteger>(decimal value) where TInteger : IBinaryInteger<TInteger>
	{
		return TInteger.CreateSaturating(value);
	}

	public static TInteger ConvertToIntegerNative<TInteger>(decimal value) where TInteger : IBinaryInteger<TInteger>
	{
		return TInteger.CreateSaturating(value);
	}

	int IFloatingPoint<decimal>.GetExponentByteCount()
	{
		return 1;
	}

	int IFloatingPoint<decimal>.GetExponentShortestBitLength()
	{
		sbyte exponent = Exponent;
		return 8 - sbyte.LeadingZeroCount(exponent);
	}

	int IFloatingPoint<decimal>.GetSignificandByteCount()
	{
		return 12;
	}

	int IFloatingPoint<decimal>.GetSignificandBitLength()
	{
		return 96;
	}

	bool IFloatingPoint<decimal>.TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= 1)
		{
			destination[0] = (byte)Exponent;
			bytesWritten = 1;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	bool IFloatingPoint<decimal>.TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= 1)
		{
			destination[0] = (byte)Exponent;
			bytesWritten = 1;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	bool IFloatingPoint<decimal>.TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= 12)
		{
			uint hi = _hi32;
			ulong lo = _lo64;
			_ = BitConverter.IsLittleEndian;
			hi = BinaryPrimitives.ReverseEndianness(hi);
			lo = BinaryPrimitives.ReverseEndianness(lo);
			ref byte reference = ref MemoryMarshal.GetReference(destination);
			Unsafe.WriteUnaligned(ref reference, hi);
			Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)4), lo);
			bytesWritten = 12;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	bool IFloatingPoint<decimal>.TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten)
	{
		if (destination.Length >= 12)
		{
			ulong lo = _lo64;
			uint hi = _hi32;
			if (!BitConverter.IsLittleEndian)
			{
			}
			ref byte reference = ref MemoryMarshal.GetReference(destination);
			Unsafe.WriteUnaligned(ref reference, lo);
			Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref reference, (nint)8), hi);
			bytesWritten = 12;
			return true;
		}
		bytesWritten = 0;
		return false;
	}

	public static decimal Clamp(decimal value, decimal min, decimal max)
	{
		return Math.Clamp(value, min, max);
	}

	public static decimal CopySign(decimal value, decimal sign)
	{
		return new decimal(in value, (value._flags & 0x7FFFFFFF) | (sign._flags & int.MinValue));
	}

	public static decimal Max(decimal x, decimal y)
	{
		if (DecCalc.VarDecCmp(in x, in y) < 0)
		{
			return y;
		}
		return x;
	}

	static decimal INumber<decimal>.MaxNumber(decimal x, decimal y)
	{
		return Max(x, y);
	}

	public static decimal Min(decimal x, decimal y)
	{
		if (DecCalc.VarDecCmp(in x, in y) >= 0)
		{
			return y;
		}
		return x;
	}

	static decimal INumber<decimal>.MinNumber(decimal x, decimal y)
	{
		return Min(x, y);
	}

	public static int Sign(decimal d)
	{
		if ((d.Low64 | d.High) != 0L)
		{
			return (d._flags >> 31) | 1;
		}
		return 0;
	}

	public static decimal Abs(decimal value)
	{
		return new decimal(in value, value._flags & 0x7FFFFFFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(decimal))
		{
			return (decimal)(object)value;
		}
		if (!TryConvertFromChecked(value, out var result) && !TOther.TryConvertToChecked<decimal>(value, out result))
		{
			ThrowHelper.ThrowNotSupportedException();
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(decimal))
		{
			return (decimal)(object)value;
		}
		if (!TryConvertFrom(value, out var result) && !TOther.TryConvertToSaturating<decimal>(value, out result))
		{
			ThrowHelper.ThrowNotSupportedException();
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static decimal CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(decimal))
		{
			return (decimal)(object)value;
		}
		if (!TryConvertFrom(value, out var result) && !TOther.TryConvertToTruncating<decimal>(value, out result))
		{
			ThrowHelper.ThrowNotSupportedException();
		}
		return result;
	}

	public static bool IsCanonical(decimal value)
	{
		if ((byte)(value._flags >> 16) == 0)
		{
			return true;
		}
		if (value._hi32 == 0)
		{
			return value._lo64 % 10 != 0;
		}
		return new UInt128(value._hi32, value._lo64) % 10u != 0u;
	}

	static bool INumberBase<decimal>.IsComplexNumber(decimal value)
	{
		return false;
	}

	public static bool IsEvenInteger(decimal value)
	{
		decimal num = Truncate(value);
		if (value == num)
		{
			return (num._lo64 & 1) == 0;
		}
		return false;
	}

	static bool INumberBase<decimal>.IsFinite(decimal value)
	{
		return true;
	}

	static bool INumberBase<decimal>.IsImaginaryNumber(decimal value)
	{
		return false;
	}

	static bool INumberBase<decimal>.IsInfinity(decimal value)
	{
		return false;
	}

	public static bool IsInteger(decimal value)
	{
		return value == Truncate(value);
	}

	static bool INumberBase<decimal>.IsNaN(decimal value)
	{
		return false;
	}

	public static bool IsNegative(decimal value)
	{
		return value._flags < 0;
	}

	static bool INumberBase<decimal>.IsNegativeInfinity(decimal value)
	{
		return false;
	}

	static bool INumberBase<decimal>.IsNormal(decimal value)
	{
		return value != 0m;
	}

	public static bool IsOddInteger(decimal value)
	{
		decimal num = Truncate(value);
		if (value == num)
		{
			return (num._lo64 & 1) != 0;
		}
		return false;
	}

	public static bool IsPositive(decimal value)
	{
		return value._flags >= 0;
	}

	static bool INumberBase<decimal>.IsPositiveInfinity(decimal value)
	{
		return false;
	}

	static bool INumberBase<decimal>.IsRealNumber(decimal value)
	{
		return true;
	}

	static bool INumberBase<decimal>.IsSubnormal(decimal value)
	{
		return false;
	}

	static bool INumberBase<decimal>.IsZero(decimal value)
	{
		return value == 0m;
	}

	public static decimal MaxMagnitude(decimal x, decimal y)
	{
		decimal num = Abs(x);
		decimal num2 = Abs(y);
		if (num > num2)
		{
			return x;
		}
		if (num == num2)
		{
			if (!IsNegative(x))
			{
				return x;
			}
			return y;
		}
		return y;
	}

	static decimal INumberBase<decimal>.MaxMagnitudeNumber(decimal x, decimal y)
	{
		return MaxMagnitude(x, y);
	}

	public static decimal MinMagnitude(decimal x, decimal y)
	{
		decimal num = Abs(x);
		decimal num2 = Abs(y);
		if (num < num2)
		{
			return x;
		}
		if (num == num2)
		{
			if (!IsNegative(x))
			{
				return y;
			}
			return x;
		}
		return y;
	}

	static decimal INumberBase<decimal>.MinMagnitudeNumber(decimal x, decimal y)
	{
		return MinMagnitude(x, y);
	}

	static decimal INumberBase<decimal>.MultiplyAddEstimate(decimal left, decimal right, decimal addend)
	{
		return left * right + addend;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertFromChecked<TOther>(TOther value, out decimal result)
	{
		return TryConvertFromChecked(value, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryConvertFromChecked<TOther>(TOther value, out decimal result) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(byte))
		{
			byte b = (byte)(object)value;
			result = b;
			return true;
		}
		if (typeof(TOther) == typeof(char))
		{
			char c = (char)(object)value;
			result = c;
			return true;
		}
		if (typeof(TOther) == typeof(ushort))
		{
			ushort num = (ushort)(object)value;
			result = num;
			return true;
		}
		if (typeof(TOther) == typeof(uint))
		{
			uint num2 = (uint)(object)value;
			result = num2;
			return true;
		}
		if (typeof(TOther) == typeof(ulong))
		{
			ulong num3 = (ulong)(object)value;
			result = num3;
			return true;
		}
		if (typeof(TOther) == typeof(UInt128))
		{
			UInt128 uInt = (UInt128)(object)value;
			result = (decimal)uInt;
			return true;
		}
		if (typeof(TOther) == typeof(nuint))
		{
			nuint num4 = (nuint)(object)value;
			result = (ulong)num4;
			return true;
		}
		result = 0m;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertFromSaturating<TOther>(TOther value, out decimal result)
	{
		return TryConvertFrom(value, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertFromTruncating<TOther>(TOther value, out decimal result)
	{
		return TryConvertFrom(value, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryConvertFrom<TOther>(TOther value, out decimal result) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(byte))
		{
			byte b = (byte)(object)value;
			result = b;
			return true;
		}
		if (typeof(TOther) == typeof(char))
		{
			char c = (char)(object)value;
			result = c;
			return true;
		}
		if (typeof(TOther) == typeof(ushort))
		{
			ushort num = (ushort)(object)value;
			result = num;
			return true;
		}
		if (typeof(TOther) == typeof(uint))
		{
			uint num2 = (uint)(object)value;
			result = num2;
			return true;
		}
		if (typeof(TOther) == typeof(ulong))
		{
			ulong num3 = (ulong)(object)value;
			result = num3;
			return true;
		}
		if (typeof(TOther) == typeof(UInt128))
		{
			UInt128 uInt = (UInt128)(object)value;
			result = ((uInt >= new UInt128(4294967295uL, ulong.MaxValue)) ? decimal.MaxValue : ((decimal)uInt));
			return true;
		}
		if (typeof(TOther) == typeof(nuint))
		{
			nuint num4 = (nuint)(object)value;
			result = (ulong)num4;
			return true;
		}
		result = 0m;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertToChecked<TOther>(decimal value, [MaybeNullWhen(false)] out TOther result)
	{
		if (typeof(TOther) == typeof(double))
		{
			double num = (double)value;
			result = (TOther)(object)num;
			return true;
		}
		if (typeof(TOther) == typeof(Half))
		{
			Half half = (Half)value;
			result = (TOther)(object)half;
			return true;
		}
		if (typeof(TOther) == typeof(short))
		{
			short num2 = (short)value;
			result = (TOther)(object)num2;
			return true;
		}
		if (typeof(TOther) == typeof(int))
		{
			int num3 = (int)value;
			result = (TOther)(object)num3;
			return true;
		}
		if (typeof(TOther) == typeof(long))
		{
			long num4 = (long)value;
			result = (TOther)(object)num4;
			return true;
		}
		if (typeof(TOther) == typeof(Int128))
		{
			Int128 @int = (Int128)value;
			result = (TOther)(object)@int;
			return true;
		}
		if (typeof(TOther) == typeof(nint))
		{
			nint num5 = checked((nint)(long)value);
			result = (TOther)(object)num5;
			return true;
		}
		if (typeof(TOther) == typeof(sbyte))
		{
			sbyte b = (sbyte)value;
			result = (TOther)(object)b;
			return true;
		}
		if (typeof(TOther) == typeof(float))
		{
			float num6 = (float)value;
			result = (TOther)(object)num6;
			return true;
		}
		result = default(TOther);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertToSaturating<TOther>(decimal value, [MaybeNullWhen(false)] out TOther result)
	{
		return TryConvertTo<TOther>(value, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool INumberBase<decimal>.TryConvertToTruncating<TOther>(decimal value, [MaybeNullWhen(false)] out TOther result)
	{
		return TryConvertTo<TOther>(value, out result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryConvertTo<TOther>(decimal value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
	{
		if (typeof(TOther) == typeof(double))
		{
			double num = (double)value;
			result = (TOther)(object)num;
			return true;
		}
		if (typeof(TOther) == typeof(Half))
		{
			Half half = (Half)value;
			result = (TOther)(object)half;
			return true;
		}
		if (typeof(TOther) == typeof(short))
		{
			short num2 = ((value >= 32767m) ? short.MaxValue : ((value <= -32768m) ? short.MinValue : ((short)value)));
			result = (TOther)(object)num2;
			return true;
		}
		if (typeof(TOther) == typeof(int))
		{
			int num3 = ((value >= 2147483647m) ? int.MaxValue : ((value <= -2147483648m) ? int.MinValue : ((int)value)));
			result = (TOther)(object)num3;
			return true;
		}
		if (typeof(TOther) == typeof(long))
		{
			long num4 = ((value >= 9223372036854775807m) ? long.MaxValue : ((value <= -9223372036854775808m) ? long.MinValue : ((long)value)));
			result = (TOther)(object)num4;
			return true;
		}
		if (typeof(TOther) == typeof(Int128))
		{
			Int128 @int = (Int128)value;
			result = (TOther)(object)@int;
			return true;
		}
		if (typeof(TOther) == typeof(nint))
		{
			nint num5 = (nint)((value >= (decimal)(long)IntPtr.MaxValue) ? IntPtr.MaxValue : ((value <= (decimal)(long)IntPtr.MinValue) ? IntPtr.MinValue : ((long)value)));
			result = (TOther)(object)num5;
			return true;
		}
		if (typeof(TOther) == typeof(sbyte))
		{
			sbyte b = ((value >= 127m) ? sbyte.MaxValue : ((value <= -128m) ? sbyte.MinValue : ((sbyte)value)));
			result = (TOther)(object)b;
			return true;
		}
		if (typeof(TOther) == typeof(float))
		{
			float num6 = (float)value;
			result = (TOther)(object)num6;
			return true;
		}
		result = default(TOther);
		return false;
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out decimal result)
	{
		return TryParse(s, NumberStyles.Number, provider, out result);
	}

	public static decimal Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		return Parse(s, NumberStyles.Number, provider);
	}

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out decimal result)
	{
		return TryParse(s, NumberStyles.Number, provider, out result);
	}

	public static decimal Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		return Number.ParseDecimal(utf8Text, style, NumberFormatInfo.GetInstance(provider));
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out decimal result)
	{
		NumberFormatInfo.ValidateParseStyleInteger(style);
		return Number.TryParseDecimal(utf8Text, style, NumberFormatInfo.GetInstance(provider), out result) == Number.ParsingStatus.OK;
	}

	public static decimal Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
	{
		return Parse(utf8Text, NumberStyles.Number, provider);
	}

	public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out decimal result)
	{
		return TryParse(utf8Text, NumberStyles.Number, provider, out result);
	}

	private static ref DecCalc AsMutable(ref decimal d)
	{
		return ref Unsafe.As<decimal, DecCalc>(ref d);
	}

	internal static uint DecDivMod1E9(ref decimal value)
	{
		return DecCalc.DecDivMod1E9(ref AsMutable(ref value));
	}
}
