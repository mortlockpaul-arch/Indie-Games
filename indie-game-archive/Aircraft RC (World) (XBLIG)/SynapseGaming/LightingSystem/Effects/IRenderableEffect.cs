using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with basic RenderManager compatibility.
/// </summary>
public interface IRenderableEffect
{
	/// <summary>
	/// World matrix applied to geometry using this effect.
	/// </summary>
	Matrix World { get; set; }

	/// <summary>
	/// View matrix applied to geometry using this effect.
	/// </summary>
	Matrix View { get; set; }

	/// <summary>
	/// Projection matrix applied to geometry using this effect.
	/// </summary>
	Matrix Projection { get; set; }

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	bool DoubleSided { get; set; }

	/// <summary>
	/// Applies the user's effect preference. This generally trades detail
	/// for performance based on the user's selection.
	/// </summary>
	DetailPreference EffectDetail { get; set; }

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance in effects that automatically generate an inverse
	/// world matrix when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	/// </summary>
	/// <param name="world">World matrix applied to geometry using this effect.</param>
	/// <param name="worldtoobj">Inverse world matrix applied to geometry using this effect.</param>
	void SetWorldAndWorldToObject(ref Matrix world, ref Matrix worldtoobj);

	/// <summary>
	/// Sets both the view, projection, and their inverse matrices.  Used to improve
	/// performance in effects that automatically generate an inverse
	/// matrix when the view and project are set, by providing a cached
	/// or precalculated inverse matrix with the view and project matrices.
	/// </summary>
	/// <param name="view">View matrix applied to geometry using this effect.</param>
	/// <param name="viewtoworld">Inverse view matrix applied to geometry using this effect.</param>
	/// <param name="projection">Projection matrix applied to geometry using this effect.</param>
	/// <param name="projectiontoview">Inverse projection matrix applied to geometry using this effect.</param>
	void SetViewAndProjection(Matrix view, Matrix viewtoworld, Matrix projection, Matrix projectiontoview);
}
