using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a velocity motor that works on three degrees of freedom.
/// Used when the MotorSettings' motorType is set to velocityMotor.
/// </summary>
public class VelocityMotorSettings3D : VelocityMotorSettings
{
	internal Vector3 goalVelocity;

	/// <summary>
	/// Gets or sets the goal position of the servo.
	/// </summary>
	public Vector3 GoalVelocity
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

	internal VelocityMotorSettings3D(MotorSettings motorSettings)
		: base(motorSettings)
	{
	}
}
