#define DEBUG
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Emitters;

namespace ProjectMercury.Renderers;

public abstract class Renderer : IDisposable
{
	public IGraphicsDeviceService GraphicsDeviceService;

	protected virtual void Dispose(bool disposing)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~Renderer()
	{
		Dispose(disposing: false);
	}

	public virtual void LoadContent(ContentManager content)
	{
	}

	public virtual void RenderEmitter(Emitter emitter)
	{
		Guard.ArgumentNull("emitter", emitter);
		Matrix transform = Matrix.Identity;
		RenderEmitter(emitter, ref transform);
	}

	public abstract void RenderEmitter(Emitter emitter, ref Matrix transform);

	public virtual void RenderEffect(ParticleEffect effect)
	{
		Guard.ArgumentNull("effect", effect);
		Matrix transform = Matrix.Identity;
		RenderEffect(effect, ref transform);
	}

	public virtual void RenderEffect(ParticleEffect effect, ref Matrix transform)
	{
		Guard.ArgumentNull("effect", effect);
		for (int i = 0; i < effect.Count; i++)
		{
			RenderEmitter(effect[i], ref transform);
		}
	}
}
