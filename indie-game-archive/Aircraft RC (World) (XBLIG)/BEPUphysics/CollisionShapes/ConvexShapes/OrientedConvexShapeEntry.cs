using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  A shape associated with an orientation.
/// </summary>
public struct OrientedConvexShapeEntry
{
	/// <summary>
	///  The entry's shape.
	/// </summary>
	public ConvexShape CollisionShape;

	/// <summary>
	///  The entry's orientation.
	/// </summary>
	public Quaternion Orientation;

	/// <summary>
	///  Constructs a new entry.
	/// </summary>
	/// <param name="orientation">Orientation of the entry.</param>
	/// <param name="shape">Shape of the entry.</param>
	public OrientedConvexShapeEntry(Quaternion orientation, ConvexShape shape)
	{
		Orientation = orientation;
		CollisionShape = shape;
	}

	/// <summary>
	///  Constructs a new entry with identity orientation.
	/// </summary>
	/// <param name="shape">Shape of the entry.</param>
	public OrientedConvexShapeEntry(ConvexShape shape)
	{
		Orientation = Quaternion.Identity;
		CollisionShape = shape;
	}
}
