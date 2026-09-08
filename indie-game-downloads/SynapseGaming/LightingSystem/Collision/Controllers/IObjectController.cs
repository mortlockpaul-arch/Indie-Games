using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Collision.Controllers;

/// <summary>
/// Interface used by classes that control other objects by converting input to object movement.
/// </summary>
public interface IObjectController
{
	/// <summary>
	/// Object movement is applied to.
	/// </summary>
	ICollisionObject ParentObject { get; }

	/// <summary>
	/// Applies movement in object space to the ParentObject.
	/// </summary>
	/// <param name="objectvector"></param>
	void Move(Vector2 objectvector);

	/// <summary>
	/// Applies forward movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	void MoveForward(float amount);

	/// <summary>
	/// Applies backward movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	void MoveBackward(float amount);

	/// <summary>
	/// Applies left movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	void MoveLeft(float amount);

	/// <summary>
	/// Applies right movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	void MoveRight(float amount);

	/// <summary>
	/// Applies object space rotation around the Y axis (the "Up" axis) to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	void Turn(float amount);
}
