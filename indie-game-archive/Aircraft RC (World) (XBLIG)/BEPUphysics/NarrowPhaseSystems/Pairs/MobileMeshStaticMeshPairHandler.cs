using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Materials;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a mobile mesh-static mesh collision pair.
/// </summary>
public class MobileMeshStaticMeshPairHandler : MobileMeshMeshPairHandler
{
	private StaticMesh mesh;

	public override Collidable CollidableB => mesh;

	public override Entity EntityB => null;

	protected override Material MaterialB => mesh.material;

	protected override TriangleCollidable GetOpposingCollidable(int index)
	{
		TriangleCollidable triangleCollidable = Resources.GetTriangleCollidable();
		TriangleShape shape = triangleCollidable.Shape;
		mesh.Mesh.Data.GetTriangle(index, out shape.vA, out shape.vB, out shape.vC);
		Vector3.Add(ref shape.vA, ref shape.vB, out var result);
		Vector3.Add(ref result, ref shape.vC, out result);
		Vector3.Multiply(ref result, 1f / 3f, out result);
		Vector3.Subtract(ref shape.vA, ref result, out shape.vA);
		Vector3.Subtract(ref shape.vB, ref result, out shape.vB);
		Vector3.Subtract(ref shape.vC, ref result, out shape.vC);
		triangleCollidable.worldTransform.Position = result;
		triangleCollidable.worldTransform.Orientation = Quaternion.Identity;
		triangleCollidable.UpdateBoundingBoxInternal(0f);
		shape.sidedness = mesh.sidedness;
		shape.collisionMargin = mobileMesh.Shape.MeshCollisionMargin;
		return triangleCollidable;
	}

	protected override void ConfigureCollidable(TriangleEntry entry, float dt)
	{
	}

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mesh = entryA as StaticMesh;
		if (mesh == null)
		{
			mesh = entryB as StaticMesh;
			if (mesh == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		mesh = null;
	}

	protected override void UpdateContainedPairs(float dt)
	{
		RawList<int> intList = Resources.GetIntList();
		mesh.Mesh.Tree.GetOverlaps(mobileMesh.boundingBox, intList);
		for (int i = 0; i < intList.count; i++)
		{
			TryToAdd(intList.Elements[i]);
		}
		Resources.GiveBack(intList);
	}
}
