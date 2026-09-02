using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RacingGame.Graphics;

public struct TangentVertex
{
	public Vector3 pos;

	public Vector2 uv;

	public Vector3 normal;

	public Vector3 tangent;

	private static readonly VertexElement[] VertexElements;

	public static VertexDeclaration VertexDeclaration;

	public static int SizeInBytes => 44;

	public float U => uv.X;

	public float V => uv.Y;

	public TangentVertex(Vector3 setPos, float setU, float setV, Vector3 setNormal, Vector3 setTangent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		pos = setPos;
		uv = new Vector2(setU, setV);
		normal = setNormal;
		tangent = setTangent;
	}

	public TangentVertex(Vector3 setPos, Vector2 setUv, Vector3 setNormal, Vector3 setTangent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		pos = setPos;
		uv = setUv;
		normal = setNormal;
		tangent = setTangent;
	}

	public override string ToString()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("TangentVertex(pos=", pos, ", u=", uv.X, ", v=", uv.Y, ", normal=", normal, ", tangent=", tangent, ")");
	}

	private static VertexElement[] GenerateVertexElements()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		return (VertexElement[])(object)new VertexElement[4]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0),
			new VertexElement((short)0, (short)20, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)0),
			new VertexElement((short)0, (short)32, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)6, (byte)0)
		};
	}

	public static bool IsTangentVertexDeclaration(VertexElement[] declaration)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		if (declaration == null)
		{
			throw new ArgumentNullException("declaration");
		}
		if (declaration.Length == 4 && (int)((VertexElement)(ref declaration[0])).VertexElementUsage == 0 && (int)((VertexElement)(ref declaration[1])).VertexElementUsage == 5 && (int)((VertexElement)(ref declaration[2])).VertexElementUsage == 3)
		{
			return (int)((VertexElement)(ref declaration[3])).VertexElementUsage == 6;
		}
		return false;
	}

	static TangentVertex()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		VertexElements = GenerateVertexElements();
		VertexDeclaration = new VertexDeclaration(BaseGame.Device, VertexElements);
	}
}
