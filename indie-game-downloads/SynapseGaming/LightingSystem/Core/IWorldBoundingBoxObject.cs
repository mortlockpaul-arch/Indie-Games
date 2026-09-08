using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by objects that occupy spatial volume.
/// </summary>
public interface IWorldBoundingBoxObject
{
	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	BoundingBox WorldBoundingBox { get; }
}
