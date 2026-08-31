namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Contains settings for motors which act on one degree of freedom.
/// </summary>
public class MotorSettings1D : MotorSettings
{
	internal ServoSettings1D servo;

	internal VelocityMotorSettings1D velocityMotor;

	/// <summary>
	/// Gets the settings that govern the behavior of this motor if it is a servomechanism.
	/// </summary>
	public ServoSettings1D Servo => servo;

	/// <summary>
	/// Gets the settings that govern the behavior of this motor if it is a velocity motor.
	/// </summary>
	public VelocityMotorSettings1D VelocityMotor => velocityMotor;

	internal MotorSettings1D(Motor motor)
		: base(motor)
	{
		servo = new ServoSettings1D(this);
		velocityMotor = new VelocityMotorSettings1D(this);
	}
}
