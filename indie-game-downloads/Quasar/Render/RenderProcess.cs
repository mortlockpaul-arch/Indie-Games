using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Render;

public class RenderProcess : IDisposable
{
	protected List<RenderPass> renderPasses = new List<RenderPass>();

	protected bool enabled = true;

	protected RenderPass finalRenderPass;

	public RenderTarget2D RenderTarget
	{
		get
		{
			if (finalRenderPass == null)
			{
				return null;
			}
			if (!(finalRenderPass is RenderPass2D renderPass2D))
			{
				return null;
			}
			return renderPass2D.RenderTarget;
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

	public RenderPass FinalRenderPass => finalRenderPass;

	public void SetRenderTarget(RenderTarget2D target, bool setOwner)
	{
		if (finalRenderPass != null && finalRenderPass is RenderPass2D renderPass2D)
		{
			renderPass2D.SetRenderTarget(target, setOwner);
		}
	}

	public RenderProcess()
	{
		finalRenderPass = new RenderPass2D(createRenderTarget: false);
		finalRenderPass.MustClearColor = true;
		finalRenderPass.MustClearDepth = true;
	}

	public RenderProcess(Scene scene)
		: this()
	{
		addSource(scene);
	}

	public RenderProcess(RenderTarget2D renderTarget)
		: this()
	{
		((RenderPass2D)finalRenderPass).SetRenderTarget(renderTarget, setOwner: true);
	}

	public RenderProcess(RenderTarget2D renderTarget, Scene scene)
		: this(renderTarget)
	{
		addSource(scene);
	}

	public RenderProcess(RenderPass pass)
	{
		finalRenderPass = pass;
	}

	public RenderProcess(RenderPass pass, Scene source)
		: this(pass)
	{
		addSource(source);
	}

	public void addIntermediatePass(RenderPass pass)
	{
		renderPasses.Add(pass);
	}

	public virtual void Render()
	{
		if (enabled)
		{
			int count = renderPasses.Count;
			for (int i = 0; i < count; i++)
			{
				renderPasses[i].Render();
			}
			if (finalRenderPass != null)
			{
				finalRenderPass.Render();
			}
		}
	}

	public void addSource(Scene scene)
	{
		addSource(new SceneRenderData(scene));
	}

	public virtual void addSource(SceneRenderData scene)
	{
		if (finalRenderPass != null)
		{
			finalRenderPass.addSource(scene);
		}
	}

	public virtual void ClearPasses()
	{
		renderPasses.Clear();
	}

	~RenderProcess()
	{
		Dispose();
	}

	public virtual void Dispose()
	{
		if (renderPasses != null)
		{
			renderPasses.Clear();
			renderPasses = null;
		}
		if (finalRenderPass != null)
		{
			finalRenderPass = null;
		}
	}
}
