using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Convex shape entry to a WrappedShape.
/// </summary>
public struct ConvexShapeEntry
{
	/// <summary>
	/// Convex shape of the entry.
	/// </summary>
	public ConvexShape CollisionShape;

	/// <summary>
	/// Local transform of the entry.
	/// </summary>
	public RigidTransform Transform;

	/// <summary>
	/// Constructs a convex shape entry.
	/// </summary>
	/// <param name="position">Local position of the entry.</param>
	/// <param name="shape">Shape of the entry.</param>
	public ConvexShapeEntry(Vector3 position, ConvexShape shape)
	{
		Transform = new RigidTransform(position);
		CollisionShape = shape;
	}

	/// <summary>
	/// Constructs a convex shape entry.
	/// </summary>
	/// <param name="orientation">Local orientation of the entry.</param>
	/// <param name="shape">Shape of the entry.</param>
	public ConvexShapeEntry(Quaternion orientation, ConvexShape shape)
	{
		Transform = new RigidTransform(orientation);
		CollisionShape = shape;
	}

	/// <summary>
	/// Constructs a convex shape entry.
	/// </summary>
	/// <param name="transform">Local transform of the entry.</param>
	/// <param name="shape">Shape of the entry.</param>
	public ConvexShapeEntry(RigidTransform transform, ConvexShape shape)
	{
		Transform = transform;
		CollisionShape = shape;
	}

	/// <summary>
	///  Constructs a convex shape entry with identity transformation.
	/// </summary>
	/// <param name="shape">Shape of the entry.</param>
	public ConvexShapeEntry(ConvexShape shape)
	{
		Transform = RigidTransform.Identity;
		CollisionShape = shape;
	}
}
