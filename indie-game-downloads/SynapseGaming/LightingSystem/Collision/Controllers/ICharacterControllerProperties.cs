using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Collision.Controllers;

/// <summary>
/// Interface that exposes properties common to character controllers.
/// </summary>
public interface ICharacterControllerProperties
{
	/// <summary>
	/// Amount of smoothing applied to object movement.
	/// </summary>
	float MovementSmoothing { get; set; }

	/// <summary>
	/// Height of object when in crouched stance.
	/// </summary>
	float CrouchHeight { get; set; }

	/// <summary>
	/// Vertical speed applied to object when jumping.
	/// </summary>
	float JumpSpeed { get; set; }

	/// <summary>
	/// Maximum height of obstruction object can step up onto.
	/// </summary>
	float MaximumStepHeight { get; set; }

	/// <summary>
	/// Maximum speed of object when moving.
	/// </summary>
	float MaximumSpeed { get; set; }

	/// <summary>
	/// Maximum speed of object when moving in crouched stance.
	/// </summary>
	float MaximumCrouchSpeed { get; set; }

	/// <summary>
	/// Maximum speed of object when sliding across a surface.
	/// </summary>
	float MaximumSlideSpeed { get; set; }

	/// <summary>
	/// Maximum speed of object when falling.
	/// </summary>
	float MaximumAirborneSpeed { get; set; }

	/// <summary>
	/// Amount of force applied to the object each update. This controls how quickly the object reaches its maximum speed.
	/// </summary>
	float MovementForce { get; set; }

	/// <summary>
	/// Player the object is controlled by.
	/// </summary>
	PlayerIndex PlayerIndex { get; set; }
}
