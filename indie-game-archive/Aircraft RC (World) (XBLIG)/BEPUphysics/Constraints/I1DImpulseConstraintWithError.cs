namespace BEPUphysics.Constraints;

/// <summary>
/// Implemented by solver updateables which have a one dimensional impulse.
/// </summary>
public interface I1DImpulseConstraintWithError : I1DImpulseConstraint
{
	/// <summary>
	/// Gets the current constraint error.
	/// </summary>
	float Error { get; }
}
