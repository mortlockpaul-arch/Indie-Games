using System;
using System.Collections.Generic;
using System.Xml.Linq;
using _0002;
using z;

namespace _0002
{
	internal class B
	{
		private z.D HCB = new z.D();

		private z.D HC_0002 = new z.D();

		private z.X HC_0012 = new z.X();

		private List<byte[]> HCH = new List<byte[]>();

		internal B(byte[] P_0, byte[] P_1)
		{
			HC_0002.PersistKeyInCsp = false;
			HC_0002.ImportCspBlob(P_0);
			HCB.PersistKeyInCsp = false;
			HCB.ImportCspBlob(P_1);
		}

		internal byte[] _0013(byte[] P_0)
		{
			int num = _0002.z(P_0, 0);
			byte[] array = new byte[num];
			byte[] array2 = new byte[P_0.Length - (num + 4)];
			Array.Copy(P_0, 4, array, 0, array.Length);
			Array.Copy(P_0, num + 4, array2, 0, array2.Length);
			byte[] rgbHash = HC_0012.ComputeHash(array2);
			if (!HCB.VerifyHash(rgbHash, z.H.MapNameToOID("SHA1"), array))
			{
				return null;
			}
			return X(array2);
		}

		private byte[] X(byte[] P_0)
		{
			int num = HC_0002.KeySize / 8;
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 0;
			HCH.Clear();
			while (num2 < P_0.Length)
			{
				int num4 = Math.Min(num, P_0.Length - num2);
				byte[] array2 = array;
				if (num4 < array2.Length)
				{
					array2 = new byte[num4];
				}
				Array.Copy(P_0, num2, array2, 0, num4);
				byte[] array3 = HC_0002.Decrypt(array2, fOAEP: true);
				num2 += num4;
				num3 += array3.Length;
				HCH.Add(array3);
			}
			byte[] array4 = new byte[num3];
			num2 = 0;
			foreach (byte[] item in HCH)
			{
				item.CopyTo(array4, num2);
				num2 += item.Length;
			}
			return array4;
		}
	}
}
namespace _0012
{
	internal class B
	{
		private _0002.B HCB;

		internal B(_0002.B P_0)
		{
			HCB = P_0;
		}

		internal void Y(byte[] P_0, out string P_1, out uint P_2, out uint P_3, out DateTime P_4)
		{
			byte[] array = HCB._0013(P_0);
			if (array.Length <= 8)
			{
				P_1 = "";
				P_2 = 0u;
				P_3 = 0u;
				P_4 = DateTime.Now;
				return;
			}
			P_1 = "";
			byte[] array2 = new byte[2];
			for (int i = 0; i < array.Length - 16; i++)
			{
				array2[0] = array[i];
				array2[1] = 0;
				P_1 += global::_0002._0002.c(array2, 0);
			}
			P_4 = DateTime.Now;
			P_2 = global::_0002._0002.A(array, array.Length - 8);
			P_3 = global::_0002._0002.A(array, array.Length - 4);
		}
	}
}
namespace _0001
{
	internal class B
	{
		public string Name = "";

		public string InnerText = "";

		public H Attributes = new H();

		public _0002 ChildNodes = new _0002();

		private static char[] HCB = new char[1] { '/' };

		public _0012 GetAttributeNode(string name)
		{
			foreach (_0012 attribute in Attributes)
			{
				if (attribute.Name == name)
				{
					return attribute;
				}
			}
			return null;
		}

		public B SelectSingleNode(string limited_xpath)
		{
			_0002 obj = SelectNodes(limited_xpath);
			if (obj.Count > 0)
			{
				return obj[0];
			}
			return null;
		}

		public _0002 SelectNodes(string limited_xpath)
		{
			string[] array = limited_xpath.Split(HCB);
			_0002 obj = new _0002();
			if (array.Length > 0)
			{
				S(ChildNodes, obj, array, 0);
			}
			return obj;
		}

		private static void S(_0002 P_0, _0002 P_1, string[] P_2, int P_3)
		{
			bool flag = P_2.Length - 1 <= P_3;
			string text = P_2[P_3];
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("Wp7 and Xbox minimal Xpath support does not include absolute paths.");
			}
			if (text == ".." || text == ".")
			{
				throw new Exception("Wp7 and Xbox minimal Xpath support does not include '..' and '.' paths.");
			}
			foreach (B item in P_0)
			{
				if (!(item.Name != text))
				{
					if (flag)
					{
						P_1.Add(item);
					}
					else
					{
						S(item.ChildNodes, P_1, P_2, P_3 + 1);
					}
				}
			}
		}

