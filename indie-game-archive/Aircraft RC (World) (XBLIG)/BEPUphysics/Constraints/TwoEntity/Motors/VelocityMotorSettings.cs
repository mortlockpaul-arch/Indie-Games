namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a velocity motor.
/// Used when the MotorSettings' motorType is set to velocityMotor.
/// </summary>
public class VelocityMotorSettings
{
	internal MotorSettings motorSettings;

	/// <summary>
	/// Softness of this constraint.
	/// Higher values of softness allow the constraint to be violated more.
	/// Must be greater than zero.
	/// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
	/// </summary>
	internal float softness = 0.0001f;

	/// <summary>
	/// Gets and sets the softness of this constraint.
	/// Higher values of softness allow the constraint to be violated more.
	/// Must be greater than zero.
	/// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
	/// For motors, softness can be used to implement damping.  For a damping constant k, the appropriate softness is 1/k.
	/// </summary>
	public float Softness
	{
		get
		{
			return softness;
		}
		set
		{
			value = ((value < 0f) ? 0f : value);
			if (softness != value)
			{
				softness = value;
				motorSettings.WakeUpEntities();
			}
		}
	}

	internal VelocityMotorSettings(MotorSettings motorSettings)
	{
		this.motorSettings = motorSettings;
	}
}
