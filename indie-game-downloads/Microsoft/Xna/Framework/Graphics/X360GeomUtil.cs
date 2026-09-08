namespace Microsoft.Xna.Framework.Graphics;

internal static class X360GeomUtil
{
	internal static void SwapIndices(byte[] indexData, bool sixteenBits)
	{
		SwapComponents(indexData, 0, indexData.Length, sixteenBits ? 2 : 4);
	}

	internal static void SwapVertices(byte[] vertexData, VertexDeclaration declaration, int vertexCount)
	{
		int vertexStride = declaration.VertexStride;
		VertexElement[] vertexElements = declaration.GetVertexElements();
		for (int i = 0; i < vertexCount; i++)
		{
			int num = i * vertexStride;
			for (int j = 0; j < vertexElements.Length; j++)
			{
				VertexElementFormat vertexElementFormat = vertexElements[j].VertexElementFormat;
				SwapComponents(vertexData, num + vertexElements[j].Offset, VertexDeclaration.GetTypeSize(vertexElementFormat), GetComponentSize(vertexElementFormat));
			}
		}
	}

	private static int GetComponentSize(VertexElementFormat elementFormat)
	{
		if ((uint)(elementFormat - 6) <= 5u)
		{
			return 2;
		}
		return 4;
	}

	private static void SwapComponents(byte[] data, int offset, int length, int componentSize)
	{
		for (int i = offset; i < offset + length; i += componentSize)
		{
			int num = i;
			int num2 = i + componentSize - 1;
			while (num < num2)
			{
				byte b = data[num];
				data[num] = data[num2];
				data[num2] = b;
				num++;
				num2--;
			}
		}
	}
}
