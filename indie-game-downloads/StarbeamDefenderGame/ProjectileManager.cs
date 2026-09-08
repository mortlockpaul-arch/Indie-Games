using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class ProjectileManager
{
	private EnemyBlasterProjectile[] enemyprojectiles;

	private PlayerBlaster[] playerprojectiles;

	private AudioManager soundeffectmanager;

	public EnemyBlasterProjectile[] EnemyProjectiles => enemyprojectiles;

	public PlayerBlaster[] PlayerProjectiles => playerprojectiles;

	public ProjectileManager(AudioManager soundeffectmanager)
	{
		enemyprojectiles = new EnemyBlasterProjectile[25];
		playerprojectiles = new PlayerBlaster[25];
		this.soundeffectmanager = soundeffectmanager;
		for (int i = 0; i < enemyprojectiles.Count(); i++)
		{
			enemyprojectiles[i] = new EnemyBlasterProjectile();
			playerprojectiles[i] = new PlayerBlaster();
		}
	}

	public void Reset()
	{
		for (int i = 0; i < playerprojectiles.Count(); i++)
		{
			playerprojectiles[i].Active = false;
		}
		for (int j = 0; j < enemyprojectiles.Count(); j++)
		{
			enemyprojectiles[j].Active = false;
		}
	}

	public void Update(int timems, ExplosionManager explosions)
	{
		for (int i = 0; i < enemyprojectiles.Count(); i++)
		{
			enemyprojectiles[i].Update(timems, explosions, soundeffectmanager);
			playerprojectiles[i].Update(timems, explosions, soundeffectmanager);
		}
	}

	public void FireEnemyProjectile(Vector2 pos, Building target)
	{
		for (int i = 0; i < enemyprojectiles.Count(); i++)
		{
			if (!enemyprojectiles[i].Active)
			{
				enemyprojectiles[i].Create(pos, target, 5f, 25);
				soundeffectmanager.PlayEnemyBlasterSound();
				break;
			}
		}
	}

	public void FirePlayerProjectile(Vector2 pos, Vector2 AimPoint, float speed, Turret fireingplayer)
	{
		for (int i = 0; i < playerprojectiles.Count(); i++)
		{
			if (!playerprojectiles[i].Active)
			{
				playerprojectiles[i].Create(pos, AimPoint, speed, fireingplayer, fireingplayer.PlayerColor);
				soundeffectmanager.PlayPlayerBlasterSound();
				break;
			}
		}
	}
}
