using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct VertexPositionNormalTextureTangentBinormal
{
	public Vector3 Position;

	public Vector3 Normal;

	public Vector2 TextureCoordinate;

	public Vector3 Tangent;

	public Vector3 Binormal;

	public static readonly VertexElement[] VertexElements;

	public static int SizeInBytes => 56;

	public VertexPositionNormalTextureTangentBinormal(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Vector3 tangent, Vector3 binormal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Position = position;
		Normal = normal;
		TextureCoordinate = textureCoordinate;
		Tangent = tangent;
		Binormal = binormal;
	}

	static VertexPositionNormalTextureTangentBinormal()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		VertexElements = (VertexElement[])(object)new VertexElement[5]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)0),
			new VertexElement((short)0, (short)24, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0),
			new VertexElement((short)0, (short)32, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)6, (byte)0),
			new VertexElement((short)0, (short)44, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)7, (byte)0)
		};
	}
}
