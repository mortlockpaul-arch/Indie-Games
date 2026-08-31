namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Interface that provides materials with physics properties.
/// </summary>
public interface ICollisionMaterial
{
	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	int CollisionId { get; }

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	float Elasticity { get; set; }

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	float Friction { get; set; }
}
