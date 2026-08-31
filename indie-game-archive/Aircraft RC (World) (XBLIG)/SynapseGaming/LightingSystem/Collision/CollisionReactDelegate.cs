using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Delegate used when two object collide.
/// </summary>
/// <param name="collider">The moving object.</param>
/// <param name="collidee">The object hit by the moving object.</param>
/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
/// <param name="collisionhandled">Determines if the collision was handled by a prior event hander.
/// If this value is true do NOT process any collision reaction code. If the event handler processes
/// collision reaction code set this value to true to avoid another handler or SunBurn's built-in
/// reaction code from processing.</param>
public delegate void CollisionReactDelegate(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled);
