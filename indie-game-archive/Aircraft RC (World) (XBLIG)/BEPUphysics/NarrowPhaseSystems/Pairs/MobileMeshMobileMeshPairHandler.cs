using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.Materials;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a mobile mesh-mobile mesh collision pair.
/// </summary>
public class MobileMeshMobileMeshPairHandler : MobileMeshMeshPairHandler
{
	private MobileMeshCollidable mesh;

	public override Collidable CollidableB => mesh;

	public override Entity EntityB => mesh.entity;

	protected override Material MaterialB => mesh.entity.material;

	protected override TriangleCollidable GetOpposingCollidable(int index)
	{
		TriangleCollidable triangleCollidable = Resources.GetTriangleCollidable();
		triangleCollidable.Shape.sidedness = mesh.Shape.Sidedness;
		triangleCollidable.Shape.collisionMargin = mobileMesh.Shape.MeshCollisionMargin;
		triangleCollidable.Entity = mesh.entity;
		return triangleCollidable;
	}

	protected override void CleanUpCollidable(TriangleCollidable collidable)
	{
		collidable.Entity = null;
		base.CleanUpCollidable(collidable);
	}

	protected override void ConfigureCollidable(TriangleEntry entry, float dt)
	{
		TriangleShape shape = entry.Collidable.Shape;
		mesh.Shape.TriangleMesh.Data.GetTriangle(entry.Index, out shape.vA, out shape.vB, out shape.vC);
		Matrix3X3.CreateFromQuaternion(ref mesh.worldTransform.Orientation, out var result);
		Matrix3X3.Transform(ref shape.vA, ref result, out shape.vA);
		Matrix3X3.Transform(ref shape.vB, ref result, out shape.vB);
		Matrix3X3.Transform(ref shape.vC, ref result, out shape.vC);
		Vector3.Add(ref shape.vA, ref shape.vB, out var result2);
		Vector3.Add(ref result2, ref shape.vC, out result2);
		Vector3.Multiply(ref result2, 1f / 3f, out result2);
		Vector3.Subtract(ref shape.vA, ref result2, out shape.vA);
		Vector3.Subtract(ref shape.vB, ref result2, out shape.vB);
		Vector3.Subtract(ref shape.vC, ref result2, out shape.vC);
		Vector3.Add(ref result2, ref mesh.worldTransform.Position, out result2);
		entry.Collidable.worldTransform.Position = result2;
		entry.Collidable.worldTransform.Orientation = Quaternion.Identity;
		entry.Collidable.UpdateBoundingBoxInternal(dt);
	}

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mesh = (MobileMeshCollidable)entryB;
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
		AffineTransform.CreateFromRigidTransform(ref mesh.worldTransform, out var affine);
		Vector3.Subtract(ref mobileMesh.entity.linearVelocity, ref mesh.entity.linearVelocity, out var result);
		Vector3.Multiply(ref result, dt, out result);
		mobileMesh.Shape.GetSweptLocalBoundingBox(ref mobileMesh.worldTransform, ref affine, ref result, out var boundingBox);
		mesh.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, intList);
		for (int i = 0; i < intList.count; i++)
		{
			TryToAdd(intList.Elements[i]);
		}
		Resources.GiveBack(intList);
	}
}
