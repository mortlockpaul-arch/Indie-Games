using System.Collections.Generic;

namespace _0003;

internal class g
{
	private struct DA_0018
	{
		internal string _3A_0018;

		internal float _3AL;

		internal DA_0018(string P_0, float P_1)
		{
			_3A_0018 = P_0;
			_3AL = P_1;
		}
	}

	private static List<DA_0018> _3A_0018 = new List<DA_0018>(32);

	internal static void L_000E(string P_0)
	{
		_3A_0018.Add(new DA_0018(P_0, -1f));
	}

	internal static void L9(string P_0, float P_1)
	{
		_3A_0018.Add(new DA_0018(P_0, P_1));
	}

	internal static void L_0004(string P_0)
	{
	}
}
