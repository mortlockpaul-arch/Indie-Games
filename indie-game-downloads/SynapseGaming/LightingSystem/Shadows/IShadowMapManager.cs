using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Interface that provides access to the scene's shadow map manager. The shadow map manager
/// provides methods for creating and caching scene shadow maps.
/// </summary>
public interface IShadowMapManager : IRenderableManager, IManagerService, IManager, IUnloadable
{
	/// <summary>
	/// Organizes the provided lights into shadow and render target groups.
	/// </summary>
	/// <param name="rendertargetgroups">Returned render target groups.</param>
	/// <param name="lights">Lights to organize.</param>
	/// <param name="usedefaultgrouping">Determines if ungrouped lights should be placed in a
	/// single default group (recommended: true for deferred rendering and false for forward).</param>
	void BuildShadows(List<ShadowRenderTargetGroup> rendertargetgroups, List<BaseLight> lights, bool usedefaultgrouping);
}
