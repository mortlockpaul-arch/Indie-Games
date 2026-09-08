using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with transparency support.
/// </summary>
public interface ITransparentEffect
{
	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	TransparencyMode TransparencyMode { get; }

	/// <summary>
	/// Used with TransparencyMode to determine the effect clipped transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the *shadow*
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.
	/// </summary>
	float TransparencyThreshold { get; }

	/// <summary>
	/// The texture map used for transparency (values are pulled from the alpha channel).
	/// </summary>
	Texture TransparencyMap { get; }

	/// <summary>
	/// Sets all transparency information at once.  Used to improve performance
	/// by avoiding multiple effect technique changes.
	/// </summary>
	/// <param name="mode">The transparency style used when rendering the effect.</param>
	/// <param name="threshold">Used with TransparencyMode to determine the effect transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the shadow
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.</param>
	/// <param name="map">The texture map used for transparency (values are pulled from the alpha channel).</param>
	void SetTransparencyModeAndMap(TransparencyMode mode, float threshold, Texture map);
}
