using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.EntityStateManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Pill-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
/// </summary>
public class Capsule : Entity<ConvexCollidable<CapsuleShape>>
{
	/// <summary>
	/// Gets or sets the length of the capsule.
	/// </summary>
	public float Length
	{
		get
		{
			return base.CollisionInformation.Shape.Length;
		}
		set
		{
			base.CollisionInformation.Shape.Length = value;
		}
	}

	/// <summary>
	/// Gets or sets the radius of the capsule.
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

	private Capsule(float len, float rad)
		: base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)))
	{
	}

	private Capsule(float len, float rad, float mass)
		: base(new ConvexCollidable<CapsuleShape>(new CapsuleShape(len, rad)), mass)
	{
	}

	/// <summary>
	///  Computes an orientation and length from a line segment.
	/// </summary>
	/// <param name="start">Starting point of the line segment.</param>
	/// <param name="end">Endpoint of the line segment.</param>
	/// <param name="orientation">Orientation of a line that fits the line segment.</param>
	/// <param name="length">Length of the line segment.</param>
	public static void GetCapsuleInformation(ref Vector3 start, ref Vector3 end, out Quaternion orientation, out float length)
	{
		Vector3.Subtract(ref end, ref start, out var result);
		length = result.Length();
		if (length > 0f)
		{
			Vector3.Divide(ref result, length, out result);
			Toolbox.GetQuaternionBetweenNormalizedVectors(ref Toolbox.UpVector, ref result, out orientation);
		}
		else
		{
			orientation = Quaternion.Identity;
		}
	}

	/// <summary>
	///  Constructs a new kinematic capsule.
	/// </summary>
	/// <param name="start">Line segment start point.</param>
	/// <param name="end">Line segment end point.</param>
	/// <param name="radius">Radius of the capsule to expand the line segment by.</param>
	public Capsule(Vector3 start, Vector3 end, float radius)
		: this((end - start).Length(), radius)
	{
		GetCapsuleInformation(ref start, ref end, out var quaternion, out var _);
		base.Orientation = quaternion;
		Vector3.Add(ref start, ref end, out var result);
		Vector3.Multiply(ref result, 0.5f, out result);
		base.Position = result;
	}

	/// <summary>
	///  Constructs a new dynamic capsule.
	/// </summary>
	/// <param name="start">Line segment start point.</param>
	/// <param name="end">Line segment end point.</param>
	/// <param name="radius">Radius of the capsule to expand the line segment by.</param>
	///  <param name="mass">Mass of the entity.</param>
	public Capsule(Vector3 start, Vector3 end, float radius, float mass)
		: this((end - start).Length(), radius, mass)
	{
		GetCapsuleInformation(ref start, ref end, out var quaternion, out var _);
		base.Orientation = quaternion;
		Vector3.Add(ref start, ref end, out var result);
		Vector3.Multiply(ref result, 0.5f, out result);
		base.Position = result;
	}

	/// <summary>
	/// Constructs a physically simulated capsule.
	/// </summary>
	/// <param name="position">Position of the capsule.</param>
	/// <param name="length">Length of the capsule.</param>
	/// <param name="radius">Radius of the capsule.</param>
	/// <param name="mass">Mass of the object.</param>
	public Capsule(Vector3 position, float length, float radius, float mass)
		: this(length, radius, mass)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a nondynamic capsule.
	/// </summary>
	/// <param name="position">Position of the capsule.</param>
	/// <param name="length">Length of the capsule.</param>
	/// <param name="radius">Radius of the capsule.</param>
	public Capsule(Vector3 position, float length, float radius)
		: this(length, radius)
	{
		base.Position = position;
	}

	/// <summary>
	/// Constructs a dynamic capsule.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="length">Length of the capsule.</param>
	/// <param name="radius">Radius of the capsule.</param>
	/// <param name="mass">Mass of the object.</param>
	public Capsule(MotionState motionState, float length, float radius, float mass)
		: this(length, radius, mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic capsule.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="length">Length of the capsule.</param>
	/// <param name="radius">Radius of the capsule.</param>
	public Capsule(MotionState motionState, float length, float radius)
		: this(length, radius)
	{
		base.MotionState = motionState;
	}
}
