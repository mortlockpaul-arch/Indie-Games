using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame.Game.AI;

public class Asteroid : SaucerBase
{
	private Vector2 destination;

	private float speed;

	private Building target;

	private int angle;

	private int spinspeed;

	private int spriteindex;

	public int SpriteIndex => spriteindex;

	public int Angle => angle;

	public Vector2 Size => new Vector2(1.28f * (float)life, 1.28f * (float)life);

	public Asteroid()
	{
		position = Vector2.Zero;
		destination = Vector2.Zero;
		active = false;
		life = 0;
		score = 0;
		speed = 0f;
	}

	public void Create(int speed, Building[] buildings)
	{
		Random random = new Random();
		bool flag = false;
		for (int i = 0; i < buildings.Count(); i++)
		{
			if (buildings[i].Shields > 0f)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			int num = random.Next(buildings.Count());
			while (buildings[num].Shields < 1f)
			{
				num = random.Next(buildings.Count());
			}
			int num2 = random.Next(1080);
			target = buildings[num];
			position = new Vector2(num2 - 32, -64f);
			destination = buildings[num].Position + buildings[num].Size / 2f;
			this.speed = speed;
			life = 100;
			score = 100;
			spriteindex = random.Next(5);
			spinspeed = random.Next(6) - 3;
			angle = random.Next(360);
			active = true;
		}
	}

	public override void Update(int timems, ProjectileManager projectilemanager, Building[] buildings, AudioManager soundmanager, ExplosionManager explosions)
	{
		if (!active)
		{
			return;
		}
		base.Update(timems, projectilemanager, buildings, soundmanager, explosions);
		Vector2 vector = Vector2.Normalize(destination - position);
		if (destination.Y - position.Y < 1f)
		{
			explosions.CreateGraphicalExplosion(destination, new Vector2(Size.X, Size.Y));
			active = false;
		}
		else
		{
			position += vector * speed;
			angle += spinspeed;
			if (angle > 360)
			{
				angle -= 360;
			}
			if (angle < 0)
			{
				angle += 360;
			}
		}
		if (!target.CheckCollision(position + new Vector2(Size.X / 2f, Size.Y)))
		{
			return;
		}
		if (target.Shields > 0f)
		{
			target.Shields -= 50f;
			if (target.Shields < 0f)
			{
				target.Shields = 0f;
				explosions.CreateGraphicalExplosion(target.Position, new Vector2(target.Size.X, target.Size.X));
			}
			else
			{
				explosions.CreateGraphicalExplosion(position, Size);
			}
			active = false;
		}
		else
		{
			active = false;
		}
	}
}
