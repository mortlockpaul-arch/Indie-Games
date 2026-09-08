using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class EnemyBlasterProjectile : Projectile
{
	private Building target;

	private Vector2 destination;

	public EnemyBlasterProjectile()
	{
		position = Vector2.Zero;
		speed = 0f;
		movevec = Vector2.Zero;
		destination = Vector2.Zero;
		active = false;
	}

	public void Create(Vector2 pos, Building target, float speed, int damage)
	{
		destination.X = target.Position.X + target.Size.X / 2f;
		destination.Y = target.Position.Y + target.Size.Y / 2f;
		position = pos;
		base.damage = damage;
		base.speed = speed;
		active = true;
		this.target = target;
		movevec = destination - pos;
		movevec.Normalize();
	}

	public override void Update(int timems, ExplosionManager explosions, AudioManager soundeffects)
	{
		if (!active)
		{
			return;
		}
		_ = position - destination;
		if (target.CheckCollision(position))
		{
			position = destination;
			target.Shields -= damage;
			if (target.Shields < 0f)
			{
				target.Shields = 0f;
				explosions.CreateGraphicalExplosion(target.Position, new Vector2(target.Size.X, target.Size.X));
			}
			active = false;
		}
		else
		{
			position += movevec * speed;
		}
	}
}
