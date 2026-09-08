using StarbeamDefenderGame.Game.AI;

namespace StarbeamDefenderGame;

public class AIManager
{
	private AISaucerManager saucermanager;

	private AIMissileManager missilemanager;

	private AsteroidManager asteroidmanager;

	private float playerscountmod;

	private int round;

	private int difficultyrating;

	public int PlayersCount
	{
		set
		{
			playerscountmod = 1f + (float)(value - 1) * 0.5f;
		}
	}

	public MissileSaucer[] missilesaucers => saucermanager.Saucers;

	public Asteroid[] Asteroids => asteroidmanager.Asteroids;

	public int RoundNumber => round + 1;

	public int Difficulty
	{
		get
		{
			return difficultyrating;
		}
		set
		{
			difficultyrating = value;
		}
	}

	public AIManager()
	{
		missilemanager = new AIMissileManager();
		saucermanager = new AISaucerManager();
		asteroidmanager = new AsteroidManager();
	}

	public void Update(int timems, MissileManager missiles, Building[] buildings, ProjectileManager projectiles, AudioManager soundmanager, ExplosionManager explosions)
	{
		missilemanager.Update(timems, missiles, buildings);
		saucermanager.Update(timems, projectiles, buildings, soundmanager, explosions);
		asteroidmanager.Update(timems, projectiles, buildings, soundmanager, explosions);
	}

	private void DoEasyNextRound()
	{
		round++;
		float num = round / 5;
		if (num < 0.5f)
		{
			num = 0.5f;
		}
		if (num > 5f)
		{
			num = 5f;
		}
		float num2 = num - 2f;
		if (num2 < 0.5f)
		{
			num2 = 0.5f;
		}
		int num3 = round * 5;
		if (num3 < 30)
		{
			num3 = 30;
		}
		if (num3 > 70)
		{
			num3 = 70;
		}
		num3 = (int)((float)num3 * playerscountmod);
		int spawnrate = 30000 / num3;
		missilemanager.Setup(num3, num, num2, spawnrate);
		num3 = (round - 3) * 2;
		if (num3 > 0)
		{
			if (num3 > 8)
			{
				num3 = 8;
			}
			num3 = (int)((float)num3 * playerscountmod);
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			saucermanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			saucermanager.Setup(0, 1f, 1f, 1);
		}
		num3 = (round - 7) * 2;
		if (num3 > 0)
		{
			if (num3 > 8)
			{
				num3 = 8;
			}
			num3 = (int)((float)num3 * playerscountmod);
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			asteroidmanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			asteroidmanager.Setup(0, 1f, 1f, 1);
		}
	}

	private void DoMediumNextRound()
	{
		round++;
		float num = round / 5;
		if (num < 1f)
		{
			num = 1f;
		}
		if (num > 6f)
		{
			num = 6f;
		}
		float num2 = num - 2f;
		if (num2 < 0.8f)
		{
			num2 = 0.8f;
		}
		int num3 = round * 5;
		if (num3 < 35)
		{
			num3 = 35;
		}
		if (num3 > 70)
		{
			num3 = 70;
		}
		int spawnrate = 30000 / num3;
		missilemanager.Setup(num3, num, num2, spawnrate);
		num3 = (round - 2) * 2;
		if (num3 > 0)
		{
			if (num3 > 8)
			{
				num3 = 8;
			}
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			saucermanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			saucermanager.Setup(0, 1f, 1f, 1);
		}
		num3 = (round - 5) * 2;
		if (num3 > 0)
		{
			if (num3 > 8)
			{
				num3 = 8;
			}
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			asteroidmanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			asteroidmanager.Setup(0, 1f, 1f, 1);
		}
	}

	private void DoHardNextRound()
	{
		round++;
		float num = round / 4;
		if (num < 1f)
		{
			num = 1f;
		}
		if (num > 6f)
		{
			num = 6f;
		}
		float num2 = num - 2f;
		if (num2 < 1f)
		{
			num2 = 1f;
		}
		int num3 = round * 8;
		if (num3 < 40)
		{
			num3 = 40;
		}
		if (num3 > 80)
		{
			num3 = 80;
		}
		int spawnrate = 30000 / num3;
		missilemanager.Setup(num3, num, num2, spawnrate);
		num3 = (round - 1) * 3;
		if (num3 > 0)
		{
			if (num3 > 8)
			{
				num3 = 8;
			}
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			saucermanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			saucermanager.Setup(0, 1f, 1f, 1);
		}
		num3 = (round - 4) * 3;
		if (num3 > 0)
		{
			if (num3 > 12)
			{
				num3 = 12;
			}
			num = round / 3;
			num2 = num / 2f;
			spawnrate = 30000 / num3;
			asteroidmanager.Setup(num3, num, num2, spawnrate);
		}
		else
		{
			asteroidmanager.Setup(0, 1f, 1f, 1);
		}
	}

	public void NextRound()
	{
		switch (difficultyrating)
		{
		case 0:
			DoEasyNextRound();
			break;
		case 1:
			DoMediumNextRound();
			break;
		case 2:
			DoHardNextRound();
			break;
		}
	}

	public void Reset()
	{
		round = 0;
	}

	public bool RoundComplete()
	{
		if (!missilemanager.Completed)
		{
			return false;
		}
		if (!saucermanager.Completed)
		{
			return false;
		}
		if (!asteroidmanager.Completed)
		{
			return false;
		}
		return true;
	}
}
