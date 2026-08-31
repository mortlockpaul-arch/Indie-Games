using System;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables;

/// <summary>
///  Heightfield-based unmovable collidable object.
/// </summary>
public class Terrain : StaticCollidable
{
	internal AffineTransform worldTransform;

	internal bool improveBoundaryBehavior = true;

	protected internal ContactEventManager<Terrain> events;

	internal float thickness;

	/// <summary>
	///  Gets the shape of this collidable.
	/// </summary>
	public new TerrainShape Shape
	{
		get
		{
			return (TerrainShape)shape;
		}
		set
		{
			base.Shape = value;
		}
	}

	/// <summary>
	///  Gets or sets the affine transform of the terrain.
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
	///  Gets the event manager used by the Terrain.
	/// </summary>
	public ContactEventManager<Terrain> Events
	{
		get
		{
			return events;
		}
		set
		{
			if (value.Owner != null && value != events)
			{
				throw new Exception("Event manager is already owned by a Terrain; event managers cannot be shared.");
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
	/// Gets or sets the thickness of the terrain.  This defines how far below the triangles of the terrain's surface the terrain 'body' extends.
	/// Anything within the body of the terrain will be pulled back up to the surface.
	/// </summary>
	public float Thickness
	{
		get
		{
			return thickness;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Cannot use a negative thickness value.");
			}
			Vector3 vector = Vector3.Normalize(worldTransform.LinearTransform.Down);
			Vector3 vector2 = vector * (value - thickness);
			if (vector.X < 0f)
			{
				boundingBox.Min.X += vector2.X;
			}
			else
			{
				boundingBox.Max.X += vector2.X;
			}
			if (vector.Y < 0f)
			{
				boundingBox.Min.Y += vector2.Y;
			}
			else
			{
				boundingBox.Max.Y += vector2.Y;
			}
			if (vector.Z < 0f)
			{
				boundingBox.Min.Z += vector2.Z;
			}
			else
			{
				boundingBox.Max.Z += vector2.Z;
			}
			thickness = value;
		}
	}

	/// <summary>
	///  Constructs a new Terrain.
	/// </summary>
	/// <param name="shape">Shape to use for the terrain.</param>
	/// <param name="worldTransform">Transform to use for the terrain.</param>
	public Terrain(TerrainShape shape, AffineTransform worldTransform)
	{
		this.worldTransform = worldTransform;
		Shape = shape;
		Events = new ContactEventManager<Terrain>();
	}

	/// <summary>
	///  Constructs a new Terrain.
	/// </summary>
	/// <param name="heights">Height data to use to create the TerrainShape.</param>
	/// <param name="worldTransform">Transform to use for the terrain.</param>
	public Terrain(float[,] heights, AffineTransform worldTransform)
		: this(new TerrainShape(heights), worldTransform)
	{
	}

	/// <summary>
	///  Updates the bounding box of the terrain.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		Shape.GetBoundingBox(ref worldTransform, out boundingBox);
		Vector3 vector = Vector3.Normalize(worldTransform.LinearTransform.Down) * thickness;
		if (vector.X < 0f)
		{
			boundingBox.Min.X += vector.X;
		}
		else
		{
			boundingBox.Max.X += vector.X;
		}
		if (vector.Y < 0f)
		{
			boundingBox.Min.Y += vector.Y;
		}
		else
		{
			boundingBox.Max.Y += vector.Y;
		}
		if (vector.Z < 0f)
		{
			boundingBox.Min.Z += vector.Z;
		}
		else
		{
			boundingBox.Max.Z += vector.Z;
		}
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
		return Shape.RayCast(ref ray, maximumLength, ref worldTransform, out rayHit);
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
		castShape.GetSweptLocalBoundingBox(ref startingTransform, ref worldTransform, ref sweep, out var localSpaceBoundingBox);
		TriangleShape triangle = Resources.GetTriangle();
		RawList<TriangleMeshConvexContactManifold.TriangleIndices> triangleIndicesList = Resources.GetTriangleIndicesList();
		if (Shape.GetOverlaps(localSpaceBoundingBox, triangleIndicesList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < triangleIndicesList.count; i++)
			{
				Shape.GetTriangle(ref triangleIndicesList.Elements[i], ref worldTransform, out triangle.vA, out triangle.vB, out triangle.vC);
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
			Resources.GiveBack(triangleIndicesList);
			return hit.T != float.MaxValue;
		}
		Resources.GiveBack(triangle);
		Resources.GiveBack(triangleIndicesList);
		return false;
	}

	/// <summary>
	///  Gets the normal of a vertex at the given indices.
	/// </summary>
	/// <param name="i">First dimension index into the heightmap array.</param>
	/// <param name="j">Second dimension index into the heightmap array.</param>
	/// <param name="normal">Normal at the given indices.</param>
	public void GetNormal(int i, int j, out Vector3 normal)
	{
		Shape.GetNormal(i, j, ref worldTransform, out normal);
	}

	/// <summary>
	///  Gets the position of a vertex at the given indices.
	/// </summary>
	/// <param name="i">First dimension index into the heightmap array.</param>
	/// <param name="j">Second dimension index into the heightmap array.</param>
	/// <param name="position">Position at the given indices.</param>
	public void GetPosition(int i, int j, out Vector3 position)
	{
		Shape.GetPosition(i, j, ref worldTransform, out position);
	}
}
