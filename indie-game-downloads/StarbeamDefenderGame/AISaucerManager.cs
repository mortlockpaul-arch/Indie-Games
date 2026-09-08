using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class AISaucerManager
{
	private int remainingsaucers;

	private float maxspeed;

	private float minspeed;

	private int spawnmin;

	private int spawnmax;

	private int nextspawn;

	private MissileSaucer[] saucers;

	private Random randomiser;

	public MissileSaucer[] Saucers => saucers;

	public bool Completed
	{
		get
		{
			if (remainingsaucers == 0)
			{
				for (int i = 0; i < saucers.Count(); i++)
				{
					if (saucers[i].Active)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}

	public AISaucerManager()
	{
		remainingsaucers = 0;
		maxspeed = 0f;
		minspeed = 0f;
		randomiser = new Random();
		saucers = new MissileSaucer[10];
		for (int i = 0; i < saucers.Count(); i++)
		{
			saucers[i] = new MissileSaucer();
		}
	}

	public void Setup(int saucercount, float maxspeed, float minspeed, int spawnrate)
	{
		remainingsaucers = saucercount;
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
			if (remainingsaucers > 0)
			{
				for (int i = 0; i < saucers.Count(); i++)
				{
					if (!saucers[i].Active)
					{
						int num = randomiser.Next(1280);
						float speed = randomiser.Next((int)(maxspeed - minspeed)) + 1;
						Vector2 destination = new Vector2(randomiser.Next(800) + 280, randomiser.Next(100) + 400);
						saucers[i].Create(new Vector2(num, -64f), destination, speed, 100);
						remainingsaucers--;
						break;
					}
				}
			}
		}
		for (int j = 0; j < saucers.Count(); j++)
		{
			saucers[j].Update(timems, projectiles, buildings, soundmanager);
		}
		CheckCollisions(projectiles, explosions);
	}

	public void CheckCollisions(ProjectileManager projectiles, ExplosionManager explosions)
	{
		CollisionSphere col = new CollisionSphere(20f, new Vector2(25f, 25f));
		CollisionSphere col2 = new CollisionSphere(4f, new Vector2(4f, 4f));
		for (int i = 0; i < projectiles.PlayerProjectiles.Count(); i++)
		{
			if (!projectiles.PlayerProjectiles[i].Active)
			{
				continue;
			}
			for (int j = 0; j < saucers.Count(); j++)
			{
				if (saucers[j].Active && CollisionSphere.Intersects(col, saucers[j].Position, col2, projectiles.PlayerProjectiles[i].Position))
				{
					saucers[j].Active = false;
					explosions.CreateGraphicalExplosion(saucers[j].Position, new Vector2(50f, 50f));
					projectiles.PlayerProjectiles[i].Active = false;
					projectiles.PlayerProjectiles[i].Player.Score += saucers[j].Score;
					break;
				}
			}
		}
	}
}
