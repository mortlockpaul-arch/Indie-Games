using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexPositionTexture : IVertexType
{
	public Vector3 Position;

	public Vector2 TextureCoordinate;

	public static readonly VertexDeclaration VertexDeclaration;

	VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

	static VertexPositionTexture()
	{
		VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0))
		{
			Name = "VertexPositionTexture.VertexDeclaration"
		};
	}

	public VertexPositionTexture(Vector3 position, Vector2 textureCoordinate)
	{
		Position = position;
		TextureCoordinate = textureCoordinate;
	}

	public override int GetHashCode()
	{
		return Position.X.GetHashCode() ^ Position.Y.GetHashCode() ^ Position.Z.GetHashCode() ^ TextureCoordinate.X.GetHashCode() ^ TextureCoordinate.Y.GetHashCode();
	}

	public override string ToString()
	{
		return "{{Position:" + Position.ToString() + " TextureCoordinate:" + TextureCoordinate.ToString() + "}}";
	}

	public static bool operator ==(VertexPositionTexture left, VertexPositionTexture right)
	{
		return left.Position == right.Position && left.TextureCoordinate == right.TextureCoordinate;
	}

	public static bool operator !=(VertexPositionTexture left, VertexPositionTexture right)
	{
		return !(left == right);
	}

	public override bool Equals(object obj)
	{
		return obj is VertexPositionTexture && this == (VertexPositionTexture)obj;
	}
}
