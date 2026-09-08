using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with texture addressing support.
/// </summary>
public interface IAddressableEffect
{
	/// <summary>
	/// Determines the effect's texture address mode in the U texture-space direction.
	/// </summary>
	TextureAddressMode AddressModeU { get; set; }

	/// <summary>
	/// Determines the effect's texture address mode in the V texture-space direction.
	/// </summary>
	TextureAddressMode AddressModeV { get; set; }

	/// <summary>
	/// Determines the effect's texture address mode in the W texture-space direction.
	/// </summary>
	TextureAddressMode AddressModeW { get; set; }
}
