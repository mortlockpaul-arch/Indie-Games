using System;
using v;

namespace _7;

internal class h
{
	public enum _00065h
	{
		Negative = -1,
		Zero,
		Positive
	}

	internal sealed class _00065b
	{
		private h a5h;

		private h a5b;

		public _00065b(h modulus)
		{
			a5h = modulus;
			uint num = a5h.a56 << 1;
			a5b = new h(_00065h.Positive, num + 1);
			a5b.a5a[num] = 1u;
			a5b /= a5h;
		}

		public void BarrettReduction(h x)
		{
			h h2 = a5h;
			uint a2 = h2.a56;
			uint num = a2 + 1;
			uint num2 = a2 - 1;
			if (x.a56 >= a2)
			{
				if (x.a5a.Length < x.a56)
				{
					throw new IndexOutOfRangeException("x out of range");
				}
				h h3 = new h(_00065h.Positive, x.a56 - num2 + a5b.a56);
				_00065a.Multiply(x.a5a, num2, x.a56 - num2, a5b.a5a, 0u, a5b.a56, h3.a5a, 0u);
				uint a3 = ((x.a56 > num) ? num : x.a56);
				x.a56 = a3;
				x._0017();
				h h4 = new h(_00065h.Positive, num);
				_00065a.MultiplyMod2p32pmod(h3.a5a, num, h3.a56 - num, h2.a5a, 0u, h2.a56, h4.a5a, 0u, num);
				h4._0017();
				if (h4 <= x)
				{
					_00065a.MinusEq(x, h4);
				}
				else
				{
					h h5 = new h(_00065h.Positive, num + 1);
					h5.a5a[num] = 1u;
					_00065a.MinusEq(h5, h4);
					_00065a.PlusEq(x, h5);
				}
				while (x >= h2)
				{
					_00065a.MinusEq(x, h2);
				}
			}
		}

		public h Multiply(h a, h b)
		{
			if (a == 0u || b == 0u)
			{
				return 0;
			}
			if (a > a5h)
			{
				a %= a5h;
			}
			if (b > a5h)
			{
				b %= a5h;
			}
			h h2 = new h(a * b);
			BarrettReduction(h2);
			return h2;
		}

		public h Difference(h a, h b)
		{
			_00065h obj = _00065a.Compare(a, b);
			h h2;
			switch (obj)
			{
			case _00065h.Zero:
				return 0;
			case _00065h.Positive:
				h2 = a - b;
				break;
			case _00065h.Negative:
				h2 = b - a;
				break;
			default:
				throw new Exception();
			}
			if (h2 >= a5h)
			{
				if (h2.a56 >= a5h.a56 << 1)
				{
					h2 %= a5h;
				}
				else
				{
					BarrettReduction(h2);
				}
			}
			if (obj == _00065h.Negative)
			{
				h2 = a5h - h2;
			}
			return h2;
		}

		public h Pow(h a, h k)
		{
			h result = new h(1u);
			if (k == 0u)
			{
				return result;
			}
			h h2 = a;
			if (k.TestBit(0))
			{
				result = a;
			}
			for (int i = 1; i < k.BitCount(); i++)
			{
				h2 = Multiply(h2, h2);
				if (k.TestBit(i))
				{
					result = Multiply(h2, result);
				}
			}
			return result;
		}

		public h Pow(uint b, h exp)
		{
			return Pow(new h(b), exp);
		}
	}

	internal sealed class _000656
	{
		private _000656()
		{
		}

		public static uint Inverse(uint n)
		{
			uint num = n;
			uint num2;
			while ((num2 = n * num) != 1)
			{
				num *= 2 - num2;
			}
			return (uint)(0uL - (ulong)num);
		}

		public static h ToMont(h n, h m)
		{
			n._0017();
			m._0017();
			n <<= (int)(m.a56 * 32);
			n %= m;
			return n;
		}
	}

	private sealed class _00065a
	{
		public static h AddSameSign(h bi1, h bi2)
		{
			uint num = 0u;
			uint[] a5a;
			uint a2;
			uint[] a5a2;
			uint a3;
			if (bi1.a56 < bi2.a56)
			{
				a5a = bi2.a5a;
				a2 = bi2.a56;
				a5a2 = bi1.a5a;
				a3 = bi1.a56;
			}
			else
			{
				a5a = bi1.a5a;
				a2 = bi1.a56;
				a5a2 = bi2.a5a;
				a3 = bi2.a56;
			}
			h h2 = new h(_00065h.Positive, a2 + 1);
			uint[] a5a3 = h2.a5a;
			ulong num2 = 0uL;
			do
			{
				num2 = (ulong)((long)a5a[num] + (long)a5a2[num]) + num2;
				a5a3[num] = (uint)num2;
				num2 >>= 32;
			}
			while (++num < a3);
			bool flag = num2 != 0;
			if (flag)
			{
				if (num < a2)
				{
					do
					{
						flag = (a5a3[num] = a5a[num] + 1) == 0;
					}
					while (++num < a2 && flag);
				}
				if (flag)
				{
					a5a3[num] = 1u;
					num = (h2.a56 = num + 1);
					return h2;
				}
			}
			if (num < a2)
			{
				do
				{
					a5a3[num] = a5a[num];
				}
				while (++num < a2);
			}
			h2._0017();
			return h2;
		}

