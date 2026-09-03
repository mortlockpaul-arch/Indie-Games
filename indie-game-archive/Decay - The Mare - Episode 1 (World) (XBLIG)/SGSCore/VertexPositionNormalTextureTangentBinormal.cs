using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct VertexPositionNormalTextureTangentBinormal(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Vector3 tangent, Vector3 binormal)
{
	public Vector3 Position = position;

	public Vector3 Normal = normal;

	public Vector2 TextureCoordinate = textureCoordinate;

	public Vector3 Tangent = tangent;

	public Vector3 Binormal = binormal;

	public static readonly VertexElement[] VertexElements = new VertexElement[5]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
		new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
	};

	public static int SizeInBytes => 56;
}
