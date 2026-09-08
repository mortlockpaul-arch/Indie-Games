using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Tracks the state and information for the closest collision to the collider.
/// </summary>
public class CollisionPoint
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

	/// <summary>
	/// List of trigger objects the collider interacted with during the move time period.
	/// </summary>
	public List<ICollisionObject> Triggers = new List<ICollisionObject>(16);

	/// <summary>
	/// Resets the contact information.
	/// </summary>
	public void Clear()
	{
		ContactTime = 1f;
		Triggers.Clear();
	}
}
