using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace Z
{
	internal class H
	{
		internal const string HCB = "Locale";

		internal const string HC_0002 = "EffectFile";

		internal const string HC_0012 = "Technique";

		internal const string HCH = "DepthTechnique";

		internal const string HC7 = "GBufferTechnique";

		internal const string HC_0001 = "FinalTechnique";

		internal const string HCw = "ShadowGenerationTechnique";

		internal const string HCZ = "Elasticity";

		internal const string HC_000F = "Friction";

		internal const string HCy = "DoubleSided";

		internal const string HC6 = "TransparencyMode";

		internal const string HCD = "Transparency";

		internal const string HC_0011 = "TransparencyThreshold";

		internal const string HCK = "TransparencyAmount";

		internal const string HC_0003 = "TransparencyMapParameterName";

		internal const string HCk = "SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\Synapse Gaming - SunBurn {0} {1}";

		internal const string HCs = "\\Development\\Windows";

		internal const string HC_0013 = "\\ShaderLibrary";

		internal static object Y(Type P_0, string P_1, CultureInfo P_2)
		{
			if ((object)P_0 == typeof(string))
			{
				return P_1;
			}
			if ((object)P_0 == typeof(bool))
			{
				return _0002_0012(P_1);
			}
			if ((object)P_0 == typeof(float))
			{
				return _0002H(P_1, P_2);
			}
			if ((object)P_0 == typeof(int))
			{
				return (int)_0002H(P_1, P_2);
			}
			if ((object)P_0 == typeof(Vector4))
			{
				return _00027(P_1, P_2, Vector4.Zero);
			}
			return null;
		}

		internal static T _0002_0002<T>(string P_0)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), P_0, ignoreCase: true);
			}
			catch
			{
				throw new Exception("Invalid property value '" + P_0 + "'.");
			}
		}

		internal static bool _0002_0012(string P_0)
		{
			return bool.Parse(P_0);
		}

		internal static float _0002H(string P_0, CultureInfo P_1)
		{
			return float.Parse(P_0, P_1.NumberFormat);
		}

		internal static Vector4 _00027(string P_0, CultureInfo P_1, Vector4 P_2)
		{
			string[] array = Regex.Split(P_0, " ");
			if (array.Length < 3 || array.Length > 4)
			{
				throw new Exception("Invalid vector data.");
			}
			P_2.X = _0002H(array[0], P_1);
			P_2.Y = _0002H(array[1], P_1);
			P_2.Z = _0002H(array[2], P_1);
			if (array.Length > 3)
			{
				P_2.W = _0002H(array[3], P_1);
			}
			return P_2;
		}
	}
}
namespace z
{
	internal class H
	{
		private const string HCB = "1.3.14.3.2.26";

		private static object HC_0002;

		static H()
		{
			HC_0002 = new object();
		}

		private static void Hd()
		{
		}

		private static byte[] Hn(long P_0)
		{
			if (P_0 > int.MaxValue || P_0 < int.MinValue)
			{
				throw new OverflowException("Part of OID doesn't fit in Int32");
			}
			long num = P_0;
			int num2 = 1;
			while (num > 127)
			{
				num >>= 7;
				num2++;
			}
			byte[] array = new byte[num2];
			for (int i = 0; i < num2; i++)
			{
				num = P_0 >> 7 * i;
				num &= 0x7F;
				if (i != 0)
				{
					num += 128;
				}
				array[num2 - i - 1] = Convert.ToByte(num);
			}
			return array;
		}

		public static byte[] EncodeOID(string str)
		{
			char[] separator = new char[1] { '.' };
			string[] array = str.Split(separator);
			if (array.Length < 2)
			{
				throw new _0001("OID must have at least two parts");
			}
			byte[] array2 = new byte[str.Length];
			try
			{
				byte b = Convert.ToByte(array[0]);
				byte b2 = Convert.ToByte(array[1]);
				array2[2] = Convert.ToByte(b * 40 + b2);
			}
			catch
			{
				throw new _0001("Invalid OID");
			}
			int num = 3;
			for (int i = 2; i < array.Length; i++)
			{
				long num2 = Convert.ToInt64(array[i]);
				if (num2 > 127)
				{
					byte[] array3 = Hn(num2);
					Buffer.BlockCopy(array3, 0, array2, num, array3.Length);
					num += array3.Length;
				}
				else
				{
					array2[num++] = Convert.ToByte(num2);
				}
			}
			int num3 = 2;
			byte[] array4 = new byte[num];
			array4[0] = 6;
			if (num > 127)
			{
				throw new _0001("OID > 127 bytes");
			}
			array4[1] = Convert.ToByte(num - 2);
			Buffer.BlockCopy(array2, num3, array4, num3, num - num3);
			return array4;
		}

		public static string MapNameToOID(string name)
		{
			return "1.3.14.3.2.26";
		}
	}
}
