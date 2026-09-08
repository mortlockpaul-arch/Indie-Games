using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using l;

namespace z;

internal class X : z._0006<l.y>
{
	public X(b drawer, l.y displayedObject)
		: base(drawer, displayedObject)
	{
	}

	public override int GetTriangleCountEstimate()
	{
		return base.DisplayedObject.Data.Indices.Length / 3;
	}

	public static void GetMeshData(global::l.y mesh, List<VertexPositionNormalTexture> vertices, List<uint> indices)
	{
		VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[mesh.Data.Vertices.Length];
		for (int i = 0; i < mesh.Data.Vertices.Length; i++)
		{
			mesh.Data.GetVertexPosition(i, out var vertex);
			ref VertexPositionNormalTexture reference = ref array[i];
			reference = new VertexPositionNormalTexture(vertex, Vector3.Zero, Vector2.Zero);
		}
		for (int j = 0; j < mesh.Data.Indices.Length; j++)
		{
			indices.Add((uint)mesh.Data.Indices[j]);
		}
		for (int k = 0; k < indices.Count; k += 3)
		{
			uint num = indices[k];
			uint num2 = indices[k + 1];
			uint num3 = indices[k + 2];
			Vector3 vector = Vector3.Normalize(Vector3.Cross(array[num3].Position - array[num].Position, array[num2].Position - array[num].Position));
			array[num].Normal += vector;
			array[num2].Normal += vector;
			array[num3].Normal += vector;
		}
		for (int l = 0; l < array.Length; l++)
		{
			array[l].Normal.Normalize();
			vertices.Add(array[l]);
		}
	}

	public override void GetMeshData(List<VertexPositionNormalTexture> vertices, List<uint> indices)
	{
		GetMeshData(base.DisplayedObject, vertices, indices);
	}

	public override void Update()
	{
		base.WorldTransform = Matrix.Identity;
	}
}
