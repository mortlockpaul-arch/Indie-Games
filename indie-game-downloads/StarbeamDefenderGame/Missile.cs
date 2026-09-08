using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class Missile
{
	protected Vector2 startpos;

	protected Vector2 destination;

	protected Vector2 position;

	protected float size;

	protected Color trailcolor;

	protected Vector2 movevector;

	protected PlayerIndex playerowner;

	protected double angle;

	protected int frame;

	protected int frametime;

	protected bool active;

	public int Frame => frame;

	public int AngleOfTravel => (int)angle;

	public PlayerIndex Owner => playerowner;

	public Vector2 StartPosition => startpos;

	public Color TrailColor => trailcolor;

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

	public Missile()
	{
		destination = Vector2.Zero;
		position = Vector2.Zero;
		size = 0f;
		active = false;
		trailcolor = Color.Black;
		movevector = Vector2.Zero;
	}

	public void Create(Vector2 position, Vector2 destination, int size, Color trailcolor, float speed)
	{
		this.destination = destination;
		this.position = position;
		this.size = size;
		active = true;
		this.trailcolor = trailcolor;
		startpos = position;
		movevector = this.destination - this.position;
		movevector.Normalize();
		movevector *= speed;
		frame = 0;
		frametime = 250;
		Vector2 value = position;
		Vector2 value2 = destination;
		value.Normalize();
		value2.Normalize();
		angle = Math.Acos(Vector2.Dot(value, value2));
	}

	public virtual void Update(int timems, ExplosionManager explosionsmanager, Turret[] players, AudioManager soundeffects, SmokeCloud[] smokeclouds)
	{
	}

	public virtual void Update(int timems, ExplosionManager explosionsmanager, SmokeCloud[] smokeclouds)
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
		if (!active)
		{
			return;
		}
		position += movevector;
		if (movevector.Y > 0f)
		{
			if (position.Y > destination.Y)
			{
				active = false;
				explosionsmanager.CreateExplosion(position, (int)size, 20, 0, playerowner);
			}
		}
		else
		{
			if (!(movevector.Y < 0f) || !(position.Y < destination.Y))
			{
				return;
			}
			active = false;
			explosionsmanager.CreateExplosion(position, (int)size, 20, 50, Owner);
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

	public virtual void Update(int timems, ExplosionManager explosionsmanager, SmokeCloud[] smokeclouds, int smokelength)
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
		if (!active)
		{
			return;
		}
		position += movevector;
		if (movevector.Y > 0f)
		{
			if (position.Y > destination.Y)
			{
				position = destination;
				active = false;
				explosionsmanager.CreateExplosion(position, (int)size, 20, 0, playerowner);
			}
		}
		else
		{
			if (!(movevector.Y < 0f) || !(position.Y < destination.Y))
			{
				return;
			}
			active = false;
			position = destination;
			explosionsmanager.CreateExplosion(position, (int)size, 20, 50, Owner);
			for (int i = 0; i < smokeclouds.Count(); i++)
			{
				if (!smokeclouds[i].Active)
				{
					smokeclouds[i].Create(position, smokelength, angle, frame);
					break;
				}
			}
		}
	}
}
