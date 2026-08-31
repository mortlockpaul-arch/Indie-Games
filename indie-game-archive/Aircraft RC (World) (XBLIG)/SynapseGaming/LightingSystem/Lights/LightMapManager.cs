using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Manager used for generating, storing, and retrieving light maps.
/// </summary>
public class LightMapManager : BaseLightMapManager
{
	private LightMap HCB;

	/// <summary>
	/// Creates a new LightMapManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public LightMapManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public override void Unload()
	{
		HCB = null;
		base.Unload();
	}

	private ContentRepository H_0010(RenderableMesh P_0)
	{
		if (P_0.HC_0002 == null)
		{
			return null;
		}
		ContentRepository contentRepository = ContentRepository.Find(P_0.HC_0002.ModelAsset.ContentRepositoryName);
		if (contentRepository != null)
		{
			return contentRepository;
		}
		return ContentRepository.DefaultContentRepository;
	}

	private ContentRepository H_0014(ILight P_0)
	{
		return ContentRepository.DefaultContentRepository;
	}

	/// <summary>
	/// Set the light map associated with the provided renderable mesh unique id.
	/// </summary>
	/// <param name="meshuniqueid"></param>
	/// <param name="lightmap"></param>
	public override void SetLightMap(int meshuniqueid, LightMap lightmap)
	{
		if (lightmap != null)
		{
			if (base.MeshLightMaps.TryGetValue(meshuniqueid, out var value) && value != null && value != HCB)
			{
				base.OwnerSceneInterface.GetManager<IResourceManager>(required: false)?.AssignOwnership(value);
			}
			base.MeshLightMaps[meshuniqueid] = lightmap;
		}
	}

	/// <summary>
	/// Gets the light map associated with the provided mesh or a default light map if non exists.
	/// </summary>
	/// <param name="mesh"></param>
	/// <returns></returns>
	public override LightMap GetLightMap(RenderableMesh mesh)
	{
		if (base.MeshLightMaps.TryGetValue(mesh.HC_0012, out var value))
		{
			return value;
		}
		ContentRepository contentRepository = H_0010(mesh);
		if (contentRepository != null)
		{
			value = contentRepository.LoadLightMap(mesh);
			base.MeshLightMaps.Add(mesh.HC_0012, value);
			return value;
		}
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		if (HCB == null && graphicsDevice != null)
		{
			Texture2D texture2D = SunBurnCoreSystem.Instance._0002l("Black");
			HCB = new LightMap(texture2D, texture2D);
		}
		value = HCB;
		base.MeshLightMaps.Add(mesh.HC_0012, value);
		return value;
	}

	/// <summary>
	/// Gets the light occlusion buffer associated with the provided light or null if non exists.
	/// </summary>
	/// <param name="light"></param>
	/// <returns></returns>
	public override LightOcclusionBuffer GetLightOcclusionBuffer(ILight light)
	{
		int uniqueId = light.UniqueId;
		if (base.LightOcclusionBuffers.TryGetValue(uniqueId, out var value))
		{
			return value;
		}
		ContentRepository contentRepository = H_0014(light);
		if (contentRepository != null)
		{
			value = contentRepository.LoadLightOcclusionBuffer(light);
			base.LightOcclusionBuffers.Add(uniqueId, value);
			return value;
		}
		return null;
	}
}
