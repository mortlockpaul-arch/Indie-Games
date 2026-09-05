using System;
using System.Collections.Generic;
using MathTools;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class ParticleRect
{
	private List<ParticleEmitter> m_emitters;

	public ParticleRect(Rectangle r, int numEmitters, int spawnRate, float speed1, float speed2, float depth)
	{
		int num = (r.Width + r.Height) * 2;
		SpriteImage image = TextureContainer.GetImage("images/particle");
		m_emitters = new List<ParticleEmitter>();
		for (int i = 0; i < numEmitters; i++)
		{
			float num2 = (float)i / (float)numEmitters * (float)num;
			float num3 = 0f;
			Vector2 vector = default(Vector2);
			vector = ((!(num2 < (float)r.Width)) ? ((!(num2 < (float)(r.Width + r.Height))) ? ((!(num2 < (float)(r.Width + r.Height + r.Width))) ? new Vector2(r.Left, (float)r.Bottom - (num2 - (float)r.Width - (float)r.Height - (float)r.Width)) : new Vector2((float)r.Right - (num2 - (float)r.Width - (float)r.Height), r.Bottom)) : new Vector2(r.Right, (float)r.Top + (num2 - (float)r.Width))) : new Vector2((float)r.Left + num2, r.Top));
			Vector2 vec = vector - new Vector2(r.Center.X, r.Center.Y);
			vec.Y = 0f - vec.Y;
			num3 = (float)Math.PI + VectorTools.GetAngleFromVector(vec);
			m_emitters.Add(new ParticleEmitter(image, vector, depth, fades: true, additive: true, Color.Red, Color.Yellow, Color.Red, Color.Yellow, new Vector2(0f, 200f), -0.1f, default(Vector2), 300, 1000, speed1, speed2, num3, 1.5f, default(Vector2), 10f, 60f, 100f, 0f, spawnRate));
		}
	}

	public void Update(TimeTracker gameTime)
	{
		for (int i = 0; i < m_emitters.Count; i++)
		{
			m_emitters[i].Update(gameTime);
		}
	}
}
