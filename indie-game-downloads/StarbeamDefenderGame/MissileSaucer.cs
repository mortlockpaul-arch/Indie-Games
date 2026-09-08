using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class MissileSaucer : SaucerBase
{
	private int firerate;

	private int nextfire;

	private float speed;

	private Vector2 destination;

	private Vector2 movevector;

	private Random randomiser;

	private int bobbing;

	public MissileSaucer()
	{
		active = false;
		firerate = 0;
		nextfire = 0;
		position = new Vector2(0f, 0f);
		score = 0;
		speed = 0f;
		destination = new Vector2(0f, 0f);
		bobbing = 0;
		randomiser = new Random();
	}

	public void Create(Vector2 position, Vector2 destination, float speed, int score)
	{
		active = true;
		base.position = position;
		this.destination = destination;
		bobbing = 0;
		this.speed = speed;
		firerate = 2000;
		nextfire = firerate;
		base.score = score;
		movevector = this.destination - base.position;
		movevector.Normalize();
	}

	public override void Update(int timems, ProjectileManager projectiles, Building[] buildings, AudioManager soundmanager)
	{
		if (!active)
		{
			return;
		}
		if ((position - destination).Length() < movevector.Length() * speed)
		{
			position = destination;
			NextBobbingMovement();
		}
		else
		{
			position += movevector * speed;
		}
		if (bobbing != 0)
		{
			nextfire -= timems;
			if (nextfire < 0)
			{
				nextfire += firerate;
				Fire(projectiles, buildings);
			}
		}
	}

	private void Fire(ProjectileManager projectiles, Building[] buildings)
	{
		int num = randomiser.Next(buildings.Count());
		while (buildings[num].Shields < 1f)
		{
			num = randomiser.Next(buildings.Count());
		}
		projectiles.FireEnemyProjectile(position + new Vector2(32f, 32f), buildings[num]);
	}

	private void NextBobbingMovement()
	{
		if (bobbing == 1)
		{
			destination.Y = position.Y + 20f;
			movevector = new Vector2(0f, 1f);
			speed = 0.25f;
			bobbing = 2;
		}
		else
		{
			destination.Y = position.Y - 20f;
			movevector = new Vector2(0f, -1f);
			speed = 0.25f;
			bobbing = 1;
		}
	}
}
