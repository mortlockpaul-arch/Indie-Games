using System;
using b;

namespace _6;

internal class h
{
	private b.h a5h;

	internal h(b.h P_0)
	{
		a5h = P_0;
	}

	internal void m(byte[] P_0, out string P_1, out uint P_2, out uint P_3, out DateTime P_4)
	{
		byte[] array = a5h.Z(P_0);
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
			P_1 += global::b.b.Y(array2, 0);
		}
		P_4 = DateTime.Now;
		P_2 = global::b.b.z(array, array.Length - 8);
		P_3 = global::b.b.z(array, array.Length - 4);
	}
}
