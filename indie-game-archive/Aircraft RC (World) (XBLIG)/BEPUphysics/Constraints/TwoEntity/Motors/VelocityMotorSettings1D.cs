namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a velocity motor that works on one degree of freedom.
/// Used when the MotorSettings' motorType is set to velocityMotor.
/// </summary>
public class VelocityMotorSettings1D : VelocityMotorSettings
{
	internal float goalVelocity;

	/// <summary>
	/// Gets or sets the goal velocity of the motor.
	/// </summary>
	public float GoalVelocity
	{
		get
		{
			return goalVelocity;
		}
		set
		{
			goalVelocity = value;
			motorSettings.WakeUpEntities();
		}
	}

	internal VelocityMotorSettings1D(MotorSettings motorSettings)
		: base(motorSettings)
	{
	}
}