		public virtual void Load(XObject node)
		{
			Attributes.Clear();
			ChildNodes.Clear();
			if (node is XAttribute)
			{
				XAttribute xAttribute = node as XAttribute;
				Name = xAttribute.Name.LocalName;
				InnerText = xAttribute.Value;
			}
			else
			{
				if (!(node is XElement))
				{
					return;
				}
				XElement xElement = node as XElement;
				Name = xElement.Name.LocalName;
				if (xElement.HasAttributes)
				{
					IEnumerable<XAttribute> enumerable = xElement.Attributes();
					foreach (XAttribute item in enumerable)
					{
						_0012 obj = new _0012();
						Attributes.Add(obj);
						obj.Load(item);
					}
				}
				if (xElement.HasElements)
				{
					IEnumerable<XElement> enumerable2 = xElement.Elements();
					{
						foreach (XElement item2 in enumerable2)
						{
							_7 obj2 = new _7();
							ChildNodes.Add(obj2);
							obj2.Load(item2);
						}
						return;
					}
				}
				InnerText = xElement.Value;
			}
		}
	}
}
namespace _000F
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	internal class B : Attribute
	{
		private string HCB;

		private bool HC_0002;

		internal string TexturePathProperty => HCB;

		internal bool TextureRequiresFloatFormat => HC_0002;

		public B(string texturepathproperty, bool texturerequiresfloatformat)
		{
			HCB = texturepathproperty;
			HC_0002 = texturerequiresfloatformat;
		}
	}
}
namespace _0011
{
	internal static class B
	{
		public const string Name = "";

		public const string ModelFile = "";

		public const string ModelMesh = "";

		public const string UpdateType = "";

		public const string Visibility = "";

		public const string Position = "";

		public const string RenderingErrors = "";

		public const string StaticLightingType = "";

		public const string CustomStaticLightingColor = "";

		public const string AffectedByGravity = "";

		public const string CollisionType = "";

		public const string HullType = "";

		public const string Mass = "";

		public const string AudioType = "";

		public const string AudioVolume = "";

		public const string AudioLoop = "";

		public const string AudioPlayWhenLoaded = "";

		public const string AudioRadius = "";

		public const string AudioSoundEffect = "";
	}
}
namespace _0003
{
	internal interface B
	{
		string MaterialFile { get; }
	}
}
namespace _0013
{
	internal class B
	{
		public enum _0001CB
		{
			Negative = -1,
			Zero,
			Positive
		}

		internal sealed class _0001C_0002
		{
			private B HCB;

			private B HC_0002;

			public _0001C_0002(B modulus)
			{
				HCB = modulus;
				uint num = HCB.HC_0012 << 1;
				HC_0002 = new B(_0001CB.Positive, num + 1);
				HC_0002.HCH[num] = 1u;
				HC_0002 /= HCB;
			}

			public void BarrettReduction(B x)
			{
				B hCB = HCB;
				uint num = hCB.HC_0012;
				uint num2 = num + 1;
				uint num3 = num - 1;
				if (x.HC_0012 >= num)
				{
					if (x.HCH.Length < x.HC_0012)
					{
						throw new IndexOutOfRangeException("x out of range");
					}
					B b = new B(_0001CB.Positive, x.HC_0012 - num3 + HC_0002.HC_0012);
					_0001CH.Multiply(x.HCH, num3, x.HC_0012 - num3, HC_0002.HCH, 0u, HC_0002.HC_0012, b.HCH, 0u);
					uint num4 = ((x.HC_0012 > num2) ? num2 : x.HC_0012);
					x.HC_0012 = num4;
					x.Ha();
					B b2 = new B(_0001CB.Positive, num2);
					_0001CH.MultiplyMod2p32pmod(b.HCH, num2, b.HC_0012 - num2, hCB.HCH, 0u, hCB.HC_0012, b2.HCH, 0u, num2);
					b2.Ha();
					if (b2 <= x)
					{
						_0001CH.MinusEq(x, b2);
					}
					else
					{
						B b3 = new B(_0001CB.Positive, num2 + 1);
						b3.HCH[num2] = 1u;
						_0001CH.MinusEq(b3, b2);
						_0001CH.PlusEq(x, b3);
					}
					while (x >= hCB)
					{
						_0001CH.MinusEq(x, hCB);
					}
				}
			}

			public B Multiply(B a, B b)
			{
				if (a == 0u || b == 0u)
				{
					return 0;
				}
				if (a > HCB)
				{
					a %= HCB;
				}
				if (b > HCB)
				{
					b %= HCB;
				}
				B b2 = new B(a * b);
				BarrettReduction(b2);
				return b2;
			}