		public static h Subtract(h big, h small)
		{
			h h2 = new h(_00065h.Positive, big.a56);
			uint[] a5a = h2.a5a;
			uint[] a5a2 = big.a5a;
			uint[] a5a3 = small.a5a;
			uint num = 0u;
			uint num2 = 0u;
			do
			{
				uint num3 = a5a3[num];
				num2 = ((((num3 += num2) < num2) | ((a5a[num] = a5a2[num] - num3) > ~num3)) ? 1u : 0u);
			}
			while (++num < small.a56);
			if (num != big.a56)
			{
				if (num2 == 1)
				{
					do
					{
						a5a[num] = a5a2[num] - 1;
					}
					while (a5a2[num++] == 0 && num < big.a56);
					if (num == big.a56)
					{
						goto IL_00c0;
					}
				}
				do
				{
					a5a[num] = a5a2[num];
				}
				while (++num < big.a56);
			}
			goto IL_00c0;
			IL_00c0:
			h2._0017();
			return h2;
		}

		public static void MinusEq(h big, h small)
		{
			uint[] a5a = big.a5a;
			uint[] a5a2 = small.a5a;
			uint num = 0u;
			uint num2 = 0u;
			do
			{
				uint num3 = a5a2[num];
				num2 = ((((num3 += num2) < num2) | ((a5a[num] -= num3) > ~num3)) ? 1u : 0u);
			}
			while (++num < small.a56);
			if (num != big.a56 && num2 == 1)
			{
				do
				{
					a5a[num]--;
				}
				while (a5a[num++] == 0 && num < big.a56);
			}
			while (big.a56 != 0 && big.a5a[big.a56 - 1] == 0)
			{
				big.a56--;
			}
			if (big.a56 == 0)
			{
				big.a56++;
			}
		}

		public static void PlusEq(h bi1, h bi2)
		{
			uint num = 0u;
			bool flag = false;
			uint[] a5a;
			uint a2;
			uint[] a5a2;
			uint a3;
			if (bi1.a56 < bi2.a56)
			{
				flag = true;
				a5a = bi2.a5a;
				a2 = bi2.a56;
				a5a2 = bi1.a5a;
				a3 = bi1.a56;
			}
			else
			{
				a5a = bi1.a5a;
				a2 = bi1.a56;
				a5a2 = bi2.a5a;
				a3 = bi2.a56;
			}
			uint[] a5a3 = bi1.a5a;
			ulong num2 = 0uL;
			do
			{
				num2 += (ulong)((long)a5a[num] + (long)a5a2[num]);
				a5a3[num] = (uint)num2;
				num2 >>= 32;
			}
			while (++num < a3);
			bool flag2 = num2 != 0;
			if (flag2)
			{
				if (num < a2)
				{
					do
					{
						flag2 = (a5a3[num] = a5a[num] + 1) == 0;
					}
					while (++num < a2 && flag2);
				}
				if (flag2)
				{
					a5a3[num] = 1u;
					num = (bi1.a56 = num + 1);
					return;
				}
			}
			if (flag && num < a2 - 1)
			{
				do
				{
					a5a3[num] = a5a[num];
				}
				while (++num < a2);
			}
			bi1.a56 = a2 + 1;
			bi1._0017();
		}

		public static _00065h Compare(h bi1, h bi2)
		{
			uint num = bi1.a56;
			uint num2 = bi2.a56;
			while (num != 0 && bi1.a5a[num - 1] == 0)
			{
				num--;
			}
			while (num2 != 0 && bi2.a5a[num2 - 1] == 0)
			{
				num2--;
			}
			if (num == 0 && num2 == 0)
			{
				return _00065h.Zero;
			}
			if (num < num2)
			{
				return _00065h.Negative;
			}
			if (num > num2)
			{
				return _00065h.Positive;
			}
			uint num3 = num - 1;
			while (num3 != 0 && bi1.a5a[num3] == bi2.a5a[num3])
			{
				num3--;
			}
			if (bi1.a5a[num3] < bi2.a5a[num3])
			{
				return _00065h.Negative;
			}
			if (bi1.a5a[num3] > bi2.a5a[num3])
			{
				return _00065h.Positive;
			}
			return _00065h.Zero;
		}

		public static uint SingleByteDivideInPlace(h n, uint d)
		{
			ulong num = 0uL;
			uint a2 = n.a56;
			while (a2-- != 0)
			{
				num <<= 32;
				num |= n.a5a[a2];
				n.a5a[a2] = (uint)(num / d);
				num %= d;
			}
			n._0017();
			return (uint)num;
		}

		public static uint DwordMod(h n, uint d)
		{
			ulong num = 0uL;
			uint a2 = n.a56;
			while (a2-- != 0)
			{
				num <<= 32;
				num |= n.a5a[a2];
				num %= d;
			}
			return (uint)num;
		}

		public static h DwordDiv(h n, uint d)
		{
			h h2 = new h(_00065h.Positive, n.a56);
			ulong num = 0uL;
			uint a2 = n.a56;
			while (a2-- != 0)
			{
				num <<= 32;
				num |= n.a5a[a2];
				h2.a5a[a2] = (uint)(num / d);
				num %= d;
			}
			h2._0017();
			return h2;
		}

		public static h[] DwordDivMod(h n, uint d)
		{
			h h2 = new h(_00065h.Positive, n.a56);
			ulong num = 0uL;
			uint a2 = n.a56;
			while (a2-- != 0)
			{
				num <<= 32;
				num |= n.a5a[a2];
				h2.a5a[a2] = (uint)(num / d);
				num %= d;
			}
			h2._0017();
			h h3 = (uint)num;
			return new h[2] { h2, h3 };
		}

