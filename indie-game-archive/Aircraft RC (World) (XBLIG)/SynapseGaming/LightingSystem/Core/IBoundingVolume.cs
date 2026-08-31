using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used to provide a bounding box
/// and sphere for an object.
/// </summary>
public interface IBoundingVolume
{
	/// <summary>
	/// Bounding area that completely contains the associated object.
	/// </summary>
	BoundingBox BoundingBox { get; }

	/// <summary>
	/// Bounding area that completely contains the associated object.
	/// </summary>
	BoundingSphere BoundingSphere { get; }
}
