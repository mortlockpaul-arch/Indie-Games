using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Denotes a class that uses a single linear jacobian axis.
/// </summary>
public interface I1DJacobianConstraint
{
	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
	void GetAngularJacobianA(out Vector3 jacobian);

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
	void GetAngularJacobianB(out Vector3 jacobian);

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
	void GetLinearJacobianA(out Vector3 jacobian);

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
	void GetLinearJacobianB(out Vector3 jacobian);

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="outputMassMatrix">Constraint's mass matrix.</param>
	void GetMassMatrix(out float outputMassMatrix);
}
