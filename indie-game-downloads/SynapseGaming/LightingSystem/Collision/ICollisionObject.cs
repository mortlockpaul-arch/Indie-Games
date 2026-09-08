using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Interface used by objects that support collision.
/// </summary>
public interface ICollisionObject : ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	int CollisionId { get; }

	/// <summary>
	/// Determines if gravity will cause the object to fall. For an object to be affected
	/// by gravity its UpdateType must be Automatic and CollisionType must be Collide.
	/// </summary>
	bool AffectedByGravity { get; set; }

	/// <summary>
	/// Determines how an object interacts with the scene.
	/// </summary>
	CollisionType CollisionType { get; set; }

	/// <summary>
	/// Move helper used by this object to determine its momentum, next location, and sweep volume.
	/// </summary>
	ICollisionMove CollisionMove { get; set; }

	/// <summary>
	/// Default material used when collision surface does not implement material info.
	/// </summary>
	ICollisionMaterial DefaultCollisionMaterial { get; set; }

	/// <summary>
	/// Mass of the object.
	/// </summary>
	float Mass { get; set; }

	/// <summary>
	/// Inverse world space transform of the object.
	/// </summary>
	Matrix WorldToObject { get; }

	/// <summary>
	/// Event used to detect when the object collides with another object, or to
	/// override the default reaction behavior between objects.
	/// </summary>
	event CollisionReactDelegate CollisionReactEvent;

	/// <summary>
	/// Event used to detect when another object collides with this object, but only
	/// when this object's CollisionType is set to Trigger.
	///
	/// The event handler can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	event CollisionTriggerDelegate CollisionTriggerEvent;

	/// <summary>
	/// Used to trigger the CollisionReactEvent event when two object collide.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="collidee">The object hit by the moving object.</param>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	/// <param name="collisionhandled">Determines if the collision was handled by a prior event hander.
	/// If this value is true do NOT process any collision reaction code. If the event handler processes
	/// collision reaction code set this value to true to avoid another handler or SunBurn's built-in
	/// reaction code from processing.</param>
	void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled);

	/// <summary>
	/// Used to trigger the CollisionTriggerEvent event when an object passes through or overlaps a trigger.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger);
}
