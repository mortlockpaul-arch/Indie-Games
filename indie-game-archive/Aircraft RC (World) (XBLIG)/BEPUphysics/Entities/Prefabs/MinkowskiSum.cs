using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.EntityStateManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Shape representing the sweeping of one entity through another.  Can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
/// </summary>
public class MinkowskiSum : Entity<ConvexCollidable<MinkowskiSumShape>>
{
	/// <summary>
	/// First entity in the sum.
	/// </summary>
	public Entity EntityA;

	/// <summary>
	/// Second entity in the sum.
	/// </summary>
	public Entity EntityB;

	private MinkowskiSum(OrientedConvexShapeEntry a, OrientedConvexShapeEntry b, float m)
		: base(new ConvexCollidable<MinkowskiSumShape>(new MinkowskiSumShape(a, b)), m)
	{
		base.Position = -base.CollisionInformation.Shape.LocalOffset;
	}

	private MinkowskiSum(OrientedConvexShapeEntry a, OrientedConvexShapeEntry b)
		: base(new ConvexCollidable<MinkowskiSumShape>(new MinkowskiSumShape(a, b)))
	{
		base.Position = -base.CollisionInformation.Shape.LocalOffset;
	}

	/// <summary>
	/// Constructs a dynamic minkowski sum.
	/// </summary>
	/// <param name="position">Position of the resulting shape.</param>
	/// <param name="a">First entity in the sum.</param>
	/// <param name="b">Second entity in the sum.</param>
	/// <param name="mass">Mass of the object.</param>
	public MinkowskiSum(Vector3 position, OrientedConvexShapeEntry a, OrientedConvexShapeEntry b, float mass)
		: this(a, b, mass)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a nondynamic minkowski sum of two entities.
	/// </summary>
	/// <param name="position">Position of the resulting shape.</param>
	/// <param name="a">First entity in the sum.</param>
	/// <param name="b">Second entity in the sum.</param>
	public MinkowskiSum(Vector3 position, OrientedConvexShapeEntry a, OrientedConvexShapeEntry b)
		: this(a, b)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a dynamic minkowski sum of two entities.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="a">First entity in the sum.</param>
	/// <param name="b">Second entity in the sum.</param>
	/// <param name="mass">Mass of the object.</param>
	public MinkowskiSum(MotionState motionState, OrientedConvexShapeEntry a, OrientedConvexShapeEntry b, float mass)
		: this(a, b, mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic minkowski sum of two entities.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="a">First entity in the sum.</param>
	/// <param name="b">Second entity in the sum.</param>
	public MinkowskiSum(MotionState motionState, OrientedConvexShapeEntry a, OrientedConvexShapeEntry b)
		: this(a, b)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a dynamic minkowski sum entity.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="shapes">List of shapes to make the sum frmo.</param>
	/// <param name="mass">Mass of the object.</param>
	public MinkowskiSum(MotionState motionState, IList<OrientedConvexShapeEntry> shapes, float mass)
		: base(new ConvexCollidable<MinkowskiSumShape>(new MinkowskiSumShape(shapes)), mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic minkowski sum.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="shapes">List of shapes to make the sum frmo.</param>
	public MinkowskiSum(MotionState motionState, IList<OrientedConvexShapeEntry> shapes)
		: base(new ConvexCollidable<MinkowskiSumShape>(new MinkowskiSumShape(shapes)))
	{
		base.MotionState = motionState;
	}
}
