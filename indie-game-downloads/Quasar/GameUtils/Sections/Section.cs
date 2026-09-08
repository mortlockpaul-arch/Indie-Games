using System;
using System.Collections.Generic;
using Quasar.Render;

namespace Quasar.GameUtils.Sections;

public abstract class Section : IDisposable
{
	protected List<Scene> scenes;

	protected List<RenderProcess> extraRenderProcesses;

	protected RenderPass mainRenderPass;

	private Scene defaultScene;

	private bool initialized;

	public RenderPass MainRenderPass => mainRenderPass;

	public List<RenderProcess> ExtraRenderProcesses => extraRenderProcesses;

	public Scene DefaultScene => defaultScene;

	public bool Initialized => initialized;

	public Section()
	{
		scenes = new List<Scene>();
		extraRenderProcesses = new List<RenderProcess>();
	}

	public virtual void InitSection()
	{
		if (!initialized)
		{
			initScenes();
			initRenderProcesses();
			initialized = true;
		}
	}

	protected abstract void initScenes();

	protected virtual void clearRenderProcesses()
	{
		extraRenderProcesses.Clear();
		mainRenderPass = null;
	}

	public void AddExtraRenderProcess(RenderProcess rp)
	{
		extraRenderProcesses.Add(rp);
	}

	protected virtual void initRenderProcesses()
	{
		RenderPass renderPass = new RenderPass2D(createRenderTarget: false);
		renderPass.addSource(DefaultScene);
		renderPass.MustClearColor = false;
		renderPass.MustClearDepth = false;
		mainRenderPass = renderPass;
	}

	public void AddScene(Scene s, bool isDefault)
	{
		if (isDefault)
		{
			defaultScene = s;
		}
		scenes.Add(s);
	}

	public virtual void UpdateScenes()
	{
		foreach (Scene scene in scenes)
		{
			scene.Update();
		}
	}

	public virtual void MainLoop()
	{
	}

	public virtual void Dispose()
	{
		if (scenes != null)
		{
			scenes.Clear();
		}
		scenes = null;
		defaultScene = null;
		extraRenderProcesses.Clear();
		extraRenderProcesses = null;
		mainRenderPass = null;
		GC.SuppressFinalize(this);
	}

	~Section()
	{
		Dispose();
	}
}
