using System;
using Microsoft.Xna.Framework.Graphics;

public static class FnaXboxBufferFix
{
	public static byte[] SwapVertexData(VertexDeclaration declaration, byte[] data)
	{
		if (declaration == null || data == null)
		{
			return data;
		}
		int vertexStride = declaration.VertexStride;
		VertexElement[] vertexElements = declaration.GetVertexElements();
		for (int i = 0; i + vertexStride <= data.Length; i += vertexStride)
		{
			for (int j = 0; j < vertexElements.Length; j++)
			{
				VertexElement vertexElement = vertexElements[j];
				int offset = i + vertexElement.Offset;
				switch (vertexElement.VertexElementFormat)
				{
				case VertexElementFormat.Single:
					SwapComponents(data, offset, 4, 1);
					break;
				case VertexElementFormat.Vector2:
					SwapComponents(data, offset, 4, 2);
					break;
				case VertexElementFormat.Vector3:
					SwapComponents(data, offset, 4, 3);
					break;
				case VertexElementFormat.Vector4:
					SwapComponents(data, offset, 4, 4);
					break;
				case VertexElementFormat.Color:
					SwapComponents(data, offset, 4, 1);
					break;
				case VertexElementFormat.Short2:
				case VertexElementFormat.NormalizedShort2:
				case VertexElementFormat.HalfVector2:
					SwapComponents(data, offset, 2, 2);
					break;
				case VertexElementFormat.Short4:
				case VertexElementFormat.NormalizedShort4:
				case VertexElementFormat.HalfVector4:
					SwapComponents(data, offset, 2, 4);
					break;
				default:
					throw new NotSupportedException("Unhandled Xbox vertex format: " + vertexElement.VertexElementFormat);
				case VertexElementFormat.Byte4:
					break;
				}
			}
		}
		return data;
	}

	public static byte[] SwapIndexData(byte[] data, bool sixteenBits)
	{
		if (data == null)
		{
			return null;
		}
		SwapComponents(data, 0, sixteenBits ? 2 : 4, data.Length / (sixteenBits ? 2 : 4));
		return data;
	}

	private static void SwapComponents(byte[] data, int offset, int componentSize, int componentCount)
	{
		for (int i = 0; i < componentCount; i++)
		{
			int num = offset + i * componentSize;
			int num2 = num + componentSize - 1;
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
