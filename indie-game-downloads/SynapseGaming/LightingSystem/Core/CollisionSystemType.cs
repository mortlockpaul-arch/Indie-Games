namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Determines the type of physics / collision to use.
/// </summary>
public enum CollisionSystemType
{
	/// <summary>
	/// Use full physics with terrain, character controller support, and more.
	/// </summary>
	Physics,
	/// <summary>
	/// Use the older legacy collision.
	/// </summary>
	LegacyBasicCollision
}
