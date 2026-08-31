using System;
using System.Collections.Generic;
using System.IO;
using _0002;
using _0012;

namespace B;

internal class H
{
	internal enum _0001CB
	{
		Success,
		InvalidCode,
		HostUnreachable
	}

	private static byte[] HCB = new byte[596]
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

	private static byte[] HC_0002 = new byte[148]
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

	private global::_0002.B HC_0012;

	private char[] HCH = new char[1] { '-' };

	private Dictionary<int, byte[]> HC7 = new Dictionary<int, byte[]>(16);

	internal bool D(byte[] P_0)
	{
		if (P_0 == null)
		{
			return false;
		}
		int key = _0011(P_0);
		if (HC7.TryGetValue(key, out var value))
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
			global::_0012.B b = new global::_0012.B(HC_0012);
			b.Y(P_0, out var _, out var num, out var num2, out var dateTime);
			if (!_0003(num, num2, dateTime, _0012.HC_0002[0].DRMProductName, 1u, ref timeSpan))
			{
				return false;
			}
			HC7.Add(key, P_0);
			return true;
		}
		catch
		{
		}
		return false;
	}

	internal H()
	{
		HC_0012 = new global::_0002.B(HCB, HC_0002);
	}

	private int _0011(byte[] P_0)
	{
		int num = 0;
		for (int i = 0; i < P_0.Length; i++)
		{
			num += P_0[i] << i;
		}
		return num;
	}

	internal bool K(string P_0, string P_1, uint P_2)
	{
		if (_0003(P_0, P_1, P_2, out var _))
		{
			return true;
		}
		return false;
	}

	private bool _0003(string P_0, string P_1, uint P_2, out TimeSpan P_3)
	{
		P_3 = TimeSpan.MaxValue;
		try
		{
			s(P_0, out var text, out var num, out var num2, out var dateTime);
			if (text == "")
			{
				return false;
			}
			if (!_0003(num, num2, dateTime, P_1, P_2, ref P_3))
			{
				return false;
			}
			string[] array = text.Split(HCH);
			if (array == null || array.Length < 5)
			{
				B.HCH = "-unlicensed-";
			}
			else
			{
				B.HCH = "****-" + array[4];
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private bool _0003(uint P_0, uint P_1, DateTime P_2, string P_3, uint P_4, ref TimeSpan P_5)
	{
		if (P_1 == 0 || P_4 == 0)
		{
			return false;
		}
		uint num = (uint)k(P_3);
		if (P_0 != num || P_0 == 0 || P_3 == "")
		{
			return false;
		}
		P_5 = default(TimeSpan);
		return true;
	}

	internal static int k(string P_0)
	{
		int num = 352654597;
		int num2 = num;
		_ = P_0.Length;
		for (int i = 0; i < P_0.Length; i += 2)
		{
			char c = P_0[i];
			char c2 = '\0';
			if (i < P_0.Length - 1)
			{
				c2 = P_0[i + 1];
			}
			int num3 = (int)(c | ((uint)c2 << 16));
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

	private void s(string P_0, out string P_1, out uint P_2, out uint P_3, out DateTime P_4)
	{
		try
		{
			string path = _0012.ActivationPath + P_0;
			using FileStream fileStream = File.OpenRead(path);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			int num = global::_0002._0002.z(array, array.Length - 4);
			byte[] array2 = new byte[num];
			Array.Copy(array, array.Length - (num + 4), array2, 0, num);
			byte[] array3 = new byte[num];
			Array.Copy(array2, array3, num);
			int key = _0011(array3);
			HC7.Clear();
			HC7.Add(key, array3);
			global::_0012.B b = new global::_0012.B(HC_0012);
			b.Y(array2, out P_1, out P_2, out P_3, out P_4);
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
