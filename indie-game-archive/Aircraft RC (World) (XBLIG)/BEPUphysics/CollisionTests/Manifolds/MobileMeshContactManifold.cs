using System;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a convex and an instanced mesh.
/// </summary>
public abstract class MobileMeshContactManifold : TriangleMeshConvexContactManifold
{
	protected MobileMeshCollidable mesh;

	internal int parentContactCount;

	internal RawList<int> overlappedTriangles = new RawList<int>(4);

	private float previousDepth;

	private Vector3 lastValidConvexPosition;

	private UnsafeResourcePool<TriangleConvexPairTester> testerPool = new UnsafeResourcePool<TriangleConvexPairTester>();

	/// <summary>
	///  Gets the mesh of the pair.
	/// </summary>
	public MobileMeshCollidable Mesh => mesh;

	protected override RigidTransform MeshTransform => mesh.worldTransform;

	protected override bool UseImprovedBoundaryHandling => mesh.improveBoundaryBehavior;

	protected internal override int FindOverlappingTriangles(float dt)
	{
		AffineTransform spaceTransform = new AffineTransform(mesh.worldTransform.Orientation, mesh.worldTransform.Position);
		convex.Shape.GetLocalBoundingBox(ref convex.worldTransform, ref spaceTransform, out var boundingBox);
		Vector3 value = ((convex.entity == null) ? default(Vector3) : convex.entity.linearVelocity);
		if (mesh.entity != null)
		{
			Vector3.Subtract(ref value, ref mesh.entity.linearVelocity, out value);
		}
		Matrix3X3.TransformTranspose(ref value, ref spaceTransform.LinearTransform, out value);
		Vector3.Multiply(ref value, dt, out value);
		if (value.X > 0f)
		{
			boundingBox.Max.X += value.X;
		}
		else
		{
			boundingBox.Min.X += value.X;
		}
		if (value.Y > 0f)
		{
			boundingBox.Max.Y += value.Y;
		}
		else
		{
			boundingBox.Min.Y += value.Y;
		}
		if (value.Z > 0f)
		{
			boundingBox.Max.Z += value.Z;
		}
		else
		{
			boundingBox.Min.Z += value.Z;
		}
		mesh.Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, overlappedTriangles);
		return overlappedTriangles.count;
	}

	protected override bool ConfigureTriangle(int i, out TriangleIndices indices)
	{
		MeshBoundingBoxTreeData data = mesh.Shape.TriangleMesh.Data;
		int num = overlappedTriangles.Elements[i];
		data.GetTriangle(num, out localTriangleShape.vA, out localTriangleShape.vB, out localTriangleShape.vC);
		AffineTransform.CreateFromRigidTransform(ref mesh.worldTransform, out var affine);
		AffineTransform.Transform(ref localTriangleShape.vA, ref affine, out localTriangleShape.vA);
		AffineTransform.Transform(ref localTriangleShape.vB, ref affine, out localTriangleShape.vB);
		AffineTransform.Transform(ref localTriangleShape.vC, ref affine, out localTriangleShape.vC);
		Toolbox.GetTriangleBoundingBox(ref localTriangleShape.vA, ref localTriangleShape.vB, ref localTriangleShape.vC, out var aabb);
		aabb.Intersects(ref convex.boundingBox, out var result);
		if (!result)
		{
			indices = default(TriangleIndices);
			return false;
		}
		TriangleSidedness sidedness = mesh.Shape.solidity switch
		{
			MobileMeshSolidity.Clockwise => TriangleSidedness.Clockwise, 
			MobileMeshSolidity.Counterclockwise => TriangleSidedness.Counterclockwise, 
			MobileMeshSolidity.DoubleSided => TriangleSidedness.DoubleSided, 
			_ => mesh.Shape.solidSidedness, 
		};
		localTriangleShape.sidedness = sidedness;
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

	protected override void ProcessCandidates(RawValueList<ContactData> candidates)
	{
		if (candidates.count != 0 || parentContactCount != 0 || Mesh.Shape.solidity != MobileMeshSolidity.Solid)
		{
			return;
		}
		Matrix3X3.CreateFromQuaternion(ref mesh.worldTransform.Orientation, out var result);
		Ray ray = default(Ray);
		Vector3.Subtract(ref convex.worldTransform.Position, ref mesh.worldTransform.Position, out ray.Position);
		Matrix3X3.TransformTranspose(ref ray.Position, ref result, out ray.Position);
		Vector3.Subtract(ref lastValidConvexPosition, ref ray.Position, out ray.Direction);
		float num = ray.Direction.LengthSquared();
		if (num < 1E-07f)
		{
			ray.Direction = ray.Position;
			num = ray.Direction.LengthSquared();
			if (num < 1E-07f)
			{
				ray.Direction = Vector3.Up;
				num = 1f;
			}
		}
		Vector3.Divide(ref ray.Direction, (float)Math.Sqrt(num), out ray.Direction);
		if (mesh.Shape.IsLocalRayOriginInMesh(ref ray, out var hit))
		{
			ContactData contactCandidate = new ContactData
			{
				Id = 2
			};
			Matrix3X3.Transform(ref ray.Position, ref result, out contactCandidate.Position);
			Vector3.Add(ref contactCandidate.Position, ref mesh.worldTransform.Position, out contactCandidate.Position);
			contactCandidate.Normal = hit.Normal;
			contactCandidate.Normal.Normalize();
			Vector3.Dot(ref ray.Direction, ref contactCandidate.Normal, out var result2);
			contactCandidate.PenetrationDepth = (0f - result2) * hit.T + convex.Shape.minimumRadius;
			Matrix3X3.Transform(ref contactCandidate.Normal, ref result, out contactCandidate.Normal);
			bool flag = true;
			for (int i = 0; i < contacts.count; i++)
			{
				if (contacts.Elements[i].Id == 2)
				{
					contacts.Elements[i].Position = contactCandidate.Position;
					contacts.Elements[i].Normal = contactCandidate.Normal;
					contacts.Elements[i].PenetrationDepth = contactCandidate.PenetrationDepth;
					supplementData.Elements[i].BasePenetrationDepth = contactCandidate.PenetrationDepth;
					supplementData.Elements[i].LocalOffsetA = default(Vector3);
					supplementData.Elements[i].LocalOffsetB = ray.Position;
					flag = false;
					break;
				}
			}
			if (flag && contacts.count == 0)
			{
				Add(ref contactCandidate);
			}
			previousDepth = contactCandidate.PenetrationDepth;
		}
		else
		{
			if (previousDepth > 0f)
			{
				lastValidConvexPosition = ray.Position;
			}
			previousDepth = 0f;
		}
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		mesh = null;
		convex = null;
		parentContactCount = 0;
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
		mesh = newCollidableB as MobileMeshCollidable;
		if (convex == null || mesh == null)
		{
			convex = newCollidableB as ConvexCollidable;
			mesh = newCollidableA as MobileMeshCollidable;
			if (convex == null || mesh == null)
			{
				throw new Exception("Inappropriate types used to initialize contact manifold.");
			}
		}
	}

	protected override void GiveBackTester(TrianglePairTester tester)
	{
		testerPool.GiveBack((TriangleConvexPairTester)tester);
	}

	protected override TrianglePairTester GetTester()
	{
		return testerPool.Take();
	}
}
