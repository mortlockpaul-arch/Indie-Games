using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Represents approximate lighting packed into a single directional and ambient light for
/// fast single-pass lighting.
/// </summary>
public struct CompositeLighting
{
	/// <summary>
	/// Represents an unlit composite lighting. This field is read-only.
	/// </summary>
	public static readonly CompositeLighting EmptyLighting = default(CompositeLighting);

	/// <summary>
	/// Direction in world space of the light's influence.
	/// </summary>
	public Vector3 Direction;

	/// <summary>
	/// Directional lighting color given off by the light.
	/// </summary>
	public Vector3 DiffuseColor;

	/// <summary>
	/// Ambient lighting color given off by the light.
	/// </summary>
	public Vector3 AmbientColor;

	/// <summary>
	/// Determines if the supplied composite lighting contains the same lighting values.
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(CompositeLighting other)
	{
		if (Direction.Equals(other.Direction) && DiffuseColor.Equals(other.DiffuseColor))
		{
			return AmbientColor.Equals(other.AmbientColor);
		}
		return false;
	}
}
