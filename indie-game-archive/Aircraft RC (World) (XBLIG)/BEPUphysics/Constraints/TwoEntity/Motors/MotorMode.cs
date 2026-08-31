namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior style of a motor.
/// </summary>
public enum MotorMode
{
	/// <summary>
	/// Velocity motors only work to try to reach some relative velocity.
	/// They have no position goal.
	///
	/// When this type is selected, the motor settings' velocityMotor data will be used.
	/// </summary>
	VelocityMotor,
	/// <summary>
	/// Servomechanisms change their velocity in order to reach some position goal.
	///
	/// When this type is selected, the motor settings' servo data will be used.
	/// </summary>
	Servomechanism
}
