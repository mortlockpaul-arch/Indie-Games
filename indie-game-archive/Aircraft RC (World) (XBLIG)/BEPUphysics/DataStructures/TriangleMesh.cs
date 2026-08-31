using System.Collections.Generic;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BEPUphysics.DataStructures;

/// <summary>
///  Data structure containing triangle mesh data and its associated bounding box tree.
/// </summary>
public class TriangleMesh
{
	private MeshBoundingBoxTreeData data;

	private MeshBoundingBoxTree tree;

	/// <summary>
	///  Gets or sets the bounding box data used in the mesh.
	/// </summary>
	public MeshBoundingBoxTreeData Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
			tree.Data = data;
		}
	}

	/// <summary>
	///  Gets the bounding box tree that accelerates queries to this triangle mesh.
	/// </summary>
	public MeshBoundingBoxTree Tree => tree;

	/// <summary>
	///  Constructs a new triangle mesh.
	/// </summary>
	/// <param name="data">Data to use to construct the mesh.</param>
	public TriangleMesh(MeshBoundingBoxTreeData data)
	{
		this.data = data;
		tree = new MeshBoundingBoxTree(data);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	/// <param name="hitCount">Number of hits between the ray and the mesh.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, out int hitCount)
	{
		RawList<RayHit> rayHitList = Resources.GetRayHitList();
		bool result = RayCast(ray, rayHitList);
		hitCount = rayHitList.Count;
		Resources.GiveBack(rayHitList);
		return result;
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	/// <param name="rayHit">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, out RayHit rayHit)
	{
		return RayCast(ray, float.MaxValue, TriangleSidedness.DoubleSided, out rayHit);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
	/// <param name="rayHit">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, TriangleSidedness sidedness, out RayHit rayHit)
	{
		return RayCast(ray, float.MaxValue, sidedness, out rayHit);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	/// <param name="hits">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, IList<RayHit> hits)
	{
		return RayCast(ray, float.MaxValue, TriangleSidedness.DoubleSided, hits);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
	/// <param name="hits">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, TriangleSidedness sidedness, IList<RayHit> hits)
	{
		return RayCast(ray, float.MaxValue, sidedness, hits);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="rayHit">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
	{
		return RayCast(ray, maximumLength, TriangleSidedness.DoubleSided, out rayHit);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	///  <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
	/// <param name="rayHit">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
	{
		RawList<RayHit> rayHitList = Resources.GetRayHitList();
		bool flag = RayCast(ray, maximumLength, sidedness, rayHitList);
		if (flag)
		{
			rayHit = rayHitList[0];
			for (int i = 1; i < rayHitList.Count; i++)
			{
				RayHit rayHit2 = rayHitList[i];
				if (rayHit2.T < rayHit.T)
				{
					rayHit = rayHit2;
				}
			}
		}
		else
		{
			rayHit = default(RayHit);
		}
		Resources.GiveBack(rayHitList);
		return flag;
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="hits">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, IList<RayHit> hits)
	{
		return RayCast(ray, maximumLength, TriangleSidedness.DoubleSided, hits);
	}

	/// <summary>
	///  Tests a ray against the triangle mesh.
	/// </summary>
	/// <param name="ray">Ray to test against the mesh.</param>
	///  <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	///  <param name="sidedness">Sidedness to apply to the mesh for the ray cast.</param>
	/// <param name="hits">Hit data for the ray, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, IList<RayHit> hits)
	{
		RawList<int> intList = Resources.GetIntList();
		tree.GetOverlaps(ray, maximumLength, intList);
		for (int i = 0; i < intList.Count; i++)
		{
			data.GetTriangle(intList[i], out var v, out var v2, out var v3);
			if (Toolbox.FindRayTriangleIntersection(ref ray, maximumLength, sidedness, ref v, ref v2, ref v3, out var hit))
			{
				hits.Add(hit);
			}
		}
		Resources.GiveBack(intList);
		return hits.Count > 0;
	}

	/// <summary>
	/// Gets an array of vertices and indices from the provided model.
	/// </summary>
	/// <param name="collisionModel">Model to use for the collision shape.</param>
	/// <param name="vertices">Compiled set of vertices from the model.</param>
	/// <param name="indices">Compiled set of indices from the model.</param>
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

	/// <summary>
	/// Adds a mesh's vertices and indices to the given lists.
	/// </summary>
	/// <param name="collisionModelMesh">Model to use for the collision shape.</param>
	/// <param name="transform">Transform to apply to the mesh.</param>
	/// <param name="vertices">List to receive vertices from the mesh.</param>
	/// <param name="indices">List to receive indices from the mesh.</param>
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
