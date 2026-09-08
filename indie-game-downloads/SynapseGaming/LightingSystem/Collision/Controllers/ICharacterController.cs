namespace SynapseGaming.LightingSystem.Collision.Controllers;

/// <summary>
/// Interface used by classes that control character objects.
/// </summary>
public interface ICharacterController : IObjectController, ICharacterControllerProperties
{
	/// <summary>
	/// Determines if the character is currently crouching.
	/// </summary>
	bool Crouching { get; }

	/// <summary>
	/// Determines if the character is currently sliding across a surface.
	/// </summary>
	bool Sliding { get; }

	/// <summary>
	/// Determines if the character is currently on a surface and has traction.
	/// </summary>
	bool Resting { get; }

	/// <summary>
	/// Causes the character to crouch.
	/// </summary>
	bool Crouch { get; set; }

	/// <summary>
	/// Causes the character to jump.
	/// </summary>
	void Jump();
}
