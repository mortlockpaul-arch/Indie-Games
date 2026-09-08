using System.Collections.Generic;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with RenderManager lighting support.
/// </summary>
public interface ILightingEffect
{
	/// <summary>
	/// Maximum number of light sources the effect supports.
	/// </summary>
	int MaxLightSources { get; }

	/// <summary>
	/// Light sources that apply lighting to the effect during rendering.
	/// </summary>
	List<BaseLight> LightSources { set; }
}
