using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a convex and an instanced mesh.
/// </summary>
public abstract class InstancedMeshContactManifold : TriangleMeshConvexContactManifold
{
	protected InstancedMesh mesh;

	internal RawList<int> overlappedTriangles = new RawList<int>(4);

	/// <summary>
	///  Gets the mesh of the pair.
	/// </summary>
	public InstancedMesh Mesh => mesh;

	protected override bool UseImprovedBoundaryHandling => mesh.improveBoundaryBehavior;

	protected internal override int FindOverlappingTriangles(float dt)
	{
		convex.Shape.GetLocalBoundingBox(ref convex.worldTransform, ref mesh.worldTransform, out var boundingBox);
		if (convex.entity != null)
		{
			Matrix3X3.Invert(ref mesh.worldTransform.LinearTransform, out var result);
			Matrix3X3.Transform(ref convex.entity.linearVelocity, ref result, out var result2);
			Vector3.Multiply(ref result2, dt, out result2);
			if (result2.X > 0f)
			{
				boundingBox.Max.X += result2.X;
			}
			else
			{
				boundingBox.Min.X += result2.X;
			}
			if (result2.Y > 0f)
			{
				boundingBox.Max.Y += result2.Y;
			}
			else
			{
				boundingBox.Min.Y += result2.Y;
			}
			if (result2.Z > 0f)
			{
				boundingBox.Max.Z += result2.Z;
			}
			else
			{
				boundingBox.Min.Z += result2.Z;
			}
		}
		mesh.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, overlappedTriangles);
		return overlappedTriangles.count;
	}

	protected override bool ConfigureTriangle(int i, out TriangleIndices indices)
	{
		MeshBoundingBoxTreeData data = mesh.Shape.TriangleMesh.Data;
		int num = overlappedTriangles.Elements[i];
		data.GetTriangle(num, out localTriangleShape.vA, out localTriangleShape.vB, out localTriangleShape.vC);
		AffineTransform.Transform(ref localTriangleShape.vA, ref mesh.worldTransform, out localTriangleShape.vA);
		AffineTransform.Transform(ref localTriangleShape.vB, ref mesh.worldTransform, out localTriangleShape.vB);
		AffineTransform.Transform(ref localTriangleShape.vC, ref mesh.worldTransform, out localTriangleShape.vC);
		Toolbox.GetTriangleBoundingBox(ref localTriangleShape.vA, ref localTriangleShape.vB, ref localTriangleShape.vC, out var aabb);
		aabb.Intersects(ref convex.boundingBox, out var result);
		if (!result)
		{
			indices = default(TriangleIndices);
			return false;
		}
		localTriangleShape.sidedness = mesh.sidedness;
		localTriangleShape.collisionMargin = 0f;
		indices = new TriangleIndices
		{
			A = data.indices[num],
			B = data.indices[num + 1],
			C = data.indices[num + 2]
		};
		return true;
	}

	protected internal override void CleanUpOverlappingTriangles()
	{
		overlappedTriangles.Clear();
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		mesh = null;
		convex = null;
		base.CleanUp();
	}

	/// <summary>
	///  Initializes the manifold.
	/// </summary>
	/// <param name="newCollidableA">First collidable.</param>
	/// <param name="newCollidableB">Second collidable.</param>
	public override void Initialize(Collidable newCollidableA, Collidable newCollidableB)
	{
		convex = newCollidableA as ConvexCollidable;
		mesh = newCollidableB as InstancedMesh;
		if (convex == null || mesh == null)
		{
			convex = newCollidableB as ConvexCollidable;
			mesh = newCollidableA as InstancedMesh;
			if (convex == null || mesh == null)
			{
				throw new Exception("Inappropriate types used to initialize contact manifold.");
			}
		}
	}
}
