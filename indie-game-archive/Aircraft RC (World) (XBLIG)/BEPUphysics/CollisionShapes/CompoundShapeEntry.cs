using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  Contains a shape and its local transform relative to its owning compound shape.
///  This is used to construct compound shapes.
/// </summary>
public struct CompoundShapeEntry
{
	/// <summary>
	///  Local transform of the shape relative to its owning compound shape.
	/// </summary>
	public RigidTransform LocalTransform;

	/// <summary>
	/// Shape used by the compound.
	/// </summary>
	public EntityShape Shape;

	/// <summary>
	/// Weight of the entry.  This defines how much the entry contributes to its owner
	/// for the purposes of center of rotation computation.
	/// </summary>
	public float Weight;

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="localTransform">Local transform of the shape.</param>
	/// <param name="weight">Weight of the entry.  This defines how much the entry contributes to its owner
	///  for the purposes of center of rotation computation.</param>
	public CompoundShapeEntry(EntityShape shape, RigidTransform localTransform, float weight)
	{
		LocalTransform = localTransform;
		Shape = shape;
		Weight = weight;
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="position">Local position of the shape.</param>
	/// <param name="weight">Weight of the entry.  This defines how much the entry contributes to its owner
	///  for the purposes of center of mass and inertia computation.</param>
	public CompoundShapeEntry(EntityShape shape, Vector3 position, float weight)
	{
		LocalTransform = new RigidTransform(position);
		Shape = shape;
		Weight = weight;
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="orientation">Local orientation of the shape.</param>
	/// <param name="weight">Weight of the entry.  This defines how much the entry contributes to its owner
	///  for the purposes of center of rotation computation.</param>
	public CompoundShapeEntry(EntityShape shape, Quaternion orientation, float weight)
	{
		LocalTransform = new RigidTransform(orientation);
		Shape = shape;
		Weight = weight;
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="weight">Weight of the entry.  This defines how much the entry contributes to its owner
	///  for the purposes of center of rotation computation.</param>
	public CompoundShapeEntry(EntityShape shape, float weight)
	{
		LocalTransform = RigidTransform.Identity;
		Shape = shape;
		Weight = weight;
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="localTransform">Local transform of the shape.</param>
	public CompoundShapeEntry(EntityShape shape, RigidTransform localTransform)
	{
		LocalTransform = localTransform;
		Shape = shape;
		Weight = shape.ComputeVolume();
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="position">Local position of the shape.</param>
	public CompoundShapeEntry(EntityShape shape, Vector3 position)
	{
		LocalTransform = new RigidTransform(position);
		Shape = shape;
		Weight = shape.ComputeVolume();
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	/// <param name="orientation">Local orientation of the shape.</param>
	public CompoundShapeEntry(EntityShape shape, Quaternion orientation)
	{
		LocalTransform = new RigidTransform(orientation);
		Shape = shape;
		Weight = shape.ComputeVolume();
	}

	/// <summary>
	///  Constructs a new compound shape entry using the volume of the shape as a weight.
	/// </summary>
	/// <param name="shape">Shape to use.</param>
	public CompoundShapeEntry(EntityShape shape)
	{
		LocalTransform = RigidTransform.Identity;
		Shape = shape;
		Weight = shape.ComputeVolume();
	}
}
