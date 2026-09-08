using System;

namespace L
{
	internal class L
	{
		private static byte[] _3A_0018 = new byte[8];

		internal static int T(byte[] P_0, int P_1)
		{
			_3A_0018[3] = P_0[P_1++];
			_3A_0018[2] = P_0[P_1++];
			_3A_0018[1] = P_0[P_1++];
			_3A_0018[0] = P_0[P_1++];
			return BitConverter.ToInt32(_3A_0018, 0);
		}

		internal static uint y(byte[] P_0, int P_1)
		{
			_3A_0018[3] = P_0[P_1++];
			_3A_0018[2] = P_0[P_1++];
			_3A_0018[1] = P_0[P_1++];
			_3A_0018[0] = P_0[P_1++];
			return BitConverter.ToUInt32(_3A_0018, 0);
		}

		internal static char _0015(byte[] P_0, int P_1)
		{
			_3A_0018[1] = P_0[P_1++];
			_3A_0018[0] = P_0[P_1++];
			return BitConverter.ToChar(_3A_0018, 0);
		}
	}
}
namespace l
{
	internal delegate void L(float percent, string description);
}
