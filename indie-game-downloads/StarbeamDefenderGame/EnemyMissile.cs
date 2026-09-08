using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class EnemyMissile : Missile
{
	private Building target;

	public void Create(Vector2 position, Vector2 destination, int size, Color trailcolor, float speed, Building target)
	{
		base.destination = destination;
		base.position = position;
		base.size = size;
		active = true;
		base.trailcolor = trailcolor;
		startpos = position;
		movevector = base.destination - base.position;
		movevector.Normalize();
		movevector *= speed;
		this.target = target;
		frame = 0;
		frametime = 250;
		double num = 180.0 / Math.PI * Math.Atan2(0f - (position.X - destination.X), base.Position.Y - destination.Y);
		angle = num;
		if (angle < 0.0)
		{
			angle += 360.0;
		}
		if (angle > 360.0)
		{
			angle -= 360.0;
		}
	}

	public override void Update(int timems, ExplosionManager explosionsmanager, Turret[] players, AudioManager soundeffects, SmokeCloud[] smokeclouds)
	{
		frametime -= timems;
		if (frametime < 0)
		{
			frametime = 100;
			frame++;
			if (frame > 3)
			{
				frame = 0;
			}
		}
		if (active)
		{
			position += movevector;
			if (target.Shields > 0f)
			{
				if (target.CheckCollision(position))
				{
					active = false;
					target.Shields -= 25f;
					if (target.Shields < 0f)
					{
						explosionsmanager.CreateGraphicalExplosion(target.Position, new Vector2(target.Size.X, target.Size.X));
						target.Shields = 0f;
					}
					else
					{
						soundeffects.PlayShieldHitSound();
						explosionsmanager.CreateGraphicalExplosion(position, new Vector2(20f, 20f));
					}
					for (int i = 0; i < smokeclouds.Count(); i++)
					{
						if (!smokeclouds[i].Active)
						{
							smokeclouds[i].Create(position, 1026f, angle, frame);
							break;
						}
					}
				}
			}
			else if (position.Y > destination.Y)
			{
				explosionsmanager.CreateGraphicalExplosion(position, new Vector2(20f, 20f));
				active = false;
				for (int j = 0; j < smokeclouds.Count(); j++)
				{
					if (!smokeclouds[j].Active)
					{
						smokeclouds[j].Create(position, 1026f, angle, frame);
						break;
					}
				}
			}
		}
		if (!active)
		{
			return;
		}
		for (int k = 0; k < explosionsmanager.Explosions.Count(); k++)
		{
			if (!explosionsmanager.Explosions[k].Active || !explosionsmanager.Explosions[k].Collision(position))
			{
				continue;
			}
			active = false;
			explosionsmanager.CreateExplosion(position, (int)size, 20, explosionsmanager.Explosions[k].Score * 2, explosionsmanager.Explosions[k].PlayerToScore);
			for (int l = 0; l < players.Count(); l++)
			{
				if (players[l].ControllerIndex == explosionsmanager.Explosions[k].PlayerToScore)
				{
					players[l].Score += explosionsmanager.Explosions[k].Score;
					break;
				}
			}
			for (int m = 0; m < smokeclouds.Count(); m++)
			{
				if (!smokeclouds[m].Active)
				{
					smokeclouds[m].Create(position, 1026f, angle, frame);
					break;
				}
			}
			break;
		}
	}
}
