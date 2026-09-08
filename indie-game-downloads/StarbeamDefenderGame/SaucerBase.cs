using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class SaucerBase
{
	protected Vector2 position;

	protected int life;

	protected bool active;

	protected int score;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public Vector2 Position => position;

	public int Score => score;

	public int Life
	{
		get
		{
			return life;
		}
		set
		{
			life = value;
		}
	}

	public virtual void Update(int timems, ProjectileManager projectilemanager, Building[] buildings, AudioManager soundmanager)
	{
	}

	public virtual void Update(int timems, ProjectileManager projectilemanager, Building[] buildings, AudioManager soundmanager, ExplosionManager explosions)
	{
	}
}
