using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides access to the scene's light map manager. The light map manager
/// provides methods for generating, storing, and retrieving light maps.
/// </summary>
public interface ILightMapManager : IManagerService, IManager, IUnloadable
{
	/// <summary>
	/// Gets the light map associated with the provided mesh or a default light map if non exists.
	/// </summary>
	/// <param name="mesh"></param>
	/// <returns></returns>
	LightMap GetLightMap(RenderableMesh mesh);

	/// <summary>
	/// Gets the light occlusion buffer associated with the provided light or null if non exists.
	/// </summary>
	/// <param name="light"></param>
	/// <returns></returns>
	LightOcclusionBuffer GetLightOcclusionBuffer(ILight light);
}
