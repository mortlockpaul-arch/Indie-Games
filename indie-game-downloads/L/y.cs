using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using p;
using r;
using y;

namespace l;

internal class y
{
	private _6 a5h;

	private b a5b;

	public _6 Data
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
			a5b.Data = a5h;
		}
	}

	public b Tree => a5b;

	public y(_6 data)
	{
		a5h = data;
		a5b = new b(data);
	}

	public bool RayCast(Ray ray, out int hitCount)
	{
		l._7<r._0006> rayHitList = p._6.GetRayHitList();
		bool result = RayCast(ray, rayHitList);
		hitCount = rayHitList.Count;
		p._6.GiveBack(rayHitList);
		return result;
	}

	public bool RayCast(Ray ray, out r._0006 rayHit)
	{
		return RayCast(ray, float.MaxValue, global::y._0006.DoubleSided, out rayHit);
	}

	public bool RayCast(Ray ray, global::y._0006 sidedness, out r._0006 rayHit)
	{
		return RayCast(ray, float.MaxValue, sidedness, out rayHit);
	}

	public bool RayCast(Ray ray, IList<r._0006> hits)
	{
		return RayCast(ray, float.MaxValue, global::y._0006.DoubleSided, hits);
	}

	public bool RayCast(Ray ray, global::y._0006 sidedness, IList<r._0006> hits)
	{
		return RayCast(ray, float.MaxValue, sidedness, hits);
	}

	public bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit)
	{
		return RayCast(ray, maximumLength, global::y._0006.DoubleSided, out rayHit);
	}

	public bool RayCast(Ray ray, float maximumLength, global::y._0006 sidedness, out r._0006 rayHit)
	{
		l._7<r._0006> rayHitList = p._6.GetRayHitList();
		bool flag = RayCast(ray, maximumLength, sidedness, rayHitList);
		if (flag)
		{
			rayHit = rayHitList[0];
			for (int i = 1; i < rayHitList.Count; i++)
			{
				r._0006 obj = rayHitList[i];
				if (obj.T < rayHit.T)
				{
					rayHit = obj;
				}
			}
		}
		else
		{
			rayHit = default(r._0006);
		}
		p._6.GiveBack(rayHitList);
		return flag;
	}

	public bool RayCast(Ray ray, float maximumLength, IList<r._0006> hits)
	{
		return RayCast(ray, maximumLength, global::y._0006.DoubleSided, hits);
	}

	public bool RayCast(Ray ray, float maximumLength, global::y._0006 sidedness, IList<r._0006> hits)
	{
		l._7<int> intList = p._6.GetIntList();
		a5b.GetOverlaps(ray, maximumLength, intList);
		for (int i = 0; i < intList.Count; i++)
		{
			a5h.GetTriangle(intList[i], out var v2, out var v3, out var v4);
			if (r.X.FindRayTriangleIntersection(ref ray, maximumLength, sidedness, ref v2, ref v3, ref v4, out var hit))
			{
				hits.Add(hit);
			}
		}
		p._6.GiveBack(intList);
		return hits.Count > 0;
	}

	public static void GetVerticesAndIndicesFromModel(Model collisionModel, out Vector3[] vertices, out int[] indices)
	{
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		Matrix[] array = new Matrix[collisionModel.Bones.Count];
		collisionModel.CopyAbsoluteBoneTransformsTo(array);
		foreach (ModelMesh mesh in collisionModel.Meshes)
		{
			Matrix transform = ((mesh.ParentBone == null) ? Matrix.Identity : array[mesh.ParentBone.Index]);
			AddMesh(mesh, transform, list, list2);
		}
		vertices = list.ToArray();
		indices = list2.ToArray();
	}

	public static void AddMesh(ModelMesh collisionModelMesh, Matrix transform, List<Vector3> vertices, IList<int> indices)
	{
		foreach (ModelMeshPart meshPart in collisionModelMesh.MeshParts)
		{
			int count = vertices.Count;
			Vector3[] array = new Vector3[meshPart.NumVertices];
			int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
			meshPart.VertexBuffer.GetData(meshPart.VertexOffset * vertexStride, array, 0, meshPart.NumVertices, vertexStride);
			Vector3.Transform(array, ref transform, array);
			vertices.AddRange(array);
			if (meshPart.IndexBuffer.IndexElementSize == IndexElementSize.ThirtyTwoBits)
			{
				int[] array2 = new int[meshPart.PrimitiveCount * 3];
				meshPart.IndexBuffer.GetData(meshPart.StartIndex * 4, array2, 0, meshPart.PrimitiveCount * 3);
				for (int i = 0; i < array2.Length; i++)
				{
					indices.Add(count + array2[i]);
				}
			}
			else
			{
				ushort[] array3 = new ushort[meshPart.PrimitiveCount * 3];
				meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, array3, 0, meshPart.PrimitiveCount * 3);
				for (int j = 0; j < array3.Length; j++)
				{
					indices.Add(count + array3[j]);
				}
			}
		}
	}
}
