namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Defines the bounds used in object culling and collision.
/// </summary>
public enum HullType
{
	/// <summary>
	/// Bounds use a fitted sphere.
	/// </summary>
	Sphere,
	/// <summary>
	/// Bounds use a fitted box.
	/// </summary>
	Box,
	/// <summary>
	/// Bounds are calculated using mesh geometry.
	/// </summary>
	Mesh
}
