using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexUVOnly : IVertexType
{
	public Vector2 UV;

	public static VertexElement[] Elements = new VertexElement[1]
	{
		new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => Marshal.SizeOf(typeof(VertexUVOnly));

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

	VertexDeclaration IVertexType.VertexDeclaration => Declaration;

	public VertexUVOnly(Vector2 uv)
	{
		UV = uv;
	}
}
