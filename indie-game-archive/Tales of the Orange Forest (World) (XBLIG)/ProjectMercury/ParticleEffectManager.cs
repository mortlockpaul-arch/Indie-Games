#define DEBUG
using System.Collections.Generic;
using ProjectMercury.Renderers;
using ProjectMercury.Threading;
using Parallel = ProjectMercury.Threading.Parallel;

namespace ProjectMercury;

public class ParticleEffectManager : List<ParticleEffect>
{
	public Renderer Renderer { get; set; }

	public int ActiveParticlesCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < base.Count; i++)
			{
				num += base[i].ActiveParticlesCount;
			}
			return num;
		}
	}

	public ParticleEffectManager(Renderer renderer)
		: base(20)
	{
		Guard.ArgumentNull("renderer", renderer);
		Renderer = renderer;
	}

	public void Update(float deltaSeconds, bool multithreaded)
	{
		if (!multithreaded)
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Update(deltaSeconds);
			}
		}
		else if (base.Count > 0)
		{
			Parallel.For(0, base.Count, delegate(int index)
			{
				base[index].Update(deltaSeconds);
			});
		}
	}

	public void Draw()
	{
		for (int i = 0; i < base.Count; i++)
		{
			Renderer.RenderEffect(base[i]);
		}
	}
}
