using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a servo that works on the relative orientation of two entities.
/// Used when the MotorSettings' motorType is set to servomechanism.
/// </summary>
public class ServoSettingsOrientation : ServoSettings
{
	internal Quaternion goal;

	/// <summary>
	/// Gets or sets the goal orientation of the servo.
	/// </summary>
	public Quaternion Goal
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

	internal ServoSettingsOrientation(MotorSettings motorSettings)
		: base(motorSettings)
	{
	}
}
