using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public struct VERT_PARTICLE : IVertexType
{
	public Vector3 pos;

	public Color color0;

	public Vector3 tex;

	public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0));

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	public Vector3 Position
	{
		get
		{
			return pos;
		}
		set
		{
			pos = value;
		}
	}

	public float PositionY
	{
		get
		{
			return pos.Y;
		}
		set
		{
			pos.Y = value;
		}
	}

	public byte ColorAlpha
	{
		get
		{
			return color0.A;
		}
		set
		{
			color0.A = value;
		}
	}

	public Color vertColor
	{
		get
		{
			return color0;
		}
		set
		{
			color0 = value;
		}
	}

	public Vector2 TexCoord
	{
		set
		{
			tex.X = value.X;
			tex.Y = value.Y;
		}
	}

	public float Softness
	{
		set
		{
			tex.Z = value;
		}
	}
}
