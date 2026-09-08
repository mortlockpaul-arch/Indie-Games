using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements;
using Quasar.Global;
using Quasar.Render.Sorters;

namespace Quasar.Render;

public class SceneRenderData
{
	public static SceneRenderData CurrentRenderData;

	public Scene Scene;

	public Camera Camera;

	public List<Light> CurrentLights = new List<Light>();

	public List<string> ActiveLayers = new List<string>();

	public Viewport? Viewport;

	public Sorter Sorter;

	public string ForcedTechnique;

	public Shader ForcedShader;

	private bool clearDepth;

	private List<string> renderLayers = new List<string>();

	private List<string> ignoreLayers = new List<string>();

	public bool HasForcedTechnique => ForcedTechnique != null;

	public bool HasForcedShader => ForcedShader != null;

	public bool ClearDepth
	{
		get
		{
			return clearDepth;
		}
		set
		{
			clearDepth = value;
		}
	}

	public List<string> RenderLayers => renderLayers;

	public List<string> IgnoreLayers => ignoreLayers;

	public bool CanRenderLayers()
	{
		if (renderLayers.Count == 0 || GameMath.Overlaps(renderLayers, ActiveLayers))
		{
			return !GameMath.Overlaps(ignoreLayers, ActiveLayers);
		}
		return false;
	}

	public bool CanRender(List<string> layers)
	{
		if (renderLayers.Count == 0 || GameMath.Overlaps(renderLayers, layers))
		{
			return !GameMath.Overlaps(ignoreLayers, layers);
		}
		return false;
	}

	public void SetRenderLayer(string layer)
	{
		renderLayers.Add(layer);
	}

	public void ClearRenderLayer(string layer)
	{
		renderLayers.Remove(layer);
	}

	public void SetIgnoreLayer(string layer)
	{
		ignoreLayers.Add(layer);
	}

	public void ClearIgnoreLayer(string layer)
	{
		ignoreLayers.Remove(layer);
	}

	public SceneRenderData(Scene scene, Camera camera)
	{
		Scene = scene;
		Camera = camera;
		Viewport = null;
		Sorter = AlphaSorter.Instance;
	}

	public SceneRenderData(Scene scene, Sorter sorter)
	{
		Scene = scene;
		Camera = null;
		Viewport = null;
		Sorter = sorter;
	}

	public SceneRenderData(Scene scene, Viewport viewport)
	{
		Scene = scene;
		Camera = null;
		Viewport = viewport;
		Sorter = AlphaSorter.Instance;
	}

	public SceneRenderData(Scene scene, Viewport viewport, Sorter sorter)
	{
		Scene = scene;
		Camera = null;
		Viewport = viewport;
		Sorter = sorter;
	}

	public SceneRenderData(Scene scene, Camera camera, Viewport viewport)
	{
		Scene = scene;
		Camera = camera;
		Viewport = viewport;
		Sorter = AlphaSorter.Instance;
	}

	public SceneRenderData(Scene scene, Camera camera, Sorter sorter)
	{
		Scene = scene;
		Camera = camera;
		Viewport = null;
		Sorter = sorter;
	}

	public SceneRenderData(Scene scene)
	{
		Scene = scene;
		Camera = null;
		Viewport = null;
		Sorter = AlphaSorter.Instance;
	}
}
