using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Contains the error reduction factor and softness of a constraint.
/// These can be used to make the same behaviors as the stiffness and damping constants,
/// but may provide a more intuitive representation for rigid constraints.
/// </summary>
public class SpringAdvancedSettings
{
	internal float errorReductionFactor = 0.1f;

	internal float softness = 1E-05f;

	internal bool useAdvancedSettings;

	/// <summary>
	/// Gets or sets the error reduction parameter of the spring.
	/// </summary>
	public float ErrorReductionFactor
	{
		get
		{
			return errorReductionFactor;
		}
		set
		{
			errorReductionFactor = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	/// Gets or sets the softness of the joint.  Higher values allow the constraint to be violated more.
	/// </summary>
	public float Softness
	{
		get
		{
			return softness;
		}
		set
		{
			softness = MathHelper.Max(0f, value);
		}
	}

	/// <summary>
	/// Gets or sets whether or not to use the advanced settings.
	/// If this is set to true, the errorReductionFactor and softness will be used instead
	/// of the stiffness constant and damping constant.
	/// </summary>
	public bool UseAdvancedSettings
	{
		get
		{
			return useAdvancedSettings;
		}
		set
		{
			useAdvancedSettings = value;
		}
	}
}
