using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Quasar.Global.Utils;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;

namespace Quasar.Meshes;

public class Polygon2DMesh : VertexMesh<VertexPosition>
{
	private void initializeBuffer(ReadOnlyCollection<Vector2> points)
	{
		List<Vector2> list = Triangulate.Process(points);
		initMesh(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			verticesBuffer[i].Position = new Vector3(list[i], 0f);
		}
		updateMesh();
		base.PrimitiveCount = list.Count / 3;
	}

	public Polygon2DMesh(ReadOnlyCollection<Vector2> points)
	{
		initializeBuffer(points);
		Material item = new Material();
		materials.Add(item);
	}
}
