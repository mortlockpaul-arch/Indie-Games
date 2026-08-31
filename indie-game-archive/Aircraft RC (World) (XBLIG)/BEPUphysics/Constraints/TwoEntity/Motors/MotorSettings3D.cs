namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Contains settings for motors which act on three degrees of freedom.
/// </summary>
public class MotorSettings3D : MotorSettings
{
	internal ServoSettings3D servo;

	internal VelocityMotorSettings3D velocityMotor;

	/// <summary>
	/// Gets the settings that govern the behavior of this motor if it is a servomechanism.
	/// </summary>
	public ServoSettings3D Servo => servo;

	/// <summary>
	/// Gets the settings that govern the behavior of this motor if it is a velocity motor.
	/// </summary>
	public VelocityMotorSettings3D VelocityMotor => velocityMotor;

	internal MotorSettings3D(EntitySolverUpdateable motor)
		: base(motor)
	{
		servo = new ServoSettings3D(this);
		velocityMotor = new VelocityMotorSettings3D(this);
	}
}