			public B Difference(B a, B b)
			{
				_0001CB obj = _0001CH.Compare(a, b);
				B b2;
				switch (obj)
				{
				case _0001CB.Zero:
					return 0;
				case _0001CB.Positive:
					b2 = a - b;
					break;
				case _0001CB.Negative:
					b2 = b - a;
					break;
				default:
					throw new Exception();
				}
				if (b2 >= HCB)
				{
					if (b2.HC_0012 >= HCB.HC_0012 << 1)
					{
						b2 %= HCB;
					}
					else
					{
						BarrettReduction(b2);
					}
				}
				if (obj == _0001CB.Negative)
				{
					b2 = HCB - b2;
				}
				return b2;
			}

			public B Pow(B a, B k)
			{
				B b = new B(1u);
				if (k == 0u)
				{
					return b;
				}
				B b2 = a;
				if (k.TestBit(0))
				{
					b = a;
				}
				for (int i = 1; i < k.BitCount(); i++)
				{
					b2 = Multiply(b2, b2);
					if (k.TestBit(i))
					{
						b = Multiply(b2, b);
					}
				}
				return b;
			}

			public B Pow(uint b, B exp)
			{
				return Pow(new B(b), exp);
			}
		}

		internal sealed class _0001C_0012
		{
			private _0001C_0012()
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

			public static B ToMont(B n, B m)
			{
				n.Ha();
				m.Ha();
				n <<= (int)(m.HC_0012 * 32);
				n %= m;
				return n;
			}
		}

		private sealed class _0001CH
		{
			public static B AddSameSign(B bi1, B bi2)
			{
				uint num = 0u;
				uint[] hCH;
				uint num2;
				uint[] hCH2;
				uint num3;
				if (bi1.HC_0012 < bi2.HC_0012)
				{
					hCH = bi2.HCH;
					num2 = bi2.HC_0012;
					hCH2 = bi1.HCH;
					num3 = bi1.HC_0012;
				}
				else
				{
					hCH = bi1.HCH;
					num2 = bi1.HC_0012;
					hCH2 = bi2.HCH;
					num3 = bi2.HC_0012;
				}
				B b = new B(_0001CB.Positive, num2 + 1);
				uint[] hCH3 = b.HCH;
				ulong num4 = 0uL;
				do
				{
					num4 = (ulong)((long)hCH[num] + (long)hCH2[num]) + num4;
					hCH3[num] = (uint)num4;
					num4 >>= 32;
				}
				while (++num < num3);
				bool flag = num4 != 0;
				if (flag)
				{
					if (num < num2)
					{
						do
						{
							flag = (hCH3[num] = hCH[num] + 1) == 0;
						}
						while (++num < num2 && flag);
					}
					if (flag)
					{
						hCH3[num] = 1u;
						num = (b.HC_0012 = num + 1);
						return b;
					}
				}
				if (num < num2)
				{
					do
					{
						hCH3[num] = hCH[num];
					}
					while (++num < num2);
				}
				b.Ha();
				return b;
			}

			public static B Subtract(B big, B small)
			{
				B b = new B(_0001CB.Positive, big.HC_0012);
				uint[] hCH = b.HCH;
				uint[] hCH2 = big.HCH;
				uint[] hCH3 = small.HCH;
				uint num = 0u;
				uint num2 = 0u;
				do
				{
					uint num3 = hCH3[num];
					num2 = ((((num3 += num2) < num2) | ((hCH[num] = hCH2[num] - num3) > ~num3)) ? 1u : 0u);
				}
				while (++num < small.HC_0012);
				if (num != big.HC_0012)
				{
					if (num2 == 1)
					{
						do
						{
							hCH[num] = hCH2[num] - 1;
						}
						while (hCH2[num++] == 0 && num < big.HC_0012);
						if (num == big.HC_0012)
						{
							goto IL_00c0;
						}
					}
					do
					{
						hCH[num] = hCH2[num];
					}
					while (++num < big.HC_0012);
				}
				goto IL_00c0;
				IL_00c0:
				b.Ha();
				return b;
			}

