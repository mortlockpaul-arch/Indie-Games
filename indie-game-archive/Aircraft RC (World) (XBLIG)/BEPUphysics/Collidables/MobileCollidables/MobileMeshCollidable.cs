using System;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Collidable used by compound shapes.
/// </summary>
public class MobileMeshCollidable : EntityCollidable
{
	internal bool improveBoundaryBehavior = true;

	/// <summary>
	///  Gets the shape of the collidable.
	/// </summary>
	public new MobileMeshShape Shape => (MobileMeshShape)shape;

	/// <summary>
	/// Gets or sets whether or not the collision system should attempt to improve contact behavior at the boundaries between triangles.
	/// This has a slight performance cost, but prevents objects sliding across a triangle boundary from 'bumping,' and otherwise improves
	/// the robustness of contacts at edges and vertices.
	/// </summary>
	public bool ImproveBoundaryBehavior
	{
		get
		{
			return improveBoundaryBehavior;
		}
		set
		{
			improveBoundaryBehavior = value;
		}
	}

	/// <summary>
	/// Constructs a new mobile mesh collidable.
	/// </summary>
	/// <param name="shape">Shape to use in the collidable.</param>
	public MobileMeshCollidable(MobileMeshShape shape)
		: base(shape)
	{
		base.Events = new ContactEventManager<EntityCollidable>();
	}

	protected internal override void UpdateBoundingBoxInternal(float dt)
	{
		Shape.GetBoundingBox(ref worldTransform, out boundingBox);
		ExpandBoundingBox(ref boundingBox, dt);
	}

	/// <summary>
	/// Tests a ray against the entry.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit location of the ray on the entry, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public override bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
	{
		Matrix3X3.CreateFromQuaternion(ref worldTransform.Orientation, out var result);
		Ray ray2 = default(Ray);
		Matrix3X3.TransformTranspose(ref ray.Direction, ref result, out ray2.Direction);
		Vector3.Subtract(ref ray.Position, ref worldTransform.Position, out ray2.Position);
		Matrix3X3.TransformTranspose(ref ray2.Position, ref result, out ray2.Position);
		if (Shape.solidity == MobileMeshSolidity.Solid)
		{
			if (Shape.IsLocalRayOriginInMesh(ref ray2, out rayHit))
			{
				rayHit = new RayHit
				{
					Location = ray.Position,
					Normal = Vector3.Zero,
					T = 0f
				};
				return true;
			}
			if (rayHit.T < maximumLength)
			{
				Vector3.Multiply(ref ray.Direction, rayHit.T, out rayHit.Location);
				Vector3.Add(ref rayHit.Location, ref ray.Position, out rayHit.Location);
				Matrix3X3.Transform(ref rayHit.Normal, ref result, out rayHit.Normal);
				return true;
			}
			return false;
		}
		TriangleSidedness sidedness = Shape.solidity switch
		{
			MobileMeshSolidity.Clockwise => TriangleSidedness.Clockwise, 
			MobileMeshSolidity.Counterclockwise => TriangleSidedness.Counterclockwise, 
			_ => TriangleSidedness.DoubleSided, 
		};
		if (Shape.TriangleMesh.RayCast(ray2, maximumLength, sidedness, out rayHit))
		{
			Vector3.Multiply(ref ray.Direction, rayHit.T, out rayHit.Location);
			Vector3.Add(ref rayHit.Location, ref ray.Position, out rayHit.Location);
			Matrix3X3.Transform(ref rayHit.Normal, ref result, out rayHit.Normal);
			return true;
		}
		rayHit = default(RayHit);
		return false;
	}

