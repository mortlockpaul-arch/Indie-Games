using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides effects with terrain rendering support.
/// </summary>
public interface ITerrainEffect
{
	/// <summary>
	/// Texture containing height values used to displace a terrain mesh. Also used
	/// for low frequency lighting.
	/// </summary>
	Texture2D HeightMapTexture { get; set; }

	/// <summary>
	/// Adjusts the terrain displacement magnitude.
	/// </summary>
	float HeightScale { get; set; }

	/// <summary>
	/// Adjusts the number of times the height map tiles across a terrain's
	/// mesh. Similar to uv scale when texture mapping.
	/// </summary>
	float Tiling { get; set; }

	/// <summary>
	/// Density or tessellation of the terrain mesh.
	/// </summary>
	int MeshSegments { get; set; }

	/// <summary>
	/// Determines the number of times the height map tiles before the terrain ends.
	/// </summary>
	int TileRepeatCount { get; set; }
}
