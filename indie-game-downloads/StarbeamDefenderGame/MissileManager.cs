using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class MissileManager
{
	private EnemyMissile[] enemymissiles;

	private PlayerMissile[] playermissile;

	private SmokeCloud[] smokeclouds;

	private AudioManager audiomanager;

	public SmokeCloud[] SmokeClouds => smokeclouds;

	public EnemyMissile[] EnemyMissiles => enemymissiles;

	public PlayerMissile[] PlayerMissiles => playermissile;

	public MissileManager(AudioManager audiomanager)
	{
		enemymissiles = new EnemyMissile[50];
		playermissile = new PlayerMissile[50];
		smokeclouds = new SmokeCloud[75];
		for (int i = 0; i < 50; i++)
		{
			enemymissiles[i] = new EnemyMissile();
			playermissile[i] = new PlayerMissile();
		}
		for (int j = 0; j < 75; j++)
		{
			smokeclouds[j] = new SmokeCloud();
		}
		this.audiomanager = audiomanager;
	}

	public int Update(int timems, ExplosionManager explosionsmanager, Turret[] players)
	{
		int result = 0;
		for (int i = 0; i < enemymissiles.Count(); i++)
		{
			enemymissiles[i].Update(timems, explosionsmanager, players, audiomanager, smokeclouds);
		}
		for (int j = 0; j < playermissile.Count(); j++)
		{
			playermissile[j].Update(timems, explosionsmanager, smokeclouds);
		}
		for (int k = 0; k < smokeclouds.Count(); k++)
		{
			smokeclouds[k].Update(timems);
		}
		return result;
	}

	public void FirePlayerMissile(Vector2 launchfrom, Vector2 destination, float size, float speed, Color trailcolor, PlayerIndex player)
	{
		for (int i = 0; i < playermissile.Count(); i++)
		{
			if (!playermissile[i].Active)
			{
				playermissile[i].Create(launchfrom, destination, size, speed, player, trailcolor);
				audiomanager.PlayMissileFireSound();
				break;
			}
		}
	}

	public void FireEnemyMissile(Vector2 launchfrom, Vector2 destination, int size, float speed, Building target)
	{
		for (int i = 0; i < enemymissiles.Count(); i++)
		{
			if (!enemymissiles[i].Active)
			{
				enemymissiles[i].Create(launchfrom, destination, size, Color.Red, speed, target);
				break;
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < enemymissiles.Count(); i++)
		{
			enemymissiles[i].Active = false;
		}
		for (int i = 0; i < enemymissiles.Count(); i++)
		{
			playermissile[i].Active = false;
		}
		for (int i = 0; i < smokeclouds.Count(); i++)
		{
			smokeclouds[i].Active = false;
		}
	}

	public int ActiveEnemyMissiles()
	{
		int num = 0;
		for (int i = 0; i < enemymissiles.Count(); i++)
		{
			if (enemymissiles[i].Active)
			{
				num++;
			}
		}
		return num;
	}
}
