namespace BEPUphysics.Constraints;

/// <summary>
/// Implemented by solver updateables which have a one dimensional impulse.
/// </summary>
public interface I1DImpulseConstraint
{
	/// <summary>
	/// Gets the current relative velocity of the constraint.
	/// Computed based on the current connection velocities and jacobians.
	/// </summary>
	float RelativeVelocity { get; }

	/// <summary>
	/// Gets the total impulse a constraint has applied.
	/// </summary>
	float TotalImpulse { get; }
}