		public static h[] multiByteDivide(h bi1, h bi2)
		{
			if (Compare(bi1, bi2) == _00065h.Negative)
			{
				return new h[2]
				{
					0,
					new h(bi1)
				};
			}
			bi1._0017();
			bi2._0017();
			if (bi2.a56 == 1)
			{
				return DwordDivMod(bi1, bi2.a5a[0]);
			}
			uint num = bi1.a56 + 1;
			int num2 = (int)(bi2.a56 + 1);
			uint num3 = 2147483648u;
			uint num4 = bi2.a5a[bi2.a56 - 1];
			int num5 = 0;
			int num6 = (int)(bi1.a56 - bi2.a56);
			while (num3 != 0 && (num4 & num3) == 0)
			{
				num5++;
				num3 >>= 1;
			}
			h h2 = new h(_00065h.Positive, bi1.a56 - bi2.a56 + 1);
			h h3 = bi1 << num5;
			uint[] a5a = h3.a5a;
			bi2 <<= num5;
			int num7 = (int)(num - bi2.a56);
			int num8 = (int)(num - 1);
			uint num9 = bi2.a5a[bi2.a56 - 1];
			ulong num10 = bi2.a5a[bi2.a56 - 2];
			while (num7 > 0)
			{
				ulong num11 = ((ulong)a5a[num8] << 32) + a5a[num8 - 1];
				ulong num12 = num11 / num9;
				ulong num13 = num11 % num9;
				while (num12 == 4294967296L || num12 * num10 > (num13 << 32) + a5a[num8 - 2])
				{
					num12--;
					num13 += num9;
					if (num13 >= 4294967296L)
					{
						break;
					}
				}
				uint num14 = 0u;
				int num15 = num8 - num2 + 1;
				ulong num16 = 0uL;
				uint num17 = (uint)num12;
				do
				{
					num16 += (ulong)((long)bi2.a5a[num14] * (long)num17);
					uint num18 = a5a[num15];
					a5a[num15] -= (uint)(int)num16;
					num16 >>= 32;
					if (a5a[num15] > num18)
					{
						num16++;
					}
					num14++;
					num15++;
				}
				while (num14 < num2);
				num15 = num8 - num2 + 1;
				num14 = 0u;
				if (num16 != 0)
				{
					num17--;
					ulong num19 = 0uL;
					do
					{
						num19 = (ulong)((long)a5a[num15] + (long)bi2.a5a[num14]) + num19;
						a5a[num15] = (uint)num19;
						num19 >>= 32;
						num14++;
						num15++;
					}
					while (num14 < num2);
				}
				h2.a5a[num6--] = num17;
				num8--;
				num7--;
			}
			h2._0017();
			h3._0017();
			h[] array = new h[2] { h2, h3 };
			if (num5 != 0)
			{
				h[] array2;
				(array2 = array)[1] = array2[1] >> num5;
			}
			return array;
		}

		public static h LeftShift(h bi, int n)
		{
			if (n == 0)
			{
				return new h(bi, bi.a56 + 1);
			}
			int num = n >> 5;
			n &= 0x1F;
			h h2 = new h(_00065h.Positive, bi.a56 + 1 + (uint)num);
			uint num2 = 0u;
			uint a2 = bi.a56;
			if (n != 0)
			{
				uint num3 = 0u;
				for (; num2 < a2; num2++)
				{
					uint num4 = bi.a5a[num2];
					h2.a5a[num2 + num] = (num4 << n) | num3;
					num3 = num4 >> 32 - n;
				}
				h2.a5a[num2 + num] = num3;
			}
			else
			{
				for (; num2 < a2; num2++)
				{
					h2.a5a[num2 + num] = bi.a5a[num2];
				}
			}
			h2._0017();
			return h2;
		}

		public static h RightShift(h bi, int n)
		{
			if (n == 0)
			{
				return new h(bi);
			}
			int num = n >> 5;
			int num2 = n & 0x1F;
			h h2 = new h(_00065h.Positive, (uint)((int)bi.a56 - num + 1));
			uint num3 = (uint)(h2.a5a.Length - 1);
			if (num2 != 0)
			{
				uint num4 = 0u;
				while (num3-- != 0)
				{
					uint num5 = bi.a5a[num3 + num];
					h2.a5a[num3] = (num5 >> n) | num4;
					num4 = num5 << 32 - n;
				}
			}
			else
			{
				while (num3-- != 0)
				{
					h2.a5a[num3] = bi.a5a[num3 + num];
				}
			}
			h2._0017();
			return h2;
		}

		public static h MultiplyByDword(h n, uint f)
		{
			h h2 = new h(_00065h.Positive, n.a56 + 1);
			uint num = 0u;
			ulong num2 = 0uL;
			do
			{
				num2 += (ulong)((long)n.a5a[num] * (long)f);
				h2.a5a[num] = (uint)num2;
				num2 >>= 32;
			}
			while (++num < n.a56);
			h2.a5a[num] = (uint)num2;
			h2._0017();
			return h2;
		}

		public static void Multiply(uint[] x, uint xOffset, uint xLen, uint[] y, uint yOffset, uint yLen, uint[] d, uint dOffset)
		{
			uint num = xOffset + xLen;
			uint num2 = yOffset + yLen;
			uint num3 = dOffset;
			uint num4 = xOffset;
			while (num4 < num)
			{
				ulong num5 = x[num4];
				if (num5 != 0)
				{
					ulong num6 = 0uL;
					uint num7 = num3;
					uint num8 = yOffset;
					while (num8 < num2)
					{
						num6 += num5 * y[num8] + d[num7];
						d[num7] = (uint)num6;
						num6 >>= 32;
						num8++;
						num7++;
					}
					if (num6 != 0)
					{
						d[num7] = (uint)num6;
					}
				}
				num4++;
				num3++;
			}
		}

