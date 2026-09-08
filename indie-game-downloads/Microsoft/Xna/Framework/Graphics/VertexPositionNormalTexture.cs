using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexPositionNormalTexture : IVertexType
{
	public Vector3 Position;

	public Vector3 Normal;

	public Vector2 TextureCoordinate;

	public static readonly VertexDeclaration VertexDeclaration;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	static VertexPositionNormalTexture()
	{
		VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0))
		{
			Name = "VertexPositionNormalTexture.VertexDeclaration"
		};
	}

	public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate)
	{
		Position = position;
		Normal = normal;
		TextureCoordinate = textureCoordinate;
	}

	public override int GetHashCode()
	{
		return Position.X.GetHashCode() ^ Position.Y.GetHashCode() ^ Position.Z.GetHashCode() ^ Normal.X.GetHashCode() ^ Normal.Y.GetHashCode() ^ Normal.Z.GetHashCode() ^ TextureCoordinate.X.GetHashCode() ^ TextureCoordinate.Y.GetHashCode();
	}

	public override string ToString()
	{
		return "{{Position:" + Position.ToString() + " Normal:" + Normal.ToString() + " TextureCoordinate:" + TextureCoordinate.ToString() + "}}";
	}

	public static bool operator ==(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
	{
		return left.Position == right.Position && left.Normal == right.Normal && left.TextureCoordinate == right.TextureCoordinate;
	}

	public static bool operator !=(VertexPositionNormalTexture left, VertexPositionNormalTexture right)
	{
		return !(left == right);
	}

	public override bool Equals(object obj)
	{
		return obj is VertexPositionNormalTexture && this == (VertexPositionNormalTexture)obj;
	}
}
