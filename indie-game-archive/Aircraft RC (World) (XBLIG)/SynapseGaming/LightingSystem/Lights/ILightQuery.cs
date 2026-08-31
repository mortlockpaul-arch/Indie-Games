using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides an interface for retrieving lights contained in a volume from a scenegraph.
/// </summary>
public interface ILightQuery : IQuery<BaseLight>
{
	/// <summary>
	/// Generates approximate lighting for an area in world space. The returned composite
	/// lighting is packed into a single directional and ambient light for fast single-pass lighting.
	///
	/// Note: because this information is approximated smaller world space areas will
	/// result in more accurate lighting. Also the approximation is calculated on the
	/// cpu and cannot take into account shadowing.
	/// </summary>
	/// <param name="worldbounds">Bounding area used to determine approximate lighting.</param>
	/// <param name="ambientblend">Blending value (0.0f - 1.0f) that determines how much approximate lighting
	/// contributes to ambient lighting. Approximate lighting can create highly directional lighting, using
	/// a higher blending value can create softer, more realistic lighting.</param>
	/// <param name="lightingtype">Light types to include in approximate lighting.</param>
	/// <returns>Composite lighting packed into a single directional and ambient light.</returns>
	CompositeLighting GetCompositeLighting(BoundingBox worldbounds, float ambientblend, LightingType lightingtype);

	/// <summary>
	/// Generates approximate lighting for an area in world space. The returned composite
	/// lighting is packed into a single directional and ambient light for fast single-pass lighting.
	///
	/// Note: because this information is approximated smaller world space areas will
	/// result in more accurate lighting. Also the approximation is calculated on the
	/// cpu and cannot take into account shadowing.
	/// </summary>
	/// <param name="worldbounds">Bounding area used to determine approximate lighting.</param>
	/// <param name="ambientblend">Blending value (0.0f - 1.0f) that determines how much approximate lighting
	/// contributes to ambient lighting. Approximate lighting can create highly directional lighting, using
	/// a higher blending value can create softer, more realistic lighting.</param>
	/// <param name="lightingtype">Light types to include in approximate lighting.</param>
	/// <param name="compositelighting">Composite lighting packed into a single directional and ambient light.</param>
	void GetCompositeLighting(ref BoundingBox worldbounds, float ambientblend, LightingType lightingtype, out CompositeLighting compositelighting);

	/// <summary>
	/// Generates approximate lighting for an area in world space using a custom set of lights.
	/// The returned composite lighting is packed into a single directional and ambient light for
	/// fast single-pass lighting.
	///
	/// Note: because this information is approximated smaller world space areas will
	/// result in more accurate lighting. Also the approximation is calculated on the
	/// cpu and cannot take into account shadowing.
	/// </summary>
	/// <param name="sourcelights">Lights used to generate approximate lighting.</param>
	/// <param name="worldbounds">Bounding area used to determine approximate lighting.</param>
	/// <param name="ambientblend">Blending value (0.0f - 1.0f) that determines how much approximate lighting
	/// contributes to ambient lighting. Approximate lighting can create highly directional lighting, using
	/// a higher blending value can create softer, more realistic lighting.</param>
	/// <returns>Composite lighting packed into a single directional and ambient light.</returns>
	CompositeLighting GetCompositeLighting(List<BaseLight> sourcelights, BoundingBox worldbounds, float ambientblend);

	/// <summary>
	/// Generates approximate lighting for an area in world space using a custom set of lights.
	/// The returned composite lighting is packed into a single directional and ambient light for
	/// fast single-pass lighting.
	///
	/// Note: because this information is approximated smaller world space areas will
	/// result in more accurate lighting. Also the approximation is calculated on the
	/// cpu and cannot take into account shadowing.
	/// </summary>
	/// <param name="sourcelights">Lights used to generate approximate lighting.</param>
	/// <param name="worldbounds">Bounding area used to determine approximate lighting.</param>
	/// <param name="ambientblend">Blending value (0.0f - 1.0f) that determines how much approximate lighting
	/// contributes to ambient lighting. Approximate lighting can create highly directional lighting, using
	/// a higher blending value can create softer, more realistic lighting.</param>
	/// <param name="compositelighting">Composite lighting packed into a single directional and ambient light.</param>
	void GetCompositeLighting(List<BaseLight> sourcelights, ref BoundingBox worldbounds, float ambientblend, out CompositeLighting compositelighting);
}