		public static void MultiplyMod2p32pmod(uint[] x, uint xOffset, uint xLen, uint[] y, uint yOffset, uint yLen, uint[] d, uint dOffset, uint mod)
		{
			uint num = xOffset + xLen;
			uint num2 = yOffset + yLen;
			uint num3 = dOffset;
			uint num4 = num3 + mod;
			uint num5 = xOffset;
			while (num5 < num)
			{
				ulong num6 = x[num5];
				if (num6 != 0)
				{
					ulong num7 = 0uL;
					uint num8 = num3;
					uint num9 = yOffset;
					while (num9 < num2 && num8 < num4)
					{
						num7 += num6 * y[num9] + d[num8];
						d[num8] = (uint)num7;
						num7 >>= 32;
						num9++;
						num8++;
					}
					if (num7 != 0 && num8 < num4)
					{
						d[num8] = (uint)num7;
					}
				}
				num5++;
				num3++;
			}
		}

		public static h gcd(h a, h b)
		{
			h h2 = a;
			h h3 = b;
			h h4 = h3;
			while (h2.a56 > 1)
			{
				h4 = h2;
				h2 = h3 % h2;
				h3 = h4;
			}
			if (h2 == 0u)
			{
				return h4;
			}
			uint num = h2.a5a[0];
			uint num2 = h3 % num;
			int num3 = 0;
			while (((num2 | num) & 1) == 0)
			{
				num2 >>= 1;
				num >>= 1;
				num3++;
			}
			while (num2 != 0)
			{
				while ((num2 & 1) == 0)
				{
					num2 >>= 1;
				}
				while ((num & 1) == 0)
				{
					num >>= 1;
				}
				if (num2 >= num)
				{
					num2 = num2 - num >> 1;
				}
				else
				{
					num = num - num2 >> 1;
				}
			}
			return num << num3;
		}

		public static uint modInverse(h bi, uint modulus)
		{
			uint num = modulus;
			uint num2 = bi % modulus;
			uint num3 = 0u;
			uint num4 = 1u;
			while (true)
			{
				switch (num2)
				{
				case 1u:
					return num4;
				default:
					num3 += num / num2 * num4;
					num %= num2;
					switch (num)
					{
					case 1u:
						return modulus - num3;
					default:
						goto IL_002d;
					case 0u:
						break;
					}
					break;
				case 0u:
					break;
				}
				break;
				IL_002d:
				num4 += num2 / num * num3;
				num2 %= num;
			}
			return 0u;
		}

		public static h modInverse(h bi, h modulus)
		{
			if (modulus.a56 == 1)
			{
				return modInverse(bi, modulus.a5a[0]);
			}
			h[] array = new h[2] { 0, 1 };
			h[] array2 = new h[2];
			h[] array3 = new h[2] { 0, 0 };
			int num = 0;
			h bi2 = modulus;
			h h2 = bi;
			_00065b obj = new _00065b(modulus);
			while (h2 != 0u)
			{
				if (num > 1)
				{
					h h3 = obj.Difference(array[0], array[1] * array2[0]);
					array[0] = array[1];
					array[1] = h3;
				}
				h[] array4 = multiByteDivide(bi2, h2);
				array2[0] = array2[1];
				array2[1] = array4[0];
				array3[0] = array3[1];
				array3[1] = array4[1];
				bi2 = h2;
				h2 = array4[1];
				num++;
			}
			if (array3[0] != 1u)
			{
				throw new ArithmeticException("No inverse!");
			}
			return obj.Difference(array[0], array[1] * array2[0]);
		}
	}

	private const uint a5h = 20u;

	private const string a5b = "Operation would return a negative value";

	private uint a56 = 1u;

	private uint[] a5a;

