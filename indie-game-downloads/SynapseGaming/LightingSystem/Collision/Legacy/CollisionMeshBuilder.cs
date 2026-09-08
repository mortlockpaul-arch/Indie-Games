using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Collision.Legacy;

/// <summary>
/// Provides methods for generating collision meshes and mesh
/// data from renderable meshes. Also caches the generated
/// data for later requests.
/// </summary>
public static class CollisionMeshBuilder
{
	private static List<ICollisionMaterial> a5h = new List<ICollisionMaterial>(256);

	private static SystemStatistic a5b = SystemConsole.GetStatistic("Collision_BuildWorldMesh", SystemStatisticCategory.Collision);

	/// <summary>
	/// Calculates world-space collision mesh geometry from the
	/// mesh-space collision data in the provided renderable meshes.
	/// </summary>
	/// <param name="meshes">Renderable meshes to generate the collision mesh geometry from.</param>
	/// <param name="worldmesh">Collision mesh to fill with world-space geometry.</param>
	public static void BuildWorldMesh(RenderableMeshCollection meshes, CollisionMesh worldmesh)
	{
		a5b.AccumulationValue++;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < meshes.Count; i++)
		{
			GeometryData meshData = GeometryExtractionHelper.GetMeshData(meshes[i]);
			num += meshData.Vertices.Count;
			num2 += meshData.Indices.Count;
		}
		int num3 = num2 / 3;
		if (worldmesh.Vertices == null || worldmesh.Vertices.Length < num)
		{
			worldmesh.Vertices = new Vector3[num];
		}
		if (worldmesh.Indices == null || worldmesh.Indices.Length < num2)
		{
			worldmesh.Indices = new int[num2];
		}
		a5h.Clear();
		num = 0;
		num2 = 0;
		for (int j = 0; j < meshes.Count; j++)
		{
			RenderableMesh renderableMesh = meshes[j];
			GeometryData meshData2 = GeometryExtractionHelper.GetMeshData(renderableMesh);
			ICollisionMaterial item = renderableMesh.Effect as ICollisionMaterial;
			Matrix world = renderableMesh.World;
			int num4 = num;
			int num5 = meshData2.Indices.Count / 3;
			bool flag = world.Determinant() < 0f;
			for (int k = 0; k < meshData2.Vertices.Count; k++)
			{
				ref Vector3 reference = ref worldmesh.Vertices[num++];
				reference = Vector3.Transform(meshData2.Vertices[k], world);
			}
			for (int l = 0; l < num5; l++)
			{
				int num6 = l * 3;
				if (flag)
				{
					worldmesh.Indices[num2++] = meshData2.Indices[num6] + num4;
					worldmesh.Indices[num2++] = meshData2.Indices[num6 + 2] + num4;
					worldmesh.Indices[num2++] = meshData2.Indices[num6 + 1] + num4;
				}
				else
				{
					worldmesh.Indices[num2++] = meshData2.Indices[num6] + num4;
					worldmesh.Indices[num2++] = meshData2.Indices[num6 + 1] + num4;
					worldmesh.Indices[num2++] = meshData2.Indices[num6 + 2] + num4;
				}
				a5h.Add(item);
			}
		}
		worldmesh.Surfaces.Clear();
		int num7 = 0;
		for (int m = 0; m < num3; m++)
		{
			int num8 = worldmesh.Indices[num7++];
			int num9 = worldmesh.Indices[num7++];
			int num10 = worldmesh.Indices[num7++];
			Vector3 vector = worldmesh.Vertices[num8];
			Vector3 vector2 = worldmesh.Vertices[num10];
			Vector3 vector3 = worldmesh.Vertices[num9];
			CollisionMesh.CollisionSurface collisionSurface = new CollisionMesh.CollisionSurface();
			collisionSurface.VertexIndex0 = num8;
			collisionSurface.VertexIndex1 = num10;
			collisionSurface.VertexIndex2 = num9;
			Plane surface = new Plane(vector, vector2, vector3);
			Vector3 point = vector - surface.Normal;
			Vector3 point2 = vector2 - surface.Normal;
			collisionSurface.Surface = surface;
			collisionSurface.Edge0 = new Plane(vector, vector2, point);
			collisionSurface.Edge1 = new Plane(vector2, vector3, point2);
			collisionSurface.Edge2 = new Plane(vector3, vector, point);
			collisionSurface.Material = a5h[m];
			worldmesh.Surfaces.Add(collisionSurface);
		}
		worldmesh.bD();
	}
}
