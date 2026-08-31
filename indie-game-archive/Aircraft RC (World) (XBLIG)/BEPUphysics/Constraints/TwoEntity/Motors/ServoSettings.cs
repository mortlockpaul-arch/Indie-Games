using System;

namespace BEPUphysics.Constraints.TwoEntity.Motors;

/// <summary>
/// Defines the behavior of a servo.
/// Used when the MotorSettings' motorType is set to servomechanism.
/// </summary>
public class ServoSettings : ISpringSettings
{
	internal MotorSettings motorSettings;

	/// <summary>
	/// Speed at which the servo will try to achieve its goal.
	/// </summary>
	internal float baseCorrectiveSpeed;

	/// <summary>
	/// Maximum extra velocity that the constraint will apply in an effort to correct constraint error.
	/// </summary>
	internal float maxCorrectiveVelocity = float.MaxValue;

	/// <summary>
	/// Squared maximum extra velocity that the constraint will apply in an effort to correct constraint error.
	/// </summary>
	internal float maxCorrectiveVelocitySquared = float.MaxValue;

	/// <summary>
	/// Spring settings define how a constraint responds to velocity and position error.
	/// </summary>
	internal SpringSettings springSettings = new SpringSettings();

	/// <summary>
	/// Gets and sets the speed at which the servo will try to achieve its goal.
	/// This is inactive if the constraint is not in servo mode.
	/// </summary>
	public float BaseCorrectiveSpeed
	{
		get
		{
			return baseCorrectiveSpeed;
		}
		set
		{
			value = ((value < 0f) ? 0f : value);
			if (value != baseCorrectiveSpeed)
			{
				baseCorrectiveSpeed = value;
				motorSettings.WakeUpEntities();
			}
		}
	}

	/// <summary>
	/// Gets or sets the maximum extra velocity that the constraint will apply in an effort to correct any constraint error.
	/// </summary>
	public float MaxCorrectiveVelocity
	{
		get
		{
			return maxCorrectiveVelocity;
		}
		set
		{
			value = Math.Max(0f, value);
			if (maxCorrectiveVelocity != value)
			{
				maxCorrectiveVelocity = value;
				if (maxCorrectiveVelocity >= float.MaxValue)
				{
					maxCorrectiveVelocitySquared = float.MaxValue;
				}
				else
				{
					maxCorrectiveVelocitySquared = maxCorrectiveVelocity * maxCorrectiveVelocity;
				}
				motorSettings.WakeUpEntities();
			}
		}
	}

	/// <summary>
	/// Gets the spring settings used by the constraint.
	/// Spring settings define how a constraint responds to velocity and position error.
	/// </summary>
	public SpringSettings SpringSettings => springSettings;

	internal ServoSettings(MotorSettings motorSettings)
	{
		this.motorSettings = motorSettings;
	}
}
