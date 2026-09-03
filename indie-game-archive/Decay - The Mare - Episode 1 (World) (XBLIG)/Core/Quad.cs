using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public struct Quad
{
	public Vector3 Origin;

	public Vector3 UpperLeft;

	public Vector3 LowerLeft;

	public Vector3 UpperRight;

	public Vector3 LowerRight;

	public Vector3 Normal;

	public Vector3 Up;

	public Vector3 Left;

	public VertexPositionNormalTexture[] Vertices;

	public int[] Indexes;

	public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height)
	{
		Vertices = new VertexPositionNormalTexture[4];
		Indexes = new int[6];
		Origin = origin;
		Normal = normal;
		Up = up;
		Left = Vector3.Cross(normal, Up);
		Vector3 vector = Up * height / 2f + origin;
		UpperLeft = vector + Left * width / 2f;
		UpperRight = vector - Left * width / 2f;
		LowerLeft = UpperLeft - Up * height;
		LowerRight = UpperRight - Up * height;
		FillVertices(ref Vertices, 0, ref Indexes, 0, Normal);
	}

	private void FillVertices(ref VertexPositionNormalTexture[] Vertices, int vertexOffset, ref int[] Indices, int indexOffset, Vector3 normal)
	{
		Vector2 textureCoordinate = new Vector2(0f, 0f);
		Vector2 textureCoordinate2 = new Vector2(1f, 0f);
		Vector2 textureCoordinate3 = new Vector2(0f, 1f);
		Vector2 textureCoordinate4 = new Vector2(1f, 1f);
		for (int i = 0; i < Vertices.Length; i++)
		{
			Vertices[i].Normal = normal;
		}
		Vertices[vertexOffset].Position = LowerLeft;
		Vertices[vertexOffset].TextureCoordinate = textureCoordinate3;
		Vertices[vertexOffset + 1].Position = UpperLeft;
		Vertices[vertexOffset + 1].TextureCoordinate = textureCoordinate;
		Vertices[vertexOffset + 2].Position = LowerRight;
		Vertices[vertexOffset + 2].TextureCoordinate = textureCoordinate4;
		Vertices[vertexOffset + 3].Position = UpperRight;
		Vertices[vertexOffset + 3].TextureCoordinate = textureCoordinate2;
		Indices[indexOffset] = 0;
		Indices[indexOffset + 1] = 1;
		Indices[indexOffset + 2] = 2;
		Indices[indexOffset + 3] = 2;
		Indices[indexOffset + 4] = 1;
		Indices[indexOffset + 5] = 3;
	}
}
