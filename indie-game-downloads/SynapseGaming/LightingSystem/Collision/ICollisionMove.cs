using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Interface used by classes that implement collision movement
/// and applying force to a collision object.
/// </summary>
public interface ICollisionMove
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
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	void ApplyObjectForce(Vector3 objectforce);

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="objectposition">Object-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	void ApplyObjectForce(Vector3 objectposition, Vector3 objectforce);

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	/// <param name="constantforce">Determines if the force is from a constant
	/// source such as gravity, wind, or similar (eg: applied by the caller
	/// every frame instead of a single time).</param>
	void ApplyWorldForce(Vector3 worldforce, bool constantforce);

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldposition">World-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	void ApplyWorldForce(Vector3 worldposition, Vector3 worldforce);

	/// <summary>
	/// Removes all accumulated forces acting on the object. This will halt the object
	/// movement, however future forces (such as gravity) can immediately begin acting
	/// on the object again.
	/// </summary>
	void RemoveForces();

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnSubmittedToManager(IManagerService manager);

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnRemovedFromManager(IManagerService manager);
}
