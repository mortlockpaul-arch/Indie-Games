using System;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class PlayerMissile : Missile
{
	public PlayerIndex PlayerLaunched;

	private Vector2 launchpos;

	public int LaunchLength => (int)(position - launchpos).Length();

	public PlayerMissile()
	{
		movevector = Vector2.Zero;
		position = Vector2.Zero;
		size = 0f;
		destination = Vector2.Zero;
		PlayerLaunched = PlayerIndex.One;
		trailcolor = Color.Black;
	}

	public void Create(Vector2 position, Vector2 destination, float size, float speed, PlayerIndex player, Color trailcolor)
	{
		launchpos = position;
		base.destination = destination;
		base.position = position;
		base.Active = true;
		startpos = position;
		base.size = size;
		base.trailcolor = trailcolor;
		playerowner = player;
		frametime = 250;
		frame = 0;
		movevector = base.destination - base.position;
		movevector.Normalize();
		movevector *= speed;
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

	public override void Update(int timems, ExplosionManager explosionsmanager, SmokeCloud[] smokeclouds)
	{
		base.Update(timems, explosionsmanager, smokeclouds, LaunchLength);
	}
}
