using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using _0019;
using L;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace _0018
{
	internal class _3
	{
		internal enum DA_0018
		{
			Success,
			InvalidCode,
			HostUnreachable
		}

		private static byte[] _3A_0018 = new byte[596]
		{
			7, 2, 0, 0, 0, 164, 0, 0, 82, 83,
			65, 50, 0, 4, 0, 0, 1, 0, 1, 0,
			125, 109, 98, 29, 61, 54, 25, 149, 63, 147,
			210, 190, 131, 218, 2, 19, 208, 83, 145, 134,
			203, 38, 82, 51, 0, 217, 18, 0, 78, 85,
			17, 58, 210, 79, 196, 64, 252, 60, 104, 202,
			100, 5, 133, 115, 59, 144, 26, 238, 183, 249,
			248, 111, 67, 67, 74, 252, 21, 161, 175, 162,
			75, 84, 8, 243, 74, 191, 103, 202, 242, 83,
			187, 150, 43, 128, 132, 215, 68, 225, 203, 191,
			243, 196, 101, 250, 241, 163, 3, 236, 6, 214,
			122, 191, 181, 112, 64, 149, 64, 35, 9, 1,
			32, 22, 140, 164, 231, 31, 135, 40, 239, 200,
			26, 152, 125, 253, 82, 227, 43, 12, 121, 104,
			192, 89, 122, 81, 78, 134, 95, 169, 29, 9,
			167, 147, 64, 126, 67, 60, 7, 126, 60, 2,
			90, 180, 79, 185, 217, 239, 19, 137, 231, 169,
			28, 213, 36, 133, 167, 1, 39, 222, 144, 210,
			2, 47, 115, 224, 133, 43, 9, 117, 253, 126,
			244, 84, 198, 129, 82, 96, 240, 226, 125, 79,
			218, 139, 161, 189, 66, 138, 65, 38, 89, 77,
			86, 208, 225, 39, 240, 162, 32, 169, 156, 228,
			195, 83, 174, 66, 30, 135, 125, 119, 78, 61,
			169, 155, 127, 245, 14, 212, 146, 128, 251, 218,
			38, 187, 152, 58, 139, 116, 160, 47, 55, 20,
			227, 10, 56, 196, 110, 86, 163, 238, 3, 179,
			213, 56, 212, 239, 132, 132, 49, 100, 38, 249,
			229, 156, 78, 55, 31, 208, 237, 198, 198, 194,
			117, 192, 160, 84, 185, 251, 212, 202, 0, 145,
			165, 12, 231, 83, 229, 23, 131, 168, 132, 33,
			221, 121, 99, 2, 25, 126, 89, 68, 248, 228,
			80, 254, 121, 161, 223, 203, 73, 49, 213, 217,
			6, 58, 194, 163, 153, 234, 91, 50, 194, 188,
			123, 107, 68, 179, 187, 144, 119, 61, 244, 47,
			225, 44, 33, 62, 212, 54, 212, 58, 0, 102,
			156, 211, 68, 12, 228, 233, 214, 251, 224, 232,
			53, 102, 183, 101, 77, 253, 206, 79, 204, 3,
			214, 105, 113, 42, 156, 170, 84, 54, 20, 170,
			118, 9, 75, 1, 249, 221, 138, 199, 195, 42,
			22, 206, 168, 171, 38, 209, 92, 184, 132, 15,
			244, 219, 86, 154, 93, 165, 108, 133, 93, 152,
			6, 170, 16, 155, 9, 210, 224, 144, 139, 222,
			55, 133, 159, 250, 234, 200, 28, 165, 198, 209,
			97, 3, 242, 147, 31, 136, 149, 253, 117, 120,
			237, 15, 167, 89, 121, 208, 204, 80, 85, 30,
			47, 161, 250, 60, 187, 54, 62, 79, 245, 185,
			110, 171, 137, 27, 252, 65, 189, 169, 1, 47,
			109, 140, 82, 134, 63, 10, 16, 74, 9, 215,
			50, 247, 22, 66, 172, 41, 142, 196, 75, 226,
			6, 196, 87, 26, 163, 238, 49, 98, 139, 199,
			93, 158, 49, 10, 163, 75, 117, 39, 237, 189,
			6, 156, 8, 159, 157, 174, 214, 110, 14, 200,
			137, 83, 231, 205, 243, 156, 225, 193, 254, 167,
			61, 235, 20, 13, 122, 90, 180, 242, 177, 113,
			121, 107, 176, 156, 190, 153, 192, 232, 150, 143,
			197, 249, 144, 127, 181, 80, 241, 156, 146, 178,
			204, 185, 202, 59, 232, 6, 31, 14, 227, 159,
			72, 66, 156, 107, 249, 113, 114, 25, 142, 227,
			240, 255, 56, 89, 9, 63, 93, 156, 44, 246,
			93, 182, 180, 162, 54, 42
		};

		private static byte[] _3AL = new byte[148]
		{
			6, 2, 0, 0, 0, 164, 0, 0, 82, 83,
			65, 49, 0, 4, 0, 0, 1, 0, 1, 0,
			137, 16, 179, 235, 143, 151, 177, 248, 171, 249,
			190, 6, 55, 35, 219, 161, 4, 151, 250, 70,
			41, 255, 190, 150, 134, 80, 52, 234, 61, 198,
			175, 162, 210, 76, 123, 78, 253, 195, 125, 5,
			147, 80, 228, 71, 137, 91, 225, 148, 6, 201,
			4, 182, 135, 88, 180, 128, 13, 33, 235, 80,
			46, 245, 81, 184, 78, 162, 48, 79, 9, 172,
			198, 45, 61, 101, 247, 14, 77, 11, 192, 32,
			49, 183, 54, 198, 15, 175, 7, 53, 150, 189,
			93, 161, 73, 191, 32, 27, 150, 139, 166, 230,
			116, 6, 207, 164, 225, 6, 117, 204, 231, 254,
			134, 222, 212, 102, 97, 227, 38, 133, 158, 66,
			141, 234, 50, 102, 139, 184, 198, 212
		};

		private global::L._0018 _3A_0019;

		private char[] _3A3 = new char[1] { '-' };

		private Dictionary<int, byte[]> _3A6 = new Dictionary<int, byte[]>(16);

		internal bool c(byte[] P_0)
		{
			if (P_0 == null)
			{
				return false;
			}
			int key = g(P_0);
			if (_3A6.TryGetValue(key, out var value))
			{
				if (P_0.Length != value.Length)
				{
					return false;
				}
				for (int i = 0; i < P_0.Length; i++)
				{
					if (P_0[i] != value[i])
					{
						return false;
					}
				}
				return true;
			}
			try
			{
				TimeSpan timeSpan = default(TimeSpan);
				global::_0019._0018 obj = new global::_0019._0018(_3A_0019);
				obj._0001(P_0, out var _, out var num, out var num2, out var dateTime);
				if (!_8(num, num2, dateTime, _0019._3AL[0].DRMProductName, 1u, ref timeSpan))
				{
					return false;
				}
				_3A6.Add(key, P_0);
				return true;
			}
			catch
			{
			}
			return false;
		}

		internal _3()
		{
			_3A_0019 = new global::L._0018(_3A_0018, _3AL);
		}

		private int g(byte[] P_0)
		{
			int num = 0;
			for (int i = 0; i < P_0.Length; i++)
			{
				num += P_0[i] << i;
			}
			return num;
		}

		internal bool I(string P_0, string P_1, uint P_2)
		{
			if (_8(P_0, P_1, P_2, out var _))
			{
				return true;
			}
			return false;
		}

		private bool _8(string P_0, string P_1, uint P_2, out TimeSpan P_3)
		{
			P_3 = TimeSpan.MaxValue;
			try
			{
				x(P_0, out var text, out var num, out var num2, out var dateTime);
				if (text == "")
				{
					return false;
				}
				if (!_8(num, num2, dateTime, P_1, P_2, ref P_3))
				{
					return false;
				}
				string[] array = text.Split(_3A3);
				if (array == null || array.Length < 5)
				{
					_0018._3A3 = "-unlicensed-";
				}
				else
				{
					_0018._3A3 = "****-" + array[4];
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		private bool _8(uint P_0, uint P_1, DateTime P_2, string P_3, uint P_4, ref TimeSpan P_5)
		{
			if (P_1 == 0 || P_4 == 0)
			{
				return false;
			}
			uint num = (uint)Z(P_3);
			if (P_0 != num || P_0 == 0 || P_3 == "")
			{
				return false;
			}
			P_5 = default(TimeSpan);
			return true;
		}

		internal static int Z(string P_0)
		{
			int num = 352654597;
			int num2 = num;
			_ = P_0.Length;
			for (int i = 0; i < P_0.Length; i += 2)
			{
				char c2 = P_0[i];
				char c3 = '\0';
				if (i < P_0.Length - 1)
				{
					c3 = P_0[i + 1];
				}
				int num3 = (int)(c2 | ((uint)c3 << 16));
				if (i % 4 == 0)
				{
					num = ((num << 5) + num + (num >> 27)) ^ num3;
				}
				else
				{
					num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ num3;
				}
			}
			return num + num2 * 1566083941;
		}

		private void x(string P_0, out string P_1, out uint P_2, out uint P_3, out DateTime P_4)
		{
			try
			{
				string path = _0019.ActivationPath + P_0;
				using FileStream fileStream = File.OpenRead(path);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				int num = global::L.L.T(array, array.Length - 4);
				byte[] array2 = new byte[num];
				Array.Copy(array, array.Length - (num + 4), array2, 0, num);
				byte[] array3 = new byte[num];
				Array.Copy(array2, array3, num);
				int key = g(array3);
				_3A6.Clear();
				_3A6.Add(key, array3);
				global::_0019._0018 obj = new global::_0019._0018(_3A_0019);
				obj._0001(array2, out P_1, out P_2, out P_3, out P_4);
			}
			catch
			{
				P_1 = "";
				P_2 = 0u;
				P_3 = 0u;
				P_4 = DateTime.MinValue;
			}
		}
	}
}
namespace _0003
{
	internal class _3
	{
		internal const string _3A_0018 = "Locale";

		internal const string _3AL = "EffectFile";

		internal const string _3A_0019 = "Technique";

		internal const string _3A3 = "DepthTechnique";

		internal const string _3A6 = "GBufferTechnique";

		internal const string _3AD = "FinalTechnique";

		internal const string _3A_0017 = "ShadowGenerationTechnique";

		internal const string _3A_0003 = "Elasticity";

		internal const string _3Al = "Friction";

		internal const string _3At = "DoubleSided";

		internal const string _3AF = "TransparencyMode";

		internal const string _3Ac = "Transparency";

		internal const string _3Ag = "TransparencyThreshold";

		internal const string _3AI = "TransparencyAmount";

		internal const string _3A8 = "TransparencyMapParameterName";

		internal const string _3AZ = "SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\Synapse Gaming - SunBurn {0} {1}";

		internal const string _3Ax = "\\Development\\Windows";

		internal const string _3Aq = "\\ShaderLibrary";

		internal static object _0001(Type P_0, string P_1, CultureInfo P_2)
		{
			if ((object)P_0 == typeof(string))
			{
				return P_1;
			}
			if ((object)P_0 == typeof(bool))
			{
				return L3(P_1);
			}
			if ((object)P_0 == typeof(float))
			{
				return L6(P_1, P_2);
			}
			if ((object)P_0 == typeof(int))
			{
				return (int)L6(P_1, P_2);
			}
			if ((object)P_0 == typeof(Vector4))
			{
				return LD(P_1, P_2, Vector4.Zero);
			}
			return null;
		}

		internal static T L_0019<T>(string P_0)
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

		internal static bool L3(string P_0)
		{
			return bool.Parse(P_0);
		}

		internal static float L6(string P_0, CultureInfo P_1)
		{
			return float.Parse(P_0, P_1.NumberFormat);
		}

		internal static Vector4 LD(string P_0, CultureInfo P_1, Vector4 P_2)
		{
			string[] array = Regex.Split(P_0, " ");
			if (array.Length < 3 || array.Length > 4)
			{
				throw new Exception("Invalid vector data.");
			}
			P_2.X = L6(array[0], P_1);
			P_2.Y = L6(array[1], P_1);
			P_2.Z = L6(array[2], P_1);
			if (array.Length > 3)
			{
				P_2.W = L6(array[3], P_1);
			}
			return P_2;
		}
	}
}
namespace _0016
{
	internal class _3
	{
		private Dictionary<Type, MemberInfo[]> _3A_0018 = new Dictionary<Type, MemberInfo[]>(16);

		private Dictionary<string, Type> _3AL = new Dictionary<string, Type>(16);

		internal void _6v()
		{
			_65(typeof(ContentRepository));
			_65(typeof(Vector2));
			_65(typeof(Vector3));
			_65(typeof(Vector4));
			_65(typeof(Matrix));
			_65(typeof(Scene));
			_65(typeof(SceneEnvironment));
		}

		internal void U()
		{
			_3A_0018.Clear();
			_3AL.Clear();
		}

		internal Type _6C(string P_0, string P_1, string P_2)
		{
			if (_3AL.TryGetValue(P_2, out var value))
			{
				return value;
			}
			value = _6r(P_0, P_1, P_2);
			if ((object)value == null)
			{
				throw new Exception($"Type '{P_0}' not registered with the serialization type dictionary.");
			}
			_65(P_2, value);
			return value;
		}

		internal void V(string P_0, Type P_1)
		{
			if (!string.IsNullOrEmpty(P_0) && !_3AL.ContainsKey(P_0))
			{
				_65(P_0, P_1);
			}
		}

		internal void _6a(string P_0, out Type P_1, out MemberInfo[] P_2)
		{
			if (string.IsNullOrEmpty(P_0) || !_3AL.ContainsKey(P_0))
			{
				throw new Exception("Type '" + P_0 + "' not registered with the serialization type dictionary.");
			}
			P_1 = _3AL[P_0];
			P_2 = _3A_0018[P_1];
		}

		private void _65(Type P_0)
		{
			string name = P_0.Name;
			if (!string.IsNullOrEmpty(name) && !_3AL.ContainsKey(name))
			{
				_65(name, P_0);
			}
		}

		private void _65(string P_0, Type P_1)
		{
			_3AL.Add(P_0, P_1);
			MemberInfo[] value = ((!P_1.IsGenericTypeDefinition) ? L._6u(P_1) : new MemberInfo[0]);
			if (!_3A_0018.ContainsKey(P_1))
			{
				_3A_0018.Add(P_1, value);
			}
		}

		internal static Type _6r(string P_0, string P_1, string P_2)
		{
			Type type = Type.GetType($"{P_0}, {P_1}");
			if ((object)type != null)
			{
				return type;
			}
			type = Type.GetType(P_0);
			if ((object)type != null)
			{
				return type;
			}
			P_0 = _6_0012(P_0);
			type = Type.GetType(P_0);
			if ((object)type != null)
			{
				return type;
			}
			return null;
		}

		private static string _6_0012(string P_0)
		{
			if (P_0.StartsWith("SynapseGaming.LightingSystem.Core.IComponent"))
			{
				return P_0.Replace("SynapseGaming.LightingSystem.Core.IComponent", "SynapseGaming.LightingSystem.Components.IComponent");
			}
			return P_0;
		}
	}
}
