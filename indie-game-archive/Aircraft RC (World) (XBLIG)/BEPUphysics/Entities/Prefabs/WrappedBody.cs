using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.EntityStateManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// A shape formed from the convex hull around its subbodies.  Can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
/// </summary>
public class WrappedBody : Entity<ConvexCollidable<WrappedShape>>
{
	/// <summary>
	///  Gets the list of shapes in the wrapped body.
	/// </summary>
	public ObservableList<ConvexShapeEntry> Shapes => base.CollisionInformation.Shape.Shapes;

	/// <exception cref="T:System.ArgumentException">Thrown when the subbodies list contains zero entities.</exception>
	private WrappedBody(IList<ConvexShapeEntry> subShapes, float mass)
	{
		WrappedShape shape = new WrappedShape(subShapes, out var center);
		Initialize(new ConvexCollidable<WrappedShape>(shape), mass);
		base.Position = center;
	}

	/// <exception cref="T:System.ArgumentException">Thrown when the subbodies list contains zero entities.</exception>
	private WrappedBody(IList<ConvexShapeEntry> subShapes)
	{
		WrappedShape shape = new WrappedShape(subShapes, out var center);
		Initialize(new ConvexCollidable<WrappedShape>(shape));
		base.Position = center;
	}

	/// <summary>
	/// Constructs a physically simulated box.
	/// </summary>
	/// <param name="position">Position of the box.</param>
	/// <param name="subBodies">List of entities composing the body.</param>
	/// <param name="mass">Mass of the object.</param>
	public WrappedBody(Vector3 position, IList<ConvexShapeEntry> subBodies, float mass)
		: this(subBodies, mass)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a nondynamic wrapped body.
	/// </summary>
	/// <param name="position">Position of the box.</param>
	/// <param name="subBodies">List of entities composing the body.</param>
	public WrappedBody(Vector3 position, IList<ConvexShapeEntry> subBodies)
		: this(subBodies)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a dynamic wrapped body.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="subBodies">List of entities composing the body.</param>
	/// <param name="mass">Mass of the object.</param>
	public WrappedBody(MotionState motionState, IList<ConvexShapeEntry> subBodies, float mass)
		: this(subBodies, mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic wrapped body.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="subBodies">List of entities composing the body.</param>
	public WrappedBody(MotionState motionState, IList<ConvexShapeEntry> subBodies)
		: this(subBodies)
	{
		base.MotionState = motionState;
	}
}
