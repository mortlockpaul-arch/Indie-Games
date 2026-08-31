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
///  Unmoving, collidable triangle mesh.
/// </summary>
/// <remarks>
///  The acceleration structure for the mesh is created individually for each
///  StaticMesh; if you want to create many meshes of the same model, consider using the
///  InstancedMesh.
///  </remarks>
public class StaticMesh : StaticCollidable
{
	private TriangleMesh mesh;

	internal TriangleSidedness sidedness;

	internal bool improveBoundaryBehavior = true;

	protected internal ContactEventManager<StaticMesh> events;

	/// <summary>
	///  Gets the TriangleMesh acceleration structure used by the StaticMesh.
	/// </summary>
	public TriangleMesh Mesh => mesh;

	/// <summary>
	///  Gets or sets the world transform of the mesh.
	/// </summary>
	public AffineTransform WorldTransform
	{
		get
		{
			return ((TransformableMeshData)mesh.Data).worldTransform;
		}
		set
		{
			((TransformableMeshData)mesh.Data).WorldTransform = value;
			mesh.Tree.Refit();
			UpdateBoundingBox();
		}
	}

	/// <summary>
	///  Gets the shape used by the mesh.
	/// </summary>
	public new StaticMeshShape Shape => (StaticMeshShape)shape;

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
	///  Gets the event manager used by the mesh.
	/// </summary>
	public ContactEventManager<StaticMesh> Events
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
	///  Constructs a new static mesh.
	/// </summary>
	/// <param name="vertices">Vertex positions of the mesh.</param>
	/// <param name="indices">Index list of the mesh.</param>
	public StaticMesh(Vector3[] vertices, int[] indices)
	{
		base.Shape = new StaticMeshShape(vertices, indices);
		Events = new ContactEventManager<StaticMesh>();
	}

	/// <summary>
	///  Constructs a new static mesh.
	/// </summary>
	/// <param name="vertices">Vertex positions of the mesh.</param>
	/// <param name="indices">Index list of the mesh.</param>
	///  <param name="worldTransform">Transform to use to create the mesh initially.</param>
	public StaticMesh(Vector3[] vertices, int[] indices, AffineTransform worldTransform)
	{
		base.Shape = new StaticMeshShape(vertices, indices, worldTransform);
		Events = new ContactEventManager<StaticMesh>();
	}

	protected override void OnShapeChanged(CollisionShape collisionShape)
	{
		if (!base.IgnoreShapeChanges)
		{
			mesh = new TriangleMesh(Shape.TriangleMeshData);
			UpdateBoundingBox();
		}
	}

	/// <summary>
	/// Updates the bounding box to the current state of the entry.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		boundingBox = mesh.Tree.BoundingBox;
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
		return mesh.RayCast(ray, maximumLength, sidedness, out rayHit);
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
		Toolbox.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		TriangleShape triangle = Resources.GetTriangle();
		RawList<int> intList = Resources.GetIntList();
		if (Mesh.Tree.GetOverlaps(boundingBox, intList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < intList.Count; i++)
			{
				mesh.Data.GetTriangle(intList[i], out triangle.vA, out triangle.vB, out triangle.vC);
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

	/// <summary>
	///  Tests a ray against the mesh.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length to test in units of the ray direction's length.</param>
	/// <param name="sidedness">Sidedness to use when raycasting.  Doesn't have to be the same as the mesh's own sidedness.</param>
	/// <param name="rayHit">Data about the ray's intersection with the mesh, if any.</param>
	/// <returns>Whether or not the ray hit the mesh.</returns>
	public bool RayCast(Ray ray, float maximumLength, TriangleSidedness sidedness, out RayHit rayHit)
	{
		return mesh.RayCast(ray, maximumLength, sidedness, out rayHit);
	}
}