			public static void MinusEq(B big, B small)
			{
				uint[] hCH = big.HCH;
				uint[] hCH2 = small.HCH;
				uint num = 0u;
				uint num2 = 0u;
				do
				{
					uint num3 = hCH2[num];
					num2 = ((((num3 += num2) < num2) | ((hCH[num] -= num3) > ~num3)) ? 1u : 0u);
				}
				while (++num < small.HC_0012);
				if (num != big.HC_0012 && num2 == 1)
				{
					do
					{
						hCH[num]--;
					}
					while (hCH[num++] == 0 && num < big.HC_0012);
				}
				while (big.HC_0012 != 0 && big.HCH[big.HC_0012 - 1] == 0)
				{
					big.HC_0012--;
				}
				if (big.HC_0012 == 0)
				{
					big.HC_0012++;
				}
			}

			public static void PlusEq(B bi1, B bi2)
			{
				uint num = 0u;
				bool flag = false;
				uint[] hCH;
				uint num2;
				uint[] hCH2;
				uint num3;
				if (bi1.HC_0012 < bi2.HC_0012)
				{
					flag = true;
					hCH = bi2.HCH;
					num2 = bi2.HC_0012;
					hCH2 = bi1.HCH;
					num3 = bi1.HC_0012;
				}
				else
				{
					hCH = bi1.HCH;
					num2 = bi1.HC_0012;
					hCH2 = bi2.HCH;
					num3 = bi2.HC_0012;
				}
				uint[] hCH3 = bi1.HCH;
				ulong num4 = 0uL;
				do
				{
					num4 += (ulong)((long)hCH[num] + (long)hCH2[num]);
					hCH3[num] = (uint)num4;
					num4 >>= 32;
				}
				while (++num < num3);
				bool flag2 = num4 != 0;
				if (flag2)
				{
					if (num < num2)
					{
						do
						{
							flag2 = (hCH3[num] = hCH[num] + 1) == 0;
						}
						while (++num < num2 && flag2);
					}
					if (flag2)
					{
						hCH3[num] = 1u;
						num = (bi1.HC_0012 = num + 1);
						return;
					}
				}
				if (flag && num < num2 - 1)
				{
					do
					{
						hCH3[num] = hCH[num];
					}
					while (++num < num2);
				}
				bi1.HC_0012 = num2 + 1;
				bi1.Ha();
			}

			public static _0001CB Compare(B bi1, B bi2)
			{
				uint num = bi1.HC_0012;
				uint num2 = bi2.HC_0012;
				while (num != 0 && bi1.HCH[num - 1] == 0)
				{
					num--;
				}
				while (num2 != 0 && bi2.HCH[num2 - 1] == 0)
				{
					num2--;
				}
				if (num == 0 && num2 == 0)
				{
					return _0001CB.Zero;
				}
				if (num < num2)
				{
					return _0001CB.Negative;
				}
				if (num > num2)
				{
					return _0001CB.Positive;
				}
				uint num3 = num - 1;
				while (num3 != 0 && bi1.HCH[num3] == bi2.HCH[num3])
				{
					num3--;
				}
				if (bi1.HCH[num3] < bi2.HCH[num3])
				{
					return _0001CB.Negative;
				}
				if (bi1.HCH[num3] > bi2.HCH[num3])
				{
					return _0001CB.Positive;
				}
				return _0001CB.Zero;
			}

			public static uint SingleByteDivideInPlace(B n, uint d)
			{
				ulong num = 0uL;
				uint num2 = n.HC_0012;
				while (num2-- != 0)
				{
					num <<= 32;
					num |= n.HCH[num2];
					n.HCH[num2] = (uint)(num / d);
					num %= d;
				}
				n.Ha();
				return (uint)num;
			}

			public static uint DwordMod(B n, uint d)
			{
				ulong num = 0uL;
				uint num2 = n.HC_0012;
				while (num2-- != 0)
				{
					num <<= 32;
					num |= n.HCH[num2];
					num %= d;
				}
				return (uint)num;
			}

			public static B DwordDiv(B n, uint d)
			{
				B b = new B(_0001CB.Positive, n.HC_0012);
				ulong num = 0uL;
				uint num2 = n.HC_0012;
				while (num2-- != 0)
				{
					num <<= 32;
					num |= n.HCH[num2];
					b.HCH[num2] = (uint)(num / d);
					num %= d;
				}
				b.Ha();
				return b;
			}

			public static B[] DwordDivMod(B n, uint d)
			{
				B b = new B(_0001CB.Positive, n.HC_0012);
				ulong num = 0uL;
				uint num2 = n.HC_0012;
				while (num2-- != 0)
				{
					num <<= 32;
					num |= n.HCH[num2];
					b.HCH[num2] = (uint)(num / d);
					num %= d;
				}
				b.Ha();
				B b2 = (uint)num;
				return new B[2] { b, b2 };
			}

