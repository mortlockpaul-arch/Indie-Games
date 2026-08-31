namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a servo that works on one degree of freedom.
/// Used when the MotorSettings' motorType is set to servomechanism.
/// </summary>
public class ServoSettings1D : ServoSettings
{
	internal float goal;

	/// <summary>
	/// Gets or sets the goal position of the servo.
	/// </summary>
	public float Goal
	{
		get
		{
			return goal;
		}
		set
		{
			if (goal != value)
			{
				goal = value;
				motorSettings.WakeUpEntities();
			}
		}
	}

	internal ServoSettings1D(MotorSettings motorSettings)
		: base(motorSettings)
	{
	}
}
