using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace EGEngine;

public struct VS_BakedLightStruct
{
	public Vector3 position;

	public NormalizedByte4 normal;

	public HalfVector2 textureCoord;

	public Color diffuse;

	public NormalizedByte4 tangent;
}
