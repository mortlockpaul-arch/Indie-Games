using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexPositionUV : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector2 UV;

	public static readonly VertexElement[] Elements = new VertexElement[2]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => Marshal.SizeOf(typeof(VertexPositionUV));

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

	public VertexPositionUV(Vector3 position, Vector2 UV)
	{
		Position = position;
		this.UV = UV;
	}
}