	internal static readonly uint[] a57 = new uint[783]
	{
		2u, 3u, 5u, 7u, 11u, 13u, 17u, 19u, 23u, 29u,
		31u, 37u, 41u, 43u, 47u, 53u, 59u, 61u, 67u, 71u,
		73u, 79u, 83u, 89u, 97u, 101u, 103u, 107u, 109u, 113u,
		127u, 131u, 137u, 139u, 149u, 151u, 157u, 163u, 167u, 173u,
		179u, 181u, 191u, 193u, 197u, 199u, 211u, 223u, 227u, 229u,
		233u, 239u, 241u, 251u, 257u, 263u, 269u, 271u, 277u, 281u,
		283u, 293u, 307u, 311u, 313u, 317u, 331u, 337u, 347u, 349u,
		353u, 359u, 367u, 373u, 379u, 383u, 389u, 397u, 401u, 409u,
		419u, 421u, 431u, 433u, 439u, 443u, 449u, 457u, 461u, 463u,
		467u, 479u, 487u, 491u, 499u, 503u, 509u, 521u, 523u, 541u,
		547u, 557u, 563u, 569u, 571u, 577u, 587u, 593u, 599u, 601u,
		607u, 613u, 617u, 619u, 631u, 641u, 643u, 647u, 653u, 659u,
		661u, 673u, 677u, 683u, 691u, 701u, 709u, 719u, 727u, 733u,
		739u, 743u, 751u, 757u, 761u, 769u, 773u, 787u, 797u, 809u,
		811u, 821u, 823u, 827u, 829u, 839u, 853u, 857u, 859u, 863u,
		877u, 881u, 883u, 887u, 907u, 911u, 919u, 929u, 937u, 941u,
		947u, 953u, 967u, 971u, 977u, 983u, 991u, 997u, 1009u, 1013u,
		1019u, 1021u, 1031u, 1033u, 1039u, 1049u, 1051u, 1061u, 1063u, 1069u,
		1087u, 1091u, 1093u, 1097u, 1103u, 1109u, 1117u, 1123u, 1129u, 1151u,
		1153u, 1163u, 1171u, 1181u, 1187u, 1193u, 1201u, 1213u, 1217u, 1223u,
		1229u, 1231u, 1237u, 1249u, 1259u, 1277u, 1279u, 1283u, 1289u, 1291u,
		1297u, 1301u, 1303u, 1307u, 1319u, 1321u, 1327u, 1361u, 1367u, 1373u,
		1381u, 1399u, 1409u, 1423u, 1427u, 1429u, 1433u, 1439u, 1447u, 1451u,
		1453u, 1459u, 1471u, 1481u, 1483u, 1487u, 1489u, 1493u, 1499u, 1511u,
		1523u, 1531u, 1543u, 1549u, 1553u, 1559u, 1567u, 1571u, 1579u, 1583u,
		1597u, 1601u, 1607u, 1609u, 1613u, 1619u, 1621u, 1627u, 1637u, 1657u,
		1663u, 1667u, 1669u, 1693u, 1697u, 1699u, 1709u, 1721u, 1723u, 1733u,
		1741u, 1747u, 1753u, 1759u, 1777u, 1783u, 1787u, 1789u, 1801u, 1811u,
		1823u, 1831u, 1847u, 1861u, 1867u, 1871u, 1873u, 1877u, 1879u, 1889u,
		1901u, 1907u, 1913u, 1931u, 1933u, 1949u, 1951u, 1973u, 1979u, 1987u,
		1993u, 1997u, 1999u, 2003u, 2011u, 2017u, 2027u, 2029u, 2039u, 2053u,
		2063u, 2069u, 2081u, 2083u, 2087u, 2089u, 2099u, 2111u, 2113u, 2129u,
		2131u, 2137u, 2141u, 2143u, 2153u, 2161u, 2179u, 2203u, 2207u, 2213u,
		2221u, 2237u, 2239u, 2243u, 2251u, 2267u, 2269u, 2273u, 2281u, 2287u,
		2293u, 2297u, 2309u, 2311u, 2333u, 2339u, 2341u, 2347u, 2351u, 2357u,
		2371u, 2377u, 2381u, 2383u, 2389u, 2393u, 2399u, 2411u, 2417u, 2423u,
		2437u, 2441u, 2447u, 2459u, 2467u, 2473u, 2477u, 2503u, 2521u, 2531u,
		2539u, 2543u, 2549u, 2551u, 2557u, 2579u, 2591u, 2593u, 2609u, 2617u,
		2621u, 2633u, 2647u, 2657u, 2659u, 2663u, 2671u, 2677u, 2683u, 2687u,
		2689u, 2693u, 2699u, 2707u, 2711u, 2713u, 2719u, 2729u, 2731u, 2741u,
		2749u, 2753u, 2767u, 2777u, 2789u, 2791u, 2797u, 2801u, 2803u, 2819u,
		2833u, 2837u, 2843u, 2851u, 2857u, 2861u, 2879u, 2887u, 2897u, 2903u,
		2909u, 2917u, 2927u, 2939u, 2953u, 2957u, 2963u, 2969u, 2971u, 2999u,
		3001u, 3011u, 3019u, 3023u, 3037u, 3041u, 3049u, 3061u, 3067u, 3079u,
		3083u, 3089u, 3109u, 3119u, 3121u, 3137u, 3163u, 3167u, 3169u, 3181u,
		3187u, 3191u, 3203u, 3209u, 3217u, 3221u, 3229u, 3251u, 3253u, 3257u,
		3259u, 3271u, 3299u, 3301u, 3307u, 3313u, 3319u, 3323u, 3329u, 3331u,
		3343u, 3347u, 3359u, 3361u, 3371u, 3373u, 3389u, 3391u, 3407u, 3413u,
		3433u, 3449u, 3457u, 3461u, 3463u, 3467u, 3469u, 3491u, 3499u, 3511u,
		3517u, 3527u, 3529u, 3533u, 3539u, 3541u, 3547u, 3557u, 3559u, 3571u,
		3581u, 3583u, 3593u, 3607u, 3613u, 3617u, 3623u, 3631u, 3637u, 3643u,
		3659u, 3671u, 3673u, 3677u, 3691u, 3697u, 3701u, 3709u, 3719u, 3727u,
		3733u, 3739u, 3761u, 3767u, 3769u, 3779u, 3793u, 3797u, 3803u, 3821u,
		3823u, 3833u, 3847u, 3851u, 3853u, 3863u, 3877u, 3881u, 3889u, 3907u,
		3911u, 3917u, 3919u, 3923u, 3929u, 3931u, 3943u, 3947u, 3967u, 3989u,
		4001u, 4003u, 4007u, 4013u, 4019u, 4021u, 4027u, 4049u, 4051u, 4057u,
		4073u, 4079u, 4091u, 4093u, 4099u, 4111u, 4127u, 4129u, 4133u, 4139u,
		4153u, 4157u, 4159u, 4177u, 4201u, 4211u, 4217u, 4219u, 4229u, 4231u,
		4241u, 4243u, 4253u, 4259u, 4261u, 4271u, 4273u, 4283u, 4289u, 4297u,
		4327u, 4337u, 4339u, 4349u, 4357u, 4363u, 4373u, 4391u, 4397u, 4409u,
		4421u, 4423u, 4441u, 4447u, 4451u, 4457u, 4463u, 4481u, 4483u, 4493u,
		4507u, 4513u, 4517u, 4519u, 4523u, 4547u, 4549u, 4561u, 4567u, 4583u,
		4591u, 4597u, 4603u, 4621u, 4637u, 4639u, 4643u, 4649u, 4651u, 4657u,
		4663u, 4673u, 4679u, 4691u, 4703u, 4721u, 4723u, 4729u, 4733u, 4751u,
		4759u, 4783u, 4787u, 4789u, 4793u, 4799u, 4801u, 4813u, 4817u, 4831u,
		4861u, 4871u, 4877u, 4889u, 4903u, 4909u, 4919u, 4931u, 4933u, 4937u,
		4943u, 4951u, 4957u, 4967u, 4969u, 4973u, 4987u, 4993u, 4999u, 5003u,
		5009u, 5011u, 5021u, 5023u, 5039u, 5051u, 5059u, 5077u, 5081u, 5087u,
		5099u, 5101u, 5107u, 5113u, 5119u, 5147u, 5153u, 5167u, 5171u, 5179u,
		5189u, 5197u, 5209u, 5227u, 5231u, 5233u, 5237u, 5261u, 5273u, 5279u,
		5281u, 5297u, 5303u, 5309u, 5323u, 5333u, 5347u, 5351u, 5381u, 5387u,
		5393u, 5399u, 5407u, 5413u, 5417u, 5419u, 5431u, 5437u, 5441u, 5443u,
		5449u, 5471u, 5477u, 5479u, 5483u, 5501u, 5503u, 5507u, 5519u, 5521u,
		5527u, 5531u, 5557u, 5563u, 5569u, 5573u, 5581u, 5591u, 5623u, 5639u,
		5641u, 5647u, 5651u, 5653u, 5657u, 5659u, 5669u, 5683u, 5689u, 5693u,
		5701u, 5711u, 5717u, 5737u, 5741u, 5743u, 5749u, 5779u, 5783u, 5791u,
		5801u, 5807u, 5813u, 5821u, 5827u, 5839u, 5843u, 5849u, 5851u, 5857u,
		5861u, 5867u, 5869u, 5879u, 5881u, 5897u, 5903u, 5923u, 5927u, 5939u,
		5953u, 5981u, 5987u
	};

