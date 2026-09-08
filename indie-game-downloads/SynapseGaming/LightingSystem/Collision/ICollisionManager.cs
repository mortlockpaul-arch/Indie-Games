using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Interface that provides access to the scene's collision manager. The collision manager
/// provides automatic collision and physics for scene objects.
/// </summary>
public interface ICollisionManager : IManagerService, IRenderableManager, IUpdatableManager, IManager, IUnloadable
{
	/// <summary>
	/// Casts a ray into the scene and returns the first intersected collidable object.
	/// </summary>
	/// <param name="startposition">World space start position of the ray.</param>
	/// <param name="endposition">World space end position of the ray.</param>
	/// <param name="firsthit">Output intersection information.</param>
	/// <returns>Returns true if an intersection occurs.</returns>
	bool RayCast(Vector3 startposition, Vector3 endposition, out RayCollisionPoint firsthit);

	/// <summary>
	/// Casts a ray into the scene and returns all intersected collidable object.
	/// </summary>
	/// <param name="hits">Resulting intersection information.</param>
	/// <param name="startposition">World space start position of the ray.</param>
	/// <param name="endposition">World space end position of the ray.</param>
	void RayCast(List<RayCollisionPoint> hits, Vector3 startposition, Vector3 endposition);

	/// <summary>
	/// Casts a ray into the scene and returns the first intersected collidable object.
	/// </summary>
	/// <param name="ray">Normalized world space ray.</param>
	/// <param name="castdistance">Distance to cast ray.</param>
	/// <param name="firsthit">Output intersection information.</param>
	/// <returns>Returns true if an intersection occurs.</returns>
	bool RayCast(Ray ray, float castdistance, out RayCollisionPoint firsthit);

	/// <summary>
	/// Casts a ray into the scene and returns all intersected collidable object.
	/// </summary>
	/// <param name="hits">Resulting intersection information.</param>
	/// <param name="ray">Normalized world space ray.</param>
	/// <param name="castdistance">Distance to cast ray.</param>
	void RayCast(List<RayCollisionPoint> hits, Ray ray, float castdistance);
}