			public static B[] multiByteDivide(B bi1, B bi2)
			{
				if (Compare(bi1, bi2) == _0001CB.Negative)
				{
					return new B[2]
					{
						0,
						new B(bi1)
					};
				}
				bi1.Ha();
				bi2.Ha();
				if (bi2.HC_0012 == 1)
				{
					return DwordDivMod(bi1, bi2.HCH[0]);
				}
				uint num = bi1.HC_0012 + 1;
				int num2 = (int)(bi2.HC_0012 + 1);
				uint num3 = 2147483648u;
				uint num4 = bi2.HCH[bi2.HC_0012 - 1];
				int num5 = 0;
				int num6 = (int)(bi1.HC_0012 - bi2.HC_0012);
				while (num3 != 0 && (num4 & num3) == 0)
				{
					num5++;
					num3 >>= 1;
				}
				B b = new B(_0001CB.Positive, bi1.HC_0012 - bi2.HC_0012 + 1);
				B b2 = bi1 << num5;
				uint[] hCH = b2.HCH;
				bi2 <<= num5;
				int num7 = (int)(num - bi2.HC_0012);
				int num8 = (int)(num - 1);
				uint num9 = bi2.HCH[bi2.HC_0012 - 1];
				ulong num10 = bi2.HCH[bi2.HC_0012 - 2];
				while (num7 > 0)
				{
					ulong num11 = ((ulong)hCH[num8] << 32) + hCH[num8 - 1];
					ulong num12 = num11 / num9;
					ulong num13 = num11 % num9;
					while (num12 == 4294967296L || num12 * num10 > (num13 << 32) + hCH[num8 - 2])
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
						num16 += (ulong)((long)bi2.HCH[num14] * (long)num17);
						uint num18 = hCH[num15];
						hCH[num15] -= (uint)(int)num16;
						num16 >>= 32;
						if (hCH[num15] > num18)
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
							num19 = (ulong)((long)hCH[num15] + (long)bi2.HCH[num14]) + num19;
							hCH[num15] = (uint)num19;
							num19 >>= 32;
							num14++;
							num15++;
						}
						while (num14 < num2);
					}
					b.HCH[num6--] = num17;
					num8--;
					num7--;
				}
				b.Ha();
				b2.Ha();
				B[] array = new B[2] { b, b2 };
				if (num5 != 0)
				{
					B[] array2;
					(array2 = array)[1] = array2[1] >> num5;
				}
				return array;
			}

			public static B LeftShift(B bi, int n)
			{
				if (n == 0)
				{
					return new B(bi, bi.HC_0012 + 1);
				}
				int num = n >> 5;
				n &= 0x1F;
				B b = new B(_0001CB.Positive, bi.HC_0012 + 1 + (uint)num);
				uint num2 = 0u;
				uint num3 = bi.HC_0012;
				if (n != 0)
				{
					uint num4 = 0u;
					for (; num2 < num3; num2++)
					{
						uint num5 = bi.HCH[num2];
						b.HCH[num2 + num] = (num5 << n) | num4;
						num4 = num5 >> 32 - n;
					}
					b.HCH[num2 + num] = num4;
				}
				else
				{
					for (; num2 < num3; num2++)
					{
						b.HCH[num2 + num] = bi.HCH[num2];
					}
				}
				b.Ha();
				return b;
			}

			public static B RightShift(B bi, int n)
			{
				if (n == 0)
				{
					return new B(bi);
				}
				int num = n >> 5;
				int num2 = n & 0x1F;
				B b = new B(_0001CB.Positive, (uint)((int)bi.HC_0012 - num + 1));
				uint num3 = (uint)(b.HCH.Length - 1);
				if (num2 != 0)
				{
					uint num4 = 0u;
					while (num3-- != 0)
					{
						uint num5 = bi.HCH[num3 + num];
						b.HCH[num3] = (num5 >> n) | num4;
						num4 = num5 << 32 - n;
					}
				}
				else
				{
					while (num3-- != 0)
					{
						b.HCH[num3] = bi.HCH[num3 + num];
					}
				}
				b.Ha();
				return b;
			}

