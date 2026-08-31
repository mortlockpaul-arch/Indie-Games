using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a Terrain and a convex.
/// </summary>
public abstract class TerrainContactManifold : TriangleMeshConvexContactManifold
{
	protected Terrain terrain;

	internal RawList<TriangleIndices> overlappedTriangles = new RawList<TriangleIndices>(4);

	/// <summary>
	///  Gets the terrain associated with this pair.
	/// </summary>
	public Terrain Terrain => terrain;

	protected override bool UseImprovedBoundaryHandling => terrain.improveBoundaryBehavior;

	protected internal override int FindOverlappingTriangles(float dt)
	{
		convex.Shape.GetLocalBoundingBox(ref convex.worldTransform, ref terrain.worldTransform, out var boundingBox);
		if (convex.entity != null)
		{
			Matrix3X3.Invert(ref terrain.worldTransform.LinearTransform, out var result);
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
		terrain.Shape.GetOverlaps(boundingBox, overlappedTriangles);
		return overlappedTriangles.count;
	}

	protected override bool ConfigureTriangle(int i, out TriangleIndices indices)
	{
		indices = overlappedTriangles.Elements[i];
		terrain.Shape.GetTriangle(ref indices, ref terrain.worldTransform, out localTriangleShape.vA, out localTriangleShape.vB, out localTriangleShape.vC);
		localTriangleShape.collisionMargin = 0f;
		Vector3.Subtract(ref localTriangleShape.vB, ref localTriangleShape.vA, out var result);
		Vector3.Subtract(ref localTriangleShape.vC, ref localTriangleShape.vA, out var result2);
		Vector3.Cross(ref result, ref result2, out var result3);
		Vector3 vector = new Vector3(terrain.worldTransform.LinearTransform.M21, terrain.worldTransform.LinearTransform.M22, terrain.worldTransform.LinearTransform.M23);
		Vector3.Dot(ref vector, ref result3, out var result4);
		if (result4 > 0f)
		{
			localTriangleShape.sidedness = TriangleSidedness.Clockwise;
		}
		else
		{
			localTriangleShape.sidedness = TriangleSidedness.Counterclockwise;
		}
		return true;
	}

	protected internal override void CleanUpOverlappingTriangles()
	{
		overlappedTriangles.Clear();
	}

	protected override void ProcessCandidates(RawValueList<ContactData> candidates)
	{
		if (!((candidates.count == 0) & (terrain.thickness > 0f)))
		{
			return;
		}
		Ray ray = new Ray
		{
			Position = convex.worldTransform.Position,
			Direction = terrain.worldTransform.LinearTransform.Up
		};
		ray.Direction.Normalize();
		if (!terrain.Shape.RayCast(ref ray, terrain.thickness, ref terrain.worldTransform, TriangleSidedness.DoubleSided, out var hit))
		{
			return;
		}
		hit.Normal.Normalize();
		Vector3.Dot(ref ray.Direction, ref hit.Normal, out var result);
		ContactData item = new ContactData
		{
			Normal = hit.Normal,
			Position = convex.worldTransform.Position,
			Id = 2,
			PenetrationDepth = (0f - hit.T) * result + convex.Shape.minimumRadius
		};
		bool flag = false;
		for (int i = 0; i < contacts.count; i++)
		{
			if (contacts.Elements[i].Id == 2)
			{
				contacts.Elements[i].Normal = item.Normal;
				contacts.Elements[i].Position = item.Position;
				contacts.Elements[i].PenetrationDepth = item.PenetrationDepth;
				supplementData.Elements[i].BasePenetrationDepth = item.PenetrationDepth;
				supplementData.Elements[i].LocalOffsetA = default(Vector3);
				supplementData.Elements[i].LocalOffsetB = ray.Position;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			candidates.Add(ref item);
		}
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		terrain = null;
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
		terrain = newCollidableB as Terrain;
		if (convex == null || terrain == null)
		{
			convex = newCollidableB as ConvexCollidable;
			terrain = newCollidableA as Terrain;
			if (convex == null || terrain == null)
			{
				throw new Exception("Inappropriate types used to initialize contact manifold.");
			}
		}
	}
}
