using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexPosition : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public static readonly VertexElement[] Elements = new VertexElement[1]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => Marshal.SizeOf(typeof(VertexPosition));

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

	public VertexPosition(Vector3 position)
	{
		Position = position;
	}
}