	private static v.W a5_0006;

	private static v.W Rng
	{
		get
		{
			if (a5_0006 == null)
			{
				a5_0006 = v.W.Create();
			}
			return a5_0006;
		}
	}

	public h()
	{
		a5a = new uint[20];
		a56 = 20u;
	}

	public h(_00065h sign, uint len)
	{
		a5a = new uint[len];
		a56 = len;
	}

	public h(h bi)
	{
		a5a = (uint[])bi.a5a.Clone();
		a56 = bi.a56;
	}

	public h(h bi, uint len)
	{
		a5a = new uint[len];
		for (uint num = 0u; num < bi.a56; num++)
		{
			a5a[num] = bi.a5a[num];
		}
		a56 = bi.a56;
	}

	public h(byte[] inData)
	{
		a56 = (uint)inData.Length >> 2;
		int num = inData.Length & 3;
		if (num != 0)
		{
			a56++;
		}
		a5a = new uint[a56];
		int num2 = inData.Length - 1;
		int num3 = 0;
		while (num2 >= 3)
		{
			a5a[num3] = (uint)((inData[num2 - 3] << 24) | (inData[num2 - 2] << 16) | (inData[num2 - 1] << 8) | inData[num2]);
			num2 -= 4;
			num3++;
		}
		switch (num)
		{
		case 1:
			a5a[a56 - 1] = inData[0];
			break;
		case 2:
			a5a[a56 - 1] = (uint)((inData[0] << 8) | inData[1]);
			break;
		case 3:
			a5a[a56 - 1] = (uint)((inData[0] << 16) | (inData[1] << 8) | inData[2]);
			break;
		}
		_0017();
	}

	public h(uint[] inData)
	{
		a56 = (uint)inData.Length;
		a5a = new uint[a56];
		int num = (int)(a56 - 1);
		int num2 = 0;
		while (num >= 0)
		{
			a5a[num2] = inData[num];
			num--;
			num2++;
		}
		_0017();
	}

	public h(uint ui)
	{
		a5a = new uint[1] { ui };
	}

	public h(ulong ul)
	{
		a5a = new uint[2]
		{
			(uint)ul,
			(uint)(ul >> 32)
		};
		a56 = 2u;
		_0017();
	}

	public static implicit operator h(uint value)
	{
		return new h(value);
	}

	public static implicit operator h(int value)
	{
		if (value < 0)
		{
			throw new ArgumentOutOfRangeException("value");
		}
		return new h((uint)value);
	}

	public static implicit operator h(ulong value)
	{
		return new h(value);
	}

	public static h Parse(string number)
	{
		if (number == null)
		{
			throw new ArgumentNullException("number");
		}
		int i = 0;
		int length = number.Length;
		bool flag = false;
		h h2 = new h(0u);
		if (number[i] == '+')
		{
			i++;
		}
		else if (number[i] == '-')
		{
			throw new FormatException("Operation would return a negative value");
		}
		for (; i < length; i++)
		{
			char c = number[i];
			switch (c)
			{
			case '\0':
				i = length;
				continue;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				h2 = h2 * 10 + (c - 48);
				flag = true;
				continue;
			}
			if (char.IsWhiteSpace(c))
			{
				for (i++; i < length; i++)
				{
					if (!char.IsWhiteSpace(number[i]))
					{
						throw new FormatException();
					}
				}
				break;
			}
			throw new FormatException();
		}
		if (!flag)
		{
			throw new FormatException();
		}
		return h2;
	}

