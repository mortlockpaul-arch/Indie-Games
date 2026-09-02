using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game;

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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Vertices = (VertexPositionNormalTexture[])(object)new VertexPositionNormalTexture[4];
		Indexes = new int[6];
		Origin = origin;
		Normal = normal;
		Up = up;
		Left = Vector3.Cross(normal, Up);
		Vector3 val = Up * height / 2f + origin;
		UpperLeft = val + Left * width / 2f;
		UpperRight = val - Left * width / 2f;
		LowerLeft = UpperLeft - Up * height;
		LowerRight = UpperRight - Up * height;
		FillVertices(ref Vertices, 0, ref Indexes, 0, Normal);
	}

	private void FillVertices(ref VertexPositionNormalTexture[] Vertices, int vertexOffset, ref int[] Indices, int indexOffset, Vector3 normal)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 textureCoordinate = default(Vector2);
		((Vector2)(ref textureCoordinate))._002Ector(0f, 0f);
		Vector2 textureCoordinate2 = default(Vector2);
		((Vector2)(ref textureCoordinate2))._002Ector(1f, 0f);
		Vector2 textureCoordinate3 = default(Vector2);
		((Vector2)(ref textureCoordinate3))._002Ector(0f, 1f);
		Vector2 textureCoordinate4 = default(Vector2);
		((Vector2)(ref textureCoordinate4))._002Ector(1f, 1f);
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
