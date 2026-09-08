using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class ExplosionManager
{
	private Explosion[] explosions;

	private AudioManager audiomanager;

	private GraphicExplosion[] graphicalexplosions;

	public Explosion[] Explosions => explosions;

	public GraphicExplosion[] GraphicExplosions => graphicalexplosions;

	public ExplosionManager(AudioManager audiomanager)
	{
		explosions = new Explosion[100];
		graphicalexplosions = new GraphicExplosion[25];
		for (int i = 0; i < explosions.Count(); i++)
		{
			explosions[i] = new Explosion();
		}
		for (int j = 0; j < graphicalexplosions.Count(); j++)
		{
			graphicalexplosions[j] = new GraphicExplosion();
		}
		this.audiomanager = audiomanager;
	}

	public void Reset()
	{
		for (int i = 0; i < explosions.Count(); i++)
		{
			explosions[i].Active = false;
		}
		for (int j = 0; j < graphicalexplosions.Count(); j++)
		{
			graphicalexplosions[j].Active = false;
		}
	}

	public void CreateExplosion(Vector2 position, int maxsize, int speedms, int score, PlayerIndex owner)
	{
		for (int i = 0; i < explosions.Count(); i++)
		{
			if (!explosions[i].Active)
			{
				explosions[i].Create(position, maxsize, speedms, score, owner);
				audiomanager.PlayExplosionSound();
				break;
			}
		}
	}

	public void CreateGraphicalExplosion(Vector2 position, Vector2 size)
	{
		for (int i = 0; i < graphicalexplosions.Count(); i++)
		{
			if (!graphicalexplosions[i].Active)
			{
				graphicalexplosions[i].Create(position, size);
				audiomanager.PlayExplosionSound();
				break;
			}
		}
	}

	public void Update(int timems)
	{
		for (int i = 0; i < explosions.Count(); i++)
		{
			if (explosions[i].Active)
			{
				explosions[i].Update(timems);
			}
		}
		for (int j = 0; j < graphicalexplosions.Count(); j++)
		{
			graphicalexplosions[j].Update(timems);
		}
	}
}
