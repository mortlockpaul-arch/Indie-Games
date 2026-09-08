namespace System.Numerics;

internal static class Crc32ReflectedTable
{
	internal static uint[] Generate(uint reflectedPolynomial)
	{
		uint[] array = new uint[256];
		for (int i = 0; i < 256; i++)
		{
			uint num = (uint)i;
			for (int j = 0; j < 8; j++)
			{
				num = (((num & 1) != 0) ? ((num >> 1) ^ reflectedPolynomial) : (num >> 1));
			}
			array[i] = num;
		}
		return array;
	}
}