	public static h operator +(h bi1, h bi2)
	{
		if (bi1 == 0u)
		{
			return new h(bi2);
		}
		if (bi2 == 0u)
		{
			return new h(bi1);
		}
		return _00065a.AddSameSign(bi1, bi2);
	}

	public static h operator -(h bi1, h bi2)
	{
		if (bi2 == 0u)
		{
			return new h(bi1);
		}
		if (bi1 == 0u)
		{
			throw new ArithmeticException("Operation would return a negative value");
		}
		return _00065a.Compare(bi1, bi2) switch
		{
			_00065h.Zero => 0, 
			_00065h.Positive => _00065a.Subtract(bi1, bi2), 
			_00065h.Negative => throw new ArithmeticException("Operation would return a negative value"), 
			_ => throw new Exception(), 
		};
	}

	public static int operator %(h bi, int i)
	{
		if (i > 0)
		{
			return (int)_00065a.DwordMod(bi, (uint)i);
		}
		return (int)(0 - _00065a.DwordMod(bi, (uint)(-i)));
	}

	public static uint operator %(h bi, uint ui)
	{
		return _00065a.DwordMod(bi, ui);
	}

	public static h operator %(h bi1, h bi2)
	{
		return _00065a.multiByteDivide(bi1, bi2)[1];
	}

	public static h operator /(h bi, int i)
	{
		if (i > 0)
		{
			return _00065a.DwordDiv(bi, (uint)i);
		}
		throw new ArithmeticException("Operation would return a negative value");
	}

	public static h operator /(h bi1, h bi2)
	{
		return _00065a.multiByteDivide(bi1, bi2)[0];
	}

	public static h operator *(h bi1, h bi2)
	{
		if (bi1 == 0u || bi2 == 0u)
		{
			return 0;
		}
		if (bi1.a5a.Length < bi1.a56)
		{
			throw new IndexOutOfRangeException("bi1 out of range");
		}
		if (bi2.a5a.Length < bi2.a56)
		{
			throw new IndexOutOfRangeException("bi2 out of range");
		}
		h h2 = new h(_00065h.Positive, bi1.a56 + bi2.a56);
		_00065a.Multiply(bi1.a5a, 0u, bi1.a56, bi2.a5a, 0u, bi2.a56, h2.a5a, 0u);
		h2._0017();
		return h2;
	}

	public static h operator *(h bi, int i)
	{
		if (i < 0)
		{
			throw new ArithmeticException("Operation would return a negative value");
		}
		return i switch
		{
			0 => 0, 
			1 => new h(bi), 
			_ => _00065a.MultiplyByDword(bi, (uint)i), 
		};
	}

	public static h operator <<(h bi1, int shiftVal)
	{
		return _00065a.LeftShift(bi1, shiftVal);
	}

	public static h operator >>(h bi1, int shiftVal)
	{
		return _00065a.RightShift(bi1, shiftVal);
	}

	public static h Add(h bi1, h bi2)
	{
		return bi1 + bi2;
	}

	public static h Subtract(h bi1, h bi2)
	{
		return bi1 - bi2;
	}

	public static int Modulus(h bi, int i)
	{
		return bi % i;
	}

	public static uint Modulus(h bi, uint ui)
	{
		return bi % ui;
	}

	public static h Modulus(h bi1, h bi2)
	{
		return bi1 % bi2;
	}

	public static h Divid(h bi, int i)
	{
		return bi / i;
	}

	public static h Divid(h bi1, h bi2)
	{
		return bi1 / bi2;
	}

	public static h Multiply(h bi1, h bi2)
	{
		return bi1 * bi2;
	}

	public static h Multiply(h bi, int i)
	{
		return bi * i;
	}

	public static h GenerateRandom(int bits, v.W rng)
	{
		int num = bits >> 5;
		int num2 = bits & 0x1F;
		if (num2 != 0)
		{
			num++;
		}
		h h2 = new h(_00065h.Positive, (uint)(num + 1));
		byte[] array = new byte[num << 2];
		rng.GetBytes(array);
		Buffer.BlockCopy(array, 0, h2.a5a, 0, num << 2);
		if (num2 != 0)
		{
			uint num3 = (uint)(1 << num2 - 1);
			h2.a5a[num - 1] |= num3;
			num3 = uint.MaxValue >> 32 - num2;
			h2.a5a[num - 1] &= num3;
		}
		else
		{
			h2.a5a[num - 1] |= 2147483648u;
		}
		h2._0017();
		return h2;
	}

	public static h GenerateRandom(int bits)
	{
		return GenerateRandom(bits, Rng);
	}

	public int BitCount()
	{
		_0017();
		uint num = a5a[a56 - 1];
		uint num2 = 2147483648u;
		uint num3 = 32u;
		while (num3 != 0 && (num & num2) == 0)
		{
			num3--;
			num2 >>= 1;
		}
		return (int)(num3 + (a56 - 1 << 5));
	}

	public bool TestBit(uint bitNum)
	{
		uint num = bitNum >> 5;
		byte b2 = (byte)(bitNum & 0x1F);
		uint num2 = (uint)(1 << (int)b2);
		return (a5a[num] & num2) != 0;
	}

