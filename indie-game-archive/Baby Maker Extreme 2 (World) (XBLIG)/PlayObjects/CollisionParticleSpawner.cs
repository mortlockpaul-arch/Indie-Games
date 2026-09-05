using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Renderer;
using Screens;

namespace PlayObjects;

public class CollisionParticleSpawner
{
	private const int NUM_PARTICLESPAWNERS = 2;

	private List<Vector2> m_Velocities;

	private List<ParticleEmitter> m_particleEmitters;

	private Vector2 m_vGravity;

	private int m_iLifeTime;

	private RenderLight m_light;

	public CollisionParticleSpawner(bool isVirtualObj)
	{
		m_iLifeTime = 0;
		m_Velocities = new List<Vector2>();
		m_particleEmitters = new List<ParticleEmitter>();
		SpriteImage image = TextureContainer.GetImage("images/particle");
		m_vGravity = new Vector2(0f, 600f);
		m_light = new RenderLight(default(Vector3), 1f, 600, Color.Cyan);
		if (isVirtualObj)
		{
			m_Velocities.Add(default(Vector2));
			m_particleEmitters.Add(new ParticleEmitter(image, default(Vector2), 100f, fades: true, additive: true, Color.Lime, Color.Lime, Color.Green, Color.Green, new Vector2(0f, 300f), -0.1f, default(Vector2), 800, 2000, 300f, 700f, (float)Math.PI, 1f, default(Vector2), 20f, 50f, 60f, (float)Math.PI, 100));
			m_Velocities.Add(default(Vector2));
			m_particleEmitters.Add(new ParticleEmitter(image, default(Vector2), 100f, fades: true, additive: true, Color.Lime, Color.Lime, Color.Lime, Color.Lime, new Vector2(0f, 300f), -0.1f, default(Vector2), 1000, 2300, 500f, 1000f, (float)Math.PI, 2.1991148f, default(Vector2), 1f, 23f, 35f, (float)Math.PI, 100));
		}
		else
		{
			m_Velocities.Add(default(Vector2));
			m_particleEmitters.Add(new ParticleEmitter(image, default(Vector2), 100f, fades: true, additive: true, Color.Blue, Color.Red, Color.Purple, Color.Red, new Vector2(0f, 300f), -0.1f, default(Vector2), 800, 2000, 300f, 700f, (float)Math.PI, 1f, default(Vector2), 30f, 100f, 170f, (float)Math.PI, 100));
			m_Velocities.Add(default(Vector2));
			m_particleEmitters.Add(new ParticleEmitter(image, default(Vector2), 100f, fades: true, additive: true, Color.Yellow, Color.Red, Color.Yellow, Color.Red, new Vector2(0f, 300f), -0.1f, default(Vector2), 1000, 2300, 500f, 1000f, (float)Math.PI, 2.1991148f, default(Vector2), 1f, 30f, 60f, (float)Math.PI, 100));
		}
	}

	public void Initialize(Vector2 pos)
	{
		m_iLifeTime = 5000;
		for (int i = 0; i < 2; i++)
		{
			float rand = SceneRenderer.GetRand(0f, (float)Math.PI * 2f);
			float rand2 = SceneRenderer.GetRand(300f, 500f);
			m_Velocities[i] = rand2 * new Vector2((float)(0.0 - Math.Cos(rand)), (float)Math.Sin(rand));
			m_particleEmitters[i].Position = pos;
			m_particleEmitters[i].CreateBurst(20 * (i + 1));
		}
		m_light.pos = new Vector3(pos.X, 0f - pos.Y, 300f);
	}

	public void Update(TimeTracker gameTime)
	{
		if (m_iLifeTime > 0)
		{
			m_iLifeTime -= gameTime.ElapsedMilli;
			m_light.color.A = (byte)(255f * ((float)m_iLifeTime / 5000f));
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		if (m_iLifeTime > 0)
		{
			m_light.Draw(gameTime);
		}
	}
}
