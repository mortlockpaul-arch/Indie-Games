using System;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Interface to a 3rd party collision / physics entity, which represents a SunBurn scene object in the simulation.
/// </summary>
public interface ICollisionEntity : IDisposable
{
	/// <summary>
	/// Distance the object will move this frame.
	/// </summary>
	float Distance { get; }

	/// <summary>
	/// Normalized direction the object will move this frame.
	/// </summary>
	Vector3 Normal { get; }

	/// <summary>
	/// SunBurn scene object represented in the collision / physics simulation by this entity.
	/// </summary>
	ICollisionObject Object { get; }

	/// <summary>
	/// Determines if the SunBurn scene object represented by this entity has changed and
	/// information needs to be resynchronized with the entity.
	/// </summary>
	/// <returns></returns>
	bool CheckSceneObjectChanged();

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	void ApplyWorldForce(ref Vector3 worldforce);

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldposition">World-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	void ApplyWorldForce(ref Vector3 worldposition, ref Vector3 worldforce);

	/// <summary>
	/// Removes all accumulated forces acting on the object. This will halt the object
	/// movement, however future forces (such as gravity) can immediately begin acting
	/// on the object again.
	/// </summary>
	void RemoveForces();

	/// <summary>
	/// Applies changes made on the SunBurn scene object to the collision / physics entity.
	/// </summary>
	void SyncToPhysicsEntity();

	/// <summary>
	/// Applies changes made on the collision / physics entity to the SunBurn scene object.
	/// </summary>
	void SyncToSceneObject();
}
