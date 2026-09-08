using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements;
using Quasar.Global;

namespace Quasar.Render;

public abstract class RenderPass : IDisposable
{
	protected List<SceneRenderData> sources = new List<SceneRenderData>();

	private Color backgroundColor = new Color(1f, 0f, 1f, 1f);

	private bool mustclearColor;

	private bool mustclearDepth;

	private bool enabled = true;

	public Color BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			backgroundColor = value;
		}
	}

	public bool MustClearColor
	{
		get
		{
			return mustclearColor;
		}
		set
		{
			mustclearColor = value;
		}
	}

	public bool MustClearDepth
	{
		get
		{
			return mustclearDepth;
		}
		set
		{
			mustclearDepth = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public void addSource(Scene scene)
	{
		sources.Add(new SceneRenderData(scene));
	}

	public void removeSource(Scene scene)
	{
		for (int i = 0; i < sources.Count; i++)
		{
			SceneRenderData sceneRenderData = sources[i];
			if (sceneRenderData.Scene == scene)
			{
				sources.RemoveAt(i);
				break;
			}
		}
	}

	public void insertSource(int index, Scene scene)
	{
		sources.Insert(index, new SceneRenderData(scene));
	}

	public void Clear()
	{
		sources.Clear();
	}

	public void addSource(SceneRenderData sceneRender)
	{
		sources.Add(sceneRender);
	}

	public void addSource(Scene scene, Camera camera)
	{
		sources.Add(new SceneRenderData(scene, camera));
	}

	public void addSource(Scene scene, Sorter sorter)
	{
		sources.Add(new SceneRenderData(scene, sorter));
	}

	public void insertSource(int index, SceneRenderData sceneRender)
	{
		sources.Insert(index, sceneRender);
	}

	public void insertSource(int index, Scene scene, Camera camera)
	{
		sources.Insert(index, new SceneRenderData(scene, camera));
	}

	protected abstract void SetRenderTarget();

	public virtual void Render()
	{
		if (!enabled)
		{
			return;
		}
		SetRenderTarget();
		if (mustclearColor | mustclearDepth)
		{
			Engine.Device.Clear((ClearOptions)((mustclearColor ? 1 : 0) | (mustclearDepth ? 2 : 0)), BackgroundColor, 1f, 0);
		}
		int count = sources.Count;
		for (int i = 0; i < count; i++)
		{
			SceneRenderData sceneRenderData = sources[i];
			if (sceneRenderData.Scene == null)
			{
				continue;
			}
			if (sceneRenderData.Viewport.HasValue)
			{
				Engine.Device.Viewport = sceneRenderData.Viewport.Value;
			}
			else
			{
				Engine.Device.Viewport = Engine.DefaultViewport;
			}
			SceneRenderData.CurrentRenderData = sceneRenderData;
			if (i > 0 && sceneRenderData.ClearDepth)
			{
				Engine.Device.Clear(ClearOptions.DepthBuffer, BackgroundColor, 1f, 0);
			}
			Camera camera = sceneRenderData.Camera;
			if (sceneRenderData.Camera == null)
			{
				sceneRenderData.Camera = sceneRenderData.Scene.Camera;
			}
			foreach (Light defaultLight in sceneRenderData.Scene.DefaultLights)
			{
				SceneRenderData.CurrentRenderData.CurrentLights.Add(defaultLight);
			}
			sceneRenderData.Sorter.BeginRender();
			sceneRenderData.Scene.Render();
			sceneRenderData.Sorter.EndRender();
			if (sceneRenderData.Scene.DefaultLights.Count > 0)
			{
				int index = SceneRenderData.CurrentRenderData.CurrentLights.Count - sceneRenderData.Scene.DefaultLights.Count;
				SceneRenderData.CurrentRenderData.CurrentLights.RemoveRange(index, sceneRenderData.Scene.DefaultLights.Count);
			}
			sceneRenderData.Camera = camera;
		}
		SceneRenderData.CurrentRenderData = null;
	}

	public virtual void Dispose()
	{
		sources.Clear();
		GC.SuppressFinalize(this);
	}

	~RenderPass()
	{
		Dispose();
	}
}
