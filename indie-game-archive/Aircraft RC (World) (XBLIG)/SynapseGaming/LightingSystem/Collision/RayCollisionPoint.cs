using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Provides the state and information for a ray intersection with a collidable object.
/// </summary>
public struct RayCollisionPoint
{
	/// <summary>
	/// Normalized time (from 0.0f to 1.0f) to the closest collision.
	/// </summary>
	public float ContactTime;

	/// <summary>
	/// Contact point of the closest collision.
	/// </summary>
	public Vector3 ContactPoint;

	/// <summary>
	/// Surface normal of the closest collision.
	/// </summary>
	public Vector3 SurfaceNormal;

	/// <summary>
	/// Collidee object of the closest collision.
	/// </summary>
	public ICollisionObject ContactObject;

	/// <summary>
	/// Collision material of the closest collision.
	/// </summary>
	public ICollisionMaterial Material;
}
