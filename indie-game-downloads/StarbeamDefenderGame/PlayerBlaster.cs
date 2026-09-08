using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class PlayerBlaster : Projectile
{
	private Turret owningplayer;

	public Turret Player
	{
		get
		{
			return owningplayer;
		}
		set
		{
			owningplayer = Player;
		}
	}

	public void Create(Vector2 pos, Vector2 cursorposition, float speed, Turret player, Color color)
	{
		movevec = cursorposition - pos;
		movevec.Normalize();
		position = pos;
		base.speed = speed;
		active = true;
		owningplayer = player;
		base.color = color;
	}

	public override void Update(int timems, ExplosionManager explosions, AudioManager soundeffects)
	{
		if (active)
		{
			base.Update(timems, explosions, soundeffects);
			if (position.X < 0f)
			{
				active = false;
			}
			if (position.X > 1280f)
			{
				active = false;
			}
			if (position.Y < 0f)
			{
				active = false;
			}
			if (position.Y > 700f)
			{
				active = false;
			}
		}
	}
}
