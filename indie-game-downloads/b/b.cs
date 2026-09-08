using System;

namespace b;

internal class b
{
	private static byte[] a5h = new byte[8];

	internal static int L(byte[] P_0, int P_1)
	{
		a5h[3] = P_0[P_1++];
		a5h[2] = P_0[P_1++];
		a5h[1] = P_0[P_1++];
		a5h[0] = P_0[P_1++];
		return BitConverter.ToInt32(a5h, 0);
	}

	internal static uint z(byte[] P_0, int P_1)
	{
		a5h[3] = P_0[P_1++];
		a5h[2] = P_0[P_1++];
		a5h[1] = P_0[P_1++];
		a5h[0] = P_0[P_1++];
		return BitConverter.ToUInt32(a5h, 0);
	}

	internal static char Y(byte[] P_0, int P_1)
	{
		a5h[1] = P_0[P_1++];
		a5h[0] = P_0[P_1++];
		return BitConverter.ToChar(a5h, 0);
	}
}
