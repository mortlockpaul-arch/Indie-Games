using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StarbeamDefenderGame.Game.AI;

namespace StarbeamDefenderGame;

public class AsteroidManager
{
	private int remainingasteroids;

	private float maxspeed;

	private float minspeed;

	private int spawnmin;

	private int spawnmax;

	private int nextspawn;

	private Asteroid[] asteroids;

	private Random randomiser;

	public Asteroid[] Asteroids => asteroids;

	public bool Completed
	{
		get
		{
			if (remainingasteroids == 0)
			{
				for (int i = 0; i < asteroids.Count(); i++)
				{
					if (asteroids[i].Active)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}

	public AsteroidManager()
	{
		remainingasteroids = 0;
		maxspeed = 0f;
		minspeed = 0f;
		randomiser = new Random();
		asteroids = new Asteroid[10];
		for (int i = 0; i < asteroids.Count(); i++)
		{
			asteroids[i] = new Asteroid();
		}
	}

	public void Setup(int asteroidcount, float maxspeed, float minspeed, int spawnrate)
	{
		remainingasteroids = asteroidcount;
		this.maxspeed = maxspeed;
		this.minspeed = minspeed;
		spawnmin = spawnrate / 5;
		spawnmax = spawnrate;
	}

	public void Update(int timems, ProjectileManager projectiles, Building[] buildings, AudioManager soundmanager, ExplosionManager explosions)
	{
		if (Completed)
		{
			return;
		}
		nextspawn -= timems;
		if (nextspawn < 0)
		{
			nextspawn = randomiser.Next(spawnmax - spawnmin) + spawnmin;
			if (remainingasteroids > 0)
			{
				for (int i = 0; i < asteroids.Count(); i++)
				{
					if (!asteroids[i].Active)
					{
						int speed = randomiser.Next((int)(maxspeed - minspeed)) + 1;
						asteroids[i].Create(speed, buildings);
						remainingasteroids--;
						break;
					}
				}
			}
		}
		for (int j = 0; j < asteroids.Count(); j++)
		{
			asteroids[j].Update(timems, projectiles, buildings, soundmanager, explosions);
		}
		CheckCollisions(projectiles, explosions);
	}

	public void CheckCollisions(ProjectileManager projectiles, ExplosionManager explosions)
	{
		CollisionSphere col = new CollisionSphere(50f, new Vector2(50f, 50f));
		CollisionSphere col2 = new CollisionSphere(4f, new Vector2(4f, 4f));
		for (int i = 0; i < projectiles.PlayerProjectiles.Count(); i++)
		{
			if (!projectiles.PlayerProjectiles[i].Active)
			{
				continue;
			}
			for (int j = 0; j < asteroids.Count(); j++)
			{
				if (asteroids[j].Active && CollisionSphere.Intersects(col, asteroids[j].Position, col2, projectiles.PlayerProjectiles[i].Position))
				{
					asteroids[j].Life -= 100;
					if (asteroids[j].Life < 1)
					{
						asteroids[j].Active = false;
						explosions.CreateGraphicalExplosion(asteroids[j].Position, new Vector2(128f, 128f));
						projectiles.PlayerProjectiles[i].Player.Score += asteroids[j].Score;
						projectiles.PlayerProjectiles[i].Active = false;
						break;
					}
					projectiles.PlayerProjectiles[i].Active = false;
				}
			}
		}
	}
}
