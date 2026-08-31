using System;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Specifies the way in which a constraint's spring component behaves.
/// </summary>
public class SpringSettings
{
	private readonly SpringAdvancedSettings advanced = new SpringAdvancedSettings();

	internal float dampingConstant = 90000f;

	internal float stiffnessConstant = 600000f;

	/// <summary>
	/// Gets an object containing the solver's direct view of the spring behavior.
	/// </summary>
	public SpringAdvancedSettings Advanced => advanced;

	/// <summary>
	/// Gets or sets the damping constant of this spring.  Higher values reduce oscillation more.
	/// </summary>
	public float DampingConstant
	{
		get
		{
			return dampingConstant;
		}
		set
		{
			dampingConstant = MathHelper.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets or sets the spring constant of this spring.  Higher values make the spring stiffer.
	/// </summary>
	public float StiffnessConstant
	{
		get
		{
			return stiffnessConstant;
		}
		set
		{
			stiffnessConstant = Math.Max(0f, value);
		}
	}

	/// <summary>
	/// Computes the error reduction parameter and softness of a constraint based on its constants.
	/// Automatically called by constraint presteps to compute their per-frame values.
	/// </summary>
	/// <param name="dt">Simulation timestep.</param>
	/// <param name="errorReduction">Error reduction factor to use this frame.</param>
	/// <param name="softness">Adjusted softness of the constraint for this frame.</param>
	public void ComputeErrorReductionAndSoftness(float dt, out float errorReduction, out float softness)
	{
		if (advanced.useAdvancedSettings)
		{
			errorReduction = advanced.errorReductionFactor / dt;
			softness = advanced.softness / dt;
			return;
		}
		if (stiffnessConstant == 0f && dampingConstant == 0f)
		{
			throw new InvalidOperationException("Constraints cannot have both 0 stiffness and 0 damping.");
		}
		errorReduction = stiffnessConstant / (dt * stiffnessConstant + dampingConstant);
		softness = 1f / (dt * (dt * stiffnessConstant + dampingConstant));
	}
}
