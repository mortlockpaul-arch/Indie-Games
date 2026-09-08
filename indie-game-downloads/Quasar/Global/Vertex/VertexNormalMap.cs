using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Global.Vertex;

public struct VertexNormalMap : IPositionVertex, IVertexType
{
	public Vector3 Position;

	public Vector3 Normal;

	public Vector2 UV;

	public Vector3 Tangent;

	public Vector3 Binormal;

	public static VertexElement[] Elements = new VertexElement[5]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
		new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
	};

	private static VertexDeclaration declaration = null;

	public static int SizeBytes => Marshal.SizeOf(typeof(VertexNormalMap));

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

	public void Normalize()
	{
		Normal.Normalize();
		Tangent.Normalize();
		Binormal.Normalize();
	}

	public VertexNormalMap(Vector3 position, Vector2 UV, Vector3 normal, Vector3 tangent, Vector3 binormal)
	{
		Position = position;
		this.UV = UV;
		Normal = normal;
		Tangent = tangent;
		Binormal = binormal;
	}
}
