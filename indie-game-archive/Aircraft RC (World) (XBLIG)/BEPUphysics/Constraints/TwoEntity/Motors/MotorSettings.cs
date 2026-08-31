namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Contains genereal settings for motors.
/// </summary>
public abstract class MotorSettings
{
	internal EntitySolverUpdateable motor;

	internal float maximumForce = float.MaxValue;

	internal MotorMode mode;

	/// <summary>
	/// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
	/// This field can be used to simulate friction in a constraint.
	/// </summary>
	public float MaximumForce
	{
		get
		{
			if (maximumForce > 0f)
			{
				return maximumForce;
			}
			return 0f;
		}
		set
		{
			value = ((value >= 0f) ? value : 0f);
			if (value != maximumForce)
			{
				maximumForce = value;
				WakeUpEntities();
			}
		}
	}

	/// <summary>
	/// Gets or sets what kind of motor this is.
	///
	/// If velocityMotor is chosen, the motor will try to achieve some velocity using the VelocityMotorSettings.
	/// If servomechanism is chosen, the motor will try to reach some position using the ServoSettings.
	/// </summary>
	public MotorMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if (mode != value)
			{
				mode = value;
				WakeUpEntities();
			}
		}
	}

	internal MotorSettings(EntitySolverUpdateable motor)
	{
		this.motor = motor;
	}

	internal void WakeUpEntities()
	{
		for (int i = 0; i < motor.involvedEntities.count; i++)
		{
			if (motor.involvedEntities[i].isDynamic)
			{
				motor.involvedEntities[i].activityInformation.Activate();
				break;
			}
		}
	}
}
