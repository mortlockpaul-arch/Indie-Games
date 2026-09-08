using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;

namespace Quasar.Meshes;

public class PostprocessMesh : UserVertexMesh<VertexPositionUV>
{
	private void initializeBuffer()
	{
		initMesh(4);
		verticesBuffer[0].Position = new Vector3(0f, 0f, 0f);
		verticesBuffer[0].UV = new Vector2(0f, 1f);
		verticesBuffer[1].Position = new Vector3(0f, 1f, 0f);
		verticesBuffer[1].UV = new Vector2(0f, 0f);
		verticesBuffer[2].Position = new Vector3(1f, 0f, 0f);
		verticesBuffer[2].UV = new Vector2(1f, 1f);
		verticesBuffer[3].Position = new Vector3(1f, 1f, 0f);
		verticesBuffer[3].UV = new Vector2(1f, 0f);
		base.PrimitiveCount = 2;
		base.PrimitiveType = PrimitiveType.TriangleStrip;
		updateBoundingSphere(4);
	}

	public PostprocessMesh(Texture source)
	{
		initializeBuffer();
		Material item = new Material
		{
			Texture = source
		};
		materials.Add(item);
	}
}
