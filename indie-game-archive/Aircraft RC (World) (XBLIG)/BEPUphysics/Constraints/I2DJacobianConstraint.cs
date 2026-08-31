using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Denotes a class that uses two linear jacobian axes.
/// </summary>
public interface I2DJacobianConstraint
{
	/// <summary>
	/// Gets the angular jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the first connected entity.</param>
	void GetAngularJacobianA(out Vector3 jacobianX, out Vector3 jacobianY);

	/// <summary>
	/// Gets the angular jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First angular jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second angular jacobian entry for the second connected entity.</param>
	void GetAngularJacobianB(out Vector3 jacobianX, out Vector3 jacobianY);

	/// <summary>
	/// Gets the linear jacobian entry for the first connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the first connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the first connected entity.</param>
	void GetLinearJacobianA(out Vector3 jacobianX, out Vector3 jacobianY);

	/// <summary>
	/// Gets the linear jacobian entry for the second connected entity.
	/// </summary>
	/// <param name="jacobianX">First linear jacobian entry for the second connected entity.</param>
	/// <param name="jacobianY">Second linear jacobian entry for the second connected entity.</param>
	void GetLinearJacobianB(out Vector3 jacobianX, out Vector3 jacobianY);

	/// <summary>
	/// Gets the mass matrix of the constraint.
	/// </summary>
	/// <param name="massMatrix">Constraint's mass matrix.</param>
	void GetMassMatrix(out Matrix2X2 massMatrix);
}
