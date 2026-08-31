using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Implemented by solver updateables which have a three dimensional impulse.
/// </summary>
public interface I3DImpulseConstraint
{
	/// <summary>
	/// Gets the current relative velocity of the constraint.
	/// Computed based on the current connection velocities and jacobians.
	/// </summary>
	Vector3 RelativeVelocity { get; }

	/// <summary>
	/// Gets the total impulse a constraint has applied.
	/// </summary>
	Vector3 TotalImpulse { get; }
}
