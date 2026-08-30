using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class ParticleSystem
{
	private Texture2D textureNoise;

	private Texture2D textureDots;

	private SoundEffect explosionSound;

	private Random random;

	public List<Particles> particles;

	public List<Particles> playerTrails;

	public List<Particles> enemyTrails;

	public List<Particles> itemTrails;

	public void Initialize(Texture2D textureNoise, Texture2D textureDots, SoundEffect explosionSound)
	{
		random = new Random();
		this.explosionSound = explosionSound;
		this.textureDots = textureDots;
		this.textureNoise = textureNoise;
		particles = new List<Particles>(50);
		playerTrails = new List<Particles>(200);
		enemyTrails = new List<Particles>(100);
		itemTrails = new List<Particles>(100);
	}

	public void Load()
	{
	}

	public void Update(GameTime gameTime)
	{
		UpdateParticles(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < particles.Count; i++)
		{
			particles[i].Draw(spriteBatch);
		}
	}

	public void DrawTrails(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < playerTrails.Count; i++)
		{
			playerTrails[i].Draw(spriteBatch);
		}
		for (int i = 0; i < enemyTrails.Count; i++)
		{
			enemyTrails[i].Draw(spriteBatch);
		}
	}

	public void DrawItemTrails(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < itemTrails.Count; i++)
		{
			itemTrails[i].Draw(spriteBatch);
		}
	}

	public void AddParticles(Vector2 pos, Texture2D texture, float rotation, Color color)
	{
		Particles particles = new Particles();
		int num = random.Next(10, 100);
		Vector2 vector = new Vector2(random.Next(-num, num), random.Next(-num, num));
		particles.Initialize(pos, pos + vector, texture, (float)random.Next(3, 10) * 0.03f, 0.1f, (float)random.Next(15, 60) / 5f, random.Next(6), color);
		particles.rotation = rotation;
		this.particles.Add(particles);
		particles = null;
	}

	public void AddTrails(Vector2 pos, Texture2D texture, float iniSize, float finalSize, float speed, float rotation, float rotationSpeed, Color color)
	{
		Particles particles = new Particles();
		particles.Initialize(pos, pos, texture, speed, iniSize, finalSize, rotation, color);
		particles.rotation = rotationSpeed;
		playerTrails.Add(particles);
		particles = null;
	}

	public void AddItemTrails(Vector2 posIni, Vector2 posFin, Texture2D texture, float iniSize, float finalSize, float speed, float rotation, float rotationSpeed, Color color)
	{
		Particles particles = new Particles();
		particles.Initialize(posIni, posFin, texture, speed, iniSize, finalSize, rotation, color);
		particles.rotation = rotationSpeed;
		itemTrails.Add(particles);
		particles = null;
	}

	public void UpdateParticles(GameTime gameTime)
	{
		for (int i = 0; i < particles.Count; i++)
		{
			particles[i].Update(gameTime);
			if ((float)(int)particles[i].color.A < 0.1f)
			{
				particles.RemoveAt(i);
			}
		}
		if (playerTrails.Count > 0)
		{
			for (int i = 0; i < playerTrails.Count; i++)
			{
				playerTrails[i].Update(gameTime);
				if ((float)(int)playerTrails[i].color.A < 0.1f)
				{
					playerTrails.RemoveAt(i);
				}
			}
		}
		if (enemyTrails.Count > 0)
		{
			for (int i = 0; i < enemyTrails.Count; i++)
			{
				enemyTrails[i].Update(gameTime);
				if ((float)(int)enemyTrails[i].color.A < 0.1f)
				{
					enemyTrails.RemoveAt(i);
				}
			}
		}
		if (itemTrails.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < itemTrails.Count; i++)
		{
			itemTrails[i].Update(gameTime);
			if ((float)(int)itemTrails[i].color.A < 0.1f)
			{
				itemTrails.RemoveAt(i);
			}
		}
	}

	public void createExplosion(Vector2 position, Vector2 screenPos, GraphicsDevice graphicsDevice, float volume)
	{
		float pan = MathHelper.Clamp(screenPos.X * 4f / (float)graphicsDevice.Viewport.Width - 2f, -1f, 1f);
		float volume2 = MathHelper.Clamp((0.03f + (float)random.Next(3) / 100f) * (volume / 100f), 0f, 1f);
		try
		{
			explosionSound.Play(volume2, (float)random.Next(-100, -50) / 100f, pan);
		}
		catch
		{
		}
		AddParticles(position, textureDots, 0f, new Color((float)random.Next(100, 200) / 50f + 1.5f, (float)random.Next(10, 20) / 50f + 0.1f, (float)random.Next(5) / 100f, (float)random.Next(200, 400) / 50f + 8f));
		AddParticles(position, textureDots, 0f, new Color((float)random.Next(100, 200) / 50f + 1.5f, (float)random.Next(10, 20) / 50f + 0.1f, (float)random.Next(5) / 100f, (float)random.Next(200, 400) / 50f + 8f));
		AddParticles(position, textureNoise, (float)random.Next(-50, 50) / 2000f, new Color((float)random.Next(25) / 50f + 0.25f, (float)random.Next(50, 75) / 50f + 0.75f, (float)random.Next(50, 100) / 50f + 1f, (float)random.Next(100, 200) / 50f + 8f));
		AddParticles(position, textureNoise, (float)random.Next(-50, 50) / 2000f, new Color((float)random.Next(25) / 50f + 0.25f, (float)random.Next(50, 75) / 50f + 0.75f, (float)random.Next(50, 100) / 50f + 1f, (float)random.Next(100, 200) / 50f + 8f));
	}
}
