using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class Projectile
{
	protected Vector2 position;

	protected Vector2 movevec;

	protected int damage;

	protected float speed;

	protected bool active;

	protected Color color;

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

	public Color Color => color;

	public Projectile()
	{
		position = Vector2.Zero;
		movevec = Vector2.Zero;
	}

	public virtual void Update(int timems, ExplosionManager explosions, AudioManager soundeffects)
	{
		position += movevec * speed;
	}

	public virtual void Update(int timems, ProjectileManager projectiles, Building[] buildings)
	{
	}
}
