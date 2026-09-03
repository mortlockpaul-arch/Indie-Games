using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct VertexPositionBoneIndicesBoneWeightsNormalTexture(Vector4 position, Vector4 blendindicies, Vector4 blendweight, Vector3 normal, Vector2 textureCoordinate)
{
	public Vector4 Position = position;

	public Vector4 BlendIndices = blendindicies;

	public Vector4 BlendWeight = blendweight;

	public Vector3 Normal = normal;

	public Vector2 TextureCoordinate = textureCoordinate;

	public static readonly VertexElement[] VertexElements = new VertexElement[5]
	{
		new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
		new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendIndices, 0),
		new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
		new VertexElement(48, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		new VertexElement(60, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
	};

	public static int SizeInBytes => 68;
}