	public bool TestBit(int bitNum)
	{
		if (bitNum < 0)
		{
			throw new IndexOutOfRangeException("bitNum out of range");
		}
		uint num = (uint)bitNum >> 5;
		byte b2 = (byte)(bitNum & 0x1F);
		uint num2 = (uint)(1 << (int)b2);
		return (a5a[num] | num2) == a5a[num];
	}

	public void SetBit(uint bitNum)
	{
		SetBit(bitNum, value: true);
	}

	public void ClearBit(uint bitNum)
	{
		SetBit(bitNum, value: false);
	}

	public void SetBit(uint bitNum, bool value)
	{
		uint num = bitNum >> 5;
		if (num < a56)
		{
			uint num2 = (uint)(1 << (int)(bitNum & 0x1F));
			if (value)
			{
				a5a[num] |= num2;
			}
			else
			{
				a5a[num] &= ~num2;
			}
		}
	}

	public int LowestSetBit()
	{
		if (this == 0u)
		{
			return -1;
		}
		int i;
		for (i = 0; !TestBit(i); i++)
		{
		}
		return i;
	}

	public byte[] GetBytes()
	{
		if (this == 0u)
		{
			return new byte[1];
		}
		int num = BitCount();
		int num2 = num >> 3;
		if ((num & 7) != 0)
		{
			num2++;
		}
		byte[] array = new byte[num2];
		int num3 = num2 & 3;
		if (num3 == 0)
		{
			num3 = 4;
		}
		int num4 = 0;
		for (int num5 = (int)(a56 - 1); num5 >= 0; num5--)
		{
			uint num6 = a5a[num5];
			for (int num7 = num3 - 1; num7 >= 0; num7--)
			{
				array[num4 + num7] = (byte)(num6 & 0xFF);
				num6 >>= 8;
			}
			num4 += num3;
			num3 = 4;
		}
		return array;
	}

	public static bool operator ==(h bi1, uint ui)
	{
		if (bi1.a56 != 1)
		{
			bi1._0017();
		}
		if (bi1.a56 == 1)
		{
			return bi1.a5a[0] == ui;
		}
		return false;
	}

	public static bool operator !=(h bi1, uint ui)
	{
		if (bi1.a56 != 1)
		{
			bi1._0017();
		}
		if (bi1.a56 == 1)
		{
			return bi1.a5a[0] != ui;
		}
		return true;
	}

	public static bool operator ==(h bi1, h bi2)
	{
		if ((object)bi1 == bi2)
		{
			return true;
		}
		if (null == bi1 || null == bi2)
		{
			return false;
		}
		return _00065a.Compare(bi1, bi2) == _00065h.Zero;
	}

	public static bool operator !=(h bi1, h bi2)
	{
		if ((object)bi1 == bi2)
		{
			return false;
		}
		if (null == bi1 || null == bi2)
		{
			return true;
		}
		return _00065a.Compare(bi1, bi2) != _00065h.Zero;
	}

	public static bool operator >(h bi1, h bi2)
	{
		return _00065a.Compare(bi1, bi2) > _00065h.Zero;
	}

	public static bool operator <(h bi1, h bi2)
	{
		return _00065a.Compare(bi1, bi2) < _00065h.Zero;
	}

	public static bool operator >=(h bi1, h bi2)
	{
		return _00065a.Compare(bi1, bi2) >= _00065h.Zero;
	}

	public static bool operator <=(h bi1, h bi2)
	{
		return _00065a.Compare(bi1, bi2) <= _00065h.Zero;
	}

	public _00065h Compare(h bi)
	{
		return _00065a.Compare(this, bi);
	}

	public string ToString(uint radix)
	{
		return ToString(radix, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ");
	}

	public string ToString(uint radix, string characterSet)
	{
		if (characterSet.Length < radix)
		{
			throw new ArgumentException("charSet length less than radix", "characterSet");
		}
		if (radix == 1)
		{
			throw new ArgumentException("There is no such thing as radix one notation", "radix");
		}
		if (this == 0u)
		{
			return "0";
		}
		if (this == 1u)
		{
			return "1";
		}
		string text = "";
		h h2 = new h(this);
		while (h2 != 0u)
		{
			uint index = _00065a.SingleByteDivideInPlace(h2, radix);
			text = characterSet[(int)index] + text;
		}
		return text;
	}

	private void _0017()
	{
		while (a56 != 0 && a5a[a56 - 1] == 0)
		{
			a56--;
		}
		if (a56 == 0)
		{
			a56++;
		}
	}

	public void Clear()
	{
		for (int i = 0; i < a56; i++)
		{
			a5a[i] = 0u;
		}
	}

	public override int GetHashCode()
	{
		uint num = 0u;
		for (uint num2 = 0u; num2 < a56; num2++)
		{
			num ^= a5a[num2];
		}
		return (int)num;
	}

	public override string ToString()
	{
		return ToString(10u);
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (o is int)
		{
			if ((int)o >= 0)
			{
				return this == (uint)o;
			}
			return false;
		}
		h h2 = o as h;
		if (h2 == null)
		{
			return false;
		}
		return _00065a.Compare(this, h2) == _00065h.Zero;
	}

	public h GCD(h bi)
	{
		return _00065a.gcd(this, bi);
	}

	public h ModInverse(h modulus)
	{
		return _00065a.modInverse(this, modulus);
	}

	public h ModPow(h exp, h n)
	{
		_00065b obj = new _00065b(n);
		return obj.Pow(this, exp);
	}
}
