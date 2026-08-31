using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.EntityStateManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Cylinder-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
/// </summary>
public class Cylinder : Entity<ConvexCollidable<CylinderShape>>
{
	/// <summary>
	/// Gets or sets the height of the cylinder.
	/// </summary>
	public float Height
	{
		get
		{
			return base.CollisionInformation.Shape.Height;
		}
		set
		{
			base.CollisionInformation.Shape.Height = value;
		}
	}

	/// <summary>
	/// Gets or sets the radius of the cylinder.
	/// </summary>
	public float Radius
	{
		get
		{
			return base.CollisionInformation.Shape.Radius;
		}
		set
		{
			base.CollisionInformation.Shape.Radius = value;
		}
	}

	private Cylinder(float high, float rad, float mass)
		: base(new ConvexCollidable<CylinderShape>(new CylinderShape(high, rad)), mass)
	{
	}

	private Cylinder(float high, float rad)
		: base(new ConvexCollidable<CylinderShape>(new CylinderShape(high, rad)))
	{
	}

	/// <summary>
	/// Constructs a physically simulated cylinder.
	/// </summary>
	/// <param name="position">Position of the cylinder.</param>
	/// <param name="height">Height of the cylinder.</param>
	/// <param name="radius">Radius of the cylinder.</param>
	/// <param name="mass">Mass of the object.</param>
	public Cylinder(Vector3 position, float height, float radius, float mass)
		: this(height, radius, mass)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a nondynamic cylinder.
	/// </summary>
	/// <param name="position">Position of the cylinder.</param>
	/// <param name="height">Height of the cylinder.</param>
	/// <param name="radius">Radius of the cylinder.</param>
	public Cylinder(Vector3 position, float height, float radius)
		: this(height, radius)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a physically simulated cylinder.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="height">Height of the cylinder.</param>
	/// <param name="radius">Radius of the cylinder.</param>
	/// <param name="mass">Mass of the object.</param>
	public Cylinder(MotionState motionState, float height, float radius, float mass)
		: this(height, radius, mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic cylinder.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="height">Height of the cylinder.</param>
	/// <param name="radius">Radius of the cylinder.</param>
	public Cylinder(MotionState motionState, float height, float radius)
		: this(height, radius)
	{
		base.MotionState = motionState;
	}
}