	/// <summary>
	///  Tests a ray against the surface of the mesh.  This does not take into account solidity.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray to test; in units of the ray's direction's length.</param>
	/// <param name="sidedness">Sidedness to use during the ray cast.  This does not have to be the same as the mesh's sidedness.</param>
	/// <param name="rayHit">The hit location of the ray on the mesh, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
	{
		Matrix3X3.CreateFromQuaternion(ref worldTransform.Orientation, out var result);
		Ray ray2 = default(Ray);
		Matrix3X3.TransformTranspose(ref ray.Direction, ref result, out ray2.Direction);
		Vector3.Subtract(ref ray.Position, ref worldTransform.Position, out ray2.Position);
		Matrix3X3.TransformTranspose(ref ray2.Position, ref result, out ray2.Position);
		if (Shape.TriangleMesh.RayCast(ray2, maximumLength, sidedness, out rayHit))
		{
			Vector3.Multiply(ref ray.Direction, rayHit.T, out rayHit.Location);
			Vector3.Add(ref rayHit.Location, ref ray.Position, out rayHit.Location);
			Matrix3X3.Transform(ref rayHit.Normal, ref result, out rayHit.Normal);
			return true;
		}
		rayHit = default(RayHit);
		return false;
	}

	/// <summary>
	/// Casts a convex shape against the collidable.
	/// </summary>
	/// <param name="castShape">Shape to cast.</param>
	/// <param name="startingTransform">Initial transform of the shape.</param>
	/// <param name="sweep">Sweep to apply to the shape.</param>
	/// <param name="hit">Hit data, if any.</param>
	/// <returns>Whether or not the cast hit anything.</returns>
	public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit)
	{
		if (Shape.solidity == MobileMeshSolidity.Solid)
		{
			Ray ray = new Ray
			{
				Position = startingTransform.Position,
				Direction = Toolbox.UpVector
			};
			if (Shape.IsLocalRayOriginInMesh(ref ray, out hit))
			{
				hit = new RayHit
				{
					Location = startingTransform.Position,
					Normal = default(Vector3),
					T = 0f
				};
				return true;
			}
		}
		hit = default(RayHit);
		AffineTransform spaceTransform = new AffineTransform
		{
			Translation = worldTransform.Position
		};
		Matrix3X3.CreateFromQuaternion(ref worldTransform.Orientation, out spaceTransform.LinearTransform);
		castShape.GetSweptLocalBoundingBox(ref startingTransform, ref spaceTransform, ref sweep, out var boundingBox);
		TriangleShape triangle = Resources.GetTriangle();
		RawList<int> intList = Resources.GetIntList();
		if (Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, intList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < intList.Count; i++)
			{
				Shape.TriangleMesh.Data.GetTriangle(intList[i], out triangle.vA, out triangle.vB, out triangle.vC);
				AffineTransform.Transform(ref triangle.vA, ref spaceTransform, out triangle.vA);
				AffineTransform.Transform(ref triangle.vB, ref spaceTransform, out triangle.vB);
				AffineTransform.Transform(ref triangle.vC, ref spaceTransform, out triangle.vC);
				Vector3.Add(ref triangle.vA, ref triangle.vB, out var result);
				Vector3.Add(ref result, ref triangle.vC, out result);
				Vector3.Multiply(ref result, 1f / 3f, out result);
				Vector3.Subtract(ref triangle.vA, ref result, out triangle.vA);
				Vector3.Subtract(ref triangle.vB, ref result, out triangle.vB);
				Vector3.Subtract(ref triangle.vC, ref result, out triangle.vC);
				triangle.maximumRadius = triangle.vA.LengthSquared();
				float num = triangle.vB.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				num = triangle.vC.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				triangle.maximumRadius = (float)Math.Sqrt(triangle.maximumRadius);
				triangle.collisionMargin = 0f;
				RigidTransform transformB = new RigidTransform
				{
					Orientation = Quaternion.Identity,
					Position = result
				};
				if (MPRToolbox.Sweep(castShape, triangle, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref transformB, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
				}
			}
			triangle.maximumRadius = 0f;
			Resources.GiveBack(triangle);
			Resources.GiveBack(intList);
			return hit.T != float.MaxValue;
		}
		Resources.GiveBack(triangle);
		Resources.GiveBack(intList);
		return false;
	}
}
