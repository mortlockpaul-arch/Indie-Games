using System;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables;

/// <summary>
///  Collidable mesh which can be created from a reusable InstancedMeshShape.
///  Very little data is needed for each individual InstancedMesh object, allowing
///  a complicated mesh to be repeated many times.  Since the hierarchy used to accelerate
///  collisions is purely local, it may be marginally slower than an individual StaticMesh.
/// </summary>
public class InstancedMesh : StaticCollidable
{
	internal AffineTransform worldTransform;

	internal TriangleSidedness sidedness;

	internal bool improveBoundaryBehavior = true;

	protected internal ContactEventManager<InstancedMesh> events;

	/// <summary>
	///  Gets or sets the world transform of the mesh.
	/// </summary>
	public AffineTransform WorldTransform
	{
		get
		{
			return worldTransform;
		}
		set
		{
			worldTransform = value;
			Shape.ComputeBoundingBox(ref value, out boundingBox);
		}
	}

	/// <summary>
	///  Gets the shape used by the instanced mesh.
	/// </summary>
	public new InstancedMeshShape Shape => (InstancedMeshShape)shape;

	/// <summary>
	///  Gets or sets the sidedness of the mesh.  This can be used to ignore collisions and rays coming from a direction relative to the winding of the triangle.
	/// </summary>
	public TriangleSidedness Sidedness
	{
		get
		{
			return sidedness;
		}
		set
		{
			sidedness = value;
		}
	}

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
	///  Gets the event manager of the mesh.
	/// </summary>
	public ContactEventManager<InstancedMesh> Events
	{
		get
		{
			return events;
		}
		set
		{
			if (value.Owner != null && value != events)
			{
				throw new Exception("Event manager is already owned by a mesh; event managers cannot be shared.");
			}
			if (events != null)
			{
				events.Owner = null;
			}
			events = value;
			if (events != null)
			{
				events.Owner = this;
			}
		}
	}

	protected internal override IContactEventTriggerer EventTriggerer => events;

	protected override IDeferredEventCreator EventCreator => events;

	/// <summary>
	/// Updates the bounding box to the current state of the entry.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		Shape.ComputeBoundingBox(ref worldTransform, out boundingBox);
	}

	/// <summary>
	///  Constructs a new InstancedMesh.
	/// </summary>
	/// <param name="meshShape">Shape to use for the instance.</param>
	public InstancedMesh(InstancedMeshShape meshShape)
		: this(meshShape, AffineTransform.Identity)
	{
	}

	/// <summary>
	///  Constructs a new InstancedMesh.
	/// </summary>
	/// <param name="meshShape">Shape to use for the instance.</param>
	/// <param name="worldTransform">Transform to use for the instance.</param>
	public InstancedMesh(InstancedMeshShape meshShape, AffineTransform worldTransform)
	{
		this.worldTransform = worldTransform;
		base.Shape = meshShape;
		Events = new ContactEventManager<InstancedMesh>();
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
		return RayCast(ray, maximumLength, sidedness, out rayHit);
	}

	/// <summary>
	///  Tests a ray against the instance.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length of the ray to test; in units of the ray's direction's length.</param>
	/// <param name="sidedness">Sidedness to use during the ray cast.  This does not have to be the same as the mesh's sidedness.</param>
	/// <param name="rayHit">The hit location of the ray on the mesh, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
	{
		AffineTransform.Invert(ref worldTransform, out var inverse);
		Ray ray2 = default(Ray);
		Matrix3X3.Transform(ref ray.Direction, ref inverse.LinearTransform, out ray2.Direction);
		AffineTransform.Transform(ref ray.Position, ref inverse, out ray2.Position);
		if (Shape.TriangleMesh.RayCast(ray2, maximumLength, sidedness, out rayHit))
		{
			Vector3.Multiply(ref ray.Direction, rayHit.T, out rayHit.Location);
			Vector3.Add(ref rayHit.Location, ref ray.Position, out rayHit.Location);
			Matrix3X3.TransformTranspose(ref rayHit.Normal, ref inverse.LinearTransform, out rayHit.Normal);
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
		hit = default(RayHit);
		castShape.GetSweptLocalBoundingBox(ref startingTransform, ref worldTransform, ref sweep, out var boundingBox);
		TriangleShape triangle = Resources.GetTriangle();
		RawList<int> intList = Resources.GetIntList();
		if (Shape.TriangleMesh.Tree.GetOverlaps(boundingBox, intList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < intList.Count; i++)
			{
				Shape.TriangleMesh.Data.GetTriangle(intList[i], out triangle.vA, out triangle.vB, out triangle.vC);
				AffineTransform.Transform(ref triangle.vA, ref worldTransform, out triangle.vA);
				AffineTransform.Transform(ref triangle.vB, ref worldTransform, out triangle.vB);
				AffineTransform.Transform(ref triangle.vC, ref worldTransform, out triangle.vC);
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
