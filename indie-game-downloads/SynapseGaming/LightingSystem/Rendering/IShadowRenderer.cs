using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface used by objects that perform custom rendering during shadow map generation.
/// </summary>
public interface IShadowRenderer
{
	/// <summary>
	/// Prepares for shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	void BeginShadowGroupRendering(ShadowGroup shadowgroup);

	/// <summary>
	/// Performs shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	/// <param name="surface"></param>
	/// <param name="shadoweffect"></param>
	/// <returns></returns>
	bool RenderToShadowMapSurface(ShadowGroup shadowgroup, ShadowMapSurface surface, Effect shadoweffect);

	/// <summary>
	/// Finalizes shadow map rendering.
	/// </summary>
	/// <param name="shadowgroup"></param>
	void EndShadowGroupRendering(ShadowGroup shadowgroup);
}
