using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct VertexPositionBoneIndicesBoneWeightsNormalTexture
{
	public Vector4 Position;

	public Vector4 BlendIndices;

	public Vector4 BlendWeight;

	public Vector3 Normal;

	public Vector2 TextureCoordinate;

	public static readonly VertexElement[] VertexElements;

	public static int SizeInBytes => 68;

	public VertexPositionBoneIndicesBoneWeightsNormalTexture(Vector4 position, Vector4 blendindicies, Vector4 blendweight, Vector3 normal, Vector2 textureCoordinate)
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
		BlendIndices = blendindicies;
		BlendWeight = blendweight;
		Normal = normal;
		TextureCoordinate = textureCoordinate;
	}

	static VertexPositionBoneIndicesBoneWeightsNormalTexture()
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
			new VertexElement((short)0, (short)0, (VertexElementFormat)3, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)16, (VertexElementFormat)3, (VertexElementMethod)0, (VertexElementUsage)2, (byte)0),
			new VertexElement((short)0, (short)32, (VertexElementFormat)3, (VertexElementMethod)0, (VertexElementUsage)1, (byte)0),
			new VertexElement((short)0, (short)48, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)0),
			new VertexElement((short)0, (short)60, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0)
		};
	}
}
