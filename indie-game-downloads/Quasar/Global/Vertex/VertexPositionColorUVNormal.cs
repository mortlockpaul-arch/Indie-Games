using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexPositionColorUVNormal : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector3 Normal;

	public Vector2 UV;

	public Color Color;

	public static readonly VertexElement[] Elements = new VertexElement[4]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => Marshal.SizeOf(typeof(VertexPositionColorUVNormal));

	public static VertexDeclaration Declaration
	{
		get
		{
			if (declaration == null)
			{
				declaration = new VertexDeclaration(Elements);
			}
			return declaration;
		}
	}

	Vector3 IPositionVertex.Position => Position;

	VertexDeclaration IVertexType.VertexDeclaration => Declaration;

	public VertexPositionColorUVNormal(Vector3 position, Color color, Vector2 UV, Vector3 normal)
	{
		Position = position;
		Color = color;
		this.UV = UV;
		Normal = normal;
	}
}