			public static B MultiplyByDword(B n, uint f)
			{
				B b = new B(_0001CB.Positive, n.HC_0012 + 1);
				uint num = 0u;
				ulong num2 = 0uL;
				do
				{
					num2 += (ulong)((long)n.HCH[num] * (long)f);
					b.HCH[num] = (uint)num2;
					num2 >>= 32;
				}
				while (++num < n.HC_0012);
				b.HCH[num] = (uint)num2;
				b.Ha();
				return b;
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

			public static B gcd(B a, B b)
			{
				B b2 = a;
				B b3 = b;
				B b4 = b3;
				while (b2.HC_0012 > 1)
				{
					b4 = b2;
					b2 = b3 % b2;
					b3 = b4;
				}
				if (b2 == 0u)
				{
					return b4;
				}
				uint num = b2.HCH[0];
				uint num2 = b3 % num;
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

			public static uint modInverse(B bi, uint modulus)
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

			public static B modInverse(B bi, B modulus)
			{
				if (modulus.HC_0012 == 1)
				{
					return modInverse(bi, modulus.HCH[0]);
				}
				B[] array = new B[2] { 0, 1 };
				B[] array2 = new B[2];
				B[] array3 = new B[2] { 0, 0 };
				int num = 0;
				B bi2 = modulus;
				B b = bi;
				_0001C_0002 obj = new _0001C_0002(modulus);
				while (b != 0u)
				{
					if (num > 1)
					{
						B b2 = obj.Difference(array[0], array[1] * array2[0]);
						array[0] = array[1];
						array[1] = b2;
					}
					B[] array4 = multiByteDivide(bi2, b);
					array2[0] = array2[1];
					array2[1] = array4[0];
					array3[0] = array3[1];
					array3[1] = array4[1];
					bi2 = b;
					b = array4[1];
					num++;
				}
				if (array3[0] != 1u)
				{
					throw new ArithmeticException("No inverse!");
				}
				return obj.Difference(array[0], array[1] * array2[0]);
			}
		}

		private const uint HCB = 20u;

		private const string HC_0002 = "Operation would return a negative value";

		private uint HC_0012 = 1u;

		private uint[] HCH;

		internal static readonly uint[] HC7 = new uint[783]
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

		private static z.y HC_0001;

		private static z.y Rng
		{
			get
			{
				if (HC_0001 == null)
				{
					HC_0001 = z.y.Create();
				}
				return HC_0001;
			}
		}

		public B()
		{
			HCH = new uint[20];
			HC_0012 = 20u;
		}

		public B(_0001CB sign, uint len)
		{
			HCH = new uint[len];
			HC_0012 = len;
		}

		public B(B bi)
		{
			HCH = (uint[])bi.HCH.Clone();
			HC_0012 = bi.HC_0012;
		}

		public B(B bi, uint len)
		{
			HCH = new uint[len];
			for (uint num = 0u; num < bi.HC_0012; num++)
			{
				HCH[num] = bi.HCH[num];
			}
			HC_0012 = bi.HC_0012;
		}

		public B(byte[] inData)
		{
			HC_0012 = (uint)inData.Length >> 2;
			int num = inData.Length & 3;
			if (num != 0)
			{
				HC_0012++;
			}
			HCH = new uint[HC_0012];
			int num2 = inData.Length - 1;
			int num3 = 0;
			while (num2 >= 3)
			{
				HCH[num3] = (uint)((inData[num2 - 3] << 24) | (inData[num2 - 2] << 16) | (inData[num2 - 1] << 8) | inData[num2]);
				num2 -= 4;
				num3++;
			}
			switch (num)
			{
			case 1:
				HCH[HC_0012 - 1] = inData[0];
				break;
			case 2:
				HCH[HC_0012 - 1] = (uint)((inData[0] << 8) | inData[1]);
				break;
			case 3:
				HCH[HC_0012 - 1] = (uint)((inData[0] << 16) | (inData[1] << 8) | inData[2]);
				break;
			}
			Ha();
		}

		public B(uint[] inData)
		{
			HC_0012 = (uint)inData.Length;
			HCH = new uint[HC_0012];
			int num = (int)(HC_0012 - 1);
			int num2 = 0;
			while (num >= 0)
			{
				HCH[num2] = inData[num];
				num--;
				num2++;
			}
			Ha();
		}

		public B(uint ui)
		{
			HCH = new uint[1] { ui };
		}

		public B(ulong ul)
		{
			HCH = new uint[2]
			{
				(uint)ul,
				(uint)(ul >> 32)
			};
			HC_0012 = 2u;
			Ha();
		}

		public static implicit operator B(uint value)
		{
			return new B(value);
		}

		public static implicit operator B(int value)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			return new B((uint)value);
		}

		public static implicit operator B(ulong value)
		{
			return new B(value);
		}

		public static B Parse(string number)
		{
			if (number == null)
			{
				throw new ArgumentNullException("number");
			}
			int i = 0;
			int length = number.Length;
			bool flag = false;
			B b = new B(0u);
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
					b = b * 10 + (c - 48);
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
			return b;
		}

