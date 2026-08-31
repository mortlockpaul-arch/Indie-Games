using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Collidable with a convex shape.
/// </summary>
public abstract class ConvexCollidable : EntityCollidable
{
	/// <summary>
	///  Gets the shape of the collidable.
	/// </summary>
	public new ConvexShape Shape => (ConvexShape)shape;

	protected ConvexCollidable(ConvexShape shape)
		: base(shape)
	{
		base.Events = new ContactEventManager<EntityCollidable>();
	}

	public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit)
	{
		return MPRToolbox.Sweep(castShape, Shape, ref sweep, ref Toolbox.ZeroVector, ref startingTransform, ref worldTransform, out hit);
	}
}
/// <summary>
///  Collidable with a convex shape of a particular type.
/// </summary>
/// <typeparam name="T">ConvexShape type.</typeparam>
public class ConvexCollidable<T> : ConvexCollidable where T : ConvexShape
{
	/// <summary>
	///  Gets the shape of the collidable.
	/// </summary>
	public new T Shape => (T)shape;

	/// <summary>
	///  Constructs a new convex collidable.
	/// </summary>
	/// <param name="shape">Shape to use in the collidable.</param>
	public ConvexCollidable(T shape)
		: base(shape)
	{
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
		T val = Shape;
		return val.RayTest(ref ray, ref worldTransform, maximumLength, out rayHit);
	}

	protected internal override void UpdateBoundingBoxInternal(float dt)
	{
		T val = Shape;
		val.GetBoundingBox(ref worldTransform, out boundingBox);
		ExpandBoundingBox(ref boundingBox, dt);
	}
}
