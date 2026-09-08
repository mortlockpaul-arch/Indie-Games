using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Determines how an object interacts with the scene.
/// </summary>
public enum CollisionType
{
	/// <summary>
	/// Object does not collide with the scene.
	/// </summary>
	[EditorEnumDescription("No Collisions")]
	None,
	/// <summary>
	/// Object does collide with the scene.
	/// </summary>
	[EditorEnumDescription("Solid Object")]
	Collide,
	/// <summary>
	/// Object does not collide with the scene, but will trigger events as other objects pass through it.
	/// </summary>
	[EditorEnumDescription("Trigger")]
	Trigger
}