		public static B operator +(B bi1, B bi2)
		{
			if (bi1 == 0u)
			{
				return new B(bi2);
			}
			if (bi2 == 0u)
			{
				return new B(bi1);
			}
			return _0001CH.AddSameSign(bi1, bi2);
		}

		public static B operator -(B bi1, B bi2)
		{
			if (bi2 == 0u)
			{
				return new B(bi1);
			}
			if (bi1 == 0u)
			{
				throw new ArithmeticException("Operation would return a negative value");
			}
			return _0001CH.Compare(bi1, bi2) switch
			{
				_0001CB.Zero => 0, 
				_0001CB.Positive => _0001CH.Subtract(bi1, bi2), 
				_0001CB.Negative => throw new ArithmeticException("Operation would return a negative value"), 
				_ => throw new Exception(), 
			};
		}

		public static int operator %(B bi, int i)
		{
			if (i > 0)
			{
				return (int)_0001CH.DwordMod(bi, (uint)i);
			}
			return (int)(0 - _0001CH.DwordMod(bi, (uint)(-i)));
		}

		public static uint operator %(B bi, uint ui)
		{
			return _0001CH.DwordMod(bi, ui);
		}

		public static B operator %(B bi1, B bi2)
		{
			return _0001CH.multiByteDivide(bi1, bi2)[1];
		}

		public static B operator /(B bi, int i)
		{
			if (i > 0)
			{
				return _0001CH.DwordDiv(bi, (uint)i);
			}
			throw new ArithmeticException("Operation would return a negative value");
		}

		public static B operator /(B bi1, B bi2)
		{
			return _0001CH.multiByteDivide(bi1, bi2)[0];
		}

		public static B operator *(B bi1, B bi2)
		{
			if (bi1 == 0u || bi2 == 0u)
			{
				return 0;
			}
			if (bi1.HCH.Length < bi1.HC_0012)
			{
				throw new IndexOutOfRangeException("bi1 out of range");
			}
			if (bi2.HCH.Length < bi2.HC_0012)
			{
				throw new IndexOutOfRangeException("bi2 out of range");
			}
			B b = new B(_0001CB.Positive, bi1.HC_0012 + bi2.HC_0012);
			_0001CH.Multiply(bi1.HCH, 0u, bi1.HC_0012, bi2.HCH, 0u, bi2.HC_0012, b.HCH, 0u);
			b.Ha();
			return b;
		}

		public static B operator *(B bi, int i)
		{
			if (i < 0)
			{
				throw new ArithmeticException("Operation would return a negative value");
			}
			return i switch
			{
				0 => 0, 
				1 => new B(bi), 
				_ => _0001CH.MultiplyByDword(bi, (uint)i), 
			};
		}

		public static B operator <<(B bi1, int shiftVal)
		{
			return _0001CH.LeftShift(bi1, shiftVal);
		}

		public static B operator >>(B bi1, int shiftVal)
		{
			return _0001CH.RightShift(bi1, shiftVal);
		}

		public static B Add(B bi1, B bi2)
		{
			return bi1 + bi2;
		}

		public static B Subtract(B bi1, B bi2)
		{
			return bi1 - bi2;
		}

		public static int Modulus(B bi, int i)
		{
			return bi % i;
		}

		public static uint Modulus(B bi, uint ui)
		{
			return bi % ui;
		}

		public static B Modulus(B bi1, B bi2)
		{
			return bi1 % bi2;
		}

		public static B Divid(B bi, int i)
		{
			return bi / i;
		}

		public static B Divid(B bi1, B bi2)
		{
			return bi1 / bi2;
		}

		public static B Multiply(B bi1, B bi2)
		{
			return bi1 * bi2;
		}

		public static B Multiply(B bi, int i)
		{
			return bi * i;
		}

		public static B GenerateRandom(int bits, z.y rng)
		{
			int num = bits >> 5;
			int num2 = bits & 0x1F;
			if (num2 != 0)
			{
				num++;
			}
			B b = new B(_0001CB.Positive, (uint)(num + 1));
			byte[] array = new byte[num << 2];
			rng.GetBytes(array);
			Buffer.BlockCopy(array, 0, b.HCH, 0, num << 2);
			if (num2 != 0)
			{
				uint num3 = (uint)(1 << num2 - 1);
				b.HCH[num - 1] |= num3;
				num3 = uint.MaxValue >> 32 - num2;
				b.HCH[num - 1] &= num3;
			}
			else
			{
				b.HCH[num - 1] |= 2147483648u;
			}
			b.Ha();
			return b;
		}

