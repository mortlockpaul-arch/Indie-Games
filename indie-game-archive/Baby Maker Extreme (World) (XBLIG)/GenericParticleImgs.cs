using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Renderer;

public static class GenericParticleImgs
{
	private static List<SpriteImage> m_particle;

	public static void Initialize()
	{
		m_particle = new List<SpriteImage>();
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(5, 3, 96, 68)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(104, 9, 104, 92)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(221, 12, 80, 81)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(304, 3, 121, 113)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(0, 118, 101, 86)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(115, 103, 142, 136)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(258, 119, 120, 116)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(386, 134, 114, 116)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(13, 259, 148, 142)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(188, 263, 146, 148)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(350, 250, 162, 169)));
		m_particle.Add(SpriteManager.GetImage("images/particles", new Rectangle(23, 410, 100, 100)));
	}

	public static SpriteImage GetParticle()
	{
		return m_particle[(int)SceneRenderer.GetRand(0f, m_particle.Count)];
	}
}
