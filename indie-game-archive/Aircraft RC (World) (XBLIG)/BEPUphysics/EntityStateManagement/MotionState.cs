using System;
using Microsoft.Xna.Framework;

namespace BEPUphysics.EntityStateManagement;

/// <summary>
///  State describing the position, orientation, and velocity of an entity.
/// </summary>
public struct MotionState : IEquatable<MotionState>
{
	/// <summary>
	///  Position of an entity.
	/// </summary>
	public Vector3 Position;

	/// <summary>
	///  Orientation of an entity.
	/// </summary>
	public Quaternion Orientation;

	/// <summary>
	///  Linear velocity of an entity.
	/// </summary>
	public Vector3 LinearVelocity;

	/// <summary>
	///  Angular velocity of an entity.
	/// </summary>
	public Vector3 AngularVelocity;

	/// <summary>
	///  Orientation matrix of an entity.
	/// </summary>
	public Matrix OrientationMatrix
	{
		get
		{
			Matrix.CreateFromQuaternion(ref Orientation, out var result);
			return result;
		}
	}

	/// <summary>
	///  World transform of an entity.
	/// </summary>
	public Matrix WorldTransform
	{
		get
		{
			Matrix.CreateFromQuaternion(ref Orientation, out var result);
			result.Translation = Position;
			return result;
		}
	}

	public bool Equals(MotionState other)
	{
		if (other.AngularVelocity == AngularVelocity && other.LinearVelocity == LinearVelocity && other.Position == Position)
		{
			return other.Orientation == Orientation;
		}
		return false;
	}
}