		public static B GenerateRandom(int bits)
		{
			return GenerateRandom(bits, Rng);
		}

		public int BitCount()
		{
			Ha();
			uint num = HCH[HC_0012 - 1];
			uint num2 = 2147483648u;
			uint num3 = 32u;
			while (num3 != 0 && (num & num2) == 0)
			{
				num3--;
				num2 >>= 1;
			}
			return (int)(num3 + (HC_0012 - 1 << 5));
		}

		public bool TestBit(uint bitNum)
		{
			uint num = bitNum >> 5;
			byte b = (byte)(bitNum & 0x1F);
			uint num2 = (uint)(1 << (int)b);
			return (HCH[num] & num2) != 0;
		}

		public bool TestBit(int bitNum)
		{
			if (bitNum < 0)
			{
				throw new IndexOutOfRangeException("bitNum out of range");
			}
			uint num = (uint)bitNum >> 5;
			byte b = (byte)(bitNum & 0x1F);
			uint num2 = (uint)(1 << (int)b);
			return (HCH[num] | num2) == HCH[num];
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
			if (num < HC_0012)
			{
				uint num2 = (uint)(1 << (int)(bitNum & 0x1F));
				if (value)
				{
					HCH[num] |= num2;
				}
				else
				{
					HCH[num] &= ~num2;
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
			for (int num5 = (int)(HC_0012 - 1); num5 >= 0; num5--)
			{
				uint num6 = HCH[num5];
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

		public static bool operator ==(B bi1, uint ui)
		{
			if (bi1.HC_0012 != 1)
			{
				bi1.Ha();
			}
			if (bi1.HC_0012 == 1)
			{
				return bi1.HCH[0] == ui;
			}
			return false;
		}

		public static bool operator !=(B bi1, uint ui)
		{
			if (bi1.HC_0012 != 1)
			{
				bi1.Ha();
			}
			if (bi1.HC_0012 == 1)
			{
				return bi1.HCH[0] != ui;
			}
			return true;
		}

		public static bool operator ==(B bi1, B bi2)
		{
			if ((object)bi1 == bi2)
			{
				return true;
			}
			if (null == bi1 || null == bi2)
			{
				return false;
			}
			return _0001CH.Compare(bi1, bi2) == _0001CB.Zero;
		}

		public static bool operator !=(B bi1, B bi2)
		{
			if ((object)bi1 == bi2)
			{
				return false;
			}
			if (null == bi1 || null == bi2)
			{
				return true;
			}
			return _0001CH.Compare(bi1, bi2) != _0001CB.Zero;
		}

		public static bool operator >(B bi1, B bi2)
		{
			return _0001CH.Compare(bi1, bi2) > _0001CB.Zero;
		}

		public static bool operator <(B bi1, B bi2)
		{
			return _0001CH.Compare(bi1, bi2) < _0001CB.Zero;
		}

		public static bool operator >=(B bi1, B bi2)
		{
			return _0001CH.Compare(bi1, bi2) >= _0001CB.Zero;
		}

		public static bool operator <=(B bi1, B bi2)
		{
			return _0001CH.Compare(bi1, bi2) <= _0001CB.Zero;
		}

		public _0001CB Compare(B bi)
		{
			return _0001CH.Compare(this, bi);
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
			B b = new B(this);
			while (b != 0u)
			{
				uint index = _0001CH.SingleByteDivideInPlace(b, radix);
				text = characterSet[(int)index] + text;
			}
			return text;
		}

		private void Ha()
		{
			while (HC_0012 != 0 && HCH[HC_0012 - 1] == 0)
			{
				HC_0012--;
			}
			if (HC_0012 == 0)
			{
				HC_0012++;
			}
		}

		public void Clear()
		{
			for (int i = 0; i < HC_0012; i++)
			{
				HCH[i] = 0u;
			}
		}

		public override int GetHashCode()
		{
			uint num = 0u;
			for (uint num2 = 0u; num2 < HC_0012; num2++)
			{
				num ^= HCH[num2];
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
			B b = o as B;
			if (b == null)
			{
				return false;
			}
			return _0001CH.Compare(this, b) == _0001CB.Zero;
		}

		public B GCD(B bi)
		{
			return _0001CH.gcd(this, bi);
		}

		public B ModInverse(B modulus)
		{
			return _0001CH.modInverse(this, modulus);
		}

		public B ModPow(B exp, B n)
		{
			_0001C_0002 obj = new _0001C_0002(n);
			return obj.Pow(this, exp);
		}
	}
}
