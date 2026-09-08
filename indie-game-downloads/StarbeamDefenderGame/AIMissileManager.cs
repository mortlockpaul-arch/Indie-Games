using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class AIMissileManager
{
	private int remainingmissiles;

	private float maxspeed;

	private float minspeed;

	private int spawnmin;

	private int spawnmax;

	private int nextspawn;

	private Random randomiser;

	public bool Completed
	{
		get
		{
			if (remainingmissiles == 0)
			{
				return true;
			}
			return false;
		}
	}

	public AIMissileManager()
	{
		remainingmissiles = 0;
		maxspeed = 0f;
		minspeed = 0f;
		randomiser = new Random();
	}

	public void Setup(int missilecount, float maxspeed, float minspeed, int spawnrate)
	{
		remainingmissiles = missilecount;
		this.maxspeed = maxspeed;
		this.minspeed = minspeed;
		spawnmin = spawnrate / 5;
		spawnmax = spawnrate;
	}

	public void Update(int timems, MissileManager missilemanager, Building[] buildings)
	{
		if (Completed)
		{
			return;
		}
		nextspawn -= timems;
		if (nextspawn < 0)
		{
			nextspawn = randomiser.Next(spawnmax - spawnmin) + spawnmin;
			if (remainingmissiles > 0)
			{
				int num = randomiser.Next(1280);
				int num2 = randomiser.Next(buildings.Count());
				float speed = minspeed + (float)randomiser.Next((int)(maxspeed - minspeed));
				missilemanager.FireEnemyMissile(new Vector2(num, 0f), buildings[num2].Position + buildings[num2].Size / 2f, 50, speed, buildings[num2]);
				remainingmissiles--;
			}
		}
	}
}
